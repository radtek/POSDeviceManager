using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;
using DevicesCommon;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace DevicesBase
{
    /// <summary>
    /// Базовый класс для устройств управления турникетом
    /// </summary>
    public abstract class CustomTurnstileDevice : CustomRS485Device, ITurnstileDevice
    {
        #region Константы

        private const Int32 FlashSleep = 80;
        private const Int32 ShortSleep = 100;
        private const Int32 PassWatchSleep = 10;
        private const Int32 MaxZeroRead = 25;
        private const string OperationCancelled = "Операция прервана устройством. Ответ: {0}";

        #endregion

        #region Поля

        private TurnstileDirection _direction;
        private object _syncObject;
        private Int32 _timeout;
        
        #endregion

        #region Закрытые свойства и методы

        /// <summary>
        /// Моргание красным индикатором
        /// </summary>
        /// <param name="count">Количество повторов</param>
        /// <param name="beep">Сопровождать звуковым сигналом</param>
        private void FlashRed(Int32 count, Boolean beep)
        {
            // обязательно выключаем зеленый 
            OnGreen(false);

            for (Int32 i = 0; i < count; i++)
            {
                OnRed(true);
                if (beep)
                    OnBeep(true);
                Thread.Sleep(FlashSleep);
                if (beep)
                    OnBeep(false);
                OnRed(false);
            }
        }

        /// <summary>
        /// Моргание зеленым индикатором
        /// </summary>
        /// <param name="count">Количество повторов</param>
        /// <param name="beep">Сопровождать звуковым сигналом</param>
        private void FlashGreen(Int32 count, Boolean beep)
        {
            // обязательно выключаем красный
            OnRed(false);

            for (Int32 i = 0; i < count; i++)
            {
                OnGreen(true);
                if (beep)
                    OnBeep(true);
                Thread.Sleep(FlashSleep);
                if (beep)
                    OnBeep(false);
                OnGreen(false);
            }
        }

        #endregion

        #region Свойства и методы, реализуемые в потомках

        /// <summary>
        /// Стоп-символ
        /// </summary>
        protected abstract Byte TermChar { get; }

        /// <summary>
        /// Чтение идентификационных данных
        /// </summary>
        /// <returns>Данные, подготовленные для обработки</returns>
        protected abstract string OnReadIdData();

        /// <summary>
        /// Управление красным индикатором
        /// </summary>
        /// <param name="flashOn">Включить</param>
        protected abstract void OnRed(Boolean flashOn);

        /// <summary>
        /// Управление зеленым индикатором
        /// </summary>
        /// <param name="flashOn">Включить</param>
        protected abstract void OnGreen(Boolean flashOn);

        /// <summary>
        /// Управление звуком
        /// </summary>
        /// <param name="beepOn">Включить</param>
        protected abstract void OnBeep(Boolean beepOn);

        /// <summary>
        /// Отправка сигнала открытия для турникета
        /// </summary>
        protected abstract void OnOpen();

        /// <summary>
        /// Отправка сигнала закрытия для турникета
        /// </summary>
        protected abstract void OnClose();

        /// <summary>
        /// Проверка факта совершения прохода через турникет
        /// </summary>
        /// <returns>true, если был совершен проход</returns>
        protected abstract Boolean OnPassComplete();

        #endregion

        #region Перегрузка методов базового класса

        /// <summary>
        /// Действия после активации устройства
        /// </summary>
        protected override void OnAfterActivate()
        {
            base.OnAfterActivate();
            FlashRed(3, true);
            Close(false);
        }

        /// <summary>
        /// Действия перед деактивацией устройства
        /// </summary>
        protected override void OnBeforeDeactivate()
        {
            FlashRed(3, true);
            Close(false);
            base.OnBeforeDeactivate();
        }

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        protected CustomTurnstileDevice()
            : base()
        {
            _direction = TurnstileDirection.Entry;
            _syncObject = new object();
            _timeout = 15;
        }

        #endregion

        #region Реализация ITurnstileDevice Members

        /// <summary>
        /// Направление, в котором работает турникет
        /// </summary>
        public TurnstileDirection Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        /// <summary>
        /// Таймаут открытия турникета
        /// </summary>
        public Int32 Timeout 
        {
            get { return _timeout; }
            set { _timeout = value; } 
        }

        /// <summary>
        /// Открыть турникет
        /// </summary>
        /// <returns>true, если в течение таймаута через турникет совершен проход</returns>
        public Boolean Open()
        {
            TimeSpan passTimeOut = new TimeSpan(0, 0, _timeout);
            lock (_syncObject)
            {
                // включаем сигнализацию
                FlashGreen(2, true);
                OnGreen(true);
                // открываем турникет
                OnOpen();

                try
                {
                    // засекаем время
                    DateTime fixedDt = DateTime.Now;

                    // ожидаем факт прохода через него
                    Boolean passComplete = false;
                    do
                    {
                        passComplete = OnPassComplete();
                        if (!passComplete)
                            Thread.Sleep(PassWatchSleep);
                    }
                    while (!passComplete && (DateTime.Now - fixedDt) < passTimeOut);
                    return passComplete;
                }
                finally
                {
                    // закрываем турникет, независимо от того, выполнен проход или нет
                    FlashGreen(2, true);
                    Close(false);
                }
            }
        }

        /// <summary>
        /// Закрыть турникет
        /// </summary>
        /// <param name="accessDenied">Доступ запрещен</param>
        public void Close(Boolean accessDenied)
        {
            lock (_syncObject)
            {
                // закрываем турникет
                if (accessDenied)
                {
                    // турникет закрылся, потому что доступ запрещен
                    FlashRed(3, true);
                }
                OnRed(true);
                OnClose();
            }
        }

        /// <summary>
        /// Очередной блок идентификационных данных от устройства
        /// </summary>
        public string IdentificationData 
        { 
            get
            {
                string data = string.Empty;
                lock (_syncObject)
                {
                    data = OnReadIdData();
                    if (!string.IsNullOrEmpty(data))
                    {
                        FlashRed(1, true);
                        OnRed(true);
                    }
                }
                return data;
            }
        }

        #endregion
    }
}
