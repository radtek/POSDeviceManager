using System;
using System.Collections.Generic;
using System.Text;
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
        EasyCommunicationPort GetPort(String deviceId, String portName);

        /// <summary>
        /// ��������� ���������������� ����
        /// </summary>
        /// <param name="deviceId">������������� ����������</param>
        /// <param name="portName">��� �����</param>
        /// <param name="waitIfCaptured">������� ������������ �����</param>
        /// <param name="waitTime">�����, � ������� �������� ������� ������������</param>
        /// <returns>���� �� ����</returns>
        EasyCommunicationPort CapturePort(String deviceId, String portName, Boolean waitIfCaptured,
            TimeSpan waitTime);

        /// <summary>
        /// ��������� ���������������� ����. ������� ������� ����� � ������� 
        /// ������������ ��������� �������
        /// </summary>
        /// <param name="deviceId">������������� ����������</param>
        /// <param name="portName">��� �����</param>
        /// <returns>���� �� ����</returns>
        EasyCommunicationPort CapturePort(String deviceId, String portName);

        /// <summary>
        /// ���������� ���������������� ����
        /// </summary>
        /// <param name="deviceId">������������� ����������</param>
        /// <param name="portName">��� �����</param>
        void ReleasePort(String deviceId, String portName);
    }
}
