using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon;
using DevicesCommon.Connectors;
using System.Windows.Forms;

namespace DevmanConfig
{
    /// <summary>
    /// ������� ��� ���������� ������ ����������
    /// </summary>
    /// <param name="device">��������� ����������</param>
    internal delegate void DeviceTestDelegate<TIntf>(TIntf device) where TIntf : IDevice;

    /// <summary>
    /// ��������������� ����� ��� ���������� ������ 
    /// ���������
    /// </summary>
    internal sealed class DeviceTester<TIntf>
        where TIntf: IDevice
    {
        private string _deviceId;
        private DeviceTestDelegate<TIntf> _testCallback;

        /// <summary>
        /// ������� 
        /// </summary>
        /// <param name="deviceId">������������� ����������</param>
        /// <param name="testCallback">������� ��� ���������� ������</param>
        public DeviceTester(string deviceId, DeviceTestDelegate<TIntf> testCallback)
        {
            _deviceId = deviceId;
            _testCallback = testCallback;
        }

        /// <summary>
        /// ���������� �����
        /// </summary>
        public void Execute()
        {
            try
            {
                using (DeviceManagerClient dmClient = new DeviceManagerClient("localhost"))
                {
                    dmClient.Login();
                    if (dmClient.Capture(_deviceId, 5))
                    {
                        try
                        {
                            if (_testCallback != null)
                                _testCallback((TIntf)dmClient[_deviceId]);
                        }
                        finally
                        {
                            dmClient.Release(_deviceId);
                        }
                    }
                    else
                        MessageBox.Show(
                            string.Format("�� ������� �������� ������ � ���������� \"{0}\"",
                            _deviceId), "���� ����������", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                }
            }
            catch (Exception e)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("������ ������������ ���������� \"{0}\".", 
                    _deviceId));
                sb.AppendLine(string.Format("���: {0}.", e.GetType().Name));
                sb.AppendLine(string.Format("���������: {0}.", e.Message));
                sb.AppendLine("����������� �����:");
                sb.Append(e.StackTrace);

                MessageBox.Show(sb.ToString(), "���� ����������", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
