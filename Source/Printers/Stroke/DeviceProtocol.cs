using System;
using System.Text;
using DevicesCommon.Helpers;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace Stroke
{
    internal struct StrokePrinterFlags
    {
        /// <summary>
        /// Режим ККМ (открытая/закрытая смена, открытый документ, блокировка и т.д.)
        /// </summary>
        public byte Mode;

        /// <summary>
        /// Статус режима (тип открытого документа, статус подкладного документа)
        /// </summary>
        public byte StateMode;

        /// <summary>
        /// Подрежим ФР (состояние бумаги и статус печати)
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
        #region Константы

        private const int MAX_CMD_LEN = 128;
        private const int MAX_RETRIES_COUNT = 5;

        private const byte ENQ = 0x05;
        private const byte STX = 0x02;
        private const byte ACK = 0x06;
        private const byte NAK = 0x15;
        private const byte ETX = 0x03;
        private const byte SYN = 0x16;

        /// <summary>
        /// Таймаут ожидания приема каждого байта при чтении ответа на конанду
        /// </summary>
        private const int T1 = 500;

        /// <summary>
        /// Таймаут ожидания реакции на запрос ENQ (суммарное время ожидания не менее 10с)
        /// </summary>
        private const int T2 = 10000 / MAX_RETRIES_COUNT;

        /// <summary>
        /// Таймаут ожидания приема STX после отправки команды
        /// </summary>
        private const int T3 = 15000;

        /// <summary>
        /// Интервал проверки состояния принтера при ожидании завершения печати
        /// </summary>
        private const int T4 = 500;

        /// <summary>
        /// Пароль оператора по умолчанию
        /// </summary>
        private const uint DEF_OPERATOR_PASSWD = 30;

        #endregion

        #region Поля

        private int _cmdLen = 0;
        private int _rspLen = 0;
        private byte[] _cmdBuffer = new byte[MAX_CMD_LEN];
        private byte[] _rspBuffer = new byte[MAX_CMD_LEN];
        private EasyCommunicationPort _port;
        private StringBuilder _debugInfo = new StringBuilder();

        #endregion

        #region Конструкторы

        public StrokeProtocol(EasyCommunicationPort port)
        {
            _port = port;
        }

        #endregion

        #region Свойства

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

        #region Методы

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
                _debugInfo.Append("\tнет\r\n");
        }

        public void CreateCommand(byte nCmd)
        {
            _cmdBuffer = new byte[MAX_CMD_LEN];
            _rspBuffer = new byte[MAX_CMD_LEN];
            _cmdLen = 0;
            _rspLen = 0;

            // начало команды
            _cmdBuffer[_cmdLen++] = STX;
            // длина команды
            _cmdBuffer[_cmdLen++] = 0;
            // номер команды
            _cmdBuffer[_cmdLen++] = nCmd;
        }

        public void CreateCommand(byte nCmd, long nPasswd)
        {
            CreateCommand(nCmd);
            // пароль
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
            // режим ККМ: младший полубайт байта ответа 5
            printerFlags.Mode = (byte)(Response[5] & 0x0F);
            // статус режима: старший полубайт байта ответа 5
            printerFlags.StateMode = (byte)(Response[5] >> 4);
            // подрежим ФР: байт ответа 6
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
                    // режим ККМ: младший полубайт байта ответа 5
                    printerFlags.Mode = (byte)(Response[5] & 0x0F);
                    // статус режима: старший полубайт байта ответа 5
                    printerFlags.StateMode = (byte)(Response[5] >> 4);
                    // подрежим ФР: байт ответа 6
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

            // отправка запроса на передачу данных
            int retries = 0;
            do
            {
                _port.DiscardBuffers();
                _port.ReadTimeout = T2;
                try
                {
                    WriteDebugLine("Отправка запроса на передачу данных, попытка " + (retries + 1));
                    _port.WriteByte(ENQ);
                    rspByte = (byte)_port.ReadByte();
                }
                catch (TimeoutException)
                {
                    WriteDebugLine("Таймаут при отправке запроса на передачу данных");
                    if (retries++ >= MAX_RETRIES_COUNT - 1)
                    {
                        WriteDebugLine("Исчерпаны все попытки начать обмен (шаг 1)");
                        throw;
                    }
                }
                // если вдруг ФР хочет передать ответ на предыдущую команду, нужно его выслушать
                if (rspByte == ACK)
                {
                    WriteDebugLine("Получет байт ACK. Чтение ответа на предыдущую команду");
                    SkipResponseBytes();
                }
                WriteDebugLine("Получен байт ответа " + rspByte);
            }
            while (rspByte != NAK);

            retries = 0;
            do
            {
                _port.DiscardBuffers();
                _port.ReadTimeout = T1;
                try
                {
                    WriteDebugLine("Отправка данных команды, попытка " + (retries + 1));
                    _port.Write(_cmdBuffer, 0, _cmdLen);
                    rspByte = (byte)_port.ReadByte();
                }
                catch (TimeoutException)
                {
                    WriteDebugLine("Таймаут при передаче данных команды");
                    if (retries++ >= MAX_RETRIES_COUNT - 1)
                    {
                        WriteDebugLine("Исчерпаны все попытки передачи данных (шаг 4)");
                        throw;
                    }
                }

                if(rspByte == NAK)
                    WriteDebugLine("Получен байт NAK (шаг 2)");
                else if (rspByte != ACK)
                    WriteDebugLine("Ответ не соответствует протоколу (шаг 3): " + rspByte);
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
            return string.Format("Байты команды ({0}):\n{1:X}\nБайты ответа ({2}):\n{3:X}",
                ReqLen, string.Join(" ", reqDump, 0, ReqLen),
                RspLen, string.Join(" ", rspDump, 0, RspLen));
        }

        public int GetReceiptNo(long password)
        {
            WriteDebugLine("Запрос сквозного номера документа");
            ExecuteCommand(0x11, password);
            return (int)GetInt(11, 2);
        }

        #endregion

        #region Скрытые методы

        // Ожидание завешения выполнения команды
        private void DoWaitCommandExecute()
        {            
            WriteDebugLine("DoWaitCommandExecute");
            try
            {
                WriteDebugLine("Запрос состояния");
                ExecuteCommand(0x10, DEF_OPERATOR_PASSWD);
                while ((Response[6] == 5) || (Response[6] == 4))
                {
                    System.Threading.Thread.Sleep(T4);
                    WriteDebugLine("Запрос состояния");
                    ExecuteCommand(0x10, DEF_OPERATOR_PASSWD);
                }
            }
            catch (TimeoutException)
            {
                WriteDebugLine("Таймат во время ожидания завершения выполнения команды");
            }
        }

        /// <summary>
        /// Чтение байтов ответа на предыдущее сообщение без анализа содержимого
        /// </summary>
        private void SkipResponseBytes()
        {
            WriteDebugLine("SkipResponseBytes");
            _port.ReadTimeout = T3;

            if ((byte)_port.ReadByte() != STX)
                return; // не ответное сообщение

            // длина ответа без контрольной суммы
            int size = _port.ReadByte();
            // чтение ответа
            byte[] buffer = new byte[MAX_CMD_LEN];
            _port.Read(buffer, 0, size + 1);
            // подтверждение приема
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
                    // устанавливаем таймаут ожидания приема STX (15с)
                    _port.ReadTimeout = T3;

                    // если предыдущая попытка закончилась безуспешно, запрос повтора ответа на команду
                    if (!success)
                    {
                        WriteDebugLine("Отправка запроса повтора ответа на команду (отправка NAK)");
                        _port.WriteByte(NAK);
                    }

                    WriteDebugLine("Чтение ответа на команду, попытка " + (retries + 1));
                    success = (byte)_port.ReadByte() == STX;
                    if (!success)
                    {
                        WriteDebugLine("Первый байт ответа не соответствует протоколу");
                        if (retries++ >= MAX_RETRIES_COUNT - 1)
                            throw new TimeoutException("Первый байт ответа не соответствует протоколу");
                        continue;
                    }

                    // устанавливаем таймаут ожидания каждого байта 500мс
                    _port.ReadTimeout = T1;

                    // длина ответа без контрольной суммы
                    _rspLen = (byte)_port.ReadByte();

                    // чтение ответа
                    _port.Read(_rspBuffer, 0, _rspLen);

                    // вычисление контрольной суммы
                    byte nCRC = (byte)_rspLen;
                    for (int i = 0; i < _rspLen; i++)
                        nCRC ^= _rspBuffer[i];
                    success = nCRC == (byte)_port.ReadByte();

                    if (!success)
                    {
                        WriteDebugLine("Ошибка контрольной суммы в ответе на команду");
                        if (retries++ >= MAX_RETRIES_COUNT - 1)
                            throw new TimeoutException("Ошибка контрольной суммы в ответе на команду");
                        continue;
                    }
                    else
                    {
                        // подтверждение успешного приема
                        WriteDebugLine("Подтверждение успешного приема ответа на команду (отправка ACK)");
                        _port.WriteByte(ACK);
                    }
                }
                while (!success);


            }
            catch (TimeoutException)
            {
                WriteDebugLine("Таймаут приема ответа на команду, принятые байты ответа:", _rspBuffer, _rspLen);
                throw;
            }

            if (_rspBuffer[1] != 0)
                throw new DeviceErrorException(_rspBuffer[1]);
        }

        private void CompleteCmd()
        {
            // добавляем длину команды
            _cmdBuffer[1] = (byte)(_cmdLen - 2);
            // добавляем контрольную сумму
            for (int i = 1; i < _cmdLen; i++)
                _cmdBuffer[_cmdLen] ^= _cmdBuffer[i];
            _cmdLen++;
        }

        #endregion
    }
}