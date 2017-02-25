using System;
using System.Collections.Generic;
using System.Text;
using DevicesBase;
using DevicesBase.Helpers;
using DevicesCommon;
using DevicesCommon.Helpers;
using System.Drawing.Imaging;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace Stroke
{
    internal enum StrokeType { strokeFR, strokeMini, elves, stroke500, strokeCombo, strokeM, other };

    [Flags]
    internal enum CommandFlags 
    {
        None = 0,
        CheckActive = 1, 
        Fiscal = 2, 
        TapePrinter = 4,
        CheckPaperStatus = 8
    };

    internal class DeviceErrorException : Exception
    {
        private short _errorCode;

        public short ErrorCode
        {
            get { return _errorCode; }
        }

        public DeviceErrorException(short errorCode)
        {
            _errorCode = errorCode;
        }
    }

    [Serializable]
    [FiscalDeviceAttribute(DeviceNames.ecrTypeStroke)]
    public class StrokeFiscalDevice : CustomFiscalDevice
    {
        #region ���������

        // ������������ ������� ��
        private string[] stateModes = new string[] {"������� �����", "������ ������", 
            "�������� �����, 24 ���� �� ���������", "�������� �����, 24 ���� ���������", "�������� �����",
            "���������� �� ������������� ������ ���������� ����������", "�������� ������������� ����� ����",
            "���������� ��������� ��������� ���������� �����", "�������� ��������",
            "����� ���������� ���������������� ���������", "�������� ������", 
            "������ ������� ����������� ������", "������ ������ ����", 
            "������ � ���������� ���������� ����������", "������ ����������� ���������",
            "���������� ���������� �������� �����������"};

        // ������������ ���������� ��
        private string[] stateSubModes = new string[] {"�������", "�������", "������� �������", 
            "������� �������", "�������� �����", "������� �����", "������ ������������� ���������" };

        // ������ ��������� �� ���������
        private const uint DEF_OPERATOR_PASSWD = 30;

        // ������� �������� ����� ��������� ����������� ���������
        private const long SLIP_WAIT_TIMEOUT = 3 * 60 * 1000;

        private const short PAPER_OUT_PASSIVE = 200;
        private const short PAPER_OUT_ACTIVE = 201;

        #endregion

        #region ����

        private uint[] _paymentAmount = new uint[5]; 
        private StrokeType _deviceType;
        private DocumentType _currDocType = DocumentType.Other;
        private StrokeProtocol _protocol;
        private PrintableDeviceInfo _printerInfo = new PrintableDeviceInfo(new PrintableTapeWidth(40, 0),
            true);

        private int _maxSlipLines = 40;
        private int _slipLineNo = 0;
        private bool _firstSlip = false;
        private bool _needPrintTopMargin = false;

        #endregion

        #region �����������

        public StrokeFiscalDevice()
            : base()
        {
            AddSpecificError(1, "���������� ���������� ��1, ��2 ��� ����");
            AddSpecificError(2, "����������� ��1");
            AddSpecificError(3, "����������� ��2");
            AddSpecificError(4, "������������ ��������� � ������� ��������� � ��");
            AddSpecificError(5, "��� ����������� ������");
            AddSpecificError(6, "�� � ������ ������ ������");
            AddSpecificError(7, "������������ ��������� � ������� ��� ������ ���������� ��");
            AddSpecificError(8, "������� �� �������������� � ������ ���������� ��");
            AddSpecificError(9, "������������ ����� �������");
            AddSpecificError(10, "������ ������ �� BCD");
            AddSpecificError(11, "���������� ������ ������ �� ��� ������ �����");
            AddSpecificError(17, "�� ������� ��������");
            AddSpecificError(18, "��������� ����� ��� ������");
            AddSpecificError(19, "������� ���� ������ ���� ��������� ������ � ��");
            AddSpecificError(20, "������� ������� ������ �� �����������");
            AddSpecificError(21, "����� ��� �������");
            AddSpecificError(22, "����� �� �������");
            AddSpecificError(23, "����� ������ ����� ������ ������ ��������� �����");
            AddSpecificError(24, "���� ������ ����� ������ ���� ��������� �����");
            AddSpecificError(25, "��� ������ � ��");
            AddSpecificError(26, "������� ��������������� �� �����������");
            AddSpecificError(27, "��������� ����� �� ������");
            AddSpecificError(28, "� �������� ��������� ���� ������������ ������");
            AddSpecificError(29, "���������� �������� ������ ������� ������");
            AddSpecificError(30, "������� ��������������� �� �����������");
            AddSpecificError(31, "����������� ������ ���������");
            AddSpecificError(32, "������������ ��������� �������� ��� ����������");
            AddSpecificError(33, "���������� ����� ������ ����������� ��������� ��������");
            AddSpecificError(34, "�������� ����");

            AddSpecificError(35, "��� ������ �����������");
            AddSpecificError(36, "������� ����������� �����������");
            AddSpecificError(37, "��� ����������� � ������������� �������");
            AddSpecificError(38, "�������� �������� ����� ������ ����� ����");
            AddSpecificError(43, "���������� �������� ���������� �������");
            AddSpecificError(44, "���������� ����� (��������� ������� ����������)");
            AddSpecificError(45, "����� ���� �� ������ ������ ����� ������");
            AddSpecificError(46, "� �� ��� ����� ��� �������");
            AddSpecificError(48, "�� ������������, ���� ����� ������ ��");
            AddSpecificError(50, "��������� ���������� ������ �������");

            AddSpecificError(51, "������������ ��������� � �������");
            AddSpecificError(52, "��� ������");
            AddSpecificError(53, "������������ �������� ��� ������ ����������");
            AddSpecificError(54, "������������ ��������� � ������� ��� ������ ���������� ��");
            AddSpecificError(55, "������� �� �������������� � ������ ���������� ��");
            AddSpecificError(56, "������ � ���");
            AddSpecificError(57, "���������� ������ �� ��");
            AddSpecificError(58, "������������ ���������� �� ��������� � �����");
            AddSpecificError(59, "������������ ���������� � �����");
            AddSpecificError(60, "����: �������� ��������������� �����");
            AddSpecificError(61, "����� �� ������� - �������� ����������");

            AddSpecificError(62, "������������ ���������� �� ������� � �����");
            AddSpecificError(63, "������������ ���������� �� ������� � �����");
            AddSpecificError(64, "������������ ��������� ������");
            AddSpecificError(65, "������������ ��������� ������ ���������");
            AddSpecificError(66, "������������ ��������� ������ ����� 2");
            AddSpecificError(67, "������������ ��������� ������ ����� 3");
            AddSpecificError(68, "������������ ��������� ������ ����� 4");
            AddSpecificError(69, "����� ���� ����� ������ ������ ����� ����");
            AddSpecificError(70, "�� ������� ���������� � �����");
            AddSpecificError(71, "������������ ���������� �� ������� � �����");
            AddSpecificError(72, "������������ ����� ����");
            AddSpecificError(73, "�������� ���������� � �������� ���� ������� ����");

            AddSpecificError(74, "������ ��� - �������� ����������");
            AddSpecificError(75, "����� ���� ����������");
            AddSpecificError(76, "������������ ���������� �� ������� ������� � �����");
            AddSpecificError(77, "�������� ����������� ������� ����� ������ ����� ����");
            AddSpecificError(78, "����� ��������� 24 ����");
            AddSpecificError(79, "�������� ������");
            AddSpecificError(80, "���� ������ ���������� �������");
            AddSpecificError(81, "������������ ���������� ��������� � �����");
            AddSpecificError(82, "������������ ���������� �� ���� ������ 2 � �����");
            AddSpecificError(83, "������������ ���������� �� ���� ������ 3 � �����");
            AddSpecificError(84, "������������ ���������� �� ���� ������ 4 � �����");
            AddSpecificError(85, "��� ������ - �������� ����������");

            AddSpecificError(86, "��� ��������� ��� �������");
            AddSpecificError(87, "����: ���������� �������� ���� �� ��������� � ��");

            AddSpecificError(88, "�������� ������� ����������� ������");
            AddSpecificError(89, "�������� ������ ������ ����������");
            AddSpecificError(90, "������ ��������� ���������� � ����");

            AddSpecificError(91, "������������ ��������� ��������");
            AddSpecificError(92, "�������� ���������� 24�");
            AddSpecificError(93, "������� �� ����������");
            AddSpecificError(94, "������������ ��������");
            AddSpecificError(95, "������������� ���� ����");
            AddSpecificError(96, "������������ ��� ���������");
            AddSpecificError(97, "������������ ��������� ����");
            AddSpecificError(98, "������������ ��������� ����������");
            AddSpecificError(99, "������������ ��������� ������");
            AddSpecificError(100, "�� �����������");
            AddSpecificError(101, "�� ������� ����� � ������");
            AddSpecificError(102, "������������ ����� � ������");
            AddSpecificError(103, "������ ����� � ��");
            AddSpecificError(104, "�� ������� ����� �� ������� �������");
            AddSpecificError(105, "������������ �� ������� �������");
            AddSpecificError(106, "������ ������� � ������ ������ �� I2C");

            AddSpecificError(107, "��� ������� �����");
            AddSpecificError(108, "��� ����������� �����");
            AddSpecificError(109, "�� ������� ����� �� ������");
            AddSpecificError(110, "������������ ����� �� ������");
            AddSpecificError(111, "������������ �� ������� � �����");
            AddSpecificError(112, "������������ ��");
            AddSpecificError(113, "������ ���������");
            AddSpecificError(114, "������� �� �������������� � ������ ���������");
            AddSpecificError(115, "������� �� �������������� � ������ ������");
            AddSpecificError(116, "������ ���");
            AddSpecificError(117, "������ �������");
            AddSpecificError(118, "������ ��������: ��� ��������� � ��������������");
            AddSpecificError(119, "������ ��������: ��� ������� � ��������");
            AddSpecificError(120, "������ ��");
            AddSpecificError(121, "������ ��");
            AddSpecificError(122, "���� �� �������������");
            AddSpecificError(123, "������ ������������");
            AddSpecificError(124, "�� ��������� ����");
            AddSpecificError(125, "�������� ������ ����");
            AddSpecificError(126, "�������� �������� � ���� �����");
            AddSpecificError(127, "������������ ��������� ����� ����");
            AddSpecificError(128, "������ ����� � ��");
            AddSpecificError(129, "������ ����� � ��");
            AddSpecificError(130, "������ ����� � ��");
            AddSpecificError(131, "������ ����� � ��");
            AddSpecificError(132, "������������ ����������");
            AddSpecificError(133, "������������ �� �������� � �����");
            AddSpecificError(134, "������������ �� �������� � �����");
            AddSpecificError(135, "������������ �� ��������� ������ � �����");
            AddSpecificError(136, "������������ �� ��������� ������� � �����");
            AddSpecificError(137, "������������ �� �������� � �����");
            AddSpecificError(138, "������������ �� ��������� � ����");
            AddSpecificError(139, "������������ �� ������� � ����");
            AddSpecificError(140, "������������� ���� �������� � ����");
            AddSpecificError(141, "������������� ���� ������ � ����");
            AddSpecificError(142, "������� ���� ����");
            AddSpecificError(143, "����� �� ���������������");
            AddSpecificError(144, "���� ��������� ������, ������������� � ����������");
            AddSpecificError(145, "����� �� ������� ���� ������ ��� ������ ���������� ������");
            AddSpecificError(146, "��������� �����");
            AddSpecificError(147, "�������������� ��� ������ �������");
            AddSpecificError(148, "�������� ����� �������� � ����");
            AddSpecificError(160, "������ ����� � ����");
            AddSpecificError(161, "���� �����������");
            AddSpecificError(162, "����: ������������ ������ ��� �������� �������");
            AddSpecificError(163, "������������ ��������� ����");
            AddSpecificError(164, "������ ����");
            AddSpecificError(165, "������ �� � ������� ����");
            AddSpecificError(166, "�������� ��������� ������ ����");
            AddSpecificError(167, "���� �����������");
            AddSpecificError(168, "����: �������� ���� � �����");
            AddSpecificError(169, "����: ��� ����������� ������");
            AddSpecificError(170, "������������ ���� (������������� ���� ���������)");
            AddSpecificError(176, "����: ������������ � ��������� ����������");
            AddSpecificError(177, "����: ������������ � ��������� �����");
            AddSpecificError(178, "���� ��� ��������������");
            AddSpecificError(192, "�������� ���� � ������� (����������� ���� � �����)");
            AddSpecificError(193, "����: �������� ����� � �������� �������� ������");
            AddSpecificError(194, "���������� ���������� � ����� �������");
            AddSpecificError(195, "������������ ������ ���� � ����");
            AddSpecificError(196, "������������ ������� ����");
            AddSpecificError(197, "����� ����������� ��������� ����");
            AddSpecificError(198, "���������� �������� �����������");
            AddSpecificError(199, "���� �� ������������� � ������ ������");

            AddSpecificError(PAPER_OUT_PASSIVE, "����������� ������ � ��������");
            AddSpecificError(PAPER_OUT_ACTIVE, "����������� ������ � ��������. ���������� ������� ��������");

            AddSpecificError(-1, "����������� ������");
        }

        #endregion

        #region ���������� ������

        // ������ �������� � ������� ��
        private void SetTableValue(byte table, int row, byte field, string value, int size)
        {            
            _protocol.CreateCommand(0x1E, DEF_OPERATOR_PASSWD);
            _protocol.AddByte(table);
            _protocol.AddInt(row, 2);
            _protocol.AddByte(field);
            _protocol.AddString(value, size);
            _protocol.ExecuteCommand();
        }

        // ��������� ������������ ��������
        private void CloseNonFiscalDoc()
        {
            // ������ ������������ �������
            DrawGraphicFooter();
            // ������ �������
            DrawFooter();
            // �������� �����
            _protocol.CreateCommand(0x29, DEF_OPERATOR_PASSWD);
            _protocol.AddBytes(0x02, 0x04);
            _protocol.ExecuteCommand();
            // ������� �����
            _protocol.CreateCommand(0x25, DEF_OPERATOR_PASSWD);
            _protocol.AddByte(1);
            _protocol.ExecuteCommand();
            // ������ ����� �����
            DrawHeader();
        }

        // �������� ����������� ��������� � ��������� ��������� ������ ����
        private void CloseFiscalDoc()
        {
            // ��������� ������� �������� ����� ���������
            int currReceiptNo = _protocol.GetReceiptNo(DEF_OPERATOR_PASSWD);

            try
            {
                _protocol.CreateCommand(0x85, DEF_OPERATOR_PASSWD);
                _protocol.AddInt(_paymentAmount[(int)FiscalPaymentType.Cash], 5);
                _protocol.AddInt(_paymentAmount[(int)FiscalPaymentType.Card], 5);
                _protocol.AddInt(_paymentAmount[(int)FiscalPaymentType.Other1], 5);
                _protocol.AddInt(_paymentAmount[(int)FiscalPaymentType.Other2], 5);
                _protocol.AddInt(0, 2);
                _protocol.AddEmptyBytes(4);
                _protocol.AddString(string.Empty, 40);
                _protocol.ExecuteCommand(true);
            }
            catch (TimeoutException)
            {
                _protocol.WriteDebugLine("������� �������� ������ ��� ���������� ������� �������� ���������");
                _protocol.WriteDebugLine("����������� �������� ����� ���������: " + currReceiptNo);
                // �������� ������ ���������
                int newReceiptNo = _protocol.GetReceiptNo(DEF_OPERATOR_PASSWD);
                _protocol.WriteDebugLine("������� �������� ����� ���������: " + newReceiptNo);
                if (newReceiptNo > currReceiptNo)
                    _protocol.WriteDebugLine("�������� ��� ������ �� ��");
                else
                {
                    _protocol.WriteDebugLine("�������� �� ��� ������ �� ��");
                    throw;
                }
            }
        }

        // ���������� ����� ���������� ������ ������ ���������
        private void DoAfterClose()
        {
            // ������ ������������ ���������
            DrawGraphicHeader();
            // ��������� ���������
            _paymentAmount = new uint[5]; 
            _currDocType = DocumentType.Other;
        }

        // ������ ���������, ���� �� ������
        private void CancelOpenedDocument(bool fiscal)
        {
            StrokePrinterFlags printerFlags = _protocol.GetPrinterFlags(DEF_OPERATOR_PASSWD);
            if (printerFlags.Mode == 8 && fiscal)
                // ������� ������ ���������
                _protocol.ExecuteCommand(0x88, DEF_OPERATOR_PASSWD, true);
            else if (printerFlags.Mode != 8 && !fiscal)
            {
                _protocol.CreateCommand(0x17, DEF_OPERATOR_PASSWD);
                _protocol.AddByte(2);
                _protocol.AddString("��� �����������", 40);
                _protocol.ExecuteCommand();
                CloseNonFiscalDoc();
            }
            else
                return;
            DoAfterClose();
        }

        // ����������� ���� ���������� � ��������� ����� ������
        private void SetDeviceType()
        {
            _protocol.ExecuteCommand(0xFC);
            if (_protocol.Response[2] == 0)
            {
                switch (_protocol.Response[6])
                {
                    case 0:
                    case 1:
                    case 4: // �����-�� � ��� �����������
                        _deviceType = StrokeType.strokeFR;
                        PrinterInfo.TapeWidth.MainPrinter = 36;
                        break;
                    case 2:
                    case 6: // �����-����
                        _deviceType = StrokeType.elves;
                        PrinterInfo.TapeWidth.MainPrinter = 32;
                        break;
                    case 7: // �����-����
                        _deviceType = StrokeType.strokeMini;
                        PrinterInfo.TapeWidth.MainPrinter = 50;
                        break;
                    case 9: // �����-�����-��-�
                    case 12: // �����-�����-��-� (������ 02)
                        _deviceType = StrokeType.strokeCombo;
                        PrinterInfo.TapeWidth.MainPrinter = 48;
                        PrinterInfo.TapeWidth.AdditionalPrinter1 = 48;
                        int lineInterval = 24;
                        double Y_UNIT_SIZE = 0.176;
                        if (_printerInfo.SlipFormLength > 0)
                            _maxSlipLines = (int)((_printerInfo.SlipFormLength - 20) / (Y_UNIT_SIZE * lineInterval));
                        break;
                    case 250: // �����-�
                        _deviceType = StrokeType.strokeM;
                        PrinterInfo.TapeWidth.MainPrinter = 48;
                        break;
                    default: // ������ ������ �������������
                        _deviceType = StrokeType.other;
                        PrinterInfo.TapeWidth.MainPrinter = 50;
                        break;
                }
            }
            else if (_protocol.Response[2] == 5)
            {
                PrinterInfo.TapeWidth.MainPrinter = 32;
                _deviceType = StrokeType.stroke500;
            }
        }

        // �������� ��������� ������ � ��������� ���� ������
        private void CheckPaperStatus()
        {
            switch (_protocol.GetPrinterFlags(DEF_OPERATOR_PASSWD).tapePaperStatus)
            {
                case PaperOutStatus.OutPassive:
                    // ������ �����������
                    throw new DeviceErrorException(PAPER_OUT_PASSIVE);
                case PaperOutStatus.OutActive:
                    // ������ �����������, ������� �� ���������
                    throw new DeviceErrorException(PAPER_OUT_ACTIVE);
                case PaperOutStatus.OutAfterActive:
                    // ������ ������������, ���������� ��������� ������� ����������� ������
                    _protocol.ExecuteCommand(0xB0, DEF_OPERATOR_PASSWD, true);
                    break;
            }
        }

        // ��������� ������ ��� ������ �����������
        private byte[] GetImageBytes(System.Drawing.Bitmap image, out int stride)
        {
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);
            BitmapData bmpData = image.LockBits(rect, ImageLockMode.ReadOnly, image.PixelFormat);
            byte[] imageBytes = new byte[image.Height * bmpData.Stride];
            stride = bmpData.Stride;
            try
            {
                System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, imageBytes, 0, bmpData.Height * stride);
                if ((ulong)image.Palette.Entries[0].ToArgb() == 0xff000000)
                {
                    for (int rowNo = 0; rowNo < image.Height; rowNo++)
                        for (int i = 0; i < stride; i++)
                        {
                            if (i * 8 < image.Width)
                                imageBytes[rowNo * stride + i] = (byte)(imageBytes[rowNo * stride + i] ^ 0xFF);
                            else
                            {
                                // ����������� ���� ��������� �� ������� �����������
                                int bitsInvert = (i * 8) - image.Width;
                                int invertMask = 0xFF >> (8 - bitsInvert);
                                imageBytes[rowNo * stride + i] = (byte)(imageBytes[rowNo * stride + i] ^ invertMask);
                            }
                        }
                }
                return imageBytes;
            }
            finally
            {
                image.UnlockBits(bmpData);
            }
        }

        // ��������� ������ ��� ������ �����������
        private byte[] GetColorImageBytes(System.Drawing.Bitmap image, out int stride)
        {
            System.Drawing.Bitmap newImage = new System.Drawing.Bitmap(image.Width, image.Height, PixelFormat.Format1bppIndexed);
            BitmapData bmpData = newImage.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height), 
                ImageLockMode.ReadOnly, newImage.PixelFormat);
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
                            b += (byte)(0x01 << (7 - x));
                    }
                    imageBytes[y * stride + i] = b;
                }
            }
            return imageBytes;
        }

        private bool WaitForSlipState(bool needState, long slipWaitTimeout)
        {
            long waitTimeout = slipWaitTimeout * 10000;
            long startTicks = DateTime.Now.Ticks;
            StrokePrinterFlags printerFlags = new StrokePrinterFlags();
            do
            {
                printerFlags = _protocol.GetPrinterFlags(DEF_OPERATOR_PASSWD);

                if (printerFlags.slipPaperPresent != needState)
                    System.Threading.Thread.Sleep(100);
                else
                    return true;
            }
            while (DateTime.Now.Ticks - startTicks < waitTimeout);
            //m_nErrorCode = PAPER_OUT_PASSIVE;
            return false;
        }

        private void StartSlip()
        {
            // �������� ����� ����������� ���������
            _protocol.ExecuteCommand(0x7C, DEF_OPERATOR_PASSWD);
            _firstSlip = true;
            _slipLineNo = 1;
        }

        private void EndSlip()
        {
            // ���� �������� ������, �������
            if (_slipLineNo <= 1)
                return;
            // ������� ���� ������ ������� ��������
            if (!_firstSlip && !WaitForSlipState(false, SLIP_WAIT_TIMEOUT))
                return;
            // ������� ���� ������� ����� ��������
            if (!WaitForSlipState(true, _firstSlip ? 0 : SLIP_WAIT_TIMEOUT))
                return;
            // �������� ����� ����������� ���������
            PrintSlipBuffer();

            // ������� ���������� ������
            while (_protocol.GetPrinterFlags(DEF_OPERATOR_PASSWD).Mode == 14)
                System.Threading.Thread.Sleep(100);

            _needPrintTopMargin = true;
        }

        private void PrintSlipBuffer()
        {
            _firstSlip = false;
            _slipLineNo = 1;
            _protocol.CreateCommand(0x7D, DEF_OPERATOR_PASSWD);
            _protocol.AddBytes(0x00, 0x02);
            _protocol.ExecuteCommand();

            // ������� ����� ������
//            _protocol.ExecuteCommand(0x7C, DEF_OPERATOR_PASSWD);
        }

        private void PrintSlipLine(string source, FontStyle style)
        {
            // ������ �������� ������
            if (_needPrintTopMargin)
            {
                _needPrintTopMargin = false;
                PrintTopMargin();
            }

            // ���� _slipLineNo == 0, ��������

            // ��������� ������ ����������� ���������
            _protocol.CreateCommand(0x7A, DEF_OPERATOR_PASSWD);
            _protocol.AddByte(_slipLineNo++);
            _protocol.AddByte(0x1B);
            // ��������� ������
            _protocol.AddByte((style == FontStyle.DoubleWidth || style == FontStyle.DoubleAll) ? 0x02 : 0x01);
            _protocol.AddString(source, (style == FontStyle.DoubleWidth) || (style == FontStyle.DoubleAll) 
                ? PrinterInfo.TapeWidth.AdditionalPrinter1 / 2 : PrinterInfo.TapeWidth.AdditionalPrinter1);
            _protocol.ExecuteCommand();

            // ���� ������ ��������� ��������, �� �������� ���
            if (_slipLineNo > _maxSlipLines)
                EndSlip();
        }

        #endregion

        private delegate void ExecuteCommandDelegate();

        private void ExecuteDriverCommand(CommandFlags commandFlags, ExecuteCommandDelegate executeCommandDelegate)
        {
            try
            {
                this.ErrorCode = new ServerErrorCode(this, GeneralError.Success);
                // �������� ���������� ����������
                if ((commandFlags & CommandFlags.CheckActive) == CommandFlags.CheckActive)
                    if (!Active)
                    {
                        this.ErrorCode = new ServerErrorCode(this, GeneralError.Inactive);
                        return;
                    }

                // ������� �� �������������� ��������� "�����-500"
                if ((commandFlags & CommandFlags.Fiscal) == CommandFlags.Fiscal)
                    if (_deviceType == StrokeType.stroke500)
                        return;

                // ������� �� �������������� ���������� ���������
                if ((commandFlags & CommandFlags.TapePrinter) == CommandFlags.TapePrinter)
                    if (PrinterNumber == PrinterNumber.AdditionalPrinter1 && _deviceType == StrokeType.strokeCombo)
                        return;

                executeCommandDelegate();

                //if ((commandFlags & CommandFlags.CheckPaperStatus) == CommandFlags.CheckPaperStatus)
                //    CheckPaperStatus();
            }
            catch (DeviceErrorException E)
            {
                if (E.ErrorCode >= 0x1000)
                {
                    this.ErrorCode = new ServerErrorCode(this, (GeneralError)E.ErrorCode);
                    return;
                }
                string message = GetSpecificDescription(E.ErrorCode);
                string dumpStr = _protocol.GetDumpStr();
                // ��� ������ "������� ����������� � ������ ������" ������������� ���������� �����
                if (E.ErrorCode == 115)
                {
                    StrokePrinterFlags printerFlags;
                    if (_protocol.TryGetPrinterFlags(DEF_OPERATOR_PASSWD, out printerFlags))    
                        message = String.Format("{0}. �����: {1}. ��������: {2}", message, stateModes[printerFlags.Mode], stateSubModes[printerFlags.Submode]);
                }
                this.ErrorCode = new ServerErrorCode(this, E.ErrorCode, message, dumpStr);

                // ������ ��������� ������������� ���������
                try
                {
                    CancelOpenedDocument(false);
                }
                catch (Exception)
                {
                }
            }
            catch (TimeoutException)
            {
                this.ErrorCode = new ServerErrorCode(this, GeneralError.Timeout, _protocol.GetDumpStr());
            }
            catch (Exception E)
            {
                this.ErrorCode = new ServerErrorCode(this, E);
            }
            finally
            {
                if (!ErrorCode.Succeeded && Logger.DebugInfo && !String.IsNullOrEmpty(_protocol.DebugInfo))
                    Logger.SaveDebugInfo(this, _protocol.DebugInfo);

                _protocol.ClearDebugInfo();
            }
        }

        #region ���������� ����������� �������

        protected override void SetCommStateEventHandler(object sender, CommStateEventArgs e)
        {
            DCB dcb = e.DCB;
            dcb.XonChar = 0x11;
            dcb.XoffChar = 0x13;
            dcb.XonLim = 2048;
            dcb.XoffLim = 512;

            dcb.fDtrControl = (uint)DtrControl.Disable;
            dcb.fAbortOnError = 0;
            dcb.fDsrSensitivity = 0;
            dcb.fOutxDsrFlow = 0;
            dcb.fOutxCtsFlow = 0;

            dcb.fTXContinueOnXoff = 0;
            dcb.fInX = 0;
            dcb.fOutX = 0;
            dcb.fRtsControl = (uint)RtsControl.Disable;
            dcb.fNull = 0;

            e.DCB = dcb;
            e.Handled = true;
            base.SetCommStateEventHandler(sender, e);
        }

        protected override void OnAfterActivate()
        {
            _protocol = new StrokeProtocol(Port);            
            ExecuteDriverCommand(CommandFlags.None, delegate()
            {
                // ����������� ���� ����������
                SetDeviceType();
                // �������� ��������� ������
                CheckPaperStatus();
                // ������ ��������� ���������
                CancelOpenedDocument(true);
                // ��������� ����� � �������
                if (!(_deviceType == StrokeType.stroke500 || _protocol.GetPrinterFlags(DEF_OPERATOR_PASSWD).OpenedShift))
                {
                    // ��������� ���������
                    if (DocumentHeader != null)
                    {
                        int firstRow = (_deviceType == StrokeType.strokeM) ? 11 : 6;

                        for (int i = 0; i < 4; i++)
                            SetTableValue(4, firstRow + i, 1, i < DocumentHeader.Length ? DocumentHeader[i] : string.Empty, 
                                PrinterInfo.TapeWidth.MainPrinter);
                    }

                    // ��������� �������
                    if (DocumentFooter != null)
                        for (int i = 0; i < 3; i++)
                            SetTableValue(4, i + 1, 1, i < DocumentFooter.Length ? DocumentFooter[i] : string.Empty,
                                PrinterInfo.TapeWidth.MainPrinter);
                }
            });
        }

        protected override void OnOpenDocument(DocumentType docType,
            String cashierName)
        {
            FiscalStatusFlags fsFlags = Status;
            _needPrintTopMargin = false;
            _protocol.ClearDebugInfo();
            _protocol.WriteDebugLine("OnOpenDocument");
            ExecuteDriverCommand(CommandFlags.CheckActive | CommandFlags.Fiscal | CommandFlags.CheckPaperStatus, delegate()
            {
                if (PrinterNumber == PrinterNumber.AdditionalPrinter1 && _deviceType == StrokeType.strokeCombo)
                    StartSlip();

                // �������� ��������� ������
                CheckPaperStatus();
                
                // ������ ��������� ���������
                CancelOpenedDocument(true);
                
                // ��������� ����� �������
                SetTableValue(2, 30, 2, cashierName, 21);
                
                // ��������� ��� ���������
                _currDocType = docType;

                if (_currDocType == DocumentType.Sale || _currDocType == DocumentType.Refund)
                    if (this.HasNonzeroRegistrations)
                    {
                        _protocol.CreateCommand(0x8D, DEF_OPERATOR_PASSWD);
                        _protocol.AddByte(docType == DocumentType.Sale ? 0 : 2);
                        _protocol.ExecuteCommand();
                    }

            });
        }

        protected override void OnCustomTopMarginPrinted()
        {
            if (PrinterNumber == PrinterNumber.AdditionalPrinter1 && _deviceType == StrokeType.strokeCombo)
                // �������� �����
                DrawHeader();
        }

        protected override void OnCloseDocument(bool cutPaper)
        {
            _protocol.WriteDebugLine("OnCloseDocument");
            ExecuteDriverCommand(CommandFlags.CheckActive | CommandFlags.Fiscal | CommandFlags.CheckPaperStatus, delegate()
            {
                if (PrinterNumber == PrinterNumber.AdditionalPrinter1 && _deviceType == StrokeType.strokeCombo)
                {
                    // �������� ������
                    DrawFooter();
                    EndSlip();
                    return;
                }

                switch (_currDocType)
                {
                    case DocumentType.Sale: // �������
                    case DocumentType.Refund: // �������
                        StrokePrinterFlags printerFlags = _protocol.GetPrinterFlags(DEF_OPERATOR_PASSWD);
                        if (printerFlags.Mode != 8) // ���� ���������� �������� �� ��� ������
                            CloseNonFiscalDoc();
                        else
                            CloseFiscalDoc();
                        break;
                    case DocumentType.PayingIn: // ��������
                    case DocumentType.PayingOut: // �������
                        _protocol.CreateCommand(_currDocType == DocumentType.PayingIn ? (byte)0x50 : (byte)0x51,
                            DEF_OPERATOR_PASSWD);
                        _protocol.AddInt(_paymentAmount[(uint)FiscalPaymentType.Cash], 5);
                        _protocol.ExecuteCommand(true);
                        break;
                    case DocumentType.XReport:  // X-�����
                        _protocol.ExecuteCommand(0x40, DEF_OPERATOR_PASSWD, true);
                        break;
                    case DocumentType.ZReport:  // Z-�����
                        _protocol.ExecuteCommand(0x41, DEF_OPERATOR_PASSWD, true);
                        break;
                    case DocumentType.SectionsReport:   // ����� �� �������
                        _protocol.ExecuteCommand(0x42, DEF_OPERATOR_PASSWD, true);
                        break;
                    case DocumentType.Other: // ������������ ��������
                        CloseNonFiscalDoc();
                        break;
                }
                DoAfterClose();
            });
        }

        protected override void OnOpenDrawer()
        {
            _protocol.WriteDebugLine("OnOpenDrawer");
            ExecuteDriverCommand(CommandFlags.CheckActive, delegate()
            {
                _protocol.CreateCommand(0x28, DEF_OPERATOR_PASSWD);
                _protocol.AddByte(0);
                _protocol.ExecuteCommand();
            });
        }

        protected override void OnPrintString(String source, FontStyle style)
        {
            _protocol.WriteDebugLine("OnPrintString");
            ExecuteDriverCommand(CommandFlags.CheckActive | CommandFlags.CheckPaperStatus, delegate()
            {
                if (PrinterNumber == PrinterNumber.AdditionalPrinter1 && _deviceType == StrokeType.strokeCombo)
                    PrintSlipLine(source, style);
                else
                {
                    _protocol.CreateCommand(style == FontStyle.DoubleWidth || style == FontStyle.DoubleAll ?
                        (byte)0x12 : (byte)0x17, DEF_OPERATOR_PASSWD);
                    _protocol.AddByte(2);
                    int strLen = (style == FontStyle.DoubleWidth) || (style == FontStyle.DoubleAll) ? PrinterInfo.TapeWidth.MainPrinter / 2 : PrinterInfo.TapeWidth.MainPrinter;
                    int maxStrLen = (style == FontStyle.DoubleWidth) || (style == FontStyle.DoubleAll) ? 20 : 40;
                    _protocol.AddString(source, strLen < maxStrLen ? maxStrLen : strLen);
                    _protocol.ExecuteCommand(true);
                }
            });
        }

        protected override void OnPrintBarcode(String barcode, AlignOptions align,
            bool readable)
        {
            _protocol.WriteDebugLine("OnPrintBarcode");
            ExecuteDriverCommand(CommandFlags.CheckActive | CommandFlags.TapePrinter | CommandFlags.CheckPaperStatus, delegate()
            {
                _protocol.CreateCommand(0xC2, DEF_OPERATOR_PASSWD);
                _protocol.AddInt(Convert.ToInt64(barcode), 5);
                _protocol.ExecuteCommand();
            });
        }

        protected override void OnPrintImage(System.Drawing.Bitmap image, AlignOptions align)
        {
            _protocol.WriteDebugLine("OnPrintImage");
            ExecuteDriverCommand(CommandFlags.CheckActive | CommandFlags.TapePrinter | CommandFlags.CheckPaperStatus, delegate()
            {
                int stride = 0;
                byte[] imageBytes = image.PixelFormat == PixelFormat.Format1bppIndexed ?
                    GetImageBytes(image, out stride) :
                    GetColorImageBytes(image, out stride);

                int leftBytesCount = 0;
                int rightBytesCount = 0;
                if (stride < 40)
                    switch (align)
                    {
                        case AlignOptions.Left:
                            rightBytesCount = 40 - stride;
                            break;
                        case AlignOptions.Center:
                            leftBytesCount = (40 - stride) / 2;
                            rightBytesCount = 40 - stride - leftBytesCount;
                            break;
                        case AlignOptions.Right:
                            leftBytesCount = 40 - stride;
                            break;
                    }

                for (int row = 0; (row < image.Height) && (row < 200); row++)
                {
                    _protocol.CreateCommand(0xC0, DEF_OPERATOR_PASSWD);
                    _protocol.AddByte(row);
                    _protocol.AddEmptyBytes(leftBytesCount);
                    for (int i = 0; (i < stride) && (i < 40); i++)
                    {
                        byte b = imageBytes[row * stride + i];
                        // ��������������� �����
                        ulong n = (((b * 0x0802LU & 0x22110LU) | (b * 0x8020LU & 0x88440LU)) * 0x10101LU >> 16);
                        _protocol.AddByte((int)n);
                    }
                    _protocol.AddEmptyBytes(rightBytesCount);
                    _protocol.ExecuteCommand();
                }

                // ������ �������
                _protocol.CreateCommand(0xC1, DEF_OPERATOR_PASSWD);
                _protocol.AddByte(1);
                _protocol.AddByte(Math.Max(image.Height, 199) + 1);
                _protocol.ExecuteCommand();
            });
        }

        protected override void OnRegistration(String commentary, UInt32 quantity, UInt32 amount,
            Byte section)
        {
            _protocol.WriteDebugLine("OnRegistration");
            ExecuteDriverCommand(CommandFlags.CheckActive | CommandFlags.Fiscal | CommandFlags.TapePrinter | CommandFlags.CheckPaperStatus, delegate()
            {
                StrokePrinterFlags printerFlags = _protocol.GetPrinterFlags(DEF_OPERATOR_PASSWD);

                // ���������� ��������, �������
                if (printerFlags.Mode == 8 && printerFlags.StateMode == 0)
                    _protocol.CreateCommand(0x80, DEF_OPERATOR_PASSWD);
                // ���������� ��������, �������
                else if (printerFlags.Mode == 8 && printerFlags.StateMode == 2)
                    _protocol.CreateCommand(0x82, DEF_OPERATOR_PASSWD);
                // ������������ ��������
                else
                    return;

                _protocol.AddInt(quantity, 5);
                _protocol.AddInt(amount, 5);
                _protocol.AddByte(section);
                _protocol.AddEmptyBytes(4);
                _protocol.AddString(commentary, 40);

                _protocol.ExecuteCommand();
            });
        }

        protected override void OnPayment(UInt32 amount, FiscalPaymentType paymentType)
        {
            ExecuteDriverCommand(CommandFlags.CheckActive | CommandFlags.Fiscal | CommandFlags.TapePrinter, delegate()
            {
                _paymentAmount[(int)paymentType] += amount;
            });
        }

        protected override void OnCash(UInt32 amount)
        {
            ExecuteDriverCommand(CommandFlags.CheckActive | CommandFlags.Fiscal | CommandFlags.TapePrinter, delegate()
            {
                _paymentAmount[(int)FiscalPaymentType.Cash] += amount;
            });
        }

        protected override void OnContinuePrint()
        {
            _protocol.WriteDebugLine("OnContinuePrint");
            ExecuteDriverCommand(CommandFlags.CheckActive | CommandFlags.TapePrinter, delegate()
            {
                _protocol.ExecuteCommand(0xB0, DEF_OPERATOR_PASSWD, true);
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
                _protocol.WriteDebugLine("CurrentTimestamp_get");
                DateTime currentTimestamp = DateTime.Now;
                ExecuteDriverCommand(CommandFlags.Fiscal, delegate()
                {
                    _protocol.ExecuteCommand(0x11, DEF_OPERATOR_PASSWD);
                    currentTimestamp = new DateTime(
                        _protocol.Response[27] + 2000, _protocol.Response[26], _protocol.Response[25],
                        _protocol.Response[28], _protocol.Response[29], _protocol.Response[30]);
                });
                return currentTimestamp;
            }
            set
            {
                _protocol.WriteDebugLine("CurrentTimestamp_set");
                ExecuteDriverCommand(CommandFlags.Fiscal, delegate()
                {
                    // ��������� �������
                    _protocol.CreateCommand(0x21, DEF_OPERATOR_PASSWD);
                    _protocol.AddBytes(value.Hour, value.Minute, value.Second);
                    _protocol.ExecuteCommand();
                    // ��������� ����
                    _protocol.CreateCommand(0x22, DEF_OPERATOR_PASSWD);
                    _protocol.AddBytes(value.Day, value.Month, value.Year % 100);
                    _protocol.ExecuteCommand();
                    // ������������� ��������� ����
                    _protocol.CreateCommand(0x23, DEF_OPERATOR_PASSWD);
                    _protocol.AddBytes(value.Day, value.Month, value.Year % 100);
                    _protocol.ExecuteCommand();
                });
            }
        }

        public override PrintableDeviceInfo PrinterInfo
        {
            get { return _printerInfo; }
        }

        public override FiscalDeviceInfo Info
        {
            get
            {
                _protocol.WriteDebugLine("Info_get");
                string SerialNo = "";
                string DeviceType = DeviceNames.ecrTypeStroke;

                ExecuteDriverCommand(CommandFlags.Fiscal, delegate()
                {
                    _protocol.ExecuteCommand(0x11, DEF_OPERATOR_PASSWD);
                    SerialNo = _protocol.GetInt(32, 4).ToString();
                });
                return new FiscalDeviceInfo(DeviceType, SerialNo);
            }
        }

        public override PrinterStatusFlags PrinterStatus
        {
            get
            {
                _protocol.WriteDebugLine("PrinterStatus_get");
                StrokePrinterFlags printerFlags = new StrokePrinterFlags();
                ExecuteDriverCommand(CommandFlags.CheckActive, delegate()
                {
                    printerFlags = _protocol.GetPrinterFlags(DEF_OPERATOR_PASSWD);
                });
                return new PrinterStatusFlags(printerFlags.Printing, printerFlags.tapePaperStatus, 
                    printerFlags.Mode == 8, printerFlags.drawerOpened);
            }
        }

        public override FiscalStatusFlags Status
        {
            get
            {
                _protocol.WriteDebugLine("Status_get");
                bool bFiscalized = false;
                ulong nDocumentAmount = 0;
                ulong nCashInDrawer = 0;
                StrokePrinterFlags printerFlags = new StrokePrinterFlags();
                ExecuteDriverCommand(CommandFlags.Fiscal, delegate()
                {
                    printerFlags = _protocol.GetPrinterFlags(DEF_OPERATOR_PASSWD);

                    _protocol.ExecuteCommand(0x11, DEF_OPERATOR_PASSWD);
                    bFiscalized = _protocol.Response[40] > 0;

                    _protocol.CreateCommand(0x1A, DEF_OPERATOR_PASSWD);
                    _protocol.AddByte(241);
                    _protocol.ExecuteCommand();
                    nCashInDrawer = (ulong)_protocol.GetInt(3, 6);

                    // ������� ����� ��������� �� ���� ���������
                    int nRegister = 0;
                    // ���� ������ ���������� ��������
                    if (printerFlags.Mode == 8)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            _protocol.CreateCommand(0x1A, DEF_OPERATOR_PASSWD);
                            nRegister += 4;
                            _protocol.AddByte(nRegister);
                            _protocol.ExecuteCommand();
                            nDocumentAmount += (ulong)_protocol.GetInt(3, 6);
                        }
                    }
                });
                return new FiscalStatusFlags(printerFlags.OpenedShift, printerFlags.OverShift, 
                    printerFlags.Locked, bFiscalized, nDocumentAmount, nCashInDrawer);
            }
        }

        #endregion

        #region ������

        public override void FiscalReport(FiscalReportType reportType, bool full, params object[] reportParams)
        {
            _protocol.WriteDebugLine("FiscalReport");
            ExecuteDriverCommand(CommandFlags.CheckActive | CommandFlags.Fiscal | CommandFlags.CheckPaperStatus, delegate()
            {
                switch (reportType)
                {
                    case FiscalReportType.ByDates:
                        DateTime dtParam = (DateTime)(reportParams[0]);
                        _protocol.CreateCommand(0x66, TaxerPassword);
                        _protocol.AddByte(Convert.ToByte(full));
                        _protocol.AddBytes(dtParam.Day, dtParam.Month, dtParam.Year % 100);
                        dtParam = (DateTime)(reportParams[1]);
                        _protocol.AddBytes(dtParam.Day, dtParam.Month, dtParam.Year % 100);
                        break;
                    case FiscalReportType.ByShifts:
                        _protocol.CreateCommand(0x67, TaxerPassword);
                        _protocol.AddByte(Convert.ToByte(full));
                        _protocol.AddInt((int)(reportParams[0]), 2);
                        _protocol.AddInt((int)(reportParams[1]), 2);
                        break;
                }
                _protocol.ExecuteCommand(true);
            });
        }

        public override void Fiscalization(int newPassword, long registrationNumber, long taxPayerNumber)
        {
            _protocol.WriteDebugLine("Fiscalization");
            ExecuteDriverCommand(CommandFlags.CheckActive | CommandFlags.Fiscal, delegate()
            {
                _protocol.CreateCommand(0x65, TaxerPassword);
                _protocol.AddInt(newPassword, 4);
                _protocol.AddInt(registrationNumber, 5);
                _protocol.AddInt(taxPayerNumber, 6);
                _protocol.ExecuteCommand(true);
            });
        }

        public override void GetLifetime(out DateTime firstDate, out DateTime lastDate, out int firstShift, out int lastShift)
        {
            _protocol.WriteDebugLine("GetLifetime");
            DateTime _firstDate = DateTime.Now;
            DateTime _lastDate = DateTime.Now;
            int _firstShift = 1;
            int _lastShift = 9999;

            ExecuteDriverCommand(CommandFlags.CheckActive | CommandFlags.Fiscal, delegate()
            {
                _protocol.ExecuteCommand(0x64, TaxerPassword);
                _firstDate = new DateTime(_protocol.Response[4] + 2000, _protocol.Response[3], _protocol.Response[2]);
                _lastDate = new DateTime(_protocol.Response[7] + 2000, _protocol.Response[6], _protocol.Response[5]);
                _firstShift = (int)_protocol.GetInt(8, 2);
                _lastShift = (int)_protocol.GetInt(10, 2);
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
