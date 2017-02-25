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
        private const String Dash = "-";
        // ���������
        private const Char Tab = '\t';
        // ����� ����� � ������
        private const Int32 FieldsCount = 6;

        #endregion

        #region �������� ����, �������� � ������

        /// <summary>
        /// ���������� �������� ���� � ������� �� ������ �� ����
        /// </summary>
        /// <param name="date">��������� ������������� ����</param>
        /// <param name="time">��������� ������������� �������</param>
        private static DateTime TimestampFromStorage(String date, String time)
        {
            Int32[] dateParts = StringToInt32Array(date);
            Int32[] timeParts = StringToInt32Array(time);

            return new DateTime(dateParts[2], dateParts[1], dateParts[0],
                timeParts[0], timeParts[1], timeParts[2]);
        }

        /// <summary>
        /// ����������� ������ � ������ Int32
        /// </summary>
        /// <param name="source">�������� ������</param>
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
        /// ��������� ��������� ������ ���� �� ����
        /// </summary>
        /// <param name="storageEntry">��������� ������ ����</param>
        /// <returns>��������� ������ ����, �������� �� ����</returns>
        internal static String[] GetRawEntry(String storageEntry)
        {
            return storageEntry.Split(EventRecordHelper.Tab);
        }

        /// <summary>
        /// ��������� ���������� ������ �� ���������� �����
        /// </summary>
        /// <param name="rawEntry">��������� ������ ����, �������� �� ����</param>
        internal static Boolean IsValidEntry(String[] rawEntry)
        {
            return rawEntry.Length == EventRecordHelper.FieldsCount;
        }

        /// <summary>
        /// ������� ������� �� ������ ����
        /// </summary>
        /// <param name="storageEntry">������ ����</param>
        /// <returns>����� �������</returns>
        internal static EventRecord CreateFromStorageEntry(String[] storageEntry)
        {
            // ������� ������
            EventRecord record = new EventRecord(
                storageEntry[0],
                TimestampFromStorage(storageEntry[1], storageEntry[2]),
                storageEntry[3],
                EventTypeConvertor.ConvertTo(storageEntry[4]),
                new String[] { storageEntry[5] });

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
                logLine.Append(record.Id.Replace(Dash, String.Empty));
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
