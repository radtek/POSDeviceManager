using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevicesCommon;
using DevicesCommon.Helpers;
using DevicesCommon.Connectors;

namespace DevmanConfig
{
    /// <summary>
    /// ������ ������������ ���������
    /// </summary>
    public partial class TurnstileTestForm : Form
    {
        /// <summary>
        /// ������������� ����������
        /// </summary>
        public string deviceId;

        /// <summary>
        /// ���� ����������
        /// </summary>
        /// <param name="deviceId">������������� ����������</param>
        internal static void TestDevice(string deviceId)
        {
            using (TurnstileTestForm testDlg = new TurnstileTestForm())
            {
                testDlg.deviceId = deviceId;
                testDlg.ShowDialog();
            }
        }

        /// <summary>
        /// �����������
        /// </summary>
        public TurnstileTestForm()
        {
            InitializeComponent();
        }

        #region ��������� �������

        private void btnRead_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                DeviceTester<ITurnstileDevice> tester =
                    new DeviceTester<ITurnstileDevice>(deviceId,
                    delegate(ITurnstileDevice device)
                    {
                        tbData.Text = device.IdentificationData.Trim();
                    });
                tester.Execute();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                DeviceTester<ITurnstileDevice> tester =
                    new DeviceTester<ITurnstileDevice>(deviceId,
                    delegate(ITurnstileDevice device)
                    {
                        device.Close(true);
                    });
                tester.Execute();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                DeviceTester<ITurnstileDevice> tester =
                    new DeviceTester<ITurnstileDevice>(deviceId,
                    delegate(ITurnstileDevice device)
                    {
                        device.Open();
                    });
                tester.Execute();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        #endregion
    }
}