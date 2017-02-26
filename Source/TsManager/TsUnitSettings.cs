using System;
using System.ComponentModel;

namespace TsManager
{
    /// <summary>
    /// Настройки турникета
    /// </summary>
    [Serializable]
    public class TsUnitSettings
    {
        private string _name;
        private string _hostOrIp;
        private string _deviceId;
        private int _port;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        public TsUnitSettings()
        {
            _name = string.Empty;
            _hostOrIp = "localhost";
            _deviceId = string.Empty;
            _port = 35100;
        }

        /// <summary>
        /// Имя хоста для подключения к диспетчеру устройств
        /// </summary>
        [DisplayName("Имя хоста")]
        [Description("Имя хоста для подключения к \"Форинт-С: Диспетчер устройств\"")]
        [Category("Подключение")]
        [DefaultValue("localhost")]
        public string HostOrIp
        {
            get { return _hostOrIp; }
            set { _hostOrIp = value; }
        }

        /// <summary>
        /// Порт для подключения к диспетчеру устройств
        /// </summary>
        [DisplayName("Порт")]
        [Description("TCP-порт для подключения к \"Форинт-С: Диспетчер устройств\"")]
        [Category("Подключение")]
        [DefaultValue(35100)]
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        /// <summary>
        /// Идентификатор устройства
        /// </summary>
        [DisplayName("Устройство")]
        [Description("Идентификатор устройства в конфигурации диспетчера устройств")]
        [Category("Прочее")]
        public string DeviceId
        {
            get { return _deviceId; }
            set { _deviceId = value; }
        }

        /// <summary>
        /// Наименование турникета
        /// </summary>
        [DisplayName("Наименование")]
        [Description("Наименование турникета")]
        [Category("Прочее")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Строковое представление объекта
        /// </summary>
        public override string ToString()
        {
            return string.Format("\"{0}\", [{1}:{2}/{3}]", _name, _hostOrIp, _port, _deviceId);
        }
    }
}
