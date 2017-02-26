using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using DevicesCommon.Helpers;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace DevicesBase.Communicators
{
    /// <summary>
    /// Коммуникатор для последовательного порта
    /// </summary>
    public sealed class SerialCommunicator : CustomCommunicator
    {
        // полседовательный порт для связи с устройством
        EasyCommunicationPort _port;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="portName">Имя порта</param>
        /// <param name="baudRate">Скорость обмена</param>
        public SerialCommunicator(string portName, Int32 baudRate)
            : base()
        {
            _port = new EasyCommunicationPort();
            _port.PortName = portName;
            _port.BaudRate = baudRate;
        }

        /// <summary>
        /// Таймаут чтения
        /// </summary>
        public override Int32 ReadTimeout
        {
            get { return _port.ReadTimeout; }
            set { _port.ReadTimeout = value; }
        }

        /// <summary>
        /// Таймаут записи
        /// </summary>
        public override Int32 WriteTimeout
        {
            get { return _port.WriteTimeout; }
            set { _port.WriteTimeout = value; }
        }

        /// <summary>
        /// Установка соединения с устройством
        /// </summary>
        public override void Open()
        {
            try
            {
                _port.Open();
                _port.DiscardBuffers();
            }
            catch (Win32Exception e)
            {
                throw new CommunicationException(
                    string.Format("Не удалось открыть порт {0}", _port.PortName), e);
            }
        }

        /// <summary>
        /// Закрывает подключение к устройству
        /// </summary>
        public override void Close()
        {
            try
            {
                _port.Close();
            }
            catch (Win32Exception e)
            {
                throw new CommunicationException(
                    string.Format("Не удалось закрыть порт {0}", _port.PortName), e);
            }
        }

        /// <summary>
        /// Чтение данных из коммуникатора
        /// </summary>
        /// <param name="buffer">Буфер данных</param>
        /// <param name="offset">Смещение от начала буфера</param>
        /// <param name="size">Размер принимаемых данных</param>
        public override Int32 Read(Byte[] buffer, Int32 offset, Int32 size)
        {
            try
            {
                return _port.Read(buffer, offset, size);
            }
            catch (TimeoutException e)
            {
                throw new CommunicationException("Таймаут чтения данных", e);
            }
            catch (Win32Exception e)
            {
                throw new CommunicationException(
                    string.Format("Ошибка чтения из порта {0}", _port.PortName), e);
            }
        }

        /// <summary>
        /// Запись данных в коммуникатор
        /// </summary>
        /// <param name="buffer">Буфер данных</param>
        /// <param name="offset">Смещение от начала буфера</param>
        /// <param name="size">Размер записываемых данных</param>
        public override Int32 Write(Byte[] buffer, Int32 offset, Int32 size)
        {
            try
            {
                return _port.Write(buffer, offset, size);
            }
            catch (TimeoutException e)
            {
                throw new CommunicationException("Таймаут записи данных", e);
            }
            catch (Win32Exception e)
            {
                throw new CommunicationException(
                    string.Format("Ошибка чтения из порта {0}", _port.PortName), e);
            }
        }
    }
}
