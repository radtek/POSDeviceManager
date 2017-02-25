using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.ComponentModel;

namespace ERPService.SharedLibs.PropertyGrid.Converters
{
    /// <summary>
    /// ��������� ��� �������, �������� ������� �� ����� ���������� � 
    /// �������������, � ������� ������������� � ������� ���������
    /// ����������
    /// </summary>
    public class HideValueConverter : TypeConverter
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
            if (destinationType == typeof(String))
            {
                // ���������� ��������� ����������� ��������� �������� ��������
                // � �������������
                return "(������� �� ������ ������)";
            }
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
