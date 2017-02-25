using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.ComponentModel;

namespace DevicesCommon.Helpers
{
    /// <summary>
    /// ������ ����� �� ���������� �����������
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class PrintableTapeWidth: ISerializable
    {
        private Int32 _mainPrinter;
        private Int32 _additionalPrinter1;

        /// <summary>
        /// ���������� ��������� ������������� �������
        /// </summary>
        public override String ToString()
        {
            return String.Format("{0}; {1}", _mainPrinter, _additionalPrinter1);
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        public PrintableTapeWidth()
        {
            _mainPrinter = 0;
            _additionalPrinter1 = 0;
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="mainPrinter">������ ����� �� �������� ��������</param>
        /// <param name="additionalPrinter1">������ ����� �� ������ �������������� 
        /// ��������</param>
        public PrintableTapeWidth(Int32 mainPrinter, Int32 additionalPrinter1)
        {
            _mainPrinter = mainPrinter;
            _additionalPrinter1 = additionalPrinter1;
        }

		/// <summary>
		/// ����������� ������������
		/// </summary>
		/// <param name="info">����� <see cref="SerializationInfo"/> ��� ���������� � ������������</param>
		/// <param name="context">�������� ������������</param>
        protected PrintableTapeWidth(SerializationInfo info, StreamingContext context)
		{
            _mainPrinter = info.GetInt32("_mainPrinter");
            _additionalPrinter1 = info.GetInt32("_additionalPrinter1");
		}

        /// <summary>
        /// ������������ �������� ������
        /// </summary>
        /// <param name="info">����� <see cref="SerializationInfo"/> ��� ���������� � ������������</param>
        /// <param name="context">�������� ������������</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_mainPrinter", _mainPrinter);
            info.AddValue("_additionalPrinter1", _additionalPrinter1);
        }

        /// <summary>
        /// ������ ����� �� �������� ��������
        /// </summary>
        [DefaultValue(0)]
        [Description("������ ����� �� �������� ��������")]
        [DisplayName("�������� �������")]
        public Int32 MainPrinter
        {
            get { return _mainPrinter; }
            set { _mainPrinter = value; }
        }

        /// <summary>
        /// ������ ����� �� ������ �������������� ��������
        /// </summary>
        [DefaultValue(0)]
        [Description("������ ����� �� ������ �������������� ��������")]
        [DisplayName("�������������� ������� �1")]
        public Int32 AdditionalPrinter1
        {
            get { return _additionalPrinter1; }
            set { _additionalPrinter1 = value; }
        }
    }

	/// <summary>
	/// ���������� �������������� ����������� ����������
	/// </summary>
	[Serializable]
	public class PrintableDeviceInfo : ISerializable
	{
        // ������ ������� �����
        private PrintableTapeWidth _tapeWidth;
        // ������ � ������� �� �������� ���� ���������
        private Int32 _topMargin;
        // ����� ������ ����������� ���������
        private Int32 _slipFormLength;
        // ��� ����������� ����������
        private PrinterKind _kind;
        // ���������� �������� �� ��������� ������
        private Boolean _dsrFlowControl; 

		#region ��������������

        /// <summary>
        /// ��� ����������� ����������
        /// </summary>
        public PrinterKind Kind
        {
            get { return _kind; }
            set { _kind = value; }
        }

		/// <summary>
		/// ������ ������� ����� � ��������
		/// </summary>
        public PrintableTapeWidth TapeWidth
        {
            get { return _tapeWidth; }
            set { _tapeWidth = value; }
        }

        /// <summary>
        /// ������� ������ �� ���� �������� �����, �����
        /// </summary>
        public Int32 TopMargin
        {
            get { return _topMargin; }
            set { _topMargin = value; }
        }

        /// <summary>
        /// ����� ������ ����������� ��������� � �����������
        /// </summary>
        public Int32 SlipFormLength
        {
            get { return _slipFormLength; }
            set { _slipFormLength = value; }
        }

        /// <summary>
        /// ���������� �������� DTR/DSR
        /// </summary>
        public Boolean DsrFlowControl
        {
            get { return _dsrFlowControl; }
            set { _dsrFlowControl = value; }
        }

        /// <summary>
        /// �������������� ��� ��� ������ ����� (��������� �� ������)
        /// </summary>
        public readonly bool SupportsBoldFont;

		#endregion

		#region �����������

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="tapeWidth">������ ������� ����� � ��������</param>
        /// <param name="supportsBoldFont">�������������� ��� ��� ������ ����� (��������� �� ������)</param>
        /// <param name="topMargin">������� ������ �� ���� �������� �����, �����</param>
        /// <param name="slipFormLength">����� ������ ����������� ��������� � �����������</param>
        /// <param name="kind">��� ����������� ����������</param>
        /// <param name="dsrFlowControl">���������� �������� DTR/DSR</param>
        public PrintableDeviceInfo(PrintableTapeWidth tapeWidth, Boolean supportsBoldFont, Int32 topMargin,
            Int32 slipFormLength, PrinterKind kind, Boolean dsrFlowControl)
        {
            _tapeWidth = tapeWidth;
            _topMargin = topMargin;
            _slipFormLength = slipFormLength;
            _kind = kind;
            _dsrFlowControl = dsrFlowControl;
            SupportsBoldFont = supportsBoldFont;
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="tapeWidth">������ ������� ����� � ��������</param>
        /// <param name="supportsBoldFont">�������������� ��� ��� ������ ����� (��������� �� ������)</param>
        /// <param name="topMargin">������� ������ �� ���� �������� �����, �����</param>
        /// <param name="slipFormLength">����� ������ ����������� ��������� � �����������</param>
        /// <param name="kind">��� ����������� ����������</param>
        public PrintableDeviceInfo(PrintableTapeWidth tapeWidth, Boolean supportsBoldFont, Int32 topMargin,
            Int32 slipFormLength, PrinterKind kind)
            : this(tapeWidth, supportsBoldFont, topMargin, slipFormLength, kind, false)
        {
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="tapeWidth">������ ������� ����� � ��������</param>
        /// <param name="supportsBoldFont">�������������� ��� ��� ������ ����� (��������� �� ������)</param>
        public PrintableDeviceInfo(PrintableTapeWidth tapeWidth, Boolean supportsBoldFont)
            : this(tapeWidth, supportsBoldFont, 0, 0, PrinterKind.Receipt, false)
        {
        }

		/// <summary>
		/// ����������� ������������
		/// </summary>
		/// <param name="info">����� <see cref="SerializationInfo"/> ��� ���������� � ������������</param>
		/// <param name="context">�������� ������������</param>
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

		#region ������������

		/// <summary>
		/// ������������ �������� ������
		/// </summary>
		/// <param name="info">����� <see cref="SerializationInfo"/> ��� ���������� � ������������</param>
		/// <param name="context">�������� ������������</param>
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
	/// ���������� �������������� ����������� ����������
	/// </summary>
	[Serializable]
	public class FiscalDeviceInfo : ISerializable
	{
		#region ��������������

		/// <summary>
		/// ��� ���������� ������
		/// </summary>
		public readonly String DeviceType;

		/// <summary>
		/// �������� �����
		/// </summary>
		public readonly String SerialNo;

		#endregion

		#region �����������

		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="deviceType">��� ���������� ������</param>
		/// <param name="serialNo">������� ����� ����������</param>
		public FiscalDeviceInfo(String deviceType, String serialNo)
		{
			DeviceType = deviceType;
			SerialNo = serialNo;
		}

		/// <summary>
		/// ����������� ��� ������������
		/// </summary>
		/// <param name="info">����� <see cref="SerializationInfo"/> ��� ���������� � ������������</param>
		/// <param name="context">�������� ������������</param>
		protected FiscalDeviceInfo(SerializationInfo info, StreamingContext context)
		{
			DeviceType = info.GetString("FiscalDeviceType");
			SerialNo = info.GetString("SerialNo");
		}

		#endregion

		#region ������������

		/// <summary>
		/// ������������ �������� ������
		/// </summary>
		/// <param name="info">����� <see cref="SerializationInfo"/> ��� ���������� � ������������</param>
		/// <param name="context">�������� ������������</param>
		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("FiscalDeviceType", DeviceType);
			info.AddValue("SerialNo", SerialNo);
		}

		#endregion
	}
}
