using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;

namespace ERPService.SharedLibs.PropertyGrid
{
    /// <summary>
    /// Базовый класс для редакторов свойств, отображающих выпадающий список
    /// </summary>
    public abstract class CustomDropdownEditor : CustomEditor
    {
        /// <summary>
        /// Gets the editor style used by the EditValue method
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that can be used to gain additional context information</param>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            // реактирование будет производится в диалоговом окне
            return UITypeEditorEditStyle.DropDown;
        }

        /// <summary>
        /// Возвращает измененное значение свойства
        /// </summary>
        /// <param name="value">Исходное значение</param>
        protected override Object OnEdit(Object value)
        {
            // создаем выпадающий список значений
            ListBox valuesList = new ListBox();
            valuesList.BorderStyle = BorderStyle.None;
            valuesList.BeginUpdate();
            try
            {
                // заполняем список значениями
                valuesList.Items.AddRange(Values);
            }
            finally
            {
                valuesList.EndUpdate();
            }
            // определяем высоту списка
            Int32 heightMultiplier = valuesList.Items.Count > 7 ? 7 : valuesList.Items.Count;
            valuesList.Height = valuesList.ItemHeight * (heightMultiplier + 1);
            // выбираем строку в списке в зависимости от значения свойства
            valuesList.SelectedIndex = ObjectToIndex(value);
            // добавляем поддержку закрытия по щелчку мыши
            valuesList.Click += new EventHandler(valuesList_Click);
            // открываем список значений
            EdSvc.DropDownControl(valuesList);
            // возвращаем выбранное значение свойства
            return IndexToObject(valuesList.SelectedIndex);
        }

        /// <summary>
        /// Закрытие списка по двойному щелчку мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void valuesList_Click(Object sender, EventArgs e)
        {
            EdSvc.CloseDropDown();
        }

        /// <summary>
        /// Возвращает список возможных значений свойства
        /// </summary>
        abstract public String[] Values { get; }

        /// <summary>
        /// Возвращает индекс выбранного значения
        /// </summary>
        /// <param name="value">Исходное значение свойства</param>
        abstract protected Int32 ObjectToIndex(Object value);

        /// <summary>
        /// Конвертирует выбранное строковое значение в нужный тип
        /// </summary>
        /// <param name="selectedIndex">Идекс строки в списке</param>
        abstract protected Object IndexToObject(Int32 selectedIndex);
    }
}
