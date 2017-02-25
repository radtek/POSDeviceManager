using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.Helpers
{
    /// <summary>
    /// Вспомогательный класс для работы с датой и временем
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// Установка даты/времени в начало дня
        /// </summary>
        /// <param name="value">Любое значение даты/времени</param>
        public static DateTime StartOfDay(DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day);
        }

        /// <summary>
        /// Установка даты/времени в конец дня
        /// </summary>
        /// <param name="value">Любое значение даты/времени</param>
        public static DateTime EndOfDay(DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 23, 59, 59, 999);
        }
    }
}
