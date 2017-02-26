using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.PropertyGrid.Editors
{
    /// <summary>
    /// Редактор свойств-дней недели
    /// </summary>
    public class DayOfWeekEditor : CustomDropdownEditor
    {
        /// <summary>
        /// Возвращает список возможных значений свойства
        /// </summary>
        public override string[] Values
        {
            get
            {
                return new string[] {
                    "Понедельник",
                    "Вторник",
                    "Среда",
                    "Четверг",
                    "Пятница",
                    "Суббота",
                    "Воскресенье"};
            }
        }

        /// <summary>
        /// Возвращает индекс выбранного значения
        /// </summary>
        /// <param name="value">Исходное значение свойства</param>
        protected override int ObjectToIndex(Object value)
        {
            return (int)value - 1;
        }

        /// <summary>
        /// Конвертирует выбранное строковое значение в нужный тип
        /// </summary>
        /// <param name="selectedIndex">Идекс строки в списке</param>
        protected override Object IndexToObject(int selectedIndex)
        {
            return selectedIndex + 1;
        }
    }
}
