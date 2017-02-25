using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon;
using System.IO.Ports;
using System.Collections;
using System.Xml;
using DevicesCommon.Helpers;
using System.IO;
using System.Diagnostics;
using DevicesBase.Helpers;

namespace DevicesBase
{
    #region ��������������� ������ � ������������

    /// <summary>
	/// ����� ������ � �������� ������
	/// </summary>
	internal enum DrawerOption
	{
		/// <summary>
		/// ������� �� ������ ������
		/// </summary>
		OpenBefore,

		/// <summary>
		/// ������� ����� ���������� ������
		/// </summary>
		OpenAfter,

		/// <summary>
		/// �� ���������
		/// </summary>
		OpenNever
	}

	/// <summary>
	/// �������� ������� �������
	/// </summary>
	internal class ColumnAttributes
	{
		public AlignOptions align;
		public Int32 width;
	}

	/// <summary>
	/// ��������� ������ ����� �������
	/// </summary>
	[Flags]
	internal enum GridOption
	{
		/// <summary>
		/// ����� ���
		/// </summary>
		None = 0,

		/// <summary>
		/// ������� ������� �����
		/// </summary>
		Top = 1,

		/// <summary>
		/// ������� ����� ������ � ����� �������
		/// </summary>
		Middle = 2,

		/// <summary>
		/// ������ ������� ���� �������
		/// </summary>
		Bottom = 4,

		/// <summary>
		/// ��� ����� �����
		/// </summary>
		All = 16
	}

    #endregion

    /// <summary>
	/// ������� ����� ��� ���������� ���������
	/// </summary>
	public abstract class CustomPrintableDevice : CustomSerialDevice, IPrintableDevice
	{
        // ��������� � �������
		private List<String> documentFooter;
		private List<String> documentHeader;
        private System.Drawing.Bitmap _graphicHeader;
        private System.Drawing.Bitmap _graphicFooter;

        // ����� ������ ���������� � ��������
        private Boolean _printHeader;
        private Boolean _printFooter;
        private Boolean _printGraphicHeader;
        private Boolean _printGraphicFooter;

		// ������-�����������
		private Char separator;
        // ���������� �������� � ������ ��������
        private String previousXmlData;
        // ������������ ��� ������� ������� ������
        private const String LineFeedString = " ";
        // ���� �� � ��������� ������ ���� "registration" � ��������� ������
        private Boolean hasNonzeroRegistrations;
        // ����� �������� ��� ���������
        private PrinterNumber printerNumber;

        #region ���������� ������� �������� ������

        /// <summary>
        /// ����� ��������� ����������
        /// </summary>
        protected override void OnAfterActivate()
        {
            // ��������� ����������� ���������� �������
            // �� ������ DTR/DSR
            Port.DsrFlow = PrinterInfo.DsrFlowControl;
            base.OnAfterActivate();
        }

        #endregion

        #region �����������

        /// <summary>
		/// ������� ���������� ����������
		/// </summary>
		protected CustomPrintableDevice() : base()
		{
			separator = '=';
            previousXmlData = String.Empty;
            hasNonzeroRegistrations = false;
            printerNumber = PrinterNumber.MainPrinter;
            documentFooter = new List<String>();
            documentHeader = new List<String>();
		}

		#endregion

		#region ������, ����������� � ��������

        /// <summary>
        /// ���������� ����� �������� ������� �� �������� ���� ����� 
        /// ��� �������� �������� �������� �������
        /// </summary>
        protected virtual void OnCustomTopMarginPrinted()
        {
        }

        /// <summary>
        /// ���������� ��� ������������� ������� ������� ����������� ������
        /// </summary>
        protected virtual void OnContinuePrint()
        {
        }

		/// <summary>
		/// ��������� ��� �������� ���������
		/// </summary>
		/// <param name="docType">��� ���������</param>
		/// <param name="cashierName">��� �������, ������������ ��������</param>
		protected virtual void OnOpenDocument(DocumentType docType,
            String cashierName)
		{
		}

		/// <summary>
		/// ���������� ��� �������� ���������
		/// </summary>
		/// <param name="cutPaper">������� �����</param>
		protected virtual void OnCloseDocument(bool cutPaper)
		{
		}

		/// <summary>
		/// ���������� ��� �������� ��������� �����
		/// </summary>
		protected virtual void OnOpenDrawer()
		{
		}

		/// <summary>
		/// ���������� ��� ������ ������
		/// </summary>
		/// <param name="source">������ ��� ������</param>
		/// <param name="style">����� ������ ������</param>
		protected virtual void OnPrintString(String source, FontStyle style)
		{
		}

		/// <summary>
		/// ���������� ��� ������ �����-����
		/// </summary>
		/// <param name="align">������������ ���������</param>
		/// <param name="barcode">������ ���������</param>
		/// <param name="readable">�������� ��� �� �������� �����</param>
		protected virtual void OnPrintBarcode(String barcode, AlignOptions align, 
			bool readable)
		{

		}

