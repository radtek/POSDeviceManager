using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Security.Permissions;
using System.IO;
using ERPService.SharedLibs.Eventlog;

namespace ERPService.SharedLibs.Remoting.Sinks
{
    /// <summary>
    /// ��������� ��������, ����������� ��� ���������� ����� ��� ��� IP-�����, 
    /// ������� � �������
    /// </summary>
    public class IpFixServerChannelSink : BaseChannelSinkWithProperties, IServerChannelSink
    {
        #region ����

        private const string _eventSource = "Channel Sink";

        // ������ �� ��������� �������� � ����
        private IServerChannelSink _nextSink;
        // ��� ���������� ����� ��� ��� IP-�����, ������� � �������
        private Object _serverHostNameOrIp;
        // ��� ���������������� �������
        private IEventLink _eventLink;

        #endregion

        #region �����������

        /// <summary>
        /// ������� ��������� ���������
        /// </summary>
        /// <param name="nextSink">������ �� ��������� �������� � ����</param>
        /// <param name="eventLink">��� ���������������� �������</param>
        [SecurityPermission(SecurityAction.LinkDemand)]
        public IpFixServerChannelSink(IServerChannelSink nextSink, IEventLink eventLink)
        {
            if (nextSink == null) 
                throw new ArgumentNullException("nextSink");

            _nextSink = nextSink;
            _eventLink = eventLink;
        }

        #endregion

        #region ���������� IServerChannelSink

        /// <summary>
        /// ����������� ��������� ������
        /// </summary>
        /// <param name="sinkStack">���� ����������</param>
        /// <param name="state">����������, ��������� � ���� ����������</param>
        /// <param name="msg">���������</param>
        /// <param name="headers">��������� ���������</param>
        /// <param name="stream">����� ���������</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public void AsyncProcessResponse(IServerResponseChannelSinkStack sinkStack, object state, 
            IMessage msg, ITransportHeaders headers, Stream stream)
        {
            // �������������� ����� ���������� ��������� � ����
            sinkStack.AsyncProcessResponse(msg, headers, stream);
        }

        /// <summary>
        /// ���������� �����, � ������� ����� ������������� �������� ���������
        /// </summary>
        /// <param name="sinkStack">���� ����������</param>
        /// <param name="state">���������, ���������� � ���� ���� ����������</param>
        /// <param name="msg">���������</param>
        /// <param name="headers">��������� ���������</param>
        /// <returns>�����, � ������� ����� ������������� �������� ���������</returns>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public Stream GetResponseStream(IServerResponseChannelSinkStack sinkStack, object state, 
            IMessage msg, ITransportHeaders headers)
        {
            return null;
        }

        /// <summary>
        /// ��������� �������� � ����
        /// </summary>
        public IServerChannelSink NextChannelSink
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
            get 
            { 
                return _nextSink; 
            }
        }

        /// <summary>
        /// ��������� ���������
        /// </summary>
        /// <param name="sinkStack">���� ����������</param>
        /// <param name="requestMsg">������</param>
        /// <param name="requestHeaders">��������� �������</param>
        /// <param name="requestStream">����� �������</param>
        /// <param name="responseMsg">�����</param>
        /// <param name="responseHeaders">��������� ������</param>
        /// <param name="responseStream">����� ������</param>
        /// <returns>��������� ��������� ���������</returns>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public ServerProcessing ProcessMessage(IServerChannelSinkStack sinkStack, IMessage requestMsg, 
            ITransportHeaders requestHeaders, Stream requestStream, out IMessage responseMsg, 
            out ITransportHeaders responseHeaders, out Stream responseStream)
        {
            // ���������, ������ �� ��� ���������� ����� ��� ��� �����
            _serverHostNameOrIp = requestHeaders["serverHostNameOrIp"];
            if (_serverHostNameOrIp != null)
            {
                // �������� ��� � �������� ������
                CallContext.SetData("serverHostNameOrIp", _serverHostNameOrIp);
                // �������������
                if (_eventLink != null)
                {
                    _eventLink.Post(_eventSource, string.Format("��� ��� IP-����� ������� ������: [{0}]",
                        _serverHostNameOrIp));
                }
            }
            else
            {
                // ������� ��������, ���������� �� ���������� �������
                CallContext.FreeNamedDataSlot("serverHostNameOrIp");
                // �������������
                if (_eventLink != null)
                {
                    _eventLink.Post(_eventSource, EventType.Warning, "��� ��� IP-����� ������� �� ������");
                }
            }

            // �������������� ��������� ��������� ���������� ��������� � ����
            sinkStack.Push(this, null);
            ServerProcessing status = _nextSink.ProcessMessage(sinkStack, requestMsg, requestHeaders, 
                requestStream, out responseMsg, out responseHeaders, out responseStream);

            return status;
        }

        #endregion
    }
}
