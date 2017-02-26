namespace DevicesCommon
{
    /// <summary>
    /// Интерфейс устройств, работающих по RS-485
    /// </summary>
    public interface IRS485Device : ISerialDevice
    {
        /// <summary>
        /// Адрес устройства
        /// </summary>
        int Address { get; set; }
    }
}
