using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using DevicesCommon;
using DevicesCommon.Helpers;
using DevicesBase.Helpers;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace DevicesBase
{
	/// <summary>
	/// Базовый класс для всех устройств, подключаемых через последовательные и 
    /// параллельные порты
	/// </summary>
	public abstract class CustomSerialDevice : CustomConnectableDevice, 
        ISerialDevice, IDisposable
	{
        private String _portName;
        private Int32 _baudRate;
        private Boolean _portCaptured;
        private Boolean _portOpened;
        private Boolean _portActivated;
        private Boolean _blockPortGetterReentrancy;
        private Boolean _disposed;

        #region Закрытые методы

        private EasyCommunicationPort CaptureAndGetPort()
        {
            EasyCommunicationPort port;

            if (_portCaptured)
            {
                port = SerialPortsPool.GetPort(DeviceId, _portName);
            }
            else
            {
                port = SerialPortsPool.CapturePort(DeviceId, _portName, false, TimeSpan.MinValue);
                _portCaptured = true;
            }

            return port;
        }

        private void OpenPort(EasyCommunicationPort port)
        {
            if (_portOpened)
                return;

            port.SetCommStateEvent += SetCommStateEventHandler;
            try
            {
                // дополнительные действия до открытия порта
                OnBeforeActivate();

                // открываем порт на заданной скорости
                port.BaudRate = _baudRate;
                port.Open();

                _portOpened = true;
            }
            catch (Win32Exception e)
            {
                // устанавливаем код ошибки
                ErrorCode = new ServerErrorCode(this, e);
                throw;
            }
            finally
            {
                port.SetCommStateEvent -= SetCommStateEventHandler;
            }
        }

        private void ActivatePort(EasyCommunicationPort port)
        {
            if (_portActivated)
                return;

            try
            {
                // дополнительные действия после открытия порта
                OnAfterActivate();
                _portActivated = true;
            }
            catch (Win32Exception e)
            {
                ErrorCode = new ServerErrorCode(this, e);
                throw;
            }
        }

        #endregion

        #region Защищенные свойства и методы

        /// <summary>
		/// Создает устройство, подключаемое к последовательному порту
		/// </summary>
		protected CustomSerialDevice() : base()
		{
            _portName = "COM1";
            _baudRate = 9600;
		}

        /// <summary>
        /// Возвращает признак работы по последовательному порту
        /// </summary>
        protected virtual Boolean IsSerial
        {
            get { return Port.IsSerial; }
        }

        /// <summary>
        /// Коммуникационный порт
        /// </summary>
        protected virtual EasyCommunicationPort Port
        {
            get 
            {
                var port = CaptureAndGetPort();

                if (!_blockPortGetterReentrancy)
                {
                    // недопускаем реентерабельности этого блока,
                    // которая возможна из-за обращения к свойству Port из методов
                    // OnBeforeActivate и OnAfterActivate

                    _blockPortGetterReentrancy = true;
                    try
                    {
                        OpenPort(port);
                        ActivatePort(port);
                    }
                    finally
                    {
                        _blockPortGetterReentrancy = false;
                    }
                }

                return port;
            }
        }

		#endregion

		#region Методы, реализуемые в классах-потомках

		/// <summary>
		/// Вызывается перед активацией устройства
		/// </summary>
		protected virtual void OnBeforeActivate()
		{
		}

		/// <summary>
		/// Вызывается после активации устройства
		/// </summary>
		protected virtual void OnAfterActivate()
		{
		}

        /// <summary>
        /// Вызывается перед деактивацией устройства
        /// </summary>
        protected virtual void OnBeforeDeactivate()
        {
        }

        /// <summary>
        /// Вызывается после деактивации устройства
        /// </summary>
        protected virtual void OnAfterDeactivate()
        {
        }

        /// <summary>
        /// Обработчик события расширенной настройки параметров порта
        /// </summary>
        /// <param name="sender">Коммуникационный порт</param>
        /// <param name="e">Аргументы события</param>
        protected virtual void SetCommStateEventHandler(Object sender, CommStateEventArgs e)
        {
        }

		#endregion

		#region Реализация ISerialDevice

		/// <summary>
		/// Скорость передачи данных через порт
		/// </summary>
		public Int32 Baud
		{
			get { return _baudRate; }
			set { _baudRate = value; }
		}

		/// <summary>
		/// Имя порта (напр., COM1, LPT1...)
		/// </summary>
		public override String PortName
		{
			get { return _portName; }
            set { _portName = value; } 
		}

		/// <summary>
		/// Подключение и отключение устройства
		/// </summary>
        /// <remarks>
        /// Подключение устройств сделано ленивым. См. <see cref="CustomSerialDevice.Port"/>.
        /// </remarks>
		public override Boolean Active
		{
            get 
            {
                // теперь всегда считаем устройство активным,
                // т.к. открытие порта сделано ленивым
                return true;
            }
            set
			{
                // просто игнорируем;
                // порт будет закрываться пулом
			}
		}

        #endregion

        #region Реализация IDisposable

        /// <summary>
		/// Освобождение ресурсов
		/// </summary>
		public virtual void Dispose()
		{
            if (_disposed)
                return;

            if (_portCaptured)
            {
                // дополнительные действия до освобождения порта
                if (_portActivated)
                {
                    OnBeforeDeactivate();
                    _portActivated = false;
                }

                // осовобождаем порт
                SerialPortsPool.ReleasePort(DeviceId, _portName);
                _portCaptured = false;
                
                // дополнительные действия после освобождения порта
                OnAfterDeactivate();
            }

            _disposed = true;
        }

		#endregion
    }
}
