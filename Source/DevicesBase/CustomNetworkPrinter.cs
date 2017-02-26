using System;
using DevicesBase.Communicators;
using DevicesCommon.Helpers;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace DevicesBase
{
    /// <summary>
    /// Базовый класс для печатающих устройств, подключаемых по TCP
    /// </summary>
    public abstract class CustomNetworkPrinter : CustomPrintableDevice
    {
        #region Поля

        private TcpCommunicator _communicator;

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        protected CustomNetworkPrinter()
            : base()
        {
        }

        #endregion

        #region Закрытые методы

        private TcpCommunicator CreateCommunicator()
        {
            return new TcpCommunicator(PortName, TcpPort)
            {
                ReadTimeout = this.ReadTimeout,
                WriteTimeout = this.WriteTimeout
            };
        }

        #endregion

        #region Перегрузка методов базовых классов для исключения работы с последовательными портами

        /// <summary>
        /// Работа по последовательному порту
        /// </summary>
        protected override bool IsSerial
        {
            get { return false; }
        }

        /// <summary>
        /// Коммуникационный порт
        /// </summary>
        protected override EasyCommunicationPort Port
        {
            get
            {
                throw new InvalidOperationException("Это устройство предназначено для работы по TCP/IP");
            }
        }

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public override void Dispose()
        {
            if (_communicator != null)
            {
                _communicator.Dispose();
                _communicator = null;
            }
        }

        /// <summary>
        /// Вызывается перед активацией устройства
        /// </summary>
        protected override void OnBeforeActivate()
        {
        }

        /// <summary>
        /// Вызывается после активации устройства
        /// </summary>
        protected override void OnAfterActivate()
        {
        }

        /// <summary>
        /// Вызывается перед деактивацией устройства
        /// </summary>
        protected override void OnBeforeDeactivate()
        {
        }

        /// <summary>
        /// Вызывается после деактивации устройства
        /// </summary>
        protected override void OnAfterDeactivate()
        {
        }

        /// <summary>
        /// Активация устройства
        /// </summary>
        public override bool Active
        {
            get 
            {
                // устройство всегда активно
                return true; 
            } 
            set
            {
                if (value)
                {
                    OnBeforeActivate();
                    OnAfterActivate();
                }
                else
                {
                    OnBeforeDeactivate();
                    Dispose();
                    OnAfterDeactivate();
                }
            }
        }

        /// <summary>
        /// Печать документа
        /// </summary>
        /// <param name="xmlData">Данные документа</param>
        public override void Print(string xmlData)
        {
            try
            {
                using (_communicator = CreateCommunicator())
                {
                    _communicator.Open();
                    base.Print(xmlData);
                }
            }
            finally
            {
                _communicator = null;
            }
        }

        /// <summary>
        /// Состояние печатающего устройства
        /// </summary>
        public override PrinterStatusFlags PrinterStatus
        {
            get
            {
                if (_communicator != null)
                {
                    return OnQueryPrinterStatus(_communicator);
                }
                else
                {
                    using (var communicator = CreateCommunicator())
                    {
                        communicator.Open();
                        return OnQueryPrinterStatus(communicator);
                    }
                }
            }
        }

        #endregion

        #region Для работы с TCP-принтером

        /// <summary>
        /// TCP-порт принтера
        /// </summary>
        protected abstract int TcpPort
        {
            get;
        }

        /// <summary>
        /// Таймаут чтения
        /// </summary>
        protected abstract int ReadTimeout
        {
            get;
        }

        /// <summary>
        /// Таймаут записи
        /// </summary>
        protected abstract int WriteTimeout
        {
            get;
        }

        /// <summary>
        /// Запрос статуса принтера
        /// </summary>
        /// <param name="communicator">Коммуникатор</param>
        protected abstract PrinterStatusFlags OnQueryPrinterStatus(TcpCommunicator communicator);

        /// <summary>
        /// Коммуникатор для чтения/записи данных в порт принтера
        /// </summary>
        protected TcpCommunicator Communicator
        {
            get { return _communicator; }
        }

        #endregion
    }
}
