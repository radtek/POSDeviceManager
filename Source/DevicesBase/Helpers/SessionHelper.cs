using System;
using System.Collections.Generic;
using System.Text;

namespace DevicesBase.Helpers
{
    /// <summary>
    /// ��������������� ����� ��� ������ � ����������� ��������
    /// </summary>
    internal class SessionHelper
    {
        private DateTime accessDateTime;
        private String sessionID;
        private Int32 sessionTimeout;

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="sessionID">������������� ������</param>
        /// <param name="sessionTimeout">������� ������</param>
        public SessionHelper(String sessionID, Int32 sessionTimeout)
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
        public String SessionID
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
