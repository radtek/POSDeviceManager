using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using ERPService.SharedLibs.PropertyGrid.Forms;

namespace TsManager
{
    /// <summary>
    /// ‘орма редактора настроек реализации логики работы — ”ƒ
    /// </summary>
    public partial class FormAMCSLogicEditor : FormModalEditor
    {
        private Object _obj;
        private Boolean _modified;

        /// <summary>
        /// —оздает экземпл€р класса
        /// </summary>
        public FormAMCSLogicEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// –едактируемое значение
        /// </summary>
        public override Object Value
        {
            get
            {
                return _modified ? _obj : propertyGrid1.SelectedObject;
            }
            set
            {
                // своздаем копию объекта (копируем значени€ публичных свойств)
                Type objType = value.GetType();
                // новый экземпл€р объекта
                _obj = Activator.CreateInstance(objType);
                // копируем значени€ публичных свойств
                foreach (PropertyInfo pInfo in objType.GetProperties(
                    BindingFlags.Public | BindingFlags.Instance))
                {
                    try
                    {
                        Object propertyValue = pInfo.GetValue(value, null);
                        pInfo.SetValue(_obj, propertyValue, null);
                    }
                    catch (ArgumentException)
                    {
                        // свойство не удалось считать или записать
                    }
                }
                propertyGrid1.SelectedObject = _obj;
            }
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            // флаг наличи€ изменений
            _modified = true;
        }
    }
}