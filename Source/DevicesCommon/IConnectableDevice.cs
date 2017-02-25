using System;
using System.Collections.Generic;
using System.Text;

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
		String PortName { get; set; }
	}
}
