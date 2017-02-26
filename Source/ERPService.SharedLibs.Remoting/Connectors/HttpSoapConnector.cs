using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;

namespace ERPService.SharedLibs.Remoting.Connectors
{
    /// <summary>
    /// ����� ��� ����������� � ���������-�������� �� HTTP, SOAP-�������������� ���������
    /// </summary>
    /// <typeparam name="T">��������� �������</typeparam>
    public class HttpSoapConnector<T> : CustomHttpConnector<T>
    {
        /// <summary>
        /// ������� ��������� ��� ����������, ���������� �� �������������� ���������
        /// </summary>
        /// <returns>��������� ��� ����������, ���������� �� �������������� ���������</returns>
        protected override IClientFormatterSinkProvider CreateFormatterSinkProvider()
        {
            return new SoapClientFormatterSinkProvider();
        }

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
        public HttpSoapConnector(string serverNameOrIp, int port, string objectName,
            int clientConnectionLimit, string proxyName, int proxyPort)
            : base(serverNameOrIp, port, objectName, clientConnectionLimit, proxyName, proxyPort)
        {
        }

        /// <summary>
        /// ������� ��������� ������ ��� ����������� � ���������� �������
        /// </summary>
        /// <param name="port">���� �������</param>
        /// <param name="objectName">��� �������</param>
        /// <param name="clientConnectionLimit">������� ����������� ����� ���� ������������ 
        /// ������� � ���������� �������</param>
        /// <param name="proxyName">��� ��� IP-����� ������-�������</param>
        /// <param name="proxyPort">���� ������-�������</param>
        public HttpSoapConnector(int port, string objectName, int clientConnectionLimit,
            string proxyName, int proxyPort)
            : this(CustomConnector<T>.Localhost, port, objectName, clientConnectionLimit, 
            proxyName, proxyPort)
        {
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        public HttpSoapConnector()
            : base()
        {
        }
    }
}
