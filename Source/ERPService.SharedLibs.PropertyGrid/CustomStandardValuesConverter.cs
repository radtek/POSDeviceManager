using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace ERPService.SharedLibs.PropertyGrid
{
    /// <summary>
    /// Ѕазовый класс-конвертор дл€ свойств, чьи значени€ выбираютс€ из списка
    /// </summary>
    public abstract class CustomStandardValuesConverter : StringConverter
    {
        /// <summary>
        /// Returns whether this object supports a standard set of values that can be picked from a list,
        /// using the specified context
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context</param>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            // да, выбор значений из списка
            return true;
        }

        /// <summary>
        /// Returns whether the collection of standard values returned from GetStandardValues is an 
        /// exclusive list of possible values, using the specified context
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context</param>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            // значени€ можно вводить вручную
            return false;
        }

        /// <summary>
        /// Returns a collection of standard values for the data type this type converter is designed for
        /// when provided with a format context
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context</param>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            // возвращаем список строк из настроек программы
            // (базы данных, интернет и т.д.)
            return new StandardValuesCollection(StandardValues);
        }

        /// <summary>
        /// ¬озвращает список стандартных значений
        /// </summary>
        abstract protected String[] StandardValues { get; }
    }
}
