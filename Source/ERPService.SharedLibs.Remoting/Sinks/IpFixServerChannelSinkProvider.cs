using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Security.Permissions;
using ERPService.SharedLibs.Eventlog;

namespace ERPService.SharedLibs.Remoting.Sinks
{
    /// <summary>
    /// ��������� ����������, ����������� ��� ���������� ����� ��� ��� IP-�����, 
    /// ������� � �������
    /// </summary>
    public class IpFixServerChannelSinkProvider : IServerChannelSinkProvider
    {
        // ������ �� ���������� ���������� � ����
        private IServerChannelSinkProvider _nextProvider;
        // ��� ���������������� �������
        private IEventLink _eventLink;

        #region �������� ��������

        /// <summary>
        /// ��� ���������������� �������
        /// </summary>
        public IEventLink EventLink
        {
            get { return _eventLink; }
            set { _eventLink = value; }
        }

        #endregion

        #region ������������

        /// <summary>
        /// ����������� ��� ������, ����� ��������� ������������� � ������� ����� ������������
        /// </summary>
        /// <param name="properties">�������� ����������</param>
        /// <param name="providerData">������ ����������</param>
        public IpFixServerChannelSinkProvider(IDictionary properties, ICollection providerData) 
        { 
        }

        /// <summary>
        /// ������� ��������� ����������
        /// </summary>
        public IpFixServerChannelSinkProvider()
        {
        }

        #endregion

        #region ���������� IServerChannelSinkProvider

        /// <summary>
        /// ������� ��������� �������� ���������
        /// </summary>
        /// <param name="channel">�����, ��� �������� ��������� ��������</param>
        /// <returns>��������� �������� ���������</returns>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public IServerChannelSink CreateSink(IChannelReceiver channel)
        {
            IServerChannelSink nextSink = null;

            if (_nextProvider != null)
                nextSink = _nextProvider.CreateSink(channel);
            return new IpFixServerChannelSink(nextSink, _eventLink);
        }

        /// <summary>
        /// ���������� ������ ������
        /// </summary>
        /// <param name="channelData">������, � ������� ������������ ������ ������</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public void GetChannelData(IChannelDataStore channelData)
        {
        }

        /// <summary>
        /// ��������� ��������� � ����
        /// </summary>
        public IServerChannelSinkProvider Next
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
            get
            {
                return _nextProvider;
            }
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
            set
            {
                _nextProvider = value;
            }
        }

        #endregion
    }
}
