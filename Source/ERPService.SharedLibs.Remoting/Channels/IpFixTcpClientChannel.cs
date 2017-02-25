using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

namespace ERPService.SharedLibs.Remoting.Channels
{
    /// <summary>
    /// Канал для связи по TCP
    /// </summary>
    public class IpFixTcpClientChannel : TcpClientChannel
    {
        private IpFixChannelHelper _channelHelper;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="properties">Свойства канала</param>
        /// <param name="sinkProvider">Провайдер клиентских приемников</param>
        public IpFixTcpClientChannel(IDictionary properties, IClientChannelSinkProvider sinkProvider)
            : base(properties, sinkProvider)
        {
            _channelHelper = new IpFixChannelHelper(properties);
        }

        /// <summary>
        /// Returns a channel message sink that delivers messages to the specified URL or channel data object
        /// </summary>
        /// <param name="url">The URL to which the new sink delivers messages</param>
        /// <param name="remoteChannelData">The channel data object of the remote host to which the new sink will deliver messages</param>
        /// <param name="objectURI">When this method returns, contains a URI of the new channel message sink that delivers messages to the specified URL or channel data object</param>
        /// <returns>A channel message sink that delivers messages to the specified URL or channel data object</returns>
        public override IMessageSink CreateMessageSink(String url, Object remoteChannelData, 
            out String objectURI)
        {
            objectURI = String.Empty;
            return _channelHelper.Match(url, remoteChannelData) ?
                base.CreateMessageSink(url, remoteChannelData, out objectURI) : null;
        }
    }
}
