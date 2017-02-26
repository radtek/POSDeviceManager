namespace DevicesCommon
{
    /// <summary>
    /// ��������� ���������, ���������� �� RS-485
    /// </summary>
    public interface IRS485Device : ISerialDevice
    {
        /// <summary>
        /// ����� ����������
        /// </summary>
        int Address { get; set; }
    }
}
