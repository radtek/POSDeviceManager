using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// Конвертор значений типов события в строку и обратно
    /// </summary>
    public static class EventTypeConvertor
    {
        /// <summary>
        /// Преобразует значение типа события в строку
        /// </summary>
        /// <param name="value">Значение типа события</param>
        /// <returns>Строковое представление типа события</returns>
        public static String ConvertFrom(EventType value)
        {
            switch (value)
            {
                case EventType.Error:
                    return "Ошибка";
                case EventType.Information:
                    return "Информация";
                case EventType.Warning:
                    return "Предупреждение";
                default:
                    return "Не определен";
            }
        }

        /// <summary>
        /// Преобразует строку в значение типа события
        /// </summary>
        /// <param name="value">Исходная строка</param>
        /// <returns>Значение типа события</returns>
        public static EventType ConvertTo(String value)
        {
            switch (value.TrimEnd())
            {
                case "Ошибка":
                    return EventType.Error;
                case "Информация":
                    return EventType.Information;
                case "Предупреждение":
                    return EventType.Warning;
                default:
                    return EventType.Undefined;
            }
        }
    }
}
