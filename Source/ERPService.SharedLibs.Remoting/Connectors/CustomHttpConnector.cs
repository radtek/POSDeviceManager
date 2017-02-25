using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Collections;
using ERPService.SharedLibs.Remoting.Channels;

namespace ERPService.SharedLibs.Remoting.Connectors
{
    /// <summary>
    /// ������� ����� ��� ����������� � ���������-�������� �� HTTP
    /// </summary>
    /// <typeparam name="T">��������� �������</typeparam>
    public abstract class CustomHttpConnector<T> : CustomConnector<T>
    {
        private String _proxyName;
        private Int32 _proxyPort;
        private Int32 _clientConnectionLimit;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="serverNameOrIp">��� ��� IP-����� �������</param>
        /// <param name="port">���� �������</param>
        /// <param name="objectName">��� �������</param>
        /// <param name="clientConnectionLimit">������� ����������� ����� ���� ������������ 
        /// ������� � ���������� �������</param>
        /// <param name="proxyName">��� ��� IP-����� ������-�������</param>
        /// <param name="proxyPort">���� ������-�������</param>
        protected CustomHttpConnector(String serverNameOrIp, Int32 port, String objectName,
            Int32 clientConnectionLimit, String proxyName, Int32 proxyPort)
            : base(serverNameOrIp, port, objectName)
        {
            _clientConnectionLimit = clientConnectionLimit;
            _proxyName = proxyName;
            _proxyPort = proxyPort;
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        protected CustomHttpConnector()
            : base()
        {
            _clientConnectionLimit = 2;
            _proxyName = String.Empty;
            _proxyPort = 8080;
        }

        /// <summary>
        /// ��� ��� IP-����� ������-�������
        /// </summary>
        public String ProxyName
        {
            get { return _proxyName; }
            set
            {
                ThrowIfEmpty(value, "��� ��� IP-����� ������-�������");
                _proxyName = value;
            }
        }

        /// <summary>
        /// ���� ������-�������
        /// </summary>
        public Int32 ProxyPort
        {
            get { return _proxyPort; }
            set
            {
                ThrowIfOutOfRange(value);
                _proxyPort = value;
            }
        }

        /// <summary>
        /// ������� ����������� ����� ���� ������������ 
        /// ������� � ���������� �������
        /// </summary>
        public Int32 ClientConnectionLimit
        {
            get { return _clientConnectionLimit; }
            set { _clientConnectionLimit = value; }
        }

        /// <summary>
        /// ������� ���������� �����
        /// </summary>
        /// <param name="sinkProvider">��������� ���������� ������</param>
        /// <param name="channelName">��� ������</param>
        /// <returns>���������� �����</returns>
        protected override IChannel CreateChannel(IClientChannelSinkProvider sinkProvider, String channelName)
        {
            IDictionary channelProps = GetBasicChannelProperties(channelName);
            channelProps["clientConnectionLimit"] = _clientConnectionLimit;
            if (!String.IsNullOrEmpty(_proxyName))
            {
                channelProps["proxyName"] = _proxyName;
                channelProps["proxyPort"] = _proxyPort;
            }

            return new IpFixHttpClientChannel(channelProps, sinkProvider);
        }

        /// <summary>
        /// ������� ��������� ����� � ��������
        /// </summary>
        protected override string Protocol
        {
            get { return "http"; }
        }
    }
}
