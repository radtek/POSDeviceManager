using System;
using System.Collections.Generic;
using System.Text;
using DevicesBase;
using DevicesBase.Helpers;
using DevicesCommon;
using DevicesCommon.Helpers;
using System.Drawing.Imaging;

namespace EpsonPrn
{
    [PrintableDevice(DeviceNames.printerTypeGenericEpson + " (TCP)")]
    public class EpsonNetworkPrinter : CustomNetworkPrinter
    {
        #region Константы

        private const int DEF_TCP_PORT = 9100;
        private const int DEF_CODEPAGE_INDEX = 0;
        private const int MAX_CMD_LEN = 1024;

        /// <summary>
        /// Ширина ленты в символах
        /// </summary>
        private const int TAPE_WIDTH_CHAR = 42;

        /// <summary>
        /// Ширина ленты в пикселях
        /// </summary>
        private const int TAPE_WIDTH_PX = 512;

        private const string _errorWaitingEndOfPrint = 
            "Ошибка во время ожидания завершения печати документа.\n" +
            "Тип: {0}\nТекст: {1}\nСтек:\n\n{2}";

        #endregion

        #region Переменные

        private bool bOpenedDoc;
        private readonly PrintableDeviceInfo printerInfo;
        private readonly StringBuilder debugInfo;
        private readonly Encoding defaultEncoding;

        #endregion

        #region Свойства

        protected override int TcpPort
        {
            get { return DEF_TCP_PORT; }
        }

        protected virtual int CodePageIndex
        {
            get { return DEF_CODEPAGE_INDEX; }
        }

        #endregion

        #region Конструктор

        public EpsonNetworkPrinter()
            : base()
        {
            this.printerInfo = new PrintableDeviceInfo(new PrintableTapeWidth(TAPE_WIDTH_CHAR, 0), true);
            this.debugInfo = new StringBuilder();
            this.defaultEncoding = Encoding.GetEncoding(866);

            AddSpecificError(0x100, "Аппаратная ошибка");
            AddSpecificError(0x101, "Повреждение ножа");
            AddSpecificError(0x102, "Неисправимая ошибка");
            AddSpecificError(0x103, "Исправимая ошибка");
        }

        #endregion

        #region Внутренние методы

        private byte[] GetImageBytes(System.Drawing.Bitmap image, out int stride)
        {
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);

            BitmapData bmpData = image.LockBits(rect, ImageLockMode.ReadOnly, image.PixelFormat);
            byte[] imageBytes = new byte[image.Height * bmpData.Stride];
            stride = bmpData.Stride;
            try
            {
                System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, imageBytes, 0, bmpData.Height * stride);

