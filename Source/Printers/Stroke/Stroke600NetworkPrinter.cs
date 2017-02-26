using System;
using System.Text;
using DevicesBase;
using DevicesBase.Helpers;
using DevicesCommon;
using DevicesCommon.Helpers;

namespace Stroke
{
    [PrintableDevice("Штрих-600 (TCP)")]
    public class Stroke600NetworkPrinter : CustomNetworkPrinter
    {
        #region Константы

        private const int DEF_TCP_PORT = 9100;

        private const int TAPE_WIDTH = 42;
        private const int MAX_CMD_LEN = 1024;

        private const string _errorWaitingEndOfPrint =
            "Ошибка во время ожидания завершения печати документа.\n" +
            "Тип: {0}\nТекст: {1}\nСтек:\n\n{2}";

        #endregion

        #region Переменные

        private bool _openedDoc = false;

        private byte[] _docBuffer = new byte[1024 * 1024];

        private int _bufSize = 0;

        private PrintableDeviceInfo _printerInfo = new PrintableDeviceInfo(new PrintableTapeWidth(TAPE_WIDTH, 0), false);

        #endregion

        protected override int TcpPort
        {
            get { return DEF_TCP_PORT; }
        }

        #region Реализация интерфейса IPrintableDevice

        public override event EventHandler<PrinterBreakEventArgs> PrinterBreak;

        public override PrintableDeviceInfo PrinterInfo
        {
            get { return _printerInfo; }
        }

        #endregion

        protected override PrinterStatusFlags OnQueryPrinterStatus(DevicesBase.Communicators.TcpCommunicator communicator)
        {
            PaperOutStatus poStatus = PaperOutStatus.Present;
            bool bDrawerOpened = false;
            bool bPrinting = false;
            ErrorCode = new ServerErrorCode(this, GeneralError.Success);
            return new PrinterStatusFlags(bPrinting, poStatus, _openedDoc, bDrawerOpened);
        }

        protected override void OnOpenDocument(DocumentType docType,
            string cashierName)
        {
            _bufSize = 0;
            // инициализация принтера
            _docBuffer[_bufSize++] = 0x1B;
            _docBuffer[_bufSize++] = 0x40;

            // установка размера шрифта
            _docBuffer[_bufSize++] = 0x1B;
            _docBuffer[_bufSize++] = 0x4D;

            // жирный шрифт
            _docBuffer[_bufSize++] = 0x1B;
            _docBuffer[_bufSize++] = 0x45;

            this.DrawHeader();

            _openedDoc = true;
        }

        protected override void OnCloseDocument(bool cutPaper)
        {
            this.DrawFooter();
            Communicator.Write(_docBuffer, 0, _bufSize);
            _bufSize = 0;
            _openedDoc = false;
        }

        protected override void OnPrintBarcode(string barcode, AlignOptions align, bool readable)
        {
            _docBuffer[_bufSize++] = 0x1B;
            _docBuffer[_bufSize++] = 0x28;
            _docBuffer[_bufSize++] = 0x42;

            _docBuffer[_bufSize++] = (byte)(barcode.Length + 6);   // nL
            _docBuffer[_bufSize++] = 0x00; // nH

            _docBuffer[_bufSize++] = 0x00; // k

            _docBuffer[_bufSize++] = 0x02; // m

            _docBuffer[_bufSize++] = 0xFE; // s

            _docBuffer[_bufSize++] = 0x84; // v1
            _docBuffer[_bufSize++] = 0x00; // v2
            _docBuffer[_bufSize++] = 0x05; // c

            Encoding.ASCII.GetBytes(barcode).CopyTo(_docBuffer, _bufSize);
            _bufSize += barcode.Length;

            _docBuffer[_bufSize++] = 0x0D;
            _docBuffer[_bufSize++] = 0x0A;
            _docBuffer[_bufSize++] = 0x0D;
            _docBuffer[_bufSize++] = 0x0A;
        }

        protected override void OnPrintString(string source, FontStyle style)
        {
            // обрезаем строку до максимальной длины
            if (source.Length > PrinterInfo.TapeWidth.MainPrinter)
                source = source.Substring(0, PrinterInfo.TapeWidth.MainPrinter);

            _docBuffer[_bufSize++] = 0x1B;
            _docBuffer[_bufSize++] = 0x74;
            _docBuffer[_bufSize++] = 0x02;

            switch (style)
            {
                case FontStyle.DoubleHeight:
                    _docBuffer[_bufSize++] = 0x1B;
                    _docBuffer[_bufSize++] = 0x77;
                    _docBuffer[_bufSize++] = 0x01;
                    break;
                case FontStyle.DoubleWidth:
                    _docBuffer[_bufSize++] = 0x1B;
                    _docBuffer[_bufSize++] = 0x57;
                    _docBuffer[_bufSize++] = 0x01;
                    break;
                case FontStyle.DoubleAll:
                    _docBuffer[_bufSize++] = 0x1B;
                    _docBuffer[_bufSize++] = 0x57;
                    _docBuffer[_bufSize++] = 0x01;
                    _docBuffer[_bufSize++] = 0x1B;
                    _docBuffer[_bufSize++] = 0x77;
                    _docBuffer[_bufSize++] = 0x01;
                    break;
            }


            byte[] data = Encoding.GetEncoding(866).GetBytes(source);
            data.CopyTo(_docBuffer, _bufSize);
            _bufSize += data.Length;

            // отмена двойной ширины
            _docBuffer[_bufSize++] = 0x1B;
            _docBuffer[_bufSize++] = 0x57;
            _docBuffer[_bufSize++] = 0x00;

            // отмена двойной высоты
            _docBuffer[_bufSize++] = 0x1B;
            _docBuffer[_bufSize++] = 0x77;
            _docBuffer[_bufSize++] = 0x00;

            _docBuffer[_bufSize++] = 0x0D;
            _docBuffer[_bufSize++] = 0x0A;
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

        protected override void OnOpenDrawer()
        {
            _docBuffer[_bufSize++] = 0x1B;
            _docBuffer[_bufSize++] = Convert.ToByte('p');
            _docBuffer[_bufSize++] = 0;
            _docBuffer[_bufSize++] = 100;
            _docBuffer[_bufSize++] = 0xFF;

            _docBuffer[_bufSize++] = 0x1B;
            _docBuffer[_bufSize++] = Convert.ToByte('p');
            _docBuffer[_bufSize++] = 1;
            _docBuffer[_bufSize++] = 100;
            _docBuffer[_bufSize++] = 0xFF;

        }

        protected override int ReadTimeout
        {
            get { return 10000; }
        }

        protected override int WriteTimeout
        {
            get { return 10000; }
        }
    }
}
