using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.PropertyGrid.Editors
{
    /// <summary>
    /// Редактор значений типа System.Boolean
    /// </summary>
    [Obsolete]
    public class BooleanEditor : CustomDropdownEditor
    {
        /// <summary>
        /// Возвращает список возможных значений свойства
        /// </summary>
        public override string[] Values
        {
            get { return new string[] { "Да", "Нет" }; }
        }

        /// <summary>
        /// Возвращает индекс выбранного значения
        /// </summary>
        /// <param name="value">Исходное значение свойства</param>
        protected override int ObjectToIndex(Object value)
        {
            return (bool)value ? 0 : 1;
        }

        /// <summary>
        /// Конвертирует выбранное строковое значение в нужный тип
        /// </summary>
        /// <param name="selectedIndex">Идекс строки в списке</param>
        protected override Object IndexToObject(int selectedIndex)
        {
            return selectedIndex == 0;
        }
    }
}
