using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace DevicesCommon.Helpers
{
	#region Исключения диспетчера устройств

    /// <summary>
    /// Исключение коммуникатора
    /// </summary>
    [Serializable]
    public class CommunicationException : Exception, ISerializable
    {
        private const String _commException = "Ошибка связи с устройством: {0}.";
        private String _reason;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="reason">Причина возникновения ошибки</param>
        /// <param name="innerException">Внутреннее исключение</param>
        public CommunicationException(String reason, Exception innerException)
            : base(String.Format(_commException, reason), innerException)
        {
            _reason = reason;
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="reason">Причина возникновения ошибки</param>
        public CommunicationException(String reason)
            : this(reason, null)
        {
        }

        /// <summary>
        /// Конструктор для сериализации
        /// </summary>
        /// <param name="info">Информация о сериализации</param>
        /// <param name="context">Контекст сериализации</param>
        protected CommunicationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _reason = info.GetString("_reason");
        }

        /// <summary>
        /// Причина возникновения исключения
        /// </summary>
        public String Reason
        {
            get { return _reason; }
        }

        #region Реализация ISerializable

        /// <summary>
        /// Сериализация объектов класса
        /// </summary>
        /// <param name="info">Класс <see cref="SerializationInfo"/>
        /// для информации о сериализации</param>
        /// <param name="context">Контекст сериализации</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_reason", _reason);
            base.GetObjectData(info, context);
        }

        #endregion
    }

	/// <summary>
	/// Исключение при попытке обращения к несуществующему устройству
	/// </summary>
	[Serializable]
	public class DeviceNoFoundException: System.Exception, ISerializable
	{
		private const String deviceNotFound = "Устройство с идентификатором {0} не найдено на сервере {1}";
		private const String deviceNotFound2 = "Устройство с идентификатором {0} не найдено";
		private String deviceId;
		private String serverName;

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="deviceId">Идентификатор устройства</param>
		/// <param name="serverName">Имя сервера</param>
		public DeviceNoFoundException(String deviceId, String serverName): 
			base(String.Format(deviceNotFound, deviceId, serverName))
		{
			this.deviceId = deviceId;
			this.serverName = serverName;
		}

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="deviceId">Идентификатор устройства</param>
		public DeviceNoFoundException(String deviceId):
			base(String.Format(deviceNotFound2, deviceId))
		{
			this.deviceId = deviceId;
			this.serverName = String.Empty;
		}

		/// <summary>
		/// Конструктор для сериализации
		/// </summary>
		/// <param name="info">Информация о сериализации</param>
		/// <param name="context">Контекст сериализации</param>
		protected DeviceNoFoundException(SerializationInfo info, StreamingContext context):
			base(info, context)
		{
			deviceId = info.GetString("deviceId");
			serverName = info.GetString("serverName");
		}

		/// <summary>
		/// Идентификатор устройства
		/// </summary>
		public String DeviceId
		{
			get
			{
				return deviceId;
			}
		}

		/// <summary>
		/// Имя сервера
		/// </summary>
		public String ServerName
		{
			get
			{
				return serverName;
			}
		}

		/// <summary>
		/// Сериализация объектов класса
		/// </summary>
		/// <param name="info">Класс <see cref="SerializationInfo"/> для информации о сериализации</param>
		/// <param name="context">Контекст сериализации</param>
		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("deviceId", deviceId);
			info.AddValue("serverName", serverName);
			base.GetObjectData(info, context);
		}
	}

	/// <summary>
	/// Исключение при попытке обратится к диспетчеру устройств без 
	/// создания клиентской сессии
	/// </summary>
	[Serializable]
	public class LoginToDeviceManagerException : System.Exception
	{
		/// <summary>
		/// Конструктор
		/// </summary>
		public LoginToDeviceManagerException() : base("Создайте сессию в диспетчере устройств")
		{
		}

		/// <summary>
		/// Конструктор для сериализации
		/// </summary>
		/// <param name="info">Информация о сериализации</param>
		/// <param name="context">Контекст сериализации</param>
		protected LoginToDeviceManagerException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}

	/// <summary>
	/// Ошибка во время загрузки конфигурации диспетчера устройств
	/// </summary>
	[Serializable]
	public class DeviceManagerConfigException : System.Exception
	{
		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="message">Текст сообщения об ошибке</param>
		public DeviceManagerConfigException(String message) : base(message)
		{
		}

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="info">Информация о сериализации</param>
		/// <param name="context">Контекст сериализации</param>
		protected DeviceManagerConfigException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}

	/// <summary>
	/// Общее исключение диспетчера устройств
	/// </summary>
	[Serializable]
	public class DeviceManagerException : System.Exception
	{
		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="message">Текст сообщения об ошибке</param>
		public DeviceManagerException(String message): base(message)
		{
		}

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="info">Информация о сериализации</param>
		/// <param name="context">Контекст сериализации</param>
		protected DeviceManagerException(SerializationInfo info, StreamingContext context): 
			base(info, context)
		{
		}
	}

	#endregion
}
