using System;
using System.Collections.Generic;
using System.Text;

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
        Int32 ReadTimeout { get; set; }
        
        /// <summary>
        /// Таймаут записи
        /// </summary>
        Int32 WriteTimeout { get; set; }

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
        Int32 Read(Byte[] buffer, Int32 offset, Int32 size);

        /// <summary>
        /// Запись данных в коммуникатор
        /// </summary>
        /// <param name="buffer">Буфер данных</param>
        /// <param name="offset">Смещение от начала буфера</param>
        /// <param name="size">Размер записываемых данных</param>
        Int32 Write(Byte[] buffer, Int32 offset, Int32 size);

        /// <summary>
        /// Чтение байта из коммуникатора
        /// </summary>
        Byte ReadByte();

        /// <summary>
        /// Запись байта в коммуникатор
        /// </summary>
        /// <param name="b">Байт для записи</param>
        void WriteByte(Byte b);
    }
}
