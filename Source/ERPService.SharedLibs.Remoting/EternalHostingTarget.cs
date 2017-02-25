using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.Remoting
{
    /// <summary>
    /// Базовый класс для объектов, предназначенных для хостинга на ремоутинг-сервере c 
    /// "бесконечным" временем жизни
    /// </summary>
    public abstract class EternalHostingTarget : HostingTarget
    {
        /// <summary>
        /// Определяет время жизни объекта на сервере
        /// </summary>
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
