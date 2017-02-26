using DevicesCommon.Helpers;

namespace DevicesCommon
{
    /// <summary>
    /// Базовый интерфейс для устройств
    /// </summary>
    public interface IDevice
	{
		#region Общие для всех устройств свойства

		/// <summary>
		/// Возвращает код ошибки
		/// </summary>
        ErrorCode ErrorCode { get; }

		/// <summary>
		/// Подключение и отключение устройства
		/// </summary>
		bool Active { get; set; }

        /// <summary>
        /// Идентификатор устройства
        /// </summary>
        string DeviceId { get; set; }

        /// <summary>
        /// Интерфейс для протоколирования работы устройства
        /// </summary>
        ILogger Logger { get; set; }

		#endregion
	}
}
