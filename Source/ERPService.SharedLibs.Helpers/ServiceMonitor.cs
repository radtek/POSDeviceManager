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
    /// Мониторинг состояния и управление сервисами
    /// </summary>
    public class ServiceMonitor : IDisposable
    {
        #region Поля

        private Nullable<ServiceControllerStatus> _previousStatus;
        private ServiceController _svcCtrl;
        private List<ToolStripItem> _enabledWhenStartedItems;
        private List<ToolStripItem> _enabledWhenStoppedItems;
        private List<StatusObjectHelper> _statusObjects;
        private Timer _timer;
        private bool _mustRaiseServiceStarted;
        private bool _mustRaiseServiceStopped;

        #endregion

        #region Закрытые свойства и методы

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
                // обновляем состояние сервиса
                _svcCtrl.Refresh();

                // управляем доступностью элементов управления
                string statusString = string.Empty;
                switch (_svcCtrl.Status)
                {
                    case ServiceControllerStatus.Running:
                        statusString = "Сервис работает";
                        EnableItems(_enabledWhenStartedItems, true);
                        EnableItems(_enabledWhenStoppedItems, false);
                        if (RaiseServiceStarted)
                        {
                            OnServiceStarted(new EventArgs());
                        }
                        break;
                    case ServiceControllerStatus.Stopped:
                        statusString = "Сервис остановлен";
                        EnableItems(_enabledWhenStartedItems, false);
                        EnableItems(_enabledWhenStoppedItems, true);
                        if (RaiseServiceStopped)
                        {
                            OnServiceStopped(new EventArgs());
                        }
                        break;
                    case ServiceControllerStatus.StartPending:
                        statusString = "Сервис запускается";
                        EnableItems(_enabledWhenStartedItems, false);
                        EnableItems(_enabledWhenStoppedItems, false);
                        break;
                    case ServiceControllerStatus.StopPending:
                        statusString = "Сервис останавливается";
                        EnableItems(_enabledWhenStartedItems, false);
                        EnableItems(_enabledWhenStoppedItems, false);
                        break;
                    default:
                        statusString = "Состояние сервиса не определено";
                        EnableItems(_enabledWhenStartedItems, false);
                        EnableItems(_enabledWhenStoppedItems, false);
                        break;
                }

                // запоминаем текущее состояние сервиса
                _previousStatus = _svcCtrl.Status;

                // выводим статус
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

        #region Защищенные методы

        /// <summary>
        /// Генерация события при запуске сервиса
        /// </summary>
        /// <param name="e">Аргументы события</param>
        protected virtual void OnServiceStarted(EventArgs e)
        {
            if (ServiceStarted != null)
                ServiceStarted(this, e);
        }

        /// <summary>
        /// Генерация события при остановке сервиса
        /// </summary>
        /// <param name="e">Аргументы события</param>
        protected virtual void OnServiceStopped(EventArgs e)
        {
            if (ServiceStopped != null)
                ServiceStopped(this, e);
        }

        #endregion

        #region Конструкторы

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="serviceName">Имя сервиса</param>
        /// <param name="machineName">Имя компьютера</param>
        /// <param name="updatePeriod">Период обновления статуса элементов управления</param>
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
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="serviceName">Имя сервиса</param>
        /// <param name="updatePeriod">Период обновления статуса элементов управления</param>
        public ServiceMonitor(string serviceName, int updatePeriod)
            : this(serviceName, ".", updatePeriod)
        {
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="serviceName">Имя сервиса</param>
        public ServiceMonitor(string serviceName)
            : this(serviceName, 1000)
        {
        }

        #endregion

        #region Реализация IDisposable

        /// <summary>
        /// Освобождение ресурсов
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

        #region Открытые методы

        /// <summary>
        /// Запуск сервиса
        /// </summary>
        public void StartService()
        {
            DisableAll();
            _svcCtrl.Start();
            _mustRaiseServiceStarted = true;
        }

        /// <summary>
        /// Остановка сервиса
        /// </summary>
        public void StopService()
        {
            DisableAll();
            _svcCtrl.Stop();
            _mustRaiseServiceStopped = true;
        }

        /// <summary>
        /// Перезапуск сервиса
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
        /// Запуск монитора
        /// </summary>
        public void StartMonitor()
        {
            OnTick(this, EventArgs.Empty);
            _timer.Start();
        }

        /// <summary>
        /// Добавить элемент панели инструментов в список элементов, доступных в случае, 
        /// если сервис работает
        /// </summary>
        /// <param name="item">Элемент панели инструментов</param>
        /// <param name="isRestart">Элемент отвечает за перезапуск сервсиа</param>
        public void AppendEnabledWhenStarted(ToolStripItem item, bool isRestart)
        {
            if (item == null)
                throw new ArgumentNullException("ctrl");

            _enabledWhenStartedItems.Add(item);
            if (isRestart)
            {
                item.Click += new EventHandler(OnRestartClick);
                item.Text = "Перезапуск";
                item.ToolTipText = "Перезапуск сервиса";
            }
            else
            {
                item.Click += new EventHandler(OnEnabledWhenStartedClick);
                item.Text = "Стоп";
                item.ToolTipText = "Остановка сервиса";
            }
        }

        /// <summary>
        /// Добавить элемент панели инструментов в список элементов, доступных в случае,
        /// если сервис остановлен
        /// </summary>
        /// <param name="item">Элемент панели инструментов</param>
        public void AppendEnabledWhenStopped(ToolStripItem item)
        {
            if (item == null)
                throw new ArgumentNullException("ctrl");

            _enabledWhenStoppedItems.Add(item);
            item.Click += new EventHandler(OnEnabledWhenStoppedClick);
            item.Text = "Пуск";
            item.ToolTipText = "Запуск сервиса";
        }

        /// <summary>
        /// Добавить объект в список элементов, отображающих статус сервиса в текстовом виде
        /// </summary>
        /// <param name="statusObj">Объект, отображающий статус</param>
        /// <param name="propertyName">Имя свойства, которое будет инициализировано статусом сервиса</param>
        public void AppendStatus(Object statusObj, string propertyName)
        {
            if (statusObj == null)
                throw new ArgumentNullException("statusObj");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");

            _statusObjects.Add(new StatusObjectHelper(statusObj, propertyName));
        }

        /// <summary>
        /// Добавить объект в список элементов, отображающих статус сервиса в текстовом виде
        /// </summary>
        /// <param name="statusObj">Объект, отображающий статус</param>
        public void AppendStatus(Object statusObj)
        {
            AppendStatus(statusObj, "Text");
        }

        /// <summary>
        /// Проверка существования сервиса
        /// </summary>
        /// <param name="serviceName">Имя сервиса</param>
        /// <param name="computerName">Имя компьютера</param>
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
        /// Проверка существования сервиса
        /// </summary>
        /// <param name="serviceName">Имя сервиса</param>
        public static bool ServiceExists(string serviceName)
        {
            return ServiceExists(serviceName, ".");
        }

        #endregion

        #region События

        /// <summary>
        /// Сервис запущен
        /// </summary>
        public event EventHandler ServiceStarted;

        /// <summary>
        /// Сервис остановлен
        /// </summary>
        public event EventHandler ServiceStopped;

        #endregion
    }
}
