using System;

namespace DevicesBase.Helpers
{
    /// <summary>
    /// ��������������� ����� ��� ������ � ����������� ��������
    /// </summary>
    internal class SessionHelper
    {
        private DateTime accessDateTime;
        private string sessionID;
        private int sessionTimeout;

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="sessionID">������������� ������</param>
        /// <param name="sessionTimeout">������� ������</param>
        public SessionHelper(string sessionID, int sessionTimeout)
        {
            this.sessionID = sessionID;
            this.sessionTimeout = sessionTimeout;
            accessDateTime = DateTime.Now;
        }

        /// <summary>
        /// ���� � ����� ���������� ��������� �������
        /// </summary>
        public DateTime AccessDateTime
        {
            get
            {
                return accessDateTime;
            }
            set
            {
                accessDateTime = value;
            }
        }

        /// <summary>
        /// ������������� �����
        /// </summary>
        public string SessionID
        {
            get
            {
                return sessionID;
            }
        }

        /// <summary>
        /// ������� ������������ ���������� ������
        /// </summary>
        public bool Alive
        {
            get
            {
                if (sessionTimeout > 0)
                    // ������� ������ �����
                    return DateTime.Now.Subtract(accessDateTime).Seconds < sessionTimeout;
                else
                    // ������� ������ - �����������
                    return true;
            }
        }
    }
}