		/// <summary>
		/// ���������� ��� ������ �������
		/// </summary>
		/// <param name="align">������������ �������</param>
		/// <param name="image">�������</param>
		protected virtual void OnPrintImage(System.Drawing.Bitmap image, AlignOptions align)
		{
		}

		/// <summary>
		/// ���������� ��� ����������� �������� ��������
		/// </summary>
		/// <param name="amount">����� �������� ��������</param>
		/// <param name="commentary">�����������</param>
		/// <param name="quantity">���������� �������</param>
		/// <param name="section">������ ��� �����������</param>
		protected virtual void OnRegistration(String commentary, UInt32 quantity, UInt32 amount, 
			Byte section)
		{
		}

		/// <summary>
		/// ���������� ��� ������ ���������
		/// </summary>
		/// <param name="amount">����� ������</param>
		/// <param name="paymentType">��� ������</param>
		protected virtual void OnPayment(UInt32 amount, FiscalPaymentType paymentType)
		{
		}

		/// <summary>
		/// ���������� ��� �������� ����� �������� ��� �������
		/// </summary>
		/// <param name="amount">����� �������� ��� �������</param>
		protected virtual void OnCash(UInt32 amount)
		{
		}

		#endregion

		#region �������� � ������, ��������� �� ��������

        /// <summary>
        /// ����� �������� ��� ������ �������� ���������
        /// </summary>
        protected PrinterNumber PrinterNumber
        {
            get
            {
                return printerNumber;
            }
        }

		/// <summary>
		/// ������ �����
		/// </summary>
		/// <param name="lines">������</param>
		/// <param name="style">����� �����</param>
		protected void PrintStrings(String[] lines, FontStyle style)
		{
			if (lines == null || lines.Length == 0)
				return;

			foreach (String line in lines)
            {
                OnPrintString(line, style);
                if (ErrorCode.Failed)
                    break;
            }
				
		}

		/// <summary>
		/// ������ ��������� ���������
		/// </summary>
        /// <remarks>
        /// ��� ���������� ������������� ����� ��������� ����������������
        /// ��������� ���� � ������ OnAfterActivate. ������ � ����������� �����
        /// ����������� ����� �������� IPrintableDevice
        /// </remarks>
		protected void DrawHeader()
		{
            if (_printHeader && documentHeader.Count > 0)
            {
                PrintStrings(documentHeader.ToArray(), FontStyle.Regular);
            }
		}

		/// <summary>
		/// ������ ������� ���������
		/// </summary>
        /// <remarks>
        /// ��� ���������� ������������� ����� ��������� ����������������
        /// ������� ���� � ������ OnAfterActivate. ������ � ����������� �����
        /// ����������� ����� �������� IPrintableDevice
        /// </remarks>
        protected void DrawFooter()
		{
            if (_printFooter && documentFooter.Count > 0)
            {
                PrintStrings(documentFooter.ToArray(), FontStyle.Regular);
            }
		}

        /// <summary>
        /// ������ ������������ ���������
        /// </summary>
        protected void DrawGraphicHeader()
        {
            if (_printGraphicHeader && _graphicHeader != null)
            {
                OnPrintImage(_graphicHeader, AlignOptions.Center);
            }
        }

        /// <summary>
        /// ������ ������������ �������
        /// </summary>
        protected void DrawGraphicFooter()
        {
            if (_printGraphicFooter && _graphicFooter != null)
            {
                OnPrintImage(_graphicFooter, AlignOptions.Center);
            }
        }

        /// <summary>
        /// ���� �� � ��������� ������ ���� "registration" � ��������� ������.
        /// �������� ���������� � ������� ������ OnOpenDocument � �� ������ OnCloseDocument
        /// </summary>
        protected Boolean HasNonzeroRegistrations
        {
            get { return hasNonzeroRegistrations; }
        }

        /// <summary>
        /// ������ �������� �������
        /// </summary>
        /// <remarks>����� ����� ������ ������ ����������� ��������� �����������
        /// ��� ������ ������ �����, ������� �� �������</remarks>
        /// <returns>true, ���� ������ ��������� �������</returns>
        protected Boolean PrintTopMargin()
        {
            // ��������� ������� �������� �������
            if (CanPrintTopMargin)
            {
                for (int i = 0; i < PrinterInfo.TopMargin - 1; i++)
                {
                    OnPrintString(LineFeedString, FontStyle.Regular);
                    if (ErrorCode.Failed)
                        return false;
                }
                OnCustomTopMarginPrinted();
                if (ErrorCode.Failed)
                    return false;
            }
            return true;
        }

		#endregion

		#region �������� ���� � ������

