using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace DevicesCommon
{
    /// <summary>
    /// Интерфейс устройства, предназначенного для чтения данных карт доступа,
    /// штрихкодов и т.п.
    /// </summary>
    public interface IGenericReader : ISerialDevice
	{
        #region Свойства

		/// <summary>
		/// Контроль четности
		/// </summary>
		Parity Parity { get; set; }

        /// <summary>
        /// Стоп-символ
        /// </summary>
        byte StopChar { get; set; }

        /// <summary>
        /// Очередной блок данных
        /// </summary>
        string Data { get; }

        /// <summary>
        /// Состояние очереди данных
        /// </summary>
        bool Empty { get; set; }

        #endregion
    }
}
