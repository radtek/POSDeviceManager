using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// Базовая реализация журнала событий приложения
    /// </summary>
    public class EventLink : MarshalByRefObject, IDisposable, IEventLink
    {
        #region Виртуальные методы

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public virtual void Dispose()
        {
            if (_eventsQueue != null)
            {
                _eventsQueue.Dispose();
            }

            CloseStorage();

            if (_syncFilesMutex != null)
            {
                _syncFilesMutex.Close();
                _syncFilesMutex = null;
            }
        }

        /// <summary>
        /// Управление временем жизни объекта
        /// </summary>
        public override object InitializeLifetimeService()
        {
            return null;
        }

        #endregion

        #region Константы

        private const string _recordIdFmt = "RECID:{0}";
        private const string _defaultLogFilePrefix = "app";
        private const string _storageNameFmt = "{0}\\{1}-{2}.log";
        private const string _dateRangeError = "Ошибка задания диапазона дат. Начало диапазона {0} больше конца диапазона {1}";
        private const int _defSeekSize = 16384;
        private const int _fieldsCount = 6;

        #endregion

        #region Поля

        // для синхронизации многопоточного доступа
        private Mutex _syncFilesMutex;

        // поток журнала
        private FileStream _storageStream;
        // для записи в поток
        private StreamWriter _storageWriter;
        // папка для хранения файлов журнала
        private readonly string _storageFolder;
        // дата последнего открытия лога
        private DateTime _lastOpeningDate;
        // флаг открытия для чтения
        private readonly bool _readOnly;
        // внутренний буфер событий
        private readonly EventsQueue _eventsQueue;
        // итераторы для чтения логов
        private readonly Dictionary<string, IEnumerator<EventRecord[]>> _eventIterators;
        // префикс лог-файлов
        private readonly string _logFilePrefix;

        #endregion

        #region Конструкторы

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="storageFolder">Имя папки-хранилища для логов</param>
        public EventLink(string storageFolder)
            : this(storageFolder, false, true, 10, _defaultLogFilePrefix)
        {
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="storageFolder">Имя папки-хранилища для логов</param>
        /// <param name="logFilePrefix">Префикс лог-файлов</param>
        public EventLink(string storageFolder, string logFilePrefix)
            : this(storageFolder, false, true, 10, logFilePrefix)
        {
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="storageFolder">Имя папки-хранилища для логов</param>
        /// <param name="readOnly">Открыть только для чтения</param>
        public EventLink(string storageFolder, bool readOnly)
            : this(storageFolder, readOnly, true, 10, _defaultLogFilePrefix)
        {
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="storageFolder">Имя папки-хранилища для логов</param>
        /// <param name="readOnly">Открыть только для чтения</param>
        /// <param name="logFilePrefix">Префикс лог-файлов</param>
        public EventLink(string storageFolder, bool readOnly, string logFilePrefix)
            : this(storageFolder, readOnly, true, 10, logFilePrefix)
        {
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="storageFolder">Имя папки-хранилища для логов</param>
        /// <param name="readOnly">Открыть только для чтения</param>
        /// <param name="bufferedOutput">Буферизированный вывод событий в файл</param>
        /// <param name="flushPeriod">Периодичность сброса буфера, секунды</param>
        public EventLink(string storageFolder, bool readOnly, bool bufferedOutput,
            int flushPeriod)
            : this(storageFolder, readOnly, bufferedOutput, flushPeriod, _defaultLogFilePrefix)
        {
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="storageFolder">Имя папки-хранилища для логов</param>
        /// <param name="readOnly">Открыть только для чтения</param>
        /// <param name="bufferedOutput">Буферизированный вывод событий в файл</param>
        /// <param name="flushPeriod">Периодичность сброса буфера, секунды</param>
        /// <param name="logFilePrefix">Префикс лог-файлов</param>
        public EventLink(string storageFolder, bool readOnly, bool bufferedOutput,
            int flushPeriod, string logFilePrefix)
        {
            if (string.IsNullOrEmpty(storageFolder))
                throw new ArgumentNullException("storageFolder");
            if (flushPeriod < 0)
                throw new ArgumentOutOfRangeException("flushPeriod");
            if (string.IsNullOrEmpty(logFilePrefix))
                throw new ArgumentNullException("logFilePrefix");

            _storageFolder = storageFolder;
            _readOnly = readOnly;
            _logFilePrefix = logFilePrefix;

            _syncFilesMutex = MutexHelper.CreateSyncFilesMutex(storageFolder);

            _eventIterators = new Dictionary<string, IEnumerator<EventRecord[]>>();

            if (!Directory.Exists(_storageFolder))
                Directory.CreateDirectory(_storageFolder);

            if (!_readOnly)
            {
                // открываем лог
                OpenStorage();

                // внутренний буфер событий
                _eventsQueue = new EventsQueue(bufferedOutput, flushPeriod, FlushBuffer);
            }
        }

        #endregion

        #region Закрытые свойства и методы

        #region Сброс буфера в файл

        private void FlushBuffer(IEnumerable<EventRecord> eventRecords)
        {
            // проверяем, не изменилась ли системная дата
            if (DateChanged)
                // открываем новый лог
                OpenStorage();

            _syncFilesMutex.WaitMutex();
            try
            {
                foreach (EventRecord record in eventRecords)
                {
                    // устанавливаем идентификатор записи
                    record.Id = Guid.NewGuid().ToString();
                    // сохраняем запись
                    EventRecordHelper.Save(record, _storageWriter);
                }

                // сбрасываем буфер файлового потока
                _storageWriter.Flush();
            }
            finally
            {
                _syncFilesMutex.ReleaseMutex();
            }
        }

        #endregion

        #region Прочее

        /// <summary>
        /// Закрывает лог
        /// </summary>
        private void CloseStorage()
        {
            // если лог открыт 
            if (_storageWriter != null)
            {
                // закрываем его
                _storageWriter.Close();
                _storageWriter = null;
                _storageStream = null;
            }
            else
            {
                // если открыт поток
                if (_storageStream != null)
                {
                    // закрываем его
                    _storageStream.Close();
                    _storageStream = null;
                }
            }
        }

        /// <summary>
        /// Открывает лог
        /// </summary>
        private void OpenStorage()
        {
            _syncFilesMutex.WaitMutex();
            try
            {
                // закрываем текущий лог
                CloseStorage();

                // меняем дату открытия лога
                _lastOpeningDate = DateTime.Today;

                // формируем ожидаемое имя лога
                string logName = GetStorageName(_lastOpeningDate);

                if (File.Exists(logName))
                {
                    // открываем существующий лог
                    _storageStream = new FileStream(logName, FileMode.Open,
                        FileAccess.Write, FileShare.Read, 1024, FileOptions.WriteThrough);
                    
                    _storageStream.Seek(_storageStream.Length, SeekOrigin.Begin);
                }
                else
                {
                    // открываем новый лог 
                    _storageStream = new FileStream(logName, FileMode.Create,
                        FileAccess.Write, FileShare.Read, 1024, FileOptions.WriteThrough);
                }

                // создаем вспомогательный объект для записи в лог
                _storageWriter = new StreamWriter(_storageStream, Encoding.Default);
            }
            finally
            {
                _syncFilesMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Возвращает имя хранилища по маске даты
        /// </summary>
        /// <param name="mask">Маска даты</param>
        /// <returns>Имя хранилища по маске даты</returns>
        private string GetStorageName(DateTime mask)
        {
            // возвращаем имя хранилища
            return string.Format(_storageNameFmt,
                _storageFolder,
                _logFilePrefix,
                mask.ToString("yyyy-MM-dd"));
        }

        /// <summary>
        /// Возвращает true, если изменилась системная дата с момента
        /// последнего открытия лога
        /// </summary>
        private bool DateChanged
        {
            get
            {
                return (_lastOpeningDate != DateTime.Today);
            }
        }

        /// <summary>
        /// Преобразование комбинации флагов фильтра типов событий в список строк
        /// </summary>
        /// <param name="eventTypes">Список допустимых типов событий</param>
        private List<string> EventsToStrings(EventType[] eventTypes)
        {
            List<string> eventsToPass = new List<string>();
            if (eventTypes != null)
            {
                foreach (EventType et in eventTypes)
                {
                    eventsToPass.Add(EventTypeConvertor.ConvertFrom(et));
                }
            }
            return eventsToPass;
        }

        /// <summary>
        /// Разбивка сообщения на отдельные строки, подготовка к записи в лог
        /// </summary>
        /// <param name="rawData">Сообщение</param>
        /// <returns>Строки сообщения для записи в лог</returns>
        private string[] SplitLine(string rawData)
        {
            return rawData.Split(new Char[] { (Char)10, (Char)13, (Char)9 },
                StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion

        #endregion

        #region Реализация IEventLink

        /// <summary>
        /// Начать последовательный доступ к событиям
        /// </summary>
        /// <param name="fromDate">Фильтр по дате-времени, начальное значение</param>
        /// <param name="toDate">Фильтр по дате-времени, конечное значение</param>
        /// <param name="sourceFilter">Фильтр по источнику событий</param>
        /// <param name="eventFilter">Фильтр по типам событий</param>
        /// <param name="maxEvents">Максимальное количество событий</param>
        /// <param name="eventPerIteration">
        /// Максимальное количество событий, возвращаемое на каждый вызов
        /// метода IEventLinkBasics.GetLog
        /// </param>
        /// <returns>Идентификатор итератора</returns>
        public string BeginGetLog(DateTime fromDate, DateTime toDate, string[] sourceFilter,
            EventType[] eventFilter, int maxEvents, int eventPerIteration)
        {
            // создаем контейнер для итератора
            var iteratorContainer = new Iterators.EventsIterator(GetStorageName);
            
            // получаем итератор и сохраняем его в хэш-таблице
            var iteratorId = Guid.NewGuid().ToString();

            // блокируем доступ к логу на запись
            _syncFilesMutex.WaitMutex();

            var iterator = iteratorContainer.GetBufferedEvents(
                new Iterators.EventIteratorParams()
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    SourceFilter = sourceFilter,
                    EventTypeFilter = EventsToStrings(eventFilter),
                    MaxEvents = maxEvents,
                    MaxEventsPerIteration = eventPerIteration
                }).GetEnumerator();

            _eventIterators.Add(iteratorId, iterator);

            // возвращаем идентификатор итератора
            return iteratorId;
        }

        /// <summary>
        /// Загрузить очередной блок событий из журнала
        /// </summary>
        /// <param name="iteratorId">Идентификатор итератора</param>
        /// <returns>
        /// Очередной блок событий из журнала или null, если достигнут конец журнала
        /// </returns>
        public EventRecord[] GetLog(string iteratorId)
        {
            if (string.IsNullOrEmpty(iteratorId))
                throw new ArgumentNullException("iteratorId");
            
            IEnumerator<EventRecord[]> iterator = null;

            if (!_eventIterators.TryGetValue(iteratorId, out iterator))
                throw new InvalidOperationException(string.Format(
                    "Итератор с идентификатором [{0}] не найден", iteratorId));

            if (iterator.MoveNext())
                return iterator.Current;

            return null;
        }

        /// <summary>
        /// Завершить последовательный доступ к событиям 
        /// </summary>
        /// <param name="iteratorId">Идентификатор итератора</param>
        public void EndGetLog(string iteratorId)
        {
            if (string.IsNullOrEmpty(iteratorId))
                throw new ArgumentNullException("iteratorId");

            IEnumerator<EventRecord[]> iterator = null;

            if (!_eventIterators.TryGetValue(iteratorId, out iterator))
                throw new InvalidOperationException(string.Format(
                    "Итератор с идентификатором [{0}] не найден", iteratorId));

            iterator.Dispose();
            _eventIterators.Remove(iteratorId);

            // открываем доступ к логу на запись
            _syncFilesMutex.ReleaseMutex();
        }

        /// <summary>
        /// Загрузка журнала
        /// </summary>
        /// <param name="fromDate">Фильтр по дате-времени, начальное значение</param>
        /// <param name="toDate">Фильтр по дате-времени, конечное значение</param>
        /// <param name="sourceFilter">Фильтр по источнику событий</param>
        /// <param name="eventFilter">Фильтр по типам событий</param>
        public EventRecord[] GetLog(DateTime fromDate, DateTime toDate,
            string[] sourceFilter, EventType[] eventFilter)
        {
            return GetLog(fromDate, toDate, sourceFilter, eventFilter, -1);
        }

        /// <summary>
        /// Загрузка журнала
        /// </summary>
        /// <param name="fromDate">Фильтр по дате-времени, начальное значение</param>
        /// <param name="toDate">Фильтр по дате-времени, конечное значение</param>
        /// <param name="sourceFilter">Фильтр по источнику событий</param>
        /// <param name="eventFilter">Фильтр по типам событий</param>
        /// <param name="maxEvents">Максимальное количество событий</param>
        public EventRecord[] GetLog(DateTime fromDate, DateTime toDate,
            string[] sourceFilter, EventType[] eventFilter, int maxEvents)
        {
            if (maxEvents == 0)
                throw new ArgumentOutOfRangeException("maxEvents");

            // создаем контейнер для найденных событий
            List<EventRecord> resultEvents = new List<EventRecord>();

            // создаем итератор для перебора событий
            var iteratorId = BeginGetLog(fromDate, toDate, sourceFilter, eventFilter,
                maxEvents, 100);

            EventRecord[] eventRecords = null;
            do
            {
                // получаем очередной блок записей
                eventRecords = GetLog(iteratorId);
                if (eventRecords != null)
                {
                    resultEvents.AddRange(eventRecords);
                }
            }
            while (eventRecords != null);

            return resultEvents.ToArray();
        }

        /// <summary>
        /// Публикация информационного события
        /// </summary>
        /// <param name="sourceId">Идентификатор источника событий</param>
        /// <param name="text">Текстовые данные события</param>
        public void Post(string sourceId, string text)
        {
            Post(sourceId, EventType.Information, SplitLine(text));
        }

        /// <summary>
        /// Публикация события
        /// </summary>
        /// <param name="sourceId">Идентификатор источника событий</param>
        /// <param name="eventType">Тип события</param>
        /// <param name="text">Текстовые данные события</param>
        public void Post(string sourceId, EventType eventType, string text)
        {
            Post(sourceId, eventType, SplitLine(text));
        }

        /// <summary>
        /// Публикация информационного события 
        /// </summary>
        /// <param name="sourceId">Идентификатор источника событий</param>
        /// <param name="text">Текстовые данные события</param>
        public void Post(string sourceId, string[] text)
        {
            Post(sourceId, EventType.Information, text);
        }

        /// <summary>
        /// Публикация события
        /// </summary>
        /// <param name="sourceId">Идентификатор источника событий</param>
        /// <param name="eventType">Тип события</param>
        /// <param name="text">Текстовые данные события</param>
        public void Post(string sourceId, EventType eventType, string[] text)
        {
            if (_readOnly)
                throw new InvalidOperationException("Журнал событий открыт только для чтения");

            // создаем новую запись журнала
            // номер записи будет присвоен при сбросе буфера
            _eventsQueue.Enqueue(new EventRecord(null, DateTime.Now, sourceId, eventType, text));
        }

        /// <summary>
        /// Публикация информации об исключении
        /// </summary>
        /// <param name="sourceId">Идентификатор источника событий</param>
        /// <param name="checkPointId">Наименование контрольной точки</param>
        /// <param name="e">Исключение</param>
        public void Post(string sourceId, string checkPointId, Exception e)
        {
            Post(sourceId, EventType.Error, EventLinkExceptionHelper.GetStrings(checkPointId, e));
        }

        /// <summary>
        /// Публикация информации об исключении
        /// </summary>
        /// <param name="sourceId">Идентификатор источника событий</param>
        /// <param name="exceptionData">Данные исключения, подготовленные для публикации</param>
        public void PostException(string sourceId, string[] exceptionData)
        {
            Post(sourceId, EventType.Error, exceptionData);
        }

        /// <summary>
        /// Принудительная очистка журнала
        /// </summary>
        /// <param name="fromDate">Фильтр по дате-времени, начальное значение</param>
        /// <param name="toDate">Фильтр по дате-времени, конечное значение</param>
        public void TruncLog(DateTime fromDate, DateTime toDate)
        {
            var iterator = new Iterators.LogsIterator(GetStorageName);
            foreach (var storageName in iterator.LogsRundown(fromDate, toDate))
            {
                try
                {
                    File.Delete(storageName);
                }
                catch (IOException)
                {
                }
            }
        }

        #endregion
    }
}
