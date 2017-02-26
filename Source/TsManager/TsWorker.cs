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
    /// Работа с турникетом
    /// </summary>
    internal class TsWorker
    {
        #region Поля

        private IAMCSLogic _amcsLogic;
        private TsUnitSettings _unitSettings;
        private Thread _workingThread;
        private ManualResetEvent _terminated;
        private IEventLink _eventLink;
        private DeviceManagerClient _client;
        private ITurnstileDevice _device;

        #endregion

        #region Закрытые методы

        /// <summary>
        /// Захват устройства
        /// </summary>
        private void CaptureDevice()
        {
            if (_client != null && _device != null)
                // устройство уже захвачено
                return;

            // подключаемся к диспетчеру устройств
            _client = new DeviceManagerClient(_unitSettings.HostOrIp, _unitSettings.Port);
            _client.Login();
            // захватываем турникет
            _client.Capture(_unitSettings.DeviceId, Timeout.Infinite);
            _device = (ITurnstileDevice)_client[_unitSettings.DeviceId];
            // подключение установлено
            _eventLink.Post(TsGlobalConst.EventSource, string.Format(
                "[0] Подключение к диспетчеру устройств установлено", _unitSettings));
        }

        /// <summary>
        /// Освобождаем устройство
        /// </summary>
        private void ReleaseDevice()
        {
            if (_client == null)
                // устройство свободно
                return;

            try
            {
                // освобождаем устройство
                _client.Release(_unitSettings.DeviceId);
                // закрываем клиентскую сессию
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

                // подключение закрыто
                _eventLink.Post(TsGlobalConst.EventSource, string.Format(
                    "[0] Подключение к диспетчеру устройств закрыто", _unitSettings));
            }
        }

        /// <summary>
        /// Поток для работы с турникетами
        /// </summary>
        private void WorkWithTurnstile()
        {
            // запуск потока
            _eventLink.Post(TsGlobalConst.EventSource,
                string.Format("[{0}] Запуск рабочего потока", _unitSettings));

            // цикл опроса устройства идентификации турникета
            // проводится до тех пор, пока событие не перейдет в сигнальное состояние
            while (!_terminated.WaitOne(0, false))
            {
                try
                {
                    // захват устройства
                    CaptureDevice();

                    // запращиваем идентификационные данные с устройства
                    string idData = _device.IdentificationData;
                    if (string.IsNullOrEmpty(idData))
                        continue;

                    string direction = _device.Direction == TurnstileDirection.Entry ?
                        "Вход" : "Выход";

                    // проверяем возможность прохода в заданном направлении
                    _eventLink.Post(TsGlobalConst.EventSource,
                        string.Format("[{0}] Получены идентификационные данные [{1}]", 
                        _unitSettings, idData));

                    string reason;
                    if (_amcsLogic.IsAccessGranted(_device.Direction, idData, out reason))
                    {
                        // доступ разрешен
                        _eventLink.Post(TsGlobalConst.EventSource, string.Format(
                            "[{0}] Доступ разрешен, направление [{1}], идентификационные данные [{2}]",
                            _unitSettings, direction, idData));
                        // открываем турникет
                        bool passOk = _device.Open();
                        if (passOk)
                        {
                            // посетитель прошел
                            _eventLink.Post(TsGlobalConst.EventSource, string.Format(
                                "[{0}] Турникет закрыт, проход выполнен", _unitSettings));
                            // уведомляем об этом СКУД
                            _amcsLogic.OnAccessOccured(_device.Direction, idData);
                        }
                        else
                            _eventLink.Post(TsGlobalConst.EventSource, string.Format(
                                "[{0}] Турникет закрыт, истекло время ожидания", _unitSettings));
                    }
                    else
                    {
                        // доступ запрещен
                        _eventLink.Post(TsGlobalConst.EventSource, string.Format(
                            "[{0}] Доступ ЗАПРЕЩЕН, направление [{1}], идентификационные данные [{2}]. Причина: [{3}]",
                            _unitSettings, direction, idData, reason));
                        // закрываем турникет и сигнализируем посетителю
                        _device.Close(true);
                    }
                }
                catch (Exception e)
                {
                    // протоколируем информацию об исключении
                    _eventLink.Post(TsGlobalConst.EventSource, string.Format(
                        "[{0}] Исключение в рабочем потоке", _unitSettings), e);
                    
                    // освобождаем устройство
                    ReleaseDevice();
                }
            }

            // освобождение устройства
            ReleaseDevice();
            // остановка потока
            _eventLink.Post(TsGlobalConst.EventSource,
                string.Format("[{0}] Остановка рабочего потока", _unitSettings));
        }

        #endregion

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="amcsLogic">Реализация логики работы СКУД</param>
        /// <param name="unitSettings">Парамтеры тукникета</param>
        /// <param name="eventLink">Интерфейс журнала событий</param>
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
        /// Запуск работы с турикетом
        /// </summary>
        public void Start()
        {
            _workingThread.Start();
        }

        /// <summary>
        /// Остановка работы с турникетом
        /// </summary>
        public void Stop()
        {
            _terminated.Set();
            _workingThread.Join(30000);
        }
    }
}
