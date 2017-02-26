using System;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace DevicesBase
{
    /// <summary>
    /// ��� ���������������� ������
    /// </summary>
    public interface ISerialPortsPool
    {
        /// <summary>
        /// �������� ������ � ����� �� ����
        /// </summary>
        /// <param name="deviceId">������������� ����������</param>
        /// <param name="portName">��� �����</param>
        /// <returns>���� �� ����</returns>
        EasyCommunicationPort GetPort(string deviceId, string portName);

        /// <summary>
        /// ��������� ���������������� ����
        /// </summary>
        /// <param name="deviceId">������������� ����������</param>
        /// <param name="portName">��� �����</param>
        /// <param name="waitIfCaptured">������� ������������ �����</param>
        /// <param name="waitTime">�����, � ������� �������� ������� ������������</param>
        /// <returns>���� �� ����</returns>
        EasyCommunicationPort CapturePort(string deviceId, string portName, bool waitIfCaptured,
            TimeSpan waitTime);

        /// <summary>
        /// ��������� ���������������� ����. ������� ������� ����� � ������� 
        /// ������������ ��������� �������
        /// </summary>
        /// <param name="deviceId">������������� ����������</param>
        /// <param name="portName">��� �����</param>
        /// <returns>���� �� ����</returns>
        EasyCommunicationPort CapturePort(string deviceId, string portName);

        /// <summary>
        /// ���������� ���������������� ����
        /// </summary>
        /// <param name="deviceId">������������� ����������</param>
        /// <param name="portName">��� �����</param>
        void ReleasePort(string deviceId, string portName);
    }
}
