using System;
using System.Collections.Generic;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// ��������������� ����� ��� �������������� ���������� � ����� ����� �������
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
        /// �������������� ���������� � ����� ����� �������
        /// </summary>
        /// <param name="ex">����������</param>
        /// <param name="checkPointId">�������� ����������� �����</param>
        /// <returns>����� ����� �������</returns>
        public static string[] GetStrings(string checkPointId, Exception ex)
        {
            var exceptionLevel = 0;
            var message = new List<string>();
            message.Add(string.Format("����������� �����: {0}", checkPointId));

            Exception current = ex;
            do
            {
                // ����� � ��� ������� ����������
                message.Add(string.Format("��� ����������: {0}", current.GetType()));
                AddRange(message, string.Format("����� ����������: {0}", current.Message));
                AddRange(message, current.StackTrace);

                // ��������� ������� ����������
                exceptionLevel++;

                // ���������, ��� �� ����������� ����������
                current = current.InnerException;
                if (current != null)
                {
                    // �����������
                    message.Add(string.Empty);
                    message.Add(string.Format("���������� ���������� [{0}]:", exceptionLevel));
                    message.Add(string.Empty);
                }
            }
            while (current != null);

            return message.ToArray();
        }
    }
}
