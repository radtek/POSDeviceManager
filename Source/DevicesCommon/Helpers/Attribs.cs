using System;
using System.Collections.Generic;
using System.Text;

namespace DevicesCommon.Helpers
{
	/// <summary>
	/// Атрибут для класса диспетчера устройств
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DeviceManagerAttribute : System.Attribute
	{
	}

	/// <summary>
	/// Базовый атрибут для драйверов устройств
	/// </summary>
	public abstract class DeviceAttribute : System.Attribute
	{
		// наименование типа устройства
		private String deviceType;

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="deviceType">Тип устройства</param>
		protected DeviceAttribute(String deviceType)
		{
			this.deviceType = deviceType;
		}

		/// <summary>
		/// Тип устройства
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
    /// Атрибут реализации модуля управления турникетом
    /// </summary>
    public sealed class TurnstileDeviceAttribute : DeviceAttribute
    {
        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="turnstileType">Тип турникета</param>
        public TurnstileDeviceAttribute(String turnstileType)
            : base(turnstileType)
        {
        }

        /// <summary>
        /// Тип турникета
        /// </summary>
        public String TurnstileType
        {
            get { return base.DeviceType; }
        }
    }

    /// <summary>
    /// Атрибут реализации считывателя
    /// </summary>
    public sealed class GenericReaderAttribute : DeviceAttribute
    {
        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="readerType">Тип считывателя</param>
        public GenericReaderAttribute(String readerType)
            : base(readerType)
        {
        }

        /// <summary>
        /// Тип считывателя
        /// </summary>
        public String ReaderType
        {
            get { return base.DeviceType; }
        }
    }

    /// <summary>
    /// Атрибут реализации SMS-клиента
    /// </summary>
    public sealed class SMSClientAttribute : DeviceAttribute
    {
        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="smsClientType">Тип SMS-клиента</param>
        public SMSClientAttribute(String smsClientType)
            : base(smsClientType)
        {
        }

        /// <summary>
        /// Тип SMS-клиента
        /// </summary>
        public String SMSClientType
        {
            get { return base.DeviceType; }
        }
    }

	/// <summary>
	/// Атрибут драйвера весов
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ScaleAttribute : DeviceAttribute
	{
		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="scaleType">Тип весов</param>
		public ScaleAttribute(String scaleType) : base(scaleType)
		{
		}

		/// <summary>
		/// Тип весов
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
	/// Атрибут драйвера дисплея покупателя
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class CustomerDisplayAttribute : DeviceAttribute
	{
		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="displayType">Тип дисплея покупателя</param>
		public CustomerDisplayAttribute(String displayType) : base(displayType)
		{
		}

		/// <summary>
		/// Тип дисплея покупателя
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
	/// Атрибут драйвера фискального регистратора
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class FiscalDeviceAttribute : DeviceAttribute
	{
		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="fiscalType">Тип фискального регистратора</param>
		public FiscalDeviceAttribute(String fiscalType) : base(fiscalType)
		{
		}

		/// <summary>
		/// Тип фискального регистратора
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
	/// Атрибут драйвера печатающего устройства
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class PrintableDeviceAttribute : DeviceAttribute
	{
		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="printerType">Тип печатающего устройства</param>
		public PrintableDeviceAttribute(String printerType) : base(printerType)
		{
		}

		/// <summary>
		/// Тип печатающего устройства
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
	/// Атрибут драйвера сканера штрихкода
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class SerialScannerAttribute : DeviceAttribute
	{
		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="scannerType">Тип сканера штрихкода</param>
		public SerialScannerAttribute(String scannerType) : base(scannerType)
		{
		}

		/// <summary>
		/// Тип сканера штрихкода
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
    /// Атрибут модуля управления бильярдом
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class BilliardsManagerAttribute : DeviceAttribute
    {
        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="billiardsManagerType">Тип модуля управления бильярдом</param>
        public BilliardsManagerAttribute(String billiardsManagerType)
            : base(billiardsManagerType)
        {
        }

        /// <summary>
        /// Тип модуля управления бильярдом
        /// </summary>
        public String BilliardsManagerType
        {
            get { return base.DeviceType; }
        }
    }
}
