using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;

namespace ERPService.SharedLibs.Remoting.Hosts
{
    /// <summary>
    /// Базовый класс для хостинга объектов с помощью HTTP-каналов
    /// </summary>
    /// <typeparam name="T">Тип объекта, к которому нужно обеспечить доступ</typeparam>
    public abstract class CustomHttpHost<T> : CustomHost<T> where T : HostingTarget
    {
        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="target">Объект, к которому нужно обеспечить доступ</param>
        protected CustomHttpHost(T target)
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
        protected override IChannel CreateChannel(int port, IServerChannelSinkProvider sinkProvider,
            string channelName)
        {
            IDictionary channelProps = new Hashtable();
            channelProps["name"] = channelName;
            channelProps["port"] = port;

            return new HttpServerChannel(channelProps, sinkProvider);
        }
    }
}
