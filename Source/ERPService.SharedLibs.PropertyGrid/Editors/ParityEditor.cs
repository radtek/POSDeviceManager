using System;
using System.Collections.Generic;
using System.Text;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace ERPService.SharedLibs.PropertyGrid.Editors
{
    /// <summary>
    /// Редактор значений контроля четности
    /// </summary>
    public class ParityEditor : CustomDropdownEditor
    {
        /// <summary>
        /// Возвращает список возможных значений свойства
        /// </summary>
        public override string[] Values
        {
            get
            {
                return new string[] { 
                    "Нет", 
                    "Нечетное",
                    "Четное",
                    "По единичному биту чётности",
                    "По нулевому биту чётности"
                };
            }
        }

        /// <summary>
        /// Возвращает индекс выбранного значения
        /// </summary>
        /// <param name="value">Исходное значение свойства</param>
        protected override int ObjectToIndex(Object value)
        {
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// Конвертирует выбранное строковое значение в нужный тип
        /// </summary>
        /// <param name="selectedIndex">Идекс строки в списке</param>
        protected override Object IndexToObject(int selectedIndex)
        {
            return (Parity)selectedIndex;
        }
    }
}
