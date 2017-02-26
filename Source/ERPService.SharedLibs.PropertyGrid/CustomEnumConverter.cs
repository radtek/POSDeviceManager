using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace ERPService.SharedLibs.PropertyGrid
{
    /// <summary>
    /// Базовый класс-конвертор для перечислений
    /// </summary>
    public abstract class CustomEnumConverter : TypeConverter
    {
        private Object ThrowCantConvertException(Object value, Type destinationType)
        {
            throw new InvalidOperationException(string.Format(
                "Значение [{0}] не может быть преобразовано в тип [{1}]", value, destinationType.Name));
        }

        /// <summary>
        /// Returns whether this object supports a standard set of values that can be picked from a list,
        /// using the specified context
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context</param>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Returns whether the collection of standard values returned from GetStandardValues is an 
        /// exclusive list of possible values, using the specified context
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context</param>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Returns a collection of standard values for the data type this type converter is designed for
        /// when provided with a format context
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context</param>
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(ObjectValues);
        }

        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context</param>
        /// <param name="sourceType">A Type that represents the type you want to convert from</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            else
                return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context</param>
        /// <param name="culture">The CultureInfo to use as the current culture</param>
        /// <param name="value">The Object to convert</param>
        /// <returns>An Object that represents the converted value</returns>
        public override Object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, Object value)
        {
            if (value.GetType() == typeof(string))
            {
                for (int i = 0; i < StringValues.Length; i++)
                {
                    if (string.Compare(value.ToString(), StringValues[i], culture, CompareOptions.None) == 0)
                        return ObjectValues[i];
                }
                return ThrowCantConvertException(value, context.PropertyDescriptor.PropertyType);
            }
            else
                return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context</param>
        /// <param name="destinationType">A Type that represents the type you want to convert to</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            else
                return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context</param>
        /// <param name="culture">A CultureInfo. If nullNothingnullptra null reference (Nothing in Visual Basic) is passed, the current culture is assumed</param>
        /// <param name="value">The Object to convert</param>
        /// <param name="destinationType">The Type to convert the value parameter to</param>
        /// <returns>An Object that represents the converted value</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                for (int i = 0; i < ObjectValues.Length; i++)
                {
                    if (value.Equals(ObjectValues[i]))
                        return StringValues[i];
                }
                return ThrowCantConvertException(value, destinationType);
            }
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// Набор строковых значений, сопоставленных элементам перечисления
        /// </summary>
        protected abstract string[] StringValues { get; }
        
        /// <summary>
        /// Набор значений элементов перечисления
        /// </summary>
        protected abstract Object[] ObjectValues { get; }
    }
}
