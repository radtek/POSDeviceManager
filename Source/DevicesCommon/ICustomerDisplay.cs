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
        string this[int lineNumber] { set; }

#endregion

#region Методы

		/// <summary>
		/// Запись текущей информации во флэш-память дисплея
		/// </summary>
		void SaveToEEPROM();

#endregion
	}
}
