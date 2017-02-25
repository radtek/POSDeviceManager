using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using TsManager;
using ERPService.SharedLibs.Helpers;
using ERPService.SharedLibs.Eventlog;

namespace TsManagerConfigurator
{
    public partial class FormMain : Form
    {
        #region Поля

        private Boolean _modified;
        private TsManagerSettings _settings;
        private AMCSLogicLoader _logicLoader;
        private ServiceMonitor _svcMonitor;
        private TsManagerConfiguratorSettings _appSettings;
        private ListedEventsViewLink _eventsViewLink;

        #endregion

        public FormMain()
        {
            InitializeComponent();
            
            _svcMonitor = new ServiceMonitor("TsManager");
            _svcMonitor.AppendEnabledWhenStarted(tsmiStop, false);
            _svcMonitor.AppendEnabledWhenStarted(tsbStop, false);
            _svcMonitor.AppendEnabledWhenStarted(tsmiRestart, true);
            _svcMonitor.AppendEnabledWhenStopped(tsmiStart);
            _svcMonitor.AppendEnabledWhenStopped(tsbStart);
            _svcMonitor.AppendStatus(tsslServiceStatus);
            _svcMonitor.StartMonitor();

            _logicLoader = new AMCSLogicLoader(TsGlobalConst.GetACMSLogicDirectory());

            LoadGUISettings();
            LoadSettings();
            BuildTreeFromSettings();

            _eventsViewLink = new ListedEventsViewLink(lvLog, _appSettings.FilterSettings,
                _appSettings.LogColumns, true);

            _eventsViewLink.AddCommandItem(tsmiReloadEvents, EventsViewCommand.Update);
            _eventsViewLink.AddCommandItem(toolStripButton1, EventsViewCommand.Update);
            _eventsViewLink.AddCommandItem(tsmiDetails, EventsViewCommand.Details);
            _eventsViewLink.AddCommandItem(tsmiViewFilter, EventsViewCommand.Filter);
            _eventsViewLink.SourceConnector = new LogConnector(tsslUpdateProgress);
            _eventsViewLink.Update();
        }

        #region Работа с конфигурацией

        private void DeleteItem()
        {
            if (MessageBox.Show("Удалить выбранный элемент конфигурации?", Text, 
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                switch (tvSettings.SelectedNode.Level)
                {
                    case 1:
                        _settings.LogicSettings.Remove((AMCSLogicSettings)tvSettings.SelectedNode.Tag);
                        break;
                    case 2:
                        ((AMCSLogicSettings)tvSettings.SelectedNode.Parent.Tag).Units.Remove(
                            (TsUnitSettings)tvSettings.SelectedNode.Tag);
                        break;
                }
                tvSettings.SelectedNode.Remove();
                _modified = true;
                SettingsControlsEnabled();
            }
        }

        private void NewTurnstile()
        {
            TsUnitSettings unitSettings = new TsUnitSettings();
            unitSettings.Name = String.Format("Турникет{0}", tvSettings.SelectedNode.Nodes.Count + 1);

            AMCSLogicSettings logicSettings = (AMCSLogicSettings)tvSettings.SelectedNode.Tag;
            logicSettings.Units.Add(unitSettings);

            TreeNode node = new TreeNode(unitSettings.Name, 2, 2);
            node.Tag = unitSettings;
            tvSettings.SelectedNode.Nodes.Add(node);
            tvSettings.SelectedNode.Expand();

            _modified = true;
            SettingsControlsEnabled();
        }

        private void NewAMCS()
        {
            FormSelectAMСSLogic formSelectAMСSLogic = new FormSelectAMСSLogic();
            if (formSelectAMСSLogic.Execute(_logicLoader.GetLogicNames()))
            {
                AMCSLogicSettings logicSettings = new AMCSLogicSettings();
                logicSettings.LogicSettings = _logicLoader.CreateLogicSettings(formSelectAMСSLogic.ACMSName);
                logicSettings.AcmsName = formSelectAMСSLogic.ACMSName;
                logicSettings.Name = String.Format("СКУД{0}", tvSettings.Nodes[0].Nodes.Count + 1);

                _settings.LogicSettings.Add(logicSettings);
                
                TreeNode node = new TreeNode(logicSettings.Name, 1, 1);
                node.Tag = logicSettings;
                tvSettings.Nodes[0].Nodes.Add(node);
                tvSettings.SelectedNode = node;

                _modified = true;
                SettingsControlsEnabled();
            }
        }

        private void SaveSettings()
        {
            GenericSerializer.Serialize<TsManagerSettings>(_settings,
                TsGlobalConst.GetSettingsFile(), _logicLoader.GetLogicSettingsTypes());
            _modified = false;
            SettingsControlsEnabled();
        }

        private void LoadSettings()
        {
            _settings = GenericSerializer.Deserialize<TsManagerSettings>(
                TsGlobalConst.GetSettingsFile(), false, _logicLoader.GetLogicSettingsTypes());
            _modified = false;
            SettingsControlsEnabled();
        }

        private void SettingsControlsEnabled()
        {
            // сохранить
            tsmiSave.Enabled = _modified;
            tsbSave.Enabled = _modified;
            ctxSave.Enabled = _modified;

            if (tvSettings.SelectedNode != null)
            {
                tsmiNewACMS.Enabled = tvSettings.SelectedNode.Level == 0;
                ctxNewACMS.Enabled = tsmiNewACMS.Enabled;

                tsmiNewTurnstile.Enabled = tvSettings.SelectedNode.Level == 1;
                ctxNewTurnstile.Enabled = tsmiNewTurnstile.Enabled;

                tsmiDelete.Enabled = tvSettings.SelectedNode.Level != 0;
                ctxDelete.Enabled = tsmiDelete.Enabled;
            }
            else
            {
                tsmiNewACMS.Enabled = false;
                ctxNewACMS.Enabled = false;

                tsmiNewTurnstile.Enabled = false;
                ctxNewTurnstile.Enabled = false;

                tsmiDelete.Enabled = false;
                ctxDelete.Enabled = false;
            }
        }

        private void BuildUnitFromUnitSettings(TreeNode root, TsUnitSettings unitSettings)
        {
            TreeNode node = new TreeNode(unitSettings.Name, 2, 2);
            node.Tag = unitSettings;
            root.Nodes.Add(node);
        }

        private void BuildACMSFromLogicSettings(TreeNode root, AMCSLogicSettings logicSettings)
        {
            TreeNode node = new TreeNode(logicSettings.Name, 1, 1);
            node.Tag = logicSettings;
            foreach (TsUnitSettings unitSettings in logicSettings.Units)
            {
                BuildUnitFromUnitSettings(node, unitSettings);
            }
            root.Nodes.Add(node);
        }

        private void BuildTreeFromSettings()
        {
            tvSettings.BeginUpdate();
            try
            {
                tvSettings.Nodes.Clear();
                tvSettings.Nodes.Add(new TreeNode("Менеджер турникетов", 0, 0));
                foreach (AMCSLogicSettings logicSettings in _settings.LogicSettings)
                {
                    BuildACMSFromLogicSettings(tvSettings.Nodes[0], logicSettings);
                }
                tvSettings.Select();
                tvSettings.SelectedNode = tvSettings.Nodes[0];
                tvSettings.Nodes[0].Expand();
            }
            finally
            {
                tvSettings.EndUpdate();
            }
        }

        #endregion

        #region Прочие методы

        private void OnHome()
        {
            Process.Start("http://www.erpservice.ru");
        }

        private void OnUpdates()
        {
            Process.Start("http://www.erpservice.ru/svn/Release/%D0%A1%D0%B5%D1%80%D0%B2%D0%B8%D1%81%D0%BD%D0%BE%D0%B5%D0%9F%D0%9E/%D0%94%D0%B8%D1%81%D0%BF%D0%B5%D1%82%D1%87%D0%B5%D1%80%D0%A3%D1%81%D1%82%D1%80%D0%BE%D0%B9%D1%81%D1%82%D0%B2/trunk/files/");
        }

        private void OnAbout()
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.CopyrightYear = 2008;
            aboutBox.ProductName = "Форинт-С: Диспетчер устройств";
            aboutBox.AppName = "Менеждер турникетов";
            aboutBox.Version = VersionInfoHelper.GetVersion(Application.ExecutablePath); ;
            aboutBox.Show(false);
        }

