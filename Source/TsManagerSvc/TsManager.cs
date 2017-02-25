using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Reflection;
using TsManager;
using ERPService.SharedLibs.Eventlog;
using ERPService.SharedLibs.Helpers;

namespace TsManagerSvc
{
    public partial class TsManager : ServiceBase
    {
        private EventLink _eventLink;
        private global::TsManager.TsManager _manager;

        public TsManager()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // создаем журнал событий
            _eventLink = new EventLink(TsGlobalConst.GetLogDirectory());
            try
            {
                _eventLink.Post(TsGlobalConst.EventSource, String.Format(
                    "Сервис менеджера турникетов, версия {0}",
                    VersionInfoHelper.GetVersion(Assembly.GetExecutingAssembly())));

                // создаем менеджер турникетов
                _manager = new global::TsManager.TsManager(_eventLink);

                // запуск менеджера
                _eventLink.Post(TsGlobalConst.EventSource, "Запуск менеджера турникетов");
                _manager.Start();

                _eventLink.Post(TsGlobalConst.EventSource, "Сервис запущен");
            }
            catch (Exception e)
            {
                _eventLink.Post(TsGlobalConst.EventSource, "Старт сервиса", e);
                Stop();
            }
        }

        protected override void OnStop()
        {
            try
            {
                // останавливаем менеджер
                _eventLink.Post(TsGlobalConst.EventSource, "Остановка менеджера турникетов");
                if (_manager != null)
                    _manager.Stop();

                _eventLink.Post(TsGlobalConst.EventSource, "Сервис остановлен");
            }
            catch (Exception e)
            {
                _eventLink.Post(TsGlobalConst.EventSource, "Остановка сервиса", e);
            }
            // закрываем журнал событий
            _eventLink.Dispose();
        }
    }
}
