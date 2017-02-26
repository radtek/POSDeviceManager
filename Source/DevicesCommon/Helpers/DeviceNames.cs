namespace DevicesCommon.Helpers
{
    /// <summary>
    /// Вспомогательный класс, хранит константы типов устройств
    /// </summary>
    public static class DeviceNames
	{
		#region Фискальные регистраторы

		/// <summary>
		/// ФР производства Штрих-М
		/// </summary>
		public const string ecrTypeStroke = "Штрих";

		/// <summary>
		/// ФР производства СКБВТ ИСКРА, версия 1
		/// </summary>
        public const string ecrTypeSpark = "Искра";

		/// <summary>
		/// ФР производства АТОЛ-ТЕХНОЛОГИИ
		/// </summary>
        public const string ecrTypeAtol = "АТОЛ";

		/// <summary>
		/// ФР производства ПИЛОТ
		/// </summary>
        public const string ecrTypePilot = "Пилот";

		/// <summary>
		/// ФР производства ИНКОТЕКС
		/// </summary>
        public const string ecrTypeIncotex = "Инкотекс";

        /// <summary>
        /// ФР производства СКБВТ ИСКРА, версия 2
        /// </summary>
        public const string ecrTypeSpark2 = "Искра2";

        /// <summary>
        /// ФР производства Сервис-Плюс
        /// </summary>
        public const string ecrTypeServicePlus = "СервисПлюс";

		#endregion

		#region Принтеры чеков

		/// <summary>
		/// Стандартный принтер чеков по протоколу ESC POS
		/// </summary>
        public const string printerTypeGenericEpson = "Epson";

        /// <summary>
        /// Принтер подкладных документов по протоколу Star
        /// </summary>
        public const string printerTypeStarSlipPrinter = "Star";

		#endregion

		#region Считыватели

		/// <summary>
		/// RFID-считыватель производства Iron Logic
		/// </summary>
        public const string ironLogicRFIDReader = "Iron Logic RFID";

		#endregion

		#region Дисплеи покупателя

		/// <summary>
		/// Firich VFD
		/// </summary>
        public const string customerDisplayVFD = "VFD";

		/// <summary>
		/// DSP
		/// </summary>
        public const string customerDisplayDSP = "DSP";

		/// <summary>
		/// Epson
		/// </summary>
        public const string customerDisplayEpson = "Epson";

        /// <summary>
        /// Aedex
        /// </summary>
        public const string customerDisplayAedex = "Aedex";

		#endregion

        #region Модули управления бильярдом

        /// <summary>
        /// Модуль управления бильярдом через контроллер CL-8RC
        /// </summary>
        public const string blcCl8rc = "Контроллер CL-8RC";

        #endregion

        #region Весы

        /// <summary>
        /// Весы DIGI порционные
        /// </summary>
        public const string digiSimpleScales = "DIGI порционные";

        #endregion

        #region SMS-клиент

        /// <summary>
        /// Стандартный GSM-модем (AT-команды)
        /// </summary>
        public const string standardGSMModem = "Стандартный GSM-модем (AT-команды)";

        #endregion

        #region Модули управления турникетами

        /// <summary>
        /// Модуль управления турникетом T283 через контроллер NL-16D0-DI3
        /// </summary>
        public const string t283dualTripod = "T283 (двухпроходный)";

        #endregion
    }
}