		private const String badDocumentStructure = "�������� ��������� ���������. ��������� \"{0}\", ������� \"{1}\"";
		private const String badTableStructure = "��������� ��������� �������. ���������� �������� � ���� \"columns\" - {0}, ���������� � ������ - {1}";
        private const String attributeIsNotANumber = "�������� \"{0}\" �������� {1} �� �������� ������";
        private const String attributeIsOutOfRange = "�������� \"{0}\" �������� {1} ������� �� ������� ���������. ��������� System.UInt32";

        /// <summary>
        /// ������� ������ �����
        /// </summary>
        private Int32 CurrentTapeWidth
        {
            get
            {
                switch (printerNumber)
                {
                    case PrinterNumber.MainPrinter:
                        return PrinterInfo.TapeWidth.MainPrinter;
                    case PrinterNumber.AdditionalPrinter1:
                        return PrinterInfo.TapeWidth.AdditionalPrinter1;
                    default:
                        return 0;
                }
            }
        }

		/// <summary>
		/// �������� ������������ ����� ��������
		/// </summary>
		/// <param name="node">�������</param>
		/// <param name="expectedValue">��������� ��� ��������</param>
		private void ValidateElement(XmlElement node, String expectedValue)
		{
			if (String.Compare(node.Name, expectedValue, true) != 0)
				throw new XmlException(String.Format(badDocumentStructure, expectedValue, node.Name));
		}

		/// <summary>
		/// ���������� ��� ��������� �� �������� ��������
		/// </summary>
		/// <param name="xmlValue">�������� ��������</param>
		private DocumentType DocTypeFromXml(String xmlValue)
		{
			switch(xmlValue)
			{
				case "sale":
					return DocumentType.Sale;
				case "refund":
					return DocumentType.Refund;
				case "payIn":
					return DocumentType.PayingIn;
				case "payOut":
					return DocumentType.PayingOut;
                case "reportX":
                    return DocumentType.XReport;
                case "reportZ":
                    return DocumentType.ZReport;
                case "reportSections":
                    return DocumentType.SectionsReport;
				default:
					return DocumentType.Other;
			}
		}

		/// <summary>
		/// ���������� ��� ������ ��������� � ��
		/// </summary>
		/// <param name="drawerValue">�������� ��������</param>
		private DrawerOption DrawerOptionFromXml(String drawerValue)
		{
			switch(drawerValue)
			{
				case "openBefore":
					return DrawerOption.OpenBefore;
				case "openAfter":
					return DrawerOption.OpenAfter;
				default:
					return DrawerOption.OpenNever;
			}
		}

		/// <summary>
		/// ���������� ������������ ��������
		/// </summary>
		/// <param name="alignOption">�������� ��������</param>
		private AlignOptions AlignFromXml(String alignOption)
		{
			switch(alignOption)
			{
				case "right":
					return AlignOptions.Right;
				case "center":
					return AlignOptions.Center;
				default:
					return AlignOptions.Left;
			}
		}

        /// <summary>
        /// ������� ���������� ��� ��������� ����������� �������� �������� � �����
        /// </summary>
        /// <param name="fmtMessage">������ ��������� �� ������</param>
        /// <param name="xmlAttribute">��� ��������</param>
        /// <param name="value">�������� ��������</param>
        private void ThrowIntArgumentException(String fmtMessage, String xmlAttribute, String value)
        {
            // ����� � ���
            String message = String.Format(fmtMessage, value, xmlAttribute);
            Logger.WriteEntry(message, EventLogEntryType.Error);

            // ������� ����������
            throw new ArgumentOutOfRangeException(xmlAttribute, value, message);
        }

		/// <summary>
		/// ���������� ����� ����� �� �������� ��������
		/// </summary>
        /// <param name="lineEntry">������ ���������</param>
        /// <param name="xmlAttribute">�������, �������� �������� ����� ������������� � �����</param>
        /// <param name="defaultValue">�������� �� ���������, ���� ������� �� ������</param>
		private UInt32 IntFromXml(XmlElement lineEntry, String xmlAttribute, UInt32 defaultValue)
		{
            // �������� �������� ��������
            String intValue = lineEntry.GetAttribute(xmlAttribute);

            // ���� ������� �����������, ���� ��� �������� - ������ ������
            if (String.IsNullOrEmpty(intValue))
                // ���������� �������� �� ���������
                return defaultValue;
            else
            {
                // �������� ������������� ��� � �����
                try
                {
                    return Convert.ToUInt32(intValue);
                }
                catch (FormatException)
                {
                    ThrowIntArgumentException(attributeIsNotANumber, xmlAttribute, intValue);
                    return 0;
                }
                catch (OverflowException)
                {
                    ThrowIntArgumentException(attributeIsOutOfRange, xmlAttribute, intValue);
                    return 0;
                }
            }
		}

