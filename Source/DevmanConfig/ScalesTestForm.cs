using System;
using System.Windows.Forms;
using DevicesCommon;

namespace DevmanConfig
{
    internal partial class ScalesTestForm : Form
    {
        public string deviceId;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        public ScalesTestForm()
        {
            InitializeComponent();
        }

        internal static void TestScales(string deviceId)
        {
            using (ScalesTestForm testDlg = new ScalesTestForm())
            {
                testDlg.deviceId = deviceId;
                testDlg.ShowDialog();
            }
        }

        private void btnGetWeight_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                DeviceTester<IScaleDevice> tester = new DeviceTester<IScaleDevice>(deviceId,
                    delegate(IScaleDevice device)
                    {
                        tbWeight.Text = device.Weight.ToString();
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