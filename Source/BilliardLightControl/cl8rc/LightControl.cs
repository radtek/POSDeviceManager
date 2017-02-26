using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using ERPService.SharedLibs.Helpers.SerialCommunications;
using DevicesBase;
using DevicesCommon;
using DevicesCommon.Helpers;
using DevicesBase.Helpers;

namespace cl8rc
{
    /// <summary>
    /// Вспомогательный класс "Релейный модуль"
    /// </summary>
    internal class RelayModule
    {
        IDevice _parent;
        EasyCommunicationPort _port;
        byte _address;
        byte _relayStatus;
        bool _connected;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="parent">Родительское устройство</param>
        /// <param name="port">Коммуникационный порт</param>
        /// <param name="address">Адрес модуля</param>
        public RelayModule(IDevice parent, EasyCommunicationPort port, byte address)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            if (port == null)
                throw new ArgumentNullException("port");
            if (address > 7)
                throw new ArgumentOutOfRangeException("address");

            _parent = parent;
            _port = port;
            _address = address;

            // сбрасываем все реле модуля в "выключено"
            ExecuteCommand(0, 1);
        }

        /// <summary>
        /// Выполнение команды
        /// </summary>
        /// <param name="relayNo">Номер реле</param>
        /// <param name="switchOn">Включить</param>
        public ErrorCode ExecuteCommand(byte relayNo, bool switchOn)
        {
            if (!_connected)
                throw new InvalidOperationException(
                    string.Format("Модуль {0:X2} отключен", _address));
            if (relayNo > 7)
                throw new ArgumentOutOfRangeException("relayNo");

            // переключатель
            int switchByte = Pow2(relayNo);

            // формируем новый статус реле модуля
            byte nextRelayStatus = switchOn ?
                (byte)(_relayStatus | switchByte) :
                (byte)(_relayStatus & ~switchByte);

            // выполняем команду
            return ExecuteCommand(nextRelayStatus, 3);
        }

        /// <summary>
        /// Степень двойки
        /// </summary>
        /// <param name="pow">Показатель степени</param>
        private int Pow2(int pow)
        {
            if (pow < 1)
                return 1;

            int pow2 = 1;
            for (int i = 1; i <= pow; i++)
            {
                pow2 = pow2 * 2;
            }
            return pow2;
        }

        private ErrorCode OnException(bool saveErrors, Exception e)
        {
            if (saveErrors)
                return new ServerErrorCode(_parent, e);

            _connected = false;
            return new ServerErrorCode(_parent, GeneralError.Success);
        }

        /// <summary>
        /// Выполнение команды
        /// </summary>
        /// <param name="nextRelayStatus">Новое состояние реле</param>
        /// <param name="retryCount">Число попыток повтора</param>
        private ErrorCode ExecuteCommand(byte nextRelayStatus, byte retryCount)
        {

            // формируем команду
            string command = string.Format("@{0:X2}{1:X2}00\r",
                _address, nextRelayStatus);

            bool saveConnected = retryCount == 1;
            ErrorCode storedErrorCode;
            do
            {
                try
                {
                    // сбрасываем буферы порта
                    _port.DiscardBuffers();

                    // записываем команду в порт
                    _port.Write(Encoding.Default.GetBytes(command));

                    // читаем ответ
                    byte[] answer = new byte[4];
                    _port.Read(answer, 0, answer.Length);

                    if (saveConnected)
                        _connected = true;

                    if (answer[0] == (byte)'!')
                    {
                        _relayStatus = nextRelayStatus;
                        return new ServerErrorCode(_parent, GeneralError.Success);
                    }

                    return new ServerErrorCode(_parent, 1, "Команда не выполнена");
                }
                catch (TimeoutException e)
                {
                    retryCount--;
                    storedErrorCode = OnException(!saveConnected, e);
                }
                catch (Win32Exception e)
                {
                    retryCount--;
                    storedErrorCode = OnException(!saveConnected, e);
                }
            }
            while (retryCount > 0);
            return storedErrorCode;
        }
    }

    /// <summary>
    /// Управление освещением для бильярда с помощью контроллера 
    /// CL-8RC
    /// </summary>
    [BilliardsManager(DeviceNames.blcCl8rc)]
    public class LightControl : CustomBilliardsManagerDevice
    {
        private RelayModule[] _modules;

        #region Конструктор

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        public LightControl()
            : base()
        {
            _modules = new RelayModule[8];
        }

        #endregion

        #region Реализация протокола обмена

        protected override void OnAfterActivate()
        {
            base.OnAfterActivate();
            Port.DsrFlow = false;
            Port.WriteTimeout = 2000;
            Port.ReadTimeout = 1000;

            // выполняем опрос модулей
            for (int i = 0; i < _modules.Length; i++)
                _modules[i] = new RelayModule(this, Port, (byte)i);
        }

        private void SwitchTableLight(int billiardTableNo, bool switchOn)
        {
            if (billiardTableNo < 1 || billiardTableNo > 64)
                throw new ArgumentOutOfRangeException("billiardTableNo");

            // определяем адрес модуля по абсолютному номеру стола
            int moduleAddress = (billiardTableNo - 1) / 8;

            // выполняем команду
            ErrorCode = _modules[moduleAddress].ExecuteCommand((byte)((billiardTableNo - 1) % 8), switchOn);
        }

        #endregion

        /// <summary>
        /// Выключить свет
        /// </summary>
        /// <param name="billiardTableNo">Номер стола</param>
        public override void LightsOff(int billiardTableNo)
        {
            SwitchTableLight(billiardTableNo, false);
        }

        /// <summary>
        /// Включить свет
        /// </summary>
        /// <param name="billiardTableNo">Номер стола</param>
        public override void LightsOn(int billiardTableNo)
        {
            SwitchTableLight(billiardTableNo, true);
        }
    }
}
