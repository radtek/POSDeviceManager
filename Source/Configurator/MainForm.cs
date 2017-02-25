using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ERPService.SharedLibs.Eventlog;
using ERPService.SharedLibs.Helpers;
using DevicesBase;

namespace DevmanConfig
{
    public partial class MainForm : Form
    {
        #region Наименования видов устройств

        private const string PRINTABLE_DEVICES = "Печатающие устройства";
        private const string DISPLAY_DEVICES = "Дисплеи покупателя";
        private const string SCALES_DEVICES = "Электронные весы";
        private const string BILLIARD_DEVICES = "Модули управления бильярдом";
        private const string SMS_CLIENT_DEVICES = "SMS-клиенты";
        private const string READER_DEVICES = "Считыватели";
        private const string TURNSTILE_DEVICES = "Турникеты";

        #endregion

        #region Скрытые поля

        private DevmanProperties devmanProperties;
        private ServiceMonitor _srvMonitor;
        private ListedEventsViewLink _eventsViewLink;
        private ConfiguratorFormSettings _formSettings;

        #endregion

        #region Конструктор

        public MainForm()
        {
            InitializeComponent();

        }

        #endregion

        #region Скрытые методы

        /// <summary>
        /// Загрузка конфигурации диспетчера устройств
        /// </summary>
        private void LoadConfig()
        {
            devmanProperties = DevmanProperties.Load(DeviceManager.GetDeviceManagerSettingsFile());
            UpdateTreeView();
            tvDevices.SelectedNode = tvDevices.TopNode;
            miSaveConfig.Enabled = btnSave.Enabled = false;
            lbConfigVersion.Text = String.Format("Версия конфигурации: {0}", devmanProperties.Version);

            if (System.IO.File.Exists(DeviceManager.GetDeviceManagerDirectory() + "\\DevmanSvc.exe"))
                lbSvcVersion.Text = "Версия службы: " + ERPService.SharedLibs.Helpers.VersionInfoHelper.GetVersion(DeviceManager.GetDeviceManagerDirectory() + "\\DevmanSvc.exe");
            else
                lbSvcVersion.Text = "Версия службы: не определена";
        }

