using System;
using System.Collections.Generic;
using System.Text;
using ERPService.SharedLibs.Helpers.SerialCommunications;
using System.Threading;

namespace SparkTK
{
    [Flags]
    internal enum SparkStatusByte: byte
    {
        CommandReceived = 0x04,
        CommandComplete = 0x08,
        PrinterOK = 0x20,
        PrinterMode = 0x40
    }

    internal interface ISparkDeviceProvider
    {
        EasyCommunicationPort GetCommunicationPort();

        bool GetIsOnlyESCPOSMode();
    }

    internal class SparkProtocolBase
    {
        #region ����

        // ��������� �������
        private Encoding _logEnconing = Encoding.GetEncoding(1251);

        private ISparkDeviceProvider _deviceProvider;

        protected StringBuilder _debugInfo = new StringBuilder();

        #endregion

        private delegate void CommOperationDelegate();

        protected EasyCommunicationPort CommPort { get { return _deviceProvider.GetCommunicationPort(); } }

        protected bool OnlyESCPOSMode { get { return _deviceProvider.GetIsOnlyESCPOSMode(); } }

        public string DebugInfo
        {
            get { return _debugInfo.ToString(); }
        }

        #region �����������

        public SparkProtocolBase(ISparkDeviceProvider deviceProvider)
        {
            _deviceProvider = deviceProvider;
        }

        #endregion

        #region ������� ������

        private void ExecuteCommOperation(string errMessage, int retriesCount,
            CommOperationDelegate commOperation)
        {
            do
            {
                try
                {
                    commOperation();
                    return;
                }
                catch (TimeoutException)
                {
                    WriteDebugLine(String.Format("{0}. �������", errMessage));
                    throw;
                }
                catch (System.ComponentModel.Win32Exception E)
                {
                    WriteDebugLine(String.Format("{0}. ������ {1}: {2}", errMessage, E.NativeErrorCode, E.Message));
                    if (E.NativeErrorCode == 995 && retriesCount > 0)
                    {
                        // ����� ������ � ������ �������
                        WriteDebugLine("����� ����� ������ ����������");
                        CommPort.ClearError();
                        retriesCount--;
                        Thread.Sleep(10);
                    }
                    else
                        throw;
                }
            }
            while (true);
        }

        #endregion

        #region ��������� ������

        public void ClearDebugInfo()
        {
            _debugInfo = new StringBuilder();
        }

        public byte ShortStatusRequest(bool ignoreDSR, byte mode, int retriesCount)
        {
            if (ignoreDSR)
                SetDsrFlow(false);
            try
            {
                do
                {
                    try
                    {
                        // ������� ������� �����
                        CommPort.DiscardBuffers();
                        // ������ ���������� �����
                        Write(new byte[] { 0x10, mode });
                        // ������ ������
                        return ReadByte();
                    }
                    catch (TimeoutException)
                    {
                        if (retriesCount > 0)
                            retriesCount--;
                        else
                            throw;
                    }
                }
                while (true);
            }
            finally
            {
                if (ignoreDSR)
                    SetDsrFlow(true);
            }
        }

        public byte ShortStatusRequest(bool ignoreDSR, byte mode)
        {
            if (ignoreDSR)
                SetDsrFlow(false);
            try
            {
                // ������� ������� �����
                CommPort.DiscardBuffers();
                // ������ ���������� �����
                Write(new byte[] { 0x10, mode });
                // ������ ������
                return ReadByte();
            }
            finally
            {
                if (ignoreDSR)
                    SetDsrFlow(true);
            }
        }

        /// <summary>
        /// �������� ������������ �������
        /// </summary>
        /// <param name="buffer">������ �������</param>
        /// <param name="bytesCount">����� �������</param>
        protected void WriteCommand(byte[] buffer, int bytesCount)
        {
            if (bytesCount > 0)
                ExecuteCommOperation("Write", 5, delegate() { CommPort.Write(buffer, 0, bytesCount); });
        }

        /// <summary>
        /// ������ � ����
        /// </summary>
        /// <param name="buffer">����� ������</param>
        public void Write(byte[] buffer)
        {
            try
            {
                if (buffer.Length > 0)
                    ExecuteCommOperation("Write", 5, delegate() { CommPort.Write(buffer); });
            }
            catch (TimeoutException)
            {
                byte statusByte = ShortStatusRequest(true, 0x30);

                // ������ ����������� ����������
                if ((statusByte & 0x20) == 0x0)
                {
                    WriteDebugLine("Write: ������ ����������� ����������");
                    throw new PrintableErrorException();
                }

            }
        }

        /// <summary>
        /// ������ � ����
        /// </summary>
        /// <param name="buffer">����� ������</param>
        public void WriteByte(byte value)
        {
            try
            {
                ExecuteCommOperation("WriteByte", 5, () => CommPort.WriteByte(value));
            }
            catch (TimeoutException)
            {
                byte statusByte = ShortStatusRequest(true, 0x30);

                // ������ ����������� ����������
                if ((statusByte & 0x20) == 0x0)
                {
                    WriteDebugLine("WriteByte: ������ ����������� ����������");
                    throw new PrintableErrorException();
                }

            }
        }

        #endregion

        #region ���������� ������

        protected byte ReadByte()
        {
            byte readByte = 0;
            ExecuteCommOperation("ReadByte", 5, delegate() { readByte = (byte)CommPort.ReadByte(); });
            return readByte;
        }

        protected void Read(byte[] buffer, int offset, int count)
        {
            ExecuteCommOperation("Read", 1, delegate() { CommPort.Read(buffer, offset, count); });
//            _commPort.Read(buffer, offset, count);
        }

        protected void SetDsrFlow(bool value)
        {
            WriteDebugLine(String.Format("SetDsrFlow({0})", value));
            ExecuteCommOperation("SetDsrFlow", 5, delegate() { CommPort.DsrFlow = value; });
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
                _debugInfo.AppendFormat("\t{0:X}\r\n", String.Join(" ", bufDump, 0, nBufferLen));
            else
                _debugInfo.Append("\t���\r\n");
        }

        #endregion
    }
}
