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
    /// ������������ ���������� POS-���������
    /// </summary>
    [Serializable]
    public class DevmanProperties
    {
        /// <summary>
        /// ������� ������ ������������
        /// </summary>
        public static string CURRENT_VERSION = "1.0";

        #region ����
        
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

        #region �����������

        /// <summary>
        /// �����������. ��������� ���������� �� ���������
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

        #region ��������

        /// <summary>
        /// ������������ ����� ������������� ����������� � ���������� POS-���������
        /// </summary>
        [Browsable(true)]
        [DefaultValue(-1)]
        [Category("����� ���������")]
        [Description("������������ ����� ������������� ����������� � ���������� POS-���������")]
        [DisplayName("����. ����� ���������� ������")]
        [PropertyOrder(0)]
        public int MaxSessionsCount
        {
            set { _maxSessionsCount = value; }
            get { return _maxSessionsCount; }
        }

        /// <summary>
        /// ������������ �������� ������� � ��������, � ������� �������� ���� ������ 
        /// ����� ����������� ����������. �� ��������� ����� ��������� ��������� 
        /// ������� ���������� ���������
        /// </summary>
        [Browsable(true)]
        [DefaultValue(-1)]
        [Category("����� ���������")]
        [Description("������������ �������� ������� � ��������, � ������� �������� ���� ������ ����� ����������� ����������. �� ��������� ����� ��������� ���������	������� ���������� ���������.")]
        [DisplayName("������� ���������� ������")]
        [PropertyOrder(1)]
        public int SessionTimeout
        {
            set { _sessionTimeout = value; }
            get { return _sessionTimeout; }
        }

        /// <summary>
        /// ����� TCP/IP ����� ������ ���������� POS-���������
        /// </summary>
        [Browsable(true)]
        [DefaultValue(35100)]
        [Category("����� ���������")]
        [Description("����� TCP/IP ����� ������ ���������� POS-���������")]
        [DisplayName("���� ������")]
        [PropertyOrder(2)]
        public int ServicePort
        {
            set { _servicePort = value; }
            get { return _servicePort; }
        }

        /// <summary>
        /// ���������� ���������� ����� � �������������� ����������
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("����� ���������")]
        [Description("���������� ���������� ����� � �������������� ����������")]
        [DisplayName("���������� ����������")]
        [PropertyOrder(3)]
        [TypeConverter(typeof(RussianBooleanConverter))]
        public bool DebugInfo
        {
            set { _debugInfo = value; }
            get { return _debugInfo; }
        }

        /// <summary>
        /// ������ ����� ������������
        /// </summary>
        [Browsable(false)]
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        /// <summary>
        /// ������ ���������� ���������
        /// </summary>
        [Browsable(false)]
        public List<PrintableDeviceProperties> PrintableDevices
        {
            get { return _printableDevices; }
            set { _printableDevices = value; }
        }

        /// <summary>
        /// ������� ����������
        /// </summary>
        [Browsable(false)]
        public List<DisplayDeviceProperties> DisplayDevices
        {
            get { return _displayDevices; }
            set { _displayDevices = value; }
        }

        /// <summary>
        /// ������ ���������� ����������
        /// </summary>
        [Browsable(false)]
        public List<BilliardManagerProperties> BillardDevices
        {
            get { return _billardDevices; }
            set { _billardDevices = value; }
        }

        /// <summary>
        /// ������ ����������� �����
        /// </summary>
        [Browsable(false)]
        public List<ScalesDeviceProperties> ScalesDevices
        {   
            get { return _scalesDevices; }
            set { _scalesDevices = value; }
        }

        /// <summary>
        /// ������ ����������
        /// </summary>
        [Browsable(false)]
        public List<TurnstileProperties> TurnstileDevices
        {
            get { return _turnstileDevices; }
            set { _turnstileDevices = value; }
        }

        /// <summary>
        /// ������ ������������
        /// </summary>
        [Browsable(false)]
        public List<ReaderDeviceProperties> ReaderDevices
        {
            get { return _readerDevices; }
            set { _readerDevices = value; }
        }

        /// <summary>
        /// ������ SMS-��������
        /// </summary>
        [Browsable(false)]
        public List<SMSClientProperties> SMSClientDevices
        {
            get { return _SMSClientDevices; }
            set { _SMSClientDevices = value; }
        }

        #endregion

        #region ������ ������

        /// <summary>
        /// �������� ������������ �� ���������� �����
        /// </summary>
        /// <param name="fileName">���� � �����</param>
        /// <returns></returns>
        public static DevmanProperties Load(string fileName)
        {
            // ��������� ������������� ����� ������������
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
                // ������������ �� ���������
                return new DevmanProperties();
        }

        /// <summary>
        /// ���������� ������������ � ��������� ����
        /// </summary>
        /// <param name="fileName">���� � �����</param>
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
    /// ������� ����� ������������ ����������
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
        #region ����

        private string _deviceType;
        private string _deviceId;

        #endregion

        #region ��������

        /// <summary>
        /// ������������� ����������. ������ ���� ���������� � �������� ���� 
        /// ������������
        /// </summary>
        [Browsable(true)]
        [Category("�����")]
        [Description("������������� ����������. ������ ���� ���������� � �������� ���� ������������")]
        [DisplayName("������������� ����������")]
        [PropertyOrder(0)]
        public string DeviceId
        {
            get { return _deviceId; }
            set { _deviceId = value; }
        }

        /// <summary>
        /// ��� ����������. ���������� ������������ �������
        /// </summary>
        [Browsable(true)]
        [Category("�����")]
        [Description("��� ���������� ���������� ������������ �������")]
        [DisplayName("��� ����������")]
        [PropertyOrder(1)]
        public virtual string DeviceType
        {
            get { return _deviceType; }
            set { _deviceType = value; }
        }

        #endregion

        #region �����������

        /// <summary>
        /// �����������
        /// </summary>
        public BaseDeviceProperties()
        {
        }

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="deviceId">������������� ����������</param>
        public BaseDeviceProperties(string deviceId)
            :this()
        {            
            _deviceId = deviceId;
        }

        #endregion

        #region ������

        /// <summary>
        /// ������������ ����������. ���������������� � �����������
        /// </summary>
        public virtual void TestDevice()
        {
            MessageBox.Show("������� ������������ ��� ���������� �� ����������", "������������ ����������", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion
    }

    /// <summary>
    /// ������������ ������������� ����������
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorter))]
    public class ConnectableDeviceProperties : BaseDeviceProperties
    {
        #region ����

        private string _port;
        private int _baudRate;

        #endregion

        #region ��������

        /// <summary>
        /// ��� ����� ��� ����������� �c��������
        /// </summary>
        [Browsable(true)]
        [DefaultValue("COM1")]
        [Category("��������� �����������")]
        [Description("���������������� ���� ��� ����������� �c��������")]
        [DisplayName("����")]
        [TypeConverter(typeof(PortTypeConverter))]
        [PropertyOrder(2)]
        public string Port
        {
            set { _port = value; }
            get { return _port; }
        }

        /// <summary>
        /// �������� �������� ������ ����� ����
        /// </summary>
        [Browsable(true)]
        [DefaultValue(9600)]
        [Category("��������� �����������")]
        [Description("�������� �������� ������ ����� ����")]
        [DisplayName("��������")]
        [TypeConverter(typeof(BaudConverter))]
        [Editor(typeof(BaudEditor), typeof(UITypeEditor))]
        [PropertyOrder(3)]        
        public int BaudRate
        {
            set { _baudRate = value; }
            get { return _baudRate; }
        }

        #endregion

        #region �����������

        /// <summary>
        /// �����������
        /// </summary>
        public ConnectableDeviceProperties()
        {
            _port = "COM1";
            _baudRate = 9600;
        }

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="deviceId">������������� ����������</param>
        public ConnectableDeviceProperties(string deviceId)
            : base(deviceId)
        {
            _port = "COM1";
            _baudRate = 9600;
        }

        #endregion
    }


    /// <summary>
    /// ������������ ����������� ����������
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorter))]
    public class PrintableDeviceProperties : ConnectableDeviceProperties
    {
        #region ����

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

        #region �����������

        /// <summary>
        /// �����������
        /// </summary>
        public PrintableDeviceProperties()
            : base()
        {
        }

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="deviceId">������������� ����������</param>
        public PrintableDeviceProperties(string deviceId)
            : base(deviceId)
        {
        }

        #endregion

        #region ��������

        /// <summary>
        /// ���������� �������� �������� ������
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Category("��������� �����������")]
        [Description("�������� ����� DTR/DSR")]
        [DisplayName("�������� DTR/DSR")]
        [PropertyOrder(10)]
        [TypeConverter(typeof(RussianBooleanConverter))]
        public bool DtrEnabled
        {
            get { return _dtrEnabled; }
            set { _dtrEnabled = value; }
        }

        /// <summary>
        /// ��� ����������. ���������� ������������ �������
        /// </summary>
        [Browsable(true)]
        [Category("�����")]
        [Description("��� ���������� ���������� ������������ �������")]
        [DisplayName("��� ����������")]
        [PropertyOrder(1)]
        [TypeConverter(typeof(PrintableTypesConverter))]
        public override string DeviceType
        {
            get { return base.DeviceType; }
            set { base.DeviceType = value; }
        }

        /// <summary>
        /// ������ ������: �� ������� �����, ���������� ��������� ��� ���������������
        /// </summary>
        [Browsable(true)]
        [DefaultValue(PrinterKind.Receipt)]
        [Category("������")]
        [Description("������ ������: �� ������� �����, ���������� ��������� ��� ���������������")]
        [DisplayName("������ ������")]
        [TypeConverter(typeof(PrinterKindConverter))]
        [Editor(typeof(PrinterKindEditor), typeof(UITypeEditor))]
        [PropertyOrder(3)]
        public PrinterKind PaperType
        {
            set { _paperType = value; }
            get { return _paperType; }
        }

        /// <summary>
        /// ������ ����� � ��������
        /// </summary>
        [Browsable(true)]
        [Category("������")]
        [Description("������ ����� � ��������")]
        [DisplayName("�������� � ������")]
        [PropertyOrder(4)]
        public PrintableTapeWidth TapeWidth
        {
            set { _tapeWidth = value; }
            get { return _tapeWidth; }
        }

        /// <summary>
        /// ������� ������ � ������� �� ������ ���������
        /// </summary>
        [Browsable(true)]
        [DefaultValue(0)]
        [Category("������")]
        [Description("������� ������ � ������� �� ������ ���������")]
        [DisplayName("������� ������")]
        [PropertyOrder(5)]
        public int TopMargin
        {
            set { _topMargin = value; }
            get { return _topMargin; }
        }

        /// <summary>
        /// ����� ������ ����������� ��������� � �����������
        /// </summary>
        [Browsable(true)]
        [DefaultValue(0)]
        [Category("������")]
        [Description("����� ������ ����������� ��������� � �����������")]
        [DisplayName("����� ����������� ���������")]
        [PropertyOrder(6)]
        public int SlipHeight
        {
            set { _slipHeight = value; }
            get { return _slipHeight; }
        }

        #region �����

        /// <summary>
        /// �������� ������ �����
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Category("������ �����")]
        [Description("�������� �����")]
        [DisplayName("�������� �����")]
        [PropertyOrder(7)]
        [TypeConverter(typeof(RussianBooleanConverter))]
        public bool PrintHeader
        {
            get { return _printHeader; }
            set { _printHeader = value; }
        }

        /// <summary>
        /// ������, ���������� ����� ����������
        /// </summary>
        [Browsable(true)]
        [Category("������ �����")]
        [Description("������, ���������� ����� ����������")]
        [DisplayName("����� ����")]
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
        /// �������� ����������� �����
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("������ �����")]
        [Description("�������� ����������� �����")]
        [DisplayName("�������� ����������� �����")]
        [TypeConverter(typeof(RussianBooleanConverter))]
        [PropertyOrder(9)]
        public bool PrintGraphicHeader
        {
            get { return _printGraphicHeader; }
            set { _printGraphicHeader = value; }
        }

        /// <summary>
        /// ����������� �����
        /// </summary>
        [Browsable(true)]
        [DefaultValue(null)]
        [Category("������ �����")]
        [Description("����������� �����")]
        [DisplayName("����������� �����")]
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

        #region ������

        /// <summary>
        /// �������� ������
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Category("������ �������")]
        [Description("�������� ������")]
        [DisplayName("�������� ������")]
        [TypeConverter(typeof(RussianBooleanConverter))]
        [PropertyOrder(11)]
        public bool PrintFooter
        {
            get { return _printFooter; }
            set { _printFooter = value; }
        }

        /// <summary>
        /// ������, ���������� ����� ���������
        /// </summary>
        [Browsable(true)]
        [Category("������ �������")]
        [Description("������, ���������� ����� ���������")]
        [DisplayName("������ ����")]
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
        /// �������� ����������� ������
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("������ �������")]
        [Description("�������� ����������� ������")]
        [DisplayName("�������� ����������� ������")]
        [TypeConverter(typeof(RussianBooleanConverter))]
        [PropertyOrder(13)]
        public bool PrintGraphicFooter
        {
            get { return _printGraphicFooter; }
            set { _printGraphicFooter = value; }
        }

        /// <summary>
        /// ����������� ������
        /// </summary>
        [Browsable(true)]
        [DefaultValue(null)]
        [Category("������ �������")]
        [Description("����������� ������")]
        [DisplayName("����������� ������")]
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
        /// �������� ��� ������������ ������������ ��������� ����.
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
        /// �������� ��� ������������ ������������ ������� ����.
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

        #region ������

        /// <summary>
        /// �������� ������ ����� ��������� ���������
        /// </summary>
        public override void TestDevice()
        {
            TestPrintForm.TestPrint(DeviceId);
        }

        #endregion
    }

    /// <summary>
    /// ������������ ������� ����������
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorter))]
    public class DisplayDeviceProperties : ConnectableDeviceProperties
    {
        #region ��������

        /// <summary>
        /// ��� ����������. ���������� ������������ �������
        /// </summary>
        [Browsable(true)]
        [Category("�����")]
        [Description("��� ���������� ���������� ������������ �������")]
        [DisplayName("��� ����������")]
        [PropertyOrder(1)]
        [TypeConverter(typeof(DeviceTypesConverter<CustomerDisplayAttribute>))]
        public override string DeviceType
        {
            get { return base.DeviceType; }
            set { base.DeviceType = value; }
        }

        #endregion

        #region �����������

        /// <summary>
        /// ����������� �� ���������
        /// </summary>
        public DisplayDeviceProperties()
            : base()
        {
        }

        /// <summary>
        /// �����������
        /// </summary>
        public DisplayDeviceProperties(string deviceId)
            : base(deviceId)
        {
        }

        #endregion

        #region ������

        /// <summary>
        /// ������������ ����������.
        /// </summary>
        public override void TestDevice()
        {
            DisplayTestForm.TestDisplay(DeviceId);
        }

        #endregion

    }

    /// <summary>
    /// ������������ �����������
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorter))]
    public class ReaderDeviceProperties : ConnectableDeviceProperties
    {
        #region ���������

        private const byte DEF_STOP_CHAR = 0xA;

        //private const ERPService.SharedLibs.Helpers.SerialCommunications.Parity DEF_PARITY =
            //ERPService.SharedLibs.Helpers.SerialCommunications.Parity.None;
        
        #endregion

        #region ����

        private byte _stopChar = DEF_STOP_CHAR;

        private ERPService.SharedLibs.Helpers.SerialCommunications.Parity _parity = ERPService.SharedLibs.Helpers.SerialCommunications.Parity.Even;

        #endregion

        #region ��������

        /// <summary>
        /// ��� ����������. ���������� ������������ �������
        /// </summary>
        [Browsable(true)]
        [Category("�����")]
        [Description("��� ���������� ���������� ������������ �������")]
        [DisplayName("��� ����������")]
        [PropertyOrder(1)]
        [TypeConverter(typeof(DeviceTypesConverter<GenericReaderAttribute>))]
        public override string DeviceType
        {
            get { return base.DeviceType; }
            set { base.DeviceType = value; }
        }

        /// <summary>
        /// ����-������
        /// </summary>
        [Browsable(true)]
        [Category("��������� �����������")]
        [DisplayName("����-������")]
        [Description("���������� ��� ����-�������")]
        [PropertyOrder(4)]
        [DefaultValue(DEF_STOP_CHAR)]
        public byte StopChar
        {
            get { return _stopChar; }
            set { _stopChar = value; }
        }

        /// <summary>
        /// �������� ��������
        /// </summary>
        [Browsable(true)]
        [Category("��������� �����������")]
        [DisplayName("�������� ��������")]
        [Description("���� �������� ��������")]
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

        #region �����������

        /// <summary>
        /// ����������� �� ���������
        /// </summary>
        public ReaderDeviceProperties()
            : base()
        {
            DeviceType = DevicesCommon.Helpers.DeviceNames.ironLogicRFIDReader;
        }

        /// <summary>
        /// �����������
        /// </summary>
        public ReaderDeviceProperties(string deviceId)
            : base(deviceId)
        {
            DeviceType = DevicesCommon.Helpers.DeviceNames.ironLogicRFIDReader;
        }

        #endregion

        #region ������

        /// <summary>
        /// ������������ ����������.
        /// </summary>
        public override void TestDevice()
        {
            ReaderTestForm.TestReader(DeviceId);
        }

        #endregion
    }

    /// <summary>
    /// ������������ ������ ���������� ����������
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorter))]
    public class BilliardManagerProperties : ConnectableDeviceProperties
    {
        #region ������

        /// <summary>
        /// ������������ ����������.
        /// </summary>
        public override void TestDevice()
        {
            TestBilliardForm.TestBilliard(DeviceId);
        }

        #endregion

        #region ��������

        /// <summary>
        /// ��� ����������. ���������� ������������ �������
        /// </summary>
        [Browsable(true)]
        [Category("�����")]
        [Description("��� ���������� ���������� ������������ �������")]
        [DisplayName("��� ����������")]
        [PropertyOrder(1)]
        [TypeConverter(typeof(DeviceTypesConverter<BilliardsManagerAttribute>))]
        public override string DeviceType
        {
            get { return base.DeviceType; }
            set { base.DeviceType = value; }
        }

        #endregion

        #region ����������

        /// <summary>
        /// ����������� ��� ����������
        /// </summary>
        public BilliardManagerProperties()
        {
            DeviceType = DevicesCommon.Helpers.DeviceNames.blcCl8rc;
        }

        /// <summary>
        /// ����������� � ����������
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
    /// ������������ ���������
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorter))]
    public class TurnstileProperties : ConnectableDeviceProperties
    {
        #region ����

        private TurnstileDirection _direction;

        private int _timeout = 15;

        private int _address = 0;

        #endregion

        #region ������

        /// <summary>
        /// ������������ ����������.
        /// </summary>
        public override void TestDevice()
        {
            TurnstileTestForm.TestDevice(DeviceId);
        }

        #endregion

        #region ��������

        /// <summary>
        /// ��� ����������. ���������� ������������ �������
        /// </summary>
        [Browsable(true)]
        [Category("�����")]
        [Description("��� ���������� ���������� ������������ �������")]
        [DisplayName("��� ����������")]
        [PropertyOrder(1)]
        [TypeConverter(typeof(DeviceTypesConverter<TurnstileDeviceAttribute>))]
        public override string DeviceType
        {
            get { return base.DeviceType; }
            set { base.DeviceType = value; }
        }

        /// <summary>
        /// ����� ����������
        /// </summary>
        [Browsable(true)]
        [Category("��������� �����������")]
        [Description("����� ����������")]
        [DisplayName("�����")]
        [PropertyOrder(1)]
        [DefaultValue(0)]
        public int Address
        {
            get { return _address; }
            set { _address = value; }
        }

        /// <summary>
        /// �����������
        /// </summary>
        [Browsable(true)]
        [Category("������")]
        [Description("����������� �������")]
        [DisplayName("�����������")]
        [PropertyOrder(1)]
        [TypeConverter(typeof(EntryTypeConverter))]
        [Editor(typeof(EntryTypeEditor), typeof(UITypeEditor))]
        public TurnstileDirection Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        /// <summary>
        /// ������� ���������
        /// </summary>
        [Browsable(true)]
        [Category("������")]
        [Description("�����, � ������� �������� �������� �������� ��������")]
        [DisplayName("�������, �")]
        [PropertyOrder(2)]
        [DefaultValue(15)]
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }       

        #endregion

        #region ����������

        /// <summary>
        /// ����������� ��� ����������
        /// </summary>
        public TurnstileProperties()
        {
            DeviceType = DevicesCommon.Helpers.DeviceNames.t283dualTripod;
        }

        /// <summary>
        /// ����������� � ����������
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
    /// ������������ ����������� �����
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorter))]
    public class ScalesDeviceProperties : BaseDeviceProperties
    {
        #region ����

        private string _connectionString;

        #endregion

        #region ��������

        /// <summary>
        /// ��� ����������. ���������� ������������ �������
        /// </summary>
        [Browsable(true)]
        [Category("�����")]
        [Description("��� ���������� ���������� ������������ �������")]
        [DisplayName("��� ����������")]
        [PropertyOrder(1)]
        [TypeConverter(typeof(DeviceTypesConverter<ScaleAttribute>))]
        public override string DeviceType
        {
            get { return base.DeviceType; }
            set { base.DeviceType = value; }
        }

        /// <summary>
        /// ������ ����������� � �����
        /// </summary>
        [Browsable(true)]
        [Category("��������� �����������")]
        [Description("������ �����������")]
        [DisplayName("������ �����������")]
        [Editor(typeof(ScalesConnectionEditor), typeof(UITypeEditor))]
        [PropertyOrder(1)]
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        #endregion

        #region ������

        /// <summary>
        /// ������������ ����������.
        /// </summary>
        public override void TestDevice()
        {
            ScalesTestForm.TestScales(DeviceId);
        }

        #endregion

        #region �����������

        /// <summary>
        /// ����������� ��� ����������
        /// </summary>
        public ScalesDeviceProperties()
        {
        }

        /// <summary>
        /// ����������� � ����������
        /// </summary>
        /// <param name="deviceId"></param>
        public ScalesDeviceProperties(string deviceId)
            : base(deviceId)
        {
        }

        #endregion
    }

    /// <summary>
    /// ������������ SMS-�������
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorter))]
    public class SMSClientProperties : BaseDeviceProperties
    {
        #region ����

        private SerializableDictionary<string, string> _settings;

        #endregion            

        #region ��������

        /// <summary>
        /// ��� ����������. ���������� ������������ �������
        /// </summary>
        [Browsable(true)]
        [Category("�����")]
        [Description("��� ���������� ���������� ������������ �������")]
        [DisplayName("��� ����������")]
        [PropertyOrder(1)]
        [TypeConverter(typeof(DeviceTypesConverter<SMSClientAttribute>))]
        public override string DeviceType
        {
            get { return base.DeviceType; }
            set { base.DeviceType = value; }
        }

        /// <summary>
        /// ��������� ����������
        /// </summary>
        [Browsable(true)]
        [Category("�����")]
        [Description("������ ���������� ����������")]
        [DisplayName("��������� ����������")]
        [TypeConverter(typeof(HideValueConverter))]
        [Editor(typeof(KeyValueEditor), typeof(UITypeEditor))]
        public SerializableDictionary<string, string> Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        #endregion

        #region ������

        /// <summary>
        /// ������������ ����������.
        /// </summary>
        public override void TestDevice()
        {
            SMSClientTestForm.TestSMSClient(DeviceId);
        }

        #endregion

        #region ����������

        /// <summary>
        /// ����������� ��� ����������
        /// </summary>
        public SMSClientProperties()
        {
            DeviceType = DevicesCommon.Helpers.DeviceNames.standardGSMModem;
            _settings = new SerializableDictionary<string, string>();
        }

        /// <summary>
        /// ����������� � ����������
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