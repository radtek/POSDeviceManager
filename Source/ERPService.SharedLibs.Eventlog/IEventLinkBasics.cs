using System;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// Интерфейс, декларирующий основную функциональность при работе с журналом событий
    /// </summary>
    public interface IEventLinkBasics
    {
        /// <summary>
        /// Загрузка журнала
        /// </summary>
        /// <param name="fromDate">Фильтр по дате-времени, начальное значение</param>
        /// <param name="toDate">Фильтр по дате-времени, конечное значение</param>
        /// <param name="sourceFilter">Фильтр по источнику событий</param>
        /// <param name="eventFilter">Фильтр по типам событий</param>
        EventRecord[] GetLog(DateTime fromDate, DateTime toDate,
            string[] sourceFilter, EventType[] eventFilter);

        /// <summary>
        /// Загрузка журнала
        /// </summary>
        /// <param name="fromDate">Фильтр по дате-времени, начальное значение</param>
        /// <param name="toDate">Фильтр по дате-времени, конечное значение</param>
        /// <param name="sourceFilter">Фильтр по источнику событий</param>
        /// <param name="eventFilter">Фильтр по типам событий</param>
        /// <param name="maxEvents">Максимальное количество событий</param>
        EventRecord[] GetLog(DateTime fromDate, DateTime toDate,
            string[] sourceFilter, EventType[] eventFilter, int maxEvents);

        /// <summary>
        /// Принудительная очистка журнала
        /// </summary>
        /// <param name="fromDate">Фильтр по дате-времени, начальное значение</param>
        /// <param name="toDate">Фильтр по дате-времени, конечное значение</param>
        void TruncLog(DateTime fromDate, DateTime toDate);

        #region Последовательный доступ к событиям

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
        string BeginGetLog(DateTime fromDate, DateTime toDate, string[] sourceFilter, 
            EventType[] eventFilter, int maxEvents, int eventPerIteration);

        /// <summary>
        /// Загрузить очередной блок событий из журнала
        /// </summary>
        /// <param name="iteratorId">Идентификатор итератора</param>
        /// <returns>
        /// Очередной блок событий из журнала или null, если достигнут конец журнала
        /// </returns>
        EventRecord[] GetLog(string iteratorId);

        /// <summary>
        /// Завершить последовательный доступ к событиям 
        /// </summary>
        /// <param name="iteratorId">Идентификатор итератора</param>
        void EndGetLog(string iteratorId);

        #endregion
    }
}
