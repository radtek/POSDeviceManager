using System;
using System.Windows.Forms;
using DevicesCommon;

namespace DevmanConfig
{
    /// <summary>
    /// ����� ��� �������� SMS-���������
    /// </summary>
    public partial class SMSClientTestForm : Form
    {
        /// <summary>
        /// ������������� ����������
        /// </summary>
        public string deviceId;

        internal static void TestSMSClient(string deviceId)
        {
            using (SMSClientTestForm testDlg = new SMSClientTestForm())
            {
                testDlg.deviceId = deviceId;
                testDlg.ShowDialog();
            }
        }

        /// <summary>
        /// ����������� �����
        /// </summary>
        public SMSClientTestForm()
        {
            InitializeComponent();
        }

        private void OnSend(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                DeviceTester<ISMSClient> tester = new DeviceTester<ISMSClient>(deviceId,
                    delegate(ISMSClient device)
                    {
                        // �������� ���������
                        device.Send(tbRecipientNumber.Text, tbMessageText.Text);
                        MessageBox.Show(this, "��������� ����������", "����������", MessageBoxButtons.OK, MessageBoxIcon.Information);
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