using System;
using System.Collections.Generic;
using System.Text;

namespace DevicesCommon
{
    /// <summary>
    /// ��������� ��� ������ � SMS
    /// </summary>
    public interface ISMSClient : IDevice, IDisposable
    {
        /// <summary>
        /// �������� ��������� ���������� ���������
        /// </summary>
        /// <param name="recipientNumber">����� �������� ����������</param>
        /// <param name="messageText">����� ���������</param>
        /// <remarks>����� �������� �������� � ������������� �������: +123(456)789-01-23
        /// ����� ��������� ����� ���� �������������.</remarks>
        void Send(String recipientNumber, String messageText);

        /// <summary>
        /// ������������� ���������� ��� �����������, �������������� ��� �������� SMS
        /// </summary>
        /// <param name="paramName">��� ���������</param>
        /// <param name="paramValue">�������� ���������</param>
        void SetConnectivityParam(String paramName, String paramValue);
    }
}
