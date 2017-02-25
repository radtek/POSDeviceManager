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
        String sessionID;

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="device">��������� ����������</param>
        public DeviceHelper(IDevice device)
        {
            this.device = device;
            sessionID = String.Empty;
        }

        /// <summary>
        /// ���������, ������������� �� ����������
        /// </summary>
        public bool Captured
        {
            get
            {
                return !String.IsNullOrEmpty(sessionID);
            }
        }

        /// <summary>
        /// �������������� ���������� ������,
        /// ��������������� ����������
        /// </summary>
        public String SessionID
        {
            get
            {
                return sessionID;
            }

            set
            {
                if (Captured)
                    throw new DeviceManagerException(
                        String.Format("���������� �������������, ������������� ������ {0}",
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
            sessionID = String.Empty;
        }
    }
}
