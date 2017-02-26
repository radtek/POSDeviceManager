using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.PropertyGrid.Editors
{
    /// <summary>
    /// –едактор значений свойств-скоростей передачи данных
    /// </summary>
    public class BaudEditor : CustomDropdownEditor
    {
        /// <summary>
        /// ¬озвращает список возможных значений свойства
        /// </summary>
        public override string[] Values
        {
            get
            {
                return new string[] { 
                    "4800", 
                    "9600",
                    "19200",
                    "38400",
                    "57600",
                    "115200"
                };
            }
        }

        /// <summary>
        /// ¬озвращает индекс выбранного значени€
        /// </summary>
        /// <param name="value">»сходное значение свойства</param>
        protected override int ObjectToIndex(Object value)
        {
            return Array.IndexOf<String>(Values, value.ToString());
        }

        /// <summary>
        ///  онвертирует выбранное строковое значение в нужный тип
        /// </summary>
        /// <param name="selectedIndex">»декс строки в списке</param>
        protected override Object IndexToObject(int selectedIndex)
        {
            return Convert.ToInt32(Values[selectedIndex]);
        }
    }
}
