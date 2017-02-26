using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERPService.SharedLibs.Helpers.SerialCommunications;
using DevicesBase;
using DevicesCommon;
using DevicesCommon.Helpers;
using DevicesBase.Helpers;

namespace ICPCON7065
{
    /// <summary>
    /// Управление освещением для бильярда с помощью контроллера 
    /// ICPCON-7065
    /// </summary>
    [BilliardsManager("Контроллер ICPCON-7065")]
    public class LightControl : CustomBilliardsManagerDevice
    {
        #region Константы

        private const bool USE_CHECKSUM = false;
        private const int READ_TIMEOUT = 2000;
        private const int WRITE_TIMEOUT = 1000;
        private const int MAX_RETRIES_COUNT = 5;

        #endregion

        #region Переопределенные методы

        protected override void OnAfterActivate()
        {
            base.OnAfterActivate();
            Port.DsrFlow = false;
            Port.WriteTimeout = WRITE_TIMEOUT;
            Port.ReadTimeout = READ_TIMEOUT;
        }

        public override void LightsOff(int billiardTableNo)
        {
            SetDigitalOutput(billiardTableNo, 0);
        }

        public override void LightsOn(int billiardTableNo)
        {
            SetDigitalOutput(billiardTableNo, 1);
        }

        #endregion

        #region Скрытые методы

        /// <summary>
        /// Выполнение команды
        /// </summary>
        /// <param name="command">Строка команды</param>
        /// <returns>Код ошибки</returns>
        private ErrorCode ExecuteCommand(string command)
        {
            if (USE_CHECKSUM)
            {
                int crc = 0;
                foreach (var c in Encoding.ASCII.GetBytes(command))
                    crc += c;
                crc = crc & 0xFF;
                command += crc.ToString("X2");
            }
            command += '\r';

            int retryCount = 0;
            bool success = false;
            do
            {
                try
                {
                    retryCount++;
                    // передача команды
                    Port.DiscardBuffers();
                    Port.Write(Encoding.ASCII.GetBytes(command), 0, Encoding.ASCII.GetByteCount(command));

                    // чтение ответа
                    byte[] response = new byte[4];
                    Port.Read(response, 0, USE_CHECKSUM ? 4 : 2);
                    switch ((char)response[0])
                    {
                        case '<':
                            success = true;
                            break;
                        case '?':
                            if (retryCount >= MAX_RETRIES_COUNT)
                                return new ServerErrorCode(this, 1, "Команда не выполнена");
                            break;
                        case '!':
                            if (retryCount >= MAX_RETRIES_COUNT)
                                return new ServerErrorCode(this, 2, "Команда проигнорирована");
                            break;
                        default:
                            if (retryCount >= MAX_RETRIES_COUNT)
                                return new ServerErrorCode(this, 3, "Некорректный ответ устройства");
                            break;
                    }
                }
                catch (System.ComponentModel.Win32Exception E)
                {
                    if (retryCount >= MAX_RETRIES_COUNT)
                        return new ServerErrorCode(this, E);
                }
                catch (TimeoutException)
                {
                    if (retryCount >= MAX_RETRIES_COUNT)
                        return new ServerErrorCode(this, GeneralError.Timeout);
                }
            }
            while (!success);

            return new ServerErrorCode(this, GeneralError.Success);
        }

        /// <summary>
        /// Установка уровня выходного сигнала 
        /// </summary>
        /// <param name="billiardTableNo">Номер стола</param>
        /// <param name="switchOn">Уровень сигнала</param>
        private void SetDigitalOutput(int billiardTableNo, int switchOn)
        {
            if (billiardTableNo < 1)
                throw new ArgumentOutOfRangeException("billiardTableNo");

            // определяем адрес модуля по абсолютному номеру стола
            int address = (billiardTableNo - 1) / 5 + 1;
            int output = (billiardTableNo - 1) % 5;

            // выполняем команду
            ErrorCode = ExecuteCommand(string.Format("#{0:D2}1{1}{2:D2}", address, output, switchOn));
        }

        #endregion
    }
}
