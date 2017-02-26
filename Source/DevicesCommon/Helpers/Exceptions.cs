using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace DevicesCommon.Helpers
{
    #region ���������� ���������� ���������

    /// <summary>
    /// ���������� �������������
    /// </summary>
    [Serializable]
    public class CommunicationException : Exception, ISerializable
    {
        private const string _commException = "������ ����� � �����������: {0}.";
        private string _reason;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="reason">������� ������������� ������</param>
        /// <param name="innerException">���������� ����������</param>
        public CommunicationException(string reason, Exception innerException)
            : base(string.Format(_commException, reason), innerException)
        {
            _reason = reason;
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="reason">������� ������������� ������</param>
        public CommunicationException(string reason)
            : this(reason, null)
        {
        }

        /// <summary>
        /// ����������� ��� ������������
        /// </summary>
        /// <param name="info">���������� � ������������</param>
        /// <param name="context">�������� ������������</param>
        protected CommunicationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _reason = info.GetString("_reason");
        }

        /// <summary>
        /// ������� ������������� ����������
        /// </summary>
        public string Reason
        {
            get { return _reason; }
        }

        #region ���������� ISerializable

        /// <summary>
        /// ������������ �������� ������
        /// </summary>
        /// <param name="info">����� <see cref="SerializationInfo"/>
        /// ��� ���������� � ������������</param>
        /// <param name="context">�������� ������������</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_reason", _reason);
            base.GetObjectData(info, context);
        }

        #endregion
    }

	/// <summary>
	/// ���������� ��� ������� ��������� � ��������������� ����������
	/// </summary>
	[Serializable]
	public class DeviceNoFoundException: System.Exception, ISerializable
	{
		private const string deviceNotFound = "���������� � ��������������� {0} �� ������� �� ������� {1}";
		private const string deviceNotFound2 = "���������� � ��������������� {0} �� �������";
		private string deviceId;
		private string serverName;

		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="deviceId">������������� ����������</param>
		/// <param name="serverName">��� �������</param>
		public DeviceNoFoundException(string deviceId, string serverName): 
			base(string.Format(deviceNotFound, deviceId, serverName))
		{
			this.deviceId = deviceId;
			this.serverName = serverName;
		}

		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="deviceId">������������� ����������</param>
		public DeviceNoFoundException(string deviceId):
			base(string.Format(deviceNotFound2, deviceId))
		{
			this.deviceId = deviceId;
			this.serverName = string.Empty;
		}

		/// <summary>
		/// ����������� ��� ������������
		/// </summary>
		/// <param name="info">���������� � ������������</param>
		/// <param name="context">�������� ������������</param>
		protected DeviceNoFoundException(SerializationInfo info, StreamingContext context):
			base(info, context)
		{
			deviceId = info.GetString("deviceId");
			serverName = info.GetString("serverName");
		}

		/// <summary>
		/// ������������� ����������
		/// </summary>
		public string DeviceId
		{
			get
			{
				return deviceId;
			}
		}

		/// <summary>
		/// ��� �������
		/// </summary>
		public string ServerName
		{
			get
			{
				return serverName;
			}
		}

		/// <summary>
		/// ������������ �������� ������
		/// </summary>
		/// <param name="info">����� <see cref="SerializationInfo"/> ��� ���������� � ������������</param>
		/// <param name="context">�������� ������������</param>
		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("deviceId", deviceId);
			info.AddValue("serverName", serverName);
			base.GetObjectData(info, context);
		}
	}

	/// <summary>
	/// ���������� ��� ������� ��������� � ���������� ��������� ��� 
	/// �������� ���������� ������
	/// </summary>
	[Serializable]
	public class LoginToDeviceManagerException : System.Exception
	{
		/// <summary>
		/// �����������
		/// </summary>
		public LoginToDeviceManagerException() : base("�������� ������ � ���������� ���������")
		{
		}

		/// <summary>
		/// ����������� ��� ������������
		/// </summary>
		/// <param name="info">���������� � ������������</param>
		/// <param name="context">�������� ������������</param>
		protected LoginToDeviceManagerException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}

	/// <summary>
	/// ������ �� ����� �������� ������������ ���������� ���������
	/// </summary>
	[Serializable]
	public class DeviceManagerConfigException : System.Exception
	{
		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="message">����� ��������� �� ������</param>
		public DeviceManagerConfigException(string message) : base(message)
		{
		}

		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="info">���������� � ������������</param>
		/// <param name="context">�������� ������������</param>
		protected DeviceManagerConfigException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}

	/// <summary>
	/// ����� ���������� ���������� ���������
	/// </summary>
	[Serializable]
	public class DeviceManagerException : System.Exception
	{
		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="message">����� ��������� �� ������</param>
		public DeviceManagerException(string message): base(message)
		{
		}

		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="info">���������� � ������������</param>
		/// <param name="context">�������� ������������</param>
		protected DeviceManagerException(SerializationInfo info, StreamingContext context): 
			base(info, context)
		{
		}
	}

	#endregion
}
