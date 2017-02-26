using System.Runtime.Remoting.Channels;
using System.Runtime.Serialization.Formatters;

namespace ERPService.SharedLibs.Remoting.Hosts
{
    /// <summary>
    /// ����� ��� �������� �������� � ������� HTTP-������� � 
    /// �������� ��������������� ���������
    /// </summary>
    /// <typeparam name="T">��� �������, � �������� ����� ���������� ������</typeparam>
    public class HttpBinaryHost<T> : CustomHttpHost<T> where T : HostingTarget
    {
        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="target">������, � �������� ����� ���������� ������</param>
        public HttpBinaryHost(T target)
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
