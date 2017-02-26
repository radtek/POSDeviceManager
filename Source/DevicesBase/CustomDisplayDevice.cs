using DevicesCommon;

namespace DevicesBase
{
    /// <summary>
    /// Базовый класс для дисплеев покупателя
    /// </summary>
    public abstract class CustomDisplayDevice : CustomSerialDevice, ICustomerDisplay
	{
		#region Конструктор

		/// <summary>
		/// Создает дисплей покупателя
		/// </summary>
		protected CustomDisplayDevice() : base()
		{
		}

		#endregion

		#region Реализация ICustomerDisplay

		/// <summary>
		/// Запись текущей информации во флэш-память дисплея
		/// </summary>
		public abstract void SaveToEEPROM();

		/// <summary>
		/// Вывод строк на дисплей
		/// </summary>
		/// <param name="lineNumber">Номер строки</param>
		public abstract string this[int lineNumber] { set; }

		#endregion
}
}
