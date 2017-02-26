using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;
using ERPService.SharedLibs.Remoting.Channels;

namespace ERPService.SharedLibs.Remoting.Connectors
{
    /// <summary>
    /// Базовый класс для подключения к ремоутинг-объектам по TCP
    /// </summary>
    /// <typeparam name="T">Интерфейс объекта</typeparam>
    public abstract class CustomTcpConnector<T> : CustomConnector<T>
    {
        /// <summary>
        /// Префикс протокола связи с объектом
        /// </summary>
        protected override string Protocol
        {
            get { return "tcp"; }
        }

        /// <summary>
        /// Создает клиентский канал
        /// </summary>
        /// <param name="sinkProvider">Провайдер приемников канала</param>
        /// <param name="channelName">Имя канала</param>
        /// <returns>Клиентский канал</returns>
        protected override IChannel CreateChannel(IClientChannelSinkProvider sinkProvider, string channelName)
        {
            return new IpFixTcpClientChannel(GetBasicChannelProperties(channelName), sinkProvider);
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        protected CustomTcpConnector()
            : base()
        {
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="serverNameOrIp">Имя или IP-адрес сервера</param>
        /// <param name="port">Порт сервера</param>
        /// <param name="objectName">Имя объекта</param>
        protected CustomTcpConnector(string serverNameOrIp, int port, string objectName)
            : base(serverNameOrIp, port, objectName)
        {
        }
    }
}
