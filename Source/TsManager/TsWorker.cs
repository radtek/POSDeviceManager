using System;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Threading;
using DevicesCommon;
using DevicesCommon.Connectors;
using DevicesCommon.Helpers;
using ERPService.SharedLibs.Eventlog;

namespace TsManager
{
    /// <summary>
    /// ������ � ����������
    /// </summary>
    internal class TsWorker
    {
        #region ����

        private IAMCSLogic _amcsLogic;
        private TsUnitSettings _unitSettings;
        private Thread _workingThread;
        private ManualResetEvent _terminated;
        private IEventLink _eventLink;
        private DeviceManagerClient _client;
        private ITurnstileDevice _device;

        #endregion

        #region �������� ������

        /// <summary>
        /// ������ ����������
        /// </summary>
        private void CaptureDevice()
        {
            if (_client != null && _device != null)
                // ���������� ��� ���������
                return;

            // ������������ � ���������� ���������
            _client = new DeviceManagerClient(_unitSettings.HostOrIp, _unitSettings.Port);
            _client.Login();
            // ����������� ��������
            _client.Capture(_unitSettings.DeviceId, Timeout.Infinite);
            _device = (ITurnstileDevice)_client[_unitSettings.DeviceId];
            // ����������� �����������
            _eventLink.Post(TsGlobalConst.EventSource, string.Format(
                "[0] ����������� � ���������� ��������� �����������", _unitSettings));
        }

        /// <summary>
        /// ����������� ����������
        /// </summary>
        private void ReleaseDevice()
        {
            if (_client == null)
                // ���������� ��������
                return;

            try
            {
                // ����������� ����������
                _client.Release(_unitSettings.DeviceId);
                // ��������� ���������� ������
                _client.Dispose();
            }
            catch (LoginToDeviceManagerException)
            {
            }
            catch (DeviceManagerException)
            {
            }
            catch (DeviceNoFoundException)
            {
            }
            catch (SocketException)
            {
            }
            catch (RemotingException)
            {
            }
            finally
            {
                _client = null;
                _device = null;

                // ����������� �������
                _eventLink.Post(TsGlobalConst.EventSource, string.Format(
                    "[0] ����������� � ���������� ��������� �������", _unitSettings));
            }
        }

        /// <summary>
        /// ����� ��� ������ � �����������
        /// </summary>
        private void WorkWithTurnstile()
        {
            // ������ ������
            _eventLink.Post(TsGlobalConst.EventSource,
                string.Format("[{0}] ������ �������� ������", _unitSettings));

            // ���� ������ ���������� ������������� ���������
            // ���������� �� ��� ���, ���� ������� �� �������� � ���������� ���������
            while (!_terminated.WaitOne(0, false))
            {
                try
                {
                    // ������ ����������
                    CaptureDevice();

                    // ����������� ����������������� ������ � ����������
                    string idData = _device.IdentificationData;
                    if (string.IsNullOrEmpty(idData))
                        continue;

                    string direction = _device.Direction == TurnstileDirection.Entry ?
                        "����" : "�����";

                    // ��������� ����������� ������� � �������� �����������
                    _eventLink.Post(TsGlobalConst.EventSource,
                        string.Format("[{0}] �������� ����������������� ������ [{1}]", 
                        _unitSettings, idData));

                    string reason;
                    if (_amcsLogic.IsAccessGranted(_device.Direction, idData, out reason))
                    {
                        // ������ ��������
                        _eventLink.Post(TsGlobalConst.EventSource, string.Format(
                            "[{0}] ������ ��������, ����������� [{1}], ����������������� ������ [{2}]",
                            _unitSettings, direction, idData));
                        // ��������� ��������
                        bool passOk = _device.Open();
                        if (passOk)
                        {
                            // ���������� ������
                            _eventLink.Post(TsGlobalConst.EventSource, string.Format(
                                "[{0}] �������� ������, ������ ��������", _unitSettings));
                            // ���������� �� ���� ����
                            _amcsLogic.OnAccessOccured(_device.Direction, idData);
                        }
                        else
                            _eventLink.Post(TsGlobalConst.EventSource, string.Format(
                                "[{0}] �������� ������, ������� ����� ��������", _unitSettings));
                    }
                    else
                    {
                        // ������ ��������
                        _eventLink.Post(TsGlobalConst.EventSource, string.Format(
                            "[{0}] ������ ��������, ����������� [{1}], ����������������� ������ [{2}]. �������: [{3}]",
                            _unitSettings, direction, idData, reason));
                        // ��������� �������� � ������������� ����������
                        _device.Close(true);
                    }
                }
                catch (Exception e)
                {
                    // ������������� ���������� �� ����������
                    _eventLink.Post(TsGlobalConst.EventSource, string.Format(
                        "[{0}] ���������� � ������� ������", _unitSettings), e);
                    
                    // ����������� ����������
                    ReleaseDevice();
                }
            }

            // ������������ ����������
            ReleaseDevice();
            // ��������� ������
            _eventLink.Post(TsGlobalConst.EventSource,
                string.Format("[{0}] ��������� �������� ������", _unitSettings));
        }

        #endregion

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="amcsLogic">���������� ������ ������ ����</param>
        /// <param name="unitSettings">��������� ���������</param>
        /// <param name="eventLink">��������� ������� �������</param>
        public TsWorker(IAMCSLogic amcsLogic, TsUnitSettings unitSettings, IEventLink eventLink)
        {
            if (amcsLogic == null)
                throw new ArgumentNullException("amcsLogic");
            if (unitSettings == null)
                throw new ArgumentNullException("unitSettings");
            if (eventLink == null)
                throw new ArgumentNullException("eventLink");

            _amcsLogic = amcsLogic;
            _unitSettings = unitSettings;
            _eventLink = eventLink;
            _terminated = new ManualResetEvent(false);
            _workingThread = new Thread(WorkWithTurnstile);
        }

        /// <summary>
        /// ������ ������ � ���������
        /// </summary>
        public void Start()
        {
            _workingThread.Start();
        }

        /// <summary>
        /// ��������� ������ � ����������
        /// </summary>
        public void Stop()
        {
            _terminated.Set();
            _workingThread.Join(30000);
        }
    }
}
