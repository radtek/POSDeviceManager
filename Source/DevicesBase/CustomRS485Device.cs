using DevicesCommon;

namespace DevicesBase
{
    /// <summary>
    /// ������� ����� ��� ���������, ���������� �� ���������� RS-485
    /// </summary>
    public abstract class CustomRS485Device : CustomSerialDevice, IRS485Device
    {
        #region ����

        private int _address;

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
        public int Address
        {
            get { return _address; }
            set { _address = value; }
        }

        #endregion
    }
}

