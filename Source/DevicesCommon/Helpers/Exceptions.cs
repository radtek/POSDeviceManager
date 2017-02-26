using System;
using System.Runtime.Serialization;
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
        private const string _commException = "Ошибка связи с устройством: {0}.";
        private string _reason;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="reason">Причина возникновения ошибки</param>
        /// <param name="innerException">Внутреннее исключение</param>
        public CommunicationException(string reason, Exception innerException)
            : base(string.Format(_commException, reason), innerException)
        {
            _reason = reason;
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="reason">Причина возникновения ошибки</param>
        public CommunicationException(string reason)
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
        public string Reason
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
		private const string deviceNotFound = "Устройство с идентификатором {0} не найдено на сервере {1}";
		private const string deviceNotFound2 = "Устройство с идентификатором {0} не найдено";
		private string deviceId;
		private string serverName;

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="deviceId">Идентификатор устройства</param>
		/// <param name="serverName">Имя сервера</param>
		public DeviceNoFoundException(string deviceId, string serverName): 
			base(string.Format(deviceNotFound, deviceId, serverName))
		{
			this.deviceId = deviceId;
			this.serverName = serverName;
		}

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="deviceId">Идентификатор устройства</param>
		public DeviceNoFoundException(string deviceId):
			base(string.Format(deviceNotFound2, deviceId))
		{
			this.deviceId = deviceId;
			this.serverName = string.Empty;
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
		public string DeviceId
		{
			get
			{
				return deviceId;
			}
		}

		/// <summary>
		/// Имя сервера
		/// </summary>
		public string ServerName
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
		public DeviceManagerConfigException(string message) : base(message)
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
		public DeviceManagerException(string message): base(message)
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
