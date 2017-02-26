using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon;
using DevicesCommon.Helpers;

namespace DevicesBase.Helpers
{
    /// <summary>
    /// Вспомогательный класс для работы с таблицей устройств
    /// </summary>
    internal class DeviceHelper
    {
        IDevice device;
        string sessionID;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="device">Интерфейс устройства</param>
        public DeviceHelper(IDevice device)
        {
            this.device = device;
            sessionID = string.Empty;
        }

        /// <summary>
        /// Проверяет, заблокировано ли устройство
        /// </summary>
        public bool Captured
        {
            get
            {
                return !string.IsNullOrEmpty(sessionID);
            }
        }

        /// <summary>
        /// Индентификатор клиентской сессии,
        /// заблокировавшей устройство
        /// </summary>
        public string SessionID
        {
            get
            {
                return sessionID;
            }

            set
            {
                if (Captured)
                    throw new DeviceManagerException(
                        string.Format("Устройство заблокировано, идентификатор сессии {0}",
                        sessionID));

                // блокировка устройства
                sessionID = value;
            }
        }

        /// <summary>
        /// Интерфейс устройства
        /// </summary>
        public IDevice Device
        {
            get
            {
                return device;
            }
        }

        /// <summary>
        /// Освобождение устройства
        /// </summary>
        public void Release()
        {
            sessionID = string.Empty;
        }
    }
}
