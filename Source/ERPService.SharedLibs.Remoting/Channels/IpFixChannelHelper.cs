using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Runtime.Remoting.Channels;

namespace ERPService.SharedLibs.Remoting.Channels
{
    /// <summary>
    /// ��������������� ����� ��� ����������� ������������ ������ URL ���������� �������
    /// </summary>
    internal class IpFixChannelHelper
    {
        private String _serverNameOrIp;

        internal IpFixChannelHelper(IDictionary channelProps)
        {
            _serverNameOrIp = channelProps["ServerNameOrIp"].ToString();
        }

        private Boolean Match(String url)
        {
            UriBuilder uriBuilder = new UriBuilder(url);
            return String.Compare(uriBuilder.Host, _serverNameOrIp, true) == 0;
        }

        /// <summary>
        /// ���������� ������� ������������ ������� ���������-������ ����� ��� ������ �����
        /// ��� ����������� � ���������� �������
        /// </summary>
        /// <param name="url">URL �������</param>
        /// <param name="remoteChannelData">�������������� ������ ������</param>
        internal Boolean Match(String url, Object remoteChannelData)
        {
            if (String.IsNullOrEmpty(url))
            {
                // ��������� ������ ������
                if (remoteChannelData is ChannelDataStore)
                {
                    foreach (String uri in ((ChannelDataStore)remoteChannelData).ChannelUris)
                    {
                        if (String.IsNullOrEmpty(uri))
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
