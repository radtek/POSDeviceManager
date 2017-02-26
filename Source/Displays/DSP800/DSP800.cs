using System;
using System.Text;
using DevicesBase;
using DevicesBase.Helpers;
using DevicesCommon.Helpers;

namespace DSP800
{
    [Serializable]
    [CustomerDisplayAttribute(DeviceNames.customerDisplayDSP)]
    public class CustomerDisplay : CustomDisplayDevice
    {
        private const int DISPLAY_WIDTH = 20;
        private const byte EOT = 0x04;
        private const byte SOH = 0x01;
        private const byte ETB = 0x17;
        private const byte ACK = 0x06;

        private string[] DisplayLines = new string[2];

        public override void SaveToEEPROM()
        {
            ErrorCode = new ServerErrorCode(this, GeneralError.Success);
            if ((DisplayLines[0].Length == 0) && (DisplayLines[1].Length == 0))
                return;

            try
            {
                this[0] = DisplayLines[0];
                this[1] = DisplayLines[1];

                // устанавливаем курсор в заданную позицию
                byte[] nCmd = new byte[5];
                nCmd[0] = EOT;
                nCmd[1] = SOH;
                nCmd[2] = Convert.ToByte('S');
                nCmd[3] = 0x31;
                nCmd[4] = ETB;
                Port.Write(nCmd, 0, 5);
                //                if(Port.ReadByte() == ACK)
                //                    ErrorCode = (short)ErrorCodes.e_success;
                //                else
                //                    ErrorCode = (short)ErrorCodes.e_timeout;
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
                    DisplayLines[lineNumber] = value;

                    if (lineNumber == 0)
                        Locate(0x31);
                    else
                        Locate(0x45);

                    if (!ErrorCode.Succeeded)
                        return;

                    byte[] nLine = Encoding.GetEncoding(866).GetBytes(value.PadRight(DISPLAY_WIDTH, ' '));
                    Port.Write(nLine, 0, DISPLAY_WIDTH);
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

        private void Locate(byte nPos)
        {
            // устанавливаем курсор в заданную позицию
            byte[] nCmd = new byte[5];
            nCmd[0] = EOT;
            nCmd[1] = SOH;
            nCmd[2] = Convert.ToByte('P');
            nCmd[3] = nPos;
            nCmd[4] = ETB;

            try
            {
                Port.Write(nCmd, 0, 5);
            }
            catch (TimeoutException)
            {
                ErrorCode = new ServerErrorCode(this, GeneralError.Timeout);
            }
        }
    }
}
