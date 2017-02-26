using System;
using DevicesCommon;

namespace TsManager
{
    /// <summary>
    /// Интерфейс, реализующий логику работы СКУД
    /// </summary>
    public interface IAMCSLogic
    {
        /// <summary>
        /// Параметры, необходимые для работы
        /// </summary>
        Object Settings { get; set; }

        /// <summary>
        /// Проверяет возможность доступа в заданном направлении
        /// по идентификационным данным
        /// </summary>
        /// <param name="direction">Направление</param>
        /// <param name="idData">Идентификационные данные</param>
        /// <param name="reason">Причина отказа в доступе</param>
        /// <returns>true, если доступ разрешен</returns>
        bool IsAccessGranted(TurnstileDirection direction, string idData, 
            out string reason);

        /// <summary>
        /// Фиксирует факт доступа в заданном направлении
        /// по идентификационным данным
        /// </summary>
        /// <param name="direction">Направление</param>
        /// <param name="idData">Идентификационные данные</param>
        void OnAccessOccured(TurnstileDirection direction, string idData);
    }
}
