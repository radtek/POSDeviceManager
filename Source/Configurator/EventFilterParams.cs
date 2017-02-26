using System;
using ERPService.SharedLibs.Eventlog;

namespace DevmanConfig
{
    public class EventFilterParams : EventLinkFilterBase
    {
        public override Object Clone()
        {
            var clone = new EventFilterParams();
            clone.Assign(this);
            return clone;
        }

        public override string[] GetAvailableEventSources()
        {
            return new string[] { "Диспетчер устройств", "Tracking handler" };
        }
    }
}
