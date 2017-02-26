namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// ��������� ��� ����� ���� ���������� ��������� �������
    /// � ������ �������
    /// </summary>
    public interface IEventsViewLink
    {
        /// <summary>
        /// ����������� � ���������� �������
        /// </summary>
        EventRecord NextEvent();

        /// <summary>
        /// ����������� � ����������� �������
        /// </summary>
        EventRecord PreviousEvent();

        /// <summary>
        /// ������� �������
        /// </summary>
        EventRecord Current { get; }
    }
}
