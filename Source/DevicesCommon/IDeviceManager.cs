using System;
using System.Collections.Generic;
using System.Text;

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
		IDevice GetDevice(String sessionId, String deviceId);

		/// <summary>
		/// Получить монопольный доступ к устройству
		/// </summary>
		/// <param name="sessionId">Идентификатор сессии</param>
		/// <param name="deviceId">Идентификатор устройства</param>
        /// <param name="waitTimeout">Таймаут ожидания захвата устройства, секунды</param>
		bool Capture(String sessionId, String deviceId, Int32 waitTimeout);

		/// <summary>
		/// Освободить устройство
		/// </summary>
		/// <param name="sessionId">Идентификатор сессии</param>
		/// <param name="deviceId">Идентификатор устройства</param>
		bool Release(String sessionId, String deviceId);

		/// <summary>
		/// Регистрация клиентской сессии в диспетчере устройств
		/// </summary>
		/// <param name="sessionId">Идентификатор сессии</param>
		void Login(out String sessionId);

		/// <summary>
		/// Завершение клиентской сессии в диспетчере устройств
		/// </summary>
		/// <param name="sessionId">Идентификатор сессии</param>
		void Logout(String sessionId);
	}
}
