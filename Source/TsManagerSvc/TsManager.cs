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
            // ������� ������ �������
            _eventLink = new EventLink(TsGlobalConst.GetLogDirectory());
            try
            {
                _eventLink.Post(TsGlobalConst.EventSource, String.Format(
                    "������ ��������� ����������, ������ {0}",
                    VersionInfoHelper.GetVersion(Assembly.GetExecutingAssembly())));

                // ������� �������� ����������
                _manager = new global::TsManager.TsManager(_eventLink);

                // ������ ���������
                _eventLink.Post(TsGlobalConst.EventSource, "������ ��������� ����������");
                _manager.Start();

                _eventLink.Post(TsGlobalConst.EventSource, "������ �������");
            }
            catch (Exception e)
            {
                _eventLink.Post(TsGlobalConst.EventSource, "����� �������", e);
                Stop();
            }
        }

        protected override void OnStop()
        {
            try
            {
                // ������������� ��������
                _eventLink.Post(TsGlobalConst.EventSource, "��������� ��������� ����������");
                if (_manager != null)
                    _manager.Stop();

                _eventLink.Post(TsGlobalConst.EventSource, "������ ����������");
            }
            catch (Exception e)
            {
                _eventLink.Post(TsGlobalConst.EventSource, "��������� �������", e);
            }
            // ��������� ������ �������
            _eventLink.Dispose();
        }
    }
}
