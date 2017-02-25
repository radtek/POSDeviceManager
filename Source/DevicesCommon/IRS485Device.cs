using System;
using System.Collections.Generic;
using System.Text;

namespace DevicesCommon
{
    /// <summary>
    /// ��������� ���������, ���������� �� RS-485
    /// </summary>
    public interface IRS485Device : ISerialDevice
    {
        /// <summary>
        /// ����� ����������
        /// </summary>
        Int32 Address { get; set; }
    }
}
