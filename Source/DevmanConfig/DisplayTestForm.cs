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
    /// ����� ��� ������������ ������� ����������
    /// </summary>
    internal partial class DisplayTestForm : Form
    {
        /// <summary>
        /// ������������� ����������
        /// </summary>
        public string deviceId;


        internal static void TestDisplay(string deviceId)
        {
            using (DisplayTestForm testDlg = new DisplayTestForm())
            {
                testDlg.deviceId = deviceId;
                testDlg.ShowDialog();
            }
        }


        /// <summary>
        /// ����������� �����
        /// </summary>
        public DisplayTestForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ����� ������� ������� �������
        /// </summary>
        private void UpdateCurrentPos()
        {
            int rowNo = 0;
            int currPos = 0;
            int pos = 0;

            pos = tbDisplayLines.Text.IndexOf("\n", currPos);
            while (pos >= 0 && pos < tbDisplayLines.SelectionStart + tbDisplayLines.SelectionLength)
            {
                currPos = pos + 1;
                rowNo++;
                pos = tbDisplayLines.Text.IndexOf("\n", currPos);
            }

            lbPosition.Text = string.Format("{0}:{1}", tbDisplayLines.SelectionStart + tbDisplayLines.SelectionLength - currPos + 1, rowNo + 1);
        }

        /// <summary>
        /// ������������ ����������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTest(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                DeviceTester<ICustomerDisplay> tester = new DeviceTester<ICustomerDisplay>(deviceId,
                    delegate(ICustomerDisplay device)
                    {
                        // ����� ������
                        for (int i = 0; i < tbDisplayLines.Lines.Length; i++)
                            device[i] = tbDisplayLines.Lines[i];
                        // ������ � ������ �������
                        if (cbSaveLines.Checked)
                            device.SaveToEEPROM();
                    });
                tester.Execute();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        #region ��������� ������� ����������� �������

        private void tbDisplayLines_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateCurrentPos();
        }

        private void tbDisplayLines_MouseClick(object sender, MouseEventArgs e)
        {
            UpdateCurrentPos();
        }

        private void DisplayTestForm_Shown(object sender, EventArgs e)
        {
            UpdateCurrentPos();
        }

        #endregion
    }
}