using System;
using System.Collections.Generic;
using ERPService.SharedLibs.Eventlog;
using ERPService.SharedLibs.Helpers;

namespace TsManager
{
    /// <summary>
    /// �������� ����������
    /// </summary>
    public class TsManager
    {
        private TsManagerSettings _settings;
        private List<TsWorker> _workers;
        private IEventLink _eventLink;
        private AMCSLogicLoader _logicLoader;

        #region �������� ������

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
        /// ������� ��������� ������
        /// </summary>
        /// <param name="eventLink">��������� ������� �������</param>
        public TsManager(IEventLink eventLink)
        {
            if (eventLink == null)
                throw new ArgumentNullException("eventLink");

            // ��������� ���������� ������ ������ ����
            _eventLink = eventLink;
            _eventLink.Post(TsGlobalConst.EventSource, "�������� ���������� ������ ������ ����");
            _logicLoader = new AMCSLogicLoader(TsGlobalConst.GetACMSLogicDirectory());

            // �������� ������������
            _eventLink.Post(TsGlobalConst.EventSource, "�������� ������������ ���������");
            _settings = GenericSerializer.Deserialize<TsManagerSettings>(TsGlobalConst.GetSettingsFile(),
                false, _logicLoader.GetLogicSettingsTypes());
            _workers = new List<TsWorker>();

            // ������������� ������������ � ������� �� �������� ������ ��� ������� ���������
            foreach (AMCSLogicSettings logicSettings in _settings.LogicSettings)
            {
                foreach (TsUnitSettings unitSettings in logicSettings.Units)
                {
                    try
                    {
                        // ������� ���������� ������ ������ ���� � �������������� �� ���������
                        IAMCSLogic logic = _logicLoader.CreateLogic(logicSettings.AcmsName);
                        logic.Settings = logicSettings.LogicSettings;

                        // ������� ������� �����
                        TsWorker worker = new TsWorker(logic, unitSettings, _eventLink);
                        _eventLink.Post(TsGlobalConst.EventSource, string.Format(
                            "������ ������� ����� ��� ��������� {0}, ���� \"{1}\"",
                            unitSettings, logicSettings.AcmsName));

                        // ��������� ��� � ������
                        _workers.Add(worker);
                    }
                    catch (Exception e)
                    {
                        string checkPoint = string.Format(
                            "�������� �������� ������ ��� ��������� {0}, ���� \"{1}\"",
                            unitSettings, logicSettings.AcmsName);
                        _eventLink.Post(TsGlobalConst.EventSource, checkPoint, e);
                    }
                }
            }
        }

        /// <summary>
        /// ������ ���������
        /// </summary>
        public void Start()
        {
            EnumWorkers("������ ������� �������", delegate(TsWorker worker)
            {
                worker.Start();
            });
        }

        /// <summary>
        /// ��������� ���������
        /// </summary>
        public void Stop()
        {
            EnumWorkers("��������� ������� �������", delegate(TsWorker worker)
            {
                worker.Stop();
            });
        }
    }
}
