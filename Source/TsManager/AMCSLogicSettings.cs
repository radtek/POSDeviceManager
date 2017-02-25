using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using ERPService.SharedLibs.PropertyGrid.Converters;

namespace TsManager
{
    /// <summary>
    /// Настройки реализации логики работы СКУД
    /// </summary>
    [Serializable]
    public class AMCSLogicSettings
    {
        private List<TsUnitSettings> _units;
        private Object _logicSettings;
        private String _acmsName;
        private String _name;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        public AMCSLogicSettings()
        {
            _units = new List<TsUnitSettings>();
            _logicSettings = null;
            _acmsName = String.Empty;
            _name = String.Empty;
        }

        /// <summary>
        /// Список настроек турникетов
        /// </summary>
        [Browsable(false)]
        public List<TsUnitSettings> Units
        {
            get { return _units; }
            set { _units = value; }
        }

        /// <summary>
        /// Параметры реализации логики работы СКУД
        /// </summary>
        [DisplayName("Параметры")]
        [Category("Прочее")]
        [Description("Параметры, необходимые для работы модуля")]
        [Editor(typeof(AMCSLogicEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(HideValueConverter))]
        public Object LogicSettings
        {
            get { return _logicSettings; }
            set { _logicSettings = value; }
        }

        /// <summary>
        /// Наименование СКУД
        /// </summary>
        [ReadOnly(true)]
        [DisplayName("Наименование СКУД")]
        [Category("Прочее")]
        [Description("Наименование СКУД, логика которой реализована в этом модуле")]
        public String AcmsName
        {
            get { return _acmsName; }
            set { _acmsName = value; }
        }

        /// <summary>
        /// Наименование элемента
        /// </summary>
        [DisplayName("Наименование элемента")]
        [Category("Прочее")]
        [Description("Наименование элемента в конфигурации менеджера турникетов")]
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

    }
}
