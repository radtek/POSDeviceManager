using System;

namespace ERPService.SharedLibs.PropertyGrid.Forms
{
    /// <summary>
    /// Форма редактора свойств, представляющих сложные типы
    /// </summary>
    public partial class FormComplexPropertyEditor : FormModalEditor
    {
        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        public FormComplexPropertyEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Редактируемое значение
        /// </summary>
        public override Object Value
        {
            get
            {
                return propertyGrid1.SelectedObject;
            }
            set
            {
                propertyGrid1.SelectedObject = value;
            }
        }
    }
}