		/// <summary>
		/// ���������� 
		/// </summary>
		/// <param name="sectionValue"></param>
		/// <returns></returns>
		private Byte SectionFromXml(String sectionValue)
		{
            if (String.IsNullOrEmpty(sectionValue))
                return 1;
            else
            {
                Byte section = Convert.ToByte(sectionValue);
                if (section > 0 && section < 99)
                    return section;
                else
                    throw new DeviceManagerException(String.Format("����� ������ ��� ���������", section));
            }
		}

		/// <summary>
		/// ���������� ��� ������ �� �������� ��������
		/// </summary>
		/// <param name="paymentTypeValue"></param>
		/// <returns></returns>
		private FiscalPaymentType PaymentTypeFromXml(String paymentTypeValue)
		{
			switch(paymentTypeValue)
			{
				case "card":
					return FiscalPaymentType.Card;
				case "other1":
					return FiscalPaymentType.Other1;
				case "other2":
					return FiscalPaymentType.Other2;
				case "other3":
					return FiscalPaymentType.Other3;
				default:
					return FiscalPaymentType.Cash;
			}
		}

		private const Char Space = ' ';

        /// <summary>
        /// ���������, �������� �� �������� ������ amp-�������
        /// </summary>
        /// <param name="source">�������� ������</param>
        /// <param name="parts">����� � ������ ����� amp-������</param>
        /// <returns>True, ���� �������� ������ �������� amp-�������</returns>
        private Boolean IsAmpString(String source, out String[] parts)
        {
            String amp = "##";
            if (String.IsNullOrEmpty(source))
            {
                parts = null;
                return false;
            }
            else
            {
                // ��������� ������� ���������� ##
                Int32 ampIndex = source.IndexOf(amp);
                if (ampIndex == -1)
                {
                    // �������� ������ �� �������� amp-�������
                    parts = null;
                    return false;
                }
                else
                {
                    // �������� ������ �������� �������� amp-�������
                    parts = source.Split(new String[] { amp }, 
                        StringSplitOptions.RemoveEmptyEntries);
                    return parts.Length > 1;
                }
            }
        }

		/// <summary>
		/// ������������ ������ �� ������� �������� ������
		/// </summary>
		/// <param name="source">�������� ������</param>
		/// <param name="align">������������</param>
		/// <param name="width">������ �������</param>
		/// <param name="isBold">������ �����</param>
		/// <param name="lastSpace">�������� ��������� ������ �� ������</param>
		private String PrepareString(String source, AlignOptions align, Int32 width, 
            bool isBold, bool lastSpace)
		{
			StringBuilder dest = new StringBuilder();
			int spacesCount;

			if (isBold)
				width = width / 2;

			if (lastSpace)
				width--;

            String[] ampStrParts;
            if (IsAmpString(source, out ampStrParts))
            {
                // amp-������
                // ������� ���������� ��������
                spacesCount = width - (ampStrParts[0].Length + ampStrParts[1].Length);

                if (spacesCount <= 0)
                {
                    // ������� ������� ������ ��� �������� ������ ������� �����
                    var longStr = String.Concat(ampStrParts[0], Space, ampStrParts[1]);

                    switch (align)
                    {
                        case AlignOptions.Right:
                            // �������� ������ �����
                            dest.Append(longStr.Substring(longStr.Length - width));
                            break;
                        default:
                            // �������� ������ ������
                            dest.Append(longStr.Substring(0, width));
                            break;
                    }
                }
                else
                {
                    // ������ �����
                    dest.Append(ampStrParts[0]);
                    dest.Append(new String(Space, spacesCount));
                    dest.Append(ampStrParts[1]);
                }
            }
            else
            {
                // ������� ������
                if (source.Length >= width)
                {
                    // ������������ ����������
                    if (width > 0)
                        dest.Append(source, 0, width);
                }
                else
                    // ����������� �����
                    switch (align)
                    {
                        case AlignOptions.Center:
                            // ������������ �� ������
                            spacesCount = (width - source.Length) / 2;
                            dest.Append(Space, spacesCount);
                            dest.Append(source);
                            dest.Append(Space, spacesCount);
                            if (dest.Length < width)
                                dest.Append(new String(Space, width - dest.Length));
                            break;
                        case AlignOptions.Right:
                            // ������������ �� ������� ����
                            spacesCount = width - source.Length;
                            dest.Append(Space, spacesCount);
                            dest.Append(source);
                            break;
                        default:
                            // ������������ �� ������ ����
                            spacesCount = width - source.Length;
                            dest.Append(source);
                            dest.Append(Space, spacesCount);
                            break;
                    }
            }

			if (lastSpace)
				dest.Append(Space);
			return dest.ToString();
		}

