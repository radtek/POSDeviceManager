using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// ������� ���������� ������� ������� ����������
    /// </summary>
    public class EventLink : MarshalByRefObject, IDisposable, IEventLink
    {
        #region ����������� ������

        /// <summary>
        /// ������������ ��������
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
        /// ���������� �������� ����� �������
        /// </summary>
        public override object InitializeLifetimeService()
        {
            return null;
        }

        #endregion

        #region ���������

        private const String _recordIdFmt = "RECID:{0}";
        private const String _defaultLogFilePrefix = "app";
        private const String _storageNameFmt = "{0}\\{1}-{2}.log";
        private const String _dateRangeError = "������ ������� ��������� ���. ������ ��������� {0} ������ ����� ��������� {1}";
        private const Int32 _defSeekSize = 16384;
        private const Int32 _fieldsCount = 6;

        #endregion

        #region ����

        // ��� ������������� �������������� �������
        private Mutex _syncFilesMutex;

        // ����� �������
        private FileStream _storageStream;
        // ��� ������ � �����
        private StreamWriter _storageWriter;
        // ����� ��� �������� ������ �������
        private readonly String _storageFolder;
        // ���� ���������� �������� ����
        private DateTime _lastOpeningDate;
        // ���� �������� ��� ������
        private readonly Boolean _readOnly;
        // ���������� ����� �������
        private readonly EventsQueue _eventsQueue;
        // ��������� ��� ������ �����
        private readonly Dictionary<String, IEnumerator<EventRecord[]>> _eventIterators;
        // ������� ���-������
        private readonly String _logFilePrefix;

        #endregion

        #region ������������

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="storageFolder">��� �����-��������� ��� �����</param>
        public EventLink(String storageFolder)
            : this(storageFolder, false, true, 10, _defaultLogFilePrefix)
        {
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="storageFolder">��� �����-��������� ��� �����</param>
        /// <param name="logFilePrefix">������� ���-������</param>
        public EventLink(String storageFolder, String logFilePrefix)
            : this(storageFolder, false, true, 10, logFilePrefix)
        {
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="storageFolder">��� �����-��������� ��� �����</param>
        /// <param name="readOnly">������� ������ ��� ������</param>
        public EventLink(String storageFolder, Boolean readOnly)
            : this(storageFolder, readOnly, true, 10, _defaultLogFilePrefix)
        {
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="storageFolder">��� �����-��������� ��� �����</param>
        /// <param name="readOnly">������� ������ ��� ������</param>
        /// <param name="logFilePrefix">������� ���-������</param>
        public EventLink(String storageFolder, Boolean readOnly, String logFilePrefix)
            : this(storageFolder, readOnly, true, 10, logFilePrefix)
        {
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="storageFolder">��� �����-��������� ��� �����</param>
        /// <param name="readOnly">������� ������ ��� ������</param>
        /// <param name="bufferedOutput">���������������� ����� ������� � ����</param>
        /// <param name="flushPeriod">������������� ������ ������, �������</param>
        public EventLink(String storageFolder, Boolean readOnly, Boolean bufferedOutput, 
            Int32 flushPeriod)
            : this(storageFolder, readOnly, bufferedOutput, flushPeriod, _defaultLogFilePrefix)
        {
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="storageFolder">��� �����-��������� ��� �����</param>
        /// <param name="readOnly">������� ������ ��� ������</param>
        /// <param name="bufferedOutput">���������������� ����� ������� � ����</param>
        /// <param name="flushPeriod">������������� ������ ������, �������</param>
        /// <param name="logFilePrefix">������� ���-������</param>
        public EventLink(String storageFolder, Boolean readOnly, Boolean bufferedOutput,
            Int32 flushPeriod, String logFilePrefix)
        {
            if (String.IsNullOrEmpty(storageFolder))
                throw new ArgumentNullException("storageFolder");
            if (flushPeriod < 0)
                throw new ArgumentOutOfRangeException("flushPeriod");
            if (String.IsNullOrEmpty(logFilePrefix))
                throw new ArgumentNullException("logFilePrefix");

            _storageFolder = storageFolder;
            _readOnly = readOnly;
            _logFilePrefix = logFilePrefix;

            _syncFilesMutex = MutexHelper.CreateSyncFilesMutex(storageFolder);

            _eventIterators = new Dictionary<String, IEnumerator<EventRecord[]>>();

            if (!Directory.Exists(_storageFolder))
                Directory.CreateDirectory(_storageFolder);

            if (!_readOnly)
            {
                // ��������� ���
                OpenStorage();

                // ���������� ����� �������
                _eventsQueue = new EventsQueue(bufferedOutput, flushPeriod, FlushBuffer);
            }
        }

        #endregion

        #region �������� �������� � ������

        #region ����� ������ � ����

        private void FlushBuffer(IEnumerable<EventRecord> eventRecords)
        {
            // ���������, �� ���������� �� ��������� ����
            if (DateChanged)
                // ��������� ����� ���
                OpenStorage();

            _syncFilesMutex.WaitMutex();
            try
            {
                foreach (EventRecord record in eventRecords)
                {
                    // ������������� ������������� ������
                    record.Id = Guid.NewGuid().ToString();
                    // ��������� ������
                    EventRecordHelper.Save(record, _storageWriter);
                }

                // ���������� ����� ��������� ������
                _storageWriter.Flush();
            }
            finally
            {
                _syncFilesMutex.ReleaseMutex();
            }
        }

        #endregion

        #region ������

        /// <summary>
        /// ��������� ���
        /// </summary>
        private void CloseStorage()
        {
            // ���� ��� ������ 
            if (_storageWriter != null)
            {
                // ��������� ���
                _storageWriter.Close();
                _storageWriter = null;
                _storageStream = null;
            }
            else
            {
                // ���� ������ �����
                if (_storageStream != null)
                {
                    // ��������� ���
                    _storageStream.Close();
                    _storageStream = null;
                }
            }
        }

        /// <summary>
        /// ��������� ���
        /// </summary>
        private void OpenStorage()
        {
            _syncFilesMutex.WaitMutex();
            try
            {
                // ��������� ������� ���
                CloseStorage();

                // ������ ���� �������� ����
                _lastOpeningDate = DateTime.Today;

                // ��������� ��������� ��� ����
                String logName = GetStorageName(_lastOpeningDate);

                if (File.Exists(logName))
                {
                    // ��������� ������������ ���
                    _storageStream = new FileStream(logName, FileMode.Open,
                        FileAccess.Write, FileShare.Read, 1024, FileOptions.WriteThrough);
                    
                    _storageStream.Seek(_storageStream.Length, SeekOrigin.Begin);
                }
                else
                {
                    // ��������� ����� ��� 
                    _storageStream = new FileStream(logName, FileMode.Create,
                        FileAccess.Write, FileShare.Read, 1024, FileOptions.WriteThrough);
                }

                // ������� ��������������� ������ ��� ������ � ���
                _storageWriter = new StreamWriter(_storageStream, Encoding.Default);
            }
            finally
            {
                _syncFilesMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// ���������� ��� ��������� �� ����� ����
        /// </summary>
        /// <param name="mask">����� ����</param>
        /// <returns>��� ��������� �� ����� ����</returns>
        private String GetStorageName(DateTime mask)
        {
            // ���������� ��� ���������
            return String.Format(_storageNameFmt,
                _storageFolder,
                _logFilePrefix,
                mask.ToString("yyyy-MM-dd"));
        }

        /// <summary>
        /// ���������� true, ���� ���������� ��������� ���� � �������
        /// ���������� �������� ����
        /// </summary>
        private Boolean DateChanged
        {
            get
            {
                return (_lastOpeningDate != DateTime.Today);
            }
        }

        /// <summary>
        /// �������������� ���������� ������ ������� ����� ������� � ������ �����
        /// </summary>
        /// <param name="eventTypes">������ ���������� ����� �������</param>
        private List<String> EventsToStrings(EventType[] eventTypes)
        {
            List<String> eventsToPass = new List<String>();
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
        /// �������� ��������� �� ��������� ������, ���������� � ������ � ���
        /// </summary>
        /// <param name="rawData">���������</param>
        /// <returns>������ ��������� ��� ������ � ���</returns>
        private String[] SplitLine(String rawData)
        {
            return rawData.Split(new Char[] { (Char)10, (Char)13, (Char)9 },
                StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion

        #endregion

        #region ���������� IEventLink

        /// <summary>
        /// ������ ���������������� ������ � ��������
        /// </summary>
        /// <param name="fromDate">������ �� ����-�������, ��������� ��������</param>
        /// <param name="toDate">������ �� ����-�������, �������� ��������</param>
        /// <param name="sourceFilter">������ �� ��������� �������</param>
        /// <param name="eventFilter">������ �� ����� �������</param>
        /// <param name="maxEvents">������������ ���������� �������</param>
        /// <param name="eventPerIteration">
        /// ������������ ���������� �������, ������������ �� ������ �����
        /// ������ IEventLinkBasics.GetLog
        /// </param>
        /// <returns>������������� ���������</returns>
        public String BeginGetLog(DateTime fromDate, DateTime toDate, String[] sourceFilter,
            EventType[] eventFilter, Int32 maxEvents, Int32 eventPerIteration)
        {
            // ������� ��������� ��� ���������
            var iteratorContainer = new Iterators.EventsIterator(GetStorageName);
            
            // �������� �������� � ��������� ��� � ���-�������
            var iteratorId = Guid.NewGuid().ToString();

            // ��������� ������ � ���� �� ������
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

            // ���������� ������������� ���������
            return iteratorId;
        }

        /// <summary>
        /// ��������� ��������� ���� ������� �� �������
        /// </summary>
        /// <param name="iteratorId">������������� ���������</param>
        /// <returns>
        /// ��������� ���� ������� �� ������� ��� null, ���� ��������� ����� �������
        /// </returns>
        public EventRecord[] GetLog(String iteratorId)
        {
            if (String.IsNullOrEmpty(iteratorId))
                throw new ArgumentNullException("iteratorId");
            
            IEnumerator<EventRecord[]> iterator = null;

            if (!_eventIterators.TryGetValue(iteratorId, out iterator))
                throw new InvalidOperationException(String.Format(
                    "�������� � ��������������� [{0}] �� ������", iteratorId));

            if (iterator.MoveNext())
                return iterator.Current;

            return null;
        }

        /// <summary>
        /// ��������� ���������������� ������ � �������� 
        /// </summary>
        /// <param name="iteratorId">������������� ���������</param>
        public void EndGetLog(String iteratorId)
        {
            if (String.IsNullOrEmpty(iteratorId))
                throw new ArgumentNullException("iteratorId");

            IEnumerator<EventRecord[]> iterator = null;

            if (!_eventIterators.TryGetValue(iteratorId, out iterator))
                throw new InvalidOperationException(String.Format(
                    "�������� � ��������������� [{0}] �� ������", iteratorId));

            iterator.Dispose();
            _eventIterators.Remove(iteratorId);

            // ��������� ������ � ���� �� ������
            _syncFilesMutex.ReleaseMutex();
        }

        /// <summary>
        /// �������� �������
        /// </summary>
        /// <param name="fromDate">������ �� ����-�������, ��������� ��������</param>
        /// <param name="toDate">������ �� ����-�������, �������� ��������</param>
        /// <param name="sourceFilter">������ �� ��������� �������</param>
        /// <param name="eventFilter">������ �� ����� �������</param>
        public EventRecord[] GetLog(DateTime fromDate, DateTime toDate,
            String[] sourceFilter, EventType[] eventFilter)
        {
            return GetLog(fromDate, toDate, sourceFilter, eventFilter, -1);
        }

        /// <summary>
        /// �������� �������
        /// </summary>
        /// <param name="fromDate">������ �� ����-�������, ��������� ��������</param>
        /// <param name="toDate">������ �� ����-�������, �������� ��������</param>
        /// <param name="sourceFilter">������ �� ��������� �������</param>
        /// <param name="eventFilter">������ �� ����� �������</param>
        /// <param name="maxEvents">������������ ���������� �������</param>
        public EventRecord[] GetLog(DateTime fromDate, DateTime toDate,
            String[] sourceFilter, EventType[] eventFilter, Int32 maxEvents)
        {
            if (maxEvents == 0)
                throw new ArgumentOutOfRangeException("maxEvents");

            // ������� ��������� ��� ��������� �������
            List<EventRecord> resultEvents = new List<EventRecord>();

            // ������� �������� ��� �������� �������
            var iteratorId = BeginGetLog(fromDate, toDate, sourceFilter, eventFilter,
                maxEvents, 100);

            EventRecord[] eventRecords = null;
            do
            {
                // �������� ��������� ���� �������
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
        /// ���������� ��������������� �������
        /// </summary>
        /// <param name="sourceId">������������� ��������� �������</param>
        /// <param name="text">��������� ������ �������</param>
        public void Post(String sourceId, String text)
        {
            Post(sourceId, EventType.Information, SplitLine(text));
        }

        /// <summary>
        /// ���������� �������
        /// </summary>
        /// <param name="sourceId">������������� ��������� �������</param>
        /// <param name="eventType">��� �������</param>
        /// <param name="text">��������� ������ �������</param>
        public void Post(String sourceId, EventType eventType, String text)
        {
            Post(sourceId, eventType, SplitLine(text));
        }

        /// <summary>
        /// ���������� ��������������� ������� 
        /// </summary>
        /// <param name="sourceId">������������� ��������� �������</param>
        /// <param name="text">��������� ������ �������</param>
        public void Post(String sourceId, String[] text)
        {
            Post(sourceId, EventType.Information, text);
        }

        /// <summary>
        /// ���������� �������
        /// </summary>
        /// <param name="sourceId">������������� ��������� �������</param>
        /// <param name="eventType">��� �������</param>
        /// <param name="text">��������� ������ �������</param>
        public void Post(String sourceId, EventType eventType, String[] text)
        {
            if (_readOnly)
                throw new InvalidOperationException("������ ������� ������ ������ ��� ������");

            // ������� ����� ������ �������
            // ����� ������ ����� �������� ��� ������ ������
            _eventsQueue.Enqueue(new EventRecord(null, DateTime.Now, sourceId, eventType, text));
        }

        /// <summary>
        /// ���������� ���������� �� ����������
        /// </summary>
        /// <param name="sourceId">������������� ��������� �������</param>
        /// <param name="checkPointId">������������ ����������� �����</param>
        /// <param name="e">����������</param>
        public void Post(String sourceId, String checkPointId, Exception e)
        {
            Post(sourceId, EventType.Error, EventLinkExceptionHelper.GetStrings(checkPointId, e));
        }

        /// <summary>
        /// ���������� ���������� �� ����������
        /// </summary>
        /// <param name="sourceId">������������� ��������� �������</param>
        /// <param name="exceptionData">������ ����������, �������������� ��� ����������</param>
        public void PostException(String sourceId, String[] exceptionData)
        {
            Post(sourceId, EventType.Error, exceptionData);
        }

        /// <summary>
        /// �������������� ������� �������
        /// </summary>
        /// <param name="fromDate">������ �� ����-�������, ��������� ��������</param>
        /// <param name="toDate">������ �� ����-�������, �������� ��������</param>
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
