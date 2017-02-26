using System;
using System.Text.RegularExpressions;

namespace ERPService.SharedLibs.Remoting.Connectors
{
    /// <summary>
    /// Вспомогательный класс для работы 
    /// </summary>
    public static class ConnectorHelper
    {
        private const string urlTemplate =
            @"(?<Protocol>\w+)(?:\u003A\u002F\u002F)(?<Host>\w+)(?:\u003A)(?<Port>\d+)(?:\u002F)(?<ObjectName>\w+)(?:\u002F\u003Fformat\u003D)(?<Format>\w+)";
        private const string badUrlFormat =
            "Строка подключения не соответствует формату [Протокол]://[Имя или IP-адрес]:[Порт]/[Имя объекта]/?format=[Формат данных]";
        private const string badProtocol = "Протокол [{0}] не поддерживается";
        private const string badDataFormat = "Формат данных [{0}] не поддерживается";

        /// <summary>
        /// Создает коннектор по строке подключения
        /// </summary>
        /// <typeparam name="TConnector">Тип удаленного объекта для подключения</typeparam>
        /// <param name="connectionString">Строка подключения</param>
        /// <returns>Коннектор для удаленного объекта</returns>
        /// <remarks>
        /// Примеры строк подключения:
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
