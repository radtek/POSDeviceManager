using System;
using System.Collections.Generic;
using ERPService.SharedLibs.Eventlog;
using ERPService.SharedLibs.Helpers;

namespace TsManager
{
    /// <summary>
    /// Менеджер турникетов
    /// </summary>
    public class TsManager
    {
        private TsManagerSettings _settings;
        private List<TsWorker> _workers;
        private IEventLink _eventLink;
        private AMCSLogicLoader _logicLoader;

        #region Закрытые методы

        private delegate void EnumWorkerDelegate(TsWorker worker);

        private void EnumWorkers(string checkPointId, EnumWorkerDelegate workerDelegate)
        {
            foreach (TsWorker worker in _workers)
            {
                try
                {
                    workerDelegate(worker);
                }
                catch (Exception e)
                {
                    _eventLink.Post(TsGlobalConst.EventSource, checkPointId, e);
                }
            }
        }

        #endregion

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="eventLink">Интерфейс журнала событий</param>
        public TsManager(IEventLink eventLink)
        {
            if (eventLink == null)
                throw new ArgumentNullException("eventLink");

            // загружаем реализации логики работы СКУД
            _eventLink = eventLink;
            _eventLink.Post(TsGlobalConst.EventSource, "Загрузка реализаций логики работы СКУД");
            _logicLoader = new AMCSLogicLoader(TsGlobalConst.GetACMSLogicDirectory());

            // загрузка конфигурации
            _eventLink.Post(TsGlobalConst.EventSource, "Загрузка конфигурации менеджера");
            _settings = GenericSerializer.Deserialize<TsManagerSettings>(TsGlobalConst.GetSettingsFile(),
                false, _logicLoader.GetLogicSettingsTypes());
            _workers = new List<TsWorker>();

            // просматриваем конфигурацию и создаем по рабочему потоку для каждого турникета
            foreach (AMCSLogicSettings logicSettings in _settings.LogicSettings)
            {
                foreach (TsUnitSettings unitSettings in logicSettings.Units)
                {
                    try
                    {
                        // создаем реализацию логики работы СКУД и инициализируем ее параметры
                        IAMCSLogic logic = _logicLoader.CreateLogic(logicSettings.AcmsName);
                        logic.Settings = logicSettings.LogicSettings;

                        // создаем рабочий поток
                        TsWorker worker = new TsWorker(logic, unitSettings, _eventLink);
                        _eventLink.Post(TsGlobalConst.EventSource, string.Format(
                            "Создан рабочий поток для турникета {0}, СКУД \"{1}\"",
                            unitSettings, logicSettings.AcmsName));

                        // добавляем его в список
                        _workers.Add(worker);
                    }
                    catch (Exception e)
                    {
                        string checkPoint = string.Format(
                            "Создание рабочего потока для турникета {0}, СКУД \"{1}\"",
                            unitSettings, logicSettings.AcmsName);
                        _eventLink.Post(TsGlobalConst.EventSource, checkPoint, e);
                    }
                }
            }
        }

        /// <summary>
        /// Запуск менеджера
        /// </summary>
        public void Start()
        {
            EnumWorkers("Запуск рабочих потоков", delegate(TsWorker worker)
            {
                worker.Start();
            });
        }

        /// <summary>
        /// Остановка менеджера
        /// </summary>
        public void Stop()
        {
            EnumWorkers("Остановка рабочих потоков", delegate(TsWorker worker)
            {
                worker.Stop();
            });
        }
    }
}
