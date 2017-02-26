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
        void WriteEntry(string message, EventLogEntryType type);

        /// <summary>
        /// ��������� ������ � �������� ������ ����������
        /// </summary>
        /// <param name="message">����� ���������</param>
        /// <param name="type">��� ���������</param>
        /// <param name="sender">����������, ������� ����� � ���</param>
        void WriteEntry(IDevice sender, string message, EventLogEntryType type);

        /// <summary>
        /// ����� �������
        /// </summary>
        bool DebugInfo { get; }

        /// <summary>
        /// ���������� ���������� ����������
        /// </summary>
        /// <param name="sender">����������, �������� ����� ��������� ���������� ����������</param>
        /// <param name="info">���������� ����������</param>
        void SaveDebugInfo(IDevice sender, string info);
    }
}
