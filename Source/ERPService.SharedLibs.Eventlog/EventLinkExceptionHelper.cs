using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// ��������������� ����� ��� �������������� ���������� � ����� ����� �������
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
        /// �������������� ���������� � ����� ����� �������
        /// </summary>
        /// <param name="ex">����������</param>
        /// <param name="checkPointId">�������� ����������� �����</param>
        /// <returns>����� ����� �������</returns>
        public static String[] GetStrings(String checkPointId, Exception ex)
        {
            var exceptionLevel = 0;
            var message = new List<String>();
            message.Add(String.Format("����������� �����: {0}", checkPointId));

            Exception current = ex;
            do
            {
                // ����� � ��� ������� ����������
                message.Add(String.Format("��� ����������: {0}", current.GetType()));
                AddRange(message, String.Format("����� ����������: {0}", current.Message));
                AddRange(message, current.StackTrace);

                // ��������� ������� ����������
                exceptionLevel++;

                // ���������, ��� �� ����������� ����������
                current = current.InnerException;
                if (current != null)
                {
                    // �����������
                    message.Add(String.Empty);
                    message.Add(String.Format("���������� ���������� [{0}]:", exceptionLevel));
                    message.Add(String.Empty);
                }
            }
            while (current != null);

            return message.ToArray();
        }
    }
}