		/// <summary>
		/// ��������� ����� ������ �� �������� ��������
		/// </summary>
		/// <param name="fontStyleValue">�������� ��������</param>
		private FontStyle FontStyleFromXml(String fontStyleValue)
		{
			switch(fontStyleValue)
			{
				case "doubleAll":
                    // ���������, ������������ �� ������� ����� ��������� ������
                    if (PrinterInfo.SupportsBoldFont)
                        // ��������������
                        return FontStyle.DoubleAll;
                    else
                        // �� ��������������
                        return FontStyle.Regular;

				case "doubleWidth":
                    if (PrinterInfo.SupportsBoldFont)
                        return FontStyle.DoubleWidth;
                    else
                        return FontStyle.Regular;
				case "doubleHeight":
					return FontStyle.DoubleHeight;
				default:
					return FontStyle.Regular;
			}
		}

		/// <summary>
		/// ������ �������
		/// </summary>
		/// <param name="tableEntry">XML-�������, �������������� �������</param>
		private void PrintTable(XmlElement tableEntry)
		{
			FontStyle tableStyle = FontStyleFromXml(tableEntry.GetAttribute("style"));

			// ���������� ����� �������
			XmlElement columns = tableEntry["columns"];
			List<ColumnAttributes> attribsList = new List<ColumnAttributes>();

			StringBuilder currentLine = new StringBuilder();
			Int32 
				// ���������� �������, ��� ������� �� ������ ������
				zeroColumns = 0, 
				// ��������� �������� ������ �������
				totalSetWidth = 0, 
				// ����������� ������ �������, ��� ������� �� ������ ������
				zeroWidth = 0;

			foreach (XmlElement column in columns)
			{
				// ������������
				ColumnAttributes attribs = new ColumnAttributes();
				attribs.align = AlignFromXml(column.GetAttribute("align"));

				// ������
				if (column.HasAttribute("width"))
				{
					// ������ ���������� ������
					attribs.width = Convert.ToInt32(column.GetAttribute("width"));
					totalSetWidth += attribs.width;
				}
				else 
				{
					if (column.HasAttribute("relativeWidth"))
					{
						// ������ ������������� ������
						// ���� ����� ������, ��������� ������ � ��� ����
                        Int32 tapeWidth = 
                            tableStyle == FontStyle.DoubleAll || 
                            tableStyle == FontStyle.DoubleWidth ? 
                            CurrentTapeWidth / 2 : CurrentTapeWidth;

						attribs.width = tapeWidth * Convert.ToInt32(column.GetAttribute("relativeWidth")) / 100;
						totalSetWidth += attribs.width;
					}
					else
						// ����������� ����� �������, ��� ������� �� ������ ������
						zeroColumns++;
				}
				attribsList.Add(attribs);
			}

			// ������ ��� ������� � ������� ������� ���������� ��
			if (zeroColumns > 0)
                zeroWidth = (CurrentTapeWidth - totalSetWidth) / zeroColumns;
			
			// ��������� ������ ��������� � ����������� ������ ��� ��� �������,
			// � ������� ��� ���� ����� ����
			for (int i = 0; i < columns.ChildNodes.Count; i++)
			{
				ColumnAttributes attribs = attribsList[i];
				if (attribs.width == 0)
					attribs.width = zeroWidth;

				// ��������� ��������� ������� � ������
				XmlNode column = columns.ChildNodes[i];
				currentLine.Append(PrepareString(column.InnerText, attribs.align, attribs.width,
					false, column != column.ParentNode.LastChild));
			}

			// ����� �������
			GridOption grid = GridOption.None;
			String gridLine = String.Empty;
			if (tableEntry.HasAttribute("grid"))
			{
				// ����� ����
				grid = GridOptionFromXml(tableEntry.GetAttribute("gridOptions"));
				gridLine = BuildCustomSeparator(tableEntry.GetAttribute("grid"));

				// ���������, ����� �� �������� ������� ������� �����
				if ((grid & GridOption.All) == GridOption.All || (grid & GridOption.Top) == GridOption.Top)
                {
                    OnPrintString(gridLine, FontStyle.Regular);
                    if (ErrorCode.Failed)
                        return;
                }
			}

            String phAttr = tableEntry.GetAttribute("printHeader");
            if (String.IsNullOrEmpty(phAttr) || String.Compare(phAttr, "true", true) == 0)
            {
			    // ������� ��������� �������
                if (!PrintCurrentLine(currentLine, tableStyle))
                    return;
            }

			// ���������, ����� �� �������� ������� ����� ������ � ����� �������
			if ((grid & GridOption.All) == GridOption.All || (grid & GridOption.Middle) == GridOption.Middle)
            {
                OnPrintString(gridLine, FontStyle.Regular);
                if (ErrorCode.Failed)
                    return;
            }

			// ������ �����
			foreach (XmlElement row in tableEntry["rows"])
			{
				// ��������� ���������� ����� � ��������� ������ �������
				if (row.ChildNodes.Count != columns.ChildNodes.Count)
					throw new XmlException(String.Format(badTableStructure, 
						columns.ChildNodes.Count, row.ChildNodes.Count));

				// ��������� ��������� ������
				for (int i = 0; i < row.ChildNodes.Count; i++)
				{
					XmlNode currentNode = row.ChildNodes[i];
					currentLine.Append(PrepareString(currentNode.InnerText, attribsList[i].align, 
						attribsList[i].width, false, currentNode != currentNode.ParentNode.LastChild));
				}

				// ����� �� ������ ��������� ������
                if (!PrintCurrentLine(currentLine, tableStyle))
                    return;
			}

			// ���������, ����� �� �������� ������ ������� ���� �������
			if ((grid & GridOption.All) == GridOption.All || (grid & GridOption.Bottom) == GridOption.Bottom)
                OnPrintString(gridLine, FontStyle.Regular);
		}

