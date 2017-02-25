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
        #region Поля

        // кодировка журнала
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

        #region Конструктор

        public SparkProtocolBase(ISparkDeviceProvider deviceProvider)
        {
            _deviceProvider = deviceProvider;
        }

        #endregion

        #region Скрытые методы

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
                    WriteDebugLine(String.Format("{0}. Таймаут", errMessage));
                    throw;
                }
                catch (System.ComponentModel.Win32Exception E)
                {
                    WriteDebugLine(String.Format("{0}. Ошибка {1}: {2}", errMessage, E.NativeErrorCode, E.Message));
                    if (E.NativeErrorCode == 995 && retriesCount > 0)
                    {
                        // сброс ошибки и повтор попытки
                        WriteDebugLine("Сброс флага ошибки устройства");
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

        #region Публичные методы

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
                        // очистка буферов порта
                        CommPort.DiscardBuffers();
                        // запрос статусного байта
                        Write(new byte[] { 0x10, mode });
                        // чтение ответа
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
                // очистка буферов порта
                CommPort.DiscardBuffers();
                // запрос статусного байта
                Write(new byte[] { 0x10, mode });
                // чтение ответа
                return ReadByte();
            }
            finally
            {
                if (ignoreDSR)
                    SetDsrFlow(true);
            }
        }

        /// <summary>
        /// Передача протокольной команды
        /// </summary>
        /// <param name="buffer">Данные команды</param>
        /// <param name="bytesCount">Длина команды</param>
        protected void WriteCommand(byte[] buffer, int bytesCount)
        {
            if (bytesCount > 0)
                ExecuteCommOperation("Write", 5, delegate() { CommPort.Write(buffer, 0, bytesCount); });
        }

        /// <summary>
        /// Запись в порт
        /// </summary>
        /// <param name="buffer">Байты данных</param>
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

                // ошибка печатающего устройства
                if ((statusByte & 0x20) == 0x0)
                {
                    WriteDebugLine("Write: ошибка печатающего устройства");
                    throw new PrintableErrorException();
                }

            }
        }

        /// <summary>
        /// Запись в порт
        /// </summary>
        /// <param name="buffer">Байты данных</param>
        public void WriteByte(byte value)
        {
            try
            {
                ExecuteCommOperation("WriteByte", 5, () => CommPort.WriteByte(value));
            }
            catch (TimeoutException)
            {
                byte statusByte = ShortStatusRequest(true, 0x30);

                // ошибка печатающего устройства
                if ((statusByte & 0x20) == 0x0)
                {
                    WriteDebugLine("WriteByte: ошибка печатающего устройства");
                    throw new PrintableErrorException();
                }

            }
        }

        #endregion

        #region Защищенные методы

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
                _debugInfo.Append("\tнет\r\n");
        }

        #endregion
    }
}
