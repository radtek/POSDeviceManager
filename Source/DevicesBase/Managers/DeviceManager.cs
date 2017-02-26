using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using DevicesBase.Helpers;
using DevicesCommon;
using DevicesCommon.Connectors;
using DevicesCommon.Helpers;
using DevmanConfig;
using ERPService.SharedLibs.Eventlog;
using ERPService.SharedLibs.Helpers;
using ERPService.SharedLibs.Helpers.SerialCommunications;
using ERPService.SharedLibs.Remoting;

namespace DevicesBase
{
    /// <summary>
    /// Аргументы события активации/деактивации устройства
    /// </summary>
    public class DeviceStatusEventArgs : EventArgs
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="deviceId">Идентификатор устройства</param>
        /// <param name="onActivate">Когда возникло событие, во время активации или во время
        /// декативации</param>
        /// <param name="success">Признак успешного выполения</param>
        public DeviceStatusEventArgs(string deviceId, bool onActivate, bool success)
            : base()
        {
            DeviceId = deviceId;
            OnActivate = onActivate;
            Success = success;
        }

        /// <summary>
        /// Идентификатор устройства
        /// </summary>
        public string DeviceId { get; }

        /// <summary>
        /// Когда возникло событие
        /// </summary>
        public bool OnActivate { get; }

