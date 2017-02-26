using System;
using System.Collections;
using System.Runtime.Remoting.Channels;

namespace ERPService.SharedLibs.Remoting.Channels
{
    /// <summary>
    /// ��������������� ����� ��� ����������� ������������ ������ URL ���������� �������
    /// </summary>
    internal class IpFixChannelHelper
    {
        private string _serverNameOrIp;

        internal IpFixChannelHelper(IDictionary channelProps)
        {
            _serverNameOrIp = channelProps["ServerNameOrIp"].ToString();
        }

        private bool Match(string url)
        {
            UriBuilder uriBuilder = new UriBuilder(url);
            return string.Compare(uriBuilder.Host, _serverNameOrIp, true) == 0;
        }

        /// <summary>
        /// ���������� ������� ������������ ������� ���������-������ ����� ��� ������ �����
        /// ��� ����������� � ���������� �������
        /// </summary>
        /// <param name="url">URL �������</param>
        /// <param name="remoteChannelData">�������������� ������ ������</param>
        internal bool Match(string url, Object remoteChannelData)
        {
            if (string.IsNullOrEmpty(url))
            {
                // ��������� ������ ������
                if (remoteChannelData is ChannelDataStore)
                {
                    foreach (string uri in ((ChannelDataStore)remoteChannelData).ChannelUris)
                    {
                        if (string.IsNullOrEmpty(uri))
                            continue;

                        if (Match(uri))
                            return true;
                    }
                    return false;
                }
                else
                    return false;
            }
            else
                return Match(url);
        }
    }
}
