using System;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace DevicesBase.Helpers
{
    /// <summary>
    /// Вспомогательный класс для организации пула портов
    /// </summary>
    internal class SerialPortsHelper : IDisposable
    {
        private string _deviceId;
        private EasyCommunicationPort _port;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="portName">Имя порта</param>
        public SerialPortsHelper(string portName)
        {
            _port = new EasyCommunicationPort();
            _port.PortName = portName;
            _deviceId = string.Empty;
        }

        /// <summary>
        /// Имя порта
        /// </summary>
        public string PortName
        {
            get { return _port.PortName; }
        }

        /// <summary>
        /// Идентификатор устройства
        /// </summary>
        public string DeviceId
        {
            get { return _deviceId; }
            set { _deviceId = value; }
        }

        /// <summary>
        /// Коммуникационный порт
        /// </summary>
        public EasyCommunicationPort Port
        {
            get { return _port; }
        }

        #region IDisposable Members

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public void Dispose()
        {
            if (_port != null)
            {
                _port.Dispose();
                _port = null;
            }
        }

        #endregion
    }
}
