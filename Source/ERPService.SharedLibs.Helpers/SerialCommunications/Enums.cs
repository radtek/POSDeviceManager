namespace ERPService.SharedLibs.Helpers.SerialCommunications
{
    /// <summary>
    /// Четность
    /// </summary>
    public enum Parity : byte
    {
        /// <summary>
        /// Нет
        /// </summary>
        None,

        /// <summary>
        /// Нечетное
        /// </summary>
        Odd,

        /// <summary>
        /// Четное
        /// </summary>
        Even,

        /// <summary>
        /// По единичному биту чётности
        /// </summary>
        Mark,

        /// <summary>
        /// По нулевому биту чётности
        /// </summary>
        Space
    }

    /// <summary>
    /// Стоповых бит
    /// </summary>
    public enum StopBits : byte
    {
        /// <summary>
        /// Один
        /// </summary>
        One,

        /// <summary>
        /// Полтора
        /// </summary>
        OneAndHalf,

        /// <summary>
        /// Два
        /// </summary>
        Two
    }
}
