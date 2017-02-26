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
        byte StopChar { get; set; }

        /// <summary>
        /// ��������� ���� ������
        /// </summary>
        string Data { get; }

        /// <summary>
        /// ��������� ������� ������
        /// </summary>
        bool Empty { get; set; }

        #endregion
    }
}
