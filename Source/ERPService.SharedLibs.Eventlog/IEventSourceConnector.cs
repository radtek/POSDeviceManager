using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// �������� ��� ����������� � ��������� �������
    /// </summary>
    public interface IEventSourceConnector
    {
        /// <summary>
        /// ��������� ����������� � ��������� �������
        /// </summary>
        void OpenConnector();

        /// <summary>
        /// ��������� ����������� � ��������� �������
        /// </summary>
        void CloseConnector();

        /// <summary>
        /// �������� �������
        /// </summary>
        IEventLinkBasics Source { get; }

        /// <summary>
        /// ����������� � �������� �������� �������
        /// </summary>
        /// <param name="eventsLoaded">����������� ����� �������</param>
        void ReloadProgress(int eventsLoaded);
    }
}
