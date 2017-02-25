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
        public abstract Int32 ReadTimeout
        {
            get; set;
        }

        /// <summary>
        /// Таймаут записи
        /// </summary>
        public abstract Int32 WriteTimeout
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
        public abstract Int32 Read(Byte[] buffer, Int32 offset, Int32 size);

        /// <summary>
        /// Запись данных в коммуникатор
        /// </summary>
        /// <param name="buffer">Буфер данных</param>
        /// <param name="offset">Смещение от начала буфера</param>
        /// <param name="size">Размер записываемых данных</param>
        public abstract Int32 Write(Byte[] buffer, Int32 offset, Int32 size);

        /// <summary>
        /// Чтение байта из коммуникатора
        /// </summary>
        public Byte ReadByte()
        {
            var buf = new Byte[1];
            if (Read(buf, 0, buf.Length) != buf.Length)
                throw new IOException("Операция чтения завершена с ошибкой.");

            return buf[0];
        }

        /// <summary>
        /// Запись байта в коммуникатор
        /// </summary>
        /// <param name="b">Байт для записи</param>
        public void WriteByte(Byte b)
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
