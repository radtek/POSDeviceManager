using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using DevicesCommon;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace DevicesBase
{
	/// <summary>
    /// Базовый класс для устройств, предназначенных для чтения данных карт доступа
	/// </summary>
	public abstract class CustomGenericReader : CustomSerialDevice, IGenericReader
    {
        #region Поля 

        private Parity _parity;
        private Byte _stopChar;
        private Queue<string> _data;
        private object _syncObject;
        private Thread _readerThread;
        private Boolean _terminated;
        private Byte[] _buffer;
        private StringBuilder _tempData;

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        protected CustomGenericReader()
            : base()
        {
            _parity = Parity.None;
            _terminated = false;
            _syncObject = new object();
            _stopChar = 0x0A;
            _data = new Queue<string>();
            _readerThread = new Thread(ReadData);
            _buffer = new Byte[1024];
            _tempData = new StringBuilder();
        }

        #endregion

        #region Закрытые свойства и методы

        private Boolean Terminated
        {
            get
            {
                Boolean value;
                lock (_syncObject)
                {
                    value = _terminated;
                }
                return value;
            }
            set
            {
                lock (_syncObject)
                {
                    _terminated = value;
                }
            }
        }

        private void LogException(Exception e)
        {
            _tempData.Length = 0;
            Logger.WriteEntry(e.Message, EventLogEntryType.Error);
        }

        /// <summary>
        /// Подготовка полученных от сканера данных
        /// </summary>
        /// <param name="rawData">"Сырые" данные</param>
        protected abstract string Prepare(string rawData);

        /// <summary>
        /// После активации устройства
        /// </summary>
        protected override void OnAfterActivate()
        {
            Port.Parity = _parity;
            Port.DsrFlow = false;
            Port.ReadTimeout = -1;
            Port.DiscardBuffers();

            // начинаем читать данные
            _readerThread.Start();
        }

        /// <summary>
        /// Перед деактивацией устройства
        /// </summary>
        protected override void OnBeforeDeactivate()
        {
            // остановка потока, читающего данные
            Terminated = true;
            _readerThread.Join(30000);
        }

        private void ReadData()
        {
            while (!Terminated)
            {
                try
                {
                    // читаем все, что есть во входящем буфере порта
                    Array.Clear(_buffer, 0, _buffer.Length);
                    Int32 bytesRead = Port.Read(_buffer, 0, _buffer.Length);

                    if (bytesRead > 0)
                    {
                        // помещаем считанные данные во временную строку
                        for (Int32 i = 0; i < bytesRead; i++)
                        {
                            if (_buffer[i] == _stopChar)
                            {
                                // завершена очередная строка данных
                                // разбор строки
                                string preparedData = Prepare(_tempData.ToString());
                                if (!string.IsNullOrEmpty(preparedData))
                                {
                                    lock (_syncObject)
                                    {
                                        // помещаем в очередь
                                        _data.Enqueue(preparedData);
                                    }
                                }

                                // очищаем временное хранилище данных
                                _tempData.Length = 0;
                            }
                            else
                            {
                                // во временное хранилище попадают только читаемые символы
                                if (_buffer[i] > 0x20)
                                    _tempData.Append((Char)_buffer[i]);
                            }
                        }
                    }
                    else
                        Thread.Sleep(100);
                }
                catch (Win32Exception e)
                {
                    LogException(e);
                }
                catch (TimeoutException e)
                {
                    LogException(e);
                }
            }
        }

        #endregion

        #region Реализация ICardReader

        /// <summary>
        /// Четность
        /// </summary>
        public Parity Parity
        {
            get { return _parity; }
            set { _parity = value; }
        }

        /// <summary>
        /// Стоп-символ
        /// </summary>
        public Byte StopChar
        {
            get { return _stopChar; }
            set { _stopChar = value; }
        }

        /// <summary>
        /// Очередной блок данных
        /// </summary>
        public string Data
        {
            get 
            {
                string nextData;
                lock (_syncObject)
                {
                    // извлекаем очередную строку данных из очереди
                    nextData = _data.Dequeue();
                }
                return nextData;
            }
        }

        /// <summary>
        /// Состояние очереди
        /// </summary>
        public Boolean Empty
        {
            get
            {
                Boolean state;
                lock (_syncObject)
                {
                    state = _data.Count == 0;
                }
                return state;
            }
            set
            {
                // очистка очереди данных
                if (value)
                {
                    lock (_syncObject)
                    {
                        _data.Clear();
                    }
                }
            }
        }

        #endregion
    }
}
