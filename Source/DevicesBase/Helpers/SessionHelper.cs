using System;

namespace DevicesBase.Helpers
{
    /// <summary>
    /// Вспомогательный класс для работы с клиентскими сессиями
    /// </summary>
    internal class SessionHelper
    {
        private DateTime accessDateTime;
        private string sessionID;
        private int sessionTimeout;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="sessionID">Идентификатор сессии</param>
        /// <param name="sessionTimeout">Таймаут сессии</param>
        public SessionHelper(string sessionID, int sessionTimeout)
        {
            this.sessionID = sessionID;
            this.sessionTimeout = sessionTimeout;
            accessDateTime = DateTime.Now;
        }

        /// <summary>
        /// Дата и время последнего обращения клиента
        /// </summary>
        public DateTime AccessDateTime
        {
            get
            {
                return accessDateTime;
            }
            set
            {
                accessDateTime = value;
            }
        }

        /// <summary>
        /// Идентификатор сесии
        /// </summary>
        public string SessionID
        {
            get
            {
                return sessionID;
            }
        }

        /// <summary>
        /// Признак актуальности клиентской сессии
        /// </summary>
        public bool Alive
        {
            get
            {
                if (sessionTimeout > 0)
                    // таймаут сессии задан
                    return DateTime.Now.Subtract(accessDateTime).Seconds < sessionTimeout;
                else
                    // таймаут сессии - бесконечный
                    return true;
            }
        }
    }
}
