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