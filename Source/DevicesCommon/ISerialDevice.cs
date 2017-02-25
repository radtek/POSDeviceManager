using System;
using System.Collections.Generic;
using System.Text;

namespace DevicesCommon
{
    /// <summary>
    /// ����������, ������������ �� ����������������� �����
    /// </summary>
    public interface ISerialDevice : IConnectableDevice
    {
        /// <summary>
        /// �������� �������� ������ ����� ���� (���)
        /// </summary>
        Int32 Baud { get; set; }
    }
}
