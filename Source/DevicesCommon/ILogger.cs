using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace DevicesCommon
{
    /// <summary>
    /// ��������� ��� ���������������� ������ ���������
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// ��������� ������ � �������� ������ ����������
        /// </summary>
        /// <param name="message">����� ���������</param>
        /// <param name="type">��� ���������</param>
        void WriteEntry(String message, EventLogEntryType type);

        /// <summary>
        /// ��������� ������ � �������� ������ ����������
        /// </summary>
        /// <param name="message">����� ���������</param>
        /// <param name="type">��� ���������</param>
        /// <param name="sender">����������, ������� ����� � ���</param>
        void WriteEntry(IDevice sender, String message, EventLogEntryType type);

        /// <summary>
        /// ����� �������
        /// </summary>
        Boolean DebugInfo { get; }

        /// <summary>
        /// ���������� ���������� ����������
        /// </summary>
        /// <param name="sender">����������, �������� ����� ��������� ���������� ����������</param>
        /// <param name="info">���������� ����������</param>
        void SaveDebugInfo(IDevice sender, String info);
    }
}
