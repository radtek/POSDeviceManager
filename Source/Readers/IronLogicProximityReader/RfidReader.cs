using System.Text.RegularExpressions;
using DevicesBase;
using DevicesCommon.Helpers;

namespace IronLogicProximityReader
{
    /// <summary>
    /// RFID-считыватель Iron Logic
    /// </summary>
    [GenericReader(DeviceNames.ironLogicRFIDReader)]
    public class RfidReader : CustomGenericReader
    {
        private const string _rfidMask = 
            @"(?:\w*\u002D\w*)(?:\u005B\w+\u005D)(?:\s*)(\d+\u002C\d+)";

        protected override string Prepare(string rawData)
        {
            Match match = Regex.Match(rawData, _rfidMask);
            return match.Success ? match.Groups[1].Value : string.Empty;
        }
    }
}
