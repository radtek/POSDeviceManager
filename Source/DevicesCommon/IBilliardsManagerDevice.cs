using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon.Helpers;

namespace DevicesCommon
{
    /// <summary>
    /// ��������� ����������, ������������ ����������� �������
    /// </summary>
    public interface IBilliardsManagerDevice : ISerialDevice
    {
        /// <summary>
        /// �������� ���� ��� ���������� ������
        /// </summary>
        /// <param name="billiardTableNo">����� ����������� �����</param>
        void LightsOn(Int32 billiardTableNo);

        /// <summary>
        /// ��������� ���� ��� ���������� ������
        /// </summary>
        /// <param name="billiardTableNo">����� ����������� �����</param>
        void LightsOff(Int32 billiardTableNo);
    }
}
