using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace DevicesBase.Helpers
{
    /// <summary>
    /// ��������������� ����� ��� ����������� ���� ������
    /// </summary>
    internal class SerialPortsHelper : IDisposable
    {
        private String _deviceId;
        private EasyCommunicationPort _port;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="portName">��� �����</param>
        public SerialPortsHelper(String portName)
        {
            _port = new EasyCommunicationPort();
            _port.PortName = portName;
            _deviceId = String.Empty;
        }

        /// <summary>
        /// ��� �����
        /// </summary>
        public String PortName
        {
            get { return _port.PortName; }
        }

        /// <summary>
        /// ������������� ����������
        /// </summary>
        public String DeviceId
        {
            get { return _deviceId; }
            set { _deviceId = value; }
        }

        /// <summary>
        /// ���������������� ����
        /// </summary>
        public EasyCommunicationPort Port
        {
            get { return _port; }
        }

        #region IDisposable Members

        /// <summary>
        /// ������������ ��������
        /// </summary>
        public void Dispose()
        {
            if (_port != null)
            {
                _port.Dispose();
                _port = null;
            }
        }

        #endregion
    }
}