		/// <summary>
		/// ����������� ��������� ������ �����
		/// </summary>
		/// <param name="gridOptionValue"></param>
		/// <returns></returns>
		private GridOption GridOptionFromXml(String gridOptionValue)
		{
			if (String.IsNullOrEmpty(gridOptionValue))
				return GridOption.All;

			String[] options = gridOptionValue.Split('|');
			GridOption result = GridOption.None;
			foreach (String option in options)
			{
				switch(option)
				{
					case "top":
						result = result | GridOption.Top;
						break;
					case "middle":
						result = result | GridOption.Middle;
						break;
					case "bottom":
						result = result | GridOption.Bottom;
						break;
					default:
						result = result | GridOption.All;
						break;
				}
			}
			return result;
		}

		/// <summary>
		/// ������ ������� ������ �������
		/// </summary>
		/// <param name="currentLine">������� ������</param>
		/// <param name="tableStyle">����� ������</param>
		private bool PrintCurrentLine(StringBuilder currentLine, FontStyle tableStyle)
		{
            if (currentLine.Length > CurrentTapeWidth)
                OnPrintString(currentLine.ToString().Substring(0, CurrentTapeWidth), tableStyle);
			else
				OnPrintString(currentLine.ToString(), tableStyle);
			currentLine.Length = 0;
            return ErrorCode.Succeeded;
		}

		/// <summary>
		/// ���������� ������-�����������, �������������� �� ��������� �������
		/// </summary>
		/// <param name="source">������</param>
		private String BuildCustomSeparator(String source)
		{
			if (String.IsNullOrEmpty(source))
				// ����������� ����������� �� ���������� ����������
                return new String(Separator, CurrentTapeWidth);

			// ����������� ����� ��������
            if (source.Length >= CurrentTapeWidth)
				// ������ ������� �������
                return source.Substring(0, CurrentTapeWidth);

			// ������ ��������� � ������ ����� ������ ����
			StringBuilder separator = new StringBuilder();
            while (separator.Length + source.Length <= CurrentTapeWidth)
				separator.Append(source);

			// ��������, ���� �� ��������� ����� � ����� ������
            if (separator.Length < CurrentTapeWidth)
                separator.Append(source.Substring(0, CurrentTapeWidth - separator.Length));

			return separator.ToString();
		}

        /// <summary>
        /// ����� �� �������� ������� ������
        /// </summary>
        private Boolean CanPrintTopMargin
        {
            get
            {
                // ������� ������ (���� �����), ����������:
                // 1) ���� ������� ������� - �� ������� �����
                // 2) ���� ������� ���������� - �� ���������� ���������
                // 3) ���� ������� ��������������� - ������ �� �� ������� �����

                if (PrinterInfo.TopMargin == 0)
                    // ������ �� �����
                    return false;

                switch (PrinterInfo.Kind)
                {
                    case PrinterKind.Combo:
                        return printerNumber != PrinterNumber.MainPrinter; 
                    default:
                        return true;
                }
            }
        }

