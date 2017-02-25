using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon.Helpers;

namespace DevicesCommon
{
    /// <summary>
    /// ���� � ������� ��������
    /// </summary>
    public interface IScaleDevice : IDevice
    {
        /// <summary>
        /// �������� ������ � ����
        /// </summary>
        /// <param name="xmlData"></param>
        void Upload(String xmlData);

        /// <summary>
        /// ������� ��������� ����
        /// </summary>
        Int32 Weight { get; }

        /// <summary>
        /// ������ � ����������� ����������� � �����
        /// <example>
        /// tcp://host:port
        /// udp://host:port
        /// rs://port_name:baud
        /// </example>
        /// </summary>
        String ConnectionString { get; set; }
    }
}
