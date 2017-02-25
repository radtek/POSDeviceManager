using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceProcess;
using System.Reflection;
using DevicesBase;
using ERPService.SharedLibs.Remoting.Hosts;
using ERPService.SharedLibs.Eventlog;
using ERPService.SharedLibs.Helpers;

namespace DevmanSvc
{
    /// <summary>
    /// ������� ��� ���������� ���������
    /// </summary>
    public class DeviceManagerService : ServiceBase
    {
        #region ���������

        private const String serviceStarted = "������ �������";
        private const String serviceStopped = "������ ����������";
        private const String serviceHello = "��������� POS-���������, ������ {0}";

        #endregion

        #region ����

        private EventLink _eventLink;
        private TcpBinaryHost<DeviceManager> _devmanHost;

        #endregion

        #region �������� ������

        private void InitializeComponent()
        {
            // 
            // DeviceManagerService
            // 
            this.ServiceName = "POSDeviceManager";
        }

        #endregion

        #region �����������

        public DeviceManagerService()
        {
            InitializeComponent();
        }

        #endregion

        #region ���������� ������� �������� ������

        /// <summary>
        /// ������ �������
        /// </summary>
        /// <param name="args">��������� ��������� ������</param>
        protected override void OnStart(String[] args)
        {
            // ������� ������ �������
            _eventLink = new EventLink(DeviceManager.GetDeviceManagerLogDirectory());
            try
            {
                // ������ �������
                _eventLink.Post(DeviceManager.EventSource, String.Format(serviceHello, 
                    VersionInfoHelper.GetVersion(Assembly.GetExecutingAssembly())));

                // ������� � ��������� ���������
                _devmanHost = new TcpBinaryHost<DeviceManager>(new DeviceManager(_eventLink));
                _devmanHost.EventLink = _devmanHost.Target.DebugInfo ? _eventLink : null;
                _devmanHost.Marshal();
                _eventLink.Post(DeviceManager.EventSource, serviceStarted);
            }
            catch (Exception e)
            {
                _eventLink.Post(DeviceManager.EventSource, "����� �������", e);
                // ������������� ������ �������
                Stop();
            }
        }

        /// <summary>
        /// ��������� �������
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                if (_devmanHost != null)
                {
                    // ������������� ���������
                    _devmanHost.Dispose();
                    _devmanHost = null;
                }
                _eventLink.Post(DeviceManager.EventSource, serviceStopped);
            }
            catch (Exception e)
            {
                _eventLink.Post(DeviceManager.EventSource, "��������� �������", e);
            }
            // ��������� ������ �������
            _eventLink.Dispose();
        }

        #endregion
    }
}
