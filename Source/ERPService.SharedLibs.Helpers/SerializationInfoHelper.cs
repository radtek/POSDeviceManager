using System;
using System.Runtime.Serialization;

namespace ERPService.SharedLibs.Helpers
{
    /// <summary>
    /// Вспомогательный класс для работы с <see cref="SerializationInfo"/>
    /// </summary>
    public sealed class SerializationInfoHelper
    {
        private SerializationInfo _info;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="info">Информация для сериализации/десериализации</param>
        public SerializationInfoHelper(SerializationInfo info)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            _info = info;
        }

        /// <summary>
        /// Возврат значения для десериализации
        /// </summary>
        /// <typeparam name="T">Тип значения</typeparam>
        /// <param name="name">Наименование значения</param>
        /// <returns>Десериализованное значение</returns>
        public T GetValue<T>(string name)
        {
            return (T)_info.GetValue(name, typeof(T));
        }

        /// <summary>
        /// Добавление значения для сериализации
        /// </summary>
        /// <typeparam name="T">Тип значения</typeparam>
        /// <param name="name">Наименование значения</param>
        /// <param name="value">Значение</param>
        public void AddValue<T>(string name, T value)
        {
            _info.AddValue(name, value, typeof(T));
        }
    }
}
