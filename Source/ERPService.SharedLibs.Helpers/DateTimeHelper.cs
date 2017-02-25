using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.Helpers
{
    /// <summary>
    /// ��������������� ����� ��� ������ � ����� � ��������
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// ��������� ����/������� � ������ ���
        /// </summary>
        /// <param name="value">����� �������� ����/�������</param>
        public static DateTime StartOfDay(DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day);
        }

        /// <summary>
        /// ��������� ����/������� � ����� ���
        /// </summary>
        /// <param name="value">����� �������� ����/�������</param>
        public static DateTime EndOfDay(DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 23, 59, 59, 999);
        }
    }
}
