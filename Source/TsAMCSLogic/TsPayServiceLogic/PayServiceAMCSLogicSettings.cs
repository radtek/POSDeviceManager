using System;
using System.ComponentModel;

namespace TsPayServiceLogic
{
    [Serializable]
    public class PayServiceAMCSLogicSettings
    {
        private string _hostOrIp;
        private int _port;

        public PayServiceAMCSLogicSettings()
        {
            _hostOrIp = "localhost";
            _port = 34601;
        }

        [DisplayName("Имя хоста")]
        [Description("Имя хоста или его IP-адрес для подключения к \"Форинт-С: Платежи и скидки\"")]
        [Category("Прочее")]
        [DefaultValue("localhost")]
        public string HostOrIp
        {
            get { return _hostOrIp; }
            set { _hostOrIp = value; }
        }

        [DisplayName("Порт")]
        [Description("TCP-порт для подключения к \"Форинт-С: Платежи и скидки\"")]
        [Category("Прочее")]
        [DefaultValue(34601)]
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }
    }
}
