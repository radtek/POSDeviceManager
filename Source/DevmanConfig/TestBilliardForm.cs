using System;
using System.Windows.Forms;
using DevicesCommon;

namespace DevmanConfig
{
    internal partial class TestBilliardForm : Form
    {
        public string deviceId = string.Empty;

        public TestBilliardForm()
        {
            InitializeComponent();
        }

        public static void TestBilliard(string deviceId)
        {
            using (TestBilliardForm testDlg = new TestBilliardForm())
            {
                testDlg.deviceId = deviceId;
                testDlg.ShowDialog();
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                DeviceTester<IBilliardsManagerDevice> tester = 
                    new DeviceTester<IBilliardsManagerDevice>(deviceId, 
                    delegate(IBilliardsManagerDevice device)
                    {
                        if (rbTurnOn.Checked)
                            device.LightsOn((int)numTable.Value);
                        else
                            device.LightsOff((int)numTable.Value);
                    });
                tester.Execute();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
    }
}