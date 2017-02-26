using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon;
using DevicesCommon.Helpers;
using DevicesBase;
using DevicesBase.Helpers;

namespace Firich
{
    [Serializable]
    [CustomerDisplayAttribute(DeviceNames.customerDisplayVFD)]
    public class CustomerDisplay : CustomDisplayDevice
    {
        private const int DISPLAY_WIDTH = 20;

        private string[] DisplayLines = new string[2];

        public override void SaveToEEPROM()
        {
            ErrorCode = new ServerErrorCode(this, GeneralError.Success);
            if ((DisplayLines[0].Length == 0) && (DisplayLines[1].Length == 0))
                return;

            try
            {
                byte[] nCmd = Encoding.Default.GetBytes(new string(' ', 44));
                nCmd[0] = 0x0C;

                byte[] nLine = Encoding.GetEncoding(866).GetBytes(DisplayLines[0]);
                if (nLine.Length > 20)
                    Array.Copy(nLine, 0, nCmd, 1, 20);
                else
                    Array.Copy(nLine, 0, nCmd, 1, nLine.Length);

                nLine = Encoding.GetEncoding(866).GetBytes(DisplayLines[1]);
                if (nLine.Length > 20)
                    Array.Copy(nLine, 0, nCmd, 21, 20);
                else
                    Array.Copy(nLine, 0, nCmd, 21, nLine.Length);

                nCmd[41] = 0x1B;
                nCmd[42] = 0x53;
                nCmd[43] = 0x31;

                Port.Write(nCmd, 0, 44);
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

        public override string this[int lineNumber]
        {
            set
            {
                ErrorCode = new ServerErrorCode(this, GeneralError.Success);
                if (value.Length == 0)
                    return;

                try
                {
                    byte[] nCmd = Encoding.Default.GetBytes(new string(' ', 24));
                    DisplayLines[lineNumber] = value;

                    nCmd[0] = 0x1B;
                    nCmd[1] = 0x51;
                    if (lineNumber == 0)
                        nCmd[2] = 0x41;
                    else
                        nCmd[2] = 0x42;

                    byte[] nLine = Encoding.GetEncoding(866).GetBytes(value);
                    if (nLine.Length > DISPLAY_WIDTH)
                        Array.Copy(nLine, 0, nCmd, 3, DISPLAY_WIDTH);
                    else
                        Array.Copy(nLine, 0, nCmd, 3, nLine.Length);

                    nCmd[23] = 13;
                    Port.Write(nCmd, 0, 24);
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
