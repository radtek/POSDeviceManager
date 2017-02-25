using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// Запись лога
    /// </summary>
    [Serializable]
    public class EventRecord
    {
        #region Поля и константы

        private String _id;
        private DateTime _timestamp;
        private String _source;
        private EventType _eventType;
        private List<String> _text;

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
        public EventRecord(String id, DateTime timestamp, String source,
            EventType eventType, String[] text)
        {
            _id = id;
            _timestamp = timestamp;
            _source = source;
            _eventType = eventType;
            _text = new List<String>(text);
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
        public String Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Приложение-источник события
        /// </summary>
        public String Source
        {
            get { return _source; }
        }

        /// <summary>
        /// Текст события
        /// </summary>
        public List<String> Text
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
