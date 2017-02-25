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
    /// �������� ��������������� ���������
    /// </summary>
    /// <typeparam name="T">��� �������, � �������� ����� ���������� ������</typeparam>
    public class TcpBinaryHost<T> : CustomTcpHost<T> where T : HostingTarget
    {
        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="target">������, � �������� ����� ���������� ������</param>
        public TcpBinaryHost(T target)
            : base(target)
        {
        }

        /// <summary>
        /// ������� ��������� ��� ����������, ���������� �� �������������� ���������
        /// </summary>
        /// <returns>��������� ��� ����������, ���������� �� �������������� ���������</returns>
        protected override IServerFormatterSinkProvider CreateFormatterSinkProvider()
        {
            BinaryServerFormatterSinkProvider sinkProvider =
                new BinaryServerFormatterSinkProvider();
            sinkProvider.TypeFilterLevel = TypeFilterLevel.Full;

            return sinkProvider;
        }
    }
}
