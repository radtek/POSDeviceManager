using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// ������ ����
    /// </summary>
    [Serializable]
    public class EventRecord
    {
        #region ���� � ���������

        private String _id;
        private DateTime _timestamp;
        private String _source;
        private EventType _eventType;
        private List<String> _text;

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
        public String Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// ����������-�������� �������
        /// </summary>
        public String Source
        {
            get { return _source; }
        }

        /// <summary>
        /// ����� �������
        /// </summary>
        public List<String> Text
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
