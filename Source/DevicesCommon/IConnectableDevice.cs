using System;
using System.Collections.Generic;
using System.Text;

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
		String PortName { get; set; }
	}
}
