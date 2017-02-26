using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon;
using DevicesCommon.Helpers;

namespace DevicesBase.Helpers
{
    /// <summary>
    /// ��������������� ����� ��� ������ � �������� ���������
    /// </summary>
    internal class DeviceHelper
    {
        IDevice device;
        string sessionID;

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="device">��������� ����������</param>
        public DeviceHelper(IDevice device)
        {
            this.device = device;
            sessionID = string.Empty;
        }

        /// <summary>
        /// ���������, ������������� �� ����������
        /// </summary>
        public bool Captured
        {
            get
            {
                return !string.IsNullOrEmpty(sessionID);
            }
        }

        /// <summary>
        /// �������������� ���������� ������,
        /// ��������������� ����������
        /// </summary>
        public string SessionID
        {
            get
            {
                return sessionID;
            }

            set
            {
                if (Captured)
                    throw new DeviceManagerException(
                        string.Format("���������� �������������, ������������� ������ {0}",
                        sessionID));

                // ���������� ����������
                sessionID = value;
            }
        }

        /// <summary>
        /// ��������� ����������
        /// </summary>
        public IDevice Device
        {
            get
            {
                return device;
            }
        }

        /// <summary>
        /// ������������ ����������
        /// </summary>
        public void Release()
        {
            sessionID = string.Empty;
        }
    }
}
