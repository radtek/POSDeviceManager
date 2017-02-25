using System;
using System.Collections.Generic;
using System.Text;

namespace TsManager
{
    /// <summary>
    /// Атрибут, которым следует помечать реализацию логики работы СКУД
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AMCSLogicAttrubute : Attribute
    {
        private String _amcsName;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="amcsName">Наименование СКУД</param>
        public AMCSLogicAttrubute(String amcsName)
            : base()
        {
            _amcsName = amcsName;
        }

        /// <summary>
        /// Наименование СКУД
        /// </summary>
        public String AMCSName
        {
            get { return _amcsName; }
        }
    }
}