		/// <summary>
		/// ������ ���������� ���������
		/// </summary>
		/// <param name="docEntry">��������</param>
		private bool PrintDocumentEntry(XmlElement docEntry)
		{
            // �������� ������� ������
            if (!PrintTopMargin())
                return false;

			// ���� �� ���� ������� ���������
			foreach(XmlElement lineEntry in docEntry)
			{
				// �������� ������ ������
				String lineData = lineEntry.InnerText;

				// ���������� ��� ������
				switch (lineEntry.GetAttribute("type"))
				{
					case "separator":
						// ������-�����������
						OnPrintString(BuildCustomSeparator(lineData), FontStyle.Regular);
						break;

					case "barcode":
						// �����-���
						String isReadable = lineEntry.GetAttribute("readable");
						OnPrintBarcode(lineData, AlignFromXml(lineEntry.GetAttribute("align")),
							isReadable == "true" || String.IsNullOrEmpty(isReadable));
						break;

					case "image":
						// �������
                        Byte[] imageBytes = Encoding.Default.GetBytes(lineData);
                        using(MemoryStream imageStream = new MemoryStream(imageBytes))
                        {
                            OnPrintImage(new System.Drawing.Bitmap(imageStream), 
                                AlignFromXml(lineEntry.GetAttribute("align")));
                        }
						break;

					case "registration":
						// �����������
						OnRegistration(lineData,
                            IntFromXml(lineEntry, "quantity", 1000), 
							IntFromXml(lineEntry, "amount", 0),
							SectionFromXml(lineEntry.GetAttribute("section")));
						break;

					case "payment":
						// ������
						OnPayment(IntFromXml(lineEntry, "amount", 0),
							PaymentTypeFromXml(lineEntry.GetAttribute("paymentType")));
						break;

					case "cash":
						// �������� ��� �������
						OnCash(IntFromXml(lineEntry, "amount", 0));
						break;

					case "table":
						// �������
						PrintTable(lineEntry);
						break;

					default:
						// ������ �����
						FontStyle style = FontStyleFromXml(lineEntry.GetAttribute("style"));
                        
                        String s = PrepareString(
                            lineData, 
                            AlignFromXml(lineEntry.GetAttribute("align")),
                            CurrentTapeWidth, 
                            style == FontStyle.DoubleAll || style == FontStyle.DoubleWidth, 
                            false);

						OnPrintString(s, style);
						break;
				}

                // ���� ������ ��������� ������ ����������� ��������,
                // ��������� ������ ��������� � �������
                if (ErrorCode.Failed)
                    return false;
			}

            // ���������� ��������� ������
            return true;
		}

