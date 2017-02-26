using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// Интерфей для подключения к источнику событий
    /// </summary>
    public interface IEventSourceConnector
    {
        /// <summary>
        /// Открывает подключение к источнику событий
        /// </summary>
        void OpenConnector();

        /// <summary>
        /// Закрывает подключение к источнику событий
        /// </summary>
        void CloseConnector();

        /// <summary>
        /// Источник событий
        /// </summary>
        IEventLinkBasics Source { get; }

        /// <summary>
        /// Уведомление о процессе загрузки событий
        /// </summary>
        /// <param name="eventsLoaded">Загруженное число событий</param>
        void ReloadProgress(int eventsLoaded);
    }
}
