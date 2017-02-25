using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon;
using DevicesCommon.Helpers;

namespace DevicesBase
{
	/// <summary>
	/// Базовый класс для всех подключаемых устройств
	/// </summary>
	public abstract class CustomConnectableDevice : CustomDevice, IConnectableDevice
	{
		#region Конструктор

		/// <summary>
		/// Создает подключаемое устройство
		/// </summary>
		protected CustomConnectableDevice(): base()
		{
		}

		#endregion

		#region Реализация IConnectableDevice

		/// <summary>
		/// Имя порта (напр., COM1, LPT1...)
		/// </summary>
		public abstract string PortName { get; set; }

		#endregion
	}
}
