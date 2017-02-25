using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Channels;
using System.Security.Permissions;
using System.Collections;

namespace ERPService.SharedLibs.Remoting.Sinks
{
    /// <summary>
    /// ��������� ��� ����������, ���������� ��� ���������� ����� ��� ��� IP-�����, 
    /// ������� � �������
    /// </summary>
    public class IpFixClientChannelSinkProvider : IClientChannelSinkProvider
    {
        #region ����

        // ������ �� ���������� ���������� � ����
        private IClientChannelSinkProvider _nextProvider;
        // ��� ���������� ����� ��� ��� IP-�����, ������� � �������
        private String _serverHostNameOrIp;

        #endregion

        #region ������������

        /// <summary>
        /// ����������� ��� ������, ����� ��������� ������������� � ������� ����� ������������
        /// </summary>
        /// <param name="properties">�������� ����������</param>
        /// <param name="providerData">������ ����������</param>
        public IpFixClientChannelSinkProvider(IDictionary properties, ICollection providerData)
        {
        }

        /// <summary>
        /// ������� ��������� ����������
        /// </summary>
        /// <param name="serverHostNameOrIp">��� ���������� ����� ��� ��� IP-�����, ������� � �������</param>
        public IpFixClientChannelSinkProvider(String serverHostNameOrIp)
        {
            if (String.IsNullOrEmpty(serverHostNameOrIp))
                throw new ArgumentNullException("serverHostNameOrIp");

            _serverHostNameOrIp = serverHostNameOrIp;
        }

        #endregion

        #region ���������� IClientChannelSinkProvider

        /// <summary>
        /// ������� ���������� �������� ���������
        /// </summary>
        /// <param name="channel">�����, ��� �������� ��������� ��������</param>
        /// <param name="url">����� ���������� �������, � �������� ����� ����������� �����������</param>
        /// <param name="remoteChannelData">������ ������</param>
        /// <returns>���������� �������� ���������</returns>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public IClientChannelSink CreateSink(IChannelSender channel, string url, object remoteChannelData)
        {
            IClientChannelSink nextSink = null;

            if (_nextProvider != null)
                // ������� ��������� �������� � ����
                nextSink = _nextProvider.CreateSink(channel, url, remoteChannelData);

            // ��������� ��� �������� � ����
            return new IpFixClientChannelSink(nextSink, _serverHostNameOrIp);
        }

        /// <summary>
        /// ��������� ��������� � ����
        /// </summary>
        public IClientChannelSinkProvider Next
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

        #region ������ ��������

        /// <summary>
        /// ��� ���������� ����� ��� ��� IP-�����, ������� � �������
        /// </summary>
        public String ServerHostNameOrIp
        {
            get
            {
                return _serverHostNameOrIp;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value");

                _serverHostNameOrIp = value;
            }
        }

        #endregion
    }
}
