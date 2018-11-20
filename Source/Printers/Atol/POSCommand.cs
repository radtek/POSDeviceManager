using System;
using System.Runtime.Serialization;
using System.Text;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace Atol
{
    [Serializable]
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

        protected DeviceErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    internal interface ICommunicationPortProvider
    {
        EasyCommunicationPort GetCommunicationPort();
    }

    internal class AtolProtocol
    {
        #region  онстанты

        const int MAX_CMD_LEN = 255;
        const int MAX_ENQ_RETRIES = 5;
        const int MAX_CMD_RETRIES = 10;

        const int ENQ = 0x05;
        const int ACK = 0x06;
        const int NAK = 0x15;
        const int EOT = 0x04;

        const int STX = 0x02;
        const int ETX = 0x03;
        const int DLE = 0x10;

        const int T1 = 500;
        const int T2 = 2000;
        const int T3 = 500;
        const int T4 = 500;

        const int T5 = 10000;
        const int T6 = 500;
        const int T7 = 500;

        #endregion

        #region ѕол€

        private byte[] _cmdBuffer = new byte[MAX_CMD_LEN];
        private byte[] _rspBuffer = new byte[MAX_CMD_LEN];
        private int _cmdLen = 0;
        private int _rspLen = 0;
        private int _operatorPassword;
        private ICommunicationPortProvider _portProvider;

        #endregion

        #region —крытые методы

        private void CompleteCmd()
        {
            byte[] nCmd = new byte[MAX_CMD_LEN];

            int nCmdLen = 0;
            nCmd[nCmdLen++] = STX;

            for (int i = 0; i < _cmdLen; i++)
            {
                if ((_cmdBuffer[i] == DLE) || (_cmdBuffer[i] == ETX))
                    nCmd[nCmdLen++] = DLE;

                nCmd[nCmdLen++] = _cmdBuffer[i];
            }

            nCmd[nCmdLen++] = ETX;
            nCmd[nCmdLen++] = CalcCRC(nCmd, 1, nCmdLen);

            nCmd.CopyTo(_cmdBuffer, 0);
            _cmdLen = nCmdLen;
        }

        private void PassCommand()
        {
            int nRetries = 0;
            byte rspByte = 0;

            // запрос на передачу данных
            _commPort.ReadTimeout = T1;
            do
            {
                _commPort.ClearError();
                _commPort.DiscardBuffers();
                _commPort.WriteByte(ENQ);

                try
                {
                    rspByte = (byte)_commPort.ReadByte();

                    if (rspByte == ACK)
                        // приемник готов к установке св€зи
                        break;

                    if (rspByte == ENQ)
                        // конфликтна€ ситуаци€
                        System.Threading.Thread.Sleep(T7);
                    else
                        // неверный ответ на запрос установки св€зи
                        System.Threading.Thread.Sleep(T1);

                }
                catch (TimeoutException)
                {
                    // таймаут чтени€, повтор попытки
                    System.Threading.Thread.Sleep(T1);
                }
                // увеличиваем счетчик попыток
                nRetries++;
            }
            while (nRetries < MAX_ENQ_RETRIES);
            if (nRetries == MAX_ENQ_RETRIES)
                throw new TimeoutException();

            // передача данных
            nRetries = 0;
            _commPort.ReadTimeout = T3;
            do
            {
                _commPort.Write(_cmdBuffer, 0, _cmdLen);
                if (_commPort.ReadByte() == ACK)
                    break;

                nRetries++;
            }
            while (nRetries < MAX_CMD_RETRIES);
            if (nRetries == MAX_CMD_RETRIES)
                throw new TimeoutException();

            // отправка команды завершени€ сеанса св€зи
            _commPort.WriteByte(EOT);
        }

        private void ReadResponse(bool returnsError)
        {
            // ожидание начала передачи ответа на команду
            // таймаут ожидани€ начала передачи ответа выбираетс€ в зависимости от команды
            switch (_cmdBuffer[3])
            {
                case 0x4A:
                case 0x62:
                case 0xA7:
                case 0xA6:
                case 0x8D:
                case 0x8E:
                    _commPort.ReadTimeout = 20000;
                    break;
                case 0xA8:
                case 0xA9:
                case 0xAA:
                case 0xAB:
                case 0xAC:
                case 0xAD:
                    _commPort.ReadTimeout = 120000;
                    break;
                case 0x91:
                case 0x67:
                    _commPort.ReadTimeout = 45000;
                    break;
                default:
                    _commPort.ReadTimeout = T5;
                    break;
            }

            if (_commPort.ReadByte() != ENQ)
                throw new TimeoutException();

            // подтверждаем прием данных
            _commPort.WriteByte(ACK);

            int nRetries = 0;
            bool bSuccess = false;
            _commPort.ReadTimeout = T2;
            do
            {
                nRetries++;
                _rspLen = 0;
                Array.Clear(_rspBuffer, 0, _rspBuffer.Length);

                bool masked = false;
                bool lastByte = false;
                // чтение из порта пока не будет получен ETX
                do
                {
                    _rspBuffer[_rspLen] = (byte)_commPort.ReadByte();

                    lastByte = !masked && _rspBuffer[_rspLen] == ETX;
                    masked = !masked && _rspBuffer[_rspLen] == DLE;

                    _rspLen++;
                }
                while (!lastByte);

                // проверка контрольной суммы
                _rspBuffer[_rspLen++] = (byte)_commPort.ReadByte();
                bSuccess = CalcCRC(_rspBuffer, 1, _rspLen - 1) == _rspBuffer[_rspLen];

                if (!bSuccess)
                {
                    nRetries++;
                    if (nRetries > MAX_CMD_RETRIES)
                        throw new TimeoutException();
                }

                // подтверждение приема ответа
                _commPort.WriteByte(bSuccess ? ACK : NAK);
            }
            while (!bSuccess);

            // подтверждение приема ответа
            _commPort.ReadTimeout = T4;
            // чтение EOT
            _commPort.ReadByte();
            DecodeRsp();

            // провер€ем результат выполнени€ команды
            if (returnsError && (_rspBuffer[1] == 'U') && (_rspBuffer[2] != 0))
                throw new DeviceErrorException(_rspBuffer[2]);
        }

        private void DecodeRsp()
        {
            bool bMasked = false;
            int nRspLen = 0;

            byte[] nRsp = new byte[MAX_CMD_LEN];
            for (int i = 0; i < _rspLen; i++)
            {
                if ((_rspBuffer[i] == DLE) && (!bMasked))
                    bMasked = true;
                else
                {
                    nRsp[nRspLen++] = _rspBuffer[i];
                    bMasked = false;
                }
            }

            nRsp.CopyTo(_rspBuffer, 0);
            _rspLen = nRspLen;
        }

        private byte CalcCRC(byte[] nCmd, int nStart, int nLen)
        {
            byte nCRC = 0;
            for (int i = nStart; i < nLen + nStart; i++)
                nCRC = (byte)(nCRC ^ nCmd[i]);
            return nCRC;
        }

        #endregion

        #region —войства

        private EasyCommunicationPort _commPort
        {
            get { return _portProvider.GetCommunicationPort(); }
        }

        public byte[] Response
        {
            get { return _rspBuffer; }
        }

        public bool IsSlipMode
        {
            get // определение  фискальной станции
            {
                // запрос регистра
                CreateCommand(0x91);
                AddByte(0x1D);
                AddByte(0);
                AddByte(0);
                Execute();

                return Response[3] == 4;
            }
            set
            {
                // установка фискальной станции
                CreateCommand(0xB0);
                AddByte(0);
                AddBCD(value ? 4 : 1, 1);
                Execute();
            }
        }

        #endregion

        #region  онструктор

        public AtolProtocol(ICommunicationPortProvider portProvider, int operatorPassword)
        {
            _portProvider = portProvider;
            _operatorPassword = operatorPassword;
        }

        #endregion

        #region ѕубличные методы

        public void CreateCommand(byte cmdByte)
        {
            _cmdBuffer = new byte[MAX_CMD_LEN];
            _rspBuffer = new byte[MAX_CMD_LEN];
            _cmdLen = 0;
            _rspLen = 0;

            AddBCD(_operatorPassword, 2);
            _cmdBuffer[_cmdLen++] = cmdByte;
        }

        public void ExecuteCommand(byte command)
        {
            CreateCommand(command);
            Execute(command != 0x45);
        }

        public void AddBCD(long value, int size)
        {
            string sBCD = value.ToString(string.Format("d{0}", size * 2));

            int nPos = 0;
            while (size > 0)
            {
                _cmdBuffer[_cmdLen++] = (byte)(Convert.ToByte(sBCD.Substring(nPos++, 1)) * 0x10
                    + Convert.ToByte(sBCD.Substring(nPos++, 1)));
                size--;
            }
        }

        public void AddByte(byte value)
        {
            _cmdBuffer[_cmdLen++] = value;
        }

        public void AddBytes(byte[] values)
        {
            Array.Copy(values, 0, _cmdBuffer, _cmdLen, values.Length);
            _cmdLen += values.Length;
        }

        public void AddString(string value, int maxLen)
        {
            AddString(value, maxLen, false);
        }

        public void AddString(string value, int maxLen, bool doubleWidth)
        {
            if ((maxLen > 0) && (value.Length > maxLen))
                value = value.Substring(0, maxLen);

            value = value.Replace(
                System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator,
                " ");

            if (doubleWidth)
            {
                byte[] arrString = Encoding.GetEncoding(866).GetBytes(value);
                foreach (byte nChar in arrString)
                {
                    _cmdBuffer[_cmdLen++] = 0x09;
                    _cmdBuffer[_cmdLen++] = nChar;
                }
            }
            else
            {
                Array.Copy(Encoding.GetEncoding(866).GetBytes(value), 0, _cmdBuffer, _cmdLen, value.Length);
                _cmdLen += value.Length;
            }

            if ((maxLen > 0) && (value.Length < maxLen))
                _cmdLen += maxLen - value.Length;
        }

        public void SwitchToMode(int modeNo, int modePasswd)
        {
            // проверка текущего режима
            ExecuteCommand(0x45);

            // если текущий режим не тот, который нужен
            if ((Response[2] & 0x0F) != modeNo)
            {
                try
                {
                    // выход в режим выбора
                    ExecuteCommand(0x48);
                }
                catch (DeviceErrorException E)
                {
                    // если   ћ заблокирована, то не выходим, а пытаемс€ разблокировать установкой нужного режима
                    if (E.ErrorCode != 157)
                        throw;
                }

                // установка нужного режима
                CreateCommand(0x56);
                AddBCD(modeNo, 1);
                AddBCD(modePasswd, 4);
                Execute();
            }
        }

        public void Execute()
        {
            Execute(true);
        }

        /// <summary>
        /// ¬ыполнить команду. ≈сли команда возвращает код ошибки, генерируетс€ DeviceErrorException
        /// </summary>
        /// <param name="returnsError">ќтветное сообщение содержит код ошибки</param>
        /// <returns></returns>
        public void Execute(bool returnsError)
        {
            CompleteCmd();
            PassCommand();
            ReadResponse(returnsError);
        }

        public long GetFromBCD(int nStart, int nLen)
        {
            string sResult = "";
            for (int i = nStart; i < nStart + nLen; i++)
            {
                sResult += Convert.ToString(_rspBuffer[i] >> 4);
                sResult += Convert.ToString(_rspBuffer[i] & 0x0F);
            }
            return Convert.ToInt64(sResult);
        }

        internal string GetCommandDump()
        {
            string[] reqDump = Array.ConvertAll(_cmdBuffer,
                           new Converter<byte, string>(delegate(byte b) { return b.ToString("X"); }));
            string[] rspDump = Array.ConvertAll(_rspBuffer,
                new Converter<byte, string>(delegate(byte b) { return b.ToString("X"); }));
            return string.Format("Ѕайты команды ({0}):\n{1:X}\nЅайты ответа ({2}):\n{3:X}",
                _cmdLen, string.Join(" ", reqDump, 0, _cmdLen),
                _rspLen, string.Join(" ", rspDump, 0, _rspLen));
        }

        #endregion

    }
}
