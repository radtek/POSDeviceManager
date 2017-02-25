using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon;

namespace DevicesBase
{
    /// <summary>
    /// ������� ����� ��� ���������, ����������� ����������� �������
    /// </summary>
    public abstract class CustomBilliardsManagerDevice : CustomSerialDevice, 
        IBilliardsManagerDevice
    {
        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        protected CustomBilliardsManagerDevice() : base()
        {
        }

        #region ���������� IBilliardsManagerDevice

        /// <summary>
        /// ���������� ����� ��� ����������� ������
        /// </summary>
        /// <param name="billiardTableNo">����� ����������� �����</param>
        public abstract void LightsOff(int billiardTableNo);

        /// <summary>
        /// ��������� ����� ��� ���������� ������
        /// </summary>
        /// <param name="billiardTableNo">����� ����������� �����</param>
        public abstract void LightsOn(int billiardTableNo);

        #endregion
    }
}
