namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// ��������� �������� ����� ������� � ������ � �������
    /// </summary>
    public static class EventTypeConvertor
    {
        /// <summary>
        /// ����������� �������� ���� ������� � ������
        /// </summary>
        /// <param name="value">�������� ���� �������</param>
        /// <returns>��������� ������������� ���� �������</returns>
        public static string ConvertFrom(EventType value)
        {
            switch (value)
            {
                case EventType.Error:
                    return "������";
                case EventType.Information:
                    return "����������";
                case EventType.Warning:
                    return "��������������";
                default:
                    return "�� ���������";
            }
        }

        /// <summary>
        /// ����������� ������ � �������� ���� �������
        /// </summary>
        /// <param name="value">�������� ������</param>
        /// <returns>�������� ���� �������</returns>
        public static EventType ConvertTo(string value)
        {
            switch (value.TrimEnd())
            {
                case "������":
                    return EventType.Error;
                case "����������":
                    return EventType.Information;
                case "��������������":
                    return EventType.Warning;
                default:
                    return EventType.Undefined;
            }
        }
    }
}
