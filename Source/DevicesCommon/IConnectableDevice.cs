namespace DevicesCommon
{
    /// <summary>
    /// ������������ ����������
    /// </summary>
    public interface IConnectableDevice : IDevice
	{
        /// <summary>
        /// ��� ����� (����., COM1, LPT1...)
        /// </summary>
        string PortName { get; set; }
	}
}
