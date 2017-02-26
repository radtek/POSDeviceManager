using System;

namespace DevicesBase.Communicators
{
    /// <summary>
    /// Интерфейс коммункационного устройства (порт RS-232, сокет и т.п.)
    /// </summary>
    public interface ICommunicator : IDisposable
    {
        /// <summary>
        /// Таймаут чтения
        /// </summary>
        int ReadTimeout { get; set; }

        /// <summary>
        /// Таймаут записи
        /// </summary>
        int WriteTimeout { get; set; }

        /// <summary>
        /// Установка соединения с устройством
        /// </summary>
        void Open();

        /// <summary>
        /// Закрывает подключение к устройству
        /// </summary>
        void Close();

        /// <summary>
        /// Чтение данных из коммуникатора
        /// </summary>
        /// <param name="buffer">Буфер данных</param>
        /// <param name="offset">Смещение от начала буфера</param>
        /// <param name="size">Размер принимаемых данных</param>
        int Read(byte[] buffer, int offset, int size);

        /// <summary>
        /// Запись данных в коммуникатор
        /// </summary>
        /// <param name="buffer">Буфер данных</param>
        /// <param name="offset">Смещение от начала буфера</param>
        /// <param name="size">Размер записываемых данных</param>
        int Write(byte[] buffer, int offset, int size);

        /// <summary>
        /// Чтение байта из коммуникатора
        /// </summary>
        byte ReadByte();

        /// <summary>
        /// Запись байта в коммуникатор
        /// </summary>
        /// <param name="b">Байт для записи</param>
        void WriteByte(byte b);
    }
}
