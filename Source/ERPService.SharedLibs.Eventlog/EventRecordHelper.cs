using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// ��������������� ����� ��� ������ � �������� ����
    /// </summary>
    internal static class EventRecordHelper
    {
        #region ���������

        // ����
        private const string Dash = "-";
        // ���������
        private const Char Tab = '\t';
        // ����� ����� � ������
        private const int FieldsCount = 6;

        #endregion

        #region �������� ����, �������� � ������

        /// <summary>
        /// ���������� �������� ���� � ������� �� ������ �� ����
        /// </summary>
        /// <param name="date">��������� ������������� ����</param>
        /// <param name="time">��������� ������������� �������</param>
        private static DateTime TimestampFromStorage(string date, string time)
        {
            int[] dateParts = StringToInt32Array(date);
            int[] timeParts = StringToInt32Array(time);

            return new DateTime(dateParts[2], dateParts[1], dateParts[0],
                timeParts[0], timeParts[1], timeParts[2]);
        }

        /// <summary>
        /// ����������� ������ � ������ Int32
        /// </summary>
        /// <param name="source">�������� ������</param>
        private static int[] StringToInt32Array(string source)
        {
            string[] values = source.Split('.');
            int[] result = new int[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                result[i] = Convert.ToInt32(values[i]);
            }
            return result;
        }

        #endregion

        /// <summary>
        /// ��������� ��������� ������ ���� �� ����
        /// </summary>
        /// <param name="storageEntry">��������� ������ ����</param>
        /// <returns>��������� ������ ����, �������� �� ����</returns>
        internal static string[] GetRawEntry(string storageEntry)
        {
            return storageEntry.Split(EventRecordHelper.Tab);
        }

        /// <summary>
        /// ��������� ���������� ������ �� ���������� �����
        /// </summary>
        /// <param name="rawEntry">��������� ������ ����, �������� �� ����</param>
        internal static bool IsValidEntry(string[] rawEntry)
        {
            return rawEntry.Length == EventRecordHelper.FieldsCount;
        }

        /// <summary>
        /// ������� ������� �� ������ ����
        /// </summary>
        /// <param name="storageEntry">������ ����</param>
        /// <returns>����� �������</returns>
        internal static EventRecord CreateFromStorageEntry(string[] storageEntry)
        {
            // ������� ������
            EventRecord record = new EventRecord(
                storageEntry[0],
                TimestampFromStorage(storageEntry[1], storageEntry[2]),
                storageEntry[3],
                EventTypeConvertor.ConvertTo(storageEntry[4]),
                new string[] { storageEntry[5] });

            // ���������� ��
            return record;
        }

        /// <summary>
        /// ������ ������ �������
        /// </summary>
        /// <param name="record">������ ��������� ����</param>
        /// <param name="textWriter">������ ��� ������ ������</param>
        internal static void Save(EventRecord record, TextWriter textWriter)
        {
            // ������ ������ ������ ������� ���������� � ��������� ������� ����
            // ������������� ����� ������������ ����� ���������������
            foreach (var line in record.Text)
            {
                var logLine = new StringBuilder();

                // �������������
                logLine.Append(record.Id.Replace(Dash, string.Empty));
                // ���������
                logLine.Append(Tab);
                // ����
                logLine.Append(record.Timestamp.ToString("dd.MM.yyyy"));
                // ���������
                logLine.Append(Tab);
                // �����
                logLine.Append(record.Timestamp.ToString("HH.mm.ss"));
                // ���������
                logLine.Append(Tab);
                // �������� �������
                // ����������� �� ������ ���� �� 15 ��������
                logLine.Append(record.Source.PadRight(15));
                // ���������
                logLine.Append(Tab);
                // ��� �������
                logLine.Append(EventTypeConvertor.ConvertFrom(record.EventType).PadRight(15));
                // ���������
                logLine.Append(Tab);
                // ������ ������ �������
                logLine.Append(line);

                // ������ ������ � ���
                textWriter.WriteLine(logLine.ToString());
            }
        }
    }
}
