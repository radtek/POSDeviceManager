using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// Интерфейс для публикации событий
    /// </summary>
    public interface IEventLink : IEventLinkBasics
    {
        /// <summary>
        /// Публикация события
        /// </summary>
        /// <param name="sourceId">Идентификатор источника событий</param>
        /// <param name="eventType">Тип события</param>
        /// <param name="text">Текстовые данные события</param>
        void Post(string sourceId, EventType eventType, string[] text);

        /// <summary>
        /// Публикация информационного события 
        /// </summary>
        /// <param name="sourceId">Идентификатор источника событий</param>
        /// <param name="text">Текстовые данные события</param>
        void Post(string sourceId, string[] text);

        /// <summary>
        /// Публикация события
        /// </summary>
        /// <param name="sourceId">Идентификатор источника событий</param>
        /// <param name="eventType">Тип события</param>
        /// <param name="text">Текстовые данные события</param>
        void Post(string sourceId, EventType eventType, string text);

        /// <summary>
        /// Публикация информационного события
        /// </summary>
        /// <param name="sourceId">Идентификатор источника событий</param>
        /// <param name="text">Текстовые данные события</param>
        void Post(string sourceId, string text);

        /// <summary>
        /// Публикация информации об исключении
        /// </summary>
        /// <param name="sourceId">Идентификатор источника событий</param>
        /// <param name="checkPointId">Название контрольной точки</param>
        /// <param name="e">Исключение</param>
        void Post(string sourceId, string checkPointId, Exception e);

        /// <summary>
        /// Публикация информации об исключении
        /// </summary>
        /// <param name="sourceId">Идентификатор источника событий</param>
        /// <param name="exceptionData">Данные исключения, подготовленные для публикации</param>
        void PostException(string sourceId, string[] exceptionData);
    }
}
