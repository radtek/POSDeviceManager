using System;
using System.Runtime.InteropServices;
using System.Text;
using DevicesBase;
using DevicesBase.Helpers;
using DevicesCommon.Helpers;

namespace PosiflexUSB
{
    [Serializable]
    [CustomerDisplayAttribute("Posiflex DT-2600 (USB)")]
    public class CustomerDisplay : CustomDisplayDevice
    {
        private const int DISPLAY_WIDTH = 20;

        protected override void OnAfterActivate()
        {
            // подключение
            USBPDLib.OpenUSBpd();

            // очистка дисплея
            USBPDLib.WritePD(new byte[] { 0x0C }, 1);

            // установка яркости
            USBPDLib.WritePD(new byte[] { 0x1F, 0x58, 0x04 }, 3);

            // выбираем кодовую страницу для символов 0x80-0xFF
            USBPDLib.WritePD(new byte[] { 0x1B, 0x74, 0x06 }, 3);

            base.OnAfterActivate();
        }

        protected override void OnBeforeDeactivate()
        {
            USBPDLib.CloseUSBpd();
            base.OnBeforeDeactivate();
        }

        public override void SaveToEEPROM()
        {
            // не поддерживается
            ErrorCode = new ServerErrorCode(this, GeneralError.Success);
        }

        public override string this[int lineNumber]
        {
            set
            {
                ErrorCode = new ServerErrorCode(this, GeneralError.Success);
                if (value.Length == 0)
                    return;

                try
                {
                    if (lineNumber < 0)
                        throw new ArgumentException();

                    if (lineNumber > 1)
                        return;

                    // дополняем строку пробелами
                    string output = value.PadRight(DISPLAY_WIDTH, ' ');

                    // обрезаем до необходимой длины
                    output = output.Substring(0, DISPLAY_WIDTH);

                    // выбираем номер строки
                    USBPDLib.WritePD(new byte[] { 0x1F, 0x24, 0x01, (byte)(lineNumber + 1) }, 4);

                    // вывод текста
                    var data = Encoding.GetEncoding(866).GetBytes(output);
                    USBPDLib.WritePD(data, data.Length);
                }
                catch (Exception E)
                {
                    ErrorCode = new ServerErrorCode(this, E);
                }
            }
        }
    }


    public static class USBPDLib
    {
        /// <summary>
        /// Транслирует команду ESCPOS на дисплей
        /// </summary>
        /// <param name="data"></param>
        /// <param name="size"></param>
        [DllImport("USBPD.dll")]
        public extern static void WritePD(byte[] data, long size);

        [DllImport("USBPD.dll")]
        public extern static void PdState();

        /// <summary>
        /// Подключение к дисплею
        /// </summary>
        [DllImport("USBPD.dll")]
        public extern static void OpenUSBpd();

        /// <summary>
        /// Отключение от дисплея
        /// </summary>
        [DllImport("USBPD.dll")]
        public extern static void CloseUSBpd();
    }
}
