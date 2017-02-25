using System;
using System.Collections.Generic;
using System.Text;

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
		public const String ecrTypeStroke = "Штрих";

		/// <summary>
		/// ФР производства СКБВТ ИСКРА, версия 1
		/// </summary>
        public const String ecrTypeSpark = "Искра";

		/// <summary>
		/// ФР производства АТОЛ-ТЕХНОЛОГИИ
		/// </summary>
        public const String ecrTypeAtol = "АТОЛ";

		/// <summary>
		/// ФР производства ПИЛОТ
		/// </summary>
        public const String ecrTypePilot = "Пилот";

		/// <summary>
		/// ФР производства ИНКОТЕКС
		/// </summary>
        public const String ecrTypeIncotex = "Инкотекс";

        /// <summary>
        /// ФР производства СКБВТ ИСКРА, версия 2
        /// </summary>
        public const String ecrTypeSpark2 = "Искра2";

        /// <summary>
        /// ФР производства Сервис-Плюс
        /// </summary>
        public const String ecrTypeServicePlus = "СервисПлюс";

		#endregion

		#region Принтеры чеков

		/// <summary>
		/// Стандартный принтер чеков по протоколу ESC POS
		/// </summary>
        public const String printerTypeGenericEpson = "Epson";

        /// <summary>
        /// Принтер подкладных документов по протоколу Star
        /// </summary>
        public const String printerTypeStarSlipPrinter = "Star";

		#endregion

		#region Считыватели

		/// <summary>
		/// RFID-считыватель производства Iron Logic
		/// </summary>
        public const String ironLogicRFIDReader = "Iron Logic RFID";

		#endregion

		#region Дисплеи покупателя

		/// <summary>
		/// Firich VFD
		/// </summary>
        public const String customerDisplayVFD = "VFD";

		/// <summary>
		/// DSP
		/// </summary>
        public const String customerDisplayDSP = "DSP";

		/// <summary>
		/// Epson
		/// </summary>
        public const String customerDisplayEpson = "Epson";

        /// <summary>
        /// Aedex
        /// </summary>
        public const String customerDisplayAedex = "Aedex";

		#endregion

        #region Модули управления бильярдом

        /// <summary>
        /// Модуль управления бильярдом через контроллер CL-8RC
        /// </summary>
        public const String blcCl8rc = "Контроллер CL-8RC";

        #endregion

        #region Весы

        /// <summary>
        /// Весы DIGI порционные
        /// </summary>
        public const String digiSimpleScales = "DIGI порционные";

        #endregion

        #region SMS-клиент

        /// <summary>
        /// Стандартный GSM-модем (AT-команды)
        /// </summary>
        public const String standardGSMModem = "Стандартный GSM-модем (AT-команды)";

        #endregion

        #region Модули управления турникетами

        /// <summary>
        /// Модуль управления турникетом T283 через контроллер NL-16D0-DI3
        /// </summary>
        public const String t283dualTripod = "T283 (двухпроходный)";

        #endregion
    }
}
