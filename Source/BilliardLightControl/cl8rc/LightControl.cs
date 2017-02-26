using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using ERPService.SharedLibs.Helpers.SerialCommunications;
using DevicesBase;
using DevicesCommon;
using DevicesCommon.Helpers;
using DevicesBase.Helpers;

namespace cl8rc
{
    /// <summary>
    /// ��������������� ����� "�������� ������"
    /// </summary>
    internal class RelayModule
    {
        IDevice _parent;
        EasyCommunicationPort _port;
        byte _address;
        byte _relayStatus;
        bool _connected;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="parent">������������ ����������</param>
        /// <param name="port">���������������� ����</param>
        /// <param name="address">����� ������</param>
        public RelayModule(IDevice parent, EasyCommunicationPort port, byte address)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            if (port == null)
                throw new ArgumentNullException("port");
            if (address > 7)
                throw new ArgumentOutOfRangeException("address");

            _parent = parent;
            _port = port;
            _address = address;

            // ���������� ��� ���� ������ � "���������"
            ExecuteCommand(0, 1);
        }

        /// <summary>
        /// ���������� �������
        /// </summary>
        /// <param name="relayNo">����� ����</param>
        /// <param name="switchOn">��������</param>
        public ErrorCode ExecuteCommand(byte relayNo, bool switchOn)
        {
            if (!_connected)
                throw new InvalidOperationException(
                    string.Format("������ {0:X2} ��������", _address));
            if (relayNo > 7)
                throw new ArgumentOutOfRangeException("relayNo");

            // �������������
            int switchByte = Pow2(relayNo);

            // ��������� ����� ������ ���� ������
            byte nextRelayStatus = switchOn ?
                (byte)(_relayStatus | switchByte) :
                (byte)(_relayStatus & ~switchByte);

            // ��������� �������
            return ExecuteCommand(nextRelayStatus, 3);
        }

        /// <summary>
        /// ������� ������
        /// </summary>
        /// <param name="pow">���������� �������</param>
        private int Pow2(int pow)
        {
            if (pow < 1)
                return 1;

            int pow2 = 1;
            for (int i = 1; i <= pow; i++)
            {
                pow2 = pow2 * 2;
            }
            return pow2;
        }

        private ErrorCode OnException(bool saveErrors, Exception e)
        {
            if (saveErrors)
                return new ServerErrorCode(_parent, e);

            _connected = false;
            return new ServerErrorCode(_parent, GeneralError.Success);
        }

        /// <summary>
        /// ���������� �������
        /// </summary>
        /// <param name="nextRelayStatus">����� ��������� ����</param>
        /// <param name="retryCount">����� ������� �������</param>
        private ErrorCode ExecuteCommand(byte nextRelayStatus, byte retryCount)
        {

            // ��������� �������
            string command = string.Format("@{0:X2}{1:X2}00\r",
                _address, nextRelayStatus);

            bool saveConnected = retryCount == 1;
            ErrorCode storedErrorCode;
            do
            {
                try
                {
                    // ���������� ������ �����
                    _port.DiscardBuffers();

                    // ���������� ������� � ����
                    _port.Write(Encoding.Default.GetBytes(command));

                    // ������ �����
                    byte[] answer = new byte[4];
                    _port.Read(answer, 0, answer.Length);

                    if (saveConnected)
                        _connected = true;

                    if (answer[0] == (byte)'!')
                    {
                        _relayStatus = nextRelayStatus;
                        return new ServerErrorCode(_parent, GeneralError.Success);
                    }

                    return new ServerErrorCode(_parent, 1, "������� �� ���������");
                }
                catch (TimeoutException e)
                {
                    retryCount--;
                    storedErrorCode = OnException(!saveConnected, e);
                }
                catch (Win32Exception e)
                {
                    retryCount--;
                    storedErrorCode = OnException(!saveConnected, e);
                }
            }
            while (retryCount > 0);
            return storedErrorCode;
        }
    }

    /// <summary>
    /// ���������� ���������� ��� �������� � ������� ����������� 
    /// CL-8RC
    /// </summary>
    [BilliardsManager(DeviceNames.blcCl8rc)]
    public class LightControl : CustomBilliardsManagerDevice
    {
        private RelayModule[] _modules;

        #region �����������

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        public LightControl()
            : base()
        {
            _modules = new RelayModule[8];
        }

        #endregion

        #region ���������� ��������� ������

        protected override void OnAfterActivate()
        {
            base.OnAfterActivate();
            Port.DsrFlow = false;
            Port.WriteTimeout = 2000;
            Port.ReadTimeout = 1000;

            // ��������� ����� �������
            for (int i = 0; i < _modules.Length; i++)
                _modules[i] = new RelayModule(this, Port, (byte)i);
        }

        private void SwitchTableLight(int billiardTableNo, bool switchOn)
        {
            if (billiardTableNo < 1 || billiardTableNo > 64)
                throw new ArgumentOutOfRangeException("billiardTableNo");

            // ���������� ����� ������ �� ����������� ������ �����
            int moduleAddress = (billiardTableNo - 1) / 8;

            // ��������� �������
            ErrorCode = _modules[moduleAddress].ExecuteCommand((byte)((billiardTableNo - 1) % 8), switchOn);
        }

        #endregion

        /// <summary>
        /// ��������� ����
        /// </summary>
        /// <param name="billiardTableNo">����� �����</param>
        public override void LightsOff(int billiardTableNo)
        {
            SwitchTableLight(billiardTableNo, false);
        }

        /// <summary>
        /// �������� ����
        /// </summary>
        /// <param name="billiardTableNo">����� �����</param>
        public override void LightsOn(int billiardTableNo)
        {
            SwitchTableLight(billiardTableNo, true);
        }
    }
}
