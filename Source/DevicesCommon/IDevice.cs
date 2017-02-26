using DevicesCommon.Helpers;

namespace DevicesCommon
{
    /// <summary>
    /// ������� ��������� ��� ���������
    /// </summary>
    public interface IDevice
	{
		#region ����� ��� ���� ��������� ��������

		/// <summary>
		/// ���������� ��� ������
		/// </summary>
        ErrorCode ErrorCode { get; }

		/// <summary>
		/// ����������� � ���������� ����������
		/// </summary>
		bool Active { get; set; }

        /// <summary>
        /// ������������� ����������
        /// </summary>
        string DeviceId { get; set; }

        /// <summary>
        /// ��������� ��� ���������������� ������ ����������
        /// </summary>
        ILogger Logger { get; set; }

		#endregion
	}
}
