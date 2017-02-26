using System;
using System.Windows.Forms;
using DevicesCommon;

namespace DevmanConfig
{
    /// <summary>
    /// Форма для отправки SMS-сообщений
    /// </summary>
    public partial class SMSClientTestForm : Form
    {
        /// <summary>
        /// Идентификатор устройства
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
        /// Конструктор формы
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
                        // отправка сообщения
                        device.Send(tbRecipientNumber.Text, tbMessageText.Text);
                        MessageBox.Show(this, "Сообщение отправлено", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
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