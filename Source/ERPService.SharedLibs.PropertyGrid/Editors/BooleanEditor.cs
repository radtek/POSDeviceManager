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
        public override String[] Values
        {
            get { return new String[] { "Да", "Нет" }; }
        }

        /// <summary>
        /// Возвращает индекс выбранного значения
        /// </summary>
        /// <param name="value">Исходное значение свойства</param>
        protected override Int32 ObjectToIndex(Object value)
        {
            return (Boolean)value ? 0 : 1;
        }

        /// <summary>
        /// Конвертирует выбранное строковое значение в нужный тип
        /// </summary>
        /// <param name="selectedIndex">Идекс строки в списке</param>
        protected override Object IndexToObject(Int32 selectedIndex)
        {
            return selectedIndex == 0;
        }
    }
}
