using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace DevicesCommon.Helpers
{
    /// <summary>
    /// Ширина ленты на печатающих устройствах
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class PrintableTapeWidth: ISerializable
    {
        private int _mainPrinter;
        private int _additionalPrinter1;

        /// <summary>
        /// Возвращает строковое представление объекта
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0}; {1}", _mainPrinter, _additionalPrinter1);
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        public PrintableTapeWidth()
        {
            _mainPrinter = 0;
            _additionalPrinter1 = 0;
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="mainPrinter">Ширина ленты на основном принтере</param>
        /// <param name="additionalPrinter1">Ширина ленты на первом дополнительном 
        /// принтере</param>
        public PrintableTapeWidth(int mainPrinter, int additionalPrinter1)
        {
            _mainPrinter = mainPrinter;
            _additionalPrinter1 = additionalPrinter1;
        }

		/// <summary>
		/// Конструктор сериализации
		/// </summary>
		/// <param name="info">Класс <see cref="SerializationInfo"/> для информации о сериализации</param>
		/// <param name="context">Контекст сериализации</param>
        protected PrintableTapeWidth(SerializationInfo info, StreamingContext context)
		{
            _mainPrinter = info.GetInt32("_mainPrinter");
            _additionalPrinter1 = info.GetInt32("_additionalPrinter1");
		}

        /// <summary>
        /// Сериализация объектов класса
        /// </summary>
        /// <param name="info">Класс <see cref="SerializationInfo"/> для информации о сериализации</param>
        /// <param name="context">Контекст сериализации</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_mainPrinter", _mainPrinter);
            info.AddValue("_additionalPrinter1", _additionalPrinter1);
        }

        /// <summary>
        /// Ширина ленты на основном принтере
        /// </summary>
        [DefaultValue(0)]
        [Description("Ширина ленты на основном принтере")]
        [DisplayName("Основной принтер")]
        public int MainPrinter
        {
            get { return _mainPrinter; }
            set { _mainPrinter = value; }
        }

        /// <summary>
        /// Ширина ленты на первом дополнительном принтере
        /// </summary>
        [DefaultValue(0)]
        [Description("Ширина ленты на первом дополнительном принтере")]
        [DisplayName("Дополнительный принтер №1")]
        public int AdditionalPrinter1
        {
            get { return _additionalPrinter1; }
            set { _additionalPrinter1 = value; }
        }
    }

	/// <summary>
	/// Аппаратные характеристики печатающего устройства
	/// </summary>
	[Serializable]
	public class PrintableDeviceInfo : ISerializable
	{
        // ширина чековой ленты
        private PrintableTapeWidth _tapeWidth;
        // отступ в строках от верхнего края документа
        private int _topMargin;
        // длина бланка подкладного документа
        private int _slipFormLength;
        // вид печатающего устройства
        private PrinterKind _kind;
        // аппаратный контроль за передачей данных
        private bool _dsrFlowControl; 

		#region Характеристики

        /// <summary>
        /// Вид печатающего устройства
        /// </summary>
        public PrinterKind Kind
        {
            get { return _kind; }
            set { _kind = value; }
        }

		/// <summary>
		/// Ширина чековой ленты в символах
		/// </summary>
        public PrintableTapeWidth TapeWidth
        {
            get { return _tapeWidth; }
            set { _tapeWidth = value; }
        }

        /// <summary>
        /// Верхний отступ от края печатной формы, строк
        /// </summary>
        public int TopMargin
        {
            get { return _topMargin; }
            set { _topMargin = value; }
        }

        /// <summary>
        /// Длина бланка подкладного документа в миллиметрах
        /// </summary>
        public int SlipFormLength
        {
            get { return _slipFormLength; }
            set { _slipFormLength = value; }
        }

        /// <summary>
        /// Аппаратный контроль DTR/DSR
        /// </summary>
        public bool DsrFlowControl
        {
            get { return _dsrFlowControl; }
            set { _dsrFlowControl = value; }
        }

        /// <summary>
        /// Поддерживается или нет жирный шрифт (удвоенный по ширине)
        /// </summary>
        public readonly bool SupportsBoldFont;

		#endregion

		#region Конструктор

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="tapeWidth">Ширина чековой ленты в символах</param>
        /// <param name="supportsBoldFont">Поддерживается или нет жирный шрифт (удвоенный по ширине)</param>
        /// <param name="topMargin">Верхний отступ от края печатной формы, строк</param>
        /// <param name="slipFormLength">Длина бланка подкладного документа в миллиметрах</param>
        /// <param name="kind">Вид печатающего устройства</param>
        /// <param name="dsrFlowControl">Аппаратный контроль DTR/DSR</param>
        public PrintableDeviceInfo(PrintableTapeWidth tapeWidth, bool supportsBoldFont, int topMargin,
            int slipFormLength, PrinterKind kind, bool dsrFlowControl)
        {
            _tapeWidth = tapeWidth;
            _topMargin = topMargin;
            _slipFormLength = slipFormLength;
            _kind = kind;
            _dsrFlowControl = dsrFlowControl;
            SupportsBoldFont = supportsBoldFont;
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="tapeWidth">Ширина чековой ленты в символах</param>
        /// <param name="supportsBoldFont">Поддерживается или нет жирный шрифт (удвоенный по ширине)</param>
        /// <param name="topMargin">Верхний отступ от края печатной формы, строк</param>
        /// <param name="slipFormLength">Длина бланка подкладного документа в миллиметрах</param>
        /// <param name="kind">Вид печатающего устройства</param>
        public PrintableDeviceInfo(PrintableTapeWidth tapeWidth, bool supportsBoldFont, int topMargin,
            int slipFormLength, PrinterKind kind)
            : this(tapeWidth, supportsBoldFont, topMargin, slipFormLength, kind, false)
        {
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="tapeWidth">Ширина чековой ленты в символах</param>
        /// <param name="supportsBoldFont">Поддерживается или нет жирный шрифт (удвоенный по ширине)</param>
        public PrintableDeviceInfo(PrintableTapeWidth tapeWidth, bool supportsBoldFont)
            : this(tapeWidth, supportsBoldFont, 0, 0, PrinterKind.Receipt, false)
        {
        }

		/// <summary>
		/// Конструктор сериализации
		/// </summary>
		/// <param name="info">Класс <see cref="SerializationInfo"/> для информации о сериализации</param>
		/// <param name="context">Контекст сериализации</param>
		protected PrintableDeviceInfo(SerializationInfo info, StreamingContext context)
		{
            _tapeWidth = (PrintableTapeWidth)info.GetValue("_tapeWidth", 
                typeof(PrintableTapeWidth));
            _kind = (PrinterKind)info.GetValue("_kind", typeof(PrinterKind));
            _topMargin = info.GetInt32("_topMargin");
            SupportsBoldFont = info.GetBoolean("SupportsBoldFont");
            _slipFormLength = info.GetInt32("_slipFormLength");
            _dsrFlowControl = info.GetBoolean("_dsrFlowControl");
		}

		#endregion

		#region Сериализация

		/// <summary>
		/// Сериализация объектов класса
		/// </summary>
		/// <param name="info">Класс <see cref="SerializationInfo"/> для информации о сериализации</param>
		/// <param name="context">Контекст сериализации</param>
		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
            info.AddValue("_tapeWidth", _tapeWidth, typeof(PrintableTapeWidth));
            info.AddValue("SupportsBoldFont", SupportsBoldFont);
            info.AddValue("_topMargin", _topMargin);
            info.AddValue("_slipFormLength", _slipFormLength);
            info.AddValue("_kind", _kind);
            info.AddValue("_dsrFlowControl", _dsrFlowControl);
		}

		#endregion
	}

	/// <summary>
	/// Аппаратные характеристики фискального устройства
	/// </summary>
	[Serializable]
	public class FiscalDeviceInfo : ISerializable
	{
		#region Характеристики

		/// <summary>
		/// Тип фискальной памяти
		/// </summary>
		public readonly string DeviceType;

		/// <summary>
		/// Серийный номер
		/// </summary>
		public readonly string SerialNo;

		#endregion

		#region Конструктор

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="deviceType">Тип фискальной памяти</param>
		/// <param name="serialNo">Серийны номер устройства</param>
		public FiscalDeviceInfo(string deviceType, string serialNo)
		{
			DeviceType = deviceType;
			SerialNo = serialNo;
		}

		/// <summary>
		/// Конструктор для сериализации
		/// </summary>
		/// <param name="info">Класс <see cref="SerializationInfo"/> для информации о сериализации</param>
		/// <param name="context">Контекст сериализации</param>
		protected FiscalDeviceInfo(SerializationInfo info, StreamingContext context)
		{
			DeviceType = info.GetString("FiscalDeviceType");
			SerialNo = info.GetString("SerialNo");
		}

		#endregion

		#region Сериализация

		/// <summary>
		/// Сериализация объектов класса
		/// </summary>
		/// <param name="info">Класс <see cref="SerializationInfo"/> для информации о сериализации</param>
		/// <param name="context">Контекст сериализации</param>
		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("FiscalDeviceType", DeviceType);
			info.AddValue("SerialNo", SerialNo);
		}

		#endregion
	}
}
