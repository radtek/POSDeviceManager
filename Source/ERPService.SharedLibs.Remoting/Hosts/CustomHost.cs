using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using ERPService.SharedLibs.Remoting.Sinks;
using ERPService.SharedLibs.Eventlog;

namespace ERPService.SharedLibs.Remoting.Hosts
{
    /// <summary>
    /// ������� ����� ��� �������� ��������, ��������� ����� ���������
    /// </summary>
    /// <typeparam name="T">��� �������, � �������� ����� ���������� ������</typeparam>
    public abstract class CustomHost<T> : RemotingBase where T : HostingTarget
    {
        #region ����

        // ������ ��� ����������
        private T _target;
        // ������ �� ������, �������� ������ ��� ��������� ������ � MBR-�������
        private ObjRef _targetRef;
        // ��������� �����
        private IChannel _channel;
        // ������
        private IEventLink _eventLink;

        #endregion

        #region �����������

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="target">������, � �������� ����� ���������� ������</param>
        protected CustomHost(T target)
        {
            if (target == default(T))
                throw new ArgumentNullException("target");

            _target = target;
            _eventLink = null;
        }

        #endregion

        #region �������� �������� � ������

        /// <summary>
        /// ������ ��� ���������������� ������ �����
        /// </summary>
        public IEventLink EventLink
        {
            get { return _eventLink; }
            set { _eventLink = value; }
        }

        /// <summary>
        /// ���������� ������� c �������������� ����� ������� � �����, ������������ �������������
        /// </summary>
        /// <param name="objectName">��� �������</param>
        /// <param name="port">����</param>
        public void Marshal(string objectName, int port)
        {
            CheckCustomErrors();

            // ������������ ������
            IpFixTrackingHandler.RegisterTracker(_eventLink);

            // ������� ���������� ��� ����������, ����������� �� �������������� ���������
            IServerFormatterSinkProvider formatterSinkProvider = CreateFormatterSinkProvider();

            // ������� ���������� ��� ����������, ����������� ��� ���������� ����� 
            // ��� ��� IP-�����, ������� � �������
            IpFixServerChannelSinkProvider customSinkProvider = new IpFixServerChannelSinkProvider();
            customSinkProvider.EventLink = _eventLink;
            // ��������� ����������� � ����
            customSinkProvider.Next = formatterSinkProvider;

            // ������� ��������� �����
            _channel = CreateChannel(port, customSinkProvider, GetChannelName("server"));
            ChannelServices.RegisterChannel(_channel, false);

            // ��������� ������
            _targetRef = RemotingServices.Marshal(_target, objectName);
        }

        /// <summary>
        /// ���������� ������� c �������������� ����� ������� � �����, ��������� � �������
        /// </summary>
        public void Marshal()
        {
            Marshal(_target.Name, _target.Port);
        }


        /// <summary>
        /// ���������� ������� c �������������� ����� �������, ������������� �������������
        /// </summary>
        /// <param name="objectName">��� �������</param>
        /// <remarks>������������ ����, ��������� � �������</remarks>
        public void Marshal(string objectName)
        {
            Marshal(objectName, _target.Port);
        }

        /// <summary>
        /// ���������� ������� c �������������� �����, ������������� �������������
        /// </summary>
        /// <param name="port">����</param>
        /// <remarks>������������ ��� �������, ��������� � �������</remarks>
        public void Marshal(int port)
        {
            Marshal(_target.Name, port);
        }

        /// <summary>
        /// ����������� ������� � ������� ����� ���������
        /// </summary>
        /// <param name="disposeTarget">�������� IDisposable.Dispose � �������, 
        /// � �������� ������������� ������<see cref="System.IDisposable"/></param>
        public void Unmarshal(bool disposeTarget)
        {
            if (_targetRef != null)
            {
                RemotingServices.Unmarshal(_targetRef);
                _targetRef = null;
            }

            SafeUnregisterChannel(_channel);

            if (_target != null && disposeTarget)
                _target.Dispose();
        }

        /// <summary>
        /// ����������� ������� � ������� ����� ��������� c ������� IDisposable.Dispose � �������, 
        /// � �������� ������������� ������<see cref="System.IDisposable"/>
        /// </summary>
        public void Unmarshal()
        {
            Unmarshal(true);
        }

        /// <summary>
        /// ������, � �������� �������������� ������
        /// </summary>
        public T Target
        {
            get { return _target; }
        }

        #endregion

        #region ��� ���������� � �����������

        /// <summary>
        /// ������� ��������� ��� ����������, ���������� �� �������������� ���������
        /// </summary>
        /// <returns>��������� ��� ����������, ���������� �� �������������� ���������</returns>
        protected abstract IServerFormatterSinkProvider CreateFormatterSinkProvider();

        /// <summary>
        /// ������� ��������� �����
        /// </summary>
        /// <param name="sinkProvider">��������� ���������� ������</param>
        /// <param name="channelName">��� ������</param>
        /// <param name="port">���� ��� ���������� �������</param>
        /// <returns>��������� �����</returns>
        protected abstract IChannel CreateChannel(int port, 
            IServerChannelSinkProvider sinkProvider, string channelName);

        #endregion

        #region �������� ������

        private void CheckCustomErrors()
        {
            if (RemotingConfiguration.CustomErrorsMode != CustomErrorsModes.Off)
                RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
        }

        #endregion

        #region ���������� ������� �������� ������

        /// <summary>
        /// ����������� ������� � ������� ����� ��������� c ������� IDisposable.Dispose � �������, 
        /// � �������� ������������� ������<see cref="System.IDisposable"/>
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            Unmarshal();
        }

        #endregion
    }
}
