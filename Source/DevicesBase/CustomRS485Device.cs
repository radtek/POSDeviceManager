using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using DevicesCommon;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace DevicesBase
{
    /// <summary>
    /// Базовый класс для устройств, работающих по интерфейсу RS-485
    /// </summary>
    public abstract class CustomRS485Device : CustomSerialDevice, IRS485Device
    {
        #region Поля

        private int _address;

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        protected CustomRS485Device()
            : base()
        {
            _address = 0;
        }

        #endregion

        #region Реализация IRS485Device

        /// <summary>
        /// Адрес устройства
        /// </summary>
        public int Address
        {
            get { return _address; }
            set { _address = value; }
        }

        #endregion
    }
}

