using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using DevicesCommon;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace DevicesBase
{
    /// <summary>
    /// ������� ����� ��� ���������, ���������� �� ���������� RS-485
    /// </summary>
    public abstract class CustomRS485Device : CustomSerialDevice, IRS485Device
    {
        #region ����

        private Int32 _address;

        #endregion

        #region �����������

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        protected CustomRS485Device()
            : base()
        {
            _address = 0;
        }

        #endregion

        #region ���������� IRS485Device

        /// <summary>
        /// ����� ����������
        /// </summary>
        public Int32 Address
        {
            get { return _address; }
            set { _address = value; }
        }

        #endregion
    }
}

