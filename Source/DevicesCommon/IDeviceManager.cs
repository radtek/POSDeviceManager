namespace DevicesCommon
{
    /// <summary>
    /// Инерфейс диспетчера устройств
    /// </summary>
    public interface IDeviceManager
	{
		/// <summary>
		/// Возвращает ссылку на базовый интерфейс всех устройств
		/// </summary>
		/// <param name="sessionId">Идентификатор сессии</param>
		/// <param name="deviceId">Идентификатор устройства</param>
		IDevice GetDevice(string sessionId, string deviceId);

		/// <summary>
		/// Получить монопольный доступ к устройству
		/// </summary>
		/// <param name="sessionId">Идентификатор сессии</param>
		/// <param name="deviceId">Идентификатор устройства</param>
        /// <param name="waitTimeout">Таймаут ожидания захвата устройства, секунды</param>
		bool Capture(string sessionId, string deviceId, int waitTimeout);

		/// <summary>
		/// Освободить устройство
		/// </summary>
		/// <param name="sessionId">Идентификатор сессии</param>
		/// <param name="deviceId">Идентификатор устройства</param>
		bool Release(string sessionId, string deviceId);

		/// <summary>
		/// Регистрация клиентской сессии в диспетчере устройств
		/// </summary>
		/// <param name="sessionId">Идентификатор сессии</param>
		void Login(out string sessionId);

		/// <summary>
		/// Завершение клиентской сессии в диспетчере устройств
		/// </summary>
		/// <param name="sessionId">Идентификатор сессии</param>
		void Logout(string sessionId);
	}
}
