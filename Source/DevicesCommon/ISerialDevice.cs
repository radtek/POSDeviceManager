namespace DevicesCommon
{
    /// <summary>
    /// Устройство, подключаемое по последовательному порту
    /// </summary>
    public interface ISerialDevice : IConnectableDevice
    {
        /// <summary>
        /// Скорость передачи данных через порт (бод)
        /// </summary>
        int Baud { get; set; }
    }
}
