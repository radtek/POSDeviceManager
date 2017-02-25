using System;
using System.Collections.Generic;
using System.Text;

namespace DevicesCommon
{
	/// <summary>
	/// Интерфейс для дисплеев покупателя
	/// </summary>
	public interface ICustomerDisplay : ISerialDevice
	{
#region Свойства

		/// <summary>
		/// Вывод строк на дисплей
		/// </summary>
		/// <param name="lineNumber">Номер строки</param>
		String this[Int32 lineNumber] { set; }

#endregion

#region Методы

		/// <summary>
		/// Запись текущей информации во флэш-память дисплея
		/// </summary>
		void SaveToEEPROM();

#endregion
	}
}
