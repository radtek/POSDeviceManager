using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ERPService.SharedLibs.Eventlog.Iterators
{
    internal class LogsIterator
    {
        #region Константы

        private const String _dateRangeError = 
            "Ошибка задания диапазона дат. Начало диапазона {0} больше конца диапазона {1}";

        #endregion

        #region Поля

        private Func<DateTime, String> _storageNamePredicate;

        #endregion

        #region Конструктор

        internal LogsIterator(Func<DateTime, String> storageNamePredicate)
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
        internal IEnumerable<String> LogsRundown(DateTime fromDate, DateTime toDate)
        {
            // проверяем корректность задания диапазона дат
            if (fromDate > toDate)
                throw new ArgumentException(String.Format(_dateRangeError, fromDate, toDate));
            if (toDate > DateTime.Today)
                // приравниваем окончание интервала сегодняшней дата
                toDate = DateTime.Today;

            // перебираем даты по дням в обратном порядке
            DateTime nextDate = toDate;
            do
            {
                // для каждой даты ищем свой лог
                String storageName = _storageNamePredicate(nextDate);
                
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
