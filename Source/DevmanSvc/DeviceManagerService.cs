using System;
using System.Reflection;
using System.ServiceProcess;
using DevicesBase;
using ERPService.SharedLibs.Eventlog;
using ERPService.SharedLibs.Helpers;
using ERPService.SharedLibs.Remoting.Hosts;

namespace DevmanSvc
{
    /// <summary>
    /// Хостинг для диспетчера устройств
    /// </summary>
    public class DeviceManagerService : ServiceBase, IConsoleAware
    {
        #region Константы

        private const string serviceStarted = "Сервис запущен";
        private const string serviceStopped = "Сервис остановлен";
        private const string serviceHello = "Диспетчер POS-устройств, версия {0}";

        #endregion

        #region Поля

        private EventLink _eventLink;
        private TcpBinaryHost<DeviceManager> _devmanHost;

        #endregion

        #region Закрытые методы

        private void InitializeComponent()
        {
            // 
            // DeviceManagerService
            // 
            this.ServiceName = "POSDeviceManager";
        }

        #endregion

        public DeviceManagerService()
        {
            InitializeComponent();
        }

        #region Перегрузка методов базового класса

        /// <summary>
        /// Запуск сервиса
        /// </summary>
        /// <param name="args">Параметры командной строки</param>
        protected override void OnStart(string[] args)
        {
            // создаем журнал событий
            _eventLink = new EventLink(DeviceManager.GetDeviceManagerLogDirectory());
            try
            {
                // версия сервиса
                _eventLink.Post(DeviceManager.EventSource, string.Format(serviceHello, 
                    VersionInfoHelper.GetVersion(Assembly.GetExecutingAssembly())));

                // создаем и публикуем диспетчер
                _devmanHost = new TcpBinaryHost<DeviceManager>(new DeviceManager(_eventLink));
                _devmanHost.EventLink = _devmanHost.Target.DebugInfo ? _eventLink : null;
                _devmanHost.Marshal();
                _eventLink.Post(DeviceManager.EventSource, serviceStarted);
            }
            catch (Exception e)
            {
                _eventLink.Post(DeviceManager.EventSource, "Старт сервиса", e);
                // останавливаем работу сервиса
                Stop();
            }
        }

        /// <summary>
        /// Остановка сервиса
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                if (_devmanHost != null)
                {
                    // останавливаем диспетчер
                    _devmanHost.Dispose();
                    _devmanHost = null;
                }
                _eventLink.Post(DeviceManager.EventSource, serviceStopped);
            }
            catch (Exception e)
            {
                _eventLink.Post(DeviceManager.EventSource, "Остановка сервиса", e);
            }
            // закрываем журнал событий
            _eventLink.Dispose();
        }

        #endregion

        #region Реализация IConsoleAware

        public void StartApplication(string[] args)
        {
            OnStart(args);
        }

        public void StopApplication()
        {
            OnStop();
        }

        #endregion
    }
}
