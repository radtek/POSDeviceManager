using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace ERPService.SharedLibs.Eventlog.Iterators
{
    /// <summary>
    /// Параметры итератора для перебора событий
    /// </summary>
    internal class EventIteratorParams
    {
        #region Свойства

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

        #region Конструктор

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
        #region Константы

        private Int64 _reverseBlockSize = 5;

        #endregion

        #region Поля

        private readonly Func<DateTime, String> _storageNamePredicate;

        #endregion

        #region Конструктор

        internal EventsIterator(Func<DateTime, String> storageNamePredicate)
        {
            _storageNamePredicate = storageNamePredicate;
        }

        #endregion

        #region Итераторы

        /// <summary>
        /// Перебирает события в логах
        /// </summary>
        /// <param name="iteratorParams">Параметры итератора</param>
        /// <returns>Итератор для перебора событий</returns>
        private IEnumerable<EventRecord> GetEventsRundown(EventIteratorParams iteratorParams)
        {
            var logsIterator = new LogsIterator(_storageNamePredicate);
            var totalRead = 0;

            // перебираем логи в порядке убывания даты
            foreach (var helper in logsIterator.StreamedLogsRundown(
                iteratorParams.FromDate, iteratorParams.ToDate))
            {
                // читаем лог с конца, для позиционирования в логе используем индекс
                // чтение выполняем блоками по _reverseBlockSize событий

                // определяем количество итераций по текущему логу
                var iterationsCount = helper.Index.RecordCount / _reverseBlockSize;
                if (helper.Index.RecordCount % _reverseBlockSize > 0)
                    iterationsCount++;

                if (iterationsCount == 0)
                    // пустой лог
                    continue;

                for (var i = 0L; i < iterationsCount; i++)
                {
                    // флаг принудительной остановки чтения лога, если он не дописан
                    var stopReadThisLog = false;

                    // определяем индекс первой записи в блоке (начинается с нуля)
                    var firstRecordOfBlock = helper.Index.RecordCount -
                        (i + 1) * _reverseBlockSize;
                    // и число событий, которые нужно считать
                    var eventsToRead = _reverseBlockSize;

                    if (firstRecordOfBlock < 0)
                    {
                        // если попали сюда, это означает, что число записей в логе 
                        // не кратно _reverseBlockSize и нужно уточнить eventsToRead:
                        eventsToRead = _reverseBlockSize + firstRecordOfBlock;

                        // номер первой записи в этом случае равен нулю:
                        firstRecordOfBlock = 0;
                    }

                    // переходим к нужному куску индексированных данных
                    helper.Index.Seek(firstRecordOfBlock);

                    // читаем очередной блок записей
                    var bufferedEvents = new EventRecord[eventsToRead];

                    try
                    {
                        // флаг принудительного поиска по логу
                        var seekInLog = true;

                        for (var j = 0L; j < eventsToRead; j++)
                        {
                            // определяем индексированные данные очередной записи
                            var currentIndex = helper.Index.GetNext();

                            if (seekInLog)
                            {
                                // устанавливаем смещение в логе на начало очередной записи
                                helper.Reader.Seek(currentIndex.Offset, SeekOrigin.Begin);
                                seekInLog = false;
                            }

                            // читаем очередное сообщение
                            for (var k = 0; k < currentIndex.LinesCount; k++)
                            {
                                var storageEntry = EventRecordHelper.GetRawEntry(
                                    helper.Reader.ReadLine());

                                if (!iteratorParams.SourceFilter.Contains(storageEntry[3].TrimEnd()) ||
                                    !iteratorParams.EventTypeFilter.Contains(storageEntry[4].TrimEnd()))
                                {
                                    // прерываем чтение этого сообщения и взводим флаг принудительного
                                    // поиска по логу
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
                        // попали в недописанный кусок лога
                        stopReadThisLog = true;
                    }

                    // разворачиваем блок записей
                    Array.Reverse(bufferedEvents);

                    // возвращаем записи как результат работы итератора
                    foreach (var eventRecord in bufferedEvents)
                    {
                        if (eventRecord == null)
                            // пропускаем события, не попавшие под фильтр
                            continue;

                        // возвращаем событие
                        yield return eventRecord;
                        // увеличиваем счетчик событий
                        totalRead++;
                        // достигли максимального количества событий
                        if (totalRead == iteratorParams.MaxEvents)
                            yield break;
                    }

                    if (stopReadThisLog)
                        break;
                }
            }
        }

        /// <summary>
        /// Выполняет буферизованное чтение событий из логов
        /// </summary>
        /// <param name="iteratorParams">Параметры итератора</param>
        /// <returns>Итератор для буферизованного чтения событий</returns>
        internal IEnumerable<EventRecord[]> GetBufferedEvents(
            EventIteratorParams iteratorParams)
        {
            // проверяем параметры итератора
            // фильтр по типу событий пустой, возвращать нечего
            if (iteratorParams.EventTypeFilter == null ||
                iteratorParams.EventTypeFilter.Count() == 0)
                yield break;

            // фильтр по источникам событий пустой, возвращать нечего
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

            // возвращаем остаток событий
            if (buffer.Count > 0)
                yield return buffer.ToArray();
        }

        #endregion
    }
}
