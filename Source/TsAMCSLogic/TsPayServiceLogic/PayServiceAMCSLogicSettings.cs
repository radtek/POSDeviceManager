using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace TsPayServiceLogic
{
    [Serializable]
    public class PayServiceAMCSLogicSettings
    {
        private String _hostOrIp;
        private Int32 _port;

        public PayServiceAMCSLogicSettings()
        {
            _hostOrIp = "localhost";
            _port = 34601;
        }

        [DisplayName("Имя хоста")]
        [Description("Имя хоста или его IP-адрес для подключения к \"Форинт-С: Платежи и скидки\"")]
        [Category("Прочее")]
        [DefaultValue("localhost")]
        public String HostOrIp
        {
            get { return _hostOrIp; }
            set { _hostOrIp = value; }
        }

        [DisplayName("Порт")]
        [Description("TCP-порт для подключения к \"Форинт-С: Платежи и скидки\"")]
        [Category("Прочее")]
        [DefaultValue(34601)]
        public Int32 Port
        {
            get { return _port; }
            set { _port = value; }
        }
    }
}
