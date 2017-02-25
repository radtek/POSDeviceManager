using System;
using System.Collections.Generic;
using System.Text;
using ERPService.SharedLibs.Eventlog;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// Интерфейс для связи окна детального просмотра события
    /// и списка событий
    /// </summary>
    public interface IEventsViewLink
    {
        /// <summary>
        /// Перемещение к следующему событию
        /// </summary>
        EventRecord NextEvent();

        /// <summary>
        /// Перемещение к предыдущему событию
        /// </summary>
        EventRecord PreviousEvent();

        /// <summary>
        /// Текущее событие
        /// </summary>
        EventRecord Current { get; }
    }
}
