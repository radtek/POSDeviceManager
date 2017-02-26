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
    /// Форма для тестирования считывателей
    /// </summary>
    public partial class ReaderTestForm : Form
    {
        /// <summary>
        /// Идентификатор устройства
        /// </summary>
        public string deviceId;

        private DeviceManagerClient _dmClient;

        private IGenericReader _device;

        /// <summary>
        /// Тест устройства
        /// </summary>
        /// <param name="deviceId">Идентификатор устройства</param>
        internal static void TestReader(string deviceId)
        {
            using (ReaderTestForm testDlg = new ReaderTestForm())
            {
                testDlg.deviceId = deviceId;
                testDlg.ShowDialog();
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public ReaderTestForm()
        {
            InitializeComponent();
        }

        private void ReaderEnabled(bool enabled)
        {
            tbReadData.Enabled = enabled;
            try
            {
                if (enabled)
                {
                    _dmClient = new DeviceManagerClient("localhost");
                    _dmClient.Login();
                    if (_dmClient.Capture(deviceId, 5))
                        _device = (IGenericReader)_dmClient[deviceId];
                    else
                        MessageBox.Show(
                            string.Format("Не удалось получить доступ к устройству \"{0}\"",
                            deviceId), "Тест устройства", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                }
                else
                {
                    if (_dmClient != null && _dmClient.Logged)
                    {
                        if (_device!= null)
                            _dmClient.Release(deviceId);
                        _dmClient = null;
                        _device = null;
                    }
                }
            }
            catch (Exception E)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("Ошибка тестирования устройства \"{0}\".",
                    deviceId));
                sb.AppendLine(string.Format("Тип: {0}.", E.GetType().Name));
                sb.AppendLine(string.Format("Сообщение: {0}.", E.Message));
                sb.AppendLine("Трассировка стека:");
                sb.Append(E.StackTrace);

                MessageBox.Show(sb.ToString(), "Тест устройства", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Close();
            }
        }

        #region Обработка событий

        private void tbReadData_Tick(object sender, EventArgs e)
        {
            try
            {
                if (_dmClient != null && _dmClient.Logged)
                {
                    if (_device != null && !_device.Empty)
                        lbData.Items.Add(_device.Data);
                }
            }
            catch (Exception E)
            {
                tbReadData.Enabled = false;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("Ошибка тестирования устройства \"{0}\".",
                    deviceId));
                sb.AppendLine(string.Format("Тип: {0}.", E.GetType().Name));
                sb.AppendLine(string.Format("Сообщение: {0}.", E.Message));
                sb.AppendLine("Трассировка стека:");
                sb.Append(E.StackTrace);

                MessageBox.Show(sb.ToString(), "Тест устройства", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Close();
            }
        }

        private void ReaderTestForm_Load(object sender, EventArgs e)
        {
            ReaderEnabled(true);
        }

        private void ReaderTestForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ReaderEnabled(false);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using(SaveFileDialog saveDlg = new SaveFileDialog())
            {
                saveDlg.Title = "Сохраненить в файл";
                saveDlg.FileName = deviceId;
                saveDlg.DefaultExt = ".txt";

                string[] lines = new string[lbData.Items.Count];
                lbData.Items.CopyTo(lines, 0);

                if (saveDlg.ShowDialog(this) == DialogResult.OK)
                    System.IO.File.WriteAllLines(saveDlg.FileName, lines);
            }
        }

        #endregion
    }
}