        /// <summary>
        /// Признак успешного выполнения
        /// </summary>
        public bool Success { get; }
    }

    /// <summary>
    /// Диспечтер устройств
    /// </summary>
    public sealed class DeviceManager : EternalHostingTarget, IDeviceManager, ILogger, ISerialPortsPool
    {
        #region Константы

        private const string driverNotFoundMessage = 
            "Не найден драйвер для устройства \"{0}\\{1}\", тип устройства \"{2}\"";
        private const string deviceStatusSucceeded = "Успешное изменение статуса устройства \"{0}\". Текущий статус: \"{1}\"";
        private const string deviceStatusFailed = "Ошибка изменения статуса устройства \"{0}\". Текущий статус: \"{1}\". Текст ошибки: {2}";
        private const string deviceStatusActive = "Активно";
        private const string deviceStatusInactive = "Неактивно";
        private const string printerParamNotSet = "Параметр \"{0}\" устройства \"{1}\" не задан";
        private const string deviceTimeoutAfterActivate = "Таймаут после активации устройства";

        /// <summary>
        /// Источник событий
        /// </summary>
        public const string EventSource = "Диспетчер устройств";

        #endregion

        #region Поля

        // список типов во всех драйверных сборках
        private readonly ILookup<string, DeviceTypeHelper> deviceTypes;
        // таблица устройств
        private readonly Dictionary<string, DeviceHelper> devicesTable;
        // таблица клиентских сессий
        private readonly Dictionary<string, SessionHelper> sessionsTable;
        // обеспечивает потокобезопасный доступ к таблице устройств и таблице клиентов
        private readonly object dataLocker;
        // конфигурация диспетчера
        private DevmanProperties _props;
        // пул портов
        private Dictionary<string, SerialPortsHelper> _serialPortsPool;
        // обеспечивает потокобезопасный доступ к пулу портов
        private object _portsLocker;
        // ведение журнала событий
        private readonly IEventLink _eventLink;
        // трэкер
        private DeviceManagerTrackingHandler _tracker;

        #endregion

        #region Перегрузка методов базового класса

        /// <summary>
        /// Имя ремоутинг-объекта
        /// </summary>
        public override string Name
        {
            get { return "devicemanager"; }
        }

        /// <summary>
        /// Порт по умолчанию
        /// </summary>
        public override Int32 Port
        {
            get { return _props.ServicePort; }
        }

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public override void  Dispose()
        {
            DeactivateDevices();
            DestroySerialPortsPool();
            base.Dispose();
        }

        #endregion

        #region Статические методы

        /// <summary>
        /// Возвращает имя папки, в которой находится диспетчер устройств
        /// </summary>
        public static string GetDeviceManagerDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        /// Возвращает имя папки, в которой находится журнал событий диспетчера устройств
        /// </summary>
        public static string GetDeviceManagerLogDirectory()
        {
            return FileSystemHelper.GetSubDirectory(GetDeviceManagerDirectory(), "DeviceManagerLog");
        }

        /// <summary>
        /// Возвращает имя файла настроек диспетчера устройств
        /// </summary>
        public static string GetDeviceManagerSettingsFile()
        {
            return string.Format("{0}\\DeviceManager.xml", GetDeviceManagerDirectory());
        }

        /// <summary>
        /// Возвращает имя папки, в которой находится отладочная информация диспетчера устройств
        /// </summary>
        public static string GetDeviceManagerDebugDirectory()
        {
            return FileSystemHelper.GetSubDirectory(GetDeviceManagerDirectory(), "DeviceManagerDebugInfo");
        }

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="eventLink">Журнал событий</param>
        public DeviceManager(IEventLink eventLink)
        {
            if (eventLink == null)
                throw new ArgumentNullException("eventLink");

            // инициализация диспетчера
            _eventLink = eventLink;
            devicesTable = new Dictionary<string, DeviceHelper>();
            dataLocker = new object();
            sessionsTable = new Dictionary<string, SessionHelper>();

            // создание пула портов
            CreateSerialPortsPool();
            // загрузка драйверных сборок
            deviceTypes = LoadDeviceTypes(GetDeviceManagerDirectory());
            // загрузка конфигурации устройств
            LoadDevices(GetDeviceManagerSettingsFile());
            // атктивация устройств
            ActivateDevices();
        }

        #endregion

        #region Прочие методы

        /// <summary>
        /// Проверка имени последовательного порта
        /// </summary>
        /// <param name="portName"></param>
        private void CheckSerialPortName(string portName)
        {
            if (string.IsNullOrEmpty(portName))
                throw new ArgumentNullException("portName");

            if (!_serialPortsPool.ContainsKey(portName))
            {
                UpdateSerialPortsPool();
                if (!_serialPortsPool.ContainsKey(portName))
                {
                    string ports = string.Join("; ", _serialPortsPool.Keys.ToArray());
                    throw new InvalidOperationException(
                        string.Format("Последовательный порт \"{0}\" в системе не найден\n{1}", portName, ports));
                }
            }
        }

        private void ThrowPortIsBusy(string portName, string deviceName)
        {
            throw new InvalidOperationException(string.Format("Порт \"{0}\" занят устройством \"{1}\"",
                portName, deviceName));
        }

        /// <summary>
        /// Создание пула последовательных портов
        /// </summary>
        private void CreateSerialPortsPool()
        {
            _portsLocker = new object();
            _serialPortsPool = new Dictionary<string, SerialPortsHelper>();
            
            // заполняем пул портов
            UpdateSerialPortsPool();
        }

        /// <summary>
        /// Заполнение пула портов
        /// </summary>
        private void UpdateSerialPortsPool()
        {
            // заполняем пул портов
            var names = new List<string>();
            names.AddRange(SerialPortsEnumerator.Enumerate());
            names.AddRange(SerialPortsEnumerator.EnumerateLPT());
            foreach (string portName in names)
            {
                if (!_serialPortsPool.ContainsKey(portName))
                {
                    SerialPortsHelper helper = new SerialPortsHelper(portName);
                    _serialPortsPool.Add(portName, helper);
                }
            }
        }

        /// <summary>
        /// Уничтожение пула последовательных портов
        /// </summary>
        private void DestroySerialPortsPool()
        {
            if (_serialPortsPool == null)
                return;

            foreach (KeyValuePair<string, SerialPortsHelper> kvp in _serialPortsPool)
            {
                kvp.Value.Dispose();
            }
        }

        /// <summary>
        /// Загрузка драйверных сборок
        /// </summary>
        /// <param name="directoryName">Имя папки со сборками</param>
        private ILookup<string, DeviceTypeHelper> LoadDeviceTypes(string directoryName)
        {
            var fileNamePrefixesToSkip = new[]
            {
                "ERPService.SharedLibs",
                "DevicesBase",
                "DevicesCommon",
                "DevmanConfig"
            };

            return Directory.GetFiles(directoryName, "*.dll", SearchOption.AllDirectories)
                .Select(fileName =>
                {
                    // определяем имя файла без пути
                    var pureFileName = Path.GetFileName(fileName);

                    // если имя файла начинается с одного из префиксов, проускаем его и сборку не загружаем
                    if (fileNamePrefixesToSkip.Any(prefix => pureFileName.StartsWith(prefix)))
                    {
                        return null;
                    }

                    // пытаемся загрузить сборку с заданным именем файла
                    try
                    {
                        return Assembly.LoadFrom(fileName);
                    }
                    catch (FileLoadException)
                    {
                        return null;
                    }
                    catch (BadImageFormatException)
                    {
                        return null;
                    }
                })
                .Where(assembly => assembly != null)
                .SelectMany(assembly => assembly.GetExportedTypes())
                .Where(type => typeof(IDevice).IsAssignableFrom(type))
                .Select(type => new
                {
                    DeviceType = type,
                    DeviceAttribute = (DeviceAttribute)type.GetCustomAttributes(typeof(DeviceAttribute), false).FirstOrDefault()
                })
                .Where(projection => projection.DeviceAttribute != null)
                .ToLookup(projection => projection.DeviceAttribute.DeviceType, projection => new DeviceTypeHelper
                {
                    DeviceType = projection.DeviceType,
                    DeviceAttributeType = projection.DeviceAttribute.GetType()
                });
        }

        private void ThrowOnInvalidSession(string sessionId)
        {
            throw new DeviceManagerException(string.Format("Сессия {0} не зарегистрирована", sessionId));
        }

        /// <summary>
        /// Проверка существоавания сессии с заданным идентификатором
        /// </summary>
        /// <param name="sessionId">Идентификатор сессии</param>
        /// <param name="throwIfNotValid">Бросать исключение, если сессия не существует</param>
        private void ValidateSession(string sessionId, bool throwIfNotValid)
        {
            if (sessionsTable.ContainsKey(sessionId))
            {
                // сессия найдена
                SessionHelper helper = sessionsTable[sessionId];
                // фискируем время последнего обращения к диспетчеру из этой сессии
                helper.AccessDateTime = DateTime.Now;
            }
            else
            {
                if (throwIfNotValid)
                    // сессия не найдена и при этом задано сгенерировать исключение
                    ThrowOnInvalidSession(sessionId);
            }
        }

        /// <summary>
        /// Проверка актуальности клиентской сессии
        /// </summary>
        /// <param name="sessionId">Идентификатор сессии</param>
        private bool SessionAlive(string sessionId)
        {
            return sessionsTable.ContainsKey(sessionId) ? sessionsTable[sessionId].Alive : false;
        }

        /// <summary>
        /// Возвращает интерфейс устройства по его идентификатору
        /// </summary>
        /// <param name="deviceID">Идентификатор устройства</param>
        private DeviceHelper GetDeviceByID(string deviceID)
        {
            try
            {
                DeviceHelper helper = devicesTable[deviceID];
                return helper;
            }
            catch (KeyNotFoundException)
            {
                throw new DeviceNoFoundException(deviceID);
            }
        }

        private Boolean GetParamValue(XmlElement root, string paramName, object defaultValue, out string value)
        {
            XmlElement param = root[paramName];
            value = param != null ? param.InnerText : string.Empty;
            if (string.IsNullOrEmpty(value))
            {
                WriteEntry(
                    string.Format("Параметр \"{0}\" не найден в конфигурации.\nБудет использовано значение {1}.",
                        paramName, defaultValue),
                    EventLogEntryType.Warning);
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// Инициализация параметров подключаемого устройства
        /// </summary>
        /// <param name="deviceProps">Контейнер параметров устройства</param>
        /// <param name="device">Интерфейс устройства</param>
        private void InitConnectableDevice(ConnectableDeviceProperties deviceProps,
            ISerialDevice device)
        {
            // параметры связи
            device.PortName = deviceProps.Port;
            device.Baud = deviceProps.BaudRate;
        }

        /// <summary>
        /// Инициализация параметров подключаемого устройства
        /// </summary>
        /// <param name="deviceProps">Контейнер параметров устройства</param>
        /// <param name="device">Интерфейс устройства</param>
        private void InitPrintableDevice(PrintableDeviceProperties deviceProps, 
            IPrintableDevice device)
        {
            InitConnectableDevice(deviceProps, device);

            // прочие
            device.PrinterInfo.TapeWidth = deviceProps.TapeWidth;
            device.PrinterInfo.TopMargin = deviceProps.TopMargin;
            device.PrinterInfo.SlipFormLength = deviceProps.SlipHeight;
            device.PrinterInfo.Kind = deviceProps.PaperType;
            device.PrinterInfo.DsrFlowControl = deviceProps.DtrEnabled;

            // заголовок и подвал чека
            device.PrintFooter = deviceProps.PrintFooter;
            if (device.PrintFooter)
                device.DocumentFooter = deviceProps.Footer;

            device.PrintHeader = deviceProps.PrintHeader;
            if (device.PrintHeader)
                device.DocumentHeader = deviceProps.Header;

            device.PrintGraphicHeader = deviceProps.PrintGraphicHeader;
            if (device.PrintGraphicHeader)
                device.GraphicHeader = deviceProps.GraphicHeader;

            device.PrintGraphicFooter = deviceProps.PrintGraphicFooter;
            if (device.PrintGraphicFooter)
                device.GraphicFooter = deviceProps.GraphicFooter;
        }

        /// <summary>
        /// Создание ссылок для всех устройств заданного типа
        /// </summary>
        /// <typeparam name="TProps">Тип контейнера параметров</typeparam>
        /// <typeparam name="TIntf">Тип интерфейса устройства</typeparam>
        /// <param name="propsList">Список контейнеров параметров</param>
        /// <param name="attrFilter">Фильтр атрибутов реализации интерфейса устройства</param>
        /// <param name="initDeviceCallback">Делегат для инициализации заданного типа устройств</param>
        /// <param name="category">Категория устройств</param>
        private void LoadDevices<TProps, TIntf>(IEnumerable<TProps> propsList, IEnumerable<Type> attrFilter,
            Action<TProps, TIntf> initDeviceCallback, string category)
            where TProps : BaseDeviceProperties
            where TIntf : IDevice
        {
            // если список не содержит ни одного контейнера параметров устройства
            if (!propsList.Any())
                return;

            // просматриваем список контейнеров
            foreach (TProps deviceProps in propsList)
            {
                // поиск типа-реализации драйвера
                var deviceTypeHelper = deviceTypes.Contains(deviceProps.DeviceType) ?
                    deviceTypes[deviceProps.DeviceType].FirstOrDefault(helper => attrFilter.Contains(helper.DeviceAttributeType)) : null;

                if (deviceTypeHelper != null)
                {
                    // реализация найдена
                    // создаем экземпляр устройства
                    TIntf device = (TIntf)Activator.CreateInstance(deviceTypeHelper.DeviceType);

                    // задаем общие для всех устройств параметры
                    device.Logger = this;
                    device.DeviceId = deviceProps.DeviceId;

                    // если устройство унаследовано от CustomDevice
                    if (device is CustomDevice)
                    {
                        // инициализируем пул последовательных портов
                        (device as CustomDevice).SerialPortsPool = this;
                    }

                    // если требуется более детальная инициализация, вызываем ее
                    initDeviceCallback?.Invoke(deviceProps, device);

                    // добавляем устройство во внутренюю хэш-таблицу
                    devicesTable.Add(device.DeviceId, new DeviceHelper(device));
                }
                else
                {
                    // реализация не найдена, протоколируем
                    WriteEntry(string.Format(driverNotFoundMessage, category, deviceProps.DeviceId, deviceProps.DeviceType), EventLogEntryType.Warning);
                }
            }
        }

        /// <summary>
        /// Инициализация SMS-клиента
        /// </summary>
        /// <param name="props">Свойства</param>
        /// <param name="device">Интерфейс SMS-клиента</param>
        void InitSMSClient(SMSClientProperties props, ISMSClient device)
        {
            foreach (KeyValuePair<string, string> kvp in props.Settings)
            {
                device.SetConnectivityParam(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Инициализация считывателя
        /// </summary>
        /// <param name="props">Свойства</param>
        /// <param name="device">Интерфейс считывателя</param>
        void InitGenericReader(ReaderDeviceProperties props, IGenericReader device)
        {
            InitConnectableDevice(props, device);
            device.StopChar = props.StopChar;
            device.Parity = props.Parity;
        }

        void InitTurnstile(TurnstileProperties props, ITurnstileDevice device)
        {
            InitConnectableDevice(props, device);
            device.Address = props.Address;
            device.Direction = props.Direction;
            device.Timeout = props.Timeout;
        }

        /// <summary>
        /// Загрузка конфигурации диспетчера устройств
        /// </summary>
        /// <param name="configFileName">Имя файла конфигурации</param>
        private void LoadDevices(string configFileName)
        {
            // загрузка конфигурации
            _props = DevmanProperties.Load(configFileName);

            if (_props.DebugInfo)
            {
                // регистрируем трэкер
                _tracker = new DeviceManagerTrackingHandler(_eventLink, _props.DebugInfo);
            }

            // проверка версии конфигурации
            if (string.Compare(_props.Version, "1.0", true) != 0)
                throw new DeviceManagerConfigException(string.Format(
                    "Версия конфигурации {0} не поддерживается", _props.Version));

            // устанавливаем фильтр для поиска в таблице типов
            // это позволит создавать экземпляры устройств, чья реализация
            // помечена одним и только одним из перечисленных в фильтре атрибутов
            var typeFilter = new[] 
            {
                typeof(PrintableDeviceAttribute), 
                typeof(FiscalDeviceAttribute)
            };

            // создание ссылок на интерфейсы печатающих устройств
            LoadDevices<PrintableDeviceProperties, IPrintableDevice>(_props.PrintableDevices, typeFilter, InitPrintableDevice, 
                "Чековые принтеры");

            // создание ссылок на интерфейсы модулей управления бильярдом
            LoadDevices<BilliardManagerProperties, IBilliardsManagerDevice>(_props.BillardDevices, 
                new[] { typeof(BilliardsManagerAttribute) },
                InitConnectableDevice,
                "Бильярд");

            // создание ссылок на интерфейсы весов
            LoadDevices<ScalesDeviceProperties, IScaleDevice>(
                _props.ScalesDevices,
                new Type[] { typeof(ScaleAttribute) },
                delegate(ScalesDeviceProperties deviceProps, IScaleDevice device)
                {
                    // строка подключения к весам
                    device.ConnectionString = deviceProps.ConnectionString;
                },
                "Весы");

            // создание ссылок на интерфейсы дисплеев покупателей
            LoadDevices<DisplayDeviceProperties, ICustomerDisplay>(
                _props.DisplayDevices,
                new Type[] { typeof(CustomerDisplayAttribute) },
                InitConnectableDevice,
                "Дисплеи покупателя");

            // создание ссылок на интерфейсы SMS-клиентов
            LoadDevices<SMSClientProperties, ISMSClient>(
                _props.SMSClientDevices,
                new Type[] { typeof(SMSClientAttribute) },
                InitSMSClient,
                "SMS-клиенты");

            // считыватели, сканеры штрихкода
            LoadDevices<ReaderDeviceProperties, IGenericReader>(
                _props.ReaderDevices,
                new Type[] { typeof(GenericReaderAttribute) },
                InitGenericReader,
                "Считыватели");

            // турникеты
            LoadDevices<TurnstileProperties, ITurnstileDevice>(
                _props.TurnstileDevices,
                new Type[] { typeof(TurnstileDeviceAttribute) },
                InitTurnstile,
                "Турникеты");
        }

        /// <summary>
        /// Обработчик события, вызываемый при ативации или деактивации устройств
        /// </summary>
        public event EventHandler<DeviceStatusEventArgs> OnDeviceStatusChange;

        /// <summary>
        /// Активация всех устройств
        /// </summary>
        public void ActivateDevices()
        {
            ChangeDevicesStatus(true);
        }

        /// <summary>
        /// Деактивация всех устройств
        /// </summary>
        public void DeactivateDevices()
        {
            ChangeDevicesStatus(false);
        }

        /// <summary>
        /// Изменение состояния устройств
        /// </summary>
        /// <param name="activate">Аткивировать или деактивировать</param>
        private void ChangeDevicesStatus(bool activate)
        {
            foreach (DeviceHelper helper in devicesTable.Values)
            {
                try
                {
                    // меняем статус устройства
                    helper.Device.Active = activate;
                    if (activate)
                    {
                        // если происходит активация устройства, то допустимыми являются
                        // либо успешная активация, либо активация с ошибкой,
                        // специфичной для протокола обмена с устройством
                        if (helper.Device.ErrorCode.Value != GeneralError.Success &&
                            helper.Device.ErrorCode.Value != GeneralError.Specific)
                        {
                            // меняем статус на неактивный
                            if (helper.Device.Active)
                                helper.Device.Active = false;
                            // переходим к следующему устройству
                            continue;
                        }
                    }

                    // вызов делегата для протоколирования изменения статуса устройства
                    if (OnDeviceStatusChange != null)
                        OnDeviceStatusChange(this, new DeviceStatusEventArgs(helper.Device.DeviceId,
                            activate, helper.Device.ErrorCode.Succeeded));
                    else
                    {
                        // протоколируем в лог
                        string statusMessage = string.Format(deviceStatusSucceeded,
                                helper.Device.DeviceId, 
                                helper.Device.Active ? deviceStatusActive : deviceStatusInactive);

                        WriteEntry(statusMessage, EventLogEntryType.Information);
                    }
                }
                catch (Exception e)
                {
                    // изменение статуса прошло с ошибкой
                    if (OnDeviceStatusChange != null)
                        OnDeviceStatusChange(this, new DeviceStatusEventArgs(helper.Device.DeviceId,
                            activate, false));
                    else
                    {
                        string statusMessage = string.Format(deviceStatusFailed,
                                    helper.Device.DeviceId,
                                    helper.Device.Active ? deviceStatusActive : deviceStatusInactive,
                                    e.Message);

                        // протоколируем в лог
                        WriteEntry(statusMessage, EventLogEntryType.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Получить монопольный доступ к устройству
        /// </summary>
        /// <param name="sessionID">Идентификатор сессии</param>
        /// <param name="deviceID">Идентификатор устройства</param>
        private bool PerformCapture(string sessionID, string deviceID)
        {
            lock (dataLocker)
            {
                DeviceHelper helper = GetDeviceByID(deviceID);
                if (helper.Captured)
                {
                    // устройство захвачено
                    // проверим, жива ли сессия, захватившая устройство
                    if (SessionAlive(helper.SessionID))
                    {
                        // да, жива, сравним идентификаторы
                        return (helper.SessionID == sessionID);
                    }
                    else
                    {
                        // сессия метрвая, удаляем ее
                        if (sessionsTable.ContainsKey(helper.SessionID))
                            sessionsTable.Remove(helper.SessionID);

                        // освобождаем устройство
                        helper.Release();
                    }
                }

                // устройство свободно, проверим идентификатор новой сессии
                ValidateSession(sessionID, true);
                // захват устройства
                helper.SessionID = sessionID;
                return true;
            }
        }

        #endregion

        #region Реализация интерфейса IDeviceManager

        #region Работа с устройствами

        /// <summary>
        /// Получить монопольный доступ к устройству
        /// </summary>
        /// <param name="sessionID">Идентификатор сессии</param>
        /// <param name="deviceID">Идентификатор устройства</param>
        /// <param name="waitTimeout">Таймаут ожидания захвата устройства, секунды</param>
        public bool Capture(string sessionID, string deviceID, Int32 waitTimeout)
        {
            bool isCaptured;
            // засекаем время
            DateTime fixedTime = DateTime.Now;

            do
            {
                // пытаемся захватить устройство
                isCaptured = PerformCapture(sessionID, deviceID);
                // если устройство не захвачено и задано время ожидания
                if (!isCaptured && waitTimeout != WaitConstant.None)
                    // приостанавливаемся перед следующей попыткой
                    System.Threading.Thread.Sleep(0);
            }
            while (!isCaptured && DateTime.Now.Subtract(fixedTime).Seconds < waitTimeout);
            return isCaptured;
        }

        /// <summary>
        /// Освободить устройство
        /// </summary>
        /// <param name="sessionID">Идентификатор сессии</param>
        /// <param name="deviceID">Идентификатор устройства</param>
        public bool Release(string sessionID, string deviceID)
        {
            lock (dataLocker)
            {
                ValidateSession(sessionID, false);
                DeviceHelper helper = GetDeviceByID(deviceID);
                if (!helper.Captured || helper.SessionID == sessionID)
                {
                    // устройство свободно или захвачено этим клиентом
                    helper.Release();
                    return true;
                }

                // устройство захвачено другим клиентом
                return false;
            }
        }

        /// <summary>
        /// Базовый интерфейс устройства
        /// </summary>
        /// <param name="sessionID">Идентификатор сессии</param>
        /// <param name="deviceID">Идентификатор устройства</param>
        public IDevice GetDevice(string sessionID, string deviceID)
        {
            lock (dataLocker)
            {
                ValidateSession(sessionID, true);
                DeviceHelper helper = GetDeviceByID(deviceID);
                if (helper.Captured && helper.SessionID != sessionID)
                    throw new DeviceManagerException(
                        string.Format("Ошибка доступа к устройству {0}. Устройство захвачено клиентом {1}",
                        deviceID, helper.SessionID));

                // возвращаем ссылку на базовый интерфейс устройства
                return helper.Device;
            }
        }

        #endregion

        #region Работа с клиентскими сессиями

        /// <summary>
        /// Регистрация клиентской сессии в диспетчере устройств
        /// </summary>
        /// <param name="sessionID">Идентификатор сессии</param>
        /// <returns>true, если сессия успешно зарегистрирована</returns>
        public void Login(out string sessionID)
        {
            lock (dataLocker)
            {
                if (_props.MaxSessionsCount > 0 && sessionsTable.Count >= _props.MaxSessionsCount)
                    throw new DeviceManagerException(
                        string.Format("Достигнуто максимальное количество клиентских сессий - {0}.",
                        _props.MaxSessionsCount));

                // создаем новый идентификатор сессии
                sessionID = Guid.NewGuid().ToString();
                // регистрируем ее
                sessionsTable.Add(sessionID, new SessionHelper(sessionID, _props.SessionTimeout));
            }
        }

        /// <summary>
        /// Завершение клиентской сессии в диспетчере устройств
        /// </summary>
        /// <param name="sessionID">Идентификатор сессии</param>
        public void Logout(string sessionID)
        {
            lock (dataLocker)
            {
                // проверяем корректность идентификатора сессии
                ValidateSession(sessionID, false);
                
                // освобождаем все устройства, захваченные этой сессией
                foreach (DeviceHelper helper in devicesTable.Values)
                {
                    if (helper.SessionID == sessionID)
                        helper.Release();
                }
                
                // удаляем сессию
                if (sessionsTable.ContainsKey(sessionID))
                    sessionsTable.Remove(sessionID);
            }
        }

        #endregion

        #endregion

        #region Реализация интерфейса ILogger

        /// <summary>
        /// Добавляет запись в протокол работы приложения
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        /// <param name="type">Тип сообщения</param>
        public void WriteEntry(string message, EventLogEntryType type)
        {
            // конвертируем тип события
            // тип EventLogEntryType оставлен для обратной совместимости
            EventType nativeEventType = EventType.Undefined;
            switch (type)
            {
                case EventLogEntryType.Error:
                    nativeEventType = EventType.Error;
                    break;
                case EventLogEntryType.Information:
                    nativeEventType = EventType.Information;
                    break;
                case EventLogEntryType.Warning:
                    nativeEventType = EventType.Warning;
                    break;
            }

            _eventLink.Post(EventSource, nativeEventType, message);
        }

        /// <summary>
        /// Добавляет запись в протокол работы приложения
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        /// <param name="type">Тип сообщения</param>
        /// <param name="sender">Устройство, которое пишет в лог</param>
        public void WriteEntry(IDevice sender, string message, EventLogEntryType type)
        {
            WriteEntry(string.Format("Устройство [{0}]: {1}", sender.DeviceId, message), type);
        }

        /// <summary>
        /// Режим отладки
        /// </summary>
        public Boolean DebugInfo
        {
            get { return _props.DebugInfo; }
        }

        /// <summary>
        /// Сохранение отладочной информации
        /// </summary>
        /// <param name="sender">Устройство, которому нужно сохранить отладочную информацию</param>
        /// <param name="info">Отладочная информация</param>
        public void SaveDebugInfo(IDevice sender, string info)
        {
            // формируем имя файла для хранения отладдочной информации
            string debugFileName = string.Format("{0}\\{1}.dmp",
                FileSystemHelper.GetSubDirectory(GetDeviceManagerDebugDirectory(), sender.DeviceId),
                DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));

            using (StreamWriter debugWriter = new StreamWriter(debugFileName, false, Encoding.Default))
            {
                debugWriter.WriteLine(string.Format("Дата и время: {0}",
                    DateTime.Now.ToString("yyyy MMM dd, HH:mm:ss")));
                debugWriter.WriteLine(string.Format("Устройство: {0}", sender.DeviceId));
                debugWriter.WriteLine(string.Empty);
                debugWriter.WriteLine("Отладочная информация:");
                debugWriter.WriteLine(string.Empty);
                debugWriter.WriteLine(info);
            }
        }

        #endregion

        #region Реализация интерфейса ISerialPortsPool

        /// <summary>
        /// Получить доступ к порту из пула
        /// </summary>
        /// <param name="deviceId">Идентификатор устройства</param>
        /// <param name="portName">Имя порта</param>
        /// <returns>Порт из пула</returns>
        public EasyCommunicationPort GetPort(string deviceId, string portName)
        {
            CheckSerialPortName(portName);
            SerialPortsHelper helper = _serialPortsPool[portName];
            if (string.Compare(helper.DeviceId, deviceId) == 0)
                return helper.Port;
            else
            {
                ThrowPortIsBusy(portName, helper.DeviceId);
                return null;
            }
        }

        /// <summary>
        /// Захватить коммуникационный порт
        /// </summary>
        /// <param name="deviceId">Идентификатор устройства</param>
        /// <param name="portName">Имя порта</param>
        /// <param name="waitIfCaptured">Ожидать освобождения порта</param>
        /// <param name="waitTime">Время, в течение которого ожидать освобождение</param>
        public EasyCommunicationPort CapturePort(string deviceId, string portName,
            Boolean waitIfCaptured, TimeSpan waitTime)
        {
            CheckSerialPortName(portName);
            SerialPortsHelper helper = _serialPortsPool[portName];

            // попытка захвата порта
            TimeSpan timeElapsed = new TimeSpan();
            do
            {
                if (string.IsNullOrEmpty(helper.DeviceId) || string.Compare(helper.DeviceId, deviceId) == 0)
                {
                    lock (_portsLocker)
                    {
                        helper.DeviceId = deviceId;
                    }
                    return helper.Port;
                }
                else if (waitIfCaptured)
                {
                    // ждем освобождения порта
                    Thread.Sleep(50);
                    TimeSpan ts = new TimeSpan(0, 0, 0, 0, 50);
                    timeElapsed.Add(ts);
                }
            }
            while (waitIfCaptured && (timeElapsed < waitTime));

            // порт занят другим устройством
            ThrowPortIsBusy(portName, helper.DeviceId);
            return null;
        }

        /// <summary>
        /// Захватить коммуникационный порт. Ожидать захвата порта в течение 
        /// бесконечного интервала времени
        /// </summary>
        /// <param name="deviceId">Идентификатор устройства</param>
        /// <param name="portName">Имя порта</param>
        public EasyCommunicationPort CapturePort(string deviceId, string portName)
        {
            return CapturePort(deviceId, portName, true, TimeSpan.MaxValue);
        }

        /// <summary>
        /// Освободить коммуникационный порт
        /// </summary>
        /// <param name="deviceId">Идентификатор устройства</param>
        /// <param name="portName">Имя порта</param>
        public void ReleasePort(string deviceId, string portName)
        {
            CheckSerialPortName(portName);
            SerialPortsHelper helper = _serialPortsPool[portName];

            // попытка освобождение порта
            if (string.IsNullOrEmpty(helper.DeviceId) || string.Compare(helper.DeviceId, deviceId) == 0)
            {
                lock (_portsLocker)
                {
                    helper.DeviceId = string.Empty;
                }
            }
            else
                // порт занят другим устройством
                ThrowPortIsBusy(portName, helper.DeviceId);
        }

        #endregion
    }
}
