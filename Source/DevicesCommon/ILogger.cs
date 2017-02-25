using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace DevicesCommon
{
    /// <summary>
    /// Интерфейс для протоколирования работы устройств
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Добавляет запись в протокол работы приложения
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        /// <param name="type">Тип сообщения</param>
        void WriteEntry(String message, EventLogEntryType type);

        /// <summary>
        /// Добавляет запись в протокол работы приложения
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        /// <param name="type">Тип сообщения</param>
        /// <param name="sender">Устройство, которое пишет в лог</param>
        void WriteEntry(IDevice sender, String message, EventLogEntryType type);

        /// <summary>
        /// Режим отладки
        /// </summary>
        Boolean DebugInfo { get; }

        /// <summary>
        /// Сохранение отладочной информации
        /// </summary>
        /// <param name="sender">Устройство, которому нужно сохранить отладочную информацию</param>
        /// <param name="info">Отладочная информация</param>
        void SaveDebugInfo(IDevice sender, String info);
    }
}
