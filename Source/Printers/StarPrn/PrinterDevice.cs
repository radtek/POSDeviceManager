using System;
using System.Collections.Generic;
using System.Text;
using DevicesBase;
using DevicesBase.Helpers;
using DevicesCommon;
using DevicesCommon.Helpers;
using System.Drawing.Imaging;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace StarPrn
{
    [PrintableDevice(DeviceNames.printerTypeStarSlipPrinter)]
    [Serializable]
    public class PrinterDevice : CustomPrintableDevice
    {
        #region Константы

        private const short ERROR_NO_SLIP = 0x200;
        private const int ERROR_OUT_OF_PAPER = 28;
        private const int ERROR_DEVICE_NOT_CONNECTED = 1167;
        private const int ERROR_BUSY = 170;

        private const int SLIP_TAPE_WIDTH = 42;

        private const int RECEIPT_TAPE_WIDTH = 48;

        private const int MAX_CMD_LEN = 10 * 1024;

        private const int MAX_RETRIES_COUNT = 5;

        private const int MAX_PAPER_OUT_RETRIES = 15;

        private const int WRITE_TIMEOUT = 2000;

        private const int READ_TIMEOUT = 5000;

        // следующие константы нужны для формирования штрих-кода
        private const int ODD = 0;
        private const int EVEN = 1;
        private const int RIGHT = 2;

        private string[][] EAN_CHARSET = new string[][] {
            new string[] {"0001101", "0100111", "1110010"},
            new string[] {"0011001", "0110011", "1100110"},
            new string[] {"0010011", "0011011", "1101100"}, 
            new string[] {"0111101", "0100001", "1000010"},
            new string[] {"0100011", "0011101", "1011100"},
            new string[] {"0110001", "0111001", "1001110"},
            new string[] {"0101111", "0000101", "1010000"},
            new string[] {"0111011", "0010001", "1000100"},
            new string[] {"0110111", "0001001", "1001000"},
            new string[] {"0001011", "0010111", "1110100"}};

        private int[][] EAN_PARITY = new int[][] {
            new int[] {ODD, ODD,  ODD,  ODD,  ODD,  ODD },
            new int[] {ODD, ODD,  EVEN, ODD,  EVEN, EVEN},
            new int[] {ODD, ODD,  EVEN, EVEN, ODD,  EVEN},
            new int[] {ODD, ODD,  EVEN, EVEN, EVEN, ODD },
            new int[] {ODD, EVEN, ODD,  ODD,  EVEN, EVEN},
            new int[] {ODD, EVEN, EVEN, ODD,  ODD,  EVEN},
            new int[] {ODD, EVEN, EVEN, EVEN, ODD,  ODD },
            new int[] {ODD, EVEN, ODD,  EVEN, ODD,  EVEN},
            new int[] {ODD, EVEN, ODD,  EVEN, EVEN, ODD },
            new int[] {ODD, EVEN, EVEN, ODD,  EVEN, ODD }
        };

        #endregion

        #region Скрытые поля

        private PrintableDeviceInfo printerInfo = new PrintableDeviceInfo(new PrintableTapeWidth(RECEIPT_TAPE_WIDTH, SLIP_TAPE_WIDTH), true);

        private byte[] currCmd = new byte[MAX_CMD_LEN];

        private int cmdLen = 0;

        private int maxSlipLines = 0;

        private int slipLineNo = 0;

        private bool docOpened = false;

//        private PrinterStatusFlags _printerStatus = new PrinterStatusFlags(false, PaperOutStatus.Present, false, false);

        #endregion

        #region Конструктор

        public PrinterDevice()
            : base()
        {
            AddSpecificError(0x100, "Vertical parity error");
            AddSpecificError(0x101, "Framing error");
            AddSpecificError(0x102, "Printer mechanical error");

            AddSpecificError(ERROR_NO_SLIP, "Отсутствует бланк подкладного документа");
        }

        #endregion

        #region Скрытые методы

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

        private void ClearBuffer()
        {
            Array.Clear(currCmd, 0, cmdLen);
            cmdLen = 0;
        }

        private void WriteBuffer(byte b)
        {
            currCmd[cmdLen++] = b;
        }

        private void WriteBuffer(params byte[] bytes)
        {
            Array.Copy(bytes, 0, currCmd, cmdLen, bytes.Length);
            cmdLen += bytes.Length;
        }

        private void WriteBuffer(byte[] bytes, int firstByte, int bytesCount)
        {
            Array.Copy(bytes, firstByte, currCmd, cmdLen, bytesCount);
            cmdLen += bytesCount;
        }

        private void WriteBuffer(string strValue)
        {
            WriteBuffer(Encoding.GetEncoding(866).GetBytes(strValue));
        }

        private void FlushBuffer()
        {
            FlushBuffer(true, false);
        }

        private void FlushBuffer(bool throwPaperOut, bool throwOnBusy)
        {
            try
            {
                if (IsSerial)
                {
                    // добавляем в буфер команду проверки статуса
                    currCmd[cmdLen++] = 0x05;
                    WriteToSerial();
                }
                else
                    WriteToParallel(throwPaperOut, throwOnBusy);
            }
            catch (TimeoutException)
            {
                ErrorCode = new ServerErrorCode(this, GeneralError.Timeout);
            }
            catch (Exception E)
            {
                ErrorCode = new ServerErrorCode(this, E);
                // очистка буфера принтера
                try
                {
                    Port.WriteByte(0x18);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Запись в последовательный порт
        /// </summary>
        private void WriteToSerial()
        {
            Port.DiscardBuffers();
            byte nRsp;
            int bytesWritten = 0;
            int maxBufferLen = 256;
            while (bytesWritten + maxBufferLen < cmdLen)
                bytesWritten += Port.Write(currCmd, bytesWritten, maxBufferLen);
            Port.Write(currCmd, bytesWritten, cmdLen - bytesWritten);

            nRsp = (byte)Port.ReadByte();

            if ((nRsp & 0x01) == 0x01)  // vertical parity error
                ErrorCode = new ServerErrorCode(this, 0x100, GetSpecificDescription(0x100));
            else if ((nRsp & 0x02) == 0x02)  // Framing error
                ErrorCode = new ServerErrorCode(this, 0x101, GetSpecificDescription(0x101));
            else if ((nRsp & 0x04) == 0x04)  // Printer mechanical error
                ErrorCode = new ServerErrorCode(this, 0x102, GetSpecificDescription(0x102));
            else
                ErrorCode = new ServerErrorCode(this, GeneralError.Success);
        }

        /// <summary>
        /// Запись в параллельный порт
        /// </summary>
        /// <param name="checkPaperOut"></param>
        private void WriteToParallel(bool checkPaperOut, bool throwOnBusy)
        {
            bool success = false;
            int paperRetriesCount = 0;
            int bytesWritten = 0;
            do
            {
                try
                {
                    Port.DiscardBuffers();
                    // запись в параллельный порт
                    while (bytesWritten < cmdLen)
                    {
                        Port.WriteByte(currCmd[bytesWritten]);
                        bytesWritten++;
                    }
                    success = true;
                }
                catch (System.ComponentModel.Win32Exception E)
                {
                    switch (E.NativeErrorCode)
                    {
                        case ERROR_BUSY:
                            // устройство не может принять данные
                            if (throwOnBusy)
                                throw;

                            success = false;
                            break;
                        case ERROR_DEVICE_NOT_CONNECTED:
                            // устройство не подключено либо в процессе печати достигнут конец листа
                            if (throwOnBusy)
                                throw;

                            if (checkPaperOut)
                            {
                                paperRetriesCount++;
                                if (paperRetriesCount >= MAX_PAPER_OUT_RETRIES)
                                    throw new System.ComponentModel.Win32Exception(ERROR_OUT_OF_PAPER);
                                success = false;
                            }
                            else
                                success = true;

                            //success = false;
                            break;
                        case ERROR_OUT_OF_PAPER:
                            // нет  бумаги
                            if (checkPaperOut)
                            {
                                paperRetriesCount++;
                                if (paperRetriesCount >= MAX_PAPER_OUT_RETRIES)
                                    throw;
                                success = false;
                            }
                            else
                                success = true;
                            break;
                        default:
                            throw;
                    }
                }
            }
            while (!success);
        }

        /// <summary>
        /// Запрос статусного байта
        /// </summary>
        /// <param name="rspByte"></param>
        /// <returns></returns>
        private byte GetStatusByte(byte rspByte)
        {
            if (!IsSerial)
                return 0; 
            try
            {
                Port.WriteByte(rspByte);
                return (byte)Port.ReadByte();
            }
            catch (TimeoutException)
            {
                ErrorCode = new ServerErrorCode(this, GeneralError.Timeout);
                return 0;
            }
            catch (Exception E)
            {
                ErrorCode = new ServerErrorCode(this, E);
                return 0;
            }
        }

        private void WaitForPrinting()
        {
            if (!IsSerial)
                return;
            try
            {
                byte nRsp;
                // дожидаемся освобождения буфера
                do
                {
                    System.Threading.Thread.Sleep(200);
                    Port.WriteByte(0x05);
                    nRsp = (byte)Port.ReadByte();
                }
                while ((nRsp & 0x20) != 0x20);
            }
            catch (TimeoutException)
            {
                ErrorCode = new ServerErrorCode(this, GeneralError.Timeout);
            }
            catch (Exception E)
            {
                ErrorCode = new ServerErrorCode(this, E);
            }
        }

        private void WaitForNewSlip()
        {
            if (!IsSerial)
                return;
            try
            {
                byte nRsp;
                // дожидаемся появления бумаги
                do
                {
                    System.Threading.Thread.Sleep(200);
                    Port.WriteByte(0x04);
                    nRsp = (byte)Port.ReadByte();
                }
                while ((nRsp & 0x80) != 0x80);
            }
            catch (TimeoutException)
            {
                ErrorCode = new ServerErrorCode(this, GeneralError.Timeout);
            }
            catch (Exception E)
            {
                ErrorCode = new ServerErrorCode(this, E);
            }
        }

        private bool CheckSlipSize(int linesCount)
        {
            if(!PrintOnSlip)
                return true;

            if (PrinterInfo.SlipFormLength == 0)
                return true;

            slipLineNo += linesCount;
            if (slipLineNo < maxSlipLines)
                return true;

            slipLineNo = 0;
            // ждем окончания печати буфера
            WaitForPrinting();
            if (!ErrorCode.Succeeded)
                return false;

            // возвращаем бумагу
            ClearBuffer();
            WriteBuffer(0x1B, 0x0C, 0x03);
            FlushBuffer();
            if (!ErrorCode.Succeeded)
                return false;

            // ждем пока вставят новый лист
            WaitForNewSlip();

            // печатаем верхний отступ
            if(!PrintTopMargin())
                return false;

            return ErrorCode.Succeeded;
        }

        private byte[] GetBarcodeBytes(string barcode, int nWidth)
        {
            try
            {
                // расчет контрольной суммы            
                char[] barcodeChars = barcode.ToCharArray(0, 12);
                int nChecksum = 0;
                for (int i = 0; i < 12; i += 2)
                {
                    nChecksum += Convert.ToInt32(barcodeChars[i].ToString());
                    if (i + 1 < 12)
                        nChecksum += Convert.ToInt32(barcodeChars[i + 1].ToString()) * 3;
                }
                nChecksum = nChecksum % 10;
                if (nChecksum > 0)
                    nChecksum = 10 - nChecksum;
                barcode += nChecksum.ToString();

                string sBarcodeData = "101";
                int numberSystem = Convert.ToInt32(barcode[0].ToString());
                for (int i = 1; i < 7; i++)
                {
                    int value = Convert.ToInt32(barcode[i].ToString());
                    sBarcodeData += EAN_CHARSET[value][EAN_PARITY[numberSystem][i - 1]];
                }
                sBarcodeData += "01010";
                for (int i = 7; i < 13; i++)
                {
                    int value = Convert.ToInt32(barcode[i].ToString());
                    sBarcodeData += EAN_CHARSET[value][RIGHT];
                }
                sBarcodeData += "101";
                sBarcodeData += "0";

                int byteNo = 0;
                byte[] barcodeData = new byte[sBarcodeData.Length * nWidth];
                foreach (char c in sBarcodeData)
                {
                    for (int i = 0; i < nWidth; i++)
                        if (c == '1')
                            barcodeData[byteNo++] = 0xFF;
                        else
                            barcodeData[byteNo++] = 0;
                }
                return barcodeData;
            }
            catch(Exception E)
            {
                ErrorCode = new ServerErrorCode(this, E);
                return null;
            }
        }
        
        private byte[] GetImageBytes(System.Drawing.Bitmap image)
        {
            try
            {
                int rowsCount = image.Height / 8;
                if (image.Height % 8 > 0)
                    rowsCount++;

                byte[] imageBytes = new byte[rowsCount * image.Width];
                int byteNo = 0;

                for (int rowN = 0; rowN < image.Height; rowN += 8)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        for (int y = 7; y >= 0; y--)
                        {
                            if (rowN + y >= image.Height)
                                continue;
                            uint c = (uint)image.GetPixel(x, rowN + y).ToArgb();
                            if (c == 0xff000000)
                                imageBytes[byteNo] = (byte)(imageBytes[byteNo] + (0x1 << (7 - y)));
                        }
                        byteNo++;
                    }
                }
                return imageBytes;
            }
            catch (Exception E)
            {
                ErrorCode = new ServerErrorCode(this, E);
                return null;
            }
        }

        #endregion

        #region Реализация виртуальных методов CustomSerialDevice

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

        protected override void OnAfterActivate()
        {
            Port.ReadTimeout = READ_TIMEOUT;
            Port.WriteTimeout = WRITE_TIMEOUT;

            double Y_UNIT_SIZE = 0.176;
            int lineInterval = 24;
            if (printerInfo.SlipFormLength > 0)
                maxSlipLines = (int)((printerInfo.SlipFormLength - 20) / (Y_UNIT_SIZE * lineInterval));

            ClearBuffer();

            // инициализация принтера
            WriteBuffer(0x1B, 0x40);

            // устанавливаем memory switch 2
            WriteBuffer(0x1B, 0x23); // команда установки переключателей
            WriteBuffer(0x32); // переключатель №2 
            WriteBuffer(0x2C); // разделитель
            // автоматическая промотка бумаги перед отрезкой и датчик окончания ленты
            WriteBuffer("1001"); // флаги переключателя
            WriteBuffer(0x0A, 0x00); // завершение команды

            // reset (применение настроек). После выполнения команды производится 
            // тестовая печать. Если команду не выполнять, новые настройки
            // будут применены после выключения/включения принтера
            //WriteBuffer(0x1B, 0x3F, 0x0A, 0x00);

            FlushBuffer(false, true);
        }

        #endregion

        #region Реализация виртуальных методов CustomPrintableDevice

        private bool PrintOnSlip
        {
            get
            {
                return this.PrinterNumber != PrinterNumber.MainPrinter
                       || !IsSerial
                       || ((this.PrinterNumber == PrinterNumber.MainPrinter) && (GetStatusByte(0x04) & 0x10) == 0x10);
            }
        }

        private int CurrTapeWidth
        {
            get
            {
                return PrintOnSlip ? SLIP_TAPE_WIDTH : RECEIPT_TAPE_WIDTH;
/*
                if (PrinterNumber == PrinterNumber.MainPrinter)
                    return PrinterInfo.TapeWidth.MainPrinter;
                else
                    return PrinterInfo.TapeWidth.AdditionalPrinter1;
 */ 
            }
        }

        protected override void OnCustomTopMarginPrinted()
        {
            if (PrintOnSlip)
            {
//                PrintHeader();
                slipLineNo --;
                DrawGraphicHeader();
            }
        }

        protected override void OnOpenDocument(DocumentType docType, string cashierName)
        {
            ExecuteDriverCommand(delegate()
            {
                slipLineNo = 0;

                // устанавливаем режим печати: слип или лента
                ClearBuffer();
                WriteBuffer(0x1B, 0x2B, 0x41);
                WriteBuffer(this.PrinterNumber == PrinterNumber.MainPrinter ? (byte)0x30 : (byte)0x33);
                FlushBuffer(true, true);

                if (!PrintOnSlip)
                {
                    //                PrintHeader();
                    DrawGraphicHeader();
                }
                else
                {
                    // нет бумаги в подкладнике
                    byte slipStatusByte = GetStatusByte(0x04);
                    if ((slipStatusByte & 0x80) == 0)
                        ErrorCode = new ServerErrorCode(this, ERROR_NO_SLIP, GetSpecificDescription(ERROR_NO_SLIP));
                }

                docOpened = ErrorCode.Succeeded;
            });
        }

        protected override void OnCloseDocument(bool cutPaper)
        {
            ExecuteDriverCommand(delegate()
            {
                docOpened = false;

                // печать подвала
                //            PrintFooter();
                DrawGraphicFooter();

                ClearBuffer();
                // возврат бумаги для подкладника
                WriteBuffer(0x1B, 0x0C, 0x03);
                // отрезка чека для термопринтера
                WriteBuffer(0x1B, 0x64, 0x31);
                FlushBuffer();

                if (ErrorCode.Succeeded)
                    WaitForPrinting();
            });
        }

        protected override void OnPrintString(string source, FontStyle style)
        {
            ExecuteDriverCommand(delegate()
            {

                if (!CheckSlipSize(style == FontStyle.DoubleHeight || style == FontStyle.DoubleAll ? 2 : 1))
                    return;

                ClearBuffer();
                // обрезаем строку до максимальной длины
                if (source.Length > CurrTapeWidth)
                    source = source.Substring(0, CurrTapeWidth);

                // установка шрифта
                switch (style)
                {
                    case FontStyle.DoubleHeight:
                        WriteBuffer(0x1B, 0x68, 0x01);
                        break;
                    case FontStyle.DoubleWidth:
                        WriteBuffer(0x1B, 0x57, 0x01);
                        break;
                    case FontStyle.DoubleAll:
                        WriteBuffer(0x1B, 0x68, 0x01);
                        WriteBuffer(0x1B, 0x57, 0x01);
                        break;
                }
                // строка
                WriteBuffer(source);
                // CRLF
                WriteBuffer(0x0D, 0x0A);

                // сброс шрифта
                WriteBuffer(0x1B, 0x68, 0x00);
                WriteBuffer(0x1B, 0x57, 0x00);

                FlushBuffer();
            });
        }

        protected override void OnPrintBarcode(string barcode, AlignOptions align, bool readable)
        {
            ExecuteDriverCommand(delegate()
            {
                int width = 2;
                byte[] barcodeBytes = GetBarcodeBytes(barcode, width);
                if (barcodeBytes == null)
                    return;

                if (!CheckSlipSize(width * 2))
                    return;

                int paddingLength = 0;
                if (align != AlignOptions.Left)
                    paddingLength = CurrTapeWidth - barcodeBytes.Length / (PrintOnSlip ? 10 : 12);
                if (align != AlignOptions.Right)
                    paddingLength = paddingLength / 2;
                byte[] paddingBytes = Encoding.ASCII.GetBytes(new string(' ', paddingLength));

                ClearBuffer();
                for (int i = 0; i < width * 2; i++)
                {
                    // позиционирование пробелами
                    WriteBuffer(paddingBytes);
                    // команда печати графики
                    WriteBuffer(0x1B, 0x4C);
                    // длина строки
                    WriteBuffer(BitConverter.GetBytes((short)barcodeBytes.Length));
                    WriteBuffer(0x00);
                    // байты штрихкода
                    WriteBuffer(barcodeBytes);
                    // переход на новую строку
                    WriteBuffer(0x1B, 0x4A, PrintOnSlip ? (byte)8 : (byte)11);
                }

                if (readable)
                {
                    // позиционирование пробелами
                    WriteBuffer(paddingBytes);
                    WriteBuffer(0x20, 0x20);
                    if (PrintOnSlip)
                        WriteBuffer(0x20, 0x20);
                    // печать цифр штрихкода
                    WriteBuffer(barcode);
                    // CRLF
                    WriteBuffer(0x0D, 0x0A);
                }

                FlushBuffer();
            });
        }

        protected override void OnPrintImage(System.Drawing.Bitmap image, AlignOptions align)
        {
            ExecuteDriverCommand(delegate()
            {
                byte[] imageBytes = GetImageBytes(image);
                if (imageBytes == null)
                    return;

                if (!CheckSlipSize(image.Height / 8))
                    return;

                ClearBuffer();
                int paddingLength = 0;
                if (align != AlignOptions.Left)
                    paddingLength = CurrTapeWidth - imageBytes.Length / (PrintOnSlip ? 10 : 12);
                if (align != AlignOptions.Right)
                    paddingLength = paddingLength / 2;
                byte[] paddingBytes = Encoding.ASCII.GetBytes(new string(' ', paddingLength));

                // печать картинки
                int rowsCount = image.Height / 8;
                if (image.Height % 8 > 0)
                    rowsCount++;

                for (int rowNo = 0; rowNo < rowsCount; rowNo++)
                {
                    // позиционирование пробелами
                    WriteBuffer(paddingBytes);
                    // команда печати графики
                    WriteBuffer(0x1B, 0x4C);
                    // длина строки
                    WriteBuffer(BitConverter.GetBytes((short)image.Width));
                    WriteBuffer(0x00);
                    // байты строки изображения
                    WriteBuffer(imageBytes, rowNo * image.Width, image.Width);
                    // переход на новую строку
                    WriteBuffer(0x1B, 0x4A, PrintOnSlip ? (byte)8 : (byte)12);
                }

                FlushBuffer();
            });
        }

        protected override void OnOpenDrawer()
        {
            ExecuteDriverCommand(delegate()
            {
                ClearBuffer();
                WriteBuffer(0x07);
                FlushBuffer();
            });
        }

        protected override void OnRegistration(string commentary, uint quantity, uint amount, byte section)
        {
            OnPrintString(commentary, FontStyle.Regular);
        }

        protected override void OnPayment(uint amount, FiscalPaymentType paymentType)
        {
            ExecuteDriverCommand(delegate(){ });
        }

        protected override void OnCash(uint amount)
        {
            ExecuteDriverCommand(delegate() { });
        }

        #endregion

        #region Реализация интерфейса IPrintableDevice

        public override event EventHandler<PrinterBreakEventArgs> PrinterBreak;

        public override PrintableDeviceInfo PrinterInfo
        {
            get 
            {
//                printerInfo.TapeWidth.MainPrinter = CurrTapeWidth;
                return printerInfo; 
            }
        }

        public override PrinterStatusFlags PrinterStatus
        {
            get
            {
                bool bDrawerOpened = false;
                bool bPrinting = false;
                PaperOutStatus poStatus = PaperOutStatus.Present;

                ExecuteDriverCommand(delegate()
                {
                    if (IsSerial)
                    {
                        byte slipStatusByte = GetStatusByte(0x04);
                        byte printerStatusByte = GetStatusByte(0x05);

                        // нет бумаги в подкладнике
                        if (PrintOnSlip && ((slipStatusByte & 0x80) == 0))
                            poStatus = PaperOutStatus.OutPassive;
                        bPrinting = (printerStatusByte & 0x20) != 0x20;

                    }
                });
                return new PrinterStatusFlags(bPrinting, poStatus, docOpened, bDrawerOpened);
            }
        }

        #endregion
    }
}