                if (image.Palette.Entries[0].Name == "ff000000")
                {
                    for (int i = 0; i < imageBytes.Length; i++)
                        imageBytes[i] = (byte)(imageBytes[i] ^ 0xFF);
                }
                return imageBytes;
            }
            finally
            {
                image.UnlockBits(bmpData);
            }
        }

        private byte[] GetColorImageBytes(System.Drawing.Bitmap image, out int stride)
        {
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);
            System.Drawing.Bitmap newImage = new System.Drawing.Bitmap(image.Width, image.Height, PixelFormat.Format1bppIndexed);

            BitmapData bmpData = newImage.LockBits(rect, ImageLockMode.ReadOnly, newImage.PixelFormat);
            stride = bmpData.Stride;
            newImage.UnlockBits(bmpData);

            byte[] imageBytes = new byte[newImage.Height * stride];
            for (int y = 0; y < image.Height; y++)
            {
                for (int i = 0; i < stride; i++)
                {
                    byte b = 0;

                    for (int x = 0; x < 8; x++)
                    {
                        if (i * 8 + x >= image.Width)
                            break;
                        if ((ulong)image.GetPixel(i * 8 + x, y).ToArgb() != 0xffffffff)
                            b += (byte)(0x1 << (7 - x));
                    }
                    imageBytes[y * stride + i] = b;
                }
            }
            return imageBytes;
        }

        private void SetFontStyle(FontStyle style)
        {
            byte[] nCmd = new byte[MAX_CMD_LEN];
            int nLen = 0;

            nCmd[nLen++] = 0x1B;
            nCmd[nLen++] = Convert.ToByte('!');
            switch (style)
            {
                case FontStyle.Regular:
                    nCmd[nLen++] = 0x00;
                    break;
                case FontStyle.DoubleHeight:
                    nCmd[nLen++] = 0x10;
                    break;
                case FontStyle.DoubleWidth:
                    nCmd[nLen++] = 0x20;
                    break;
                case FontStyle.DoubleAll:
                    nCmd[nLen++] = 0x30;
                    break;
            }


            WriteBuffer(nCmd, nLen);
        }

        private delegate void ExecuteCommandDelegate();

        private void ExecuteDriverCommand(ExecuteCommandDelegate executeCommandDelegate)
        {
            ErrorCode = new ServerErrorCode(this, GeneralError.Success);
            try
            {
                if (!Active)
                {
                    ErrorCode = new ServerErrorCode(this, GeneralError.Inactive);
                    return;
                }

                executeCommandDelegate();
            }
            catch (TimeoutException)
            {
                ErrorCode = new ServerErrorCode(this, GeneralError.Timeout);
            }
            catch (Exception E)
            {
                debugInfo.AppendLine();
                debugInfo.AppendLine("Full exception info:");
                debugInfo.AppendLine();

                var currentException = E;
                var level = 0;
                do
                {
                    debugInfo.AppendFormat("[{0}]", level);
                    debugInfo.AppendLine();
                    debugInfo.AppendFormat("Exception: {0}", currentException.Message);
                    debugInfo.AppendFormat("Type: {0}", currentException.GetType());
                    debugInfo.AppendFormat("Stack trace: {0}", currentException.StackTrace);
                    debugInfo.AppendLine();

                    level++;
                    currentException = currentException.InnerException;
                }
                while (currentException != null);

                Logger.SaveDebugInfo(this, debugInfo.ToString());
                ClearDebugInfo();

                ErrorCode = new ServerErrorCode(this, E);
            }
            finally
            {
                if (!ErrorCode.Succeeded && Logger.DebugInfo && !string.IsNullOrEmpty(debugInfo.ToString()))
                    Logger.SaveDebugInfo(this, debugInfo.ToString());
                ClearDebugInfo();
            }
        }

        private bool WriteBuffer(byte[] nCmd, int nLen)
        {
            return WriteBuffer(nCmd, nLen, false);
        }

        private bool WriteBuffer(byte[] nCmd, int nLen, bool checkStatus)
        {
            byte nRsp = 0;
            if (WriteBuffer(nCmd, nLen, out nRsp, checkStatus))
            {
                if ((nRsp & 0x04) == 0x04)  // mechanical error
                    ErrorCode = new ServerErrorCode(this, 0x100, GetSpecificDescription(0x100));
                else if ((nRsp & 0x20) == 0x20)  // unrecoverable error
                    ErrorCode = new ServerErrorCode(this, 0x100, GetSpecificDescription(0x102));
                else if ((nRsp & 0x40) == 0x40)  // auto-recoverable error
                    ErrorCode = new ServerErrorCode(this, 0x100, GetSpecificDescription(0x103));
                else
                    ErrorCode = new ServerErrorCode(this, GeneralError.Success);
            }
            else
                ErrorCode = new ServerErrorCode(this, GeneralError.Timeout);

            return ErrorCode.Succeeded;
        }

        private bool WriteBuffer(byte[] nCmd, int nLen, out byte nRsp, bool checkStatus)
        {
            WriteDebugLine("WriteBuffer", nCmd, nLen);

            nRsp = 0;
            try
            {
                int bytesWritten = 0;
                int maxBufferLen = 256;
                while (bytesWritten + maxBufferLen < nLen)
                    bytesWritten += Communicator.Write(nCmd, bytesWritten, maxBufferLen);
                Communicator.Write(nCmd, bytesWritten, nLen - bytesWritten);

                if (checkStatus)
                    nRsp = CheckStatusByte(3);
            }
            catch (TimeoutException)
            {
                return false;
            }
            return true;
        }

        private byte CheckStatusByte(int mode)
        {
            return CheckStatusByte(Communicator, mode);
        }

        private byte CheckStatusByte(DevicesBase.Communicators.TcpCommunicator communicator, int mode)
        {
            int retriesCount = 0;
            do
            {
                try
                {
                    var buffer = new byte[256];
                    WriteDebugLine("Очистка исходящего буфера устройства");
                    communicator.Read(buffer, 0, 0);
                    WriteDebugLine("Запрос статусного байта");
                    communicator.Write(new byte[] { 0x10, 0x04, (byte)mode }, 0, 3);
                    
                    WriteDebugLine("Чтение статусного байта " + (retriesCount + 1));
                    byte nRsp = (byte)communicator.ReadByte();
                    WriteDebugLine("Статусный байт:", new byte[] { nRsp }, 1);

                    return nRsp;
                }
                catch (CommunicationException E)
                {
                    WriteDebugLine("CommunicationException" + E.ToString());
                    if (retriesCount++ >= 5)
                        throw;
                    System.Threading.Thread.Sleep(1000);
                }
            }
            while (true);
        }

        #endregion

        #region Реализация интерфейса IPrintableDevice

        public override event EventHandler<PrinterBreakEventArgs> PrinterBreak;

        public override PrintableDeviceInfo PrinterInfo
        {
            get { return printerInfo; }
        }

        #endregion

        #region Реализация виртуальных методов CustomPrintableDevice

        protected override PrinterStatusFlags OnQueryPrinterStatus(DevicesBase.Communicators.TcpCommunicator communicator)
        {
            var poStatus = PaperOutStatus.Present;
            bool bDrawerOpened = false;
            bool bPrinting = false;

            ExecuteDriverCommand(delegate()
            {
                WriteDebugLine("OnQueryPrinterStatus");

                // запрос состояния ДЯ
                byte nRsp = CheckStatusByte(communicator, 1);
                bDrawerOpened = !((nRsp & 0x04) == 0x04);

                // запрос состояния бумаги
                nRsp = CheckStatusByte(communicator, 4);
                

                if (((nRsp & 0x0C) == 0x0C) || ((nRsp & 0x60) == 0x60))
                    poStatus = PaperOutStatus.OutPassive;
                else
                    poStatus = PaperOutStatus.Present;
            });

            return new PrinterStatusFlags(bPrinting, poStatus, bOpenedDoc, bDrawerOpened);
        }

        protected override void OnOpenDocument(DocumentType docType,
            string cashierName)
        {
            ExecuteDriverCommand(delegate()
            {
                WriteDebugLine("OnOpenDocument");
                if (bOpenedDoc)
                    OnCloseDocument(true);

                // инициализация принтера
                byte[] nCmd = new byte[MAX_CMD_LEN];
                int nLen = 0;
                nCmd[nLen++] = 0x1B;
                nCmd[nLen++] = 0x40;
                WriteBuffer(nCmd, nLen, true);

                // установка кодовой страницы
                if (CodePageIndex != DEF_CODEPAGE_INDEX)
                {
                    nLen = 0;
                    nCmd[nLen++] = 0x1B;
                    nCmd[nLen++] = 0x74;
                    nCmd[nLen++] = (byte)CodePageIndex;
                    WriteBuffer(nCmd, nLen);
                }
                if (docType == DocumentType.Other)
                    bOpenedDoc = true;
            });
        }

        private void OnWaitException(Exception ex)
        {
            // записываем сообщение об ошибке в лог
            Logger.WriteEntry(this, string.Format(_errorWaitingEndOfPrint,
                ex.GetType(), ex.Message, ex.StackTrace),
                System.Diagnostics.EventLogEntryType.Error);

            System.Threading.Thread.Sleep(100);
        }

        protected override void OnCloseDocument(bool cutPaper)
        {
            ExecuteDriverCommand(delegate()
            {
                WriteDebugLine("OnCloseDocument");
                DrawGraphicFooter();
                DrawFooter();

                // отрезка
                if (cutPaper)
                {
                    byte[] nCmd = new byte[MAX_CMD_LEN];
                    int nLen = 0;

                    //                    nCmd[nLen++] = 0x0D;    // CR
                    nCmd[nLen++] = 0x0A;    // LF

                    nCmd[nLen++] = 0x1D;
                    nCmd[nLen++] = Convert.ToByte('V');
                    nCmd[nLen++] = 66;
                    nCmd[nLen++] = 0;

                    WriteBuffer(nCmd, nLen, true);

                    DrawHeader();
                    DrawGraphicHeader();
                }

                bOpenedDoc = false;
            });
        }

        protected override void OnOpenDrawer()
        {
            ExecuteDriverCommand(delegate()
            {
                WriteDebugLine("OnOpenDrawer");
                byte[] nCmd = new byte[MAX_CMD_LEN];
                int nLen = 0;

                nCmd[nLen++] = 0x1B;
                nCmd[nLen++] = Convert.ToByte('p');
                nCmd[nLen++] = 0;
                nCmd[nLen++] = 100;
                nCmd[nLen++] = 0xFF;
                WriteBuffer(nCmd, nLen);
            });
        }

        protected override void OnPrintString(string source, FontStyle style)
        {
            ExecuteDriverCommand(delegate()
            {
                WriteDebugLine("OnPrintString");
                WriteDebugLine(source);

                SetFontStyle(style);

                // обрезаем строку до максимальной длины
                if (source.Length > PrinterInfo.TapeWidth.MainPrinter)
                    source = source.Substring(0, PrinterInfo.TapeWidth.MainPrinter);

                // получаем буфер для печати в кодировке по умолчанию
                var buffer = defaultEncoding.GetBytes(string.Concat(source, Environment.NewLine));

                // печатаем буфер
                WriteBuffer(buffer, buffer.Length);

                // восстанавливаем стиль текста
                SetFontStyle(FontStyle.Regular);
            });
        }

        protected override void OnPrintBarcode(string barcode, AlignOptions align,
            bool readable)
        {
            ExecuteDriverCommand(delegate()
            {
                WriteDebugLine("OnPrintBarcode");
                WriteDebugLine(barcode);
                byte[] nCmd = new byte[MAX_CMD_LEN];
                int nLen = 0;

                // расположение
                nCmd[nLen++] = 0x1B;
                nCmd[nLen++] = Convert.ToByte('a');

                switch (align)
                {
                    case AlignOptions.Left:
                        nCmd[nLen++] = 0;
                        break;
                    case AlignOptions.Center:
                        nCmd[nLen++] = 1;
                        break;
                    case AlignOptions.Right:
                        nCmd[nLen++] = 2;
                        break;
                }

                // высота штрих-кода
                nCmd[nLen++] = 0x1D;
                nCmd[nLen++] = Convert.ToByte('h');
                nCmd[nLen++] = 70;

                // ширина кода
                nCmd[nLen++] = 0x1D;
                nCmd[nLen++] = Convert.ToByte('w');
                nCmd[nLen++] = 2;

                // цифры
                nCmd[nLen++] = 0x1D;
                nCmd[nLen++] = Convert.ToByte('H');
                if (readable)
                    nCmd[nLen++] = 2;
                else
                    nCmd[nLen++] = 0;

                // штрих-код
                nCmd[nLen++] = 0x1D;
                nCmd[nLen++] = Convert.ToByte('k');
                nCmd[nLen++] = 0x43;
                nCmd[nLen++] = 0x0C;

                // данные
                Array.Copy(Encoding.ASCII.GetBytes(barcode), 0, nCmd, nLen, barcode.Length);
                nLen += barcode.Length;

                // NUL
                nCmd[nLen++] = 0x00;

                // сброс расположения
                nCmd[nLen++] = 0x1B;
                nCmd[nLen++] = Convert.ToByte('a');
                nCmd[nLen++] = 0;

                WriteBuffer(nCmd, nLen);
            });
        }

        protected override void OnPrintImage(System.Drawing.Bitmap image, AlignOptions align)
        {
            ExecuteDriverCommand(delegate()
            {
                byte[] nCmd = new byte[MAX_CMD_LEN * 100];
                int nLen = 0;

                // для позиционирования картинки используем сдвиг отступа печати
                nCmd[nLen++] = 0x1D;
                nCmd[nLen++] = Convert.ToByte('L');

                if (align == AlignOptions.Left || TAPE_WIDTH_PX < image.Width)
                {
                    nCmd[nLen++] = 0;
                    nCmd[nLen++] = 0;
                }
                else
                    switch (align)
                    {
                        case AlignOptions.Center:
                            nCmd[nLen++] = (byte)((TAPE_WIDTH_PX - image.Width) / 2);
                            nCmd[nLen++] = 0;
                            break;
                        case AlignOptions.Right:
                            nCmd[nLen++] = (byte)((TAPE_WIDTH_PX - image.Width) % 256);
                            nCmd[nLen++] = (byte)((TAPE_WIDTH_PX - image.Width) / 256);
                            break;
                    }

                byte[] imageBytes = null;
                int stride = 0;

                if (image.PixelFormat == PixelFormat.Format1bppIndexed)
                    imageBytes = GetImageBytes(image, out stride);
                else
                    imageBytes = GetColorImageBytes(image, out stride);

                // печать картинки
                nCmd[nLen++] = 0x1D;
                nCmd[nLen++] = Convert.ToByte('v');
                nCmd[nLen++] = Convert.ToByte('0');

                nCmd[nLen++] = 0;
                nCmd[nLen++] = (byte)stride;
                nCmd[nLen++] = 0;
                nCmd[nLen++] = (byte)(image.Height % 256);
                nCmd[nLen++] = (byte)(image.Height / 256);

                Array.Copy(imageBytes, 0, nCmd, nLen, imageBytes.Length);
                nLen += imageBytes.Length;
                WriteBuffer(nCmd, nLen);

                System.Threading.Thread.Sleep(500);

                // возвращаем отступ в исходное положение
                nLen = 0;
                nCmd[nLen++] = 0x1D;
                nCmd[nLen++] = Convert.ToByte('L');
                nCmd[nLen++] = 0;
                nCmd[nLen++] = 0;

                // промотка строки
                nCmd[nLen++] = 0x0A;
                WriteBuffer(nCmd, nLen);
            });
        }

        protected override void OnRegistration(string commentary, uint quantity, uint amount,
            byte section)
        {
            OnPrintString(commentary, FontStyle.Regular);
        }

        protected override void OnPayment(uint amount, FiscalPaymentType paymentType)
        {
        }

        protected override void OnCash(uint amount)
        {
        }

        #endregion

        #region Реализация абстрактных методов

        protected override int ReadTimeout
        {
            get { return 5000; }
        }

        protected override int WriteTimeout
        {
            get { return 2000; }
        }

        #endregion

        #region Отладочный лог

        public void ClearDebugInfo()
        {
            debugInfo.Length = 0;
        }

        public void WriteDebugLine(string message)
        {
            debugInfo.AppendFormat("{0:HH:mm:ss}\t{1}\r\n", DateTime.Now, message);
        }

        public void WriteDebugLine(string message, byte[] nBuffer, int nBufferLen)
        {
            string[] bufDump = Array.ConvertAll(nBuffer, new Converter<byte, string>(delegate(byte b) { return b.ToString("X"); }));
            debugInfo.AppendFormat("{0:HH:mm:ss}\t{1}\r\n", DateTime.Now, message);
            if (nBufferLen > 0)
                debugInfo.AppendFormat("\t{0:X}\r\n", string.Join(" ", bufDump, 0, nBufferLen));
            else
                debugInfo.Append("\tнет\r\n");
        }

        #endregion
    }

    [PrintableDevice(DeviceNames.printerTypeGenericEpson + " (Advanpos/TCP)")]
    [Serializable]
    public class PrinterDeviceAdvanposTCP : EpsonNetworkPrinter
    {
        protected override int CodePageIndex
        {
            get { return 17; }
        }
    }
}
