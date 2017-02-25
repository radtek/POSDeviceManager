using System;
using System.Collections.Generic;
using System.Text;

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
		IDevice GetDevice(String sessionId, String deviceId);

		/// <summary>
		/// �������� ����������� ������ � ����������
		/// </summary>
		/// <param name="sessionId">������������� ������</param>
		/// <param name="deviceId">������������� ����������</param>
        /// <param name="waitTimeout">������� �������� ������� ����������, �������</param>
		bool Capture(String sessionId, String deviceId, Int32 waitTimeout);

		/// <summary>
		/// ���������� ����������
		/// </summary>
		/// <param name="sessionId">������������� ������</param>
		/// <param name="deviceId">������������� ����������</param>
		bool Release(String sessionId, String deviceId);

		/// <summary>
		/// ����������� ���������� ������ � ���������� ���������
		/// </summary>
		/// <param name="sessionId">������������� ������</param>
		void Login(out String sessionId);

		/// <summary>
		/// ���������� ���������� ������ � ���������� ���������
		/// </summary>
		/// <param name="sessionId">������������� ������</param>
		void Logout(String sessionId);
	}
}
