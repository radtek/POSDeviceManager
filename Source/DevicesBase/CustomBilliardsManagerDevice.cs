using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon;

namespace DevicesBase
{
    /// <summary>
    /// Ѕазовый класс дл€ устройств, управл€ющих биль€рдными столами
    /// </summary>
    public abstract class CustomBilliardsManagerDevice : CustomSerialDevice, 
        IBilliardsManagerDevice
    {
        /// <summary>
        /// —оздает экземпл€р класса
        /// </summary>
        protected CustomBilliardsManagerDevice() : base()
        {
        }

        #region –еализаци€ IBilliardsManagerDevice

        /// <summary>
        /// ќтключение света над биллиардным столом
        /// </summary>
        /// <param name="billiardTableNo">Ќомер биль€рдного стола</param>
        public abstract void LightsOff(int billiardTableNo);

        /// <summary>
        /// ¬ключение света над биль€рдным столом
        /// </summary>
        /// <param name="billiardTableNo">Ќомер биль€рдного стола</param>
        public abstract void LightsOn(int billiardTableNo);

        #endregion
    }
}
