using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace TsManager
{
    /// <summary>
    /// ��������� ���������
    /// </summary>
    [Serializable]
    public class TsUnitSettings
    {
        private String _name;
        private String _hostOrIp;
        private String _deviceId;
        private Int32 _port;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        public TsUnitSettings()
        {
            _name = String.Empty;
            _hostOrIp = "localhost";
            _deviceId = String.Empty;
            _port = 35100;
        }

        /// <summary>
        /// ��� ����� ��� ����������� � ���������� ���������
        /// </summary>
        [DisplayName("��� �����")]
        [Description("��� ����� ��� ����������� � \"������-�: ��������� ���������\"")]
        [Category("�����������")]
        [DefaultValue("localhost")]
        public String HostOrIp
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
        public Int32 Port
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
        public String DeviceId
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
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// ��������� ������������� �������
        /// </summary>
        public override String ToString()
        {
            return String.Format("\"{0}\", [{1}:{2}/{3}]", _name, _hostOrIp, _port, _deviceId);
        }
    }
}
