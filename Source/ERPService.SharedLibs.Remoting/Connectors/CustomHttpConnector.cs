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
    /// Базовый класс для подключения к ремоутинг-объектам по HTTP
    /// </summary>
    /// <typeparam name="T">Интерфейс объекта</typeparam>
    public abstract class CustomHttpConnector<T> : CustomConnector<T>
    {
        private string _proxyName;
        private int _proxyPort;
        private int _clientConnectionLimit;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="serverNameOrIp">Имя или IP-адрес сервера</param>
        /// <param name="port">Порт сервера</param>
        /// <param name="objectName">Имя объекта</param>
        /// <param name="clientConnectionLimit">Сколько подключений может быть одновременно 
        /// открыто к зададнному серверу</param>
        /// <param name="proxyName">Имя или IP-адрес прокси-сервера</param>
        /// <param name="proxyPort">Порт прокси-сервера</param>
        protected CustomHttpConnector(string serverNameOrIp, int port, string objectName,
            int clientConnectionLimit, string proxyName, int proxyPort)
            : base(serverNameOrIp, port, objectName)
        {
            _clientConnectionLimit = clientConnectionLimit;
            _proxyName = proxyName;
            _proxyPort = proxyPort;
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        protected CustomHttpConnector()
            : base()
        {
            _clientConnectionLimit = 2;
            _proxyName = string.Empty;
            _proxyPort = 8080;
        }

        /// <summary>
        /// Имя или IP-адрес прокси-сервера
        /// </summary>
        public string ProxyName
        {
            get { return _proxyName; }
            set
            {
                ThrowIfEmpty(value, "Имя или IP-адрес прокси-сервера");
                _proxyName = value;
            }
        }

        /// <summary>
        /// Порт прокси-сервера
        /// </summary>
        public int ProxyPort
        {
            get { return _proxyPort; }
            set
            {
                ThrowIfOutOfRange(value);
                _proxyPort = value;
            }
        }

        /// <summary>
        /// Сколько подключений может быть одновременно 
        /// открыто к зададнному серверу
        /// </summary>
        public int ClientConnectionLimit
        {
            get { return _clientConnectionLimit; }
            set { _clientConnectionLimit = value; }
        }

        /// <summary>
        /// Создает клиентский канал
        /// </summary>
        /// <param name="sinkProvider">Провайдер приемников канала</param>
        /// <param name="channelName">Имя канала</param>
        /// <returns>Клиентский канал</returns>
        protected override IChannel CreateChannel(IClientChannelSinkProvider sinkProvider, string channelName)
        {
            IDictionary channelProps = GetBasicChannelProperties(channelName);
            channelProps["clientConnectionLimit"] = _clientConnectionLimit;
            if (!string.IsNullOrEmpty(_proxyName))
            {
                channelProps["proxyName"] = _proxyName;
                channelProps["proxyPort"] = _proxyPort;
            }

            return new IpFixHttpClientChannel(channelProps, sinkProvider);
        }

        /// <summary>
        /// Префикс протокола связи с объектом
        /// </summary>
        protected override string Protocol
        {
            get { return "http"; }
        }
    }
}
