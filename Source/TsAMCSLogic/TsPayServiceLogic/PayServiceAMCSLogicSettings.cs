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

        [DisplayName("��� �����")]
        [Description("��� ����� ��� ��� IP-����� ��� ����������� � \"������-�: ������� � ������\"")]
        [Category("������")]
        [DefaultValue("localhost")]
        public string HostOrIp
        {
            get { return _hostOrIp; }
            set { _hostOrIp = value; }
        }

        [DisplayName("����")]
        [Description("TCP-���� ��� ����������� � \"������-�: ������� � ������\"")]
        [Category("������")]
        [DefaultValue(34601)]
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }
    }
}
