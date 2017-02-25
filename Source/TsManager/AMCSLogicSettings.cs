using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using ERPService.SharedLibs.PropertyGrid.Converters;

namespace TsManager
{
    /// <summary>
    /// ��������� ���������� ������ ������ ����
    /// </summary>
    [Serializable]
    public class AMCSLogicSettings
    {
        private List<TsUnitSettings> _units;
        private Object _logicSettings;
        private String _acmsName;
        private String _name;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        public AMCSLogicSettings()
        {
            _units = new List<TsUnitSettings>();
            _logicSettings = null;
            _acmsName = String.Empty;
            _name = String.Empty;
        }

        /// <summary>
        /// ������ �������� ����������
        /// </summary>
        [Browsable(false)]
        public List<TsUnitSettings> Units
        {
            get { return _units; }
            set { _units = value; }
        }

        /// <summary>
        /// ��������� ���������� ������ ������ ����
        /// </summary>
        [DisplayName("���������")]
        [Category("������")]
        [Description("���������, ����������� ��� ������ ������")]
        [Editor(typeof(AMCSLogicEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(HideValueConverter))]
        public Object LogicSettings
        {
            get { return _logicSettings; }
            set { _logicSettings = value; }
        }

        /// <summary>
        /// ������������ ����
        /// </summary>
        [ReadOnly(true)]
        [DisplayName("������������ ����")]
        [Category("������")]
        [Description("������������ ����, ������ ������� ����������� � ���� ������")]
        public String AcmsName
        {
            get { return _acmsName; }
            set { _acmsName = value; }
        }

        /// <summary>
        /// ������������ ��������
        /// </summary>
        [DisplayName("������������ ��������")]
        [Category("������")]
        [Description("������������ �������� � ������������ ��������� ����������")]
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

    }
}
