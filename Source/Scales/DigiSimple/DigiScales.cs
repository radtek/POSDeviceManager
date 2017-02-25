using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon;
using DevicesCommon.Helpers;
using DevicesBase;
using DevicesBase.Communicators;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace DigiSimple
{
    /// <summary>
    /// Реализация протокола весов DIGI (порционные, прилавочная торговля)
    /// </summary>
    [Scale(DeviceNames.digiSimpleScales)]
    public sealed class DigiScales : CustomSerialDevice, IScaleDevice
    {
        private String _connStr;
        private Thread _worker;
        private Int32 _weight;
        private Object _syncObject;
        private Boolean _terminated;
        private Byte[] _rawData;
        private Int32 _rawPos;
        private Byte[] _strData;
        private Int32 _timeoutsCount;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        public DigiScales()
        {
            // инициализируем значение веса
            _weight = -1;
            // создаем рабочий поток
            _worker = new Thread(WorkerRoutine);
            // создаем объект для синхронизации доступа
            _syncObject = new Object();
            // флаг для завершения рабочего потока
            _terminated = true;
        }

        /// <summary>
        /// Флаг завершения работы
        /// </summary>
        private Boolean Terminated
        {
            get
            {
                Boolean t;
                lock (_syncObject)
                {
                    t = _terminated;
                }
                return t;
            }
            set
            {
                lock (_syncObject)
                {
                    _terminated = value;
                }
            }
        }

        /// <summary>
        /// Разбор "сырых" данных из очереди
        /// </summary>
        private void ParseRawData()
        {
            // если прочитано 23 или 32 байта
            // то разбираем принятую последовательность
            //
            // 23 байта - если не установлен вес тары
            // 32 байта - если вес тары установлен
            if (_rawPos == 23 || _rawPos == 32)
            {
                // достаем из "сырых" данных строку
                // с данными веса без плавающей точки
                Int32 j = 0;
                for (Int32 i = 6; i < 13; i++)
                {
                    if (_rawData[i] != 0x2E)
                    {
                        _strData[j] = _rawData[i];
                        j++;
                    }
                }

                // строку преобразуем в целое
                Weight = Convert.ToInt32(Encoding.Default.GetString(_strData));
            }

            //переходим к началу очереди
            _rawPos = 0;
        }

        /// <summary>
        /// Метод, реализующий рабочий поток
        /// </summary>
        private void WorkerRoutine()
        {
            Byte[] opData = new Byte[1024];
            _strData = new Byte[6];
            _rawData = new Byte[50];
            _rawPos = 0;
            _timeoutsCount = 0;

            while (!Terminated)
            {
                try
                {
                    // читаем все, что есть в буфере порта
                    Int32 bytesRead = Port.Read(opData, 0, opData.Length);
                    if (bytesRead == -1)
                    {
                        // ничего не прочитали
                        // приостанавливаемся для разгрузки процессора
                        Thread.Sleep(50);
                        
                        if (_timeoutsCount < Int32.MaxValue)
                            // увеличиваем счетчик операций чтения,
                            // завершившихся таймаутом
                            _timeoutsCount++;

                        if (_timeoutsCount > 5)
                            // если число таких операций превысило 2,
                            // считаем, что нет связи с весами
                            Weight = -1;
                    }
                    else
                    {
                        // сбрасываем счетчик таймаутов
                        _timeoutsCount = 0;

                        // разбираем полученные данные на последовательности,
                        // заканчивающиеся символом LF
                        for (Int32 i = 0; i < bytesRead; i++)
                        {
                            // текущий символ равен LF
                            if (opData[i] == 0x0A)
                                // разбираем "сырые" данные, помещенные в очередь
                                ParseRawData();
                            else
                            {
                                // копируем очередной байт в очередь "сырых данных"
                                _rawData[_rawPos] = opData[i];
                                _rawPos++;

                                // если очередь заполнена, перемещаемся к ее началу
                                if (_rawPos == _rawData.Length)
                                    _rawPos = 0;
                            }
                        }
                    }
                }
                catch (Win32Exception e)
                {
                    // сбрасываем флаг ошибки
                    Port.ClearError();
                    // очищаем буферы порта
                    Port.DiscardBuffers();
                    // перемещаемся к началу очереди
                    _rawPos = 0;

                    // протоколируем сообщение об ошибке
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Ошибка во время обмена данными с весами.");
                    sb.AppendLine(String.Format("Тип: {0}", e.GetType().Name));
                    sb.AppendLine(String.Format("Текст: {0}", e.Message));
                    sb.AppendLine("Трассировка стека:");
                    sb.Append(e.StackTrace);

                    Logger.WriteEntry(sb.ToString(), EventLogEntryType.Error);
                }
            }
        }

        #region Перегрузка методов базового класса

        /// <summary>
        /// После активации устройства
        /// </summary>
        protected override void OnAfterActivate()
        {
            // параметры порта
            Port.Parity = Parity.None;
            Port.DataBits = 8;
            Port.StopBits = StopBits.One;

            // таймаут чтения
            //Port.ReadTimeout = 5000;
            Port.ReadTimeout = -1;

            // сбрасываем флаг завершения
            Terminated = false;
            // запускаем рабочий поток
            _worker.Start();
        }

        /// <summary>
        /// Перед деактивацией устройства
        /// </summary>
        protected override void OnBeforeDeactivate()
        {
            lock (_syncObject)
            {
                // устанавливаем флаг завершения
                Terminated = true;
            }

            // ожидаем завершения работы потока
            _worker.Join();
        }

        #endregion

        #region Реализация IScaleDevice

        /// <summary>
        /// Строка подключения к весам
        /// </summary>
        public string ConnectionString
        {
            get { return _connStr; }
            set
            {
                _connStr = value;
                
                // разбираем строку подключения
                ConnStrHelper connStrHelper = new ConnStrHelper(_connStr);

                // поддерживается обмен только по RS-232
                if (String.Compare(connStrHelper[1], "rs", true) != 0)
                    throw new InvalidOperationException("Весы поддерживают обмен только по интерфейсу RS-232");

                // инициализируем параметры связи
                PortName = connStrHelper[2];
                Baud = Convert.ToInt32(connStrHelper[3]);
            }
        }

        /// <summary>
        /// Выгрузка данных в весы
        /// </summary>
        /// <param name="xmlData">Данные для выгрузки</param>
        public void Upload(string xmlData)
        {
            // операция не поддерживается весами
        }

        /// <summary>
        /// Текущие показания веса
        /// </summary>
        public Int32 Weight
        {
            get 
            {
                Int32 tmp;
                lock (_syncObject)
                {
                    tmp = _weight;
                }
                return tmp;
            }
            private set
            {
                lock (_syncObject)
                {
                    _weight = value;
                }
            }
        }

        #endregion
    }
}
