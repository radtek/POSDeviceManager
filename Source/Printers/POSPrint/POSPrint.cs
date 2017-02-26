using System;
using System.Collections.Generic;
using System.Text;
using DevicesBase;
using DevicesBase.Helpers;
using DevicesCommon;
using DevicesCommon.Helpers;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace POSPrint
{
    [Serializable]
    [FiscalDeviceAttribute(DeviceNames.ecrTypePilot)]
    public class POSPrintFiscalDevice : CustomFiscalDevice
    {
        #region ���������

        // ������������ ����� ������
        private const int MAX_STRING_LEN = 48;

        private const int READ_TIMEOUT = 500;
        private const int WRITE_TIMEOUT = 500;

        // ������ ���������
        private const string OPERATOR_PASSWD = "0000";

        #endregion

        #region ���������� ����������

        // ��� ������
        private short _errorCode = 0;

        // ������� ��� ���������
        DocumentType m_CurrentDocType = DocumentType.Other;

        #endregion

        #region �����������

        public POSPrintFiscalDevice()
            : base()
        {
            AddSpecificError(1, "����������� �������. ����� �������� ��� �������");
            AddSpecificError(2, "������� ��������� � ������ ������ ��. ������� ��������� ������� ������� �� ��������� ��� ������ ��������� �������������� ���");
            AddSpecificError(3, "������� ��������� � ������ ��������� ��");
            AddSpecificError(4, "������ ������ � ��� ��");
            AddSpecificError(5, "������ ������ � NVR ��");
            AddSpecificError(6, "������ ������ ��� ��");
            AddSpecificError(7, "������ ������ NVR ��");
            AddSpecificError(8, "������ ��������");
            AddSpecificError(12, "�������� ��������� �������. ������ ���������� �������� ������� ���������� �� ���������� �������");
            AddSpecificError(13, "������������ ������ ��� �������� �����. �������� ����� �������� ����");
            AddSpecificError(14, "�������� ������ �������. ���������� ���������� � ������ ������� ���������� �� ����������");
            AddSpecificError(15, "�������� ������. ������ ��������� � ������� ���������� �� �������������� � ���");
            AddSpecificError(16, "�� ���������� ������ ��� ������������. �������� ����� ������� ��� ��������������� ��� ��� ����������� ����");
            AddSpecificError(17, "������������ �������� �����. � ���������� ������� ��������� ������� ������������ ��������������� ������� ���");
            AddSpecificError(18, "������������ ����� ���������. � ���������� ������� ��������� ������� ���������� ��������������� ������� ���������");
            AddSpecificError(19, "��������� ����������������� �����");
            AddSpecificError(21, "������ ����� ��");
            AddSpecificError(22, "������, ���� �������������� ���� �� ��������� � �������������� ��� ������� SETTIMER");
            AddSpecificError(23, "��� �������� ����� ������ ������� SETTIMER. �������� ��� ��� ���� ��������� ������� SETTIMER � ���� �� ������ ����������� ��� ���� ������������ ��� ���������� �������");
            AddSpecificError(24, "���������� ����� ����������. �������� ��� ��������� ������ �� ������ ����������� � ��� ����� � ������� ������� FLUSHBUFFER");
            AddSpecificError(25, "���������� ��������� ������� REPEAT � ��� ������������� �������");
            AddSpecificError(26, "��� ����������� ��������� ������� (��������� ��-�� ��������)");
            AddSpecificError(27, "��� ����� ��� ����");
            AddSpecificError(28, "��� ����� ��� ������ ����������� ����");
            AddSpecificError(29, "������� �� ��������� � �������� (���������) ���������");
            AddSpecificError(30, "�� �� �������������");
            AddSpecificError(31, "�� ���������� ��������� ����� ���");
            AddSpecificError(34, "�������� �� ������");
            AddSpecificError(35, "����� �� �������");
            AddSpecificError(36, "����� �� �����������");
            AddSpecificError(37, "���� ��� �� �����������");
            AddSpecificError(38, "������� �� ���������");
            AddSpecificError(39, "� �� ��� ����� ��� ������ ����");
            AddSpecificError(40, "���������� ������ � ��");
            AddSpecificError(41, "���������� ������ ����");
            AddSpecificError(42, "�� �������������");
            AddSpecificError(43, "��������� ����� ��� ����������");
            AddSpecificError(45, "��� �����������");
            AddSpecificError(46, "�������� ������");
            AddSpecificError(47, "����� �������");
            AddSpecificError(81, "������ � ���������� ����");
            AddSpecificError(82, "��������� ��������� ����");
            AddSpecificError(83, "��������� ��������� ����");
            AddSpecificError(84, "������ ���������������� ����");
            AddSpecificError(85, "�������� ��������� ����� ����");
            AddSpecificError(86, "���� �����������");
            AddSpecificError(87, "�������� ����/����� � ���������� ����");
            AddSpecificError(88, "��� ������ ��� ������");
            AddSpecificError(89, "������������ ��������� ����");
            AddSpecificError(91, "���� �� ��������������");
            AddSpecificError(92, "����� ���� ������");
            AddSpecificError(93, "���� �������������� �� �� ���� ���");
            AddSpecificError(94, "���� ��� ��������������");
            AddSpecificError(95, "�������� ������ ���� (��������� ��� ��������� �����)");
            AddSpecificError(99, "���� offline");
        }

        #endregion

        #region ���������� ������

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

                if (_errorCode < 0x1000)
                    ErrorCode = new ServerErrorCode(this, _errorCode, GetSpecificDescription(_errorCode));
                else
                    ErrorCode = new ServerErrorCode(this, (GeneralError)_errorCode);
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

        // ������ �������
        private short PrintReport(DocumentType docType)
        {
            POSCommand Cmd = null;

            switch (docType)
            {
                case DocumentType.XReport:
                case DocumentType.SectionsReport:
                    Cmd = new POSCommand(218);
                    Cmd.AddChar(OPERATOR_PASSWD, 4);
                    break;

                case DocumentType.ZReport:
                    Cmd = new POSCommand(204);
                    Cmd.AddChar(OPERATOR_PASSWD, 4);
                    Cmd.AddNumeric(1, 2);
                    break;
            }

            return Cmd.Execute(Port);
        }

        // ������ ���������
        private short CancelDocument()
        {
            POSCommand Cmd = new POSCommand(216);   // �������� ��������
            Cmd.AddChar(OPERATOR_PASSWD, 4);       // ������

            return Cmd.Execute(Port);
        }

        // ����������� ������
        private bool ContinuePrint()
        {
            POSCommand Cmd = new POSCommand(405);   // ������ �������
            Cmd.AddChar(OPERATOR_PASSWD, 4);       // ������
            _errorCode = Cmd.Execute(Port);
            return _errorCode == (short)GeneralError.Success;
        }

        // ��������� ����� ���������
        private bool SetDocumentHeader()
        {
            if (DocumentHeader == null)
                return true;

            POSCommand Cmd = null;

            // ������� �����
            Cmd = new POSCommand(201);
            Cmd.AddChar(OPERATOR_PASSWD, 4);
            _errorCode = Cmd.Execute(Port);
            if (_errorCode != (short)GeneralError.Success)
                return false;

            // ��������� ����� �����
            for (int i = 0; i < DocumentHeader.Length; i++)
            {
                Cmd = new POSCommand(202);   // ���������� ������ �����
                Cmd.AddChar(OPERATOR_PASSWD, 4);
                Cmd.AddNumeric(i + 1, 2);
                Cmd.AddNumeric(1, 2);
                if (DocumentHeader[i].Length > MAX_STRING_LEN)
                    Cmd.AddBChar(DocumentHeader[i].Substring(0, MAX_STRING_LEN));
                else
                    Cmd.AddBChar(DocumentHeader[i]);
                _errorCode = Cmd.Execute(Port);

                if (_errorCode != (short)GeneralError.Success)
                    break;
            }
            return _errorCode == (short)GeneralError.Success;
        }

        // ��������� ������� ���������
        private bool SetDocumentFooter()
        {
            if (DocumentFooter == null)
                return true;

            POSCommand Cmd = null;

            // ������� �������
            Cmd = new POSCommand(233);
            Cmd.AddChar(OPERATOR_PASSWD, 4);
            _errorCode = Cmd.Execute(Port);
            if (_errorCode != (short)GeneralError.Success)
                return false;

            // ��������� ����� �������
            for (int i = 0; i < DocumentFooter.Length; i++)
            {
                Cmd = new POSCommand(234);   // ���������� ������ �������
                Cmd.AddChar(OPERATOR_PASSWD, 4);
                Cmd.AddNumeric(i + 1, 2);
                Cmd.AddNumeric(1, 2);
                if (DocumentFooter[i].Length > MAX_STRING_LEN)
                    Cmd.AddBChar(DocumentFooter[i].Substring(0, MAX_STRING_LEN));
                else
                    Cmd.AddBChar(DocumentFooter[i]);
                _errorCode = Cmd.Execute(Port);

                if (_errorCode != (short)GeneralError.Success)
                    break;
            }

            return _errorCode == (short)GeneralError.Success;
        }

        // ��������� ���������
        private bool SetConst()
        {
            POSCommand Cmd = new POSCommand(231);
            Cmd.AddChar(OPERATOR_PASSWD, 4);
            Cmd.AddNumeric(5, 2);
            Cmd.AddBChar("������������ ��������");
            _errorCode = Cmd.Execute(Port);
            return _errorCode == (short)GeneralError.Success;
        }

        #endregion

        #region ���������� ����������� �������

        protected override void OnAfterActivate()
        {
            ExecuteDriverCommand(delegate()
            {
                _errorCode = (short)GeneralError.Success;

                Port.ReadTimeout = READ_TIMEOUT;
                Port.WriteTimeout = WRITE_TIMEOUT;

                if (!SetConst())
                    return;

                // �������� ��������� ������
                if (PrinterStatus.PaperOut == PaperOutStatus.OutAfterActive)
                    if (!ContinuePrint())
                        return;

                if (!SetDocumentHeader())
                    return;

                if (!SetDocumentFooter())
                    return;
            });
        }

        protected override void OnOpenDocument(DocumentType docType,
            string cashierName)
        {
            ExecuteDriverCommand(delegate()
            {
                _errorCode = (short)GeneralError.Success;

                if (PrinterStatus.OpenedDocument)
                    _errorCode = CancelDocument();

                m_CurrentDocType = docType;
                POSCommand cmd = new POSCommand(205);
                cmd.AddChar(OPERATOR_PASSWD, 4);
                switch (docType)
                {
                    case DocumentType.Sale:
                        cmd.AddNumeric(0, 2);
                        break;
                    case DocumentType.Refund:
                        cmd.AddNumeric(1, 2);
                        break;
                    case DocumentType.PayingIn:
                        cmd.AddNumeric(2, 2);
                        break;
                    case DocumentType.PayingOut:
                        cmd.AddNumeric(3, 2);
                        break;
                    case DocumentType.Other:
                        cmd.AddNumeric(4, 2);
                        break;
                    case DocumentType.XReport:
                    case DocumentType.ZReport:
                    case DocumentType.SectionsReport:
                        //                        m_nErrorCode = PrintReport(docType);
                        return;
                }

                cmd.AddBChar(cashierName);  // ��� ���������
                cmd.AddNumeric(13, 2);      // ��� ���������
                cmd.AddBChar("");           // �����
                cmd.AddBChar("");           // ��������

                byte[] nRsp = new byte[5];
                _errorCode = cmd.Execute(Port, nRsp);
            });
        }

        protected override void OnCloseDocument(bool cutPaper)
        {
            ExecuteDriverCommand(delegate()
            {
                _errorCode = (short)GeneralError.Success;

                switch (m_CurrentDocType)
                {
                    case DocumentType.Sale:
                    case DocumentType.Refund:
                    case DocumentType.PayingIn:
                    case DocumentType.PayingOut:
                    case DocumentType.Other:
                        POSCommand cmd = new POSCommand(215);   // ������� ��������
                        cmd.AddChar(OPERATOR_PASSWD, 4);       // ������
                        cmd.AddNumeric(Convert.ToInt32(cutPaper), 1); // �������� ���
                        _errorCode = cmd.Execute(Port);
                        if (_errorCode != (short)GeneralError.Success)
                            CancelDocument();
                        break;
                    case DocumentType.XReport:
                    case DocumentType.SectionsReport:
                    case DocumentType.ZReport:
                        _errorCode = PrintReport(m_CurrentDocType);
                        break;
                }
            });
        }

        protected override void OnOpenDrawer()
        {
            ExecuteDriverCommand(delegate()
            {
                _errorCode = (short)GeneralError.Success;

                POSCommand cmd = new POSCommand(236);
                cmd.AddChar(OPERATOR_PASSWD, 4);
                cmd.AddNumeric(0, 1);
                _errorCode = cmd.Execute(Port);
            });
        }

        protected override void OnPrintString(string source, FontStyle style)
        {
            ExecuteDriverCommand(delegate()
            {
                _errorCode = (short)GeneralError.Success;

                if (m_CurrentDocType == DocumentType.XReport
                    || m_CurrentDocType == DocumentType.ZReport
                    || m_CurrentDocType == DocumentType.SectionsReport)
                {
                    return;
                }

                POSCommand cmd = new POSCommand(206);
                cmd.AddChar(OPERATOR_PASSWD, 4);   // ������
                cmd.AddNumeric(0, 1);               // ����������
                cmd.AddBChar(source);  // �����

                _errorCode = cmd.Execute(Port);

                if (_errorCode != (short)GeneralError.Success)
                    CancelDocument();

            });
        }

        protected override void OnPrintBarcode(string barcode, AlignOptions align,
            bool readable)
        {
            ExecuteDriverCommand(delegate()
            {
                _errorCode = (short)GeneralError.Success;

                byte[] nPrintBuffer = new byte[256];
                int BufSize = 0;

                // ���������������� �� ������
                nPrintBuffer[BufSize++] = 0x1B;
                nPrintBuffer[BufSize++] = Convert.ToByte('a');
                nPrintBuffer[BufSize++] = 1;

                // ������ �����-����
                nPrintBuffer[BufSize++] = 0x1D;
                nPrintBuffer[BufSize++] = Convert.ToByte('h');
                nPrintBuffer[BufSize++] = 70;

                // ������ ����
                nPrintBuffer[BufSize++] = 0x1D;
                nPrintBuffer[BufSize++] = Convert.ToByte('w');
                nPrintBuffer[BufSize++] = 2;

                // �����
                nPrintBuffer[BufSize++] = 0x1D;
                nPrintBuffer[BufSize++] = Convert.ToByte('H');
                if (readable)
                    nPrintBuffer[BufSize++] = 2;
                else
                    nPrintBuffer[BufSize++] = 0;

                // �����-���
                nPrintBuffer[BufSize++] = 0x1D;
                nPrintBuffer[BufSize++] = Convert.ToByte('k');
                nPrintBuffer[BufSize++] = 0x43;
                nPrintBuffer[BufSize++] = 0x0C;

                POSCommand cmd = new POSCommand(237);
                cmd.AddChar(OPERATOR_PASSWD, 4);   // ������
                cmd.AddNumeric(0, 1);               // ����������
                cmd.AddBChar(Encoding.ASCII.GetString(nPrintBuffer, 0, BufSize) + barcode);          // ������

                _errorCode = cmd.Execute(Port);

                if (_errorCode != (short)GeneralError.Success)
                    CancelDocument();
            });
        }

        protected override void OnPrintImage(System.Drawing.Bitmap image, AlignOptions align)
        {
            ExecuteDriverCommand(delegate() { });
        }

        protected override void OnRegistration(string commentary, uint quantity, uint amount,
            byte section)
        {
            ExecuteDriverCommand(delegate()
            {
                _errorCode = (short)GeneralError.Success;

                POSCommand cmd = new POSCommand(209);
                cmd.AddChar(OPERATOR_PASSWD, 4);   // ������
                cmd.AddNumeric(section, 3);       // ����� ������
                cmd.AddBChar("");                   // �����-���
                cmd.AddBChar("");                   // ���������� ������� ���
                cmd.AddBChar(commentary);         // ��������
                cmd.AddNumeric((int)amount, 10);    // ����
                cmd.AddNumeric((int)quantity, 8);// ����������
                cmd.AddNumeric(0, 10);                // ��������� ����
                cmd.AddBChar("");                   // �����������

                byte[] nRsp = new byte[5];

                _errorCode = cmd.Execute(Port, nRsp);
                if (_errorCode != (short)GeneralError.Success)
                    CancelDocument();

            });
        }

        protected override void OnPayment(uint amount, FiscalPaymentType paymentType)
        {
            ExecuteDriverCommand(delegate()
            {
                _errorCode = (short)GeneralError.Success;

                POSCommand cmd = new POSCommand(214);
                cmd.AddChar(OPERATOR_PASSWD, 4);

                switch (paymentType)
                {
                    case FiscalPaymentType.Cash:
                        cmd.AddNumeric(0, 2);
                        break;
                    case FiscalPaymentType.Card:
                        cmd.AddNumeric(2, 2);
                        break;
                    default:
                        cmd.AddNumeric(3, 2);
                        break;
                }
                cmd.AddNumeric((int)amount, 10);

                byte[] nRsp = new byte[11];
                _errorCode = cmd.Execute(Port, nRsp);
                if (_errorCode != (short)GeneralError.Success)
                    CancelDocument();
            });
        }

        protected override void OnCash(uint amount)
        {
            ExecuteDriverCommand(delegate()
            {
                _errorCode = (short)GeneralError.Success;

                POSCommand Cmd = new POSCommand(214);   // ������ �����
                Cmd.AddChar(OPERATOR_PASSWD, 4);        // ������
                Cmd.AddNumeric(0, 2);                   // ��� ������
                Cmd.AddNumeric((int)amount, 10);        // �����

                byte[] nRsp = new byte[11];
                _errorCode = Cmd.Execute(Port, nRsp);
                if (_errorCode != (short)GeneralError.Success)
                    CancelDocument();
            });
        }

        protected override void OnContinuePrint()
        {
            ExecuteDriverCommand(delegate()
            {
                _errorCode = (short)GeneralError.Success;

                ContinuePrint();
            });
        }

        #endregion

        #region ���������� ����������

        #region �������

        public override event EventHandler<PrinterBreakEventArgs> PrinterBreak;

        public override event EventHandler<FiscalBreakEventArgs> FiscalBreak;

        #endregion

        #region ��������

        public override DateTime CurrentTimestamp
        {
            get
            {
                DateTime currDateTime = DateTime.Now;
                ExecuteDriverCommand(delegate()
                {
                    _errorCode = (short)GeneralError.Success;
                    POSCommand cmd = new POSCommand(104);
                    cmd.AddChar(OPERATOR_PASSWD, 4);   // ������

                    byte[] Rsp = new byte[14];
                    _errorCode = cmd.Execute(Port, Rsp);
                    if (_errorCode == (short)GeneralError.Success)
                        currDateTime = new DateTime(
                            Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 4, 4)),
                            Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 2, 2)),
                            Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 0, 2)),
                            Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 8, 2)),
                            Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 10, 2)),
                            Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 12, 2))
                            );
                });
                return currDateTime;
            }
            set
            {
                ExecuteDriverCommand(delegate()
                {
                    _errorCode = (short)GeneralError.Success;
                    DateTime dtValue = value;

                    POSCommand cmd = new POSCommand(401);
                    cmd.AddChar(OPERATOR_PASSWD, 4);       // ������
                    cmd.AddNumeric(dtValue.Day, 2);         // ����
                    cmd.AddNumeric(dtValue.Month, 2);       // �����
                    cmd.AddNumeric(dtValue.Year, 4);        // ���
                    cmd.AddNumeric(dtValue.Hour, 2);        // ���
                    cmd.AddNumeric(dtValue.Minute, 2);      // ������
                    cmd.AddNumeric(dtValue.Second, 2);      // �������

                    _errorCode = cmd.Execute(Port);
                });
            }

        }

        public override FiscalDeviceInfo Info
        {
            get
            {
                string serialNo = "";
                ExecuteDriverCommand(delegate()
                {
                    _errorCode = (short)GeneralError.Success;
                    POSCommand Cmd = new POSCommand(103);          // �������� ��������� �����
                    Cmd.AddChar(OPERATOR_PASSWD, 4);   // ������
                    byte[] Rsp = new byte[12];
                    _errorCode = Cmd.Execute(Port, Rsp);
                    if (_errorCode == (short)GeneralError.Success)
                        serialNo = Encoding.ASCII.GetString(Rsp, 0, 12);
                });
                return new FiscalDeviceInfo(DeviceNames.ecrTypePilot, serialNo);
            }
        }

        public override FiscalStatusFlags Status
        {
            get
            {
                bool bOverShift = false;
                bool bOpenedShift = false;
                bool bLocked = false;
                bool bFiscalized = false;
                int nCashInDrawer = 0;
                int nDocAmount = 0;
                ExecuteDriverCommand(delegate()
                {
                    _errorCode = (short)GeneralError.Success;
                    POSCommand Cmd = new POSCommand(101);   // �������� ������
                    Cmd.AddChar(OPERATOR_PASSWD, 4);        // ������
                    Cmd.AddNumeric(1, 1);                   // � ������� ��������
                    byte[] Rsp = new byte[4];

                    _errorCode = Cmd.Execute(Port, Rsp);
                    if (_errorCode == (short)GeneralError.Success)
                    {
                        bOpenedShift = ((Rsp[0] & 0x10) == 0x10);
                        bOverShift = ((Rsp[1] & 0x2) == 0x2);
                        bLocked = ((Rsp[1] & 0x1) == 0x1);
                        bFiscalized = ((Rsp[0] & 0x4) == 0x4);
                    }

                    // ����� � ��
                    Cmd = new POSCommand(111);   // �������� �������� ��������
                    Cmd.AddChar(OPERATOR_PASSWD, 4);       // ������
                    Cmd.AddNumeric(23, 3);                  // ����� �������� (����� � �������� �����)

                    Rsp = new byte[16];
                    _errorCode = Cmd.Execute(Port, Rsp);
                    if (_errorCode == (short)GeneralError.Success)
                        nCashInDrawer = Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 0, 16));

                    // ����� ���������
                    bool IsOpenAndFiscal = false;
                    Rsp = new byte[33];

                    Cmd = new POSCommand(110);   // �������� ������ �� �������� ���������
                    Cmd.AddChar(OPERATOR_PASSWD, 4);       // ������
                    _errorCode = Cmd.Execute(Port, Rsp);
                    if (_errorCode == (short)GeneralError.Success)
                        IsOpenAndFiscal = (Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 0, 1)) > 0) && (Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 1, 2)) != 4);

                    if (IsOpenAndFiscal)
                    {
                        Cmd = new POSCommand(111);          // �������� �������� ��������
                        Cmd.AddChar(OPERATOR_PASSWD, 4);   // ������
                        Cmd.AddNumeric(105, 3);             // ����� �������� (����� ������� ������ � ����)
                        Rsp = new byte[16];
                        _errorCode = Cmd.Execute(Port, Rsp);
                        if (_errorCode == (short)GeneralError.Success)
                            nDocAmount = Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 0, 16));
                    }
                });
                return new FiscalStatusFlags(bOpenedShift, bOverShift, bLocked, bFiscalized, (ulong)nDocAmount, (ulong)nCashInDrawer);
            }
        }

        public override PrintableDeviceInfo PrinterInfo
        {
            get
            {
                return new PrintableDeviceInfo(new PrintableTapeWidth(MAX_STRING_LEN, 0), false);
            }
        }

        public override PrinterStatusFlags PrinterStatus
        {
            get
            {
                bool bPrinting = false;
                PaperOutStatus poStatus = PaperOutStatus.OutPassive;
                bool bDrawerOpened = false;
                bool bDocOpened = false;
                ExecuteDriverCommand(delegate()
                {
                    _errorCode = (short)GeneralError.Success;
                    POSCommand Cmd = new POSCommand(101);   // �������� ������
                    Cmd.AddChar(OPERATOR_PASSWD, 4);        // ������
                    Cmd.AddNumeric(1, 1);                   // � ������� ��������
                    byte[] Rsp = new byte[4];

                    _errorCode = Cmd.Execute(Port, Rsp);
                    if (_errorCode == (short)GeneralError.Success)
                    {
                        bDocOpened = ((Rsp[0] & 0x20) == 0x20);
                        bPrinting = ((Rsp[3] & 0x2) == 0x2);
                        bool bLocked = ((Rsp[1] & 0x1) == 0x1);
                        bool bPaperOut = ((Rsp[3] & 0x4) == 0x4);

                        if (bLocked && bPaperOut)
                            poStatus = PaperOutStatus.OutActive;
                        else if (bPaperOut)
                            poStatus = PaperOutStatus.OutPassive;
                        else if (bLocked)
                            poStatus = PaperOutStatus.OutAfterActive;
                        else
                            poStatus = PaperOutStatus.Present;
                    }

                    // ��������� ��
                    Cmd = new POSCommand(128);          // �������� ��������� ��������� �����
                    Cmd.AddChar(OPERATOR_PASSWD, 4);   // ������

                    Rsp = new byte[1];
                    _errorCode = Cmd.Execute(Port, Rsp);
                    if (_errorCode == (short)GeneralError.Success)
                        bDrawerOpened = Rsp[0] == '1';
                });
                return new PrinterStatusFlags(bPrinting, poStatus, bDocOpened, bDrawerOpened);
            }
        }

        #endregion

        #region ������ ���������� ����������

        public override void FiscalReport(FiscalReportType reportType, bool full, params object[] reportParams)
        {
            ExecuteDriverCommand(delegate()
            {
                POSCommand Cmd = null;
                _errorCode = (short)GeneralError.Success;
                switch (reportType)
                {
                    case FiscalReportType.ByDates:
                        DateTime firstDate = (DateTime)reportParams[0];
                        DateTime lastDate = (DateTime)reportParams[1];

                        Cmd = new POSCommand(303);      // ���������� ����� �� �����
                        Cmd.AddNumeric(TaxerPassword, 8);    // ������
                        Cmd.AddNumeric(firstDate.Day, 2);  // ���� 
                        Cmd.AddNumeric(firstDate.Month, 2); // �����
                        Cmd.AddNumeric(firstDate.Year, 4);  // ���
                        Cmd.AddNumeric(lastDate.Day, 2);   // ����
                        Cmd.AddNumeric(lastDate.Month, 2); // �����
                        Cmd.AddNumeric(lastDate.Year, 4);  // ���
                        Cmd.AddNumeric(Convert.ToInt32(full), 1);        // ������ ��� ������� �����
                        break;
                    case FiscalReportType.ByShifts:
                        Cmd = new POSCommand(304);      // ���������� ����� �� ������
                        Cmd.AddNumeric(TaxerPassword, 8);    // ������
                        Cmd.AddNumeric((int)(reportParams[0]), 4);  // ��������� �����
                        Cmd.AddNumeric((int)(reportParams[1]), 4);  // �������� �����
                        Cmd.AddNumeric(Convert.ToInt32(full), 1);        // ������ ��� ������� �����
                        break;
                }

                _errorCode = Cmd.Execute(Port);
            });
        }

        public override void Fiscalization(int newPassword, long registrationNumber, long taxPayerNumber)
        {
            ExecuteDriverCommand(delegate()
            {
                string sCurrentPasswd;
                _errorCode = (short)GeneralError.Success;
                if (!this.Status.Fiscalized)
                {
                    StringBuilder sBuilder = new StringBuilder(8);
                    sBuilder.Insert(0, " ", 8);
                    sBuilder.Insert(0, this.Info.SerialNo);
                    sCurrentPasswd = sBuilder.ToString(0, 8);
                }
                else
                {
                    sCurrentPasswd = TaxerPassword.ToString("d8");
                }

                POSCommand Cmd = new POSCommand(302);
                Cmd.AddChar(sCurrentPasswd, 8);
                Cmd.AddNumeric((int)registrationNumber, 12);
                Cmd.AddNumeric((int)taxPayerNumber, 12);
                Cmd.AddNumeric(2, 1);
                Cmd.AddNumeric(newPassword, 8);

                _errorCode = Cmd.Execute(Port);
            });
        }

        public override void GetLifetime(out DateTime firstDate, out DateTime lastDate, out int firstShift, out int lastShift)
        {
            firstDate = new DateTime();
            lastDate = new DateTime();
            firstShift = 0;
            lastShift = 0;

            DateTime _firstDate = firstDate;
            DateTime _lastDate = lastDate;
            int _firstShift = firstShift;
            int _lastShift = lastShift;
            ExecuteDriverCommand(delegate()
            {
                _errorCode = (short)GeneralError.Success;

                POSCommand Cmd = new POSCommand(106);   // �������� ����� ��������� �������� �����
                Cmd.AddChar(OPERATOR_PASSWD, 4);
                byte[] Rsp = new byte[4];
                _errorCode = Cmd.Execute(Port, Rsp);
                if (_errorCode != (short)GeneralError.Success)
                    return;

                _firstShift = 1;
                _lastShift = Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 0, 4));

                Cmd = new POSCommand(109);              // �������� ������ � ������ �������� �����
                Cmd.AddChar(OPERATOR_PASSWD, 4);
                Cmd.AddNumeric(_firstShift, 4);

                Rsp = new byte[24];
                _errorCode = Cmd.Execute(Port, Rsp);
                if (_errorCode != (short)GeneralError.Success)
                    return;

                _firstDate = new DateTime(
                    Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 4 + 4, 4)),
                    Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 4 + 2, 2)),
                    Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 4 + 0, 2))
                    );

                Cmd = new POSCommand(109);              // �������� ������ � ��������� �������� �����
                Cmd.AddChar(OPERATOR_PASSWD, 4);
                Cmd.AddNumeric(_lastShift, 4);

                Rsp = new byte[24];
                _errorCode = Cmd.Execute(Port, Rsp);
                if (_errorCode != (short)GeneralError.Success)
                    return;

                _lastDate = new DateTime(
                    Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 4 + 4, 4)),
                    Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 4 + 2, 2)),
                    Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 4 + 0, 2)));
            });

            firstDate = _firstDate;
            lastDate = _lastDate;
            firstShift = _firstShift;
            lastShift = _lastShift;
        }

        #endregion

        #endregion
    }
}
