using System;
using System.Collections.Generic;
using System.Text;
using DevicesBase;
using DevicesBase.Helpers;
using DevicesCommon;
using DevicesCommon.Helpers;
using System.Drawing.Imaging;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace EpsonPrn
{
    [PrintableDevice(DeviceNames.printerTypeGenericEpson)]
    [Serializable]
    public class PrinterDevice : CustomPrintableDevice
    {
        #region Константы

        private const int DEF_CODEPAGE_INDEX = 0;
        private const int MAX_CMD_LEN = 1024;
        private const int READ_TIMEOUT = 1000;
        private const int WRITE_TIMEOUT = 1000;

        /// <summary>
        /// Ширина ленты в символах
        /// </summary>
        private const int TAPE_WIDTH_CHAR = 42;

        /// <summary>
        /// Ширина ленты в пикселях
        /// </summary>
        private const int TAPE_WIDTH_PX = 512;

        #endregion

        #region Переменные

        private bool bOpenedDoc = false;

        private PrintableDeviceInfo printerInfo = new PrintableDeviceInfo(new PrintableTapeWidth(TAPE_WIDTH_CHAR, 0), true);

        #endregion

        protected virtual int FontNo
        {
            get { return 0; }
        }

        protected virtual int CodePageIndex
        {
            get { return DEF_CODEPAGE_INDEX; }
        }

        #region Конструктор

        public PrinterDevice()
            : base()
        {
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

                // инвертируем цвета
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
                ErrorCode = new ServerErrorCode(this, E);
            }
            finally
            {
            }
        }

        private bool WriteBuffer(byte[] nCmd, int nLen)
        {
            // добавляем в буфер команду проверки статуса
            nCmd[nLen++] = 0x10;
            nCmd[nLen++] = 0x04;
            nCmd[nLen++] = 3;

            byte nRsp = 0;
            if (WriteBuffer(nCmd, nLen, out nRsp))
            {
                if ((nRsp & 0x04) == 0x04)  // mechanical error
                    ErrorCode = new ServerErrorCode(this, 0x100, GetSpecificDescription(0x100));
                // ошибка отрезчика игнорируется
                //                else if ((nRsp & 0x08) == 0x08)  // auto-cutter error
                //                    errorCode = new ErrorCode(this, 0x100, GetSpecificDescription(0x101));
                else if ((nRsp & 0x20) == 0x20)  // unrecoverable error
                    ErrorCode = new ServerErrorCode(this, 0x100, GetSpecificDescription(0x102));
                else if ((nRsp & 0x40) == 0x40)  // auto-recoverable error
                    ErrorCode = new ServerErrorCode(this, 0x100, GetSpecificDescription(0x103));
                else
                    ErrorCode = new ServerErrorCode(this, GeneralError.Success);
            }
            else
                ErrorCode = new ServerErrorCode(this, GeneralError.Timeout);

            WriteBuffer(new byte[] { 0x10, 0x04, 2 }, 3, out nRsp);

            return ErrorCode.Succeeded;
        }

        private bool WriteBuffer(byte[] nCmd, int nLen, out byte nRsp)
        {
            Port.DiscardBuffers();

            nRsp = 0;
            try
            {
                int bytesWritten = 0;
                int maxBufferLen = 256;
                while (bytesWritten + maxBufferLen < nLen)
                    bytesWritten += Port.Write(nCmd, bytesWritten, maxBufferLen);
                Port.Write(nCmd, bytesWritten, nLen - bytesWritten);
                nRsp = (byte)Port.ReadByte();
            }
            catch (TimeoutException)
            {
                return false;
            }
            return true;
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
                    nCmd[nLen++] = (byte)(0x00 | FontNo);
                    break;
                case FontStyle.DoubleHeight:
                    nCmd[nLen++] = (byte)(0x10 | FontNo);
                    break;
                case FontStyle.DoubleWidth:
                    nCmd[nLen++] = (byte)(0x20 | FontNo);
                    break;
                case FontStyle.DoubleAll:
                    nCmd[nLen++] = (byte)(0x30 | FontNo);
                    break;
            }

            WriteBuffer(nCmd, nLen);
        }

        #endregion

        #region Реализация интерфейса IPrintableDevice

        public override event EventHandler<PrinterBreakEventArgs> PrinterBreak;

        public override PrintableDeviceInfo PrinterInfo
        {
            get { return printerInfo; }
        }

        public override PrinterStatusFlags PrinterStatus
        {
            get
            {
                byte nRsp = 0;
                PaperOutStatus poStatus = PaperOutStatus.Present;
                bool bDrawerOpened = false;
                bool bPrinting = false;

                ExecuteDriverCommand(delegate()
                {
                    if (WriteBuffer(new byte[] { 0x10, 0x04, 1 }, 3, out nRsp))
                    {
                        bDrawerOpened = !((nRsp & 0x04) == 0x04);
                    }

                    if (WriteBuffer(new byte[] { 0x10, 0x04, 4 }, 3, out nRsp))
                    {
                        if (((nRsp & 0x0C) == 0x0C) || ((nRsp & 0x60) == 0x60))
                            poStatus = PaperOutStatus.OutPassive;
                        else
                            poStatus = PaperOutStatus.Present;
                    }
                });
                return new PrinterStatusFlags(bPrinting, poStatus, bOpenedDoc, bDrawerOpened);
            }
        }

        #endregion

        #region Реализация виртуальных методов CustomSerialDevice

        protected override void OnAfterActivate()
        {
            base.OnAfterActivate();
            Port.ReadTimeout = READ_TIMEOUT;
            Port.WriteTimeout = WRITE_TIMEOUT;

            ExecuteDriverCommand(delegate()
            {
                // инициализация принтера
                byte[] nCmd = new byte[MAX_CMD_LEN];
                int nLen = 0;
                nCmd[nLen++] = 0x1B;
                nCmd[nLen++] = 0x40;
                WriteBuffer(nCmd, nLen);

                // установка кодовой страницы
                if (CodePageIndex != DEF_CODEPAGE_INDEX)
                {
                    nLen = 0;
                    nCmd[nLen++] = 0x1B;
                    nCmd[nLen++] = 0x74;
                    nCmd[nLen++] = (byte)CodePageIndex;
                    WriteBuffer(nCmd, nLen);
                }
            });
        }

        protected override void SetCommStateEventHandler(object sender, CommStateEventArgs e)
        {
            DCB dcb = e.DCB;

            dcb.XonChar = 0;
            dcb.XoffChar = 0;
            dcb.XonLim = 0;
            dcb.XoffLim = 0;

            // shake
            dcb.fTXContinueOnXoff = 0;
            dcb.fInX = 0;
            dcb.fOutX = 0;
            dcb.fRtsControl = (uint)RtsControl.Disable;
            dcb.fNull = 0;

            // replace
            dcb.fDtrControl = (uint)DtrControl.Enable;
            dcb.fAbortOnError = 0;
            dcb.fDsrSensitivity = 0;
            dcb.fOutxDsrFlow = 0;
            dcb.fOutxCtsFlow = 0;

            e.DCB = dcb;
            e.Handled = true;
            base.SetCommStateEventHandler(sender, e);
        }

        #endregion

        #region Реализация виртуальных методов CustomPrintableDevice

        protected override void OnOpenDocument(DocumentType docType,
            string cashierName)
        {
            ExecuteDriverCommand(delegate()
            {
                // установка кодовой страницы
                if (CodePageIndex != DEF_CODEPAGE_INDEX)
                {
                    byte[] nCmd = new byte[MAX_CMD_LEN];
                    int nLen = 0;
                    nCmd[nLen++] = 0x1B;
                    nCmd[nLen++] = 0x74;
                    nCmd[nLen++] = (byte)CodePageIndex;
                    WriteBuffer(nCmd, nLen);
                }

                if (bOpenedDoc)
                    OnCloseDocument(true);

                if (docType == DocumentType.Other)
                    bOpenedDoc = true;
            });
        }

        protected override void OnCloseDocument(bool cutPaper)
        {
            ExecuteDriverCommand(delegate()
            {
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

                    WriteBuffer(nCmd, nLen);

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
                SetFontStyle(style);

                // обрезаем строку до максимальной длины
                if (source.Length > PrinterInfo.TapeWidth.MainPrinter)
                    source = source.Substring(0, PrinterInfo.TapeWidth.MainPrinter);

                byte[] nCmd = new byte[MAX_CMD_LEN];
                int nLen = 0;

                Array.Copy(Encoding.GetEncoding(866).GetBytes(source), 0, nCmd, nLen, source.Length);
                nLen += source.Length;
                nCmd[nLen++] = 0x0D;    // CR
                nCmd[nLen++] = 0x0A;    // LF

                WriteBuffer(nCmd, nLen);

                SetFontStyle(FontStyle.Regular);
            });
        }

        protected override void OnPrintBarcode(string barcode, AlignOptions align,
            bool readable)
        {
            ExecuteDriverCommand(delegate()
            {
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
            ExecuteDriverCommand(delegate() { });
        }

        protected override void OnCash(uint amount)
        {
            ExecuteDriverCommand(delegate() { });
        }

        #endregion
    }

    [PrintableDevice("Samsung SRP-270 (Epson mode)")]
    [Serializable]
    public class PrinterDeviceSRP700 : PrinterDevice
    {
        protected override int FontNo
        {
            get { return 1; }
        }
    }

    [PrintableDevice(DeviceNames.printerTypeGenericEpson + " (Advanpos/RS)")]
    [Serializable]
    public class PrinterDeviceAdvanposRS : PrinterDevice
    {
        protected override int CodePageIndex
        {
            get { return 17; }
        }
    }
}