        /// <summary>
        /// Сохранение конфигурации диспетчера устройств
        /// </summary>
        private void SaveConfig()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                devmanProperties.Save(DeviceManager.GetDeviceManagerSettingsFile());
                miSaveConfig.Enabled = btnSave.Enabled = false;
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show(String.Format("Ошибка сохранения конфигурации. {0}", e.InnerException.Message),
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (System.IO.IOException e)
            {
                MessageBox.Show(String.Format("Ошибка ввода-вывода. {0}", e.Message),
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Сохранение параметров формы
        /// </summary>
        private void SaveFormSettings()
        {
            if (WindowState != FormWindowState.Minimized)
            {
                _formSettings.Location = Location;
                _formSettings.Size = Size;
            }
            _formSettings.PropertiesHeight = propertyGrid.Height;
            _formSettings.ConfigWidth = pnlConfig.Width;
            _formSettings.DetailedViewHeight = splitContainer1.SplitterDistance;

            GenericSerializer.Serialize<ConfiguratorFormSettings>(_formSettings,
                DeviceManager.GetDeviceManagerDirectory() + "\\Configurator.xml");
        }

        /// <summary>
        /// Загрузка параметров формы
        /// </summary>
        private void LoadFormSettings()
        {
            _formSettings = GenericSerializer.Deserialize<ConfiguratorFormSettings>(
                DeviceManager.GetDeviceManagerDirectory() + "\\Configurator.xml");

            Location = _formSettings.Location;
            Size = _formSettings.Size;
            propertyGrid.Height = _formSettings.PropertiesHeight;
            pnlConfig.Width = _formSettings.ConfigWidth;
            splitContainer1.SplitterDistance = _formSettings.DetailedViewHeight;

            _eventsViewLink = new ListedEventsViewLink(lvLog, _formSettings.Filter,
                _formSettings.Columns, true);

            _eventsViewLink.AddCommandItem(miRefresh, EventsViewCommand.Update);
            _eventsViewLink.AddCommandItem(btnRefresh, EventsViewCommand.Update);
            _eventsViewLink.AddCommandItem(cmiRefresh, EventsViewCommand.Update);
            _eventsViewLink.AddCommandItem(miDetails, EventsViewCommand.Details);
            _eventsViewLink.AddCommandItem(miFilter, EventsViewCommand.Filter);
            _eventsViewLink.AddCommandItem(btnFilter, EventsViewCommand.Filter);
            _eventsViewLink.AddCommandItem(cmiFilter, EventsViewCommand.Filter);
            _eventsViewLink.AddDetailedViewControl(textBox1);
            _eventsViewLink.SourceConnector = new LogConnector(tsslEventsReloadProgress);
            _eventsViewLink.Update();

            _srvMonitor = new ServiceMonitor("POSDeviceManager", 500);
            _srvMonitor.AppendStatus(lbSvcStatus, "Text");
            _srvMonitor.AppendEnabledWhenStarted(btnSvcStop, false);
            _srvMonitor.AppendEnabledWhenStarted(miSvcStop, false);
            _srvMonitor.AppendEnabledWhenStarted(miSvcRestart, true);
            _srvMonitor.AppendEnabledWhenStopped(btnSvcStart);
            _srvMonitor.AppendEnabledWhenStopped(miSvcStart);
            _srvMonitor.StartMonitor();
        }

        /// <summary>
        /// Генерация нового наименования элемента конфигурации на основе шаблона
        /// </summary>
        /// <param name="parentNode">Родительский элемент</param>
        /// <param name="itemTemplate">Шаблон наименования</param>
        /// <returns></returns>
        private string GetNewItemName(TreeNode parentNode, string itemTemplate)
        {
            string itemName = string.Empty;
            int itemIndex = 1;
            do
                itemName = String.Format("{0} {1}", itemTemplate, itemIndex++);
            while (parentNode.Nodes.ContainsKey(itemName));
            return itemName;
        }

        /// <summary>
        /// Добавление нового элемента в дерево конфигурации
        /// </summary>
        /// <param name="nodeText">Наименование элемента</param>
        /// <param name="customData">Связанный объект</param>
        /// <param name="parentNode">Родительский элемент</param>
        /// <param name="imageIndex">Номер изображения</param>
        /// <returns></returns>
        private TreeNode AddNewNode(string nodeText, object customData, TreeNode parentNode, int imageIndex)
        {
            TreeNode newNode = new TreeNode(nodeText, imageIndex, imageIndex);
            newNode.Tag = customData;
            newNode.Name = nodeText;
            if (parentNode == null)
                tvDevices.Nodes.Add(newNode);
            else
                parentNode.Nodes.Add(newNode);

            return newNode;
        }

        /// <summary>
        /// Обновление дерева при загрузке конфигурации
        /// </summary>
        private void UpdateTreeView()
        {
            tvDevices.BeginUpdate();
            try
            {
                tvDevices.Nodes.Clear();
                // корневой узел
                AddNewNode("Диспетчер устройств", devmanProperties, null, 0);

                // Печатающие устройства
                TreeNode parentNode = AddNewNode(PRINTABLE_DEVICES, devmanProperties.PrintableDevices, tvDevices.TopNode, 2);
                foreach (PrintableDeviceProperties deviceConfig in devmanProperties.PrintableDevices)
                    AddNewNode(deviceConfig.DeviceId, deviceConfig, parentNode, 1);

                // Дисплеи покупателя
                parentNode = AddNewNode(DISPLAY_DEVICES, devmanProperties.DisplayDevices, tvDevices.TopNode, 2);
                foreach (DisplayDeviceProperties deviceConfig in devmanProperties.DisplayDevices)
                    AddNewNode(deviceConfig.DeviceId, deviceConfig, parentNode, 1);

                // Электронные весы
                parentNode = AddNewNode(SCALES_DEVICES, devmanProperties.ScalesDevices, tvDevices.TopNode, 2);
                foreach (ScalesDeviceProperties deviceConfig in devmanProperties.ScalesDevices)
                    AddNewNode(deviceConfig.DeviceId, deviceConfig, parentNode, 1);

                // Модули управления биллиардом
                parentNode = AddNewNode(BILLIARD_DEVICES, devmanProperties.BillardDevices, tvDevices.TopNode, 2);
                foreach (BilliardManagerProperties deviceConfig in devmanProperties.BillardDevices)
                    AddNewNode(deviceConfig.DeviceId, deviceConfig, parentNode, 1);

                // SMS-клиенты
                parentNode = AddNewNode(SMS_CLIENT_DEVICES, devmanProperties.SMSClientDevices, tvDevices.TopNode, 2);
                foreach (SMSClientProperties deviceConfig in devmanProperties.SMSClientDevices)
                    AddNewNode(deviceConfig.DeviceId, deviceConfig, parentNode, 1);

                // Считыватели
                parentNode = AddNewNode(READER_DEVICES, devmanProperties.ReaderDevices, tvDevices.TopNode, 2);
                foreach (ReaderDeviceProperties deviceConfig in devmanProperties.ReaderDevices)
                    AddNewNode(deviceConfig.DeviceId, deviceConfig, parentNode, 1);

                // Турникеты
                parentNode = AddNewNode(TURNSTILE_DEVICES, devmanProperties.TurnstileDevices, tvDevices.TopNode, 2);
                foreach (TurnstileProperties deviceConfig in devmanProperties.TurnstileDevices)
                    AddNewNode(deviceConfig.DeviceId, deviceConfig, parentNode, 1);

                tvDevices.ExpandAll();
            }
            finally
            {
                tvDevices.EndUpdate();
                tvDevices.SelectedNode = tvDevices.TopNode;
            }
        }

        #endregion

        #region Обработка событий

        private void tvDevices_AfterSelect(object sender, TreeViewEventArgs e)
        {
            propertyGrid.SelectedObject = e.Node.Level == 2 ? e.Node.Tag : tvDevices.TopNode.Tag;
            cmiTest.Enabled = miTest.Enabled = btnTest.Enabled = e.Node.Level == 2;
            cmiRemove.Enabled = miRemove.Enabled = btnRemove.Enabled = e.Node.Level == 2;
            cmiAddDevice.Enabled = miAddDevice.Enabled = btnAddDevice.Enabled = e.Node.Level > 0;
        }

        private void OnRemove(object sender, EventArgs e)
        {
            if (!tvDevices.Focused || tvDevices.SelectedNode.Level != 2)
                return;

            if (MessageBox.Show(this,
                String.Format("Удалить устройство \"{0}\" из конфигурации?", tvDevices.SelectedNode.Text),
                Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                return;

            switch(tvDevices.SelectedNode.Parent.Name)
            {
                case PRINTABLE_DEVICES:
                    devmanProperties.PrintableDevices.Remove((PrintableDeviceProperties)tvDevices.SelectedNode.Tag);
                    break;
                case DISPLAY_DEVICES:
                    devmanProperties.DisplayDevices.Remove((DisplayDeviceProperties)tvDevices.SelectedNode.Tag);
                    break;
                case SCALES_DEVICES:
                    devmanProperties.ScalesDevices.Remove((ScalesDeviceProperties)tvDevices.SelectedNode.Tag);
                    break;
                case BILLIARD_DEVICES:
                    devmanProperties.BillardDevices.Remove((BilliardManagerProperties)tvDevices.SelectedNode.Tag);
                    break;
                case SMS_CLIENT_DEVICES:
                    devmanProperties.SMSClientDevices.Remove((SMSClientProperties)tvDevices.SelectedNode.Tag);
                    break;
                case READER_DEVICES:
                    devmanProperties.ReaderDevices.Remove((ReaderDeviceProperties)tvDevices.SelectedNode.Tag);
                    break;
                case TURNSTILE_DEVICES:
                    devmanProperties.TurnstileDevices.Remove((TurnstileProperties)tvDevices.SelectedNode.Tag);
                    break;
            }

            tvDevices.BeginUpdate();
            try
            {
                tvDevices.SelectedNode.Remove();
                tvDevices.SelectedNode = tvDevices.TopNode;
            }
            finally
            {
                tvDevices.EndUpdate();
            }
            miSaveConfig.Enabled = btnSave.Enabled = true;
        }

        private void OnAddDevice(object sender, EventArgs e)
        {
            if (tvDevices.SelectedNode.Level < 1)
                return;

            TreeNode parentNode = tvDevices.SelectedNode.Level == 1 ? 
                tvDevices.SelectedNode: 
                tvDevices.SelectedNode.Parent;

            string deviceId = GetNewItemName(parentNode, "Устройство");
            object deviceProps = null;
            switch (parentNode.Name)
            {
                case PRINTABLE_DEVICES:
                    deviceProps = new PrintableDeviceProperties(deviceId);
                    devmanProperties.PrintableDevices.Add((PrintableDeviceProperties)deviceProps);
                    break;
                case DISPLAY_DEVICES:
                    deviceProps = new DisplayDeviceProperties(deviceId);
                    devmanProperties.DisplayDevices.Add((DisplayDeviceProperties)deviceProps);
                    break;
                case SCALES_DEVICES:
                    deviceProps = new ScalesDeviceProperties(deviceId);
                    devmanProperties.ScalesDevices.Add((ScalesDeviceProperties)deviceProps);
                    break;
                case BILLIARD_DEVICES:
                    deviceProps = new BilliardManagerProperties(deviceId);
                    devmanProperties.BillardDevices.Add((BilliardManagerProperties)deviceProps);
                    break;
                case SMS_CLIENT_DEVICES:
                    deviceProps = new SMSClientProperties(deviceId);
                    devmanProperties.SMSClientDevices.Add((SMSClientProperties)deviceProps);
                    break;
                case READER_DEVICES:
                    deviceProps = new ReaderDeviceProperties(deviceId);
                    devmanProperties.ReaderDevices.Add((ReaderDeviceProperties)deviceProps);
                    break;
                case TURNSTILE_DEVICES:
                    deviceProps = new TurnstileProperties(deviceId);
                    devmanProperties.TurnstileDevices.Add((TurnstileProperties)deviceProps);
                    break;
            }
            tvDevices.SelectedNode = AddNewNode(deviceId, deviceProps, parentNode, 1);
            miSaveConfig.Enabled = btnSave.Enabled = true;
        }

        private void OnSaveConfig(object sender, EventArgs e)
        {
            SaveConfig();
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            switch (tvDevices.SelectedNode.Level)
            {
                case 2:
                    tvDevices.SelectedNode.Text = ((BaseDeviceProperties)tvDevices.SelectedNode.Tag).DeviceId;
                    break;
            }
            btnSave.Enabled = true;
        }

        private void OnTest(object sender, EventArgs e)
        {
            if (btnSave.Enabled)
            {
                switch (MessageBox.Show(this,
                    "Конфигурация диспетчера POS-устройств изменена. Сохранить изменения?",
                    Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Cancel:
                        return;
                    case DialogResult.Yes:
                        SaveConfig();
                        break;
                }
            }
            if (tvDevices.SelectedNode.Level != 2)
                return;
            ((BaseDeviceProperties)tvDevices.SelectedNode.Tag).TestDevice();
        }

        private void OnAbout(object sender, EventArgs e)
        {
            ERPService.SharedLibs.Helpers.AboutBox aboutBox = new ERPService.SharedLibs.Helpers.AboutBox();
            aboutBox.AppName = "Конфигуратор";
            aboutBox.ProductName = "Форинт-С: Диспетчер устройств";

            aboutBox.Version = ERPService.SharedLibs.Helpers.VersionInfoHelper.GetVersion(DeviceManager.GetDeviceManagerDirectory() + "\\DevmanSvc.exe");
            aboutBox.CopyrightYear = 2009;
            aboutBox.AppendComponentInfo("DevicesCommon.dll", ERPService.SharedLibs.Helpers.VersionInfoHelper.GetVersion(DeviceManager.GetDeviceManagerDirectory() + "\\DevicesCommon.dll"));
            aboutBox.AppendComponentInfo("DevicesBase.dll", ERPService.SharedLibs.Helpers.VersionInfoHelper.GetVersion(DeviceManager.GetDeviceManagerDirectory() + "\\DevicesBase.dll"));
            aboutBox.AppendComponentInfo("DevmanConfig.dll", ERPService.SharedLibs.Helpers.VersionInfoHelper.GetVersion(DeviceManager.GetDeviceManagerDirectory() + "\\DevmanConfig.dll"));
            aboutBox.AppendComponentInfo("Configurator.exe", ERPService.SharedLibs.Helpers.VersionInfoHelper.GetVersion(DeviceManager.GetDeviceManagerDirectory() + "\\Configurator.exe"));

            aboutBox.Show(true);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnSave.Enabled)
            {
                switch (MessageBox.Show(this,
                    "Конфигурация диспетчера POS-устройств изменена. Сохранить изменения?",
                    Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                    case DialogResult.Yes:
                        SaveConfig();
                        break;
                }
            }
        }

        private void miClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveFormSettings();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadFormSettings();
            LoadConfig();
        }

        #endregion


    }
}