using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon;
using DevicesCommon.Helpers;
using DevicesBase;
using DevicesBase.Communicators;
using DevicesBase.Helpers;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace CasAP
{
    [Scale("CasAP")]
    public class CasAPScales : CustomSerialDevice, IScaleDevice
    {
        #region ���������

        private const byte ENQ = 0x05;
        private const byte ACK = 0x06;
        private const byte ENQ_WEIGHT = 0x11;

        #endregion

        #region ����

        private string _connStr;
        private System.Globalization.NumberFormatInfo _currNfi = new System.Globalization.NumberFormatInfo();

        #endregion

        #region �����������

        public CasAPScales()
        {
            _currNfi.NumberDecimalSeparator = ".";
        }

        #endregion

        #region ���������������� ������

        /// <summary>
        /// ��������� ���������� �����
        /// </summary>
        protected override void OnAfterActivate()
        {
            Port.DataBits = 8;
            Port.StopBits = StopBits.One;
            Port.Parity = Parity.None;
            Port.ReadTimeout = 1000;
        }

        #endregion

        #region ������� ������

        /// <summary>
        /// �������� ����� � ������ ����� �������� �������
        /// </summary>
        /// <returns></returns>
        private bool Syncronize()
        {
            int retriesCount = 5;
            do
            {
                Port.WriteByte(ENQ);
                if (Port.ReadByte() == ACK)
                    return true;
                retriesCount--;
            }
            while (retriesCount-- > 0);
            return false;
        }

        #endregion

        #region IScaleDevice Members

        /// <summary>
        /// ������ ����������� � �����
        /// </summary>
        public string ConnectionString
        {
            get { return _connStr; }
            set
            {
                _connStr = value;

                // ��������� ������ �����������
                ConnStrHelper connStrHelper = new ConnStrHelper(_connStr);

                // �������������� ����� ������ �� RS-232
                if (String.Compare(connStrHelper[1], "rs", true) != 0)
                    throw new InvalidOperationException("���� ������������ ����� ������ �� ���������� RS-232");

                // �������������� ��������� �����
                PortName = connStrHelper[2];
                Baud = Convert.ToInt32(connStrHelper[3]);
            }
        }

        /// <summary>
        /// �������� ������� � ����
        /// </summary>
        /// <param name="xmlData">������</param>
        public void Upload(string xmlData)
        {
            // �� ��������������
        }

        /// <summary>
        /// ������� ��������� ����
        /// </summary>
        public Int32 Weight
        {
            get 
            {
                int currWeight = -1;
                try
                {
                    // �������������
                    if (Syncronize())
                    {
                        // ������ ����
                        Port.WriteByte(ENQ_WEIGHT);
                        byte[] rsp = new byte[15];
                        Port.Read(rsp, 0, 15);
                        string s = Encoding.ASCII.GetString(rsp, 3, 7);
                        decimal dWeight = 0;

                        if (Decimal.TryParse(s.Trim(), System.Globalization.NumberStyles.Any, _currNfi, out dWeight))
                            return Convert.ToInt32(dWeight * 1000);
                    }
                }
                catch (TimeoutException)
                {
                    ErrorCode = new ServerErrorCode(this, GeneralError.Timeout);
                }
                return currWeight;
            }
        }

        #endregion
    }
}
