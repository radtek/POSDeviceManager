using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;

namespace ERPService.SharedLibs.Remoting.Connectors
{
    /// <summary>
    /// ����� ��� ����������� � ���������-�������� �� TCP, SOAP-�������������� ���������
    /// </summary>
    /// <typeparam name="T">��������� �������</typeparam>
    public class TcpSoapConnector<T> : CustomTcpConnector<T>
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
        public TcpSoapConnector()
            : base()
        {
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="serverNameOrIp">��� ��� IP-����� �������</param>
        /// <param name="port">���� �������</param>
        /// <param name="objectName">��� �������</param>
        public TcpSoapConnector(string serverNameOrIp, int port, string objectName)
            : base(serverNameOrIp, port, objectName)
        {
        }

        /// <summary>
        /// ������� ��������� ������ ��� ����������� � ���������� �������
        /// </summary>
        /// <param name="port">���� �������</param>
        /// <param name="objectName">��� �������</param>
        public TcpSoapConnector(int port, string objectName)
            : this(CustomConnector<T>.Localhost, port, objectName)
        {
        }
    }
}
