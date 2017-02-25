using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Serialization.Formatters;

namespace ERPService.SharedLibs.Remoting.Hosts
{
    /// <summary>
    /// ����� ��� �������� �������� � ������� TCP-������� � 
    /// SOAP-��������������� ���������
    /// </summary>
    /// <typeparam name="T">��� �������, � �������� ����� ���������� ������</typeparam>
    public class TcpSoapHost<T> : CustomTcpHost<T> where T : HostingTarget
    {
        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="target">������, � �������� ����� ���������� ������</param>
        public TcpSoapHost(T target)
            : base(target)
        {
        }

        /// <summary>
        /// ������� ��������� ��� ����������, ���������� �� �������������� ���������
        /// </summary>
        /// <returns>��������� ��� ����������, ���������� �� �������������� ���������</returns>
        protected override IServerFormatterSinkProvider CreateFormatterSinkProvider()
        {
            SoapServerFormatterSinkProvider sinkProvider = new SoapServerFormatterSinkProvider();
            sinkProvider.TypeFilterLevel = TypeFilterLevel.Full;

            return sinkProvider;
        }
    }
}
