using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using DevicesCommon;
using DevicesCommon.Helpers;
using ERPService.SharedLibs.PropertyGrid.Converters;
using ERPService.SharedLibs.PropertyGrid.Editors;

namespace DevmanConfig
{
    /// <summary>
    /// Конфигурация диспетчера POS-устройств
    /// </summary>
    [Serializable]
    public class DevmanProperties
    {
        /// <summary>
        /// Текущая версия конфигурации
        /// </summary>
        public static string CURRENT_VERSION = "1.0";

        #region Поля
        
        private int _maxSessionsCount;
        private int _sessionTimeout;
        private int _servicePort;
        private bool _debugInfo;
        private string _version;
        private List<PrintableDeviceProperties> _printableDevices = null;
        private List<DisplayDeviceProperties> _displayDevices = null;
        private List<BilliardManagerProperties> _billardDevices = null;
        private List<ScalesDeviceProperties> _scalesDevices = null;
        private List<ReaderDeviceProperties> _readerDevices = null;
        private List<SMSClientProperties> _SMSClientDevices = null;
        private List<TurnstileProperties> _turnstileDevices = null;

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор. Установка параметров по умолчанию
        /// </summary>
        public DevmanProperties()
        {
            _maxSessionsCount = -1;
            _sessionTimeout = -1;
            _servicePort = 35100;
            _debugInfo = false;
            _version = CURRENT_VERSION;

            _printableDevices = new List<PrintableDeviceProperties>();
            _displayDevices = new List<DisplayDeviceProperties>();
            _billardDevices = new List<BilliardManagerProperties>();
            _scalesDevices = new List<ScalesDeviceProperties>();
            _turnstileDevices = new List<TurnstileProperties>();
            _readerDevices = new List<ReaderDeviceProperties>();
            _SMSClientDevices = new List<SMSClientProperties>();
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Максимальное число одновременных подключений к диспетчеру POS-устройств
        /// </summary>
        [Browsable(true)]
        [DefaultValue(-1)]
        [Category("Общие параметры")]
        [Description("Максимальное число одновременных подключений к диспетчеру POS-устройств")]
        [DisplayName("Макс. число клиентских сессий")]
        [PropertyOrder(0)]
        public int MaxSessionsCount
        {
            set { _maxSessionsCount = value; }
            get { return _maxSessionsCount; }
        }

        /// <summary>
        /// Максимальный интервал времени в секундах, в течении которого один клиент 
        /// может блокировать устройство. По истечении этого интервала диспетчер 
        /// считает устройство свободным
        /// </summary>
        [Browsable(true)]
        [DefaultValue(-1)]
        [Category("Общие параметры")]
        [Description("Максимальный интервал времени в секундах, в течении которого один клиент может блокировать устройство. По истечении этого интервала диспетчер	считает устройство свободным.")]
        [DisplayName("Таймаут клиентской сессии")]
        [PropertyOrder(1)]
        public int SessionTimeout
        {
            set { _sessionTimeout = value; }
            get { return _sessionTimeout; }
        }

        /// <summary>
        /// Номер TCP/IP порта службы диспетчера POS-устройств
        /// </summary>
        [Browsable(true)]
        [DefaultValue(35100)]
        [Category("Общие параметры")]
        [Description("Номер TCP/IP порта службы диспетчера POS-устройств")]
        [DisplayName("Порт службы")]
        [PropertyOrder(2)]
        public int ServicePort
        {
            set { _servicePort = value; }
            get { return _servicePort; }
        }

        /// <summary>
        /// Сохранение печатаемых чеков и дополнительной информации
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Общие параметры")]
        [Description("Сохранение печатаемых чеков и дополнительной информации")]
        [DisplayName("Отладочная информация")]
        [PropertyOrder(3)]
        [TypeConverter(typeof(RussianBooleanConverter))]
        public bool DebugInfo
        {
            set { _debugInfo = value; }
            get { return _debugInfo; }
        }

        /// <summary>
        /// Версия файла конфигурации
        /// </summary>
        [Browsable(false)]
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        /// <summary>
        /// Группа печатающих устройств
        /// </summary>
        [Browsable(false)]
        public List<PrintableDeviceProperties> PrintableDevices
        {
            get { return _printableDevices; }
            set { _printableDevices = value; }
        }

        /// <summary>
        /// Дисплеи покупателя
        /// </summary>
        [Browsable(false)]
        public List<DisplayDeviceProperties> DisplayDevices
        {
            get { return _displayDevices; }
            set { _displayDevices = value; }
        }

        /// <summary>
        /// Модули управления биллиардом
        /// </summary>
        [Browsable(false)]
        public List<BilliardManagerProperties> BillardDevices
        {
            get { return _billardDevices; }
            set { _billardDevices = value; }
        }

        /// <summary>
        /// Группа электронных весов
        /// </summary>
        [Browsable(false)]
        public List<ScalesDeviceProperties> ScalesDevices
        {   
            get { return _scalesDevices; }
            set { _scalesDevices = value; }
        }

        /// <summary>
        /// Группа турникетов
        /// </summary>
        [Browsable(false)]
        public List<TurnstileProperties> TurnstileDevices
        {
            get { return _turnstileDevices; }
            set { _turnstileDevices = value; }
        }

        /// <summary>
        /// Группа считывателей
        /// </summary>
        [Browsable(false)]
        public List<ReaderDeviceProperties> ReaderDevices
        {
            get { return _readerDevices; }
            set { _readerDevices = value; }
        }

        /// <summary>
        /// Группа SMS-клиентов
        /// </summary>
        [Browsable(false)]
        public List<SMSClientProperties> SMSClientDevices
        {
            get { return _SMSClientDevices; }
            set { _SMSClientDevices = value; }
        }

        #endregion

        #region Методы класса

        /// <summary>
        /// Загрузка конфигурации из указанного файла
        /// </summary>
        /// <param name="fileName">Путь к файлу</param>
        /// <returns></returns>
        public static DevmanProperties Load(string fileName)
        {
            // проверяем существование файла конфигурации
            if (File.Exists(fileName))
            {
                using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
                {
                    try
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(DevmanProperties));
                        return (DevmanProperties)xmlSerializer.Deserialize(fileStream);
                    }
                    catch (XmlException)
                    {
                        return new DevmanProperties();
                    }
                    catch (InvalidOperationException)
                    {
                        return new DevmanProperties();
                    }
                    catch (IOException)
                    {
                        return new DevmanProperties();
                    }
                }
            }
            else
                // конфигурация по умолчанию
                return new DevmanProperties();
        }

