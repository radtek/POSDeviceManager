using System;

namespace DevicesCommon.Helpers
{
    /// <summary>
    /// Атрибут для класса диспетчера устройств
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class DeviceManagerAttribute : Attribute
    {
	}

	/// <summary>
	/// Базовый атрибут для драйверов устройств
	/// </summary>
	public abstract class DeviceAttribute : Attribute
    {
		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="deviceType">Тип устройства</param>
		protected DeviceAttribute(string deviceType)
		{
			DeviceType = deviceType;
		}

		/// <summary>
		/// Тип устройства
		/// </summary>
		public string DeviceType { get; }
	}

    /// <summary>
    /// Атрибут реализации модуля управления турникетом
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class TurnstileDeviceAttribute : DeviceAttribute
    {
        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="turnstileType">Тип турникета</param>
        public TurnstileDeviceAttribute(string turnstileType)
            : base(turnstileType)
        {
        }
    }

    /// <summary>
    /// Атрибут реализации считывателя
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class GenericReaderAttribute : DeviceAttribute
    {
        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="readerType">Тип считывателя</param>
        public GenericReaderAttribute(string readerType)
            : base(readerType)
        {
        }
    }

    /// <summary>
    /// Атрибут реализации SMS-клиента
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class SMSClientAttribute : DeviceAttribute
    {
        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="smsClientType">Тип SMS-клиента</param>
        public SMSClientAttribute(string smsClientType)
            : base(smsClientType)
        {
        }
    }

	/// <summary>
	/// Атрибут драйвера весов
	/// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ScaleAttribute : DeviceAttribute
	{
		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="scaleType">Тип весов</param>
		public ScaleAttribute(string scaleType) 
            : base(scaleType)
		{
		}
	}

    /// <summary>
    /// Атрибут драйвера дисплея покупателя
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class CustomerDisplayAttribute : DeviceAttribute
	{
		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="displayType">Тип дисплея покупателя</param>
		public CustomerDisplayAttribute(string displayType) 
            : base(displayType)
		{
		}
	}

    /// <summary>
    /// Атрибут драйвера фискального регистратора
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class FiscalDeviceAttribute : DeviceAttribute
	{
		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="fiscalType">Тип фискального регистратора</param>
		public FiscalDeviceAttribute(string fiscalType)
            : base(fiscalType)
		{
		}
	}

    /// <summary>
    /// Атрибут драйвера печатающего устройства
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class PrintableDeviceAttribute : DeviceAttribute
	{
		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="printerType">Тип печатающего устройства</param>
		public PrintableDeviceAttribute(string printerType)
            : base(printerType)
		{
		}
	}

    /// <summary>
    /// Атрибут драйвера сканера штрихкода
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class SerialScannerAttribute : DeviceAttribute
	{
		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="scannerType">Тип сканера штрихкода</param>
		public SerialScannerAttribute(string scannerType)
            : base(scannerType)
		{
		}
	}

    /// <summary>
    /// Атрибут модуля управления бильярдом
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class BilliardsManagerAttribute : DeviceAttribute
    {
        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="billiardsManagerType">Тип модуля управления бильярдом</param>
        public BilliardsManagerAttribute(string billiardsManagerType)
            : base(billiardsManagerType)
        {
        }
    }
}
