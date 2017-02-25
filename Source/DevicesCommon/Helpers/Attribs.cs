using System;
using System.Collections.Generic;
using System.Text;

namespace DevicesCommon.Helpers
{
	/// <summary>
	/// ������� ��� ������ ���������� ���������
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DeviceManagerAttribute : System.Attribute
	{
	}

	/// <summary>
	/// ������� ������� ��� ��������� ���������
	/// </summary>
	public abstract class DeviceAttribute : System.Attribute
	{
		// ������������ ���� ����������
		private String deviceType;

		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="deviceType">��� ����������</param>
		protected DeviceAttribute(String deviceType)
		{
			this.deviceType = deviceType;
		}

		/// <summary>
		/// ��� ����������
		/// </summary>
		public String DeviceType
		{
			get
			{
				return deviceType;
			}
		}
	}

    /// <summary>
    /// ������� ���������� ������ ���������� ����������
    /// </summary>
    public sealed class TurnstileDeviceAttribute : DeviceAttribute
    {
        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="turnstileType">��� ���������</param>
        public TurnstileDeviceAttribute(String turnstileType)
            : base(turnstileType)
        {
        }

        /// <summary>
        /// ��� ���������
        /// </summary>
        public String TurnstileType
        {
            get { return base.DeviceType; }
        }
    }

    /// <summary>
    /// ������� ���������� �����������
    /// </summary>
    public sealed class GenericReaderAttribute : DeviceAttribute
    {
        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="readerType">��� �����������</param>
        public GenericReaderAttribute(String readerType)
            : base(readerType)
        {
        }

        /// <summary>
        /// ��� �����������
        /// </summary>
        public String ReaderType
        {
            get { return base.DeviceType; }
        }
    }

    /// <summary>
    /// ������� ���������� SMS-�������
    /// </summary>
    public sealed class SMSClientAttribute : DeviceAttribute
    {
        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="smsClientType">��� SMS-�������</param>
        public SMSClientAttribute(String smsClientType)
            : base(smsClientType)
        {
        }

        /// <summary>
        /// ��� SMS-�������
        /// </summary>
        public String SMSClientType
        {
            get { return base.DeviceType; }
        }
    }

	/// <summary>
	/// ������� �������� �����
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ScaleAttribute : DeviceAttribute
	{
		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="scaleType">��� �����</param>
		public ScaleAttribute(String scaleType) : base(scaleType)
		{
		}

		/// <summary>
		/// ��� �����
		/// </summary>
		public String ScaleType
		{
			get
			{
				return base.DeviceType;
			}
		}
	}

	/// <summary>
	/// ������� �������� ������� ����������
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class CustomerDisplayAttribute : DeviceAttribute
	{
		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="displayType">��� ������� ����������</param>
		public CustomerDisplayAttribute(String displayType) : base(displayType)
		{
		}

		/// <summary>
		/// ��� ������� ����������
		/// </summary>
		public String DisplayType
		{
			get
			{
				return base.DeviceType;
			}
		}
	}

	/// <summary>
	/// ������� �������� ����������� ������������
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class FiscalDeviceAttribute : DeviceAttribute
	{
		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="fiscalType">��� ����������� ������������</param>
		public FiscalDeviceAttribute(String fiscalType) : base(fiscalType)
		{
		}

		/// <summary>
		/// ��� ����������� ������������
		/// </summary>
		public String FiscalType
		{
			get
			{
				return base.DeviceType;
			}
		}
	}

	/// <summary>
	/// ������� �������� ����������� ����������
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class PrintableDeviceAttribute : DeviceAttribute
	{
		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="printerType">��� ����������� ����������</param>
		public PrintableDeviceAttribute(String printerType) : base(printerType)
		{
		}

		/// <summary>
		/// ��� ����������� ����������
		/// </summary>
		public String PrinterType
		{
			get
			{
				return base.DeviceType;
			}
		}
	}

	/// <summary>
	/// ������� �������� ������� ���������
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class SerialScannerAttribute : DeviceAttribute
	{
		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="scannerType">��� ������� ���������</param>
		public SerialScannerAttribute(String scannerType) : base(scannerType)
		{
		}

		/// <summary>
		/// ��� ������� ���������
		/// </summary>
		public String ScannerType
		{
			get
			{
				return base.DeviceType;
			}
		}
	}

    /// <summary>
    /// ������� ������ ���������� ���������
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class BilliardsManagerAttribute : DeviceAttribute
    {
        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="billiardsManagerType">��� ������ ���������� ���������</param>
        public BilliardsManagerAttribute(String billiardsManagerType)
            : base(billiardsManagerType)
        {
        }

        /// <summary>
        /// ��� ������ ���������� ���������
        /// </summary>
        public String BilliardsManagerType
        {
            get { return base.DeviceType; }
        }
    }
}
