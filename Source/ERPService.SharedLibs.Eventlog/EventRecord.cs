using System;
using System.Collections.Generic;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// ������ ����
    /// </summary>
    [Serializable]
    public class EventRecord
    {
        #region ���� � ���������

        private string _id;
        private DateTime _timestamp;
        private string _source;
        private EventType _eventType;
        private List<string> _text;

        #endregion

        #region �����������

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="id">������������� �������</param>
        /// <param name="timestamp">���� � ����� �������</param>
        /// <param name="source">�������� �������</param>
        /// <param name="eventType">��� �������</param>
        /// <param name="text">����� �������</param>
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

        #region ��������

        /// <summary>
        /// ��� �������
        /// </summary>
        public EventType EventType
        {
            get { return _eventType; }
        }

        /// <summary>
        /// ������������� �������
        /// </summary>
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// ����������-�������� �������
        /// </summary>
        public string Source
        {
            get { return _source; }
        }

        /// <summary>
        /// ����� �������
        /// </summary>
        public List<string> Text
        {
            get { return _text; }
        }

        /// <summary>
        /// ���� � ����� �������
        /// </summary>
        public DateTime Timestamp
        {
            get { return _timestamp; }
        }

        #endregion
    }
}
