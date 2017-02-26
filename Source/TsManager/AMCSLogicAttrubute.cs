using System;

namespace TsManager
{
    /// <summary>
    /// Атрибут, которым следует помечать реализацию логики работы СКУД
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AMCSLogicAttrubute : Attribute
    {
        private string _amcsName;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="amcsName">Наименование СКУД</param>
        public AMCSLogicAttrubute(string amcsName)
            : base()
        {
            _amcsName = amcsName;
        }

        /// <summary>
        /// Наименование СКУД
        /// </summary>
        public string AMCSName
        {
            get { return _amcsName; }
        }
    }
}
