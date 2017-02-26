using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceProcess;
using System.Windows.Forms;

namespace ERPService.SharedLibs.Helpers
{
    internal class StatusObjectHelper
    {
        private PropertyInfo _pInfo;
        private Object _statusObj;

        public StatusObjectHelper(Object statusObj, string propertyName)
        {
            _statusObj = statusObj;
            _pInfo = _statusObj.GetType().GetProperty(propertyName);
        }

        public void SetStatus(string status)
        {
            _pInfo.SetValue(_statusObj, status, null);
        }
    }

    /// <summary>
    /// ���������� ��������� � ���������� ���������
    /// </summary>
    public class ServiceMonitor : IDisposable
    {
        #region ����

        private Nullable<ServiceControllerStatus> _previousStatus;
        private ServiceController _svcCtrl;
        private List<ToolStripItem> _enabledWhenStartedItems;
        private List<ToolStripItem> _enabledWhenStoppedItems;
        private List<StatusObjectHelper> _statusObjects;
        private Timer _timer;
        private bool _mustRaiseServiceStarted;
        private bool _mustRaiseServiceStopped;

        #endregion

        #region �������� �������� � ������

        private void EnableItems(List<ToolStripItem> items, bool enabled)
        {
            foreach (ToolStripItem item in items)
            {
                item.Enabled = enabled;
            }
        }

        private void OnEnabledWhenStartedClick(Object sender, EventArgs args)
        {
            StopService();
        }

        private void OnEnabledWhenStoppedClick(Object sender, EventArgs args)
        {
            StartService();
        }

        private void OnRestartClick(Object sender, EventArgs args)
        {
            RestartService();
        }

        private void DisableAll()
        {
            EnableItems(_enabledWhenStartedItems, false);
            EnableItems(_enabledWhenStoppedItems, false);
        }

        private bool RaiseServiceStarted
        {
            get
            {
                var result = 
                    _mustRaiseServiceStarted ||
                    (_previousStatus != null && _previousStatus.Value != _svcCtrl.Status &&
                    _svcCtrl.Status == ServiceControllerStatus.Running);

                _mustRaiseServiceStarted = false;

                return result;
            }
        }

        private bool RaiseServiceStopped
        {
            get
            {
                var result = 
                    _mustRaiseServiceStopped ||
                    (_previousStatus != null && _previousStatus.Value != _svcCtrl.Status &&
                    _svcCtrl.Status == ServiceControllerStatus.Stopped);

                _mustRaiseServiceStopped = false;

                return result;
            }
        }

        private void OnTick(Object sender, EventArgs args)
        {
            _timer.Stop();
            try
            {
                // ��������� ��������� �������
                _svcCtrl.Refresh();

                // ��������� ������������ ��������� ����������
                string statusString = string.Empty;
                switch (_svcCtrl.Status)
                {
                    case ServiceControllerStatus.Running:
                        statusString = "������ ��������";
                        EnableItems(_enabledWhenStartedItems, true);
                        EnableItems(_enabledWhenStoppedItems, false);
                        if (RaiseServiceStarted)
                        {
                            OnServiceStarted(new EventArgs());
                        }
                        break;
                    case ServiceControllerStatus.Stopped:
                        statusString = "������ ����������";
                        EnableItems(_enabledWhenStartedItems, false);
                        EnableItems(_enabledWhenStoppedItems, true);
                        if (RaiseServiceStopped)
                        {
                            OnServiceStopped(new EventArgs());
                        }
                        break;
                    case ServiceControllerStatus.StartPending:
                        statusString = "������ �����������";
                        EnableItems(_enabledWhenStartedItems, false);
                        EnableItems(_enabledWhenStoppedItems, false);
                        break;
                    case ServiceControllerStatus.StopPending:
                        statusString = "������ ���������������";
                        EnableItems(_enabledWhenStartedItems, false);
                        EnableItems(_enabledWhenStoppedItems, false);
                        break;
                    default:
                        statusString = "��������� ������� �� ����������";
                        EnableItems(_enabledWhenStartedItems, false);
                        EnableItems(_enabledWhenStoppedItems, false);
                        break;
                }

                // ���������� ������� ��������� �������
                _previousStatus = _svcCtrl.Status;

                // ������� ������
                foreach (StatusObjectHelper helper in _statusObjects)
                {
                    helper.SetStatus(statusString);
                }
            }
            finally
            {
                _timer.Start();
            }
        }

        #endregion

        #region ���������� ������

        /// <summary>
        /// ��������� ������� ��� ������� �������
        /// </summary>
        /// <param name="e">��������� �������</param>
        protected virtual void OnServiceStarted(EventArgs e)
        {
            if (ServiceStarted != null)
                ServiceStarted(this, e);
        }

        /// <summary>
        /// ��������� ������� ��� ��������� �������
        /// </summary>
        /// <param name="e">��������� �������</param>
        protected virtual void OnServiceStopped(EventArgs e)
        {
            if (ServiceStopped != null)
                ServiceStopped(this, e);
        }

        #endregion

