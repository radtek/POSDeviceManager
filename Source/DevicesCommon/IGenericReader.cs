using System;
using System.Collections.Generic;
using System.Text;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace DevicesCommon
{
	/// <summary>
    /// ��������� ����������, ���������������� ��� ������ ������ ���� �������,
    /// ���������� � �.�.
	/// </summary>
	public interface IGenericReader : ISerialDevice
	{
        #region ��������

		/// <summary>
		/// �������� ��������
		/// </summary>
		Parity Parity { get; set; }

		/// <summary>
		/// ����-������
		/// </summary>
		Byte StopChar { get; set; }

        /// <summary>
        /// ��������� ���� ������
        /// </summary>
        String Data { get; }

        /// <summary>
        /// ��������� ������� ������
        /// </summary>
        Boolean Empty { get; set; }

        #endregion
    }
}
