using System;
using System.Collections.Generic;
using System.Text;

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
        Int32 Address { get; set; }
    }
}
