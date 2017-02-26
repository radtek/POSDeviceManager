using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using DevicesBase;
using DevicesBase.Helpers;
using DevicesCommon;
using DevicesCommon.Helpers;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace SparkTK
{
    internal enum DocumentState { None, Fiscal, NonFiscal }

    /// <summary>
    /// ������� �� "����", ���������� �� ��������� ������ ������ (��� ������� ������ ������). ������ 
    /// ������������ ����� ������������ � ������ ��������. ������� ����� (������� �������� ��� 
    /// ������������ ������� ��) ����������� � ������� ��������� ������� ��������� ��������
    /// </summary>
    [Serializable]
    [FiscalDevice(DeviceNames.ecrTypeSpark)]
    public class SparkTKFiscalDevice : CustomFiscalDevice, ISparkDeviceProvider
    {
        #region ���������

        // ������������ ����� ������ (��������)
        private const int MAX_STRING_LEN = 40;

        // ������� ������ (��.)
        private const int READ_TIMEOUT = 5000;

        // ������� ������ (��.)
        private const int WRITE_TIMEOUT = 2000;

        // ��������� �� ������ ���������
        private const string CANCEL_DOCUMENT = "�������� �����������";

        #endregion

        #region ����

        private NumberFormatInfo _currNfi = new NumberFormatInfo();

        // ���������� ��� �������� �������� ����� ��������� ���������
        private uint _documentAmount = 0;

        // ��� �������� ���������
        private DocumentType _currDocType;

        // ��������� ���������
        private bool _docOpened;

        // ��� �������
        private string _cashierName;

        private SparkProtocol _deviceProtocol = null;

        private string _versNo;

        private ulong _storedCashInDrawer = 0;

        private bool _isSparkOld = false;

        #endregion

        #region ��������

        /// <summary>
        /// ���������� ������������ ������ ������������ ����� ���������� ������ ����. ����� ��������
        /// �� ������������
        /// </summary>
        protected virtual bool IsPrim02
        {
            get { return false; }
        }

        /// <summary>
        /// ������ ������������ ����� ������������ � ������ ��������. ������� ����� (������� �������� ��� 
        /// ������������ ������� ��) ���������� �������� �� ����������. ������� ������� ����� �������� � 
        /// ������ ��������
        /// </summary>
        protected virtual bool IsSparkOld
        {
            get { return _isSparkOld; }
        }

        #endregion

        #region �����������

        public SparkTKFiscalDevice()
            : base()
        {
            _currNfi.NumberDecimalSeparator = ".";
            _currNfi.NumberGroupSeparator = "";

            AddSpecificError(0x01, "�������� ������ ���������");
            AddSpecificError(0x02, "�������� ������ ����");
            AddSpecificError(0x03, "�������� ����/�����. ���������� ���������� ���������� ����/�����");
            AddSpecificError(0x04, "�������� ����������� �����");
            AddSpecificError(0x05, "�������� ������ �������� ������");
            AddSpecificError(0x06, "��� ������� � ����� �������");
            AddSpecificError(0x07, "���������� ������� \"������ ������\"");
            AddSpecificError(0x08, "����� ���������� ������, ��� �� 24 ����");
            AddSpecificError(0x09, "��������� ������������ ����� ���������� ����");
            AddSpecificError(0x0A, "��������� ������������ ����� ���������");
            AddSpecificError(0x0B, "������������ ��������");
            AddSpecificError(0x0C, "�������� ���� ��� ���������");
            AddSpecificError(0x0D, "��� ������ ��������� ��������� ��� ������� �����������");
            AddSpecificError(0x0E, "������������ ��������� ���� ����� ������� �����");
            AddSpecificError(0x0F, "������� ������� ���������");
            AddSpecificError(0x10, "������������ ��������� ��������");
            AddSpecificError(0x11, "�������� �������� ���������� ��-�� ����������� ������");
            AddSpecificError(0x12, "��� ������� �������� ��� ���������� ��������");
            AddSpecificError(0x13, "�������� �������� ��������� ����� �� ������ ��������");
            AddSpecificError(0x14, "���������� ��������� ������������ (���� ���������� ������)");
            AddSpecificError(0x15, "���������� ��������� �������� �����");
            AddSpecificError(0x16, "������� ��� ������");
            AddSpecificError(0x17, "������������ ������ ��������");
            AddSpecificError(0x18, "������� �� ����� � ������");
            AddSpecificError(0x19, "������ ������ � �����");
            AddSpecificError(0x1A, "���������� �������� ������������");
            AddSpecificError(0x1B, "�������� ������ ������� � ��");
            AddSpecificError(0x1C, "��� ��� ���������������");
            AddSpecificError(0x1D, "��������� ����� ������������");
            AddSpecificError(0x1E, "�������� ����� ������");
            AddSpecificError(0x1F, "�������� G-����");
            AddSpecificError(0x20, "�������� ����� ���� ������");
            AddSpecificError(0x21, "������� ������");
            AddSpecificError(0x22, "������ ������");
            AddSpecificError(0x23, "�������� ��������� ���");
            AddSpecificError(0x24, "������� ����� �������� � ���������. ���������� ������� \"������������\"");
            AddSpecificError(0x25, "���������� ������� \"�������� �����\"");
            AddSpecificError(0x27, "�������� ����� ���� �������");
            AddSpecificError(0x28, "�������� ��������� ��������");
            AddSpecificError(0x29, "����� ��� �������");
            AddSpecificError(0x2B, "�������� ����");
            AddSpecificError(0x2C, "��� ����� ��� ���������� ������/������������");
            AddSpecificError(0x2D, "������ ������/������������ ��� ����������");
            AddSpecificError(0x2E, "���������� ������� ����� - ���� ������������");
            AddSpecificError(0x2F, "������ ������/������������ �� ���������");
            AddSpecificError(0x30, "���������� ������ �� ��������");
            AddSpecificError(0x31, "���� ��������� ������������ ������ � �� �����, ��� ���� ��������");
            AddSpecificError(0x32, "���������� ������������� ��");
            AddSpecificError(0x33, "��������� ��� ��");
            AddSpecificError(0x34, "������������ ��������� ������ �� ������");
            AddSpecificError(0x35, "������������ ����� �� ����");
            AddSpecificError(0x36, "����������� ������� ����");
            AddSpecificError(0x37, "�������� ��������� ����");
            AddSpecificError(0x38, "������� ������ �� ����");
            AddSpecificError(0x39, "������� �������� � ����");
            AddSpecificError(0x3A, "�������� ����������� ����� ������ ����");
            AddSpecificError(0x3B, "��������� ��������� ����");
            AddSpecificError(0x3C, "��� ���������� ����� � ����");
            AddSpecificError(0x3D, "�������� ����������� ����� � ������� ����");
            AddSpecificError(0x3E, "���������� ���� �� ���������");
            AddSpecificError(0x3F, "������ � ���� �����������");
            AddSpecificError(0x40, "������ � ���� �� ����������������");
            AddSpecificError(0x41, "��������� ��������� ���");
            AddSpecificError(0x42, "�������� ���� � ����� � ������� ����");
            AddSpecificError(0x43, "����������� ����� ������������ ����");
            AddSpecificError(0x44, "������������ ����");
            AddSpecificError(0x45, "����� ����������� ���������");
            AddSpecificError(0x50, "������������ ��������� ����������� �����");
            AddSpecificError(0x51, "��������� ���������� ����������� �����");
            AddSpecificError(0xFE, "�������� �� ������");
            AddSpecificError(0xFF, "����������� ������");
            AddSpecificError(0xFC, "������� ����� ��������");

            _deviceProtocol = new SparkProtocol(this);
            //this.Logger.SaveDebugInfo()
        }

        #endregion

        private delegate void ExecuteCommandDelegate();

        #region ���������� ������

        // ��������� ��������� ���������
        private void SetHeader()
        {
            if (DocumentHeader != null && PrintHeader)
            {
                _deviceProtocol.CreateCommand("4E");
                for (int i = 0; i < 6; i++)
                    if (i >= DocumentHeader.Length)
                        _deviceProtocol.AddString("");
                    else
                        _deviceProtocol.AddString(DocumentHeader[i].Length > MAX_STRING_LEN ? DocumentHeader[i].Substring(0, MAX_STRING_LEN) : DocumentHeader[i]);

                _deviceProtocol.Execute();
            }

            // ����������� ��������� 4F
            if (GraphicHeader != null && PrintGraphicHeader)
            {
                int width = GraphicHeader.Width / 8;
                int height = GraphicHeader.Height / 8;
                _deviceProtocol.CreateCommand("4F");
                // ��� ���������
                _deviceProtocol.AddString("00");
                // ������ �� ����������� (� ������)
                _deviceProtocol.AddString(width.ToString("X2"));
                // ������ �� ��������� (� ������)
                _deviceProtocol.AddString(height.ToString("X2"));
                // ���������� �������
                _deviceProtocol.Execute();

                // �������� ������
                foreach (byte imageByte in GetImageBytes(GraphicHeader, width, height))
                    _deviceProtocol.WriteByte(imageByte);

                // ������ ������ �� �������
                _deviceProtocol.GetCommandRequest();
            }
        }

        // ��������� ������� ���������
        private void SetFooter()
        {
            if (DocumentHeader == null)
                return;

            _deviceProtocol.CreateCommand("46", true);
            for (int i = 0; i < 4; i++)
                if (i >= DocumentFooter.Length)
                    _deviceProtocol.AddString("");
                else
                    _deviceProtocol.AddString(DocumentFooter[i].Length > MAX_STRING_LEN ? DocumentFooter[i].Substring(0, MAX_STRING_LEN) : DocumentFooter[i]);

            _deviceProtocol.Execute();
        }

        /// <summary>
        /// ��������������� ������� � ����� � ������� ���� �� ��� ��������� ������������ �����
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private IEnumerable<byte> GetImageBytes(System.Drawing.Bitmap bitmap, int width, int height)
        {
            for (int x = 0; x < width * 8; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    byte b = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        if ((uint)bitmap.GetPixel(x, y * 8 + i).ToArgb() != 0xffffffff)
                            b += (byte)(0x01 << (7 - i));
                    }
                    yield return b;
                }
            }
        }


        // �������� ����������� ���������
        private void OpenFiscalDocument()
        {
            _deviceProtocol.CreateCommand("10", true);
            _deviceProtocol.AddString(_currDocType == DocumentType.Sale ? "00" : "02");
            _deviceProtocol.AddString(_cashierName);
            _deviceProtocol.AddString("");      // ���� ������ ��� 
            _deviceProtocol.AddString("");      // ������ � ����������
            _deviceProtocol.AddString("01");    // ���������� �����
            _deviceProtocol.AddString("");      // ����� �����
            _deviceProtocol.Execute();
        }

        // ������ ���������
        private void CancelDocument(bool printerMode)
        {
            try
            {
                if (printerMode)
                    CloseNonFiscalDocument(true);
                else
                {
                    switch (_deviceProtocol.GetDeviceInfo().DocState)
                    {
                        case FiscalDocState.Closed:
                            // � ���� �������� �� ���������� � ����� �������� �� �������,
                            // �� ������ ����� ������ ��� �� ���������� � �������� ������
                            break;
                        case FiscalDocState.FreeDoc:
                            // �������� ������������� ���������
                            _deviceProtocol.ExecuteCommand("52");
                            break;
                        default:
                            // ������ ����������� ���������
                            _deviceProtocol.ExecuteCommand("17", true);
                            break;
                    }
                }
                _docOpened = false;
            }
            catch (TimeoutException)
            {
                ErrorCode = new ServerErrorCode(this, GeneralError.Timeout, _deviceProtocol.GetCommandDump());
            }
            catch (PrintableErrorException)
            {
                // ������ ����������� ����������. �������� �� ����������
                ErrorCode = new ServerErrorCode(this, 0x18, GetSpecificDescription(0x18), _deviceProtocol.GetCommandDump());
            }
            catch (DeviceErrorException E)
            {
                if (E.ErrorCode != 13)
                    ErrorCode = new ServerErrorCode(this, E.ErrorCode, GetSpecificDescription(E.ErrorCode), _deviceProtocol.GetCommandDump());
                _docOpened = false;
            }
            catch (Exception E)
            {
                ErrorCode = new ServerErrorCode(this, E);
            }
            finally
            {
                if (!ErrorCode.Succeeded && Logger.DebugInfo && !string.IsNullOrEmpty(_deviceProtocol.DebugInfo))
                    Logger.SaveDebugInfo(this, _deviceProtocol.DebugInfo);
//                _deviceProtocol.ClearDebugInfo();
            }
        }

        // �������� ������������� ���������
        private void CloseNonFiscalDocument(bool bCancelDoc)
        {
            if (bCancelDoc)
                PrintStringInternal(CANCEL_DOCUMENT, FontStyle.Regular);

            // ������ �������
            if (DocumentFooter != null)
                foreach (string s in DocumentFooter)
                    PrintStringInternal(s, FontStyle.Regular);

            // ������� ����
            _deviceProtocol.Write(new byte[] { 0x1D, Convert.ToByte('V'), 65, 0 });

            // ������ �����
            if (DocumentHeader != null)
                foreach (string s in DocumentHeader)
                    PrintStringInternal(s, FontStyle.Regular);

            _deviceProtocol.IsPrinterMode = false;
            _docOpened = false;
        }

        // �������� ��������� �������� ��� ����������
        private void ClosePayInOutDocument()
        {
            decimal fAmount = (decimal)(_documentAmount / 100.0);
            _deviceProtocol.CreateCommand(_currDocType == DocumentType.PayingIn ? "32" : "33", true);
            _deviceProtocol.AddString(fAmount.ToString("f2", _currNfi));
            _deviceProtocol.Execute();
        }

        private void PrintStringInternal(string source, FontStyle style)
        {
            if (IsPrim02) // ������ ������ � �������� ��������
            {
                // ������������� �����
                switch (style)
                {
                    case FontStyle.DoubleHeight:
                        source = "~10" + source;
                        break;
                    case FontStyle.DoubleWidth:
                        source = "~20" + source;
                        break;
                    case FontStyle.DoubleAll:
                        source = "~30" + source;
                        break;
                }

                // �������� ������ �� ������������ �����
                if (source.Length > MAX_STRING_LEN)
                    source = source.Substring(0, MAX_STRING_LEN);

                _deviceProtocol.CreateCommand((_currDocType != DocumentType.Other) && HasNonzeroRegistrations ? "1C" : "51");
                _deviceProtocol.AddString(source);
                _deviceProtocol.Execute();
            }
            else         // ������ ������ � ������ ��������
            {
                // �������� ������ �� ������������ �����
                if (source.Length > MAX_STRING_LEN)
                    source = source.Substring(0, MAX_STRING_LEN);

                // ��������� ������
                switch (style)
                {
                    case FontStyle.Regular:
                        _deviceProtocol.Write(new byte[] { 0x1B, 0x21, 0x00 });
                        break;
                    case FontStyle.DoubleHeight:
                        _deviceProtocol.Write(new byte[] { 0x1B, 0x21, 0x10 });
                        break;
                    case FontStyle.DoubleWidth:
                        _deviceProtocol.Write(new byte[] { 0x1B, 0x21, 0x20 });
                        break;
                    case FontStyle.DoubleAll:
                        _deviceProtocol.Write(new byte[] { 0x1B, 0x21, 0x30 });
                        break;
                }

                // ������ ������
                _deviceProtocol.Write(Encoding.GetEncoding(866).GetBytes(source));
                _deviceProtocol.Write(new byte[] { 0x0A });

                // ����� ������
                _deviceProtocol.Write(new byte[] { 0x1B, 0x21, 0x00 });
            }
        }

        private void ExecuteDriverCommand(ExecuteCommandDelegate executeCommandDelegate)
        {
            ExecuteDriverCommand(false, executeCommandDelegate);
        }

        private void ExecuteDriverCommand(bool printerMode, ExecuteCommandDelegate executeCommandDelegate)
        {
            ErrorCode = new ServerErrorCode(this, GeneralError.Success);
            try
            {
                if (!Active)
                {
                    ErrorCode = new ServerErrorCode(this, GeneralError.Inactive);
                    return;
                }

                if(!IsPrim02)
                    // ��������� ������� ����� 
                    if (_deviceProtocol.IsPrinterMode != printerMode)
                        // ������������� ������ �����
                        _deviceProtocol.IsPrinterMode = printerMode;

                executeCommandDelegate();
            }
            catch (TimeoutException)
            {
                ErrorCode = new ServerErrorCode(this, GeneralError.Timeout, _deviceProtocol.GetCommandDump());
                if (_deviceProtocol.IsPrinterMode)
                    _deviceProtocol.IsPrinterMode = false;
            }
            catch (PrintableErrorException)
            {
                // ������ ����������� ����������. �������� �� ����������
                ErrorCode = new ServerErrorCode(this, 0x18, GetSpecificDescription(0x18), _deviceProtocol.GetCommandDump());                
            }
            catch (DeviceErrorException E)
            {
                // ������������ ������. �������� ����� ����������� ��������
                ErrorCode = new ServerErrorCode(this, E.ErrorCode, GetSpecificDescription(E.ErrorCode), _deviceProtocol.GetCommandDump());
                CancelDocument(_deviceProtocol.IsPrinterMode);
            }
            catch (Exception E)
            {
                ErrorCode = new ServerErrorCode(this, E);
            }
            finally
            {
                if (!ErrorCode.Succeeded && Logger.DebugInfo && !string.IsNullOrEmpty(_deviceProtocol.DebugInfo))
                    Logger.SaveDebugInfo(this, _deviceProtocol.DebugInfo);
//                _deviceProtocol.ClearDebugInfo();
            }
        }

        #endregion

        #region ���������� ����������� �������

        protected override void SetCommStateEventHandler(object sender, CommStateEventArgs e)
        {
            DCB dcb = e.DCB;

            // ������ ����� ��������� ������
            dcb.XonChar = 0;
            dcb.XoffChar = 0;
            dcb.XonLim = 0;
            dcb.XoffLim = 0;

            // shake
            dcb.fDsrSensitivity = 0;
            dcb.fOutxCtsFlow = 0;
            dcb.fTXContinueOnXoff = 0;
            dcb.fInX = 0;
            dcb.fOutX = 0;
            dcb.fRtsControl = (uint)RtsControl.Disable;
            dcb.fNull = 0;

            // replace
            dcb.fDtrControl = (uint)DtrControl.Handshake;
            dcb.fOutxDsrFlow = 1;
            dcb.fAbortOnError = 1;

            e.DCB = dcb;
            e.Handled = true;

            base.SetCommStateEventHandler(sender, e);
        }

        protected override void OnAfterActivate()
        {
            Port.ReadTimeout = READ_TIMEOUT;
            Port.WriteTimeout = WRITE_TIMEOUT;

            _deviceProtocol.WriteDebugLine(string.Format("OnAfterActivate({0}, {1})", PortName, Baud));
            ExecuteDriverCommand(delegate()
            {
                _deviceProtocol.IsPrinterMode = false;

                // ������� �����
                _deviceProtocol.ExecuteCommand("01", true);

                if (!Status.OpenedShift)
                {
                    // ��������� �����
                    SetHeader();

                    // ��������� �������
                    SetFooter();
                }

                // �������� ������ ��������
                _deviceProtocol.ExecuteCommand("97");
                _versNo = Encoding.ASCII.GetString(_deviceProtocol.GetField(2));
            });
        }

        protected override void OnOpenDocument(DocumentType docType,
            string cashierName)
        {
            _deviceProtocol.ClearDebugInfo();
            _deviceProtocol.WriteDebugLine(string.Format("OnOpenDocument({0}, {1})", docType, cashierName));
            ExecuteDriverCommand(delegate()
            {
                _currDocType = docType;
                _cashierName = (cashierName.Length <= 15) ? cashierName : cashierName.Substring(0, 15);

                // ���� �������� ��� ������, ��� ���� ��������
                if (_deviceProtocol.GetDeviceInfo().DocState != FiscalDocState.Closed)
                    CancelDocument(false);

                if (IsPrim02)
                {
                    switch (docType)
                    {
                        case DocumentType.Sale:
                        case DocumentType.Refund:
                            if (HasNonzeroRegistrations)
                                OpenFiscalDocument();
                            else
                                // �������� ������������� ���������
                                _deviceProtocol.ExecuteCommand("50", true);
                            break;
                        case DocumentType.Other:
                            // �������� ������������� ���������
                            _deviceProtocol.ExecuteCommand("50", true);
                            break;
                    }
                }

                _storedCashInDrawer = Status.CashInDrawer;
                _documentAmount = 0;
                _docOpened = true;
            });
        }

        protected override void OnCloseDocument(bool cutPaper)
        {
            _deviceProtocol.WriteDebugLine(string.Format("OnCloseDocument({0})", cutPaper));
            ExecuteDriverCommand(_deviceProtocol.IsPrinterMode, delegate()
            {
                switch (_currDocType)
                {
                    case DocumentType.Sale:
                    case DocumentType.Refund:
                    case DocumentType.Other:
                        if (_deviceProtocol.IsPrinterMode)
                            CloseNonFiscalDocument(false);
                        else
                        {
                            // �������� ����������� ���������
                            _storedCashInDrawer = Status.CashInDrawer;
                            _deviceProtocol.ExecuteCommand(
                                _currDocType != DocumentType.Other && HasNonzeroRegistrations ? "14" : "52");
                        }
                        break;

                    case DocumentType.PayingIn:
                    case DocumentType.PayingOut:
                        ClosePayInOutDocument();
                        break;

                    case DocumentType.SectionsReport:
                    case DocumentType.XReport:
                        _deviceProtocol.ExecuteCommand("30", true);
                        break;
                    case DocumentType.ZReport:
                        _deviceProtocol.ExecuteCommand("31", true);
                        break;
                }

                _docOpened = false;
                _currDocType = DocumentType.Other;
                _documentAmount = 0;
            });
        }

        protected override void OnOpenDrawer()
        {
            _deviceProtocol.WriteDebugLine("OnOpenDrawer");
            ExecuteDriverCommand(!IsPrim02 && _deviceProtocol.IsPrinterMode, delegate()
            {
                _deviceProtocol.Write(new byte[] { 0x05 });
                System.Threading.Thread.Sleep(1000);
            });
        }

        protected override void OnPrintString(string source, FontStyle style)
        {
            _deviceProtocol.WriteDebugLine(string.Format("OnPrintString({0}, {1})", source, style));
            ExecuteDriverCommand(!IsPrim02, delegate()
            {
                if (_currDocType != DocumentType.PayingIn && _currDocType != DocumentType.PayingOut)
                    PrintStringInternal(source, style);
            });
        }

        protected override void OnPrintBarcode(string barcode, AlignOptions align,
            bool readable)
        {
            _deviceProtocol.WriteDebugLine(string.Format("OnPrintBarcode({0}, {1}, {2})", barcode, align, readable));
            ExecuteDriverCommand(!IsPrim02, delegate()
            {
                if (IsPrim02)
                {
                    _deviceProtocol.CreateCommand("1A");
                    _deviceProtocol.AddString("02");
                    _deviceProtocol.AddString(readable ? "02" : "00");
                    _deviceProtocol.AddString("00");
                    _deviceProtocol.AddString("46");
                    _deviceProtocol.AddString("02");
                    _deviceProtocol.AddString(barcode);
                    _deviceProtocol.Execute();
                }
                else
                {
                    // ������������
                    switch (align)
                    {
                        case AlignOptions.Left:
                            _deviceProtocol.Write(new byte[] { 0x1B, Convert.ToByte('a'), 0 });
                            break;
                        case AlignOptions.Center:
                            _deviceProtocol.Write(new byte[] { 0x1B, Convert.ToByte('a'), 1 });
                            break;
                        case AlignOptions.Right:
                            _deviceProtocol.Write(new byte[] { 0x1B, Convert.ToByte('a'), 2 });
                            break;
                    }
                    // ������ �����-����
                    _deviceProtocol.Write(new byte[] { 0x1D, Convert.ToByte('h'), 70 });
                    // ������ ����
                    _deviceProtocol.Write(new byte[] { 0x1D, Convert.ToByte('w'), 2 });
                    // �����
                    _deviceProtocol.Write(new byte[] { 0x1D, Convert.ToByte('H'), readable ? (byte)2 : (byte)0 });
                    // ��� �����-����
                    _deviceProtocol.Write(new byte[] { 0x1D, Convert.ToByte('k'), 0x43, 0x0C });
                    // ������
                    _deviceProtocol.Write(Encoding.ASCII.GetBytes(barcode));
                    // NULL
                    _deviceProtocol.Write(new byte[] { 0x00 });
                    // ����� ������������
                    _deviceProtocol.Write(new byte[] { 0x1B, Convert.ToByte('a'), 0 });
                }
            });
        }

        protected override void OnPrintImage(System.Drawing.Bitmap image, AlignOptions align)
        {
            _deviceProtocol.WriteDebugLine("OnPrintImage");
            ExecuteDriverCommand(delegate() { });
        }

        protected override void OnRegistration(string commentary, uint quantity, uint amount,
            byte section)
        {
            _deviceProtocol.WriteDebugLine(string.Format("OnRegistration({0}, {1}, {2}, {3})", commentary, quantity, amount, section));
            bool nonFiscalDoc = _currDocType == DocumentType.Other || !HasNonzeroRegistrations;
            ExecuteDriverCommand(nonFiscalDoc && !IsPrim02, delegate()
            {
                // ���� ����������� ����������� � ������� ������, �������� ��������� �� �����
                // � �� ����� � ������� ������ ���������� ��� �� ���������
                if (nonFiscalDoc)
                {
                    if (!string.IsNullOrEmpty(commentary))
                        PrintStringInternal(commentary, FontStyle.Regular);
                }
                else
                {
                    // ���� ���������� �������� �� ������, �������� ��� �������
                    if (_deviceProtocol.GetDeviceInfo().DocState == FiscalDocState.Closed)
                        OpenFiscalDocument();

                    float fPrice = (float)(amount / 100.0);
                    float fQuantity = (float)(quantity / 1000.0);

                    if (fQuantity == 0.0)
                        fQuantity = 1.0f;

                    _deviceProtocol.CreateCommand("11");
                    // �������� ������
                    commentary = commentary.Length > 40 ? commentary.Substring(0, 40) : commentary;
                    _deviceProtocol.AddString(commentary.Length == 0 ? " " : commentary);
                    // �������
                    _deviceProtocol.AddString("");
                    // ����
                    _deviceProtocol.AddString(fPrice.ToString("f2", _currNfi));
                    // ����������/���
                    _deviceProtocol.AddString(fQuantity.ToString("f3", _currNfi));
                    // ������� ���������
                    _deviceProtocol.AddString(" ");
                    // ������ ������ � ����
                    _deviceProtocol.AddString(section.ToString("d2"));
                    // ������������� ������
                    _deviceProtocol.AddString("");
                    _deviceProtocol.Execute();

                    _documentAmount += (uint)Math.Round(amount * quantity / 1000.0);
                }
            });
        }

        protected override void OnPayment(uint amount, FiscalPaymentType paymentType)
        {
            _deviceProtocol.WriteDebugLine(string.Format("OnPayment({0}, {1})", amount, paymentType));

            bool nonFiscalDoc = _currDocType == DocumentType.Other || !HasNonzeroRegistrations;
            if (!nonFiscalDoc)
                ExecuteDriverCommand(delegate()
                {
                    // �������
                    FiscalDocState cuurDocState = _deviceProtocol.GetDeviceInfo().DocState;
                    if (cuurDocState == FiscalDocState.Position)
                        _deviceProtocol.ExecuteCommand("12");

                    decimal fAmount = (decimal)(amount / 100.0);
                    _deviceProtocol.CreateCommand("13");
                    switch (paymentType)
                    {
                        case FiscalPaymentType.Cash:
                            _deviceProtocol.AddString("00");
                            break;
                        case FiscalPaymentType.Card:
                            _deviceProtocol.AddString("02");
                            break;
                        case FiscalPaymentType.Other1:
                            _deviceProtocol.AddString("03");
                            break;
                        case FiscalPaymentType.Other2:
                            _deviceProtocol.AddString("04");
                            break;
                        case FiscalPaymentType.Other3:
                            _deviceProtocol.AddString("05");
                            break;
                    }

                    _deviceProtocol.AddString(fAmount.ToString("f2", _currNfi));
                    _deviceProtocol.AddString("");  // �������� ��������� �����

                    _deviceProtocol.Execute();
                });
        }

        protected override void OnCash(uint amount)
        {
            _deviceProtocol.WriteDebugLine(string.Format("OnCash({0})", amount));
            ExecuteDriverCommand(delegate()
            {
                if (_currDocType == DocumentType.PayingIn || _currDocType == DocumentType.PayingOut)
                    _documentAmount += amount;
            });
        }

        protected override void OnContinuePrint()
        {
            _deviceProtocol.WriteDebugLine("OnContinuePrint");
            base.OnContinuePrint();

            try
            {
                // ������ ����� � �����
                _docOpened = false;

                // ���� ����� � ����� �� ����������, ������ ���������� �������� �� ��� ��������
                ulong currCashInDrawer = 0;
                int retriesCount = 3;
                do
                {
                    currCashInDrawer = Status.CashInDrawer;
                    if (!ErrorCode.Succeeded && retriesCount == 0)
                        return;
                    retriesCount--;
                }
                while (!ErrorCode.Succeeded);

                _docOpened = currCashInDrawer == _storedCashInDrawer;
                if (_docOpened)
                    _deviceProtocol.WriteDebugLine("�������� �� ��� ��������. ����� ����������� ��������� ������ ���������");
                else
                    _deviceProtocol.WriteDebugLine("�������� ��� ������� ��������");
            }
            finally
            {
                Logger.SaveDebugInfo(this, _deviceProtocol.DebugInfo);
                _deviceProtocol.ClearDebugInfo();
            }
        }

        #endregion

        #region ���������� ����������

        #region �������

        public override event EventHandler<FiscalBreakEventArgs> FiscalBreak;

        public override event EventHandler<PrinterBreakEventArgs> PrinterBreak;

        #endregion

        #region ��������

        public override DateTime CurrentTimestamp
        {
            get
            {
                _deviceProtocol.WriteDebugLine("CurrentTimestamp_get");
                DateTime currTimestamp = DateTime.Now;
                ExecuteDriverCommand(delegate()
                {
                    _deviceProtocol.ExecuteCommand("43");
                    string dateTimeStr = Encoding.ASCII.GetString(_deviceProtocol.GetField(1)) + Encoding.ASCII.GetString(_deviceProtocol.GetField(2));
                    currTimestamp = DateTime.ParseExact(dateTimeStr, "ddMMyyHHmm", null);
                });
                return currTimestamp;
            }
            set
            {
                _deviceProtocol.WriteDebugLine(string.Format("CurrentTimestamp_set({0: yy.MM.dd HH:mm:ss})", value));
                ExecuteDriverCommand(delegate()
                {
                    _deviceProtocol.ExecuteCommand("42", true);
                });
            }
        }

        public override PrintableDeviceInfo PrinterInfo
        {
            get
            {
                return new PrintableDeviceInfo(new PrintableTapeWidth(MAX_STRING_LEN, 0), true);
            }
        }

        public override FiscalDeviceInfo Info
        {
            get
            {
                _deviceProtocol.WriteDebugLine("Info");
                string SerialNo = "0";
                ExecuteDriverCommand(delegate()
                {
                    SerialNo = _deviceProtocol.GetDeviceInfo().SerialNo;
                });
                return new FiscalDeviceInfo(DeviceNames.ecrTypeSpark, SerialNo);
            }
        }

        public override PrinterStatusFlags PrinterStatus
        {
            get
            {
                _deviceProtocol.WriteDebugLine("PrinterStatus");
                bool bPrinting = false;
                PaperOutStatus poStatus = PaperOutStatus.Present;
                bool bDrawerOpened = false;
                byte statusByte = 0;

                // ��������� ��������� ����������� ����������
                try
                {
                    ErrorCode = new ServerErrorCode(this, GeneralError.Success);
                    statusByte = _deviceProtocol.ShortStatusRequest(true, 0x30, 5);
                    if ((statusByte & 0x20) == 0)
                        // �������� ��������� ������ 
                        return new PrinterStatusFlags(false, PaperOutStatus.OutActive, _docOpened, false);
                    else if (_docOpened)
                        // ��������� ������� ����������� ������
                        return new PrinterStatusFlags(false, PaperOutStatus.OutAfterActive, _docOpened, false);
                }
                catch (TimeoutException)
                {
                    ErrorCode = new ServerErrorCode(this, GeneralError.Timeout);
                }

//                return new PrinterStatusFlags(false, PaperOutStatus.Present, _docOpened, false);

                ExecuteDriverCommand(delegate()
                {
                    // ���� "������ ����������� ����������"
                    statusByte = _deviceProtocol.ShortStatusRequest(false, 0x31);
                    bDrawerOpened = (statusByte & 4) == 4;

                    // ���� "��������� �������� ������"
                    statusByte = _deviceProtocol.ShortStatusRequest(false, 0x34);
                    poStatus = ((statusByte & 64) == 64) || ((statusByte & 8) == 8) ? PaperOutStatus.OutPassive : PaperOutStatus.Present;
                });
                return new PrinterStatusFlags(bPrinting, poStatus, _docOpened, bDrawerOpened);
            }
        }

        public override FiscalStatusFlags Status
        {
            get
            {
                _deviceProtocol.WriteDebugLine("Status");
                ulong cashInDrawer = 0;
                CurrentDeviceInfo currDevInfo = new CurrentDeviceInfo();

                ExecuteDriverCommand(delegate()
                {
                    currDevInfo = _deviceProtocol.GetDeviceInfo();
                    // ����� � �������� �����
                    _deviceProtocol.ExecuteCommand("37");
                    string sCash = Encoding.ASCII.GetString(_deviceProtocol.GetField(15));
                    cashInDrawer = Convert.ToUInt64(sCash.Replace(".", ""));
                });

                return new FiscalStatusFlags(currDevInfo.OpenedShift, currDevInfo.OverShift,
                    false, currDevInfo.Fiscalized, (ulong)_documentAmount, (ulong)cashInDrawer);
            }
        }

        #endregion

        #region ������

        public override void FiscalReport(FiscalReportType reportType, bool full, params object[] reportParams)
        {
            _deviceProtocol.WriteDebugLine(string.Format("FiscalReport({0}, {1})", reportType, full));
            ExecuteDriverCommand(delegate()
            {
                switch (reportType)
                {
                    case FiscalReportType.ByDates:
                        _deviceProtocol.CreateCommand(full ? "07" : "05", true);
                        _deviceProtocol.AddString(TaxerPassword.ToString());
                        _deviceProtocol.AddString(((DateTime)reportParams[0]).ToString("ddMMyy"));
                        _deviceProtocol.AddString(((DateTime)reportParams[1]).ToString("ddMMyy"));
                        break;
                    case FiscalReportType.ByShifts:
                        _deviceProtocol.CreateCommand(full ? "08" : "06", true);
                        _deviceProtocol.AddString(TaxerPassword.ToString());
                        _deviceProtocol.AddString(((int)reportParams[0]).ToString("d4"));
                        _deviceProtocol.AddString(((int)reportParams[1]).ToString("d4"));
                        break;
                }
                _deviceProtocol.Execute();
            });
        }

        public override void Fiscalization(int newPassword, long registrationNumber, long taxPayerNumber)
        {
            _deviceProtocol.WriteDebugLine("Fiscalization");
            ExecuteDriverCommand(delegate()
            {
                _deviceProtocol.CreateCommand("04", true);
                // ������ ������
                _deviceProtocol.AddString(TaxerPassword.ToString());
                // ����� ������
                _deviceProtocol.AddString(newPassword.ToString());
                // ����� ��������������� �����
                _deviceProtocol.AddString(registrationNumber.ToString());
                // ���
                _deviceProtocol.AddString(taxPayerNumber.ToString());
                // ������ ("00" - ��������)
                _deviceProtocol.AddString("00");
                // ���������� ����� ������� � ���������� ������ ("01" - ���������)
                _deviceProtocol.AddString("01");

                _deviceProtocol.Execute();
            });
        }

        public override void GetLifetime(out DateTime firstDate, out DateTime lastDate, out int firstShift, out int lastShift)
        {
            _deviceProtocol.WriteDebugLine("GetLifetime");
            firstDate = DateTime.Now;
            lastDate = DateTime.Now;
            firstShift = 1;
            lastShift = 9999;
            ExecuteDriverCommand(delegate() { });
        }

        #endregion

        #endregion

        #region ISparkDeviceProvider Members

        public EasyCommunicationPort GetCommunicationPort()
        {
            return Port;
        }

        public bool GetIsOnlyESCPOSMode()
        {
            return IsSparkOld;
        }

        #endregion
    }

    /// <summary>
    /// ������� �� "����", ���������� �� ��������� ��������� ������ (� �������� ������ ������). ����� ��������
    /// �� ������������.
    /// </summary>
    [Serializable]
    [FiscalDevice(DeviceNames.ecrTypeSpark2)]
    public class SparkTK2FiscalDevice : SparkTKFiscalDevice
    {
        protected override bool IsPrim02
        {
            get { return true; }
        }
    }

    /// <summary>
    /// ������� �� "����", ���������� �� ��������� ����� ������ ������ (��� ������� ������ ������). ������ 
    /// ������������ ����� ������������ � ������ ��������. ������� ����� (������� �������� ��� 
    /// ������������ ������� ��) ���������� �������� �� ����������. ������� ������� ����� �������� � 
    /// ������ ��������.
    /// </summary>
    [Serializable]
    [FiscalDevice("����� 1.1")]
    public class SparkTKOldFiscalDevice : SparkTKFiscalDevice
    {
        protected override bool IsSparkOld
        {
            get { return true; }
        }
    }
}