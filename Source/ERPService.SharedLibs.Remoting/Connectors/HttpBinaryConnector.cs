using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;

namespace ERPService.SharedLibs.Remoting.Connectors
{
    /// <summary>
    /// Класс для подключения к ремоутинг-объектам по HTTP, бинарное форматирование сообщений
    /// </summary>
    /// <typeparam name="T">Интерфейс объекта</typeparam>
    public class HttpBinaryConnector<T> : CustomHttpConnector<T>
    {
        /// <summary>
        /// Создает провайдер для приемников, отвечающих за форматирование сообщений
        /// </summary>
        /// <returns>Провайдер для приемников, отвечающих за форматирование сообщений</returns>
        protected override IClientFormatterSinkProvider CreateFormatterSinkProvider()
        {
            return new BinaryClientFormatterSinkProvider();
        }

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
        public HttpBinaryConnector(String serverNameOrIp, Int32 port, String objectName,
            Int32 clientConnectionLimit, String proxyName, Int32 proxyPort)
            : base(serverNameOrIp, port, objectName, clientConnectionLimit, proxyName, proxyPort)
        {
        }

        /// <summary>
        /// Создает экземпляр класса для подключения к локальному сервису
        /// </summary>
        /// <param name="port">Порт сервера</param>
        /// <param name="objectName">Имя объекта</param>
        /// <param name="clientConnectionLimit">Сколько подключений может быть одновременно 
        /// открыто к зададнному серверу</param>
        /// <param name="proxyName">Имя или IP-адрес прокси-сервера</param>
        /// <param name="proxyPort">Порт прокси-сервера</param>
        public HttpBinaryConnector(Int32 port, String objectName, Int32 clientConnectionLimit,
            String proxyName, Int32 proxyPort)
            : this(CustomConnector<T>.Localhost, port, objectName, clientConnectionLimit, 
            proxyName, proxyPort)
        {
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        public HttpBinaryConnector()
            : base()
        {
        }
    }
}
