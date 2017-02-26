using System;
using System.Collections.Generic;
using DevicesBase.Helpers;
using DevicesCommon;
using DevicesCommon.Helpers;

namespace DevicesBase
{
    /// <summary>
    /// Базовый класс для всех устройств
    /// </summary>
    public abstract class CustomDevice : MarshalByRefObject, IDevice
	{
        #region Закрытые поля класса

        // идентификатор устройства
        private string deviceId;
        // таблица описаний кодов ошибок, специфических для протокола обмена
        private Dictionary<Int16, string> specificErrors;
        // интерфейс для протоколирования работы устройства
        private ILogger logger;
        // интерфейс для доступа к пулу последовательных портов
        private ISerialPortsPool _serialPortsPool;
        // код ошибки
        private ErrorCode _errorCode;

        #endregion

        #region Открытые свойства и методы

        /// <summary>
        /// Пул последовательных портов
        /// </summary>
        public ISerialPortsPool SerialPortsPool
        {
            get { return _serialPortsPool; }
            set { _serialPortsPool = value; }
        }

        #endregion

        #region Конструктор

        /// <summary>
		/// Создает устройство общего назначения
		/// </summary>
		protected CustomDevice()
		{
            deviceId = string.Empty;
            specificErrors = new Dictionary<Int16, string>();
            ErrorCode = new ServerErrorCode(this, GeneralError.Success);
		}

		#endregion

		#region Свойства и методы, доступные из потомков

        /// <summary>
        /// Позволяет добавить описание ошибки протокола обмена с устройством в таблицу
        /// описаний кодов ошибок
        /// </summary>
        /// <param name="specificCode">Код ошибки протокола обмена с устройством</param>
        /// <param name="specificDescription">Описание кода ошибки протокола обмена с устройством</param>
        protected void AddSpecificError(Int16 specificCode, string specificDescription)
        {
            specificErrors.Add(specificCode, specificDescription);
        }

        /// <summary>
        /// Возвращает описание кода ошибки протокола обмена с устройством
        /// </summary>
        /// <param name="specificCode">Код ошибки</param>
        /// <returns>Описание кода ошибки протокола обмена с устройством</returns>
        protected string GetSpecificDescription(Int16 specificCode)
        {
            return specificErrors.ContainsKey(specificCode) ?
                specificErrors[specificCode] : string.Format("Не найдено описание для ошибки с кодом {0}", specificCode);
        }

		#endregion

		#region Реализация IDevice

		/// <summary>
		/// Подключение и отключение устройства
		/// </summary>
		public abstract bool Active { get; set; }

		/// <summary>
		/// Идентификатор устройства
		/// </summary>
		public string DeviceId
		{
			get { return deviceId; }
			set { deviceId = value; }
		}

        /// <summary>
        /// Код ошибки
        /// </summary>
        public ErrorCode ErrorCode
        {
            get { return _errorCode; }
            protected set 
            {
                _errorCode = new ErrorCode(value.Sender, value.Value, value.Description,
                    value.SpecificValue, value.SpecificDescription);
                //_errorCode = value; 
            }
        }

        /// <summary>
        /// Интерфейс для протоколирования работы устройства
        /// </summary>
        public ILogger Logger 
        {
            get { return logger; }
            set { logger = value; }
        }

		#endregion
	}
}
