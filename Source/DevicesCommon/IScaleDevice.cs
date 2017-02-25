using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon.Helpers;

namespace DevicesCommon
{
    /// <summary>
    /// Весы с печатью этикетки
    /// </summary>
    public interface IScaleDevice : IDevice
    {
        /// <summary>
        /// Выгрузка данных в весы
        /// </summary>
        /// <param name="xmlData"></param>
        void Upload(String xmlData);

        /// <summary>
        /// Текущие показания веса
        /// </summary>
        Int32 Weight { get; }

        /// <summary>
        /// Строка с параметрами подключения к весам
        /// <example>
        /// tcp://host:port
        /// udp://host:port
        /// rs://port_name:baud
        /// </example>
        /// </summary>
        String ConnectionString { get; set; }
    }
}
