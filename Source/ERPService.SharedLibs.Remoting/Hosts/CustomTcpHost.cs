using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;

namespace ERPService.SharedLibs.Remoting.Hosts
{
    /// <summary>
    /// ������� ����� ��� �������� �������� � ������� TCP-�������
    /// </summary>
    /// <typeparam name="T">��� �������, � �������� ����� ���������� ������</typeparam>
    public abstract class CustomTcpHost<T> : CustomHost<T> where T : HostingTarget
    {
        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="target">������, � �������� ����� ���������� ������</param>
        protected CustomTcpHost(T target)
            : base(target)
        {
        }

        /// <summary>
        /// ������� ��������� �����
        /// </summary>
        /// <param name="sinkProvider">��������� ������</param>
        /// <param name="channelName">��� ������</param>
        /// <param name="port">����</param>
        /// <returns>��������� �����</returns>
        protected override IChannel CreateChannel(Int32 port, IServerChannelSinkProvider sinkProvider, 
            String channelName)
        {
            IDictionary channelProps = new Hashtable();
            channelProps["name"] = channelName;
            channelProps["port"] = port;

            return new TcpServerChannel(channelProps, sinkProvider);
        }
    }
}
