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
        void LightsOn(int billiardTableNo);

        /// <summary>
        /// ќтключить свет над биль€рдным столом
        /// </summary>
        /// <param name="billiardTableNo">Ќомер биль€рдного стола</param>
        void LightsOff(int billiardTableNo);
    }
}
