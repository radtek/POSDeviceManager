using System;
using System.ComponentModel;

namespace TsManager
{
    /// <summary>
    /// ��������� ���������
    /// </summary>
    [Serializable]
    public class TsUnitSettings
    {
        private string _name;
        private string _hostOrIp;
        private string _deviceId;
        private int _port;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        public TsUnitSettings()
        {
            _name = string.Empty;
            _hostOrIp = "localhost";
            _deviceId = string.Empty;
            _port = 35100;
        }

        /// <summary>
        /// ��� ����� ��� ����������� � ���������� ���������
        /// </summary>
        [DisplayName("��� �����")]
        [Description("��� ����� ��� ����������� � \"������-�: ��������� ���������\"")]
        [Category("�����������")]
        [DefaultValue("localhost")]
        public string HostOrIp
        {
            get { return _hostOrIp; }
            set { _hostOrIp = value; }
        }

        /// <summary>
        /// ���� ��� ����������� � ���������� ���������
        /// </summary>
        [DisplayName("����")]
        [Description("TCP-���� ��� ����������� � \"������-�: ��������� ���������\"")]
        [Category("�����������")]
        [DefaultValue(35100)]
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        /// <summary>
        /// ������������� ����������
        /// </summary>
        [DisplayName("����������")]
        [Description("������������� ���������� � ������������ ���������� ���������")]
        [Category("������")]
        public string DeviceId
        {
            get { return _deviceId; }
            set { _deviceId = value; }
        }

        /// <summary>
        /// ������������ ���������
        /// </summary>
        [DisplayName("������������")]
        [Description("������������ ���������")]
        [Category("������")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// ��������� ������������� �������
        /// </summary>
        public override string ToString()
        {
            return string.Format("\"{0}\", [{1}:{2}/{3}]", _name, _hostOrIp, _port, _deviceId);
        }
    }
}
