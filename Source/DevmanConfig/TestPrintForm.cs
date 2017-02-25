using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using DevicesCommon;
using DevicesCommon.Helpers;
using DevicesCommon.Connectors;

namespace DevmanConfig
{

    internal partial class TestPrintForm : Form
    {
        #region Константы и перечисления

        private string[] docTypes = new string[] { 
            "sale", 
            "refund", 
            "other", 
            "payIn", 
            "payOut", 
            "reportX", 
            "reportZ", 
            "reportSections" };

        private enum Align { left, right, center };

        private enum Style { regular, doubleHeight, doubleWidth, doubleAll };

        private enum LineType { text, separator, barcode, image, registration, payment, cash, table };

        #endregion

        public static void TestPrint(string deviceId)
        {
            using (TestPrintForm printDlg = new TestPrintForm())
            {
                printDlg.deviceId = deviceId;
                printDlg.ShowDialog();
            }
        }

        public string deviceId;

        public TestPrintForm()
        {
            InitializeComponent();
            cbDrawer.SelectedIndex = 0;
            cbDocType.SelectedIndex = 0;
            rbFromFile_CheckedChanged(this, null);
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDlg = new OpenFileDialog())
            {
                fileDlg.Filter = "Документы XML (*.xml)|*xml|Все файлы (*.*)|*.*";
                fileDlg.InitialDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (fileDlg.ShowDialog() == DialogResult.OK)
                    tbFileName.Text = fileDlg.FileName;
            }
        }

        public string GetXmlDocument()
        {
            if (rbFromFile.Checked)
                return File.ReadAllText(tbFileName.Text, Encoding.GetEncoding(1251));
            else
                return GenerateReceipt();
        }

        #region Автоматическая генерация документа

        private string GenerateReceipt()
        {
            string fileName = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\TestPrint.xml";
            using (XmlTextWriter xmlWriter = new XmlTextWriter(fileName, Encoding.GetEncoding(1251)))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("documents");
                xmlWriter.WriteStartElement("document");
                xmlWriter.WriteAttributeString("type", docTypes[cbDocType.SelectedIndex]);
                xmlWriter.WriteAttributeString("cashier", "Кассир Вася");
                if (cbDrawer.SelectedIndex > 0)
                    xmlWriter.WriteAttributeString("drawer", cbDrawer.SelectedIndex == 1 ? "openBefore" : "openAfter");

                if (cbDocType.SelectedIndex < 3)
                {
                    WriteReceiptLine(xmlWriter, "ТЕСТОВАЯ ПЕЧАТЬ", Align.center);
                    WriteSeparator(xmlWriter, "=");
                    WriteReceiptLine(xmlWriter, cbDocType.SelectedItem.ToString(), Align.center, Style.doubleAll);
                    WriteReceiptLine(xmlWriter, DateTime.Now.ToString("Дата: dd.MM.yyyy Время: HH:mm:ss"));
                    WriteReceiptLine(xmlWriter, "Кассовый чек №1");

                    // таблица
                    xmlWriter.WriteStartElement("line");
                    xmlWriter.WriteAttributeString("type", "table");
                    xmlWriter.WriteAttributeString("grid", "-");

                    // заголовок таблицы
                    xmlWriter.WriteStartElement("columns");
                    WriteColumn(xmlWriter, Align.left, 0, "Наименование");
                    WriteColumn(xmlWriter, Align.right, 7, "Кол-во");
                    WriteColumn(xmlWriter, Align.right, 7, "Сумма");
                    xmlWriter.WriteEndElement();
                    // конец заголовка таблицы

                    // строки таблицы
                    xmlWriter.WriteStartElement("rows");
                    for (int i = 0; i < numPosCount.Value; i++)
                        WritePosition(xmlWriter, i + 1);
                    xmlWriter.WriteEndElement();
                    // конец строк таблицы

                    xmlWriter.WriteEndElement();
                    // конец таблицы

                    WriteReceiptLine(xmlWriter, "Скидка 10% 283,00", Align.right);
                    WriteReceiptLine(xmlWriter, "К ОПЛАТЕ: 2 547,00", Align.right, Style.doubleAll);
                    WriteSeparator(xmlWriter, "-");

                    // регистрация
                    if (cbDocType.SelectedIndex < 3)
                    {
                        xmlWriter.WriteStartElement("line");
                        xmlWriter.WriteAttributeString("type", "registration");
                        xmlWriter.WriteAttributeString("amount", Convert.ToInt32(numAmount.Value * 100).ToString());
                        xmlWriter.WriteAttributeString("section", "1");
                        xmlWriter.WriteEndElement();

                        // оплата
                        xmlWriter.WriteStartElement("line");
                        xmlWriter.WriteAttributeString("type", "payment");
                        xmlWriter.WriteAttributeString("paymentType", "cash");
                        xmlWriter.WriteAttributeString("amount", Convert.ToInt32(numAmount.Value * 100).ToString());
                        xmlWriter.WriteEndElement();
                    }

                }
                else if (cbDocType.SelectedIndex < 5)
                {
                    // внесение, выплата
                    xmlWriter.WriteStartElement("line");
                    xmlWriter.WriteAttributeString("type", "cash");
                    xmlWriter.WriteAttributeString("amount", Convert.ToInt32(numAmount.Value * 100).ToString());
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }
            return File.ReadAllText(fileName, Encoding.GetEncoding(1251));
        }

