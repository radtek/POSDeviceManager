using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
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
        private string _acmsName;
        private string _name;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        public AMCSLogicSettings()
        {
            _units = new List<TsUnitSettings>();
            _logicSettings = null;
            _acmsName = string.Empty;
            _name = string.Empty;
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
        public string AcmsName
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
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

    }
}
