using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using ERPService.SharedLibs.Remoting.Sinks;

namespace ERPService.SharedLibs.Remoting.Connectors
{
    /// <summary>
    /// ������� ����� ��� ����������� � ���������-��������
    /// </summary>
    /// <typeparam name="T">��������� �������</typeparam>
    public abstract class CustomConnector<T>: RemotingBase
    {
        #region ����

        private T _remoteObject;
        private IChannel _channel;
        private String _serverNameOrIp;
        private Int32 _port;
        private String _objectName;
        private String _url;
        private Int32 _timeout;

        #endregion

        #region ���������

        private const String readableServerNameOrIp = "��� ��� IP-����� �������";
        private const String readableObjectName = "��� �������";
        
        /// <summary>
        /// ��� ���������� �������
        /// </summary>
        protected const String Localhost = "localhost";

        #endregion

        /// <summary>
        /// ��������� ����������, ���� �� ���������������� ��������� ��������
        /// </summary>
        /// <param name="value">�������� �������� ��� ��������</param>
        /// <param name="readableParamName">�������� �������� ��������</param>
        protected void ThrowIfEmpty(String value, String readableParamName)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentNullException("value", 
                    String.Format("�� ������ �������� \"{0}\"", readableParamName));
        }

        /// <summary>
        /// ��������� ����������, ���� �������� TCP-����� ��� ���������
        /// </summary>
        /// <param name="value">�������� ��� ��������</param>
        protected void ThrowIfOutOfRange(Int32 value)
        {
            if (value < 0 || value > UInt16.MaxValue)
                throw new ArgumentOutOfRangeException("value", value, "�������� TCP-����� ��� ���������");
        }

        /// <summary>
        /// ������� ����� ������� ������, ��������� � ���� �������� ��������,
        /// ����� ��� ���� ����� �������
        /// </summary>
        /// <param name="channelName">��� ������</param>
        /// <returns>����� ������� ������</returns>
        protected IDictionary GetBasicChannelProperties(String channelName)
        {
            IDictionary channelProps = new Hashtable();
            
            channelProps["name"] = channelName;
            channelProps["ServerNameOrIp"] = _serverNameOrIp;;
            channelProps["timeout"] = _timeout;
            
            return channelProps;
        }

        #region ������������

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="serverNameOrIp">��� ��� IP-����� �������</param>
        /// <param name="port">���� �������</param>
        /// <param name="objectName">��� �������</param>
        protected CustomConnector(String serverNameOrIp, Int32 port, String objectName)
        {
            ServerNameOrIp = serverNameOrIp;
            Port = port;
            ObjectName = objectName;
            Timeout = -1;
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <remarks>������� �������������� ������������� �������</remarks>
        protected CustomConnector()
        {
            Timeout = -1;
        }

        #endregion

        #region �������� �������� � ������

        /// <summary>
        /// ������� ���������-������
        /// </summary>
        public Int32 Timeout
        {
            get { return _timeout; }
            set
            {
                if (value < -1 || value > Int32.MaxValue)
                    throw new ArgumentOutOfRangeException("value");

                _timeout = value;
            }
        }

        /// <summary>
        /// ��� ��� IP-����� �������
        /// </summary>
        public String ServerNameOrIp
        {
            get { return _serverNameOrIp; }
            set 
            {
                ThrowIfEmpty(value, readableServerNameOrIp);
                _serverNameOrIp = value; 
            }
        }

        /// <summary>
        /// ���� �������
        /// </summary>
        public Int32 Port
        {
            get { return _port; }
            set 
            {
                ThrowIfOutOfRange(value);
                _port = value;
            }
        }

        /// <summary>
        /// ��� �������
        /// </summary>
        public String ObjectName
        {
            get { return _objectName; }
            set 
            {
                ThrowIfEmpty(value, readableObjectName);
                _objectName = value; 
            }
        }

        /// <summary>
        /// ���������-������
        /// </summary>
        public T RemoteObject
        {
            get
            {
                if (_remoteObject == null)
                {
                    // �������� �������� �������
                    ThrowIfEmpty(_serverNameOrIp, readableServerNameOrIp);
                    ThrowIfEmpty(_objectName, readableObjectName);
                    ThrowIfOutOfRange(_port);

                    // ������� ���� ����������� ����������
                    IpFixClientChannelSinkProvider customSinkProvider = 
                        new IpFixClientChannelSinkProvider(_serverNameOrIp);

                    // �������� ��������� ����������, ���������� ��� ���������� ����� ��� ��� IP-�����, 
                    // ������� � �������, ����� ���������� ����������, ���������� �� �������������� ���������
                    IClientFormatterSinkProvider formatterSinkProvider = CreateFormatterSinkProvider();
                    formatterSinkProvider.Next = customSinkProvider;

                    // ������� � ������������ �����
                    _channel = CreateChannel(formatterSinkProvider, GetChannelName("client"));
                    ChannelServices.RegisterChannel(_channel, false);

                    // ��������� URL �������
                    _url = String.Format("{0}://{1}:{2}/{3}", Protocol, _serverNameOrIp, _port, _objectName);

                    // ������� ������ �������
                    _remoteObject = (T)Activator.GetObject(typeof(T), _url);
                }

                return _remoteObject;
            }
        }

        #endregion

        #region ��� ���������� � �������-�����������

        /// <summary>
        /// ������� ��������� ����� � ��������
        /// </summary>
        protected abstract String Protocol { get; }

        /// <summary>
        /// ������� ��������� ��� ����������, ���������� �� �������������� ���������
        /// </summary>
        /// <returns>��������� ��� ����������, ���������� �� �������������� ���������</returns>
        protected abstract IClientFormatterSinkProvider CreateFormatterSinkProvider();

        /// <summary>
        /// ������� ���������� �����
        /// </summary>
        /// <param name="sinkProvider">��������� ���������� ������</param>
        /// <param name="channelName">��� ������</param>
        /// <returns>���������� �����</returns>
        protected abstract IChannel CreateChannel(IClientChannelSinkProvider sinkProvider,
            String channelName);

        #endregion

        #region ���������� ������� �������� ������

        /// <summary>
        /// �������� ����������� � ���������-�������
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            if (_remoteObject != null)
                _remoteObject = default(T);
            
            SafeUnregisterChannel(_channel);
        }

        #endregion
    }
}
