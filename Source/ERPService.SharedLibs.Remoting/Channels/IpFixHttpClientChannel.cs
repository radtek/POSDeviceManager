using System;
using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Messaging;

namespace ERPService.SharedLibs.Remoting.Channels
{
    /// <summary>
    /// ����� ��� ����� �� HTTP
    /// </summary>
    public class IpFixHttpClientChannel : HttpClientChannel
    {
        private IpFixChannelHelper _channelHelper;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="properties">�������� ������</param>
        /// <param name="sinkProvider">��������� ���������� ����������</param>
        public IpFixHttpClientChannel(IDictionary properties, IClientChannelSinkProvider sinkProvider)
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
        public override IMessageSink CreateMessageSink(string url, Object remoteChannelData,
            out string objectURI)
        {
            objectURI = string.Empty;
            return _channelHelper.Match(url, remoteChannelData) ? 
                base.CreateMessageSink(url, remoteChannelData, out objectURI) : null;
        }
    }
}
