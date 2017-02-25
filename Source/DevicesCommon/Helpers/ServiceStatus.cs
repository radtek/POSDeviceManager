using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceProcess;

namespace DevicesCommon.Helpers
{
    /// <summary>
    /// Преобразует значение перечисления ServiceControllerStatus в строку
    /// </summary>
    public class ServiceStatus
    {
        private ServiceControllerStatus _originalStatus;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="originalStatus">Исходное значение статуса сервиса</param>
        public ServiceStatus(ServiceControllerStatus originalStatus)
        {
            _originalStatus = originalStatus;
        }

        /// <summary>
        /// Возвращает строковое представление объекта
        /// </summary>
        public override String ToString()
        {
 	        switch(_originalStatus)
            {
                case ServiceControllerStatus.ContinuePending:
                    return "Продолжение работы";
                case ServiceControllerStatus.Paused:
                    return "Приостановлен";
                case ServiceControllerStatus.PausePending:
                    return "Приостанавливается";
                case ServiceControllerStatus.Running:
                    return "Работает";
                case ServiceControllerStatus.StartPending:
                    return "Запускается";
                case ServiceControllerStatus.Stopped:
                    return "Остановлен";
                default:
                    return "Останавливается";
            }
        }
    }
}
