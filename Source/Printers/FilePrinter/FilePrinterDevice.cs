using System;
using System.Text;
using DevicesBase;
using DevicesBase.Helpers;
using DevicesCommon;
using DevicesCommon.Helpers;

namespace FilePrinter
{
    [Serializable]
    [FiscalDevice("Эмулятор")]
    public class FilePrinterDevice :  CustomFiscalDevice
    {
        #region Константы

        private const string SERIAL_NO = "0123456789";

        private const int TAPE_WIDTH = 40;

        #endregion

        #region Внутренние поля

        private bool openedShift = false;

        private bool fiscalized = false;

        private int docAmount = 0;

        private int paymentAmount = 0;

        private int cashInDrawer = 0;

        private bool openedDocument = false;

        private bool active = false;

        private string fileName;

        private int docNo = 1;

        private PrintableDeviceInfo _printerInfo = new PrintableDeviceInfo(new PrintableTapeWidth(TAPE_WIDTH, 0), true);

        #endregion

        #region Переопределенные методы

        public override bool Active
        {
            get
            {
                return active;
            }
            set
            {
                active = value;
            }
        }

        public override string PortName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
            }
        }

        #endregion

        #region Реализация виртуальных функций

        protected override void OnOpenDocument(DocumentType docType,
            string cashierName)
        {
            OnPrintString("", FontStyle.Regular);
            OnPrintString("", FontStyle.Regular);
            if (DocumentHeader != null)
                foreach (string s in DocumentHeader)
                    OnPrintString(s, FontStyle.Regular);

            string headerString = string.Format("Кассир: {0}", cashierName).PadRight(PrinterInfo.TapeWidth.MainPrinter - 4);
            OnPrintString(headerString + "#" + docNo.ToString("d3"), FontStyle.Regular);
            switch (docType)
            {
                case DocumentType.Sale:
                    OnPrintString("Продажа", FontStyle.Regular);
                    break;
                case DocumentType.Refund:
                    OnPrintString("Возврат", FontStyle.Regular);
                    break;
                case DocumentType.PayingIn:
                    OnPrintString("Внесение", FontStyle.Regular);
                    break;
                case DocumentType.PayingOut:
                    OnPrintString("Выплата", FontStyle.Regular);
                    break;
                case DocumentType.SectionsReport:
                    OnPrintString("Отчет по секциям", FontStyle.Regular);
                    break;
                case DocumentType.XReport:
                    OnPrintString("X-отчет", FontStyle.Regular);
                    break;
                case DocumentType.ZReport:
                    OnPrintString("Z-отчет", FontStyle.Regular);
                    break;
                case DocumentType.Other:
                    OnPrintString("Нефискальный документ", FontStyle.Regular);
                    break;
            }

            OnPrintString(new string(Separator, PrinterInfo.TapeWidth.MainPrinter), FontStyle.Regular);

            openedShift = true;
            docAmount = 0;
            paymentAmount = 0;
            openedDocument = true;
            ErrorCode = new ServerErrorCode(this, GeneralError.Success);
        }

        protected override void OnCloseDocument(bool cutPaper)
        {
            if (paymentAmount > docAmount)
            {
                string printLine = "Сдача:";
                OnPrintString(printLine + string.Format("{0:f2}", (paymentAmount - docAmount) / 100.0).PadLeft(PrinterInfo.TapeWidth.MainPrinter - printLine.Length), FontStyle.Regular);
            }

            OnPrintString("", FontStyle.Regular);
            if (DocumentFooter != null)
                foreach (string s in DocumentFooter)
                    OnPrintString(s, FontStyle.Regular);

            openedDocument = false;
            cashInDrawer += docAmount;
            docAmount = 0;
            paymentAmount = 0;
            docNo++;
            ErrorCode = new ServerErrorCode(this, GeneralError.Success);
        }

        protected override void OnOpenDrawer()
        {
            ErrorCode = new ServerErrorCode(this, GeneralError.Success);
        }

        protected override void OnPrintString(string source, FontStyle style)
        {
            if (!System.IO.File.Exists(fileName))
                System.IO.File.Create(fileName).Close();

            System.IO.File.AppendAllText(fileName, (source.Length > PrinterInfo.TapeWidth.MainPrinter ? source.Substring(0, PrinterInfo.TapeWidth.MainPrinter) : source) + "\n", Encoding.Default);
            ErrorCode = new ServerErrorCode(this, GeneralError.Success);
        }

        protected override void OnPrintBarcode(string barcode, AlignOptions align,
            bool readable)
        {
            OnPrintString(barcode.PadLeft((barcode.Length + PrinterInfo.TapeWidth.MainPrinter) / 2), FontStyle.Regular);
            ErrorCode = new ServerErrorCode(this, GeneralError.Success);
        }

        protected override void OnPrintImage(System.Drawing.Bitmap image, AlignOptions align)
        {
            ErrorCode = new ServerErrorCode(this, GeneralError.Success);
        }

        protected override void OnRegistration(string commentary, uint quantity, uint amount,
            byte section)
        {
            int regAmount = (int)(amount * quantity / 1000.0);
            docAmount += regAmount;

            string printLine = commentary;
            OnPrintString(printLine + string.Format("{0:f2}", regAmount / 100.0).PadLeft(PrinterInfo.TapeWidth.MainPrinter - printLine.Length), FontStyle.Regular);
            ErrorCode = new ServerErrorCode(this, GeneralError.Success);
        }

        protected override void OnPayment(uint amount, FiscalPaymentType paymentType)
        {
            string printLine = "";
            switch (paymentType)
            {
                case FiscalPaymentType.Card:
                    printLine = "Безнал. оплата:";
                    break;
                case FiscalPaymentType.Cash:
                    printLine = "Наличные:";
                    break;
                default:
                    printLine = "Оплата:";
                    break;
            }
            paymentAmount += (int)amount;
            OnPrintString(printLine + string.Format("{0:f2}", amount / 100.0).PadLeft(PrinterInfo.TapeWidth.MainPrinter - printLine.Length), FontStyle.Regular);
            ErrorCode = new ServerErrorCode(this, GeneralError.Success);
        }

        protected override void OnCash(uint amount)
        {
            string printLine = "Сумма:";
            OnPrintString(printLine + Convert.ToString(amount / 100.0).PadLeft(PrinterInfo.TapeWidth.MainPrinter - printLine.Length), FontStyle.Regular);
            ErrorCode = new ServerErrorCode(this, GeneralError.Success);
        }

        #endregion

        #region Свойства

        public override DateTime CurrentTimestamp
        {
            get { return DateTime.Now; }
            set { }
        }

        public override FiscalDeviceInfo Info
        {
            get { return new FiscalDeviceInfo("Эмулятор", SERIAL_NO); }
        }

        public override FiscalStatusFlags Status
        {
            get 
            {
                return new FiscalStatusFlags(openedShift, false, false, fiscalized, (ulong)docAmount, (ulong)cashInDrawer);
            }
        }

        public override PrintableDeviceInfo PrinterInfo
        {
            get { return _printerInfo; }
        }

        public override PrinterStatusFlags PrinterStatus
        {
            get { return new PrinterStatusFlags(false, PaperOutStatus.Present, openedDocument, false); }
        }

        #endregion

        #region События

        public override event EventHandler<FiscalBreakEventArgs> FiscalBreak;

        public override event EventHandler<PrinterBreakEventArgs> PrinterBreak;

        #endregion

        #region Методы

        public override void FiscalReport(FiscalReportType reportType, bool full, params object[] reportParams)
        {
            ErrorCode = new ServerErrorCode(this, GeneralError.Success);
        }

        public override void Fiscalization(int newPassword, long registrationNumber, long taxPayerNumber)
        {
            fiscalized = true;
            ErrorCode = new ServerErrorCode(this, GeneralError.Success);
        }

        public override void GetLifetime(out DateTime firstDate, out DateTime lastDate, out int firstShift, out int lastShift)
        {
            firstDate = DateTime.Now;
            lastDate = DateTime.Now;
            firstShift = 1;
            lastShift = 9999;

            ErrorCode = new ServerErrorCode(this, GeneralError.Success);
        }

        #endregion
    }
}
