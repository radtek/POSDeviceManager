using System;
using System.Text.RegularExpressions;

namespace ERPService.SharedLibs.Remoting.Connectors
{
    /// <summary>
    /// ��������������� ����� ��� ������ 
    /// </summary>
    public static class ConnectorHelper
    {
        private const string urlTemplate =
            @"(?<Protocol>\w+)(?:\u003A\u002F\u002F)(?<Host>\w+)(?:\u003A)(?<Port>\d+)(?:\u002F)(?<ObjectName>\w+)(?:\u002F\u003Fformat\u003D)(?<Format>\w+)";
        private const string badUrlFormat =
            "������ ����������� �� ������������� ������� [��������]://[��� ��� IP-�����]:[����]/[��� �������]/?format=[������ ������]";
        private const string badProtocol = "�������� [{0}] �� ��������������";
        private const string badDataFormat = "������ ������ [{0}] �� ��������������";

        /// <summary>
        /// ������� ��������� �� ������ �����������
        /// </summary>
        /// <typeparam name="TConnector">��� ���������� ������� ��� �����������</typeparam>
        /// <param name="connectionString">������ �����������</param>
        /// <returns>��������� ��� ���������� �������</returns>
        /// <remarks>
        /// ������� ����� �����������:
        /// tcp://localhost:9555/remoteObject/?format=binary
        /// http://server:15000/remoteObject/?format=SOAP
        /// </remarks>
        public static CustomConnector<TConnector> Create<TConnector>(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");

            Match match = Regex.Match(connectionString, urlTemplate);
            if (match.Success)
            {
                CustomConnector<TConnector> connector;
                switch (match.Groups["Protocol"].Value)
                {
                    case "tcp":
                        switch (match.Groups["Format"].Value)
                        {
                            case "binary":
                                connector = new TcpBinaryConnector<TConnector>();
                                break;
                            case "SOAP":
                                connector = new TcpSoapConnector<TConnector>();
                                break;
                            default:
                                throw new ArgumentException(string.Format(badDataFormat,
                                    match.Groups["Format"].Value));
                        }
                        break;
                    case "http":
                        switch (match.Groups["Format"].Value)
                        {
                            case "binary":
                                connector = new HttpBinaryConnector<TConnector>();
                                break;
                            case "SOAP":
                                connector = new HttpSoapConnector<TConnector>();
                                break;
                            default:
                                throw new ArgumentException(string.Format(badDataFormat,
                                    match.Groups["Format"].Value));
                        }
                        break;
                    default:
                        throw new ArgumentException(string.Format(badProtocol,
                            match.Groups["Protocol"].Value));
                }

                connector.ServerNameOrIp = match.Groups["Host"].Value;
                connector.Port = Convert.ToInt32(match.Groups["Port"].Value);
                connector.ObjectName = match.Groups["ObjectName"].Value;
                return connector;
            }

            throw new ArgumentException(badUrlFormat);
        }
    }
}
