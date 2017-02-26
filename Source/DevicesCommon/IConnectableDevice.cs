namespace DevicesCommon
{
    /// <summary>
    /// Подключаемое устройство
    /// </summary>
    public interface IConnectableDevice : IDevice
	{
        /// <summary>
        /// Имя порта (напр., COM1, LPT1...)
        /// </summary>
        string PortName { get; set; }
	}
}
