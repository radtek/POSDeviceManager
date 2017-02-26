namespace DevicesCommon
{
    /// <summary>
    /// ����������, ������������ �� ����������������� �����
    /// </summary>
    public interface ISerialDevice : IConnectableDevice
    {
        /// <summary>
        /// �������� �������� ������ ����� ���� (���)
        /// </summary>
        int Baud { get; set; }
    }
}
