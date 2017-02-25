using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.PropertyGrid.Converters
{
    /// <summary>
    /// Конвертор для <see cref="System.Boolean"/>
    /// </summary>
    public class RussianBooleanConverter : CustomEnumConverter
    {
        /// <summary>
        /// Набор строковых значений, сопоставленных элементам перечисления
        /// </summary>
        protected override String[] StringValues
        {
            get 
            {
                return new String[] { "Да", "Нет" };
            }
        }

        /// <summary>
        /// Набор значений элементов перечисления
        /// </summary>
        protected override Object[] ObjectValues
        {
            get 
            {
                return new Object[] { true, false };
            }
        }
    }
}
