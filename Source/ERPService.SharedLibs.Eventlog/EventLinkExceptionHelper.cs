using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// Вспомогательный класс для преобразования исключений в набор строк события
    /// </summary>
    public static class EventLinkExceptionHelper
    {
        private static void AddRange(List<String> destination, String value)
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
        public static String[] GetStrings(String checkPointId, Exception ex)
        {
            var exceptionLevel = 0;
            var message = new List<String>();
            message.Add(String.Format("Контрольная точка: {0}", checkPointId));

            Exception current = ex;
            do
            {
                // пишем в лог текущее исключение
                message.Add(String.Format("Тип исключения: {0}", current.GetType()));
                AddRange(message, String.Format("Текст исключения: {0}", current.Message));
                AddRange(message, current.StackTrace);

                // поднимаем уровень исключения
                exceptionLevel++;

                // проверяем, нет ли внутреннего исключения
                current = current.InnerException;
                if (current != null)
                {
                    // разделитель
                    message.Add(String.Empty);
                    message.Add(String.Format("Внутреннее исключение [{0}]:", exceptionLevel));
                    message.Add(String.Empty);
                }
            }
            while (current != null);

            return message.ToArray();
        }
    }
}
