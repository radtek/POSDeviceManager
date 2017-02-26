namespace DevicesCommon
{
    /// <summary>
    /// Направление, в котором работает турникет
    /// </summary>
    public enum TurnstileDirection
    {
        /// <summary>
        /// На вход
        /// </summary>
        Entry,

        /// <summary>
        /// На выход
        /// </summary>
        Exit
    }

    /// <summary>
    /// Интерфейс устройств для управления турникетом
    /// </summary>
    public interface ITurnstileDevice : IRS485Device
    {
        #region Управление механизмом турникета

        /// <summary>
        /// Направление, в котором работает турникет
        /// </summary>
        TurnstileDirection Direction { get; set; }

        /// <summary>
        /// Таймаут открытия турникета
        /// </summary>
        int Timeout { get; set; }

        /// <summary>
        /// Открыть турникет
        /// </summary>
        /// <returns>true, если в течение таймаута через турникет совершен проход</returns>
        bool Open();

        /// <summary>
        /// Закрыть турникет
        /// </summary>
        /// <param name="accessDenied">Доступ запрещен</param>
        void Close(bool accessDenied);

        #endregion

        #region Работа с идентификационным устройством

        /// <summary>
        /// Очередной блок идентификационных данных от устройства
        /// </summary>
        string IdentificationData { get; }

        #endregion
    }
}
