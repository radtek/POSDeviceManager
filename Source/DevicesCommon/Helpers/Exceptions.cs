using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Security;
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
        private const String _commException = "������ ����� � �����������: {0}.";
        private String _reason;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="reason">������� ������������� ������</param>
        /// <param name="innerException">���������� ����������</param>
        public CommunicationException(String reason, Exception innerException)
            : base(String.Format(_commException, reason), innerException)
        {
            _reason = reason;
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="reason">������� ������������� ������</param>
        public CommunicationException(String reason)
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
        public String Reason
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
		private const String deviceNotFound = "���������� � ��������������� {0} �� ������� �� ������� {1}";
		private const String deviceNotFound2 = "���������� � ��������������� {0} �� �������";
		private String deviceId;
		private String serverName;

		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="deviceId">������������� ����������</param>
		/// <param name="serverName">��� �������</param>
		public DeviceNoFoundException(String deviceId, String serverName): 
			base(String.Format(deviceNotFound, deviceId, serverName))
		{
			this.deviceId = deviceId;
			this.serverName = serverName;
		}

		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="deviceId">������������� ����������</param>
		public DeviceNoFoundException(String deviceId):
			base(String.Format(deviceNotFound2, deviceId))
		{
			this.deviceId = deviceId;
			this.serverName = String.Empty;
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
		public String DeviceId
		{
			get
			{
				return deviceId;
			}
		}

		/// <summary>
		/// ��� �������
		/// </summary>
		public String ServerName
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
		public DeviceManagerConfigException(String message) : base(message)
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
		public DeviceManagerException(String message): base(message)
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
