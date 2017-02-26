using System;
using System.Collections.Generic;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// Вспомогательный класс для преобразования исключений в набор строк события
    /// </summary>
    public static class EventLinkExceptionHelper
    {
        private static void AddRange(List<string> destination, string value)
        {
            destination.AddRange(value.Split(
                new Char[] { (Char)10, (Char)13, (Char)9 },
                StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Преобразование исключения в набор строк события
        /// </summary>
        /// <param name="ex">Исключение</param>
        /// <param name="checkPointId">Название контрольной точки</param>
        /// <returns>Набор строк события</returns>
        public static string[] GetStrings(string checkPointId, Exception ex)
        {
            var exceptionLevel = 0;
            var message = new List<string>();
            message.Add(string.Format("Контрольная точка: {0}", checkPointId));

            Exception current = ex;
            do
            {
                // пишем в лог текущее исключение
                message.Add(string.Format("Тип исключения: {0}", current.GetType()));
                AddRange(message, string.Format("Текст исключения: {0}", current.Message));
                AddRange(message, current.StackTrace);

                // поднимаем уровень исключения
                exceptionLevel++;

                // проверяем, нет ли внутреннего исключения
                current = current.InnerException;
                if (current != null)
                {
                    // разделитель
                    message.Add(string.Empty);
                    message.Add(string.Format("Внутреннее исключение [{0}]:", exceptionLevel));
                    message.Add(string.Empty);
                }
            }
            while (current != null);

            return message.ToArray();
        }
    }
}
