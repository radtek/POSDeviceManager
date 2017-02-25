using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon;
using DevicesCommon.Helpers;
using System.Threading;
using System.Collections;
using System.Linq;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Diagnostics;
using DevicesCommon.Connectors;
using DevmanConfig;
using DevicesBase.Helpers;
using ERPService.SharedLibs.Remoting;
using ERPService.SharedLibs.Helpers;
using ERPService.SharedLibs.Helpers.SerialCommunications;
using ERPService.SharedLibs.Eventlog;

namespace DevicesBase
{
    /// <summary>
    /// ��������� ������� ���������/����������� ����������
    /// </summary>
    public class DeviceStatusEventArgs : EventArgs
    {
        private String deviceId;
        private bool onActivate;
        private bool success;

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="deviceId">������������� ����������</param>
        /// <param name="onActivate">����� �������� �������, �� ����� ��������� ��� �� �����
        /// �����������</param>
        /// <param name="success">������� ��������� ���������</param>
        public DeviceStatusEventArgs(String deviceId, bool onActivate, bool success)
            : base()
        {
            this.deviceId = deviceId;
            this.onActivate = onActivate;
            this.success = success;
        }

        /// <summary>
        /// ������������� ����������
        /// </summary>
        public String DeviceId
        {
            get
            {
                return deviceId;
            }
        }

        /// <summary>
        /// ����� �������� �������
        /// </summary>
        public bool OnActivate
        {
            get
            {
                return onActivate;
            }
        }

        /// <summary>
        /// ������� ��������� ����������
        /// </summary>
        public bool Success
        {
            get
            {
                return success;
            }
        }
    }

