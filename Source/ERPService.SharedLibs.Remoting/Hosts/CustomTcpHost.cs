using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

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
        protected override IChannel CreateChannel(int port, IServerChannelSinkProvider sinkProvider,
            string channelName)
        {
            IDictionary channelProps = new Hashtable();
            channelProps["name"] = channelName;
            channelProps["port"] = port;

            return new TcpServerChannel(channelProps, sinkProvider);
        }
    }
}
