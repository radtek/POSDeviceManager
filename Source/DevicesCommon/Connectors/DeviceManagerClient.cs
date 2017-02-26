using System.Net.Sockets;
using System.Runtime.Remoting;
using DevicesCommon.Helpers;
using ERPService.SharedLibs.Remoting.Connectors;

namespace DevicesCommon.Connectors
{
    /// <summary>
    /// Вспомогательный класс констант времени ожидания
    /// </summary>
    public sealed class WaitConstant
    {
        /// <summary>
        /// Не ожидать освобождения устройства
        /// </summary>
        public const int None = 0;

        /// <summary>
        /// Ожидать освобождения устройства бесконечно
        /// </summary>
        public const int Infinite = -1;
    }

	/// <summary>
	/// Клиент диспетчера устройств
	/// </summary>
    public sealed class DeviceManagerClient : TcpBinaryConnector<IDeviceManager>
	{
        #region Поля

		// идентификатор сессии
		private string _sessionId;

        #endregion

        #region Конструкторы

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <remarks>Порт для подключения инициализируется значением по умолчанию - 35100</remarks>
        /// <param name="serverName">Имя сервера</param>
        public DeviceManagerClient(string serverName)
            : this(serverName, 35100)
        {
            _sessionId = string.Empty;
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="serverName">Имя сервера</param>
        /// <param name="port">Порт сервера</param>
        public DeviceManagerClient(string serverName, int port)
            : base(serverName, port, "devicemanager")
        {
            _sessionId = string.Empty;
        }

        #endregion

        #region Прочие методы

		/// <summary>
		/// Возвращает признак регистрации сессии в диспетчере устройств
		/// </summary>
		public bool Logged
		{
			get
			{
				return RemoteObject != null && !string.IsNullOrEmpty(_sessionId);
			}
		}

        /// <summary>
        /// Освобождение ресурсов объекта
        /// </summary>
        public override void Dispose()
        {
            // завершаем сессию
            try
            {
                Logout();
            }
            catch (SocketException)
            {
                // подавляем возможные исключения, связанные с остановкой 
                // ремоутинг-сервера
            }
            catch (RemotingException)
            {
            }
            base.Dispose();
        }

        #endregion

        #region Методы-обертки над интерфейсом IDeviceManager
		
		/// <summary>
		/// Регистрация сесии в диспетчере устройств
		/// </summary>
	 	public void Login()
		{
            RemoteObject.Login(out _sessionId);
		}

		/// <summary>
		/// Завершение сессии в диспетчере устройств
		/// </summary>
		public void Logout()
		{
            if (Logged)
            {
                RemoteObject.Logout(_sessionId);
                _sessionId = string.Empty;
            }
		}

        /// <summary>
        /// Получить монопольный доступ к устройству
        /// </summary>
        /// <param name="deviceId">Идентификатор устройства</param>
        /// <param name="waitTimeout">Таймаут ожидания захвата устройства, секунды<see cref="WaitConstant"/>></param>
        public bool Capture(string deviceId, int waitTimeout)
        {
            return RemoteObject.Capture(_sessionId, deviceId, waitTimeout);
        }

        /// <summary>
		/// Освободить устройство
		/// </summary>
		/// <param name="deviceId">Идентификатор устройства</param>
		public bool Release(string deviceId)
		{
            return RemoteObject.Release(_sessionId, deviceId);
		}

		/// <summary>
		/// Интерфейс устройства
		/// </summary>
		/// <param name="deviceId">Идентификатор устройства</param>
		public IDevice this[string deviceId]
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
