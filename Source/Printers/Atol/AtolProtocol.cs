using System;
using System.Collections.Generic;
using System.Text;
using DevicesBase;
using DevicesBase.Helpers;
using DevicesCommon;
using DevicesCommon.Helpers;
using System.Drawing.Imaging;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace Atol
{
    internal enum AtolModel
    {
        TriumfR     = 13,
        FelixRF     = 14, 
        Felix02K    = 15,
        Mercury140F = 16,
        Tornado     = 20,
        MercuryMSK  = 23,
        FelixRK     = 24,
        Felix3SK    = 27,
        FPrint02K   = 30,
        FPrint03K   = 31,
        FPrint88K   = 32,
        FPrint5200K = 35,
        FPrint55K   = 47,
        FPrint22K   = 52,
        Unknown     = 255
    }

    internal struct DeviceParams
    {
        // ������������ ����� ������������ ������
        public int MaxStringLen;

        // ������������ ���������� ���������� ��������
        public int StringLen;

        // ������ ������� ����� � ������
        public int TapeWidth;

        // ��������� ������ �����-����
        public int BarcodeWidth;

        // ���-�� �������� ����������� ���������
        public int SlipLineSize;

        // ��������� ���������
        public Boolean IsCutterSupported;
    }

    [Serializable]
    [FiscalDeviceAttribute(DeviceNames.ecrTypeAtol)]
    public class AtolFiscalDevice : CustomFiscalDevice, ICommunicationPortProvider
    {
        #region ���������

        private const int READ_TIMEOUT = 1000;
        private const int WRITE_TIMEOUT = 200;

        // ������ ������� � ���
        private const int OPERATOR_PASSWD = 1111;

        // ������ ������������ �������
        private const int MODE_PASSWD = 30;

        // ��������� ��������� ����� ��� ������������ �����-����
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

        #region ���� ������

        // ��� �������� ���������
        private DocumentType _currDocType;

        // ����� ���� (��� ��������/������� �����)
        private long _docAmount = 0;

        // ������ ����������
        private AtolModel _deviceModel = AtolModel.Unknown;

        private AtolProtocol _atolProtocol;

        #endregion

        #region �����������

        public AtolFiscalDevice()
            : base()
        {
            AddSpecificError(1, "����������� ����� ���������� ��� ������");
            AddSpecificError(8, "�������� ���� (�����)");
            AddSpecificError(10, "�������� ����������");
            AddSpecificError(11, "������������ �������� ����������");
            AddSpecificError(12, "���������� ������ ��������� ��������");
            AddSpecificError(13, "������ �� ���� ���������� (� ���� ���������������� ������� ���������� ������� � ��������� �����)");
            AddSpecificError(14, "���������� ������ ��������� ��������");
            AddSpecificError(15, "��������� ������ �� �������� ����������");
            AddSpecificError(16, "������/�������� �� ���������� �������� ����������");
            AddSpecificError(17, "�������� ��� ������");
            AddSpecificError(18, "�������� �����-��� ������");
            AddSpecificError(19, "�������� ������");
            AddSpecificError(20, "�������� �����");
            AddSpecificError(21, "��� ������������� � ������ ����� ����");
            AddSpecificError(22, "��������� ������������� ����� ����");
            AddSpecificError(24, "��� ������ ������ ��� �������� �� ���");
            AddSpecificError(25, "��� ������������� ��� ������ �������");
            AddSpecificError(26, "����� � �������� �������. ���� � ����� ����������");
            AddSpecificError(27, "���������� �������� ���������� ���������� (�� ��������� ����������� ���� ������)");
            AddSpecificError(30, "���� � ����� ������������");
            AddSpecificError(31, "��������� ���� � �����");
            AddSpecificError(32, "���� � ����� ��� ������, ��� � ����");
            AddSpecificError(33, "���������� ������� �����");
            AddSpecificError(50, "������������ ������� �������");
            AddSpecificError(51, "���������� ������� ������");
            AddSpecificError(61, "����� �� ������");
            AddSpecificError(62, "������� �����-��� � ����������� <>1.000");
            AddSpecificError(63, "������������ ������ ����");
            AddSpecificError(64, "������������� ���������� ������");
            AddSpecificError(65, "������������ ���������� ������ ����������");
            AddSpecificError(66, "��������������� ����� �� ������ � ������ ����");
            AddSpecificError(67, "������ ����� �� ���������� � ����, ������ ����������");
            AddSpecificError(68, "Memo Plus 3 ������������� � ��");
            AddSpecificError(69, "������ ����������� ����� ������� �������� Memo Plus 3");
            AddSpecificError(70, "�������� ������� �� ���");
            AddSpecificError(102, "������� �� ����������� � ������ ������ ���");
            AddSpecificError(103, "��� ������");
            AddSpecificError(104, "��� ����� � ��������� �����");
            AddSpecificError(105, "������������ ������ ����������� ����������");
            AddSpecificError(106, "�������� ��� ����");
            AddSpecificError(107, "��� ������ ����� ��������");
            AddSpecificError(108, "�������� ����� ��������");
            AddSpecificError(109, "������������ ������� ����������");
            AddSpecificError(110, "��� ����� � ������� ��������");
            AddSpecificError(111, "�������� ����� �������� / �������� �����������");
            AddSpecificError(112, "����� ������ ������, ��� ���� �������� ������ ����� ������");
            AddSpecificError(113, "����� �� �������� �������� ��������� ����� ����");
            AddSpecificError(114, "����� �������� ������ ����� ����");
            AddSpecificError(115, "���������� ������ ����� �������� ��� �������������");
            AddSpecificError(117, "������������ ����� ��������");
            AddSpecificError(122, "������ ������ ��� �� ����� ��������� �������");
            AddSpecificError(123, "�������� �������� ������ / ��������");
            AddSpecificError(124, "�������� ����� ������ / �������� ����������");
            AddSpecificError(125, "�������� ������");
            AddSpecificError(126, "�������� ��� ������");
            AddSpecificError(127, "������������ ��� ���������");
            AddSpecificError(128, "�������� ��������� � ������� ��������");
            AddSpecificError(129, "������������ ����� ����");
            AddSpecificError(130, "������ ��� ������������� � �������� ����������");
            AddSpecificError(132, "������������ ������ ����������� �����");
            AddSpecificError(134, "�������� �������� ����� ������ ����� ����");
            AddSpecificError(135, "������ ��� �������� � �������� ����������");
            AddSpecificError(136, "����� ��������� 24 ����");
            AddSpecificError(137, "������ ��� ������� � �������� ����������");
            AddSpecificError(138, "������������ ��");
            AddSpecificError(140, "�������� ������");
            AddSpecificError(141, "����� ����������� ����� �� ����������");
            AddSpecificError(142, "���� ��������� ����������� �����");
            AddSpecificError(143, "���������� ����� (��������� ������� ����������)");
            AddSpecificError(145, "�������� ����� �������");
            AddSpecificError(146, "�������� ����� ����");
            AddSpecificError(147, "�������� ����� ����");
            AddSpecificError(148, "�������� ����");
            AddSpecificError(149, "�������� �����");
            AddSpecificError(150, "����� ���� �� ������ ������ ����� ������");
            AddSpecificError(151, "������� ����� ����� ����������");
            AddSpecificError(152, "� ��� ��� ����� ��� �������");
            AddSpecificError(154, "��� ������ � �������� ����������");
            AddSpecificError(155, "��� ������ � �������� ����������");
            AddSpecificError(156, "����� �������, �������� ����������");
            AddSpecificError(157, "��� �������������, ���� ����� ������ ���������� ����������");
            AddSpecificError(158, "��������� ����� ��� �����");
            AddSpecificError(159, "���������� ��������������� �� ����� ���� ����� 4");
            AddSpecificError(160, "������ �.�.");
            AddSpecificError(162, "�������� �����");
            AddSpecificError(163, "�������� ��� ������");
            AddSpecificError(164, "������������ ������");
            AddSpecificError(165, "������������ ��������� ����� ���");
            AddSpecificError(166, "������������ ���");
            AddSpecificError(167, "������������ ���");
            AddSpecificError(168, "��� �� ���������������");
            AddSpecificError(169, "�� ����� ��������� �����");
            AddSpecificError(170, "��� �������");
            AddSpecificError(171, "����� �� �������������");
            AddSpecificError(172, "��� ���������� ���� � ��");
            AddSpecificError(173, "��� ������ ������� ��");
            AddSpecificError(174, "������������ ��� ��� ����� ���� ������ ���");
            AddSpecificError(176, "��������� ���������� ������ �������");
            AddSpecificError(177, "������� �� ��������� ���������� ������ ������ ���");
            AddSpecificError(178, "���������� ������ ������/��������");
            AddSpecificError(179, "���������� ������� ��� ������ ����� ������ (� ���� ������������ �������� ��� �������� ��������)");
            AddSpecificError(180, "�������� ����� ��������");
            AddSpecificError(181, "�������� ����� ��������� ����");
            AddSpecificError(182, "�������� ����� �������� ����");
            AddSpecificError(183, "�������� ��� ������");
            AddSpecificError(184, "�������� �����");
            AddSpecificError(186, "������ ������ � ���������� �������");
            AddSpecificError(190, "���������� �������� ���������������� ������");
            AddSpecificError(200, "��� ����������, ��������������� ������ �������");
            AddSpecificError(201, "��� ����� � ������� �����������");
            AddSpecificError(202, "�������� ��������� ������ ���");
            AddSpecificError(203, "� ���� ������� ������� �������� ������ ���� �����������");
            AddSpecificError(204, "�������� ����� ������ ���");
            AddSpecificError(205, "�������� ��������");
            AddSpecificError(206, "������������ ������� ����������");
            AddSpecificError(207, "� ��� ����������� 20 �����������");
            AddSpecificError(208, "����������� ������ ���� � ������� ������ ��� ����������");
            AddSpecificError(209, "������������ ����������");
            AddSpecificError(210, "������ ������ � ���� �� ������ ���������� I2C");
            AddSpecificError(211, "������ ������� �������� ����");
            AddSpecificError(212, "�������� ��������� ����");
            AddSpecificError(213, "������������ ������ ����");
            AddSpecificError(214, "������ ������-���������� ����");
            AddSpecificError(215, "�������� ��������� ������ ����");
            AddSpecificError(216, "���� �����������");
            AddSpecificError(217, "� ���� �������� �������� ���� ��� �����");
            AddSpecificError(218, "� ���� ��� ����������� ������");
            AddSpecificError(219, "������������ ���� (���� ����)");
            AddSpecificError(254, "������ ������ ����������");
            AddSpecificError(255, "����� ����� ������� ��� ������ ������������");
            AddSpecificError(-1, "����������� ������");

            _atolProtocol = new AtolProtocol(this, OPERATOR_PASSWD);
        }

        #endregion

        private delegate void ExecuteCommandDelegate();

        #region ���������� ������

        private bool IsSlipMode()
        {
            return PrinterNumber != PrinterNumber.MainPrinter && _deviceModel == AtolModel.Felix3SK;
        }

        private DeviceParams GetDeviceParams()
        {
            DeviceParams devParams = new DeviceParams();
            switch (_deviceModel)
            {
                case AtolModel.FelixRF:
                case AtolModel.Felix02K:
                    devParams.MaxStringLen = 20;
                    devParams.StringLen = 20;
                    devParams.TapeWidth = 120 - 5;
                    devParams.BarcodeWidth = 1;
                    break;
                case AtolModel.Mercury140F:
                case AtolModel.Tornado:
                    devParams.MaxStringLen = 48;
                    devParams.StringLen = 40;
                    devParams.TapeWidth = 464 - 33;
                    devParams.BarcodeWidth = 3;
                    devParams.IsCutterSupported = true;
                    break;
                case AtolModel.FelixRK:
                    devParams.MaxStringLen = 38;
                    devParams.StringLen = 32;
                    devParams.TapeWidth = 320 - 40;
                    devParams.BarcodeWidth = 2;
                    break;
                case AtolModel.Felix3SK:
                    devParams.MaxStringLen = 38;
                    devParams.StringLen = 38;
                    devParams.TapeWidth = 320 - 40;
                    devParams.BarcodeWidth = 2;
                    devParams.SlipLineSize = 38;
                    devParams.IsCutterSupported = true;
                    break;
                case AtolModel.FPrint02K:
                    devParams.MaxStringLen = 56;
                    devParams.StringLen = 40;
                    devParams.TapeWidth = 204;// 320 - 40;
                    devParams.BarcodeWidth = 1;
                    devParams.IsCutterSupported = true;
                    break;
                case AtolModel.FPrint03K:
                    devParams.MaxStringLen = 32;
                    devParams.StringLen = 32;
                    devParams.TapeWidth = 320 - 40;
                    devParams.BarcodeWidth = 2;
                    break;
                case AtolModel.FPrint5200K:
                case AtolModel.FPrint55K:
                    devParams.MaxStringLen = 36;
                    devParams.StringLen = 36;
                    devParams.TapeWidth = 360 - 40;
                    devParams.BarcodeWidth = 2;
                    devParams.IsCutterSupported = true;
                    break;
                case AtolModel.FPrint22K:
                    devParams.MaxStringLen = 48;
                    devParams.StringLen = 48;
                    devParams.TapeWidth = 390;
                    devParams.BarcodeWidth = 2;
                    devParams.IsCutterSupported = true;
                    break;
                case AtolModel.FPrint88K:
                    devParams.MaxStringLen = 56;
                    devParams.StringLen = 42;
                    devParams.TapeWidth = 340;
                    devParams.BarcodeWidth = 2;
                    devParams.IsCutterSupported = true;
                    break;
                default:
                    devParams.MaxStringLen = 48;
                    devParams.StringLen = 40;
                    devParams.TapeWidth = 464 - 33;
                    devParams.BarcodeWidth = 3;
                    break;
            }

            return devParams;
        }

        // �������� ���������� �������
        private bool WaitForStateChange(ref byte nMode, ref byte nSubMode, ref byte nFlags)
        {
            byte nCurrentMode = 0;
            byte nCurrentSubMode = 0;
            bool bSuccess = false;

            do
            {
                try
                {
                    _atolProtocol.ExecuteCommand(0x45);
                    nCurrentMode = (byte)(_atolProtocol.Response[2] & 0x0F);
                    nCurrentSubMode = (byte)(_atolProtocol.Response[2] >> 4);
                    nFlags = _atolProtocol.Response[3];
                    bSuccess = (nCurrentMode != nMode) || (nCurrentSubMode != nSubMode);
                }
                catch(TimeoutException)
                {
                    bSuccess = false;
                }
                if (!bSuccess)
                    System.Threading.Thread.Sleep(500);
            }
            while (!bSuccess);

            nMode = nCurrentMode;
            nSubMode = nCurrentSubMode;
            return true;
        }

        // ����������� ������ ���������� � ��������� �������� ������ �����
        private void SetDeviceType()
        {
            _atolProtocol.ExecuteCommand(0xA5);
            _deviceModel = (AtolModel)_atolProtocol.Response[4];
        }

        // ��������� ������� ���������
        private void SetFooter()
        {
            _atolProtocol.SwitchToMode(4, MODE_PASSWD);
            int nRow = 1;
            if (PrintFooter && DocumentFooter != null)
            {
                foreach (string s in DocumentFooter)
                {
                    _atolProtocol.CreateCommand(0x50);
                    _atolProtocol.AddByte(6);
                    _atolProtocol.AddBCD(nRow++, 2);
                    _atolProtocol.AddByte(1);
                    _atolProtocol.AddString(s, GetDeviceParams().MaxStringLen);

                    _atolProtocol.Execute();
                    if (nRow > 5)
                        break;
                }
            }

            // �������� ������������ �������
            if (PrintGraphicFooter && GraphicFooter != null)
            {
                if (!PrintGraphicHeader || GraphicHeader == null)
                {
                    // ������� ������� ��������
                    _atolProtocol.CreateCommand(0x8A);
                    _atolProtocol.AddByte(0);
                    _atolProtocol.Execute();
                }

                // �������� ����� ��������
                int imageNo = LoadImage(GraphicFooter);

                // ������ ������ ����� � ���������
                _atolProtocol.CreateCommand(0x50);
                _atolProtocol.AddByte(6);
                _atolProtocol.AddBCD(1, 2);
                _atolProtocol.AddByte(1);
                var line = new byte[GetDeviceParams().MaxStringLen];
                line[0] = 0x0A;
                line[1] = (byte)imageNo;
                line[2] = 0x00; // ��������
                line[3] = (byte)((GetDeviceParams().TapeWidth - GraphicFooter.Width) / 2); // ��������

                _atolProtocol.AddBytes(line);
                _atolProtocol.Execute();
            }
        }

        // ��������� ��������� ���������
        private void SetHeader()
        {
            _atolProtocol.SwitchToMode(4, MODE_PASSWD);
            int nRow = 6;

            // ��������� ����� �����
            if (PrintHeader && DocumentHeader != null)
            {
                foreach (string s in DocumentHeader)
                {
                    _atolProtocol.CreateCommand(0x50);
                    _atolProtocol.AddByte(6);
                    _atolProtocol.AddBCD(nRow++, 2);
                    _atolProtocol.AddByte(1);
                    _atolProtocol.AddString(s, GetDeviceParams().MaxStringLen);
                    _atolProtocol.Execute();
                    if (nRow > 8)
                        break;
                }
            }

            // �������� ������������ �����
            if (PrintGraphicHeader && GraphicHeader != null)
            {
                // ��������� ���������� ����� � ������� ��� ����������� �������� �������
                // ������� 2, ��� 1, ���� 21, �������� 30h
                _atolProtocol.CreateCommand(0x50);
                _atolProtocol.AddByte(2);
                _atolProtocol.AddBCD(1, 2);
                _atolProtocol.AddByte(21);
                _atolProtocol.AddBCD(48, 1);
                _atolProtocol.Execute();

                // ������� ������� ��������
                _atolProtocol.CreateCommand(0x8A);
                _atolProtocol.AddByte(0);
                _atolProtocol.Execute();

                int imageNo = LoadImage(GraphicHeader);

                // ������ ������ ����� � ���������
                _atolProtocol.CreateCommand(0x50);
                _atolProtocol.AddByte(6);
                _atolProtocol.AddBCD(8, 2);
                _atolProtocol.AddByte(1);
                var line = new byte[GetDeviceParams().MaxStringLen];
                line[0] = 0x0A;
                line[1] = (byte)imageNo;
                line[2] = 0x00; // ��������
                line[3] = (byte)((GetDeviceParams().TapeWidth - GraphicHeader.Width) / 2); // ��������

                _atolProtocol.AddBytes(line);
                _atolProtocol.Execute();
            }

        }

        private void SetTapeWidth()
        {
            // ��������� ���-�� �������� � ������
            _atolProtocol.SwitchToMode(4, MODE_PASSWD);
            _atolProtocol.CreateCommand(0x50);
            _atolProtocol.AddByte(2);       // �������
            _atolProtocol.AddBCD(1, 2);// ���
            _atolProtocol.AddByte(55);       // ����
            _atolProtocol.AddBCD(GetDeviceParams().StringLen, 1);
            _atolProtocol.Execute();

            // ��������� ������� �� ����������� ������
            _atolProtocol.SwitchToMode(4, MODE_PASSWD);
            _atolProtocol.CreateCommand(0x50);
            _atolProtocol.AddByte(2);       // �������
            _atolProtocol.AddBCD(1, 2);// ���
            _atolProtocol.AddByte(56);       // ����
            _atolProtocol.AddBCD(2, 1);
            _atolProtocol.Execute();
        }


        // ������ �������
        private void PrintReport(DocumentType docType)
        {
            byte nMode = 0;
            byte nSubMode = 0;
            byte nFlags = 0;
            switch (docType)
            {
                case DocumentType.XReport:
                    nMode = 2;
                    nSubMode = 2;
                    _atolProtocol.SwitchToMode(2, MODE_PASSWD);
                    _atolProtocol.CreateCommand(0x67);
                    _atolProtocol.AddBCD(1, 1);
                    _atolProtocol.Execute();
                    break;
                case DocumentType.ZReport:
                    nMode = 3;
                    nSubMode = 2;
                    _atolProtocol.SwitchToMode(3, MODE_PASSWD);
                    _atolProtocol.ExecuteCommand(0x5A);
                    break;
                case DocumentType.SectionsReport:
                    nMode = 2;
                    nSubMode = 2;
                    _atolProtocol.SwitchToMode(2, MODE_PASSWD);
                    _atolProtocol.CreateCommand(0x67);
                    _atolProtocol.AddBCD(2, 1);
                    _atolProtocol.Execute();
                    break;
            }

            if (!WaitForStateChange(ref nMode, ref nSubMode, ref nFlags))
                return;

            if (docType == DocumentType.ZReport)
            {
                if (nMode == 7 && nSubMode == 1)
                {
                    WaitForStateChange(ref nMode, ref nSubMode, ref nFlags);
                }

                if (nMode == 3 && nSubMode == 0)
                {
                    if ((nFlags & 0x01) == 0x01)
                        throw new DeviceErrorException(103);// ��� ������
                    else
                    {
                        if ((nFlags & 0x02) == 0x02)
                            throw new DeviceErrorException(104);// ��� ����� � ���������
                    }
                }
                else
                    throw new DeviceErrorException(254); // ������ ������ ����������
            }
            else
            {
                if (nMode == 2 && nSubMode == 0)
                {
                    if ((nFlags & 0x01) == 0x01)
                        throw new DeviceErrorException(103);// ��� ������
                    else
                    {
                        if ((nFlags & 0x02) == 0x02)
                            throw new DeviceErrorException(104);// ��� ����� � ���������
                    }
                }
                else
                    throw new DeviceErrorException(254);// ������ ������ ����������
            }
        }

        // ������ ���������
        private void CancelDocument()
        {
            if (!DocOpened())
            {
                PrintStringInternal("��� �����������", FontStyle.Regular);
                _atolProtocol.ExecuteCommand(0x6C);
            }
            else
                _atolProtocol.ExecuteCommand(0x59);

            _docAmount = 0;
        }

        // �������� ��������� ���������
        private bool DocOpened()
        {
            // ������ ���������
            _atolProtocol.ExecuteCommand(0x3F);
            return ((_atolProtocol.Response[18] & 1) == 1) && (_atolProtocol.Response[23] != 0);
        }

        // ������ ������
        private void PrintStringInternal(String source, FontStyle style)
        {
            _atolProtocol.CreateCommand(0x87);
            _atolProtocol.AddByte(0); // ����� ��������
            _atolProtocol.AddByte(1); // ������ �� ������� �����
            _atolProtocol.AddByte(0); // �����

            // ��������� �� ���������
            if (style == FontStyle.DoubleHeight || style == FontStyle.DoubleAll)
                _atolProtocol.AddByte(1);
            else
                _atolProtocol.AddByte(0);

            _atolProtocol.AddByte(0); // ����������                
            _atolProtocol.AddByte(0); // �������
            _atolProtocol.AddByte(1); // ����� ��
            _atolProtocol.AddByte(1); // ����� ��
            _atolProtocol.AddByte(0); // ��������������
            _atolProtocol.AddByte(0); // ������
            _atolProtocol.AddByte(0); // ������
            //                Cmd.AddByte(0); // ������
            _atolProtocol.AddString(source, source.Length, (style == FontStyle.DoubleWidth || style == FontStyle.DoubleAll)); // ������
            _atolProtocol.Execute();
        }

        // �������������� ������ �����-���� � ������ ���� ��� ������ ��������
        private byte[] GetBarcodeData(ref string barcode, int nWidth)
        {
            // ������� ����������� �����            
            char[] barcodeChars = barcode.ToCharArray(0, 12);
            int nChecksum = 0;
            for (int i = 0; i < 12; i+=2)
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

            string sWideBarcodeData = "";
            foreach (char c in sBarcodeData)
            {
                for (int i = 0; i < nWidth; i++)
                    sWideBarcodeData += c;
            }

            byte[] barcodeData = new byte[12 * nWidth];
            for (int i = 0; i < 12 * nWidth; i++)
                barcodeData[i] = Convert.ToByte(sWideBarcodeData.Substring(i * 8, 8), 2);

            return barcodeData;
        }

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
                ErrorCode = new ServerErrorCode(this, GeneralError.Timeout, _atolProtocol != null ? _atolProtocol.GetCommandDump() : String.Empty);
            }
            catch (DeviceErrorException E)
            {
                ErrorCode = new ServerErrorCode(this, E.ErrorCode, _atolProtocol != null ? GetSpecificDescription(E.ErrorCode) : String.Empty, _atolProtocol.GetCommandDump());
            }
            catch (Exception E)
            {
                ErrorCode = new ServerErrorCode(this, E, _atolProtocol != null ? _atolProtocol.GetCommandDump() : String.Empty);
            }
            finally
            {
                if (!ErrorCode.Succeeded)
                {
                    try
                    {
                        if (_atolProtocol.IsSlipMode)
                            _atolProtocol.IsSlipMode = false;
                        else if (DocOpened())
                            CancelDocument();
                    }
                    catch (DeviceErrorException)
                    {
                    }
                    catch (TimeoutException)
                    {
                    }
                }
            }
        }

        private byte[] GetImageBytes(System.Drawing.Bitmap image, out int stride)
        {
            image.RotateFlip(System.Drawing.RotateFlipType.Rotate270FlipNone);
            var rect = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);
            var bmpData = image.LockBits(rect, ImageLockMode.ReadOnly, image.PixelFormat);
            byte[] imageBytes = new byte[image.Height * bmpData.Stride];
            stride = bmpData.Stride;
            try
            {
                System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, imageBytes, 0, bmpData.Height * stride);
                // ����������� �����
                if (image.Palette.Entries[0].Name == "ff000000")
                {
                    byte invertor = 0xFF;
                    int delta = stride * 8 - image.Width;
                    byte invertor2 = (byte)(invertor << delta);
                    for (int i = 0; i < imageBytes.Length; i++)
                    {
                        if( (i+1)%stride == 0)
                            imageBytes[i] = (byte)(imageBytes[i] ^ invertor2);
                        else
                            imageBytes[i] = (byte)(imageBytes[i] ^ invertor);
                    }
                }
                return imageBytes;
            }
            finally
            {
                image.UnlockBits(bmpData);
            }
        }

        /// <summary>
        /// �������� �������� � ������ ����������
        /// </summary>
        /// <param name="image">��������</param>
        /// <param name="index">����� ��������</param>
        private int LoadImage(System.Drawing.Bitmap image)
        {
            // �������� ����� ��������
            int stride;
            var imageBytes = GetImageBytes(GraphicHeader, out stride);
            int bytesRead = 0;
            var lineBytes = new byte[stride];
            while (bytesRead < imageBytes.Length)
            {
                Array.Copy(imageBytes, bytesRead, lineBytes, 0, stride);
                _atolProtocol.CreateCommand(0x8B);
                _atolProtocol.AddBytes(lineBytes);
                _atolProtocol.Execute();
                bytesRead += stride;
            }
            // �������� ��������
            _atolProtocol.ExecuteCommand(0x9E);

            // ���������� ����� ����� ��������
            return _atolProtocol.Response[3];
        }

        /// <summary>
        /// ������ �������� �� ������ ���������� �� ������
        /// </summary>
        /// <param name="index">����� ��������</param>
        private void PrintImage(int index)
        {
            _atolProtocol.CreateCommand(0x8D);
            _atolProtocol.AddByte(1); // ������ �� ������� �����
            _atolProtocol.AddByte((byte)index); // ����� ��������
            _atolProtocol.AddBCD(0, 2); // ��������
            _atolProtocol.Execute();
        }


        #endregion

        #region ����������� ������

        protected override void OnAfterActivate()
        {
            Port.ReadTimeout = READ_TIMEOUT;
            Port.WriteTimeout = WRITE_TIMEOUT;

            ExecuteDriverCommand(delegate()
           {
               // ���������� ������ ����������
               SetDeviceType();
               // ��������� ����� ������ (������ ��� ������-3��)
               if (_deviceModel == AtolModel.Felix3SK)
                   SetTapeWidth();
               SetHeader();
               SetFooter();
           });
        }

        protected override void OnOpenDocument(DocumentType docType,
            String cashierName)
        {
            ExecuteDriverCommand(delegate()
            {
                if (PrinterStatus.OpenedDocument)
                    CancelDocument();

                switch (docType)
                {
                    case DocumentType.Sale:
                    case DocumentType.Refund:
                        if (HasNonzeroRegistrations)
                        {
                            _atolProtocol.SwitchToMode(1, MODE_PASSWD);
                            _atolProtocol.CreateCommand(0x92);
                            _atolProtocol.AddByte(0);
                            _atolProtocol.AddBCD(docType == DocumentType.Sale ? 1 : 2, 1);
                            _atolProtocol.Execute();
                        }
                        break;
                    default:
                        if (IsSlipMode())
                        {
                            // ������������ � ����� �����������
                            _atolProtocol.SwitchToMode(1, MODE_PASSWD);
                            // ��������� ���������� �������
                            _atolProtocol.IsSlipMode = true;
                        }
                        break;
                }

                _currDocType = docType;
                _docAmount = 0;
            });
        }

        protected override void OnCloseDocument(bool cutPaper)
        {
            
            ExecuteDriverCommand(delegate()
            {
                switch (_currDocType)
                {
                    case DocumentType.Sale:
                    case DocumentType.Refund:
                        if (HasNonzeroRegistrations)
                        {
                            _atolProtocol.CreateCommand(0x4A);
                            _atolProtocol.AddByte(0);
                            _atolProtocol.AddByte(1);
                            _atolProtocol.AddBCD(0, 5);
                            _atolProtocol.Execute();
                        }
                        else
                        {
                            _atolProtocol.SwitchToMode(2, MODE_PASSWD);
                            _atolProtocol.ExecuteCommand(0x73);
                        }
                        break;
                    case DocumentType.PayingIn:
                        _atolProtocol.SwitchToMode(1, MODE_PASSWD);
                        _atolProtocol.CreateCommand(0x49);
                        _atolProtocol.AddByte(0);
                        _atolProtocol.AddBCD(_docAmount, 5);
                        _atolProtocol.Execute();
                        break;
                    case DocumentType.PayingOut:
                        _atolProtocol.SwitchToMode(1, MODE_PASSWD);
                        _atolProtocol.CreateCommand(0x4F);
                        _atolProtocol.AddByte(0);
                        _atolProtocol.AddBCD(_docAmount, 5);
                        _atolProtocol.Execute();
                        break;
                    case DocumentType.Other:
                        if (IsSlipMode())
                        {
                            // ������� ���������
                            _atolProtocol.CreateCommand(0x8F);
                            _atolProtocol.AddByte(2);
                            _atolProtocol.AddBytes(new byte[] { 0x1B, 0x0C, 3});
                            _atolProtocol.Execute();

                            // ��������� ���������� �������
                            _atolProtocol.IsSlipMode = false;
                        }
                        else
                        {
                            // ������ �������
                            _atolProtocol.SwitchToMode(2, MODE_PASSWD);
                            _atolProtocol.ExecuteCommand(0x73);

                            // �������� � �������
                            if (cutPaper && _deviceModel == AtolModel.FPrint22K)
                            {
                                _atolProtocol.CreateCommand(0x75);
                                _atolProtocol.AddByte(1);
                                _atolProtocol.Execute();
                            }
                        }
                        break;
                    case DocumentType.XReport:
                    case DocumentType.ZReport:
                    case DocumentType.SectionsReport:
                        PrintReport(_currDocType);
                        break;
                }
                _docAmount = 0;
            });
        }

        protected override void OnOpenDrawer()
        {
            ExecuteDriverCommand(delegate()
            {
                _atolProtocol.ExecuteCommand(0x80);

                /*
                // ������ �������
                _atolProtocol.SwitchToMode(4, MODE_PASSWD);
                _atolProtocol.CreateCommand(0x46);
                _atolProtocol.AddByte(2);       // �������
                _atolProtocol.AddBCD(1, 2);// ���
                _atolProtocol.AddByte(56);       // ����
                _atolProtocol.Execute();
                 */ 
            });
        }

        protected override void OnPrintString(String source, FontStyle style)
        {
            ExecuteDriverCommand(delegate()
            {
                try
                {
                    PrintStringInternal(source, style);
                }
                catch (DeviceErrorException E)
                {
                    if (!IsSlipMode() || E.ErrorCode != 103)
                        throw;

                    _atolProtocol.CreateCommand(0x8F);
                    _atolProtocol.AddByte(2);
                    _atolProtocol.AddBytes(new byte[] { 0x1B, 0x0C, 3 });
                    _atolProtocol.Execute();

                    // �������� ��������� ������
                    do
                    {
                        if (PrinterStatus.PaperOut == PaperOutStatus.Present)
                            break;
                        System.Threading.Thread.Sleep(1000);
                    }
                    while (true);
                    PrintStringInternal(source, style);
                }
            });
        }

        protected override void OnPrintBarcode(String barcode, AlignOptions align,
            bool readable)
        {
            ExecuteDriverCommand(delegate()
            {
                byte[] nRaster = GetBarcodeData(ref barcode, GetDeviceParams().BarcodeWidth);
                _atolProtocol.CreateCommand(0x8E);
                _atolProtocol.AddByte(1);     // �������� �� ������� �����
                _atolProtocol.AddBCD(25 * GetDeviceParams().BarcodeWidth, 2);  // ���������� �������� ������
                switch (align)
                {
                    case AlignOptions.Left:
                        _atolProtocol.AddBCD(0, 2);   // ��������
                        break;
                    case AlignOptions.Center:
                        _atolProtocol.AddBCD((GetDeviceParams().TapeWidth - 95 * GetDeviceParams().BarcodeWidth) / 2 - 1, 2);   // ��������
                        break;
                    case AlignOptions.Right:
                        _atolProtocol.AddBCD(GetDeviceParams().TapeWidth - 95 * GetDeviceParams().BarcodeWidth - 1, 2);   // ��������
                        break;
                }

                _atolProtocol.AddBytes(nRaster);// �����
                _atolProtocol.Execute();

                if (readable)
                {
                    string barcodeString = "";
                    switch (align)
                    {
                        case AlignOptions.Left:
                            barcodeString = new string(' ', 2 * GetDeviceParams().BarcodeWidth);
                            break;
                        case AlignOptions.Center:
                            barcodeString = new string(' ', (GetDeviceParams().StringLen - barcode.Length) / 2);
                            break;
                        case AlignOptions.Right:
                            barcodeString = new string(' ', GetDeviceParams().StringLen - barcode.Length - 2 * GetDeviceParams().BarcodeWidth);
                            break;
                    }
                    barcodeString += barcode;
                    PrintStringInternal(barcodeString, FontStyle.Regular);
                }
            });
        }

        protected override void OnPrintImage(System.Drawing.Bitmap image, AlignOptions align)
        {
            ExecuteDriverCommand(delegate() 
            {
                int index = LoadImage(image);
                PrintImage(index);
            });
        }

        protected override void OnRegistration(String commentary, UInt32 quantity, UInt32 amount,
            Byte section)
        {
            ExecuteDriverCommand(delegate()
            {
                if (HasNonzeroRegistrations)
                    switch (_currDocType)
                    {
                        case DocumentType.Sale:
                            _atolProtocol.CreateCommand(0x52);
                            _atolProtocol.AddByte(0);
                            _atolProtocol.AddBCD(amount, 5);
                            _atolProtocol.AddBCD(quantity, 5);
                            _atolProtocol.AddBCD(section, 1);
                            _atolProtocol.Execute();
                            break;
                        case DocumentType.Refund:
                            _atolProtocol.CreateCommand(0x57);
                            _atolProtocol.AddByte(2);
                            _atolProtocol.AddBCD(amount, 5);
                            _atolProtocol.AddBCD(quantity, 5);
                            _atolProtocol.Execute();
                            break;
                        default:
                            break;
                    }
            });
        }

        protected override void OnPayment(UInt32 amount, FiscalPaymentType paymentType)
        {
            ExecuteDriverCommand(delegate()
            {
                if (HasNonzeroRegistrations && (_currDocType == DocumentType.Sale || 
                    _currDocType == DocumentType.Refund))
                {
                    _atolProtocol.CreateCommand(0x99);
                    _atolProtocol.AddByte(0);
                    switch (paymentType)
                    {
                        case FiscalPaymentType.Cash:
                            _atolProtocol.AddBCD(1, 1);
                            break;
                        case FiscalPaymentType.Card:
                            _atolProtocol.AddBCD(2, 1);
                            break;
                        case FiscalPaymentType.Other1:
                            _atolProtocol.AddBCD(3, 1);
                            break;
                        case FiscalPaymentType.Other2:
                            _atolProtocol.AddBCD(4, 1);
                            break;
                        case FiscalPaymentType.Other3:
                            _atolProtocol.AddBCD(5, 1);
                            break;
                    }
                    _atolProtocol.AddBCD(amount, 5);
                    _atolProtocol.Execute();
                    _docAmount += amount;
                }
                else
                    _docAmount += amount;
            });
        }

        protected override void OnCash(UInt32 amount)
        {
            ExecuteDriverCommand(delegate()
            {
                _docAmount += amount;
            });
        }

        #endregion

        #region �������

        public override event EventHandler<FiscalBreakEventArgs> FiscalBreak;

        public override event EventHandler<PrinterBreakEventArgs> PrinterBreak;

        #endregion

        #region ��������

        public override DateTime CurrentTimestamp
        {
            get
            {
                DateTime dateTime = DateTime.Now;
                ExecuteDriverCommand(delegate()
                {
                    // ������ ���������
                    _atolProtocol.ExecuteCommand(0x3F);
                    dateTime = new DateTime((int)_atolProtocol.GetFromBCD(4, 1) + 2000,
                        (int)_atolProtocol.GetFromBCD(5, 1),
                        (int)_atolProtocol.GetFromBCD(6, 1),
                        (int)_atolProtocol.GetFromBCD(7, 1),
                        (int)_atolProtocol.GetFromBCD(8, 1),
                        (int)_atolProtocol.GetFromBCD(9, 1));
                });
                return dateTime;
            }
            set
            {
                ExecuteDriverCommand(delegate()
                {
                    // ���������������� ����
                    _atolProtocol.CreateCommand(0x64);
                    _atolProtocol.AddBCD(value.Day, 1);
                    _atolProtocol.AddBCD(value.Month, 1);
                    if (value.Year < 2000)
                        _atolProtocol.AddBCD(value.Year - 1900, 1);
                    else
                        _atolProtocol.AddBCD(value.Year - 2000, 1);
                    _atolProtocol.Execute();

                    // ���������������� �������
                    _atolProtocol.CreateCommand(0x4B);
                    _atolProtocol.AddBCD(value.Hour, 1);
                    _atolProtocol.AddBCD(value.Minute, 1);
                    _atolProtocol.AddBCD(value.Second, 1);
                    _atolProtocol.Execute();
                    _atolProtocol.Execute();
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
                    // ������ ���������
                    _atolProtocol.ExecuteCommand(0x3F);
                    long sNo = _atolProtocol.GetFromBCD(11, 4);
                    serialNo = sNo.ToString();
                });
                return new FiscalDeviceInfo(DeviceNames.ecrTypeAtol, serialNo);
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
                    // ������ ���������
                    _atolProtocol.ExecuteCommand(0x3F);

                    bOpenedShift = (_atolProtocol.Response[10] & 2) == 2;
                    bFiscalized = (_atolProtocol.Response[10] & 1) == 1;
                    bLocked = ((_atolProtocol.Response[18] & 0x0F) == 5) && ((_atolProtocol.Response[18] >> 4) == 1);
                    bOverShift = false;

                    // ���������� � ��
                    _atolProtocol.ExecuteCommand(0x4D);
                    nCashInDrawer = (int)_atolProtocol.GetFromBCD(2, 7);

                    // ����� ���������
                    if (_currDocType == DocumentType.PayingIn || _currDocType == DocumentType.PayingOut)
                        nDocAmount = (int)_docAmount;
                    else
                    {
                        _atolProtocol.CreateCommand(0x91);
                        _atolProtocol.AddByte(0x14);
                        _atolProtocol.AddByte(0);
                        _atolProtocol.AddByte(0);
                        _atolProtocol.Execute();
                        nDocAmount = (int)_atolProtocol.GetFromBCD(3, 5);
                    }
                });
                return new FiscalStatusFlags(bOpenedShift, bOverShift, bLocked, bFiscalized, (ulong)nDocAmount, (ulong)nCashInDrawer);
            }
        }

        public override PrintableDeviceInfo PrinterInfo
        {
            get
            {
                return new PrintableDeviceInfo(new PrintableTapeWidth(GetDeviceParams().StringLen, GetDeviceParams().SlipLineSize), true);
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
                    // ������ ���� ���������
                    _atolProtocol.ExecuteCommand(0x45);

                    if ((_atolProtocol.Response[3] & 0x01) == 0)
                        poStatus = PaperOutStatus.Present;
                    else
                        poStatus = PaperOutStatus.OutActive;

                    // ������ ���������
                    _atolProtocol.ExecuteCommand(0x3F);

                    bDrawerOpened = (_atolProtocol.Response[10] & 4) == 4;
                    bPrinting = (_atolProtocol.Response[18] >> 4) != 0;

                    if ((_atolProtocol.Response[18] & 1) == 1)
                        bDocOpened = _atolProtocol.Response[23] != 0;
                });
                return new PrinterStatusFlags(bPrinting, poStatus, bDocOpened, bDrawerOpened);
            }
        }

        #endregion

        #region ������

        public override void FiscalReport(FiscalReportType reportType, bool full, params object[] reportParams)
        {
            ExecuteDriverCommand(delegate()
            {
                _atolProtocol.SwitchToMode(5, TaxerPassword);

                switch (reportType)
                {
                    case FiscalReportType.ByDates:
                        _atolProtocol.CreateCommand(0x65);
                        if (full)
                            _atolProtocol.AddByte(1);
                        else
                            _atolProtocol.AddByte(0);

                        DateTime dtParam = (DateTime)(reportParams[0]);
                        _atolProtocol.AddBCD(dtParam.Day, 1);
                        _atolProtocol.AddBCD(dtParam.Month, 1);
                        if (dtParam.Year < 2000)
                            _atolProtocol.AddBCD(dtParam.Year - 1900, 1);
                        else
                            _atolProtocol.AddBCD(dtParam.Year - 2000, 1);

                        dtParam = (DateTime)(reportParams[1]);
                        _atolProtocol.AddBCD(dtParam.Day, 1);
                        _atolProtocol.AddBCD(dtParam.Month, 1);
                        if (dtParam.Year < 2000)
                            _atolProtocol.AddBCD(dtParam.Year - 1900, 1);
                        else
                            _atolProtocol.AddBCD(dtParam.Year - 2000, 1);
                        _atolProtocol.Execute();
                        break;
                    case FiscalReportType.ByShifts:
                        _atolProtocol.CreateCommand(0x66);
                        if (full)
                            _atolProtocol.AddByte(1);
                        else
                            _atolProtocol.AddByte(0);

                        _atolProtocol.AddBCD((long)(int)(reportParams[0]), 2);
                        _atolProtocol.AddBCD((long)(int)(reportParams[1]), 2);
                        _atolProtocol.Execute();
                        break;
                }

                byte nMode = 5;
                byte nSubMode = 2;
                byte nFlags = 0;

                if (WaitForStateChange(ref nMode, ref nSubMode, ref nFlags))
                {
                    if ((nMode == 5) && (nSubMode == 0))
                    {
                        if ((nFlags & 0x01) == 0x01)
                            throw new DeviceErrorException(103);            // ��� ������
                        else if ((nFlags & 0x02) == 0x02)
                            throw new DeviceErrorException(104);            // ��� ����� � ���������
                    }
                    else
                        throw new DeviceErrorException(254);                // ������ ������ ����������

                }
            });
        }

        public override void Fiscalization(int newPassword, long registrationNumber, long taxPayerNumber)
        {
            ExecuteDriverCommand(delegate()
            {
                _atolProtocol.SwitchToMode(5, TaxerPassword);
                _atolProtocol.CreateCommand(0x62);
                _atolProtocol.AddBCD(registrationNumber, 5);
                _atolProtocol.AddBCD(taxPayerNumber, 6);
                _atolProtocol.AddBCD(newPassword, 4);
                _atolProtocol.Execute();
            });
        }

        public override void GetLifetime(out DateTime firstDate, out DateTime lastDate, out int firstShift, out int lastShift)
        {
            DateTime _firstDate = DateTime.Now;
            DateTime _lastDate = DateTime.Now;
            int _firstShift = 1;
            int _lastShift = 2000;
            ExecuteDriverCommand(delegate()
            {
                _atolProtocol.SwitchToMode(5, TaxerPassword);

                _atolProtocol.ExecuteCommand(0x63);

                int nDay = (int)_atolProtocol.GetFromBCD(4, 1);
                int nMonth = (int)_atolProtocol.GetFromBCD(5, 1);
                int nYear = (int)_atolProtocol.GetFromBCD(6, 1);
                _firstDate = new DateTime(nYear < 97 ? nYear += 1900 : nYear += 2000, nMonth, nDay);

                nDay = (int)_atolProtocol.GetFromBCD(7, 1);
                nMonth = (int)_atolProtocol.GetFromBCD(8, 1);
                nYear = (int)_atolProtocol.GetFromBCD(9, 1);
                _lastDate = new DateTime(nYear < 97 ? nYear += 1900 : nYear += 2000, nMonth, nDay);

                _firstShift = (int)_atolProtocol.GetFromBCD(10, 2);
                _lastShift = (int)_atolProtocol.GetFromBCD(12, 2);
            });

            firstDate = _firstDate;
            lastDate = _lastDate;
            firstShift = _firstShift;
            lastShift = _lastShift;
        }

        #endregion

        #region ��������� ICommunicationPortProvider

        public EasyCommunicationPort GetCommunicationPort()
        {
            return Port;
        }

        #endregion
    }
}