        private void LoadGUISettings()
        {
            _appSettings = GenericSerializer.Deserialize<TsManagerConfiguratorSettings>(
                TsGlobalConst.GetAppSettingsFile());
            Left = _appSettings.Left;
            Top = _appSettings.Top;
            Width = _appSettings.Width;
            Height = _appSettings.Height;
            splitContainer3.SplitterDistance = _appSettings.Splitter1;
            splitContainer2.SplitterDistance = _appSettings.Splitter2;
        }

        private void SaveGUISettings()
        {
            if (WindowState != FormWindowState.Minimized)
            {
                _appSettings.Left = Left;
                _appSettings.Top = Top;
                _appSettings.Width = Width;
                _appSettings.Height = Height;
            }
            _appSettings.Splitter1 = splitContainer3.SplitterDistance;
            _appSettings.Splitter2 = splitContainer2.SplitterDistance;

            GenericSerializer.Serialize<TsManagerConfiguratorSettings>(_appSettings,
                TsGlobalConst.GetAppSettingsFile());
        }

        #endregion

        private void tvSettings_AfterSelect(object sender, TreeViewEventArgs e)
        {
            propertyGrid1.SelectedObject = 
                tvSettings.SelectedNode == null || tvSettings.SelectedNode.Tag == null ?
                null : tvSettings.SelectedNode.Tag;
            SettingsControlsEnabled();
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_modified)
            {
                DialogResult dr = MessageBox.Show("Конфигурация изменена. Сохранить?", Text, 
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, 
                    MessageBoxDefaultButton.Button1);

                switch (dr)
                {
                    case DialogResult.Yes:
                        SaveSettings();
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }

            if (!e.Cancel)
                SaveGUISettings();
        }

        private void tsmiNewACMS_Click(object sender, EventArgs e)
        {
            NewAMCS();
        }

        private void tsmiSave_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void tsmiNewTurnstile_Click(object sender, EventArgs e)
        {
            NewTurnstile();
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            switch (tvSettings.SelectedNode.Level)
            {
                case 1:
                    tvSettings.SelectedNode.Text = ((AMCSLogicSettings)tvSettings.SelectedNode.Tag).Name;
                    break;
                case 2:
                    tvSettings.SelectedNode.Text = ((TsUnitSettings)tvSettings.SelectedNode.Tag).Name;
                    break;
            }
            _modified = true;
            SettingsControlsEnabled();
        }

        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            DeleteItem();
        }

        private void tsmiHome_Click(object sender, EventArgs e)
        {
            OnHome();
        }

        private void tsmiAbout_Click(object sender, EventArgs e)
        {
            OnAbout();
        }

        private void tsmiUpdates_Click(object sender, EventArgs e)
        {
            OnUpdates();
        }

        private void lvLog_SelectedIndexChanged(object sender, EventArgs e)
        {
            tsmiDetails.Enabled = lvLog.SelectedIndices.Count > 0;
        }

        private void tsmiDetails_Click(object sender, EventArgs e)
        {
            if (lvLog.SelectedItems.Count > 0)
                EventsView.Show(_eventsViewLink);
        }
    }
}