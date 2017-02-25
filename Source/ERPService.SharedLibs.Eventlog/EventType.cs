using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// Типы событий 
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// Тип не определен
        /// </summary>
        Undefined,

        /// <summary>
        /// Информация
        /// </summary>
        Information,

        /// <summary>
        /// Ошибка
        /// </summary>
        Error,

        /// <summary>
        /// Предупреждение
        /// </summary>
        Warning
    }
}
