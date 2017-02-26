using System;
using System.ComponentModel;
using System.Globalization;
using ERPService.SharedLibs.PropertyGrid.Editors;

namespace ERPService.SharedLibs.PropertyGrid.Converters
{
    /// <summary>
    /// Конвертор значений свойств-скоростей передачи данных
    /// </summary>
    public class BaudConverter : TypeConverter
    {
        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context</param>
        /// <param name="culture">A CultureInfo. If a null reference (Nothing in Visual Basic) is passed, the current culture is assumed</param>
        /// <param name="value">The Object to convert</param>
        /// <param name="destinationType">The Type to convert the value parameter to</param>
        public override Object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, Object value,
            Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                string[] values = new BaudEditor().Values;
                return values[Array.IndexOf<String>(values, value.ToString())];
            }
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
