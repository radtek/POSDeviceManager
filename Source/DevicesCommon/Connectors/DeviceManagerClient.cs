using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Runtime.Remoting;
using DevicesCommon.Helpers;
using ERPService.SharedLibs.Remoting.Connectors;

namespace DevicesCommon.Connectors
{
    /// <summary>
    /// ��������������� ����� �������� ������� ��������
    /// </summary>
    public sealed class WaitConstant
    {
        /// <summary>
        /// �� ������� ������������ ����������
        /// </summary>
        public const Int32 None = 0;

        /// <summary>
        /// ������� ������������ ���������� ����������
        /// </summary>
        public const Int32 Infinite = -1;
    }

	/// <summary>
	/// ������ ���������� ���������
	/// </summary>
    public sealed class DeviceManagerClient : TcpBinaryConnector<IDeviceManager>
	{
        #region ����

		// ������������� ������
		private String _sessionId;

        #endregion

        #region ������������

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <remarks>���� ��� ����������� ���������������� ��������� �� ��������� - 35100</remarks>
        /// <param name="serverName">��� �������</param>
        public DeviceManagerClient(String serverName)
            : this(serverName, 35100)
        {
            _sessionId = String.Empty;
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="serverName">��� �������</param>
        /// <param name="port">���� �������</param>
        public DeviceManagerClient(String serverName, Int32 port)
            : base(serverName, port, "devicemanager")
        {
            _sessionId = String.Empty;
        }

        #endregion

        #region ������ ������

		/// <summary>
		/// ���������� ������� ����������� ������ � ���������� ���������
		/// </summary>
		public bool Logged
		{
			get
			{
				return RemoteObject != null && !String.IsNullOrEmpty(_sessionId);
			}
		}

        /// <summary>
        /// ������������ �������� �������
        /// </summary>
        public override void Dispose()
        {
            // ��������� ������
            try
            {
                Logout();
            }
            catch (SocketException)
            {
                // ��������� ��������� ����������, ��������� � ���������� 
                // ���������-�������
            }
            catch (RemotingException)
            {
            }
            base.Dispose();
        }

        #endregion

        #region ������-������� ��� ����������� IDeviceManager
		
		/// <summary>
		/// ����������� ����� � ���������� ���������
		/// </summary>
	 	public void Login()
		{
            RemoteObject.Login(out _sessionId);
		}

		/// <summary>
		/// ���������� ������ � ���������� ���������
		/// </summary>
		public void Logout()
		{
            if (Logged)
            {
                RemoteObject.Logout(_sessionId);
                _sessionId = String.Empty;
            }
		}

        /// <summary>
        /// �������� ����������� ������ � ����������
        /// </summary>
        /// <param name="deviceId">������������� ����������</param>
        /// <param name="waitTimeout">������� �������� ������� ����������, �������<see cref="WaitConstant"/>></param>
        public bool Capture(String deviceId, Int32 waitTimeout)
        {
            return RemoteObject.Capture(_sessionId, deviceId, waitTimeout);
        }

        /// <summary>
		/// ���������� ����������
		/// </summary>
		/// <param name="deviceId">������������� ����������</param>
		public bool Release(String deviceId)
		{
            return RemoteObject.Release(_sessionId, deviceId);
		}

		/// <summary>
		/// ��������� ����������
		/// </summary>
		/// <param name="deviceId">������������� ����������</param>
		public IDevice this[String deviceId]
		{
			get
			{
                IDevice _device = RemoteObject.GetDevice(_sessionId, deviceId);
				if (_device == null)
					throw new DeviceNoFoundException(deviceId, ServerNameOrIp);
				return _device;
			}
		}

        #endregion
    }
}
