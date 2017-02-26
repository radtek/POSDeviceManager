using System;
using System.Text;
using DevicesBase;
using DevicesBase.Helpers;
using DevicesCommon.Helpers;

namespace Aedex
{
    [Serializable]
    [CustomerDisplayAttribute(DeviceNames.customerDisplayAedex)]
    public class CustomerDisplay : CustomDisplayDevice
    {
        private const int DISPLAY_WIDTH = 20;

        public override void SaveToEEPROM()
        {
            // команда не поддерживается
//            errorCode = new ErrorCode(this, GeneralError.Unsupported);
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
                    byte[] nCmd = new byte[DISPLAY_WIDTH + 4];
                    nCmd[0] = 0x21;
                    nCmd[1] = 0x23;

                    if (lineNumber == 0)
                        nCmd[2] = 0x31;
                    else
                        nCmd[2] = 0x32;

                    Encoding.GetEncoding(866).GetBytes(value.PadRight(DISPLAY_WIDTH, ' '), 
                        0, DISPLAY_WIDTH, nCmd, 3);

                    nCmd[DISPLAY_WIDTH + 3] = 0x0D;

                    Port.Write(nCmd, 0, DISPLAY_WIDTH + 4);
                }
                catch (TimeoutException)
                {
                    ErrorCode = new ServerErrorCode(this, GeneralError.Timeout);
                }
                catch (Exception E)
                {
                    ErrorCode = new ServerErrorCode(this, E);
                }
            }
        }
    }
}
