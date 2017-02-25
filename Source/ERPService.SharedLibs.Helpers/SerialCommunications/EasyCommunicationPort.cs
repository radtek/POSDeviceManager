using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace ERPService.SharedLibs.Helpers.SerialCommunications
{
    /// <summary>
    /// Аргументы события задания параметров порта
    /// </summary>
    public class CommStateEventArgs : EventArgs
    {
        private DCB _dcb;
        private Boolean _handled;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="dcb">Управляющая структура порта</param>
        public CommStateEventArgs(DCB dcb)
            : base()
        {
            _dcb = dcb;
            _handled = false;
        }

        /// <summary>
        /// Управляющая структура порта
        /// </summary>
        public DCB DCB
        {
            get
            {
                return _dcb;
            }
            set
            {
                _dcb = value;
            }
        }

        /// <summary>
        /// Событие обработано
        /// </summary>
        public Boolean Handled
        {
            get
            {
                return _handled;
            }
            set
            {
                _handled = value;
            }
        }
    }

    /// <summary>
    /// Коммуникационный порт
    /// </summary>
    public class EasyCommunicationPort : IDisposable
    {
        #region Закрытые поля класса

        // формат имени порта
        private const String _portNameFormat = @"\\.\{0}";
        // таймаут приема байта
        private const UInt32 _byteTimeout = 100;

        // внутренний буфер для чтения/записи в порт
        private Byte[] _operationBuffer = new Byte[1024];
        private Byte[] _smallBuffer = new Byte[1];

        // описатель порта
        private SafeFileHandle _handle;
        // имя порта
        private String _portName;
        // конфигурация порта
        private DCB _dcb;
        // таймауты порта
        private COMMTIMEOUTS _timeouts;
        // скорость
        private uint _baudRate;
        // четность 
        private Parity _parity;
        // стоповых бит
        private StopBits _stopBits;
        // бит данных
        private byte _dataBits;
        // контроль DTR/DSR
        private Boolean _dsrFlow;
        // таймаут чтения
        private uint _readTimeout;
        // таймаут записи
        private uint _writeTimeout;
        // признак последовательного порта
        private Boolean _isSerial;
        // генерировать ислючения при таймаутах
        private Boolean _throwTimeoutExceptions;
        // можно ли читать из параллельных портов
        private Boolean _canReadFromParallel;
        // размер входного буфера порта
        private uint _readBufferSize;
        // размер выходного буфера порта
        private uint _writeBufferSize;

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        public EasyCommunicationPort()
        {
            _handle = null;
            _portName = "COM1";
            _isSerial = true;
            _baudRate = 9600;
            _parity = Parity.None;
            _stopBits = StopBits.One;
            _dataBits = 8;
            _dsrFlow = false;
            _readTimeout = 50;
            _writeTimeout = 2000;
            _throwTimeoutExceptions = true;
            _canReadFromParallel = false;
            _readBufferSize = 2048;
            _writeBufferSize = 2048;
        }

        #endregion

        #region Закрытые свойства и методы

        /// <summary>
        /// Проверяет, открыт порт или нет
        /// </summary>
        private void CheckOpening()
        {
            if (!IsOpen)
                throw new Exception(String.Format("Порт {0} не открыт", _portName));
        }

        /// <summary>
        /// Устанавливает таймаут приема байта
        /// </summary>
        /// <param name="timeoutValue"></param>
        private void SetReadTimeout(UInt32 timeoutValue)
        {
            if (!IsSerial)
                return;
            _timeouts.ReadTotalTimeoutConstant = timeoutValue;
            WinApi.Win32Check(WinApi.SetCommTimeouts(_handle, ref _timeouts));
        }

        /// <summary>
        /// Выбрасывает исключение для таймаута в зависимости от свойства
        /// </summary>
        private void ThrowTimeoutException()
        {
            if (_throwTimeoutExceptions)
                throw new System.TimeoutException();
        }

        /// <summary>
        /// Задает размеры входного и выходного буферов порта
        /// </summary>
        private void SetupComm()
        {
            CheckOpening();
            WinApi.Win32Check(WinApi.SetupComm(_handle, _readBufferSize, _writeBufferSize));
        }

        /// <summary>
        /// Очистить буферы порта
        /// </summary>
        /// <param name="input">Очистить входящий буфер</param>
        /// <param name="output">Очистить исходящий буфер</param>
        private void DiscardBuffers(Boolean input, Boolean output)
        {
            // проверяем состояние порта
            CheckOpening();
            if (!IsSerial)
                // только для последовательных портов
                return;

            // инициализируем значение флагов
            PURGE_FLAGS flags = PURGE_FLAGS.PURGE_EMPTY;

            // если задано очистить входящий буфер
            if (input)
                // задаем флаги для очистки входящего буфера
                flags = flags | PURGE_FLAGS.PURGE_RXABORT | PURGE_FLAGS.PURGE_RXCLEAR;

            // если задано очистить исходящий буфер
            if (output)
                // задаем флаги для очистки исходящего буфера
                flags = flags | PURGE_FLAGS.PURGE_TXABORT | PURGE_FLAGS.PURGE_TXCLEAR;

            Boolean purgeResult = WinApi.PurgeComm(_handle, (uint)flags);
            if (!purgeResult)
            {
                uint errors = 0;

                if (Marshal.GetLastWin32Error() == (UInt32)SystemErrorCodes.ERROR_OPERATION_ABORTED)
                    WinApi.ClearCommError(_handle, ref errors, IntPtr.Zero);
                else
                    WinApi.Win32Check(purgeResult);
            }
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Можно ли выполнять операцию чтения на параллельных портах.
        /// Если нет, то все попытки чтения будут игнорироваться
        /// </summary>
        public Boolean CanReadFromParallel
        {
            get
            {
                return _canReadFromParallel;
            }
            set
            {
                _canReadFromParallel = value;
            }
        }

        /// <summary>
        /// Генерировать исключения по таймауту обмена с устройством
        /// </summary>
        public Boolean ThrowTimeoutExceptions
        {
            get
            {
                return _throwTimeoutExceptions;
            }
            set
            {
                _throwTimeoutExceptions = value;
            }
        }

        /// <summary>
        /// Возвращает признак работы по последовательному порту
        /// </summary>
        public Boolean IsSerial
        {
            get
            {
                return _isSerial;
            }
        }

        /// <summary>
        /// Возвращает признак открытия порта
        /// </summary>
        public Boolean IsOpen
        {
            get
            {
                return _handle != null && !_handle.IsInvalid;
            }
        }

        /// <summary>
        /// Имя порта
        /// </summary>
        public String PortName
        {
            get
            {
                return _portName;
            }
            set
            {
                Close();
                _portName = value;
                if (_portName.Length > 3)
                    // имя порта задано
                    _isSerial = String.Compare(_portName.Substring(0, 3), "COM", true) == 0;
                else
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Скорость обмена данными через порт
        /// </summary>
        public Int32 BaudRate
        {
            get
            {
                return (Int32)_baudRate;
            }
            set
            {
                _baudRate = (UInt32)value;
                if (_isSerial && IsOpen)
                {
                    _dcb.BaudRate = (uint)_baudRate;
                    WinApi.Win32Check(WinApi.SetCommState(_handle, ref _dcb));
                }
            }
        }

        /// <summary>
        /// Контроль четности
        /// </summary>
        public Parity Parity
        {
            get
            {
                return _parity;
            }
            set
            {
                _parity = value;
                if (_isSerial && IsOpen)
                {
                    _dcb.Parity = (byte)_parity;
                    WinApi.Win32Check(WinApi.SetCommState(_handle, ref _dcb));
                }
            }
        }

        /// <summary>
        /// Стоповых бит
        /// </summary>
        public StopBits StopBits
        {
            get
            {
                return _stopBits;
            }
            set
            {
                _stopBits = value;
                if (_isSerial && IsOpen)
                {
                    _dcb.StopBits = (byte)_stopBits;
                    WinApi.Win32Check(WinApi.SetCommState(_handle, ref _dcb));
                }
            }
        }

        /// <summary>
        /// Бит данных
        /// </summary>
        public Int32 DataBits
        {
            get
            {
                return _dataBits;
            }
            set
            {
                _dataBits = (byte)value;
                if (_isSerial && IsOpen)
                {
                    _dcb.ByteSize = _dataBits;
                    WinApi.Win32Check(WinApi.SetCommState(_handle, ref _dcb));
                }
            }
        }

        /// <summary>
        /// Контроль за состоянием линии DSR
        /// </summary>
        public Boolean DsrFlow
        {
            get
            {
                return _dsrFlow;
            }
            set
            {
                _dsrFlow = value;
                if (_isSerial && IsOpen)
                {
                    _dcb.fOutxDsrFlow = (UInt32)(_dsrFlow ? 1 : 0);
                    WinApi.Win32Check(WinApi.SetCommState(_handle, ref _dcb));
                }
            }
        }

        /// <summary>
        /// Таймаут чтения данных
        /// </summary>
        public Int32 ReadTimeout
        {
            get
            {
                return (Int32)_readTimeout;
            }
            set
            {
                CheckOpening();
                unchecked
                {
                    _readTimeout = (UInt32)value;
                }
                if (_isSerial)
                {
                    // таймауты чтения устанавливаются только для последовательного порта
                    if (_readTimeout == UInt32.MaxValue)
                    {
                        // операция чтения будет завершаться немедленно,
                        // возвращая данные, уже находящиеся в буфере порта
                        _timeouts.ReadIntervalTimeout = UInt32.MaxValue;
                        _timeouts.ReadTotalTimeoutMultiplier = 0;
                        _timeouts.ReadTotalTimeoutConstant = 0;
                    }
                    else
                    {
                        // операция чтения будет завершена, если было прочитано заданное 
                        // количество байт, либо истекло время ожидания
                        _timeouts.ReadIntervalTimeout = _byteTimeout;
                        _timeouts.ReadTotalTimeoutMultiplier = _byteTimeout;
                        _timeouts.ReadTotalTimeoutConstant = _readTimeout;
                    }
                    WinApi.Win32Check(WinApi.SetCommTimeouts(_handle, ref _timeouts));
                }
            }
        }

        /// <summary>
        /// Таймаут записи данных
        /// </summary>
        public Int32 WriteTimeout
        {
            get
            {
                return (Int32)_writeTimeout;
            }
            set
            {
                CheckOpening();
                _writeTimeout = (UInt32)value;
                // запись
                _timeouts.WriteTotalTimeoutMultiplier = 100;
                _timeouts.WriteTotalTimeoutConstant = _writeTimeout;
                WinApi.Win32Check(WinApi.SetCommTimeouts(_handle, ref _timeouts));
            }
        }

        /// <summary>
        /// Включает или отключает сигнал DTR
        /// </summary>
        public Boolean DtrEnable
        {
            set
            {
                CheckOpening();
                WinApi.Win32Check(WinApi.EscapeCommFunction(_handle,
                    (uint)(value ? ExtendedFunctions.SETDTR : ExtendedFunctions.CLRDTR)));
            }
        }

        /// <summary>
        /// Включает или отключает сигнал RTS
        /// </summary>
        public Boolean RtsEnable
        {
            set
            {
                CheckOpening();
                WinApi.Win32Check(WinApi.EscapeCommFunction(_handle,
                    (uint)(value ? ExtendedFunctions.SETRTS : ExtendedFunctions.CLRRTS)));
            }
        }

        /// <summary>
        /// Устанавливает размер входного буфера порта
        /// </summary>
        public int ReadBufferSize
        {
            set
            {
                _readBufferSize = (uint)value;
                SetupComm();
            }
        }

        /// <summary>
        /// Устанавливает размер выходного буфера порта
        /// </summary>
        public int WriteBufferSize
        {
            set
            {
                _writeBufferSize = (uint)value;
                SetupComm();
            }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Ожидание появления символа во входном буфере порта
        /// </summary>
        /// <param name="customChar">Символ определяется пользователем</param>
        public Boolean WaitChar(Boolean customChar)
        {
            UInt32 mask = 0;
            WinApi.Win32Check(WinApi.SetCommMask(_handle,
                customChar ? (UInt32)CommEvents.EV_RXFLAG : (UInt32)CommEvents.EV_RXCHAR));
            try
            {
                WinApi.Win32Check(WinApi.WaitCommEvent(_handle, ref mask, IntPtr.Zero));
                if (customChar)
                    return (mask & (UInt32)CommEvents.EV_RXFLAG) == (UInt32)CommEvents.EV_RXFLAG;
                else
                    return (mask & (UInt32)CommEvents.EV_RXCHAR) == (UInt32)CommEvents.EV_RXCHAR;
            }
            finally
            {
                WinApi.Win32Check(WinApi.SetCommMask(_handle, (uint)CommEvents.EV_TXEMPTY));
            }
        }

        /// <summary>
        /// Открывает коммункационный порт
        /// </summary>
        public void Open()
        {
            // если порт открыт
            if (IsOpen)
                // закрываем его
                Close();

            // открываем порт
            _handle = WinApi.CreateFile(
                String.Format(_portNameFormat, _portName),
                (uint)(DESIRED_ACCESS.GENERIC_READ | DESIRED_ACCESS.GENERIC_WRITE),
                0, 
                IntPtr.Zero, 
                CREATION_DISPOSITION.OPEN_EXISTING, 
                0, 
                IntPtr.Zero);

            // проверяем результат открытия
            WinApi.Win32Check(!_handle.IsInvalid);

            if (IsSerial)
            {
                // читаем настройки порта
                WinApi.Win32Check(WinApi.GetCommState(_handle, ref _dcb));

                // инициализируем настройки значениями по умолчанию
                _dcb.BaudRate = _baudRate;
                _dcb.ByteSize = _dataBits;
                _dcb.fOutxDsrFlow = _dsrFlow ? 1U : 0U;
                _dcb.Parity = (byte)_parity;
                _dcb.StopBits = (byte)_stopBits;

                // если есть обработчик события
                if (SetCommStateEvent != null)
                {
                    CommStateEventArgs e = new CommStateEventArgs(_dcb);
                    SetCommStateEvent(this, e);
                    if (e.Handled)
                        _dcb = e.DCB;
                }

                // задаем настройки
                WinApi.Win32Check(WinApi.SetCommState(_handle, ref _dcb));

                // устанавливаем маску событий
                WinApi.SetCommMask(_handle, (uint)CommEvents.EV_TXEMPTY);
            }
        }

        /// <summary>
        /// Закрывает коммуникационный порт
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Читает данные из порта
        /// </summary>
        /// <param name="buffer">Буфер для размещения полученных данных</param>
        /// <param name="offset">Смещение от начала буфера</param>
        /// <param name="count">Сколько прочитать</param>
        public Int32 Read(Byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "Не задан буфер данных для чтения из порта");
            if (count < 1)
                throw new ArgumentOutOfRangeException("count", count, "Длина данных для чтения слишком мала");

            CheckOpening();
            if (!IsSerial && !_canReadFromParallel)
            {
                // если параллельный порт и стоит признак запрета 
                // на чтение для параллельных портов
                Array.Clear(buffer, offset, count);
                return count;
            }

            UInt32 totalRead = 0, bytesRead;

            // очищаем массив для хранения данных
            Array.Clear(buffer, offset, count);
            // засекаем время
            DateTime startTime = DateTime.Now;
            do
            {
                // очищаем временный буфер
                Array.Clear(_operationBuffer, 0, _operationBuffer.Length);
                UInt32 recentDataSize = (UInt32)count - totalRead;
                bytesRead = 0;

                // читаем данные
                WinApi.Win32Check(WinApi.ReadFile(
                    _handle,
                    _operationBuffer,
                    recentDataSize >
                        (UInt32)_operationBuffer.Length ? (UInt32)_operationBuffer.Length : recentDataSize,
                    ref bytesRead,
                    IntPtr.Zero));

                // если что-то прочитали
                if (bytesRead > 0)
                {
                    // копируем в исходный буфер
                    Array.Copy(_operationBuffer, 0, buffer, offset + totalRead, bytesRead);
                    // увеличиваем число прочитанных байт
                    totalRead += bytesRead;
                }

                if (_readTimeout != UInt32.MaxValue)
                {
                    TimeSpan ellapsed = DateTime.Now.Subtract(startTime);
                    if (Math.Abs(_readTimeout - ellapsed.TotalMilliseconds) < _byteTimeout)
                        // если разница между значением таймаута и прощедшим временем ожидания
                        // меньше таймаута приема байта
                        ThrowTimeoutException();
                }
            }
            while (bytesRead > 0 && totalRead < count);
            if (totalRead > 0)
                return (Int32)totalRead;
            else
            {
                if (_readTimeout != UInt32.MaxValue)
                    ThrowTimeoutException();
                return -1;
            }
        }

        /// <summary>
        /// Читает один байт из входящего буфера порта
        /// </summary>
        public Int32 ReadByte()
        {
            UInt32 bytesRead = 0;
            WinApi.Win32Check(WinApi.ReadFile(_handle, _smallBuffer, 1, ref bytesRead, 
                IntPtr.Zero));

            if (bytesRead > 0)
                return (Int32)_smallBuffer[0];
            else
            {
                ThrowTimeoutException();
                return -1;
            }
        }

        /// <summary>
        /// Запись данных в порт. Пишется весь буфер целиком с нулевым смещением
        /// </summary>
        /// <param name="buffer">Буфер данных для записи</param>
        public Int32 Write(Byte[] buffer)
        {
            return Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Запись данных в порт
        /// </summary>
        /// <param name="buffer">Буфер данных для записи</param>
        /// <param name="offset">Смещение от начала буфера</param>
        /// <param name="count">Сколько записать</param>
        public Int32 Write(Byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "Не задан буфер данных для записи в порт");
            if (count < 1)
                throw new ArgumentOutOfRangeException("count", count, "Длина записываемых данных слишком мала");

            UInt32 totalWritten = 0, bytesWritten;
            CheckOpening();
            do
            {
                UInt32
                    recentDataSize = (UInt32)count - totalWritten,
                    bytesToWrite = recentDataSize > (UInt32)_operationBuffer.Length ? (UInt32)_operationBuffer.Length : recentDataSize;
                bytesWritten = 0;

                // копируем часть данных из исходного буфера во временный
                Array.Clear(_operationBuffer, 0, _operationBuffer.Length);
                Array.Copy(buffer, offset + totalWritten, _operationBuffer, 0, bytesToWrite);

                // записываем данные
                WinApi.Win32Check(WinApi.WriteFile(_handle, _operationBuffer, bytesToWrite,
                    ref bytesWritten, IntPtr.Zero));

                // если что-то записали
                if (bytesWritten > 0)
                    // увеличиваем число прочитанных байт
                    totalWritten += bytesWritten;
            }
            while (bytesWritten > 0 && totalWritten < count);
            if (totalWritten > 0)
                return (Int32)totalWritten;
            else
            {
                ThrowTimeoutException();
                return -1;
            }
        }

        /// <summary>
        /// Запись одного байта в порт
        /// </summary>
        /// <param name="byteToWrite">Байт для записи</param>
        public void WriteByte(Int32 byteToWrite)
        {
            _smallBuffer[0] = (Byte)byteToWrite;
            Int32 bytesWritten = Write(_smallBuffer, 0, 1);
            if (bytesWritten == 0)
                ThrowTimeoutException();
        }

        /// <summary>
        /// Очистить входящий буфер и прекратить прием данных
        /// </summary>
        public void DiscardInBuffer()
        {
            DiscardBuffers(true, false);
        }

        /// <summary>
        /// Очистить исходящий буфер и прекратить запись данных
        /// </summary>
        public void DiscardOutBuffer()
        {
            DiscardBuffers(false, true);
        }

        /// <summary>
        /// Очистить все буферы порта, прервать все операции ввода/вывода
        /// </summary>
        public void DiscardBuffers()
        {
            DiscardBuffers(true, true);
        }

        /// <summary>
        /// Сброс буфера порта и запись данных в устройство
        /// </summary>
        public void Flush()
        {
            WinApi.Win32Check(WinApi.FlushFileBuffers(_handle));
        }

        /// <summary>
        /// Сброс флага ошибки устройства
        /// </summary>
        public void ClearError()
        {
            UInt32 errors = 0;
            WinApi.Win32Check(WinApi.ClearCommError(_handle, ref errors, IntPtr.Zero));
        }

        #endregion

        #region События

        /// <summary>
        /// Событие задания параметров порта (расширенное)
        /// </summary>
        public event EventHandler<CommStateEventArgs> SetCommStateEvent;

        #endregion

        #region Реализация IDisposable

        /// <summary>
        /// Освобождает ресурсы, используемые устройством
        /// </summary>
        public void Dispose()
        {
            if (_handle != null)
            {
                // закрываем порт
                _handle.Dispose();
                _handle = null;
            }
        }

        #endregion
    }
}