        private void WriteReceiptLine(XmlTextWriter xmlWriter, string value, LineType lineType, Align align, Style style)
        {
            xmlWriter.WriteStartElement("line");
            if (lineType != LineType.text)
                xmlWriter.WriteAttributeString("type", lineType.ToString());
            if (align != Align.left)
                xmlWriter.WriteAttributeString("align", align.ToString());
            if (style != Style.regular)
                xmlWriter.WriteAttributeString("style", style.ToString());
            xmlWriter.WriteString(value);
            xmlWriter.WriteEndElement();
        }

        private void WriteReceiptLine(XmlTextWriter xmlWriter, string value, Align align, Style style)
        {
            WriteReceiptLine(xmlWriter, value, LineType.text, align, style);
        }

        private void WriteSeparator(XmlTextWriter xmlWriter, string separator)
        {
            WriteReceiptLine(xmlWriter, "=", LineType.separator, Align.left, Style.regular);
        }

        private void WriteReceiptLine(XmlTextWriter xmlWriter, string value, Align align)
        {
            WriteReceiptLine(xmlWriter, value, LineType.text, align, Style.regular);
        }

        private void WriteReceiptLine(XmlTextWriter xmlWriter, string value)
        {
            WriteReceiptLine(xmlWriter, value, LineType.text, Align.left, Style.regular);
        }

        private void WritePosition(XmlTextWriter xmlWriter, int n)
        {
            xmlWriter.WriteStartElement("row");
            xmlWriter.WriteElementString("field", String.Format("{0}. Шампанское Ростовское", n));
            xmlWriter.WriteElementString("field", "1");
            xmlWriter.WriteElementString("field", "135,00");
            xmlWriter.WriteEndElement();
        }

        private void WriteColumn(XmlTextWriter xmlWriter, Align align, int width, string value)
        {
            xmlWriter.WriteStartElement("column");
            xmlWriter.WriteAttributeString("align", align.ToString());
            if (width > 0)
                xmlWriter.WriteAttributeString("width", width.ToString());
            xmlWriter.WriteString(value);
            xmlWriter.WriteEndElement();
        }

        #endregion

        private void rbFromFile_CheckedChanged(object sender, EventArgs e)
        {
            lbFileName.Enabled = rbFromFile.Checked;
            tbFileName.Enabled = rbFromFile.Checked;
            lbDocType.Enabled = !rbFromFile.Checked;
            cbDocType.Enabled = !rbFromFile.Checked;
            lbDrawer.Enabled = !rbFromFile.Checked;
            cbDrawer.Enabled = !rbFromFile.Checked;
            lbPosCount.Enabled = !rbFromFile.Checked;
            numPosCount.Enabled = !rbFromFile.Checked;
            lbAmount.Enabled = !rbFromFile.Checked;
            numAmount.Enabled = !rbFromFile.Checked;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                DeviceTester<IPrintableDevice> tester = new DeviceTester<IPrintableDevice>(
                    deviceId,
                    delegate(IPrintableDevice device)
                    {
                        // проверка наличия бумаги
                        PaperOutStatus paperStatus = device.PrinterStatus.PaperOut;
                        if (!device.ErrorCode.Succeeded)
                        {
                            MessageBox.Show(this, device.ErrorCode.FullDescription,
                                Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        if (paperStatus == PaperOutStatus.OutActive ||
                            paperStatus == PaperOutStatus.OutPassive)
                        {
                            MessageBox.Show(this,
                                "Отсутствует бумага в печатающем устройстве", Text,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // печать
                        device.Print(GetXmlDocument());
                        if (!device.ErrorCode.Succeeded)
                        {
                            MessageBox.Show(this, device.ErrorCode.FullDescription,
                                Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    });
                tester.Execute();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
    }
}