        #region ������������

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="serviceName">��� �������</param>
        /// <param name="machineName">��� ����������</param>
        /// <param name="updatePeriod">������ ���������� ������� ��������� ����������</param>
        public ServiceMonitor(string serviceName, string machineName, int updatePeriod)
        {
            if (string.IsNullOrEmpty(serviceName))
                throw new ArgumentNullException("serviceName");
            if (string.IsNullOrEmpty(machineName))
                throw new ArgumentNullException("machineName");

            _svcCtrl = new ServiceController(serviceName, machineName);
            _timer = new Timer();
            _timer.Enabled = false;
            _timer.Interval = updatePeriod;
            _timer.Tick += new EventHandler(OnTick);
            _enabledWhenStartedItems = new List<ToolStripItem>();
            _enabledWhenStoppedItems = new List<ToolStripItem>();
            _statusObjects = new List<StatusObjectHelper>();
            _mustRaiseServiceStarted = false;
            _mustRaiseServiceStopped = false;
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="serviceName">��� �������</param>
        /// <param name="updatePeriod">������ ���������� ������� ��������� ����������</param>
        public ServiceMonitor(string serviceName, int updatePeriod)
            : this(serviceName, ".", updatePeriod)
        {
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="serviceName">��� �������</param>
        public ServiceMonitor(string serviceName)
            : this(serviceName, 1000)
        {
        }

        #endregion

        #region ���������� IDisposable

        /// <summary>
        /// ������������ ��������
        /// </summary>
        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }

            if (_svcCtrl != null)
            {
                _svcCtrl.Dispose();
                _svcCtrl = null;
            }
        }

        #endregion

        #region �������� ������

        /// <summary>
        /// ������ �������
        /// </summary>
        public void StartService()
        {
            DisableAll();
            _svcCtrl.Start();
            _mustRaiseServiceStarted = true;
        }

        /// <summary>
        /// ��������� �������
        /// </summary>
        public void StopService()
        {
            DisableAll();
            _svcCtrl.Stop();
            _mustRaiseServiceStopped = true;
        }

        /// <summary>
        /// ���������� �������
        /// </summary>
        public void RestartService()
        {
            if (_svcCtrl.Status == ServiceControllerStatus.Running)
                StopService();
            _svcCtrl.WaitForStatus(ServiceControllerStatus.Stopped);
            StartService();
            _svcCtrl.WaitForStatus(ServiceControllerStatus.Running);
        }

        /// <summary>
        /// ������ ��������
        /// </summary>
        public void StartMonitor()
        {
            OnTick(this, EventArgs.Empty);
            _timer.Start();
        }

        /// <summary>
        /// �������� ������� ������ ������������ � ������ ���������, ��������� � ������, 
        /// ���� ������ ��������
        /// </summary>
        /// <param name="item">������� ������ ������������</param>
        /// <param name="isRestart">������� �������� �� ���������� �������</param>
        public void AppendEnabledWhenStarted(ToolStripItem item, bool isRestart)
        {
            if (item == null)
                throw new ArgumentNullException("ctrl");

            _enabledWhenStartedItems.Add(item);
            if (isRestart)
            {
                item.Click += new EventHandler(OnRestartClick);
                item.Text = "����������";
                item.ToolTipText = "���������� �������";
            }
            else
            {
                item.Click += new EventHandler(OnEnabledWhenStartedClick);
                item.Text = "����";
                item.ToolTipText = "��������� �������";
            }
        }

        /// <summary>
        /// �������� ������� ������ ������������ � ������ ���������, ��������� � ������,
        /// ���� ������ ����������
        /// </summary>
        /// <param name="item">������� ������ ������������</param>
        public void AppendEnabledWhenStopped(ToolStripItem item)
        {
            if (item == null)
                throw new ArgumentNullException("ctrl");

            _enabledWhenStoppedItems.Add(item);
            item.Click += new EventHandler(OnEnabledWhenStoppedClick);
            item.Text = "����";
            item.ToolTipText = "������ �������";
        }

        /// <summary>
        /// �������� ������ � ������ ���������, ������������ ������ ������� � ��������� ����
        /// </summary>
        /// <param name="statusObj">������, ������������ ������</param>
        /// <param name="propertyName">��� ��������, ������� ����� ���������������� �������� �������</param>
        public void AppendStatus(Object statusObj, string propertyName)
        {
            if (statusObj == null)
                throw new ArgumentNullException("statusObj");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");

            _statusObjects.Add(new StatusObjectHelper(statusObj, propertyName));
        }

        /// <summary>
        /// �������� ������ � ������ ���������, ������������ ������ ������� � ��������� ����
        /// </summary>
        /// <param name="statusObj">������, ������������ ������</param>
        public void AppendStatus(Object statusObj)
        {
            AppendStatus(statusObj, "Text");
        }

        /// <summary>
        /// �������� ������������� �������
        /// </summary>
        /// <param name="serviceName">��� �������</param>
        /// <param name="computerName">��� ����������</param>
        public static bool ServiceExists(string serviceName, string computerName)
        {
            foreach (ServiceController svcCtrl in ServiceController.GetServices())
            {
                if (string.Compare(svcCtrl.ServiceName, serviceName, true) == 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// �������� ������������� �������
        /// </summary>
        /// <param name="serviceName">��� �������</param>
        public static bool ServiceExists(string serviceName)
        {
            return ServiceExists(serviceName, ".");
        }

        #endregion

        #region �������

        /// <summary>
        /// ������ �������
        /// </summary>
        public event EventHandler ServiceStarted;

        /// <summary>
        /// ������ ����������
        /// </summary>
        public event EventHandler ServiceStopped;

        #endregion
    }
}
