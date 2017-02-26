using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Services;
using ERPService.SharedLibs.Eventlog;

namespace ERPService.SharedLibs.Remoting
{
    /// <summary>
    /// ������, ���������� ����������� �� ����������, �������� � ���������� ��������,
    /// ��������� ����� ���������-��������������
    /// </summary>
    public class IpFixTrackingHandler : ITrackingHandler
    {
        #region ���������� ���������

        // ��� ��������� �������
        private const string EventSource = "Tracking handler";

        // ��� �������������
        private static Object _syncObject = new Object();
        // ��������� 
        private static IpFixTrackingHandler _instance = null;
        // ������
        private IEventLink _eventLink;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="eventLink">������</param>
        /// <remarks>���������� ����� ���� ������ ������� ����, ������� ����������� ������</remarks>
        private IpFixTrackingHandler(IEventLink eventLink)
        {
            _eventLink = eventLink;
        }

        /// <summary>
        /// ����������� ������� � ���������-��������������
        /// </summary>
        public static void RegisterTracker(IEventLink eventLink)
        {
            lock (_syncObject)
            {
                if (_instance == null)
                    // ������� ������, ���� �� ��� �� ������
                    _instance = new IpFixTrackingHandler(eventLink);

                // ���������, �� ��������������� �� ��� ���� ����� �� ������
                foreach (ITrackingHandler thrackingHandler in TrackingServices.RegisteredHandlers)
                {
                    if (thrackingHandler is IpFixTrackingHandler)
                        // ������ ����� �� ������ �������������� �� �����
                        return;
                }

                // ������������ ���
                TrackingServices.RegisterTrackingHandler(_instance);
            }
        }

        #endregion

        /// <summary>
        /// ������ ����� ����� ��� IP-������
        /// </summary>
        /// <param name="dataStore">������ ������</param>
        /// <param name="serverHostNameOrIp">��� ��� ����� �����, ���������� � �������</param>
        private void ReplaceHostNameOrIp(ChannelDataStore dataStore, string serverHostNameOrIp)
        {
            for (int i = 0; i < dataStore.ChannelUris.Length; i++)
            {
                if (_eventLink != null)
                {
                    _eventLink.Post(EventSource, string.Format(
                        "�������� URI � ������ ������ �����: {0}", dataStore.ChannelUris[i]));
                }

                UriBuilder ub = new UriBuilder(dataStore.ChannelUris[i]);
                
                // ���������� ��� ����� � URI ������ � �� ��, ���������� � �������
                if (string.Compare(ub.Host, serverHostNameOrIp, true) != 0)
                {
                    // ������ �� ��������, ���������� � �������
                    ub.Host = serverHostNameOrIp;
                    dataStore.ChannelUris[i] = ub.ToString();

                    if (_eventLink != null)
                    {
                        _eventLink.Post(EventSource, string.Format(
                            "���� �������. ����� URI: {0}", dataStore.ChannelUris[i]));
                    }
                }
            }
        }

        #region ���������� ITrackingHandler

        /// <summary>
        /// ���������� ������� �� ��� ������
        /// </summary>
        /// <param name="obj">����������� ������</param>
        public void DisconnectedObject(object obj)
        {
        }

        /// <summary>
        /// ���������� �������
        /// </summary>
        /// <param name="obj">������</param>
        /// <param name="or">ObjRef �������<see cref="System.Runtime.Remoting.ObjRef"/></param>
        public void MarshaledObject(object obj, ObjRef or)
        {
            // �������� �������� ��� ��� ����� ���������� ����� �� ��������� ������
            Object serverHostNameOrIp = CallContext.GetData("serverHostNameOrIp");
            if (_eventLink != null)
            {
                _eventLink.Post(EventSource, string.Format("���������� ������� {0}, URI {1}", 
                    obj.GetType(), or.URI));
                _eventLink.Post(EventSource, string.Format(
                    "��� ��� IP-����� �������, ���������� �� ��������� ������: [{0}]", serverHostNameOrIp));
            }

            if (serverHostNameOrIp != null)
            {
                // ��������, ���� �� ������������� � ������� ����� ����� ��� ������ ��� 
                // ������������ �������
                foreach (Object channelData in or.ChannelInfo.ChannelData)
                {
                    if (channelData is ChannelDataStore)
                    {
                        if (_eventLink != null)
                            _eventLink.Post(EventSource, "���������� ������ ������ �����");

                        // URI, �� �������� �������� ����������� ������, 
                        // �������� � ChannelDataStore. �������� ���� ChannelDataStore
                        // ����� ���� ���������, � ����������� �� ����� ��������� �������,
                        // �� ������� ����� �������� ������ � ��������
                        ReplaceHostNameOrIp((ChannelDataStore)channelData, serverHostNameOrIp.ToString());
                    }
                }
            }

            if (_eventLink != null)
            {
                _eventLink.Post(EventSource, string.Format("������ {0}, URI {1} �����������",
                    obj.GetType(), or.URI));
            }
        }

        /// <summary>
        /// �������� �������
        /// </summary>
        /// <param name="obj">������</param>
        /// <param name="or">ObjRef �������<see cref="System.Runtime.Remoting.ObjRef"/></param>
        public void UnmarshaledObject(object obj, ObjRef or)
        {
        }

        #endregion
    }
}