        /// <summary>
        /// Сохранение конфигурации в указанный файл
        /// </summary>
        /// <param name="fileName">Путь к файлу</param>
        public void Save(string fileName)
        {
            using (XmlTextWriter xmlWriter = new XmlTextWriter(fileName, Encoding.GetEncoding(1251)))
            {
                xmlWriter.Formatting = Formatting.Indented;
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(DevmanProperties));
                xmlSerializer.Serialize(xmlWriter, this);
            }
        }

        #endregion
    }

    /// <summary>
    /// Базовый класс конфигурации устройства
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorter))]
    [XmlInclude(typeof(PrintableDeviceProperties))]
    [XmlInclude(typeof(DisplayDeviceProperties))]
    [XmlInclude(typeof(ReaderDeviceProperties))]
    [XmlInclude(typeof(BilliardManagerProperties))]
    [XmlInclude(typeof(ScalesDeviceProperties))]
    [XmlInclude(typeof(SMSClientProperties))]
    [XmlInclude(typeof(TurnstileProperties))]    
    public class BaseDeviceProperties 
    {
        #region Поля

        private string _deviceType;
        private string _deviceId;

        #endregion

        #region Свойства

        /// <summary>
        /// Идентификатор устройства. Должен быть уникальным в пределах всей 
        /// конфигурации
        /// </summary>
        [Browsable(true)]
        [Category("Общие")]
        [Description("Идентификатор устройства. Должен быть уникальным в пределах всей конфигурации")]
        [DisplayName("Идентификатор устройства")]
        [PropertyOrder(0)]
        public string DeviceId
        {
            get { return _deviceId; }
            set { _deviceId = value; }
        }

        /// <summary>
        /// Тип устройства. Определяет используемый драйвер
        /// </summary>
        [Browsable(true)]
        [Category("Общие")]
        [Description("Тип устройства определяет используемый драйвер")]
        [DisplayName("Тип устройства")]
        [PropertyOrder(1)]
        public virtual string DeviceType
        {
            get { return _deviceType; }
            set { _deviceType = value; }
        }

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор
        /// </summary>
        public BaseDeviceProperties()
        {
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="deviceId">Идентификатор устройства</param>
        public BaseDeviceProperties(string deviceId)
            :this()
        {            
            _deviceId = deviceId;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Тестирование устройства. Переопределяется в наследниках
        /// </summary>
        public virtual void TestDevice()
        {
            MessageBox.Show("Функция тестирования для устройства не определена", "Тестирование устройства", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion
    }

    /// <summary>
    /// Конфигурация подключаемого устройства
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorter))]
    public class ConnectableDeviceProperties : BaseDeviceProperties
    {
        #region Поля

        private string _port;
        private int _baudRate;

        #endregion

        #region Свойства

        /// <summary>
        /// Имя порта для подключения уcтройства
        /// </summary>
        [Browsable(true)]
        [DefaultValue("COM1")]
        [Category("Параметры подключения")]
        [Description("Коммуникационный порт для подключения уcтройства")]
        [DisplayName("Порт")]
        [TypeConverter(typeof(PortTypeConverter))]
        [PropertyOrder(2)]
        public string Port
        {
            set { _port = value; }
            get { return _port; }
        }

        /// <summary>
        /// Скорость передачи данных через порт
        /// </summary>
        [Browsable(true)]
        [DefaultValue(9600)]
        [Category("Параметры подключения")]
        [Description("Скорость передачи данных через порт")]
        [DisplayName("Скорость")]
        [TypeConverter(typeof(BaudConverter))]
        [Editor(typeof(BaudEditor), typeof(UITypeEditor))]
        [PropertyOrder(3)]        
        public int BaudRate
        {
            set { _baudRate = value; }
            get { return _baudRate; }
        }

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор
        /// </summary>
        public ConnectableDeviceProperties()
        {
            _port = "COM1";
            _baudRate = 9600;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="deviceId">Идентификатор устройства</param>
        public ConnectableDeviceProperties(string deviceId)
            : base(deviceId)
        {
            _port = "COM1";
            _baudRate = 9600;
        }

        #endregion
    }


    /// <summary>
    /// Конфигурация печатающего устройства
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorter))]
    public class PrintableDeviceProperties : ConnectableDeviceProperties
    {
        #region Поля

        private PrintableTapeWidth _tapeWidth = new PrintableTapeWidth(40, 0);
        private int _topMargin = 0;
        private string[] _documentHeader;
        private string[] _documentFooter;
        private Bitmap _graphicHeader;
        private Bitmap _graphicFooter;
        private int _slipHeight = 0;
        private PrinterKind _paperType = PrinterKind.Receipt;
        private bool _printHeader = true;
        private bool _printFooter = true;
        private bool _printGraphicHeader = false;
        private bool _printGraphicFooter = false;
        private bool _dtrEnabled = true;


        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор
        /// </summary>
        public PrintableDeviceProperties()
            : base()
        {
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="deviceId">Идентификатор устройства</param>
        public PrintableDeviceProperties(string deviceId)
            : base(deviceId)
        {
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Аппаратный контроль передачи данных
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Category("Параметры подключения")]
        [Description("Контроль линии DTR/DSR")]
        [DisplayName("Контроль DTR/DSR")]
        [PropertyOrder(10)]
        [TypeConverter(typeof(RussianBooleanConverter))]
        public bool DtrEnabled
        {
            get { return _dtrEnabled; }
            set { _dtrEnabled = value; }
        }

        /// <summary>
        /// Тип устройства. Определяет используемый драйвер
        /// </summary>
        [Browsable(true)]
        [Category("Общие")]
        [Description("Тип устройства определяет используемый драйвер")]
        [DisplayName("Тип устройства")]
        [PropertyOrder(1)]
        [TypeConverter(typeof(PrintableTypesConverter))]
        public override string DeviceType
        {
            get { return base.DeviceType; }
            set { base.DeviceType = value; }
        }

        /// <summary>
        /// Способ печати: на чековой ленте, подкладном документе или комбинированный
        /// </summary>
        [Browsable(true)]
        [DefaultValue(PrinterKind.Receipt)]
        [Category("Печать")]
        [Description("Способ печати: на чековой ленте, подкладном документе или комбинированный")]
        [DisplayName("Способ печати")]
        [TypeConverter(typeof(PrinterKindConverter))]
        [Editor(typeof(PrinterKindEditor), typeof(UITypeEditor))]
        [PropertyOrder(3)]
        public PrinterKind PaperType
        {
            set { _paperType = value; }
            get { return _paperType; }
        }

        /// <summary>
        /// Ширина ленты в символах
        /// </summary>
        [Browsable(true)]
        [Category("Печать")]
        [Description("Ширина ленты в символах")]
        [DisplayName("Символов в строке")]
        [PropertyOrder(4)]
        public PrintableTapeWidth TapeWidth
        {
            set { _tapeWidth = value; }
            get { return _tapeWidth; }
        }

        /// <summary>
        /// Верхний отступ в строках от начала документа
        /// </summary>
        [Browsable(true)]
        [DefaultValue(0)]
        [Category("Печать")]
        [Description("Верхний отступ в строках от начала документа")]
        [DisplayName("Верхний отступ")]
        [PropertyOrder(5)]
        public int TopMargin
        {
            set { _topMargin = value; }
            get { return _topMargin; }
        }

        /// <summary>
        /// Длина бланка подкладного документа в миллиметрах
        /// </summary>
        [Browsable(true)]
        [DefaultValue(0)]
        [Category("Печать")]
        [Description("Длина бланка подкладного документа в миллиметрах")]
        [DisplayName("Длина подкладного документа")]
        [PropertyOrder(6)]
        public int SlipHeight
        {
            set { _slipHeight = value; }
            get { return _slipHeight; }
        }

        #region Клише

        /// <summary>
        /// Печатать строки клише
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Category("Печать клише")]
        [Description("Печатать клише")]
        [DisplayName("Печатать клише")]
        [PropertyOrder(7)]
        [TypeConverter(typeof(RussianBooleanConverter))]
        public bool PrintHeader
        {
            get { return _printHeader; }
            set { _printHeader = value; }
        }

        /// <summary>
        /// Строки, печатаемые перед документом
        /// </summary>
        [Browsable(true)]
        [Category("Печать клише")]
        [Description("Строки, печатаемые перед документом")]
        [DisplayName("Клише чека")]
        [DefaultValue(null)]
        [PropertyOrder(8)]
        [Editor(typeof(TextEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(HideValueConverter))]
        public string[] Header
        {
            set { _documentHeader = value; }
            get { return _documentHeader; }
        }

        /// <summary>
        /// Печатать графическое клише
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Печать клише")]
        [Description("Печатать графическое клише")]
        [DisplayName("Печатать графическое клише")]
        [TypeConverter(typeof(RussianBooleanConverter))]
        [PropertyOrder(9)]
        public bool PrintGraphicHeader
        {
            get { return _printGraphicHeader; }
            set { _printGraphicHeader = value; }
        }

        /// <summary>
        /// Графическое клише
        /// </summary>
        [Browsable(true)]
        [DefaultValue(null)]
        [Category("Печать клише")]
        [Description("Графическое клише")]
        [DisplayName("Графическое клише")]
        [Editor(typeof(GraphicHeaderEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(GraphicHeaderTypeConverter))]
        [XmlIgnore]
        [PropertyOrder(10)]
        public Bitmap GraphicHeader
        {
            get { return _graphicHeader; }
            set { _graphicHeader = value; }
        }

        #endregion

        #region Подвал

        /// <summary>
        /// Печатать подвал
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Category("Печать подвала")]
        [Description("Печатать подвал")]
        [DisplayName("Печатать подвал")]
        [TypeConverter(typeof(RussianBooleanConverter))]
        [PropertyOrder(11)]
        public bool PrintFooter
        {
            get { return _printFooter; }
            set { _printFooter = value; }
        }

        /// <summary>
        /// Строки, печатаемые после документа
        /// </summary>
        [Browsable(true)]
        [Category("Печать подвала")]
        [Description("Строки, печатаемые после документа")]
        [DisplayName("Подвал чека")]
        [DefaultValue(null)]
        [PropertyOrder(12)]
        [Editor(typeof(TextEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(HideValueConverter))]
        public string[] Footer
        {
            set { _documentFooter = value; }
            get { return _documentFooter; }
        }

        /// <summary>
        /// Печатать графический подвал
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Печать подвала")]
        [Description("Печатать графический подвал")]
        [DisplayName("Печатать графический подвал")]
        [TypeConverter(typeof(RussianBooleanConverter))]
        [PropertyOrder(13)]
        public bool PrintGraphicFooter
        {
            get { return _printGraphicFooter; }
            set { _printGraphicFooter = value; }
        }

        /// <summary>
        /// Графический подвал
        /// </summary>
        [Browsable(true)]
        [DefaultValue(null)]
        [Category("Печать подвала")]
        [Description("Графический подвал")]
        [DisplayName("Графический подвал")]
        [Editor(typeof(GraphicHeaderEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(GraphicHeaderTypeConverter))]
        [PropertyOrder(14)]
        [XmlIgnore]
        public Bitmap GraphicFooter
        {
            get { return _graphicFooter; }
            set { _graphicFooter = value; }
        }

        #endregion

        /// <summary>
        /// Свойство для сериализации графического заголовка чека.
        /// </summary>
        [Browsable(false)]
        public byte[] GraphicHeaderBytes
        {
            get
            {
                if (_graphicHeader == null)
                    return null;
                else
                {
                    using (MemoryStream imgStream = new MemoryStream())
                    {
                        _graphicHeader.Save(imgStream, System.Drawing.Imaging.ImageFormat.Bmp);
                        return imgStream.ToArray();
                    }
                }
            }
            set 
            {
                if (value != null && value.Length > 0)
                {
                    MemoryStream imgStream = new MemoryStream(value);
                    _graphicHeader = new Bitmap(imgStream);
                }
            }
        }

        /// <summary>
        /// Свойство для сериализации графического подвала чека.
        /// </summary>
        [Browsable(false)]
        public byte[] GraphicFooterBytes
        {
            get
            {
                if (_graphicFooter == null)
                    return null;
                else
                {
                    using (MemoryStream imgStream = new MemoryStream())
                    {
                        _graphicFooter.Save(imgStream, System.Drawing.Imaging.ImageFormat.Bmp);
                        return imgStream.ToArray();
                    }
                }
            }
            set
            {
                if (value != null && value.Length > 0)
                {
                    using (MemoryStream imgStream = new MemoryStream(value))
                    {
                        _graphicFooter = new Bitmap(imgStream);
                    }
                }
            }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Тестовая печать через диспетчер устройств
        /// </summary>
        public override void TestDevice()
        {
            TestPrintForm.TestPrint(DeviceId);
        }

        #endregion
    }

    /// <summary>
    /// Конфигурация дисплея покупателя
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorter))]
    public class DisplayDeviceProperties : ConnectableDeviceProperties
    {
        #region Свойства

        /// <summary>
        /// Тип устройства. Определяет используемый драйвер
        /// </summary>
        [Browsable(true)]
        [Category("Общие")]
        [Description("Тип устройства определяет используемый драйвер")]
        [DisplayName("Тип устройства")]
        [PropertyOrder(1)]
        [TypeConverter(typeof(DeviceTypesConverter<CustomerDisplayAttribute>))]
        public override string DeviceType
        {
            get { return base.DeviceType; }
            set { base.DeviceType = value; }
        }

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public DisplayDeviceProperties()
            : base()
        {
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public DisplayDeviceProperties(string deviceId)
            : base(deviceId)
        {
        }

        #endregion

        #region Методы

        /// <summary>
        /// Тестирование устройства.
        /// </summary>
        public override void TestDevice()
        {
            DisplayTestForm.TestDisplay(DeviceId);
        }

        #endregion

    }

    /// <summary>
    /// Конфигурация считывателя
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorter))]
    public class ReaderDeviceProperties : ConnectableDeviceProperties
    {
        #region Константы

        private const byte DEF_STOP_CHAR = 0xA;

        //private const ERPService.SharedLibs.Helpers.SerialCommunications.Parity DEF_PARITY =
            //ERPService.SharedLibs.Helpers.SerialCommunications.Parity.None;
        
        #endregion

        #region Поля

        private byte _stopChar = DEF_STOP_CHAR;

        private ERPService.SharedLibs.Helpers.SerialCommunications.Parity _parity = ERPService.SharedLibs.Helpers.SerialCommunications.Parity.Even;

        #endregion

        #region Свойства

        /// <summary>
        /// Тип устройства. Определяет используемый драйвер
        /// </summary>
        [Browsable(true)]
        [Category("Общие")]
        [Description("Тип устройства определяет используемый драйвер")]
        [DisplayName("Тип устройства")]
        [PropertyOrder(1)]
        [TypeConverter(typeof(DeviceTypesConverter<GenericReaderAttribute>))]
        public override string DeviceType
        {
            get { return base.DeviceType; }
            set { base.DeviceType = value; }
        }

        /// <summary>
        /// Стоп-символ
        /// </summary>
        [Browsable(true)]
        [Category("Параметры подключения")]
        [DisplayName("Стоп-символ")]
        [Description("Десятичный код стоп-символа")]
        [PropertyOrder(4)]
        [DefaultValue(DEF_STOP_CHAR)]
        public byte StopChar
        {
            get { return _stopChar; }
            set { _stopChar = value; }
        }

        /// <summary>
        /// Контроль четности
        /// </summary>
        [Browsable(true)]
        [Category("Параметры подключения")]
        [DisplayName("Контроль четности")]
        [Description("Флаг контроля четности")]
        [PropertyOrder(5)]
//        [DefaultValue(DEF_PARITY)]
        [Editor(typeof(ParityEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ParityConverter))]
        public ERPService.SharedLibs.Helpers.SerialCommunications.Parity Parity
        {
            get { return _parity; }
            set { _parity = value; }
        }

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public ReaderDeviceProperties()
            : base()
        {
            DeviceType = DevicesCommon.Helpers.DeviceNames.ironLogicRFIDReader;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public ReaderDeviceProperties(string deviceId)
            : base(deviceId)
        {
            DeviceType = DevicesCommon.Helpers.DeviceNames.ironLogicRFIDReader;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Тестирование устройства.
        /// </summary>
        public override void TestDevice()
        {
            ReaderTestForm.TestReader(DeviceId);
        }

        #endregion
    }

    /// <summary>
    /// Конфигурация модуля управления биллиардом
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorter))]
    public class BilliardManagerProperties : ConnectableDeviceProperties
    {
        #region Методы

        /// <summary>
        /// Тестирование устройства.
        /// </summary>
        public override void TestDevice()
        {
            TestBilliardForm.TestBilliard(DeviceId);
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Тип устройства. Определяет используемый драйвер
        /// </summary>
        [Browsable(true)]
        [Category("Общие")]
        [Description("Тип устройства определяет используемый драйвер")]
        [DisplayName("Тип устройства")]
        [PropertyOrder(1)]
        [TypeConverter(typeof(DeviceTypesConverter<BilliardsManagerAttribute>))]
        public override string DeviceType
        {
            get { return base.DeviceType; }
            set { base.DeviceType = value; }
        }

        #endregion

        #region Констуктор

        /// <summary>
        /// Конструктор без параметров
        /// </summary>
        public BilliardManagerProperties()
        {
            DeviceType = DevicesCommon.Helpers.DeviceNames.blcCl8rc;
        }

        /// <summary>
        /// Конструктор с параметром
        /// </summary>
        /// <param name="deviceId"></param>
        public BilliardManagerProperties(string deviceId)
            :base(deviceId)
        {
            DeviceType = DevicesCommon.Helpers.DeviceNames.blcCl8rc;
        }

        #endregion
    }

    /// <summary>
    /// Конфигурация турникета
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorter))]
    public class TurnstileProperties : ConnectableDeviceProperties
    {
        #region Поля

        private TurnstileDirection _direction;

        private int _timeout = 15;

        private int _address = 0;

        #endregion

        #region Методы

        /// <summary>
        /// Тестирование устройства.
        /// </summary>
        public override void TestDevice()
        {
            TurnstileTestForm.TestDevice(DeviceId);
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Тип устройства. Определяет используемый драйвер
        /// </summary>
        [Browsable(true)]
        [Category("Общие")]
        [Description("Тип устройства определяет используемый драйвер")]
        [DisplayName("Тип устройства")]
        [PropertyOrder(1)]
        [TypeConverter(typeof(DeviceTypesConverter<TurnstileDeviceAttribute>))]
        public override string DeviceType
        {
            get { return base.DeviceType; }
            set { base.DeviceType = value; }
        }

        /// <summary>
        /// Адрес устройства
        /// </summary>
        [Browsable(true)]
        [Category("Параметры подключения")]
        [Description("Адрес устройства")]
        [DisplayName("Адрес")]
        [PropertyOrder(1)]
        [DefaultValue(0)]
        public int Address
        {
            get { return _address; }
            set { _address = value; }
        }

        /// <summary>
        /// Направление
        /// </summary>
        [Browsable(true)]
        [Category("Проход")]
        [Description("Направление прохода")]
        [DisplayName("Направление")]
        [PropertyOrder(1)]
        [TypeConverter(typeof(EntryTypeConverter))]
        [Editor(typeof(EntryTypeEditor), typeof(UITypeEditor))]
        public TurnstileDirection Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        /// <summary>
        /// Таймаут турникета
        /// </summary>
        [Browsable(true)]
        [Category("Проход")]
        [Description("Время, в течении которого турникет остается открытым")]
        [DisplayName("Таймаут, с")]
        [PropertyOrder(2)]
        [DefaultValue(15)]
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }       

        #endregion

        #region Констуктор

        /// <summary>
        /// Конструктор без параметров
        /// </summary>
        public TurnstileProperties()
        {
            DeviceType = DevicesCommon.Helpers.DeviceNames.t283dualTripod;
        }

        /// <summary>
        /// Конструктор с параметром
        /// </summary>
        /// <param name="deviceId"></param>
        public TurnstileProperties(string deviceId)
            : base(deviceId)
        {
            DeviceType = DevicesCommon.Helpers.DeviceNames.t283dualTripod;
        }

        #endregion
    }



    /// <summary>
    /// Конфигурация электронных весов
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorter))]
    public class ScalesDeviceProperties : BaseDeviceProperties
    {
        #region Поля

        private string _connectionString;

        #endregion

        #region Свойства

        /// <summary>
        /// Тип устройства. Определяет используемый драйвер
        /// </summary>
        [Browsable(true)]
        [Category("Общие")]
        [Description("Тип устройства определяет используемый драйвер")]
        [DisplayName("Тип устройства")]
        [PropertyOrder(1)]
        [TypeConverter(typeof(DeviceTypesConverter<ScaleAttribute>))]
        public override string DeviceType
        {
            get { return base.DeviceType; }
            set { base.DeviceType = value; }
        }

        /// <summary>
        /// Строка подключения к весам
        /// </summary>
        [Browsable(true)]
        [Category("Параметры подключения")]
        [Description("Строка подключения")]
        [DisplayName("Строка подключения")]
        [Editor(typeof(ScalesConnectionEditor), typeof(UITypeEditor))]
        [PropertyOrder(1)]
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Тестирование устройства.
        /// </summary>
        public override void TestDevice()
        {
            ScalesTestForm.TestScales(DeviceId);
        }

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор без параметров
        /// </summary>
        public ScalesDeviceProperties()
        {
        }

        /// <summary>
        /// Конструктор с параметром
        /// </summary>
        /// <param name="deviceId"></param>
        public ScalesDeviceProperties(string deviceId)
            : base(deviceId)
        {
        }

        #endregion
    }

    /// <summary>
    /// Конфигурация SMS-клиента
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorter))]
    public class SMSClientProperties : BaseDeviceProperties
    {
        #region Поля

        private SerializableDictionary<string, string> _settings;

        #endregion            

        #region Свойства

        /// <summary>
        /// Тип устройства. Определяет используемый драйвер
        /// </summary>
        [Browsable(true)]
        [Category("Общие")]
        [Description("Тип устройства определяет используемый драйвер")]
        [DisplayName("Тип устройства")]
        [PropertyOrder(1)]
        [TypeConverter(typeof(DeviceTypesConverter<SMSClientAttribute>))]
        public override string DeviceType
        {
            get { return base.DeviceType; }
            set { base.DeviceType = value; }
        }

        /// <summary>
        /// Параметры устройства
        /// </summary>
        [Browsable(true)]
        [Category("Общие")]
        [Description("Список параметров устройства")]
        [DisplayName("Параметры устройства")]
        [TypeConverter(typeof(HideValueConverter))]
        [Editor(typeof(KeyValueEditor), typeof(UITypeEditor))]
        public SerializableDictionary<string, string> Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Тестирование устройства.
        /// </summary>
        public override void TestDevice()
        {
            SMSClientTestForm.TestSMSClient(DeviceId);
        }

        #endregion

        #region Констуктор

        /// <summary>
        /// Конструктор без параметров
        /// </summary>
        public SMSClientProperties()
        {
            DeviceType = DevicesCommon.Helpers.DeviceNames.standardGSMModem;
            _settings = new SerializableDictionary<string, string>();
        }

        /// <summary>
        /// Конструктор с параметром
        /// </summary>
        /// <param name="deviceId"></param>
        public SMSClientProperties(string deviceId)
            :base(deviceId)
        {
            DeviceType = DevicesCommon.Helpers.DeviceNames.standardGSMModem;
            _settings = new SerializableDictionary<string, string>();
        }

        #endregion
    }

}