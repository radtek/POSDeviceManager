using System;
using System.Collections.Generic;
using System.IO;

namespace ERPService.SharedLibs.Eventlog.Iterators
{
    internal class LogsIterator
    {
        #region Константы

        private const string _dateRangeError = 
            "Ошибка задания диапазона дат. Начало диапазона {0} больше конца диапазона {1}";

        #endregion

        #region Поля

        private Func<DateTime, string> _storageNamePredicate;

        #endregion

        #region Конструктор

        internal LogsIterator(Func<DateTime, string> storageNamePredicate)
        {
            if (storageNamePredicate == null)
                throw new ArgumentNullException("storageNamePredicate");

            _storageNamePredicate = storageNamePredicate;
        }

        #endregion

        #region Итераторы

        /// <summary>
        /// Перебор логов в заданном диапазоне дат
        /// </summary>
        /// <param name="fromDate">Дата начала диапазона</param>
        /// <param name="toDate">Дата окончания диапазона</param>
        /// <returns>Интерфейс для перебора логов</returns>
        internal IEnumerable<string> LogsRundown(DateTime fromDate, DateTime toDate)
        {
            // проверяем корректность задания диапазона дат
            if (fromDate > toDate)
                throw new ArgumentException(string.Format(_dateRangeError, fromDate, toDate));
            if (toDate > DateTime.Today)
                // приравниваем окончание интервала сегодняшней дата
                toDate = DateTime.Today;

            // перебираем даты по дням в обратном порядке
            DateTime nextDate = toDate;
            do
            {
                // для каждой даты ищем свой лог
                string storageName = _storageNamePredicate(nextDate);
                
                if (File.Exists(storageName))
                    yield return storageName;

                // уменьшаем текущую дату на день
                nextDate = nextDate.AddDays(-1);
            }
            while (nextDate >= fromDate);
        }

        /// <summary>
        /// Перебор логов в заданном диапазоне дат с открытием каждого лога на чтение
        /// </summary>
        /// <param name="fromDate">Дата начала диапазона</param>
        /// <param name="toDate">Дата окончания диапазона</param>
        /// <returns>Интерфейс для перебора логов</returns>
        internal IEnumerable<LogsIteratorHelper> StreamedLogsRundown(DateTime fromDate, 
            DateTime toDate)
        {
            foreach (var storageName in LogsRundown(fromDate, toDate))
            {
                // создаем вспомогательный объект для чтения из лога
                using (var helper = new LogsIteratorHelper(storageName))
                {
                    yield return helper;
                }
            }
        }

        #endregion
    }
}
