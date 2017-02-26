using System;
using System.Collections.Generic;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// Запись лога
    /// </summary>
    [Serializable]
    public class EventRecord
    {
        #region Поля и константы

        private string _id;
        private DateTime _timestamp;
        private string _source;
        private EventType _eventType;
        private List<string> _text;

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="id">Идентификатор события</param>
        /// <param name="timestamp">Дата и время события</param>
        /// <param name="source">Источник события</param>
        /// <param name="eventType">Тип события</param>
        /// <param name="text">Текст события</param>
        public EventRecord(string id, DateTime timestamp, string source,
            EventType eventType, string[] text)
        {
            _id = id;
            _timestamp = timestamp;
            _source = source;
            _eventType = eventType;
            _text = new List<string>(text);
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Тип события
        /// </summary>
        public EventType EventType
        {
            get { return _eventType; }
        }

        /// <summary>
        /// Идентификатор события
        /// </summary>
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Приложение-источник события
        /// </summary>
        public string Source
        {
            get { return _source; }
        }

        /// <summary>
        /// Текст события
        /// </summary>
        public List<string> Text
        {
            get { return _text; }
        }

        /// <summary>
        /// Дата и время события
        /// </summary>
        public DateTime Timestamp
        {
            get { return _timestamp; }
        }

        #endregion
    }
}
