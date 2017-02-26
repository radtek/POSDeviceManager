using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;
using DevicesCommon;
using DevicesCommon.Helpers;
using DevicesBase;
using DevicesBase.Helpers;

namespace WindowsPrinter
{
    [Serializable]
    [PrintableDevice("Windows-принтер")]
    public class WindowsPrinterDevice : CustomPrintableDevice
    {
        /// <summary>
        /// Вспомогательный класс для хранения данных строки печати
        /// </summary>
        private class LineInfo
        {
            public string Text { get; set; }

            public DevicesCommon.Helpers.FontStyle Style { get; set; }

            public AlignOptions Align { get; set; }

            public Bitmap Image { get; set; }
        }

        #region Константы

        /// <summary>
        /// Используемый для печати моноширинный шрифт
        /// </summary>
        private string FONT_FAMILY = "Lucida Console";

        /// <summary>
        /// Размер шрифта
        /// </summary>
        private const int FONT_SIZE = 8;

        /// <summary>
        /// Высота штрихкода в пикселях
        /// </summary>
        private const int BC_HEIGHT = 50;

        #endregion

        #region Поля

        /// <summary>
        /// Буфер печатаемых строк
        /// </summary>
        private List<LineInfo> _printBuffer = new List<LineInfo>();

        /// <summary>
        /// Номер текущей строки
        /// </summary>
        private int _currentLine;

        /// <summary>
        /// Текущая позиция по вертикали
        /// </summary>
        private float _yPos;

        /// <summary>
        /// Информация об устройстве
        /// </summary>
        private PrintableDeviceInfo _printerInfo = new PrintableDeviceInfo(new PrintableTapeWidth(40, 0), true);

        #endregion

        #region Методы базового класса

        public override event EventHandler<PrinterBreakEventArgs> PrinterBreak;

        public override PrintableDeviceInfo PrinterInfo { get { return _printerInfo; } }

        public override PrinterStatusFlags PrinterStatus
        {
            get { return new PrinterStatusFlags(false, PaperOutStatus.Present, false, false); }
        }

        public override bool Active { get; set; }

        public override string PortName { get; set; }

        protected override void OnPrintString(string source, DevicesCommon.Helpers.FontStyle style)
        {
            _printBuffer.Add(new LineInfo() { Text = source, Style = style });
        }

        protected override void OnPrintBarcode(string barcode, AlignOptions align, bool readable)
        {
            _printBuffer.Add(new LineInfo()
            {
                Text = readable ? barcode : string.Empty,
                Align = align,
                Image = BarcodeBuilder.GetBarcodeImage(barcode, BC_HEIGHT)
            });
        }

        protected override void OnPrintImage(Bitmap image, AlignOptions align)
        {
            _printBuffer.Add(new LineInfo() { Image = image, Align = align });
        }

        protected override void OnOpenDocument(DocumentType docType, string cashierName)
        {
            base.OnOpenDocument(docType, cashierName);
            _printBuffer.Clear();
        }

        protected override void OnCloseDocument(bool cutPaper)
        {
            try
            {
                using (var document = new PrintDocument())
                {
                    document.PrinterSettings.PrinterName = PortName;
                    document.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                    document.PrintPage += new PrintPageEventHandler(document_PrintPage);

                    _yPos = document.DefaultPageSettings.Bounds.Top;
                    _currentLine = 0;

                    document.Print();
                }
            }
            catch (InvalidPrinterException)
            {
                ErrorCode = new ServerErrorCode(this, 1, string.Format("Принтер с именем \"{0}\" не найден", PortName));
            }
            catch (Exception e)
            {
                ErrorCode = new ServerErrorCode(this, e);
            }
        }

        #endregion

        #region Метод печати

        /// <summary>
        /// Печать страницы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void document_PrintPage(object sender, PrintPageEventArgs e)
        {
            while (_currentLine < _printBuffer.Count)
            {
                if (_yPos >= e.MarginBounds.Bottom)
                {
                    e.HasMorePages = true;
                    _yPos = e.MarginBounds.Top;
                    return;
                }

                var line = _printBuffer[_currentLine++];

                if (line.Image != null)
                {
                    switch (line.Align)
                    {
                        case AlignOptions.Left:
                            e.Graphics.DrawImage(line.Image, e.MarginBounds.Left, _yPos);
                            break;
                        case AlignOptions.Center:
                            e.Graphics.DrawImage(line.Image, (e.MarginBounds.Right - line.Image.Width) / 2, _yPos);
                            break;
                        case AlignOptions.Right:
                            e.Graphics.DrawImage(line.Image, e.MarginBounds.Right - line.Image.Width, _yPos);
                            break;
                    }

                    _yPos += line.Image.Height;
                }

                if (!string.IsNullOrEmpty(line.Text))
                {
                    var printFont = new Font(FONT_FAMILY, FONT_SIZE);
                    switch (line.Style)
                    {
                        case DevicesCommon.Helpers.FontStyle.DoubleHeight:
                        case DevicesCommon.Helpers.FontStyle.DoubleWidth:
                            printFont = new Font(FONT_FAMILY, FONT_SIZE, System.Drawing.FontStyle.Bold);
                            break;
                        case DevicesCommon.Helpers.FontStyle.DoubleAll:
                            printFont = new Font(FONT_FAMILY, FONT_SIZE * 2, System.Drawing.FontStyle.Bold);
                            break;
                    }

                    e.Graphics.DrawString(line.Text, printFont, Brushes.Black, e.MarginBounds.Left,
                        _yPos);
                    _yPos += printFont.GetHeight(e.Graphics);
                }
            }
        }

        #endregion

    }
}