    /// <summary>
    /// ��������� ���������
    /// </summary>
    public sealed class DeviceManager : EternalHostingTarget, IDeviceManager, ILogger,
        ISerialPortsPool
    {
        #region ���������

        private const String driverNotFoundMessage = 
            "�� ������ ������� ��� ���������� \"{0}\\{1}\", ��� ���������� \"{2}\"";
        private const String deviceStatusSucceeded = "�������� ��������� ������� ���������� \"{0}\". ������� ������: \"{1}\"";
        private const String deviceStatusFailed = "������ ��������� ������� ���������� \"{0}\". ������� ������: \"{1}\". ����� ������: {2}";
        private const String deviceStatusActive = "�������";
        private const String deviceStatusInactive = "���������";
        private const String printerParamNotSet = "�������� \"{0}\" ���������� \"{1}\" �� �����";
        private const String deviceTimeoutAfterActivate = "������� ����� ��������� ����������";

        /// <summary>
        /// �������� �������
        /// </summary>
        public const String EventSource = "��������� ���������";

        #endregion

        #region ����

        // ������ ����� �� ���� ���������� �������
        List<Type> deviceTypes;
        // ������� ���������
        Dictionary<String, DeviceHelper> devicesTable;
        // ������� ���������� ������
        Dictionary<String, SessionHelper> sessionsTable;
        // ������������ ���������������� ������ � ������� ��������� � ������� ��������
        Object dataLocker;
        // ������������ ����������
        DevmanProperties _props;
        // ��� ������
        Dictionary<String, SerialPortsHelper> _serialPortsPool;
        // ������������ ���������������� ������ � ���� ������
        Object _portsLocker;
        // ������� ������� �������
        IEventLink _eventLink;
        // ������
        DeviceManagerTrackingHandler _tracker;

        #endregion

        #region ���������� ������� �������� ������

        /// <summary>
        /// ��� ���������-�������
        /// </summary>
        public override String Name
        {
            get { return "devicemanager"; }
        }

        /// <summary>
        /// ���� �� ���������
        /// </summary>
        public override Int32 Port
        {
            get { return _props.ServicePort; }
        }

        /// <summary>
        /// ������������ ��������
        /// </summary>
        public override void  Dispose()
        {
            DeactivateDevices();
            DestroySerialPortsPool();
            base.Dispose();
        }

        #endregion

        #region ����������� ������

        /// <summary>
        /// ���������� ��� �����, � ������� ��������� ��������� ���������
        /// </summary>
        public static String GetDeviceManagerDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        /// ���������� ��� �����, � ������� ��������� ������ ������� ���������� ���������
        /// </summary>
        public static String GetDeviceManagerLogDirectory()
        {
            return FileSystemHelper.GetSubDirectory(GetDeviceManagerDirectory(), "DeviceManagerLog");
        }

        /// <summary>
        /// ���������� ��� ����� �������� ���������� ���������
        /// </summary>
        public static String GetDeviceManagerSettingsFile()
        {
            return String.Format("{0}\\DeviceManager.xml", GetDeviceManagerDirectory());
        }

        /// <summary>
        /// ���������� ��� �����, � ������� ��������� ���������� ���������� ���������� ���������
        /// </summary>
        public static String GetDeviceManagerDebugDirectory()
        {
            return FileSystemHelper.GetSubDirectory(GetDeviceManagerDirectory(), "DeviceManagerDebugInfo");
        }

        #endregion

        #region �����������

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="eventLink">������ �������</param>
        public DeviceManager(IEventLink eventLink)
        {
            if (eventLink == null)
                throw new ArgumentNullException("eventLink");

            // ������������� ����������
            _eventLink = eventLink;
            deviceTypes = new List<Type>();
            devicesTable = new Dictionary<String, DeviceHelper>();
            dataLocker = new Object();
            sessionsTable = new Dictionary<String, SessionHelper>();

            // �������� ���� ������
            CreateSerialPortsPool();
            // �������� ���������� ������
            LoadAssemblies(GetDeviceManagerDirectory());
            // �������� ������������ ���������
            LoadDevices(GetDeviceManagerSettingsFile());
            // ���������� ���������
            ActivateDevices();
        }

        #endregion

        #region ������ ������

        /// <summary>
        /// �������� ����� ����������������� �����
        /// </summary>
        /// <param name="portName"></param>
        private void CheckSerialPortName(String portName)
        {
            if (String.IsNullOrEmpty(portName))
                throw new ArgumentNullException("portName");

            if (!_serialPortsPool.ContainsKey(portName))
            {
                UpdateSerialPortsPool();
                if (!_serialPortsPool.ContainsKey(portName))
                {
                    string ports = string.Join("; ", _serialPortsPool.Keys.ToArray());
                    throw new InvalidOperationException(
                        String.Format("���������������� ���� \"{0}\" � ������� �� ������\n{1}", portName, ports));
                }
            }
        }

        private void ThrowPortIsBusy(String portName, String deviceName)
        {
            throw new InvalidOperationException(String.Format("���� \"{0}\" ����� ����������� \"{1}\"",
                portName, deviceName));
        }

        /// <summary>
        /// �������� ���� ���������������� ������
        /// </summary>
        private void CreateSerialPortsPool()
        {
            _portsLocker = new Object();
            _serialPortsPool = new Dictionary<String, SerialPortsHelper>();
            
            // ��������� ��� ������
            UpdateSerialPortsPool();
        }

        /// <summary>
        /// ���������� ���� ������
        /// </summary>
        private void UpdateSerialPortsPool()
        {
            // ��������� ��� ������
            var names = new List<String>();
            names.AddRange(SerialPortsEnumerator.Enumerate());
            names.AddRange(SerialPortsEnumerator.EnumerateLPT());
            foreach (String portName in names)
            {
                if (!_serialPortsPool.ContainsKey(portName))
                {
                    SerialPortsHelper helper = new SerialPortsHelper(portName);
                    _serialPortsPool.Add(portName, helper);
                }
            }
        }

        /// <summary>
        /// ����������� ���� ���������������� ������
        /// </summary>
        private void DestroySerialPortsPool()
        {
            if (_serialPortsPool == null)
                return;

            foreach (KeyValuePair<String, SerialPortsHelper> kvp in _serialPortsPool)
            {
                kvp.Value.Dispose();
            }
        }

        /// <summary>
        /// �������� ���������� ������
        /// </summary>
        /// <param name="directoryName">��� ����� �� ��������</param>
        private void LoadAssemblies(String directoryName)
        {
            String[] fileNames = Directory.GetFiles(directoryName, "*.dll", SearchOption.AllDirectories);
            String thisName = Assembly.GetExecutingAssembly().Location;
            foreach (String fileName in fileNames)
            {
                // ��� ����� �� ��������� � ������� � ����� ���������� "dll"
                // ���������������� ��� - ������, �������� �� ���������
                if (String.Compare(thisName, fileName, true) != 0)
                {
                    try
                    {
                        // ��������� ������
                        Assembly a = Assembly.LoadFrom(fileName);
                        // �������� ������ �����, ������������ � ������
                        Type[] types = a.GetTypes();
                        foreach (Type t in types)
                        {
                            // �������� ������ ���������, ������������ ��� ����
                            Attribute[] attribs = Attribute.GetCustomAttributes(t);
                            // ���� ��� ������� ����������
                            if (attribs.Length > 0)
                                // ������������� ������ ���������
                                foreach (Attribute attrib in attribs)
                                {
                                    if (attrib is DeviceAttribute)
                                    {
                                        // ��������� ��� � ������ �����
                                        deviceTypes.Add(t);
                                        // ��������� � ������� ����
                                        break;
                                    }
                                }
                        }
                    }
                    catch
                    {
                        // �� ������� ��������� ������ �� �����-�� ��������
                        // ������ ���������� ����������
                    }
                }
            }
        }

        private void ThrowOnInvalidSession(String sessionId)
        {
            throw new DeviceManagerException(String.Format("������ {0} �� ����������������", sessionId));
        }

        /// <summary>
        /// �������� �������������� ������ � �������� ���������������
        /// </summary>
        /// <param name="sessionId">������������� ������</param>
        /// <param name="throwIfNotValid">������� ����������, ���� ������ �� ����������</param>
        private void ValidateSession(String sessionId, bool throwIfNotValid)
        {
            if (sessionsTable.ContainsKey(sessionId))
            {
                // ������ �������
                SessionHelper helper = sessionsTable[sessionId];
                // ��������� ����� ���������� ��������� � ���������� �� ���� ������
                helper.AccessDateTime = DateTime.Now;
            }
            else
            {
                if (throwIfNotValid)
                    // ������ �� ������� � ��� ���� ������ ������������� ����������
                    ThrowOnInvalidSession(sessionId);
            }
        }

        /// <summary>
        /// �������� ������������ ���������� ������
        /// </summary>
        /// <param name="sessionId">������������� ������</param>
        private bool SessionAlive(String sessionId)
        {
            return sessionsTable.ContainsKey(sessionId) ? sessionsTable[sessionId].Alive : false;
        }

        #region ������, ������������ ���������� ��� �������� ������ �� ���������� ���������

        /// <summary>
        /// ���������� ���������� ��� �������� ������ �� ��������� �������� ����������
        /// </summary>
        /// <param name="deviceType">��� ���������� �� ������������ (��������, "�����")</param>
        /// <param name="requiredTypes">������-������ �����-���������, �������� ������ ���� ��������
        /// ����-���������� ��������� ���������</param>
        private Type FindDeviceType(String deviceType, Type[] requiredTypes)
        {
            // ������������� ���� ������ �����-��������� ���������, 
            // ������������ � ����������� �������
            foreach (Type t in deviceTypes)
            {
                // ������������� ������-������ ���������
                foreach (Type attribType in requiredTypes)
                {
                    // ��� ���� ��������� ������� �� ������-�������
                    if (Attribute.IsDefined(t, attribType))
                    {
                        DeviceAttribute attrib = (DeviceAttribute)Attribute.GetCustomAttribute(t, attribType);
                        if (String.Compare(attrib.DeviceType, deviceType, true) == 0)
                            // ������ ������ �������
                            return t;
                    }
                }
            }

            // � ������ ��������� ��� ������� �������� ����������
            return null;
        }

        #endregion

        /// <summary>
        /// ���������� ��������� ���������� �� ��� ��������������
        /// </summary>
        /// <param name="deviceID">������������� ����������</param>
        private DeviceHelper GetDeviceByID(String deviceID)
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

        private Boolean GetParamValue(XmlElement root, String paramName, Object defaultValue, out String value)
        {
            XmlElement param = root[paramName];
            value = param != null ? param.InnerText : String.Empty;
            if (String.IsNullOrEmpty(value))
            {
                WriteEntry(
                    String.Format("�������� \"{0}\" �� ������ � ������������.\n����� ������������ �������� {1}.",
                        paramName, defaultValue),
                    EventLogEntryType.Warning);
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// ������������� ���������� ������������� ����������
        /// </summary>
        /// <param name="deviceProps">��������� ���������� ����������</param>
        /// <param name="device">��������� ����������</param>
        private void InitConnectableDevice(ConnectableDeviceProperties deviceProps,
            ISerialDevice device)
        {
            // ��������� �����
            device.PortName = deviceProps.Port;
            device.Baud = deviceProps.BaudRate;
        }

        /// <summary>
        /// ������������� ���������� ������������� ����������
        /// </summary>
        /// <param name="deviceProps">��������� ���������� ����������</param>
        /// <param name="device">��������� ����������</param>
        private void InitPrintableDevice(PrintableDeviceProperties deviceProps, 
            IPrintableDevice device)
        {
            InitConnectableDevice(deviceProps, device);

            // ������
            device.PrinterInfo.TapeWidth = deviceProps.TapeWidth;
            device.PrinterInfo.TopMargin = deviceProps.TopMargin;
            device.PrinterInfo.SlipFormLength = deviceProps.SlipHeight;
            device.PrinterInfo.Kind = deviceProps.PaperType;
            device.PrinterInfo.DsrFlowControl = deviceProps.DtrEnabled;

            // ��������� � ������ ����
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
        /// ������� ��� ������������� ���������� ���������� ������������� ����
        /// </summary>
        /// <typeparam name="TProps">��� ���������� ����������</typeparam>
        /// <typeparam name="TIntf">��� ���������� ����������</typeparam>
        /// <param name="deviceProps">��������� ����������</param>
        /// <param name="device">��������� ����������</param>
        private delegate void InitDevice<TProps, TIntf>(TProps deviceProps, TIntf device)
            where TProps : BaseDeviceProperties
            where TIntf : IDevice;

        /// <summary>
        /// �������� ������ ��� ���� ��������� ��������� ����
        /// </summary>
        /// <typeparam name="TProps">��� ���������� ����������</typeparam>
        /// <typeparam name="TIntf">��� ���������� ����������</typeparam>
        /// <param name="propsList">������ ����������� ����������</param>
        /// <param name="attrFilter">������ ��������� ���������� ���������� ����������</param>
        /// <param name="initDeviceCallback">������� ��� ������������� ��������� ���� ���������</param>
        /// <param name="category">��������� ���������</param>
        private void LoadDevices<TProps, TIntf>(IList<TProps> propsList, Type[] attrFilter,
            InitDevice<TProps, TIntf> initDeviceCallback, String category)
            where TProps : BaseDeviceProperties
            where TIntf : IDevice
        {
            // ���� ������ �� �������� �� ������ ���������� ���������� ����������
            if (propsList.Count == 0)
                return;

            // ������������� ������ �����������
            foreach (TProps deviceProps in propsList)
            {
                // ����� ����-���������� ��������
                Type t = FindDeviceType(deviceProps.DeviceType, attrFilter);

                if (t != null)
                {
                    // ���������� �������
                    // ������� ��������� ����������
                    TIntf device = (TIntf)Activator.CreateInstance(t);

                    // ������ ����� ��� ���� ��������� ���������
                    device.Logger = this;
                    device.DeviceId = deviceProps.DeviceId;

                    // ���� ���������� ������������ �� CustomDevice
                    if (device is CustomDevice)
                    {
                        // �������������� ��� ���������������� ������
                        (device as CustomDevice).SerialPortsPool = this;
                    }

                    // ���� ��������� ����� ��������� �������������
                    if (initDeviceCallback != null)
                        // �������� ��
                        initDeviceCallback(deviceProps, device);

                    // ��������� ���������� �� ��������� ���-�������
                    devicesTable.Add(device.DeviceId, new DeviceHelper(device));
                }
                else
                {
                    // ���������� �� �������
                    String s = String.Format(
                        driverNotFoundMessage,
                        category,
                        deviceProps.DeviceId,
                        deviceProps.DeviceType);

                    // ������������� ���������
                    WriteEntry(s, EventLogEntryType.Warning);
                }
            }
        }

        /// <summary>
        /// ������������� SMS-�������
        /// </summary>
        /// <param name="props">��������</param>
        /// <param name="device">��������� SMS-�������</param>
        void InitSMSClient(SMSClientProperties props, ISMSClient device)
        {
            foreach (KeyValuePair<String, String> kvp in props.Settings)
            {
                device.SetConnectivityParam(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// ������������� �����������
        /// </summary>
        /// <param name="props">��������</param>
        /// <param name="device">��������� �����������</param>
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
        /// �������� ������������ ���������� ���������
        /// </summary>
        /// <param name="configFileName">��� ����� ������������</param>
        private void LoadDevices(String configFileName)
        {
            // �������� ������������
            _props = DevmanProperties.Load(configFileName);

            if (_props.DebugInfo)
            {
                // ������������ ������
                _tracker = new DeviceManagerTrackingHandler(_eventLink, _props.DebugInfo);
            }

            // �������� ������ ������������
            if (String.Compare(_props.Version, "1.0", true) != 0)
                throw new DeviceManagerConfigException(String.Format(
                    "������ ������������ {0} �� ��������������", _props.Version));

            // ������������� ������ ��� ������ � ������� �����
            // ��� �������� ��������� ���������� ���������, ��� ����������
            // �������� ����� � ������ ����� �� ������������� � ������� ���������
            Type[] typeFilter = new Type[] { typeof(PrintableDeviceAttribute), 
                    typeof(FiscalDeviceAttribute) };

            // �������� ������ �� ���������� ���������� ���������
            LoadDevices<PrintableDeviceProperties, IPrintableDevice>(
                _props.PrintableDevices, typeFilter, InitPrintableDevice, 
                "������� ��������");

            // �������� ������ �� ���������� ������� ���������� ���������
            LoadDevices<BilliardManagerProperties, IBilliardsManagerDevice>(
                _props.BillardDevices,
                new Type[] { typeof(BilliardsManagerAttribute) },
                InitConnectableDevice,
                "�������");

            // �������� ������ �� ���������� �����
            LoadDevices<ScalesDeviceProperties, IScaleDevice>(
                _props.ScalesDevices,
                new Type[] { typeof(ScaleAttribute) },
                delegate(ScalesDeviceProperties deviceProps, IScaleDevice device)
                {
                    // ������ ����������� � �����
                    device.ConnectionString = deviceProps.ConnectionString;
                },
                "����");

            // �������� ������ �� ���������� �������� �����������
            LoadDevices<DisplayDeviceProperties, ICustomerDisplay>(
                _props.DisplayDevices,
                new Type[] { typeof(CustomerDisplayAttribute) },
                InitConnectableDevice,
                "������� ����������");

            // �������� ������ �� ���������� SMS-��������
            LoadDevices<SMSClientProperties, ISMSClient>(
                _props.SMSClientDevices,
                new Type[] { typeof(SMSClientAttribute) },
                InitSMSClient,
                "SMS-�������");

            // �����������, ������� ���������
            LoadDevices<ReaderDeviceProperties, IGenericReader>(
                _props.ReaderDevices,
                new Type[] { typeof(GenericReaderAttribute) },
                InitGenericReader,
                "�����������");

            // ���������
            LoadDevices<TurnstileProperties, ITurnstileDevice>(
                _props.TurnstileDevices,
                new Type[] { typeof(TurnstileDeviceAttribute) },
                InitTurnstile,
                "���������");
        }

        /// <summary>
        /// ���������� �������, ���������� ��� �������� ��� ����������� ���������
        /// </summary>
        public event EventHandler<DeviceStatusEventArgs> OnDeviceStatusChange;

        /// <summary>
        /// ��������� ���� ���������
        /// </summary>
        public void ActivateDevices()
        {
            ChangeDevicesStatus(true);
        }

        /// <summary>
        /// ����������� ���� ���������
        /// </summary>
        public void DeactivateDevices()
        {
            ChangeDevicesStatus(false);
        }

        /// <summary>
        /// ��������� ��������� ���������
        /// </summary>
        /// <param name="activate">������������ ��� ��������������</param>
        private void ChangeDevicesStatus(bool activate)
        {
            foreach (DeviceHelper helper in devicesTable.Values)
            {
                try
                {
                    // ������ ������ ����������
                    helper.Device.Active = activate;
                    if (activate)
                    {
                        // ���� ���������� ��������� ����������, �� ����������� ��������
                        // ���� �������� ���������, ���� ��������� � �������,
                        // ����������� ��� ��������� ������ � �����������
                        if (helper.Device.ErrorCode.Value != GeneralError.Success &&
                            helper.Device.ErrorCode.Value != GeneralError.Specific)
                        {
                            // ������ ������ �� ����������
                            if (helper.Device.Active)
                                helper.Device.Active = false;
                            // ��������� � ���������� ����������
                            continue;
                        }
                    }

                    // ����� �������� ��� ���������������� ��������� ������� ����������
                    if (OnDeviceStatusChange != null)
                        OnDeviceStatusChange(this, new DeviceStatusEventArgs(helper.Device.DeviceId,
                            activate, helper.Device.ErrorCode.Succeeded));
                    else
                    {
                        // ������������� � ���
                        String statusMessage = String.Format(deviceStatusSucceeded,
                                helper.Device.DeviceId, 
                                helper.Device.Active ? deviceStatusActive : deviceStatusInactive);

                        WriteEntry(statusMessage, EventLogEntryType.Information);
                    }
                }
                catch (Exception e)
                {
                    // ��������� ������� ������ � �������
                    if (OnDeviceStatusChange != null)
                        OnDeviceStatusChange(this, new DeviceStatusEventArgs(helper.Device.DeviceId,
                            activate, false));
                    else
                    {
                        String statusMessage = String.Format(deviceStatusFailed,
                                    helper.Device.DeviceId,
                                    helper.Device.Active ? deviceStatusActive : deviceStatusInactive,
                                    e.Message);

                        // ������������� � ���
                        WriteEntry(statusMessage, EventLogEntryType.Error);
                    }
                }
            }
        }

        /// <summary>
        /// �������� ����������� ������ � ����������
        /// </summary>
        /// <param name="sessionID">������������� ������</param>
        /// <param name="deviceID">������������� ����������</param>
        private bool PerformCapture(string sessionID, string deviceID)
        {
            lock (dataLocker)
            {
                DeviceHelper helper = GetDeviceByID(deviceID);
                if (helper.Captured)
                {
                    // ���������� ���������
                    // ��������, ���� �� ������, ����������� ����������
                    if (SessionAlive(helper.SessionID))
                    {
                        // ��, ����, ������� ��������������
                        return (helper.SessionID == sessionID);
                    }
                    else
                    {
                        // ������ �������, ������� ��
                        if (sessionsTable.ContainsKey(helper.SessionID))
                            sessionsTable.Remove(helper.SessionID);

                        // ����������� ����������
                        helper.Release();
                    }
                }

                // ���������� ��������, �������� ������������� ����� ������
                ValidateSession(sessionID, true);
                // ������ ����������
                helper.SessionID = sessionID;
                return true;
            }
        }

        #endregion

        #region ���������� ���������� IDeviceManager

        #region ������ � ������������

        /// <summary>
        /// �������� ����������� ������ � ����������
        /// </summary>
        /// <param name="sessionID">������������� ������</param>
        /// <param name="deviceID">������������� ����������</param>
        /// <param name="waitTimeout">������� �������� ������� ����������, �������</param>
        public bool Capture(string sessionID, string deviceID, Int32 waitTimeout)
        {
            bool isCaptured;
            // �������� �����
            DateTime fixedTime = DateTime.Now;

            do
            {
                // �������� ��������� ����������
                isCaptured = PerformCapture(sessionID, deviceID);
                // ���� ���������� �� ��������� � ������ ����� ��������
                if (!isCaptured && waitTimeout != WaitConstant.None)
                    // ������������������ ����� ��������� ��������
                    System.Threading.Thread.Sleep(0);
            }
            while (!isCaptured && DateTime.Now.Subtract(fixedTime).Seconds < waitTimeout);
            return isCaptured;
        }

        /// <summary>
        /// ���������� ����������
        /// </summary>
        /// <param name="sessionID">������������� ������</param>
        /// <param name="deviceID">������������� ����������</param>
        public bool Release(string sessionID, string deviceID)
        {
            lock (dataLocker)
            {
                ValidateSession(sessionID, false);
                DeviceHelper helper = GetDeviceByID(deviceID);
                if (!helper.Captured || helper.SessionID == sessionID)
                {
                    // ���������� �������� ��� ��������� ���� ��������
                    helper.Release();
                    return true;
                }

                // ���������� ��������� ������ ��������
                return false;
            }
        }

        /// <summary>
        /// ������� ��������� ����������
        /// </summary>
        /// <param name="sessionID">������������� ������</param>
        /// <param name="deviceID">������������� ����������</param>
        public IDevice GetDevice(string sessionID, string deviceID)
        {
            lock (dataLocker)
            {
                ValidateSession(sessionID, true);
                DeviceHelper helper = GetDeviceByID(deviceID);
                if (helper.Captured && helper.SessionID != sessionID)
                    throw new DeviceManagerException(
                        String.Format("������ ������� � ���������� {0}. ���������� ��������� �������� {1}",
                        deviceID, helper.SessionID));

                // ���������� ������ �� ������� ��������� ����������
                return helper.Device;
            }
        }

        #endregion

        #region ������ � ����������� ��������

        /// <summary>
        /// ����������� ���������� ������ � ���������� ���������
        /// </summary>
        /// <param name="sessionID">������������� ������</param>
        /// <returns>true, ���� ������ ������� ����������������</returns>
        public void Login(out string sessionID)
        {
            lock (dataLocker)
            {
                if (_props.MaxSessionsCount > 0 && sessionsTable.Count >= _props.MaxSessionsCount)
                    throw new DeviceManagerException(
                        String.Format("���������� ������������ ���������� ���������� ������ - {0}.",
                        _props.MaxSessionsCount));

                // ������� ����� ������������� ������
                sessionID = Guid.NewGuid().ToString();
                // ������������ ��
                sessionsTable.Add(sessionID, new SessionHelper(sessionID, _props.SessionTimeout));
            }
        }

        /// <summary>
        /// ���������� ���������� ������ � ���������� ���������
        /// </summary>
        /// <param name="sessionID">������������� ������</param>
        public void Logout(string sessionID)
        {
            lock (dataLocker)
            {
                // ��������� ������������ �������������� ������
                ValidateSession(sessionID, false);
                
                // ����������� ��� ����������, ����������� ���� �������
                foreach (DeviceHelper helper in devicesTable.Values)
                {
                    if (helper.SessionID == sessionID)
                        helper.Release();
                }
                
                // ������� ������
                if (sessionsTable.ContainsKey(sessionID))
                    sessionsTable.Remove(sessionID);
            }
        }

        #endregion

        #endregion

        #region ���������� ���������� ILogger

        /// <summary>
        /// ��������� ������ � �������� ������ ����������
        /// </summary>
        /// <param name="message">����� ���������</param>
        /// <param name="type">��� ���������</param>
        public void WriteEntry(String message, EventLogEntryType type)
        {
            // ������������ ��� �������
            // ��� EventLogEntryType �������� ��� �������� �������������
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
        /// ��������� ������ � �������� ������ ����������
        /// </summary>
        /// <param name="message">����� ���������</param>
        /// <param name="type">��� ���������</param>
        /// <param name="sender">����������, ������� ����� � ���</param>
        public void WriteEntry(IDevice sender, String message, EventLogEntryType type)
        {
            WriteEntry(String.Format("���������� [{0}]: {1}", sender.DeviceId, message), type);
        }

        /// <summary>
        /// ����� �������
        /// </summary>
        public Boolean DebugInfo
        {
            get { return _props.DebugInfo; }
        }

        /// <summary>
        /// ���������� ���������� ����������
        /// </summary>
        /// <param name="sender">����������, �������� ����� ��������� ���������� ����������</param>
        /// <param name="info">���������� ����������</param>
        public void SaveDebugInfo(IDevice sender, String info)
        {
            // ��������� ��� ����� ��� �������� ����������� ����������
            String debugFileName = String.Format("{0}\\{1}.dmp",
                FileSystemHelper.GetSubDirectory(GetDeviceManagerDebugDirectory(), sender.DeviceId),
                DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));

            using (StreamWriter debugWriter = new StreamWriter(debugFileName, false, Encoding.Default))
            {
                debugWriter.WriteLine(String.Format("���� � �����: {0}",
                    DateTime.Now.ToString("yyyy MMM dd, HH:mm:ss")));
                debugWriter.WriteLine(String.Format("����������: {0}", sender.DeviceId));
                debugWriter.WriteLine(String.Empty);
                debugWriter.WriteLine("���������� ����������:");
                debugWriter.WriteLine(String.Empty);
                debugWriter.WriteLine(info);
            }
        }

        #endregion

        #region ���������� ���������� ISerialPortsPool

        /// <summary>
        /// �������� ������ � ����� �� ����
        /// </summary>
        /// <param name="deviceId">������������� ����������</param>
        /// <param name="portName">��� �����</param>
        /// <returns>���� �� ����</returns>
        public EasyCommunicationPort GetPort(String deviceId, String portName)
        {
            CheckSerialPortName(portName);
            SerialPortsHelper helper = _serialPortsPool[portName];
            if (String.Compare(helper.DeviceId, deviceId) == 0)
                return helper.Port;
            else
            {
                ThrowPortIsBusy(portName, helper.DeviceId);
                return null;
            }
        }

        /// <summary>
        /// ��������� ���������������� ����
        /// </summary>
        /// <param name="deviceId">������������� ����������</param>
        /// <param name="portName">��� �����</param>
        /// <param name="waitIfCaptured">������� ������������ �����</param>
        /// <param name="waitTime">�����, � ������� �������� ������� ������������</param>
        public EasyCommunicationPort CapturePort(String deviceId, String portName,
            Boolean waitIfCaptured, TimeSpan waitTime)
        {
            CheckSerialPortName(portName);
            SerialPortsHelper helper = _serialPortsPool[portName];

            // ������� ������� �����
            TimeSpan timeElapsed = new TimeSpan();
            do
            {
                if (String.IsNullOrEmpty(helper.DeviceId) || String.Compare(helper.DeviceId, deviceId) == 0)
                {
                    lock (_portsLocker)
                    {
                        helper.DeviceId = deviceId;
                    }
                    return helper.Port;
                }
                else if (waitIfCaptured)
                {
                    // ���� ������������ �����
                    Thread.Sleep(50);
                    TimeSpan ts = new TimeSpan(0, 0, 0, 0, 50);
                    timeElapsed.Add(ts);
                }
            }
            while (waitIfCaptured && (timeElapsed < waitTime));

            // ���� ����� ������ �����������
            ThrowPortIsBusy(portName, helper.DeviceId);
            return null;
        }

        /// <summary>
        /// ��������� ���������������� ����. ������� ������� ����� � ������� 
        /// ������������ ��������� �������
        /// </summary>
        /// <param name="deviceId">������������� ����������</param>
        /// <param name="portName">��� �����</param>
        public EasyCommunicationPort CapturePort(String deviceId, String portName)
        {
            return CapturePort(deviceId, portName, true, TimeSpan.MaxValue);
        }

        /// <summary>
        /// ���������� ���������������� ����
        /// </summary>
        /// <param name="deviceId">������������� ����������</param>
        /// <param name="portName">��� �����</param>
        public void ReleasePort(String deviceId, String portName)
        {
            CheckSerialPortName(portName);
            SerialPortsHelper helper = _serialPortsPool[portName];

            // ������� ������������ �����
            if (String.IsNullOrEmpty(helper.DeviceId) || String.Compare(helper.DeviceId, deviceId) == 0)
            {
                lock (_portsLocker)
                {
                    helper.DeviceId = String.Empty;
                }
            }
            else
                // ���� ����� ������ �����������
                ThrowPortIsBusy(portName, helper.DeviceId);
        }

        #endregion
    }
}
