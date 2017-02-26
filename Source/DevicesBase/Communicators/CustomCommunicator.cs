using System;
using System.IO;

namespace DevicesBase.Communicators
{
    /// <summary>
    /// Базовый класс для коммуникационного устройства
    /// </summary>
    public abstract class CustomCommunicator : ICommunicator
    {
        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        protected CustomCommunicator()
        {
        }

        #region Реализация ICommunicator

        /// <summary>
        /// Таймаут чтения
        /// </summary>
        public abstract int ReadTimeout
        {
            get; set;
        }

        /// <summary>
        /// Таймаут записи
        /// </summary>
        public abstract int WriteTimeout
        {
            get; set;
        }

        /// <summary>
        /// Установка соединения с устройством
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// Закрывает подключение к устройству
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Чтение данных из коммуникатора
        /// </summary>
        /// <param name="buffer">Буфер данных</param>
        /// <param name="offset">Смещение от начала буфера</param>
        /// <param name="size">Размер принимаемых данных</param>
        public abstract int Read(byte[] buffer, int offset, int size);

        /// <summary>
        /// Запись данных в коммуникатор
        /// </summary>
        /// <param name="buffer">Буфер данных</param>
        /// <param name="offset">Смещение от начала буфера</param>
        /// <param name="size">Размер записываемых данных</param>
        public abstract int Write(byte[] buffer, int offset, int size);

        /// <summary>
        /// Чтение байта из коммуникатора
        /// </summary>
        public byte ReadByte()
        {
            var buf = new byte[1];
            if (Read(buf, 0, buf.Length) != buf.Length)
                throw new IOException("Операция чтения завершена с ошибкой.");

            return buf[0];
        }

        /// <summary>
        /// Запись байта в коммуникатор
        /// </summary>
        /// <param name="b">Байт для записи</param>
        public void WriteByte(byte b)
        {
            var buf = new[] { b };
            if (Write(buf, 0, buf.Length) != buf.Length)
                throw new IOException("Операция записи завершена с ошибкой.");
        }

        #endregion

        #region Реализация IDisposable

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        #endregion
    }
}
