namespace DevicesCommon
{
    /// <summary>
    /// �������� ���������� ���������
    /// </summary>
    public interface IDeviceManager
	{
		/// <summary>
		/// ���������� ������ �� ������� ��������� ���� ���������
		/// </summary>
		/// <param name="sessionId">������������� ������</param>
		/// <param name="deviceId">������������� ����������</param>
		IDevice GetDevice(string sessionId, string deviceId);

		/// <summary>
		/// �������� ����������� ������ � ����������
		/// </summary>
		/// <param name="sessionId">������������� ������</param>
		/// <param name="deviceId">������������� ����������</param>
        /// <param name="waitTimeout">������� �������� ������� ����������, �������</param>
		bool Capture(string sessionId, string deviceId, int waitTimeout);

		/// <summary>
		/// ���������� ����������
		/// </summary>
		/// <param name="sessionId">������������� ������</param>
		/// <param name="deviceId">������������� ����������</param>
		bool Release(string sessionId, string deviceId);

		/// <summary>
		/// ����������� ���������� ������ � ���������� ���������
		/// </summary>
		/// <param name="sessionId">������������� ������</param>
		void Login(out string sessionId);

		/// <summary>
		/// ���������� ���������� ������ � ���������� ���������
		/// </summary>
		/// <param name="sessionId">������������� ������</param>
		void Logout(string sessionId);
	}
}
