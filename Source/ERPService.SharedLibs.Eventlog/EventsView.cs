using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// ��������������� ����� ��� ���������� ��������� �������
    /// </summary>
    public static class EventsView
    {
        /// <summary>
        /// �������� �������
        /// </summary>
        /// <param name="viewLink">���������� ����� ����� ������� ������� � ��������</param>
        public static void Show(IEventsViewLink viewLink)
        {
            FormEventDetails formEventDetails = new FormEventDetails();
            formEventDetails.Execute(viewLink);
        }
    }
}
