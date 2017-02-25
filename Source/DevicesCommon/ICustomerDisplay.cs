using System;
using System.Collections.Generic;
using System.Text;

namespace DevicesCommon
{
	/// <summary>
	/// ��������� ��� �������� ����������
	/// </summary>
	public interface ICustomerDisplay : ISerialDevice
	{
#region ��������

		/// <summary>
		/// ����� ����� �� �������
		/// </summary>
		/// <param name="lineNumber">����� ������</param>
		String this[Int32 lineNumber] { set; }

#endregion

#region ������

		/// <summary>
		/// ������ ������� ���������� �� ����-������ �������
		/// </summary>
		void SaveToEEPROM();

#endregion
	}
}
