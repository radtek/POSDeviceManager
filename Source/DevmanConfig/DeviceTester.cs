using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon;
using DevicesCommon.Connectors;
using System.Windows.Forms;

namespace DevmanConfig
{
    /// <summary>
    /// Делегат для выполнения тестов устройства
    /// </summary>
    /// <param name="device">Интерфейс устройства</param>
    internal delegate void DeviceTestDelegate<TIntf>(TIntf device) where TIntf : IDevice;

    /// <summary>
    /// Вспомогательный класс для выполнения тестов 
    /// устройств
    /// </summary>
    internal sealed class DeviceTester<TIntf>
        where TIntf: IDevice
    {
        private string _deviceId;
        private DeviceTestDelegate<TIntf> _testCallback;

        /// <summary>
        /// Создает 
        /// </summary>
        /// <param name="deviceId">Идентификатор устройства</param>
        /// <param name="testCallback">Делегат для выполнения тестов</param>
        public DeviceTester(string deviceId, DeviceTestDelegate<TIntf> testCallback)
        {
            _deviceId = deviceId;
            _testCallback = testCallback;
        }

        /// <summary>
        /// Выполнение теста
        /// </summary>
        public void Execute()
        {
            try
            {
                using (DeviceManagerClient dmClient = new DeviceManagerClient("localhost"))
                {
                    dmClient.Login();
                    if (dmClient.Capture(_deviceId, 5))
                    {
                        try
                        {
                            if (_testCallback != null)
                                _testCallback((TIntf)dmClient[_deviceId]);
                        }
                        finally
                        {
                            dmClient.Release(_deviceId);
                        }
                    }
                    else
                        MessageBox.Show(
                            string.Format("Не удалось получить доступ к устройству \"{0}\"",
                            _deviceId), "Тест устройства", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                }
            }
            catch (Exception e)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("Ошибка тестирования устройства \"{0}\".", 
                    _deviceId));
                sb.AppendLine(string.Format("Тип: {0}.", e.GetType().Name));
                sb.AppendLine(string.Format("Сообщение: {0}.", e.Message));
                sb.AppendLine("Трассировка стека:");
                sb.Append(e.StackTrace);

                MessageBox.Show(sb.ToString(), "Тест устройства", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
