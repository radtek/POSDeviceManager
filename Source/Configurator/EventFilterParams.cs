using System;
using System.Collections.Generic;
using System.Text;
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

        public override String[] GetAvailableEventSources()
        {
            return new String[] { "Диспетчер устройств", "Tracking handler" };
        }
    }
}
