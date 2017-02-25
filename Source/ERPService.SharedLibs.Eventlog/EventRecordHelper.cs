using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// ¬спомогательный класс дл€ работы с запис€ми лога
    /// </summary>
    internal static class EventRecordHelper
    {
        #region  онстанты

        // тире
        private const String Dash = "-";
        // табул€ци€
        private const Char Tab = '\t';
        // число полей в записи
        private const Int32 FieldsCount = 6;

        #endregion

        #region «акрытые пол€, свойства и методы

        /// <summary>
        /// ¬озвращает значение даты и времени по данным из лога
        /// </summary>
        /// <param name="date">—троковое представление даты</param>
        /// <param name="time">—троковое представление времени</param>
        private static DateTime TimestampFromStorage(String date, String time)
        {
            Int32[] dateParts = StringToInt32Array(date);
            Int32[] timeParts = StringToInt32Array(time);

            return new DateTime(dateParts[2], dateParts[1], dateParts[0],
                timeParts[0], timeParts[1], timeParts[2]);
        }

        /// <summary>
        /// ѕреобразует строку в массив Int32
        /// </summary>
        /// <param name="source">»сходна€ строка</param>
        private static Int32[] StringToInt32Array(String source)
        {
            String[] values = source.Split('.');
            Int32[] result = new Int32[values.Length];
            for (Int32 i = 0; i < values.Length; i++)
            {
                result[i] = Convert.ToInt32(values[i]);
            }
            return result;
        }

        #endregion

        /// <summary>
        /// –азбивает считанную строку лога на пол€
        /// </summary>
        /// <param name="storageEntry">—читанна€ строка лога</param>
        /// <returns>—читанна€ строка лога, разбита€ на пол€</returns>
        internal static String[] GetRawEntry(String storageEntry)
        {
            return storageEntry.Split(EventRecordHelper.Tab);
        }

        /// <summary>
        /// ѕровер€ет валидность записи по количеству полей
        /// </summary>
        /// <param name="rawEntry">—читанна€ строка лога, разбита€ на пол€</param>
        internal static Boolean IsValidEntry(String[] rawEntry)
        {
            return rawEntry.Length == EventRecordHelper.FieldsCount;
        }

        /// <summary>
        /// —оздает событие из записи лога
        /// </summary>
        /// <param name="storageEntry">«апись лога</param>
        /// <returns>Ќовое событие</returns>
        internal static EventRecord CreateFromStorageEntry(String[] storageEntry)
        {
            // создаем запись
            EventRecord record = new EventRecord(
                storageEntry[0],
                TimestampFromStorage(storageEntry[1], storageEntry[2]),
                storageEntry[3],
                EventTypeConvertor.ConvertTo(storageEntry[4]),
                new String[] { storageEntry[5] });

            // возвращаем ее
            return record;
        }

        /// <summary>
        /// «апись данных событи€
        /// </summary>
        /// <param name="record">«апись кассового лога</param>
        /// <param name="textWriter">ќбъект дл€ записи данных</param>
        internal static void Save(EventRecord record, TextWriter textWriter)
        {
            // каждую строку текста событи€ записываем в отдельную позицию лога
            // многострочный текст определ€етс€ одним идентификатором
            foreach (var line in record.Text)
            {
                var logLine = new StringBuilder();

                // идентификатор
                logLine.Append(record.Id.Replace(Dash, String.Empty));
                // табул€ци€
                logLine.Append(Tab);
                // дата
                logLine.Append(record.Timestamp.ToString("dd.MM.yyyy"));
                // табул€ци€
                logLine.Append(Tab);
                // врем€
                logLine.Append(record.Timestamp.ToString("HH.mm.ss"));
                // табул€ци€
                logLine.Append(Tab);
                // источник событи€
                // выравниваем по левому краю до 15 символов
                logLine.Append(record.Source.PadRight(15));
                // табул€ци€
                logLine.Append(Tab);
                // тип событи€
                logLine.Append(EventTypeConvertor.ConvertFrom(record.EventType).PadRight(15));
                // табул€ци€
                logLine.Append(Tab);
                // строка текста событи€
                logLine.Append(line);

                // запись строки в лог
                textWriter.WriteLine(logLine.ToString());
            }
        }
    }
}
