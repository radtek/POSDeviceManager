using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon.Helpers;

namespace DevicesCommon
{
    /// <summary>
    /// »нтерфейс устройства, управл€ющего биль€рдными столами
    /// </summary>
    public interface IBilliardsManagerDevice : ISerialDevice
    {
        /// <summary>
        /// ¬ключить свет над биль€рдным столом
        /// </summary>
        /// <param name="billiardTableNo">Ќомер биль€рдного стола</param>
        void LightsOn(Int32 billiardTableNo);

        /// <summary>
        /// ќтключить свет над биль€рдным столом
        /// </summary>
        /// <param name="billiardTableNo">Ќомер биль€рдного стола</param>
        void LightsOff(Int32 billiardTableNo);
    }
}
