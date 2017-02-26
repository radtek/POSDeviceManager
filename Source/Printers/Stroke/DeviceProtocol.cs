using System;
using System.Text;
using DevicesCommon.Helpers;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace Stroke
{
    internal struct StrokePrinterFlags
    {
        /// <summary>
        /// ����� ��� (��������/�������� �����, �������� ��������, ���������� � �.�.)
        /// </summary>
        public byte Mode;

        /// <summary>
        /// ������ ������ (��� ��������� ���������, ������ ����������� ���������)
        /// </summary>
        public byte StateMode;

        /// <summary>
        /// �������� �� (��������� ������ � ������ ������)
        /// </summary>
        public byte Submode;

        public bool slipPaperPresent;
        public bool drawerOpened;


        public PaperOutStatus tapePaperStatus
        {
            get { return Submode < 4 ? (PaperOutStatus)Submode : PaperOutStatus.Present; }
        }

        public bool Printing
        {
            get { return !((Mode == 2) || (Mode == 3) || (Mode == 4) || (Mode == 8)) || (Submode == 4) || (Submode == 5); }
        }

        public bool OpenedShift
        {
            get { return Mode != 4; }
        }

        public bool OverShift
        {
            get { return Mode == 3; }
        }

        public bool Locked
        {
            get { return Mode == 5; }
        }
    }

    internal class StrokeProtocol
    {
        #region ���������

        private const int MAX_CMD_LEN = 128;
        private const int MAX_RETRIES_COUNT = 5;

        private const byte ENQ = 0x05;
        private const byte STX = 0x02;
        private const byte ACK = 0x06;
        private const byte NAK = 0x15;
        private const byte ETX = 0x03;
        private const byte SYN = 0x16;

        /// <summary>
        /// ������� �������� ������ ������� ����� ��� ������ ������ �� �������
        /// </summary>
        private const int T1 = 500;

        /// <summary>
        /// ������� �������� ������� �� ������ ENQ (��������� ����� �������� �� ����� 10�)
        /// </summary>
        private const int T2 = 10000 / MAX_RETRIES_COUNT;

        /// <summary>
        /// ������� �������� ������ STX ����� �������� �������
        /// </summary>
        private const int T3 = 15000;

        /// <summary>
        /// �������� �������� ��������� �������� ��� �������� ���������� ������
        /// </summary>
        private const int T4 = 500;

        /// <summary>
        /// ������ ��������� �� ���������
        /// </summary>
        private const uint DEF_OPERATOR_PASSWD = 30;

        #endregion

        #region ����

        private int _cmdLen = 0;
        private int _rspLen = 0;
        private byte[] _cmdBuffer = new byte[MAX_CMD_LEN];
        private byte[] _rspBuffer = new byte[MAX_CMD_LEN];
        private EasyCommunicationPort _port;
        private StringBuilder _debugInfo = new StringBuilder();

        #endregion

        #region ������������

        public StrokeProtocol(EasyCommunicationPort port)
        {
            _port = port;
        }

        #endregion

        #region ��������

        public int ReqLen
        {
            get { return _cmdLen; }
        }

        public int RspLen
        {
            get { return _rspLen; }
        }

        public byte[] Request
        {
            get { return _cmdBuffer; }
        }

        public byte[] Response
        {
            get { return _rspBuffer; }
        }

        public string DebugInfo
        {
            get { return _debugInfo.ToString(); }
        }

        #endregion

        #region ������

        public void ClearDebugInfo()
        {
            _debugInfo = new StringBuilder();
        }

        public void WriteDebugLine(string message)
        {
            _debugInfo.AppendFormat("{0:HH:mm:ss}\t{1}\r\n", DateTime.Now, message);
        }

        public void WriteDebugLine(string message, byte[] nBuffer, int nBufferLen)
        {
            string[] bufDump = Array.ConvertAll(nBuffer, new Converter<byte, string>(delegate(byte b) { return b.ToString("X"); }));
            _debugInfo.AppendFormat("{0:HH:mm:ss}\t{1}\r\n", DateTime.Now, message);
            if (nBufferLen > 0)
                _debugInfo.AppendFormat("\t{0:X}\r\n", string.Join(" ", bufDump, 0, nBufferLen));
            else
                _debugInfo.Append("\t���\r\n");
        }

        public void CreateCommand(byte nCmd)
        {
            _cmdBuffer = new byte[MAX_CMD_LEN];
            _rspBuffer = new byte[MAX_CMD_LEN];
            _cmdLen = 0;
            _rspLen = 0;

            // ������ �������
            _cmdBuffer[_cmdLen++] = STX;
            // ����� �������
            _cmdBuffer[_cmdLen++] = 0;
            // ����� �������
            _cmdBuffer[_cmdLen++] = nCmd;
        }

        public void CreateCommand(byte nCmd, long nPasswd)
        {
            CreateCommand(nCmd);
            // ������
            AddInt(nPasswd, 4);
        }

        public void ExecuteCommand(byte nCmd)
        {
            CreateCommand(nCmd);
            ExecuteCommand();
        }

        public void ExecuteCommand(bool waitForExecute)
        {
            ExecuteCommand();
            if (waitForExecute)
                DoWaitCommandExecute();
        }

        public void ExecuteCommand(byte nCmd, long nPasswd)
        {
            CreateCommand(nCmd, nPasswd);
            ExecuteCommand();
        }

        public void ExecuteCommand(byte nCmd, long nPasswd, bool waitForExecute)
        {
            ExecuteCommand(nCmd, nPasswd);
            if (waitForExecute)
                DoWaitCommandExecute();
        }

        public bool TryGetPrinterFlags(long nPasswd, out StrokePrinterFlags printerFlags)
        {
            printerFlags = new StrokePrinterFlags();
            try
            {
                printerFlags = GetPrinterFlags(nPasswd);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public StrokePrinterFlags GetPrinterFlags(long nPasswd)
        {
            WriteDebugLine("GetPrinterFlags");
            var printerFlags = new StrokePrinterFlags();
            ExecuteCommand(0x10, nPasswd);
            // ����� ���: ������� �������� ����� ������ 5
            printerFlags.Mode = (byte)(Response[5] & 0x0F);
            // ������ ������: ������� �������� ����� ������ 5
            printerFlags.StateMode = (byte)(Response[5] >> 4);
            // �������� ��: ���� ������ 6
            printerFlags.Submode = (byte)Response[6];

            printerFlags.slipPaperPresent = (byte)(Response[3] & 0x0C) == 0x0C;
            printerFlags.drawerOpened = (Response[4] & 0x08) == 0x08;

            return printerFlags;

/*

            int retries = 0;
            var printerFlags = new StrokePrinterFlags();
            do
            {
                try
                {
                    ExecuteCommand(0x10, nPasswd);
                    // ����� ���: ������� �������� ����� ������ 5
                    printerFlags.Mode = (byte)(Response[5] & 0x0F);
                    // ������ ������: ������� �������� ����� ������ 5
                    printerFlags.StateMode = (byte)(Response[5] >> 4);
                    // �������� ��: ���� ������ 6
                    printerFlags.Submode = (byte)Response[6];

                    printerFlags.slipPaperPresent = (byte)(Response[3] & 0x0C) == 0x0C;
                    printerFlags.drawerOpened = (Response[4] & 0x08) == 0x08;

                    return printerFlags;
                }
                catch (TimeoutException)
                {
                    if (retries++ > MAX_RETRIES_COUNT)
                        throw;
                    System.Threading.Thread.Sleep(T1);
                }
            }
            while (true);
 */ 
        }

        public void ExecuteCommand()
        {
            CompleteCmd();
            byte rspByte = 0;

            // �������� ������� �� �������� ������
            int retries = 0;
            do
            {
                _port.DiscardBuffers();
                _port.ReadTimeout = T2;
                try
                {
                    WriteDebugLine("�������� ������� �� �������� ������, ������� " + (retries + 1));
                    _port.WriteByte(ENQ);
                    rspByte = (byte)_port.ReadByte();
                }
                catch (TimeoutException)
                {
                    WriteDebugLine("������� ��� �������� ������� �� �������� ������");
                    if (retries++ >= MAX_RETRIES_COUNT - 1)
                    {
                        WriteDebugLine("��������� ��� ������� ������ ����� (��� 1)");
                        throw;
                    }
                }
                // ���� ����� �� ����� �������� ����� �� ���������� �������, ����� ��� ���������
                if (rspByte == ACK)
                {
                    WriteDebugLine("������� ���� ACK. ������ ������ �� ���������� �������");
                    SkipResponseBytes();
                }
                WriteDebugLine("������� ���� ������ " + rspByte);
            }
            while (rspByte != NAK);

            retries = 0;
            do
            {
                _port.DiscardBuffers();
                _port.ReadTimeout = T1;
                try
                {
                    WriteDebugLine("�������� ������ �������, ������� " + (retries + 1));
                    _port.Write(_cmdBuffer, 0, _cmdLen);
                    rspByte = (byte)_port.ReadByte();
                }
                catch (TimeoutException)
                {
                    WriteDebugLine("������� ��� �������� ������ �������");
                    if (retries++ >= MAX_RETRIES_COUNT - 1)
                    {
                        WriteDebugLine("��������� ��� ������� �������� ������ (��� 4)");
                        throw;
                    }
                }

                if(rspByte == NAK)
                    WriteDebugLine("������� ���� NAK (��� 2)");
                else if (rspByte != ACK)
                    WriteDebugLine("����� �� ������������� ��������� (��� 3): " + rspByte);
            }
            while (rspByte != ACK);

            ReadResponse();
        }

        public long GetInt(int nPos, int nLen)
        {
            byte[] nBuf = new byte[8];
            Array.Copy(_rspBuffer, nPos, nBuf, 0, nLen);
            return BitConverter.ToInt64(nBuf, 0);
        }

        public int GetInt32(int nPos)
        {
            return BitConverter.ToInt32(_rspBuffer, nPos);
        }

        public void AddByte(int nValue)
        {
            _cmdBuffer[_cmdLen++] = (byte)nValue;
        }

        public void AddInt(long nValue, int nLen)
        {
            byte[] nPaswdBytes = BitConverter.GetBytes(nValue);
            Array.Copy(nPaswdBytes, 0, _cmdBuffer, _cmdLen, nLen);
            _cmdLen += nLen;
        }

        public void AddString(string sValue, int nLen)
        {
            byte[] nBytes = Encoding.GetEncoding(1251).GetBytes(sValue);
            Array.Copy(nBytes, 0, _cmdBuffer, _cmdLen, Math.Min(nBytes.Length, nLen));
            _cmdLen += nLen;
        }

        public void AddBytes(params int[] bytes)
        {
            Array.ConvertAll<int, byte>(bytes, delegate(int value) { return (byte)value; }).CopyTo(_cmdBuffer, _cmdLen);
            _cmdLen += bytes.Length;
        }

        public void AddEmptyBytes(int bytesCount)
        {
            _cmdLen += bytesCount;
        }

        public string GetDumpStr()
        {
            string[] reqDump = Array.ConvertAll(Request,
                new Converter<byte, string>(delegate(byte b) { return b.ToString("X"); }));
            string[] rspDump = Array.ConvertAll(Response,
                new Converter<byte, string>(delegate(byte b) { return b.ToString("X"); }));
            return string.Format("����� ������� ({0}):\n{1:X}\n����� ������ ({2}):\n{3:X}",
                ReqLen, string.Join(" ", reqDump, 0, ReqLen),
                RspLen, string.Join(" ", rspDump, 0, RspLen));
        }

        public int GetReceiptNo(long password)
        {
            WriteDebugLine("������ ��������� ������ ���������");
            ExecuteCommand(0x11, password);
            return (int)GetInt(11, 2);
        }

        #endregion

        #region ������� ������

        // �������� ��������� ���������� �������
        private void DoWaitCommandExecute()
        {            
            WriteDebugLine("DoWaitCommandExecute");
            try
            {
                WriteDebugLine("������ ���������");
                ExecuteCommand(0x10, DEF_OPERATOR_PASSWD);
                while ((Response[6] == 5) || (Response[6] == 4))
                {
                    System.Threading.Thread.Sleep(T4);
                    WriteDebugLine("������ ���������");
                    ExecuteCommand(0x10, DEF_OPERATOR_PASSWD);
                }
            }
            catch (TimeoutException)
            {
                WriteDebugLine("������ �� ����� �������� ���������� ���������� �������");
            }
        }

        /// <summary>
        /// ������ ������ ������ �� ���������� ��������� ��� ������� �����������
        /// </summary>
        private void SkipResponseBytes()
        {
            WriteDebugLine("SkipResponseBytes");
            _port.ReadTimeout = T3;

            if ((byte)_port.ReadByte() != STX)
                return; // �� �������� ���������

            // ����� ������ ��� ����������� �����
            int size = _port.ReadByte();
            // ������ ������
            byte[] buffer = new byte[MAX_CMD_LEN];
            _port.Read(buffer, 0, size + 1);
            // ������������� ������
            _port.WriteByte(ACK);
        }

        private void ReadResponse()
        {
            WriteDebugLine("ReadResponse");
            int retries = 0;
            bool success = true;

            try
            {

                do
                {
                    // ������������� ������� �������� ������ STX (15�)
                    _port.ReadTimeout = T3;

                    // ���� ���������� ������� ����������� ����������, ������ ������� ������ �� �������
                    if (!success)
                    {
                        WriteDebugLine("�������� ������� ������� ������ �� ������� (�������� NAK)");
                        _port.WriteByte(NAK);
                    }

                    WriteDebugLine("������ ������ �� �������, ������� " + (retries + 1));
                    success = (byte)_port.ReadByte() == STX;
                    if (!success)
                    {
                        WriteDebugLine("������ ���� ������ �� ������������� ���������");
                        if (retries++ >= MAX_RETRIES_COUNT - 1)
                            throw new TimeoutException("������ ���� ������ �� ������������� ���������");
                        continue;
                    }

                    // ������������� ������� �������� ������� ����� 500��
                    _port.ReadTimeout = T1;

                    // ����� ������ ��� ����������� �����
                    _rspLen = (byte)_port.ReadByte();

                    // ������ ������
                    _port.Read(_rspBuffer, 0, _rspLen);

                    // ���������� ����������� �����
                    byte nCRC = (byte)_rspLen;
                    for (int i = 0; i < _rspLen; i++)
                        nCRC ^= _rspBuffer[i];
                    success = nCRC == (byte)_port.ReadByte();

                    if (!success)
                    {
                        WriteDebugLine("������ ����������� ����� � ������ �� �������");
                        if (retries++ >= MAX_RETRIES_COUNT - 1)
                            throw new TimeoutException("������ ����������� ����� � ������ �� �������");
                        continue;
                    }
                    else
                    {
                        // ������������� ��������� ������
                        WriteDebugLine("������������� ��������� ������ ������ �� ������� (�������� ACK)");
                        _port.WriteByte(ACK);
                    }
                }
                while (!success);


            }
            catch (TimeoutException)
            {
                WriteDebugLine("������� ������ ������ �� �������, �������� ����� ������:", _rspBuffer, _rspLen);
                throw;
            }

            if (_rspBuffer[1] != 0)
                throw new DeviceErrorException(_rspBuffer[1]);
        }

        private void CompleteCmd()
        {
            // ��������� ����� �������
            _cmdBuffer[1] = (byte)(_cmdLen - 2);
            // ��������� ����������� �����
            for (int i = 1; i < _cmdLen; i++)
                _cmdBuffer[_cmdLen] ^= _cmdBuffer[i];
            _cmdLen++;
        }

        #endregion
    }
}