namespace DevicesCommon.Helpers
{
    /// <summary>
    /// Общие для всех устройств коды ошибок
    /// </summary>
    public enum GeneralError
	{
		/// <summary>
		/// Успешно (ошибок нет)
		/// </summary>
		Success	= 0x1000,

		/// <summary>
		/// Устройство занято другим приложением
		/// </summary>
		Busy	= 0x1001,

		/// <summary>
		/// Таймаут во время обмена данными с устройством
		/// </summary>
		Timeout	= 0x1002,

        /// <summary>
        /// Устройство неактивно. Требуется активировать устройство 
        /// перед обращением к его интерфейсным методам
        /// </summary>
        Inactive  = 0x1003,

        /// <summary>
        /// Исключительная ситуация во время обмена данными с устройством
        /// </summary>
        Exception = 0x1004,

        /// <summary>
        /// Функция не поддерживается устройством
        /// </summary>
        Unsupported = 0x1005,

        /// <summary>
        /// Функция не поддерживается устройством при данных параметрах команды
        /// </summary>
        CurrentlyUnsupported = 0x1006,

        /// <summary>
        /// Ошибка, специфическая для протокола обмена с устройством
        /// </summary>
        Specific = 0x1007
	}

	/// <summary>
	/// Тип документа
	/// </summary>
	public enum DocumentType
	{
		/// <summary>
		/// Продажа
		/// </summary>
		Sale,

		/// <summary>
		/// Возврат
		/// </summary>
		Refund,

		/// <summary>
		/// Внесение денег в кассу
		/// </summary>
		PayingIn,
		
		/// <summary>
		/// Выплата денег из кассы
		/// </summary>
		PayingOut,

        /// <summary>
        /// X-отчет
        /// </summary>
        XReport,

        /// <summary>
        /// Z-отчет
        /// </summary>
        ZReport,

        /// <summary>
        /// Отчет по секциям
        /// </summary>
        SectionsReport,

		/// <summary>
		/// Нефискальный документ
		/// </summary>
		Other
	}

    /// <summary>
    /// Номер принтера для печати документа
    /// </summary>
    public enum PrinterNumber
    {
        /// <summary>
        /// Основной принтер устройства
        /// </summary>
        MainPrinter,

        /// <summary>
        /// Первый дополнительный принтер устройства
        /// </summary>
        AdditionalPrinter1,

        /// <summary>
        /// Второй дополнительный принтер устройства
        /// </summary>
        AdditionalPrinter2
    }

	/// <summary>
	/// Выравнивание строки при печати
	/// </summary>
	public enum AlignOptions
	{
		/// <summary>
		/// По левому краю
		/// </summary>
		Left,

		/// <summary>
		/// По центру
		/// </summary>
		Center,

		/// <summary>
		/// По правому краю
		/// </summary>
		Right
	}

	/// <summary>
	/// Типы оплаты
	/// </summary>
	public enum FiscalPaymentType
	{
		/// <summary>
		/// Наличными
		/// </summary>
		Cash,
		
		/// <summary>
		/// Платежной картой
		/// </summary>
		Card,
		
		/// <summary>
		/// Произвольный безналичный тип оплаты №1
		/// </summary>
		Other1,
		
		/// <summary>
		/// Произвольный безналичный тип оплаты №2
		/// </summary>
		Other2,
		
		/// <summary>
		/// Произвольный безналичный тип оплаты №3
		/// </summary>
		Other3
	}

	/// <summary>
	/// Тип сменного отчета
	/// </summary>
	public enum ShiftReportType
	{
		/// <summary>
		/// Отчет без гашения
		/// </summary>
		ReportX,

		/// <summary>
		/// Отчет по секциям
		/// </summary>
		ReportSections,

		/// <summary>
		/// Отчет с гашением
		/// </summary>
		ReportZ
	}

	/// <summary>
	/// Тип фискального отчета
	/// </summary>
	public enum FiscalReportType
	{
		/// <summary>
		/// По датам
		/// </summary>
		ByDates,

		/// <summary>
		/// По сменам
		/// </summary>
		ByShifts
	}

	/// <summary>
	/// Причина генерации события от принтера
	/// </summary>
	public enum PrinterBreak
	{
		/// <summary>
		/// Отсутствует бумага
		/// </summary>
		PaperOut,

		/// <summary>
		/// Открытый денежный ящик
		/// </summary>
		OpenedDrawer
	}

	/// <summary>
	/// Причина генерации события от ФР
	/// </summary>
	public enum FiscalBreak
	{
		/// <summary>
		/// Разница времени ОС и ФП более 10 минут
		/// </summary>
		TimeDeltaIsLarge,

		/// <summary>
		/// Регистратор заблокирован
		/// </summary>
		Locked,

		/// <summary>
		/// Смена превысила 24 часа
		/// </summary>
		OverShift,

		/// <summary>
		/// Открытая смена
		/// </summary>
		OpenedShift,

		/// <summary>
		/// Ошибка протокола обмена с ФР
		/// </summary>
		ProtocolError,

		/// <summary>
		/// Открытый документ
		/// </summary>
		OpenedDocument
	}

	/// <summary>
	/// Статус отсутствия бумаги в принтере чеков
	/// </summary>
	public enum PaperOutStatus
	{
		/// <summary>
		/// Бумага есть
		/// </summary>
		Present,

		/// <summary>
		/// Бумага отсутствует (пассивное отсутствие бумаги)
		/// </summary>
		OutPassive,

		/// <summary>
		/// Бумага отсутствует (активное отсутствие бумаги, потребуется
		/// команда продолжения печати)
		/// </summary>
		OutActive,

		/// <summary>
		/// Бумага есть (после активного отсутвтия бумаги, требуется 
		/// команда продолжения печати)
		/// </summary>
		OutAfterActive
	}

	/// <summary>
	/// Стиль шрифта на чековых принтерах
	/// </summary>
	public enum FontStyle
	{
		/// <summary>
		/// Обычный
		/// </summary>
		Regular,

		/// <summary>
		/// Двойная высота
		/// </summary>
		DoubleHeight,

		/// <summary>
		/// Двойная ширина
		/// </summary>
		DoubleWidth,

		/// <summary>
		/// Двойная высота и двойная ширина
		/// </summary>
		DoubleAll
	}

    /// <summary>
    /// Сетевой протокол (для устройств, реализующих INetworkDevice)
    /// </summary>
    public enum NetworkProtocol
    {
        /// <summary>
        /// TCP-устройство
        /// </summary>
        TCP,

        /// <summary>
        /// UDP-устройство
        /// </summary>
        UDP
    }

    /// <summary>
    /// Вид печатающего устройства
    /// </summary>
    public enum PrinterKind
    {
        /// <summary>
        /// Рулонный
        /// </summary>
        Receipt,

        /// <summary>
        /// Подкладной
        /// </summary>
        Slip,

        /// <summary>
        /// Комбинированный
        /// </summary>
        Combo
    }
}