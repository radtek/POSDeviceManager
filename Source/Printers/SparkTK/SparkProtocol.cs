using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace SparkTK
{
    internal enum FiscalDocState
    {
        Closed = 0,
        Header = 1,
        Position = 2,
        Total = 3,
        Payment = 4,
        Finalization = 5,
        Discount = 6,
        FreeDoc = 7
    }

    internal struct CurrentDeviceInfo
    {
        public FiscalDocState DocState;
        public string SerialNo;
        public bool OpenedShift;
        public bool OverShift;
        public bool Fiscalized;
    }


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

    internal class PrintableErrorException : Exception
    {
        public PrintableErrorException()
            :base()
        {
        }
    }

    internal class SparkProtocol : SparkProtocolBase
    {
        #region ���������

        // ������ �������� ������
        private const string OPERATOR_PASSWD = "AERF";

        // ������������ ���������� ������� ��� ��������� ������/�������� ������
        private const int MAX_RETRIES_COUNT = 5;
        private const int MAX_CMD_LEN = 1024;

        private const byte DLM = 0x1C;
        private const byte STX = 0x02;
        private const byte ETX = 0x03;
        private const byte ACK = 0x06;
        private const byte NACK = 0x15;

        public static string[] HEX_BYTES = new string[256] {
            "00", "01", "02", "03", "04", "05", "06", "07",
            "08", "09", "0A", "0B", "0C", "0D", "0E", "0F",
            "10", "11", "12", "13", "14", "15", "16", "17",
            "18", "19", "1A", "1B", "1C", "1D", "1E", "1F",
            "20", "21", "22", "23", "24", "25", "26", "27",
            "28", "29", "2A", "2B", "2C", "2D", "2E", "2F",
            "30", "31", "32", "33", "34", "35", "36", "37",
            "38", "39", "3A", "3B", "3C", "3D", "3E", "3F",
            "40", "41", "42", "43", "44", "45", "46", "47",
            "48", "49", "4A", "4B", "4C", "4D", "4E", "4F",
            "50", "51", "52", "53", "54", "55", "56", "57",
            "58", "59", "5A", "5B", "5C", "5D", "5E", "5F",
            "60", "61", "62", "63", "64", "65", "66", "67",
            "68", "69", "6A", "6B", "6C", "6D", "6E", "6F",
            "70", "71", "72", "73", "74", "75", "76", "77",
            "78", "79", "7A", "7B", "7C", "7D", "7E", "7F",
            "80", "81", "82", "83", "84", "85", "86", "87",
            "88", "89", "8A", "8B", "8C", "8D", "8E", "8F",
            "90", "91", "92", "93", "94", "95", "96", "97",
            "98", "99", "9A", "9B", "9C", "9D", "9E", "9F",
            "A0", "A1", "A2", "A3", "A4", "A5", "A6", "A7",
            "A8", "A9", "AA", "AB", "AC", "AD", "AE", "AF",
            "B0", "B1", "B2", "B3", "B4", "B5", "B6", "B7",
            "B8", "B9", "BA", "BB", "BC", "BD", "BE", "BF",
            "C0", "C1", "C2", "C3", "C4", "C5", "C6", "C7",
            "C8", "C9", "CA", "CB", "CC", "CD", "CE", "CF",
            "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7",
            "D8", "D9", "DA", "DB", "DC", "DD", "DE", "DF",
            "E0", "E1", "E2", "E3", "E4", "E5", "E6", "E7",
            "E8", "E9", "EA", "EB", "EC", "ED", "EE", "EF",
            "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7",
            "F8", "F9", "FA", "FB", "FC", "FD", "FE", "FF"
        };


        // ����� ��������� ����� ������
        private const int REQUEST_INFO_LEN = 1     // STX
                                            + 1     // ������������� ����
                                            + 2     // ��� �������
                                            + 1     // �����������
                                            + 2     // ���������� ������ ���
                                            + 1     // �����������
                                            + 4     // ������� ������ ���
                                            + 1     // �����������
                                            + 4     // ��� ������
                                            + 1     // �����������
                                            + 10    // ��������� ����������� ����������
                                            + 1;    // �����������


        #endregion

        #region ������� ����

        // ������������ ���������
        private Encoding _currEnconing = Encoding.GetEncoding(866);

        // �������� �������������� �����
        private byte _currDistByte = 0x20;

        // ����� �������
        private int _cmdLen = 0;

        // ����� ������
        private int _rspLen = 0;

        // ����� �������
        private byte[] _cmdBuffer = new byte[MAX_CMD_LEN];

        // ����� ������
        private byte[] _rspBuffer = new byte[MAX_CMD_LEN];

        private bool _isPrinterMode = false;

        #endregion

        #region ��������

        public byte[] Response
        {
            get { return _rspBuffer; }
        }

        public byte[] Request
        {
            get { return _cmdBuffer; }
        }

        public int RequestLen
        {
            get { return _cmdLen; }
            set { _cmdLen = value; }
        }

        public int ResponseLen
        {
            get { return _rspLen; }
            set { _rspLen = value; }
        }

        public bool IsPrinterMode
        {
            get
            {
                if (OnlyESCPOSMode)
                    return _isPrinterMode;
                else
                {
                    // ������ ������� ���������� �������
                    byte statusByte = ShortStatusRequest(false, 0x30);
                    return (statusByte & 0x40) == 0x00;
                }
            }
            set
            {
                if (value)
                {
                    WriteDebugLine("������������ � ����� ��������");
                    ExecuteCommand("70");
                    _isPrinterMode = value;
                }
                else
                {
                    WriteDebugLine("����� �� ������ ��������");
                    try
                    {
                        Write(new byte[] { 0x1B, 0x1B });
                        GetCommandRequest();
                        _isPrinterMode = value;
                    }
                    catch (DeviceErrorException E)
                    {
                        // ������ �33 ("������� ������") � �1 ("�������� ������ ���������") ������������ � 
                        // ����� �� ������� ������ �� ������ ��������, ���� �� �� ��� � ������ ��������
                        if (value == false && (E.ErrorCode == 33 || E.ErrorCode == 1))
                        {
                            _isPrinterMode = false;
                        }
                    }
                    catch (TimeoutException)
                    {
                    }
                }
            }
        }



        #endregion

        #region �����������

        public SparkProtocol(ISparkDeviceProvider deviceProvider)
            : base(deviceProvider)
        {
        }

        #endregion

        #region ��������� ������

        public void CreateCommand(string cmdCode)
        {
            CreateCommand(cmdCode, false);
        }

        public void CreateCommand(string cmdCode, bool withDateTime)
        {
            _cmdLen = 0;
            _rspLen = 0;
            _cmdBuffer = new byte[MAX_CMD_LEN];
            _rspBuffer = new byte[MAX_CMD_LEN];

            // ��������� ������ �������
            _cmdBuffer[_cmdLen++] = STX;
            // ������ �����
            _currEnconing.GetBytes(OPERATOR_PASSWD, 0, 4, _cmdBuffer, _cmdLen);
            _cmdLen += 4;
            // ������������� ����
            _cmdBuffer[_cmdLen++] = GetDistinctiveByte();
            // ��� �������
            _currEnconing.GetBytes(cmdCode, 0, 2, _cmdBuffer, _cmdLen);
            _cmdLen += 2;
            // �����������
            _cmdBuffer[_cmdLen++] = DLM;

            if (withDateTime)
            {
                AddString(DateTime.Now.ToString("ddMMyy"));
                AddString(DateTime.Now.ToString("HHmm"));
            }
        }

        public void ExecuteCommand(string cmdCode)
        {
            ExecuteCommand(cmdCode, false);
        }

        public void ExecuteCommand(string cmdCode, bool withDate)
        {
            CreateCommand(cmdCode, withDate);
            Execute();
        }

        public CurrentDeviceInfo GetDeviceInfo()
        {
            CurrentDeviceInfo devInfo = new CurrentDeviceInfo();
            ExecuteCommand("96");

            // ���� "���������� ������ ���"
            byte nStatusByte = (byte)Array.IndexOf(SparkProtocol.HEX_BYTES, Encoding.ASCII.GetString(Response, 5, 2));
            devInfo.Fiscalized = (nStatusByte & 8) == 8;

            // ���� 0 ���� "������� ������ ���"
            nStatusByte = (byte)Array.IndexOf(SparkProtocol.HEX_BYTES,
                Encoding.ASCII.GetString(Response, 8, 2));
            // ��������� ��������� - ������� 3 ����
            devInfo.DocState = (FiscalDocState)(nStatusByte & 0x07);
            // ���������� �����
            devInfo.OverShift = (nStatusByte & 16) == 16;

            // ���� 1 ���� "������� ������ ���"
            nStatusByte = (byte)Array.IndexOf(SparkProtocol.HEX_BYTES, Encoding.ASCII.GetString(Response, 10, 2));
            devInfo.OpenedShift = (nStatusByte & 8) == 8;

            // �������� �����
            devInfo.SerialNo = Encoding.ASCII.GetString(GetField(0));

            return devInfo;
        }

        public string GetCommandDump()
        {
            string[] reqDump = Array.ConvertAll(Request, new Converter<byte, string>(delegate(byte b) { return b.ToString("X"); }));
            string[] rspDump = Array.ConvertAll(Response, new Converter<byte, string>(delegate(byte b) { return b.ToString("X"); }));
            return String.Format("����� ������� ({0}):\n{1:X}\n����� ������ ({2}):\n{3:X}",
                RequestLen, String.Join(" ", reqDump, 0, RequestLen), ResponseLen, String.Join(" ", rspDump, 0, ResponseLen));
        }

        /// <summary>
        /// ���������� ��������� � �������
        /// </summary>
        /// <param name="sValue">�������� ���������</param>
        public void AddString(string sValue)
        {
            _currEnconing.GetBytes(sValue, 0, sValue.Length, _cmdBuffer, _cmdLen);
            _cmdLen += sValue.Length;
            // �����������
            _cmdBuffer[_cmdLen++] = DLM;
        }

        /// <summary>
        /// ���������� �������.
        /// </summary>
        /// <param name="spDevice">���� ����������</param>
        /// <param name="debugInfo">������ � ���</param>
        /// <returns>��� ������</returns>
        public void Execute()
        {
            CompleteCmd();
            // �������� �������
            WriteCommand(_cmdBuffer, _cmdLen);
            do
            {
                GetCommandRequest();
                if (_rspBuffer[1] != _cmdBuffer[5])
                    WriteDebugLine("Execute: �� ��������� ������������� ����� ������� � ������");
            }
            // �������� �������������� �����
            while (_rspBuffer[1] != _cmdBuffer[5]);
        }

        /// <summary>
        /// ��������� ������ �� �������.
        /// </summary>
        /// <param name="spDevice">���� ����������</param>
        /// <returns></returns>
        public void GetCommandRequest()
        {
            try
            {
                int retriesCount = 0;
                do
                {
                    // ������ �����
                    if (ReadResponse())
                    {
                        // �������� ����������� �����
                        if (_currEnconing.GetString(CalcBCC(_rspBuffer, _rspLen - 4)) == _currEnconing.GetString(_rspBuffer, _rspLen - 4, 4))
                        {
                            // ����� �������, �������
                            short nErrorCode = (short)Array.IndexOf(HEX_BYTES, _currEnconing.GetString(_rspBuffer, 13, 2));

                            // ������ ����������� ���������� (�������� ����� ���� ������)
                            if (nErrorCode >= 22 && nErrorCode <= 25)
                                throw new PrintableErrorException();
                            // ������ ������������ ������ (�������� �� ������)
                            else if (nErrorCode != 0)
                                throw new DeviceErrorException(nErrorCode);
                            return;
                        }
                        WriteDebugLine("GetCommandRequest: ������ ����������� ����� � ������");
                    }
                    // ���� � ������� ���� ����� �������� �� �������
                    // ��������� ������ ���������� �������
                    else if (!WaitForCommandExecuting())
                    {
                        WriteDebugLine("GetCommandRequest: �� ������� ��������� ���������� �������");
                        throw new TimeoutException();
                    }

                    // ���������� NAK:
                    //  - ���� ������ ����������� �����
                    //  - ���� ��������� ���������� �������
                    WriteDebugLine("GetCommandRequest: �������� NAK");
                    CommPort.DiscardBuffers();
                    Write(new byte[1] { NACK });
                }
                while (retriesCount++ < MAX_RETRIES_COUNT);
                WriteDebugLine("GetCommandRequest: ���������� ������� ������ ������ ���������");
                throw new TimeoutException();
            }
            catch (TimeoutException)
            {
                WriteDebugLine("GetCommandRequest: ������� ������. ����� �������:", _cmdBuffer, _cmdLen);
                throw;
            }
        }

        public byte[] GetField(int fieldPos)
        {
            int i = REQUEST_INFO_LEN - 1;

            do
                if (_rspBuffer[i++] == DLM)
                    fieldPos--;
            while (fieldPos > 0);

            int n = 0;
            do
                n++;
            while ((_rspBuffer[i + n] != DLM) && (_rspBuffer[i + n] != ETX));

            byte[] nRes = new byte[n];
            Array.Copy(_rspBuffer, i, nRes, 0, n);

            return nRes;
        }

        #endregion

        #region ������� ������

        private byte GetDistinctiveByte()
        {
            if (_currDistByte < 0x20)
                _currDistByte = 0x20;
            return _currDistByte++;
        }

        private void CompleteCmd()
        {
            // ������ ���������� �������
            _cmdBuffer[_cmdLen++] = ETX;
            // ����������� �����
            Array.Copy(CalcBCC(_cmdBuffer, _cmdLen), 0, _cmdBuffer, _cmdLen, 4);
            _cmdLen += 4;
        }

        private byte[] CalcBCC(byte[] nBuff, int nLen)
        {
            int nRes = 0;
            for (int i = 0; i < nLen; i++)
                nRes += nBuff[i];
            String sRes = nRes.ToString("X4");
            sRes = new String(new char[] { sRes[2], sRes[3], sRes[0], sRes[1] });
            return _currEnconing.GetBytes(sRes);
        }

        /// <summary>
        /// �������� ���������� ���������� �������. 
        /// </summary>
        /// <param name="spDevice">���� ����������</param>
        /// <returns>False, ���� ������� �� ����������; True, ���� ������� ���������� � �� ��� ���� �����</returns>
        private bool WaitForCommandExecuting()
        {
            SetDsrFlow(false);
            try
            {
                int retriesCount = 0;
                byte statusByte = 0;
                do
                {
                    try
                    {
                        Thread.Sleep(100);
                        statusByte = ShortStatusRequest(false, 0x30);
                        WriteDebugLine("WaitForCommandExecuting: ������� ���� �������:", new byte[] { statusByte }, 1);

                        // ��������� ���� �������
                        if (!(((statusByte & 0x02) == 0x02)
                            && ((statusByte & 0x10) == 0x10)
                            && ((statusByte & 0x80) != 0x80)
                            && ((statusByte & 0x01) != 0x01)))
                        {
                            // ���� ��� �� ���� �������, ��������� �������
                            WriteDebugLine("WaitForCommandExecuting: ������������ ���� �������");
                            Thread.Sleep(100);
                            continue;
                        }

                        // ������ ����������� ����������
                        if ((statusByte & 0x20) == 0x0)
                        {
                            WriteDebugLine("WaitForCommandExecuting: ������ ����������� ����������");
                            throw new PrintableErrorException();
                        }

                        // ������� �� ����������
                        if ((statusByte & 0x04) == 0x00)
                        {
                            WriteDebugLine("WaitForCommandExecuting: ������� �� ����������");
                            return false;
                        }
                    }
                    catch (TimeoutException)
                    {
                        if (retriesCount > 5)
                            throw;
                        retriesCount++;
                    }
                }
                // ���� ��� ������ �� �������
                while ((statusByte & 0x08) == 0x00);
            }
            finally
            {
                SetDsrFlow(true);
            }
            return true;
        }

        /// <summary>
        /// ������ ������ ��.
        /// </summary>
        /// <param name="spDevice">����</param>
        /// <returns>True, ���� ��� ����� ������� ��������� �������. False, ���� � �������� ������ ��������� ������� ������.</returns>
        private bool ReadResponse()
        {
            try
            {
                // ������� ������ 29 ���� ��������� ������
                Read(_rspBuffer, 0, REQUEST_INFO_LEN);
                _rspLen = REQUEST_INFO_LEN;

                // ����� ������ �� ����� ���� �� ����� ������� ETX
                do
                    _rspBuffer[_rspLen] = ReadByte();
                while (_rspBuffer[_rspLen++] != ETX);

                // ����� ETX ������ 4 ����� ����������� �����
                Read(_rspBuffer, _rspLen, 4);
                _rspLen += 4;

                return true;
            }
            catch (TimeoutException)
            {
                WriteDebugLine("ReadResponse: ������� ��� ��������� ������ ������. ����� ������:", _rspBuffer, _rspLen);
                return false;
            }
        }

        #endregion
    }
}
