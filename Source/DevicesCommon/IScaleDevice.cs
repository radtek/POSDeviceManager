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
        void Upload(string xmlData);

        /// <summary>
        /// Текущие показания веса
        /// </summary>
        int Weight { get; }

        /// <summary>
        /// Строка с параметрами подключения к весам
        /// <example>
        /// tcp://host:port
        /// udp://host:port
        /// rs://port_name:baud
        /// </example>
        /// </summary>
        string ConnectionString { get; set; }
    }
}
