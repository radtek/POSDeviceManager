using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace ERPService.SharedLibs.Eventlog.Iterators
{
    /// <summary>
    /// ��������� ��������� ��� �������� �������
    /// </summary>
    internal class EventIteratorParams
    {
        #region ��������

        internal DateTime FromDate { get; set; }
        internal DateTime ToDate { get; set; }
        internal IEnumerable<String> SourceFilter { get; set; }
        internal IEnumerable<String> EventTypeFilter { get; set; }

        private Int32 _maxEvents;
        private Int32 _maxEventsPerIteration;

        internal Int32 MaxEvents
        {
            get { return _maxEvents; }
            set 
            {
                if (value != -1 && value < 1)
                    throw new ArgumentOutOfRangeException("value");
                _maxEvents = value; 
            }
        }

        internal Int32 MaxEventsPerIteration
        {
            get { return _maxEventsPerIteration; }
            set 
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value");
                _maxEventsPerIteration = value; 
            }
        }

        #endregion

        #region �����������

        internal EventIteratorParams()
        {
            FromDate = DateTime.Today.AddDays(-7);
            ToDate = DateTime.Today;
            MaxEvents = -1;
            MaxEventsPerIteration = 100;
        }

        #endregion
    }

    internal class EventsIterator
    {
        #region ���������

        private Int64 _reverseBlockSize = 5;

        #endregion

        #region ����

        private readonly Func<DateTime, String> _storageNamePredicate;

        #endregion

        #region �����������

        internal EventsIterator(Func<DateTime, String> storageNamePredicate)
        {
            _storageNamePredicate = storageNamePredicate;
        }

        #endregion

        #region ���������

        /// <summary>
        /// ���������� ������� � �����
        /// </summary>
        /// <param name="iteratorParams">��������� ���������</param>
        /// <returns>�������� ��� �������� �������</returns>
        private IEnumerable<EventRecord> GetEventsRundown(EventIteratorParams iteratorParams)
        {
            var logsIterator = new LogsIterator(_storageNamePredicate);
            var totalRead = 0;

            // ���������� ���� � ������� �������� ����
            foreach (var helper in logsIterator.StreamedLogsRundown(
                iteratorParams.FromDate, iteratorParams.ToDate))
            {
                // ������ ��� � �����, ��� ���������������� � ���� ���������� ������
                // ������ ��������� ������� �� _reverseBlockSize �������

                // ���������� ���������� �������� �� �������� ����
                var iterationsCount = helper.Index.RecordCount / _reverseBlockSize;
                if (helper.Index.RecordCount % _reverseBlockSize > 0)
                    iterationsCount++;

                if (iterationsCount == 0)
                    // ������ ���
                    continue;

                for (var i = 0L; i < iterationsCount; i++)
                {
                    // ���� �������������� ��������� ������ ����, ���� �� �� �������
                    var stopReadThisLog = false;

                    // ���������� ������ ������ ������ � ����� (���������� � ����)
                    var firstRecordOfBlock = helper.Index.RecordCount -
                        (i + 1) * _reverseBlockSize;
                    // � ����� �������, ������� ����� �������
                    var eventsToRead = _reverseBlockSize;

                    if (firstRecordOfBlock < 0)
                    {
                        // ���� ������ ����, ��� ��������, ��� ����� ������� � ���� 
                        // �� ������ _reverseBlockSize � ����� �������� eventsToRead:
                        eventsToRead = _reverseBlockSize + firstRecordOfBlock;

                        // ����� ������ ������ � ���� ������ ����� ����:
                        firstRecordOfBlock = 0;
                    }

                    // ��������� � ������� ����� ��������������� ������
                    helper.Index.Seek(firstRecordOfBlock);

                    // ������ ��������� ���� �������
                    var bufferedEvents = new EventRecord[eventsToRead];

                    try
                    {
                        // ���� ��������������� ������ �� ����
                        var seekInLog = true;

                        for (var j = 0L; j < eventsToRead; j++)
                        {
                            // ���������� ��������������� ������ ��������� ������
                            var currentIndex = helper.Index.GetNext();

                            if (seekInLog)
                            {
                                // ������������� �������� � ���� �� ������ ��������� ������
                                helper.Reader.Seek(currentIndex.Offset, SeekOrigin.Begin);
                                seekInLog = false;
                            }

                            // ������ ��������� ���������
                            for (var k = 0; k < currentIndex.LinesCount; k++)
                            {
                                var storageEntry = EventRecordHelper.GetRawEntry(
                                    helper.Reader.ReadLine());

                                if (!iteratorParams.SourceFilter.Contains(storageEntry[3].TrimEnd()) ||
                                    !iteratorParams.EventTypeFilter.Contains(storageEntry[4].TrimEnd()))
                                {
                                    // ��������� ������ ����� ��������� � ������� ���� ���������������
                                    // ������ �� ����
                                    seekInLog = true;
                                    break;
                                }

                                if (bufferedEvents[j] == null)
                                    bufferedEvents[j] = EventRecordHelper.CreateFromStorageEntry(
                                        storageEntry);
                                else
                                    bufferedEvents[j].Text.Add(storageEntry[5]);
                            }
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        // ������ � ������������ ����� ����
                        stopReadThisLog = true;
                    }

                    // ������������� ���� �������
                    Array.Reverse(bufferedEvents);

                    // ���������� ������ ��� ��������� ������ ���������
                    foreach (var eventRecord in bufferedEvents)
                    {
                        if (eventRecord == null)
                            // ���������� �������, �� �������� ��� ������
                            continue;

                        // ���������� �������
                        yield return eventRecord;
                        // ����������� ������� �������
                        totalRead++;
                        // �������� ������������� ���������� �������
                        if (totalRead == iteratorParams.MaxEvents)
                            yield break;
                    }

                    if (stopReadThisLog)
                        break;
                }
            }
        }

        /// <summary>
        /// ��������� �������������� ������ ������� �� �����
        /// </summary>
        /// <param name="iteratorParams">��������� ���������</param>
        /// <returns>�������� ��� ��������������� ������ �������</returns>
        internal IEnumerable<EventRecord[]> GetBufferedEvents(
            EventIteratorParams iteratorParams)
        {
            // ��������� ��������� ���������
            // ������ �� ���� ������� ������, ���������� ������
            if (iteratorParams.EventTypeFilter == null ||
                iteratorParams.EventTypeFilter.Count() == 0)
                yield break;

            // ������ �� ���������� ������� ������, ���������� ������
            if (iteratorParams.SourceFilter == null ||
                iteratorParams.SourceFilter.Count() == 0)
                yield break;

            var buffer = new List<EventRecord>(iteratorParams.MaxEventsPerIteration);

            foreach (var eventRecord in GetEventsRundown(iteratorParams))
            {
                buffer.Add(eventRecord);
                if (buffer.Count == iteratorParams.MaxEventsPerIteration)
                {
                    yield return buffer.ToArray();
                    buffer.Clear();
                }
            }

            // ���������� ������� �������
            if (buffer.Count > 0)
                yield return buffer.ToArray();
        }

        #endregion
    }
}
