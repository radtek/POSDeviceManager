using System;
using System.Collections.Generic;
using System.Text;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace DevicesBase
{
    /// <summary>
    /// Пул последовательных портов
    /// </summary>
    public interface ISerialPortsPool
    {
        /// <summary>
        /// Получить доступ к порту из пула
        /// </summary>
        /// <param name="deviceId">Идентификатор устройства</param>
        /// <param name="portName">Имя порта</param>
        /// <returns>Порт из пула</returns>
        EasyCommunicationPort GetPort(string deviceId, string portName);

        /// <summary>
        /// Захватить коммуникационный порт
        /// </summary>
        /// <param name="deviceId">Идентификатор устройства</param>
        /// <param name="portName">Имя порта</param>
        /// <param name="waitIfCaptured">Ожидать освобождения порта</param>
        /// <param name="waitTime">Время, в течение которого ожидать освобождение</param>
        /// <returns>Порт из пула</returns>
        EasyCommunicationPort CapturePort(string deviceId, string portName, Boolean waitIfCaptured,
            TimeSpan waitTime);

        /// <summary>
        /// Захватить коммуникационный порт. Ожидать захвата порта в течение 
        /// бесконечного интервала времени
        /// </summary>
        /// <param name="deviceId">Идентификатор устройства</param>
        /// <param name="portName">Имя порта</param>
        /// <returns>Порт из пула</returns>
        EasyCommunicationPort CapturePort(string deviceId, string portName);

        /// <summary>
        /// Освободить коммуникационный порт
        /// </summary>
        /// <param name="deviceId">Идентификатор устройства</param>
        /// <param name="portName">Имя порта</param>
        void ReleasePort(string deviceId, string portName);
    }
}
