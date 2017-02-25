using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using DevicesCommon;
using DevicesCommon.Helpers;
using DevicesBase;
using ERPService.SharedLibs.Helpers;

namespace IronLogicProximityReader
{
    /// <summary>
    /// RFID-считыватель Iron Logic
    /// </summary>
    [GenericReader(DeviceNames.ironLogicRFIDReader)]
    public class RfidReader : CustomGenericReader
    {
        private const String _rfidMask = 
            @"(?:\w*\u002D\w*)(?:\u005B\w+\u005D)(?:\s*)(\d+\u002C\d+)";

        protected override String Prepare(String rawData)
        {
            Match match = Regex.Match(rawData, _rfidMask);
            return match.Success ? match.Groups[1].Value : String.Empty;
        }
    }
}
