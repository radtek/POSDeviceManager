using System.Runtime.Remoting.Channels;

namespace ERPService.SharedLibs.Remoting.Connectors
{
    /// <summary>
    /// ����� ��� ����������� � ���������-�������� �� TCP, �������� �������������� ���������
    /// </summary>
    /// <typeparam name="T">��������� �������</typeparam>
    public class TcpBinaryConnector<T> : CustomTcpConnector<T>
    {
        /// <summary>
        /// ������� ��������� ��� ����������, ���������� �� �������������� ���������
        /// </summary>
        /// <returns>��������� ��� ����������, ���������� �� �������������� ���������</returns>
        protected override IClientFormatterSinkProvider CreateFormatterSinkProvider()
        {
            return new BinaryClientFormatterSinkProvider();
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        public TcpBinaryConnector()
            : base()
        {
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="serverNameOrIp">��� ��� IP-����� �������</param>
        /// <param name="port">���� �������</param>
        /// <param name="objectName">��� �������</param>
        public TcpBinaryConnector(string serverNameOrIp, int port, string objectName)
            : base(serverNameOrIp, port, objectName)
        {
        }

        /// <summary>
        /// ������� ��������� ������ ��� ����������� � ���������� �������
        /// </summary>
        /// <param name="port">���� �������</param>
        /// <param name="objectName">��� �������</param>
        public TcpBinaryConnector(int port, string objectName)
            : this(CustomConnector<T>.Localhost, port, objectName)
        {
        }
    }
}
