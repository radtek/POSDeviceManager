using System;
using System.Text;
using DevicesBase;
using DevicesBase.Helpers;
using DevicesCommon.Helpers;

namespace Epson
{
    [Serializable]
    [CustomerDisplayAttribute(DeviceNames.customerDisplayEpson)]
    public class CustomerDisplay : CustomDisplayDevice
    {
        private const int DISPLAY_WIDTH = 20;

        private const int DEF_CODE_PAGE = 866;

        protected virtual int CodePage
        {
            get { return DEF_CODE_PAGE; }
        }

        public override void SaveToEEPROM()
        {
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
                    Locate((byte)(lineNumber + 1));
                    if (!ErrorCode.Succeeded)
                        return;

                    byte[] nLine = Encoding.GetEncoding(CodePage).GetBytes(value.PadRight(DISPLAY_WIDTH, ' '));
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

        private void Locate(byte nLine)
        {
            byte[] nCmd = new byte[4];
            nCmd[0] = 0x1f;
            nCmd[1] = 0x24;
            nCmd[2] = 0x01;
            nCmd[3] = nLine;

            try
            {
                Port.Write(nCmd, 0, 4);
            }
            catch (TimeoutException)
            {
                ErrorCode = new ServerErrorCode(this, GeneralError.Timeout);
            }

        }
    }

    [Serializable]
    [CustomerDisplayAttribute(DeviceNames.customerDisplayEpson + " (855)")]
    public class CustomerDisplay855 : CustomerDisplay
    {
        protected override int CodePage
        {
            get { return 855; }
        }
    }
}
