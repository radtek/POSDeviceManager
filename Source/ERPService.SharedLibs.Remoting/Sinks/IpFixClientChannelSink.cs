using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Security.Permissions;
using System.IO;
using System.Collections;

namespace ERPService.SharedLibs.Remoting.Sinks
{
    /// <summary>
    /// ���������� ��������, ���������� ��� ���������� ����� ��� ��� IP-�����, 
    /// ������� � �������
    /// </summary>
    public class IpFixClientChannelSink : BaseChannelSinkWithProperties, IClientChannelSink
    {
        #region ����

        // ������ �� ��������� �������� � ����
        private IClientChannelSink _nextSink;
        // ��� ���������� ����� ��� ��� IP-�����, ������� � �������
        private String _serverHostNameOrIp;

        #endregion

        #region �����������

        /// <summary>
        /// ������� ���������� ��������
        /// </summary>
        /// <param name="nextSink">������ �� ��������� �������� � ����</param>
        /// <param name="serverHostNameOrIp">��� ���������� ����� ��� ��� IP-�����, ������� � �������</param>
        [SecurityPermission(SecurityAction.LinkDemand)]
        public IpFixClientChannelSink(IClientChannelSink nextSink, String serverHostNameOrIp)
        {
            if (nextSink == null)
                throw new ArgumentNullException("nextSink");
            if (String.IsNullOrEmpty(serverHostNameOrIp))
                throw new ArgumentNullException("serverHostNameOrIp");

            _nextSink = nextSink;
            _serverHostNameOrIp = serverHostNameOrIp;
        }

        #endregion

        #region ���������� IClientChannelSink

        /// <summary>
        /// ����������� ��������� �������
        /// </summary>
        /// <param name="sinkStack">���� ����������</param>
        /// <param name="msg">���������</param>
        /// <param name="headers">��������� �������</param>
        /// <param name="stream">����� �������</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public void AsyncProcessRequest(IClientChannelSinkStack sinkStack, IMessage msg, 
            ITransportHeaders headers, Stream stream)
        {
            // �������������� ����� ���������� ��������� � �����
            sinkStack.Push(this, null);
            _nextSink.AsyncProcessRequest(sinkStack, msg, headers, stream);
        }

        /// <summary>
        /// ����������� ��������� ������
        /// </summary>
        /// <param name="sinkStack">���� ����������</param>
        /// <param name="state">����������, ��������� � ���� ����������</param>
        /// <param name="headers">��������� ������</param>
        /// <param name="stream">����� ������</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public void AsyncProcessResponse(IClientResponseChannelSinkStack sinkStack, object state, 
            ITransportHeaders headers, Stream stream)
        {
            // �������������� ����� ���������� ��������� � �����
            sinkStack.AsyncProcessResponse(headers, stream);
        }

        /// <summary>
        /// ���������� �����, � ������� ����� ������������� ���������
        /// </summary>
        /// <param name="msg">���������</param>
        /// <param name="headers">��������� ���������</param>
        /// <returns>�����, � ������� ����� ������������� ���������</returns>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public Stream GetRequestStream(IMessage msg, ITransportHeaders headers)
        {
            // �������������� ����� ���������� ��������� � �����
            return _nextSink.GetRequestStream(msg, headers);
        }

        /// <summary>
        /// ������ �� ��������� �������� � ����
        /// </summary>
        public IClientChannelSink NextChannelSink
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
            get 
            { 
                return _nextSink; 
            }
        }

        /// <summary>
        /// ��������� ��������� ����������
        /// </summary>
        /// <param name="msg">���������</param>
        /// <param name="requestHeaders">��������� �������</param>
        /// <param name="requestStream">����� �������</param>
        /// <param name="responseHeaders">��������� ������</param>
        /// <param name="responseStream">����� ������</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public void ProcessMessage(IMessage msg, ITransportHeaders requestHeaders, Stream requestStream, 
            out ITransportHeaders responseHeaders, out Stream responseStream)
        {
            // �������� ��� ���������� ����� ��� ��� IP-����� � ��������� �������
            requestHeaders["serverHostNameOrIp"] = _serverHostNameOrIp;

            // �������������� ����� ���������� ��������� � �����
            _nextSink.ProcessMessage(msg, requestHeaders, requestStream, 
                out responseHeaders, out responseStream);
        }

        #endregion
    }
}
