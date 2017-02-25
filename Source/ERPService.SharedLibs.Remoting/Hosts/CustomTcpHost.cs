using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;

namespace ERPService.SharedLibs.Remoting.Hosts
{
    /// <summary>
    /// Базовый класс для хостинга объектов с помощью TCP-каналов
    /// </summary>
    /// <typeparam name="T">Тип объекта, к которому нужно обеспечить доступ</typeparam>
    public abstract class CustomTcpHost<T> : CustomHost<T> where T : HostingTarget
    {
        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="target">Объект, к которому нужно обеспечить доступ</param>
        protected CustomTcpHost(T target)
            : base(target)
        {
        }

        /// <summary>
        /// Создает серверный канал
        /// </summary>
        /// <param name="sinkProvider">Провайдер канала</param>
        /// <param name="channelName">Имя канала</param>
        /// <param name="port">Порт</param>
        /// <returns>Серверный канал</returns>
        protected override IChannel CreateChannel(Int32 port, IServerChannelSinkProvider sinkProvider, 
            String channelName)
        {
            IDictionary channelProps = new Hashtable();
            channelProps["name"] = channelName;
            channelProps["port"] = port;

            return new TcpServerChannel(channelProps, sinkProvider);
        }
    }
}
