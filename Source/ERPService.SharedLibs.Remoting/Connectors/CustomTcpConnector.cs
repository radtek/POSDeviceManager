using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;
using ERPService.SharedLibs.Remoting.Channels;

namespace ERPService.SharedLibs.Remoting.Connectors
{
    /// <summary>
    /// ������� ����� ��� ����������� � ���������-�������� �� TCP
    /// </summary>
    /// <typeparam name="T">��������� �������</typeparam>
    public abstract class CustomTcpConnector<T> : CustomConnector<T>
    {
        /// <summary>
        /// ������� ��������� ����� � ��������
        /// </summary>
        protected override string Protocol
        {
            get { return "tcp"; }
        }

        /// <summary>
        /// ������� ���������� �����
        /// </summary>
        /// <param name="sinkProvider">��������� ���������� ������</param>
        /// <param name="channelName">��� ������</param>
        /// <returns>���������� �����</returns>
        protected override IChannel CreateChannel(IClientChannelSinkProvider sinkProvider, string channelName)
        {
            return new IpFixTcpClientChannel(GetBasicChannelProperties(channelName), sinkProvider);
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        protected CustomTcpConnector()
            : base()
        {
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="serverNameOrIp">��� ��� IP-����� �������</param>
        /// <param name="port">���� �������</param>
        /// <param name="objectName">��� �������</param>
        protected CustomTcpConnector(string serverNameOrIp, int port, string objectName)
            : base(serverNameOrIp, port, objectName)
        {
        }
    }
}
