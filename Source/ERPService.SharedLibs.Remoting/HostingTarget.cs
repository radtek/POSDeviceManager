using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.Remoting
{
    /// <summary>
    /// Базовый класс для объектов, предназначенных для хостинга на ремоутинг-сервере
    /// </summary>
    public abstract class HostingTarget : MarshalByRefObject, IDisposable
    {
        /// <summary>
        /// "Имя" объекта, используется для формирования URI объекта
        /// </summary>
        /// <example>http://127.0.0.1/someObject</example>
        public abstract string Name { get; }

        /// <summary>
        /// Порт, на котором будет доступен объект
        /// </summary>
        public abstract int Port { get; }

        #region Реализация IDisposable Members

        /// <summary>
        /// Действия по освобождению различных ресурсов
        /// </summary>
        public virtual void Dispose()
        {
        }

        #endregion
    }
}