        /// <summary>
        /// �������� ������������� ����� ���� "registration" c ��������� ������
        /// </summary>
        /// <param name="docEntry">�������� ������� ���������</param>
        private void InitHasNonzeroRegistrations(XmlElement docEntry)
        {
            hasNonzeroRegistrations = false;

			// ���� �� ���� ������� ���������
            foreach (XmlElement lineEntry in docEntry)
            {
                // ��������� ������������ ����� ������
                ValidateElement(lineEntry, "line");

                // ���������� ��� ������
                if (lineEntry.GetAttribute("type") == "registration")
                {
                    // ��������� �����
                    if (IntFromXml(lineEntry, "amount", 0) != 0)
                    {
                        // ���� ����������� � ��������� ������
                        hasNonzeroRegistrations = true;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// ������������� ���� "����� ��������"
        /// </summary>
        /// <param name="xmlValue">�������� ��������, ����������� �� XML-���������</param>
        private void InitPrinterNumber(String xmlValue)
        {
            switch (xmlValue)
            {
                case "additionalPrinter1":
                    printerNumber = PrinterNumber.AdditionalPrinter1;
                    break;
                case "additionalPrinter2":
                    printerNumber = PrinterNumber.AdditionalPrinter2;
                    break;
                default:
                    printerNumber = PrinterNumber.MainPrinter;
                    break;
            }
        }

        #endregion

		#region ���������� IPrintableDevice

        #region ����� � ������ ���������

        /// <summary>
        /// ��������� ���������
        /// </summary>
        public String[] DocumentHeader
        {
            get
            {
                return documentHeader.ToArray();
            }
            set
            {
                documentHeader.Clear();
                if (value != null && value.Length > 0)
                    documentHeader.AddRange(value);
            }
        }

        /// <summary>
        /// ������ ���������
        /// </summary>
        public String[] DocumentFooter		
        {
            get
            {
                return documentFooter.ToArray();
            }
            set
            {
                documentFooter.Clear();
                if (value != null && value.Length > 0)
                    documentFooter.AddRange(value);
            }
		}

        /// <summary>
        /// ����������� ��������� ���������
        /// </summary>
        public System.Drawing.Bitmap GraphicHeader 
        { 
            get { return _graphicHeader; }
            set { _graphicHeader = value; }
        }

        /// <summary>
        /// ����������� ������ ���������
        /// </summary>
        public System.Drawing.Bitmap GraphicFooter 
        { 
            get { return _graphicFooter; }
            set { _graphicFooter = value; }
        }

        /// <summary>
        /// �������� ����������� ��������� ����
        /// </summary>
        public Boolean PrintGraphicHeader 
        {
            get { return _printGraphicHeader; }
            set { _printGraphicHeader = value; }
        }

        /// <summary>
        /// �������� ����������� ������ ���������
        /// </summary>
        public Boolean PrintGraphicFooter 
        { 
            get { return _printGraphicFooter; }
            set { _printGraphicFooter = value; }
        }

        /// <summary>
        /// �������� ��������� ���������
        /// </summary>
        public Boolean PrintHeader 
        { 
            get { return _printHeader; }
            set { _printHeader = value; }
        }

        /// <summary>
        /// �������� ������ ���������
        /// </summary>
        public Boolean PrintFooter 
        { 
            get { return _printFooter; }
            set { _printFooter = value; }
        }

        #endregion

		/// <summary>
		/// ������ ���������
		/// </summary>
		/// <param name="xmlData">������ XML-���������</param>
		public virtual void Print(String xmlData)
		{
			DrawerOption drwOption;
            DocumentType docType;
            bool printResult = false;

            // ���� ������� ����� �������
            if (Logger.DebugInfo)
            {
                // ��������� ���������� ����������
                Logger.WriteEntry(this, "���������� ���������� ����������", EventLogEntryType.Information);
                Logger.SaveDebugInfo(this, xmlData);
            }

            // ��������� ������ ����������
            if (!Active)
            {
                // ���������� �� �������
                ErrorCode = new ServerErrorCode(this, GeneralError.Inactive);
                return;
            }

            // ��������, �� ��������� �� ��� ������� �������� � ����������,
            // ������������ �� ������
            if (String.Compare(xmlData, previousXmlData, false) == 0 && 
                PrinterStatus.PaperOut == PaperOutStatus.OutAfterActive)
            {
                // ������ ������� ����������� ������
                Logger.WriteEntry(this, "����������� ������ ����������� ���������", EventLogEntryType.Information);
                OnContinuePrint();
                // ��������, ���� �� �������� ��������
                if (!PrinterStatus.OpenedDocument)
                    // ��������� ��������� ���, ������, ��� ��������� ��������,
                    // �������� ������� ���� ����������
                    // �������
                    return;
            }

			XmlDocument document = new XmlDocument();
            document.LoadXml(xmlData);
            Logger.WriteEntry(this, "������ ��������� ���������", EventLogEntryType.Information);

			// ��������� �������
			XmlElement root = document.DocumentElement;
			ValidateElement(root, "documents");
			foreach(XmlElement docEntry in root)
			{
				// ��������� ��������
                Logger.WriteEntry(this, "������ ������ ���������", EventLogEntryType.Information);
                ValidateElement(docEntry, "document");
                // ���������� ��� ���������
                docType = DocTypeFromXml(docEntry.GetAttribute("type"));
                // �������������� ����� ��������
                InitPrinterNumber(docEntry.GetAttribute("printer"));
                // �������������� �������� HasNonzeroRegistrations
                InitHasNonzeroRegistrations(docEntry);
                // ��������� ��������
                OnOpenDocument(docType, docEntry.GetAttribute("cashier"));
                if (ErrorCode.Failed)
                    // ���� �������� ��������� ��������� � �������, ��������� ������ 
                    // ���� ���������� �� ��������
                    break;
				try
				{
					drwOption = DrawerOptionFromXml(docEntry.GetAttribute("drawer"));
					if (drwOption == DrawerOption.OpenBefore)
                    {
                        // ��������� �� �� ������ ���������
                        OnOpenDrawer();
                        printResult = ErrorCode.Succeeded;
                        if (!printResult)
                            break;
                    }

					// ������ ���� ���������
                    printResult = PrintDocumentEntry(docEntry);
                    if (!printResult)
                        break;

                    if (drwOption == DrawerOption.OpenAfter)
                    {
                        // ��������� �� ����� ������ ���������
                        OnOpenDrawer();
                        printResult = ErrorCode.Succeeded;
                        if (!printResult)
                            break;
                    }
				}
				finally
				{
					// ��������� ��������
                    if (printResult)
                    {
                        // ����������, ����� �� ���������� ������� ����
                        String cutAttribute = docEntry.GetAttribute("cut");
                        OnCloseDocument(cutAttribute == "true" || String.IsNullOrEmpty(cutAttribute));
                    }
				}
                Logger.WriteEntry(this, "������ ��������� ������� ���������", EventLogEntryType.Information);
			}

            // ��������� ��������
            previousXmlData = xmlData;
		}

		/// <summary>
		/// �������, ����������� ��� ������������� ������� ������������
		/// �� ��������� ����������� ����������
		/// </summary>
		public abstract event EventHandler<PrinterBreakEventArgs> PrinterBreak;

		/// <summary>
		/// ���������� �������������� ����������� ����������
		/// </summary>
		public abstract PrintableDeviceInfo PrinterInfo { get; }

		/// <summary>
		/// ����� ��������� ��������
		/// </summary>
		public abstract PrinterStatusFlags PrinterStatus { get; }

		/// <summary>
		/// ������-����������� ���������� ������ ���������
		/// </summary>
		public char Separator
		{
			get { return separator;	}
			set { separator = value; }
		}

        /// <summary>
        /// �������� ��������� �����
        /// </summary>
        public void OpenDrawer()
        {
            OnOpenDrawer();
        }

        #endregion
	}
}
