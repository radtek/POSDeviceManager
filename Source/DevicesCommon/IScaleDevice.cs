namespace DevicesCommon
{
    /// <summary>
    /// ���� � ������� ��������
    /// </summary>
    public interface IScaleDevice : IDevice
    {
        /// <summary>
        /// �������� ������ � ����
        /// </summary>
        /// <param name="xmlData"></param>
        void Upload(string xmlData);

        /// <summary>
        /// ������� ��������� ����
        /// </summary>
        int Weight { get; }

        /// <summary>
        /// ������ � ����������� ����������� � �����
        /// <example>
        /// tcp://host:port
        /// udp://host:port
        /// rs://port_name:baud
        /// </example>
        /// </summary>
        string ConnectionString { get; set; }
    }
}
