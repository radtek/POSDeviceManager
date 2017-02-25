using System;
using System.Collections.Generic;
using System.Text;
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
		Byte StopChar { get; set; }

        /// <summary>
        /// Очередной блок данных
        /// </summary>
        String Data { get; }

        /// <summary>
        /// Состояние очереди данных
        /// </summary>
        Boolean Empty { get; set; }

        #endregion
    }
}
