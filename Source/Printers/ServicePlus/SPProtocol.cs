using System;
using System.Text;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace ServicePlus
{
    public class DeviceErrorException: Exception
    {
        public short ErrorCode { get; set; }
    }

    internal class SPProtocol
    {
        #region Константы

        private const byte STX = 0x02;
        private const byte ETX = 0x03;
        private const byte ENQ = 0x05;
        private const byte ACK = 0x06;
        private const byte FS  = 0x1C;

        #endregion

        #region Поля

        private EasyCommunicationPort _port;

        private string _password;

        private Encoding _encoding = Encoding.GetEncoding(866);

        private byte[] _cmdBuffer = new byte[1024];
        private int _cmdLen = 0;
        private byte[] _rspBuffer = new byte[1024];
        private int _rspLen = 0;

        private static System.Globalization.NumberFormatInfo _nfi;

        #endregion


        public static System.Globalization.NumberFormatInfo Nfi
        {
            get             
            {
                if (_nfi == null)
                {
                    _nfi = new System.Globalization.NumberFormatInfo();
                    _nfi.NumberDecimalSeparator = ".";
                    _nfi.NumberGroupSeparator = string.Empty;
                }
                return _nfi; 
            }
        }



        #region Конструктор

        public SPProtocol(EasyCommunicationPort port, string password)
        {
            _port = port;
            _password = password;
        }

        #endregion

        internal void ExecuteCommand(string code, params string[] args)
        {
            ExecuteCommand(code, false, args);
        }

        internal void ExecuteCommand(string code, bool waitExecution, params string[] args)
        {
            _cmdBuffer = new byte[1024];
            _cmdLen = 0;
            _rspBuffer = new byte[1024];
            _rspLen = 0;

            // стартовый байт
            _cmdBuffer[_cmdLen++] = STX;
            // пароль передачи данных
            AppendString(_password);
            // отличительный байт
            _cmdBuffer[_cmdLen++] = 0x20;
            // код сообщения
            AppendString(code);
            // данные сообщения
            foreach(string value in args)
            {
                // параметр
                AppendString(value);
                // разделительный символ
                _cmdBuffer[_cmdLen++] = FS;                
            }

            // стоповый байт
            _cmdBuffer[_cmdLen++] = ETX;
            // контрольная сумма
            AppendString(CalculateBCC(_cmdBuffer, _cmdLen));

            // отправка команды

            // сброс буферов порта
            _port.DiscardBuffers();

            // передача команды
            _port.Write(_cmdBuffer, 0, _cmdLen);


            // дожидаемся завершения выполнения команды
            if (waitExecution)
            {
                DoWaitForExecute();
                _rspBuffer[_rspLen++] = STX;
            }

            // чтение ответа
            _port.Read(_rspBuffer, _rspLen, 6);
            _rspLen += 6;
            while (_rspBuffer[_rspLen - 1] != ETX)
                _rspBuffer[_rspLen++] = (byte)_port.ReadByte();
            _port.Read(_rspBuffer, _rspLen, 2);
            _rspLen += 2;

            // проверка контрольной суммы
            string rspBcc = _encoding.GetString(_rspBuffer, _rspLen - 2, 2);
            if(rspBcc != CalculateBCC(_rspBuffer, _rspLen))
            {

            }

            // проверка кода ошибки
            int errorCode = Convert.ToInt32(_encoding.GetString(_rspBuffer, 4, 2));
            if (errorCode != 0)
                throw new DeviceErrorException() { ErrorCode = (short)errorCode };
        }

        private void DoWaitForExecute()
        {
            do            
            {
                System.Threading.Thread.Sleep(500);
                _port.WriteByte(ENQ);
            }
            while(_port.ReadByte() == ACK);
        }

        internal string GetFieldAsString(int index)
        {
            byte[] field = new byte[256];
            int size = 0;

            int currField = 1;
            for (int i = 6; i < _rspLen - 3 && index >= currField; i++)
            {
                if (_rspBuffer[i] == FS)
                    currField++;
                else if (index == currField)
                    field[size++] = _rspBuffer[i];
            }

            if (size == 0)
                throw new ArgumentOutOfRangeException();

            return _encoding.GetString(field, 0, size);
        }

        internal int GetFieldAsInt(int index)
        {
            return Convert.ToInt32(GetFieldAsString(index));
        }

        internal decimal GetFieldAsDecimal(int index)
        {
            return Convert.ToDecimal(GetFieldAsString(index));
        }


        private string CalculateBCC(byte[] buffer, int size)
        {
            byte bcc = 0;

            // считаем сумму байт команды
            for (int i = 1; i < size; i++)
                bcc ^= buffer[i];

            // преобразуем числовое значение контрольной суммы
            // в шестнадцатиричное строковое представление
            return bcc.ToString("X2").ToUpper();
        }

        private void AppendString(string value)
        {
            Array.Copy(_encoding.GetBytes(value), 0, _cmdBuffer, _cmdLen, _encoding.GetByteCount(value));
            _cmdLen += _encoding.GetByteCount(value);
        }

        internal bool ShortStatusInquiry()
        {
            try
            {
                _port.WriteByte(ENQ);
                return _port.ReadByte() == ACK;
            }
            catch (TimeoutException)
            {
                return false;
            }
        }

        internal string GetCommandDump()
        {
            string[] reqDump = Array.ConvertAll<byte, string>(_cmdBuffer, b => b.ToString("X"));
            string[] rspDump = Array.ConvertAll<byte, string>(_rspBuffer, b => b.ToString("X"));
            return string.Format("Байты команды ({0}):\n{1:X}\nБайты ответа ({2}):\n{3:X}",
                _cmdLen, string.Join(" ", reqDump, 0, _cmdLen), _rspLen, string.Join(" ", rspDump, 0, _rspLen));
        }
    }
}
