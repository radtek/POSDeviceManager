using System;
using System.Collections.Generic;
using System.Text;

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
        Int32 Baud { get; set; }
    }
}
