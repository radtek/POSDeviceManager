using System;
using System.Collections.Generic;
using System.Text;
using ERPService.SharedLibs.Eventlog;
using TsManager;

namespace TsManagerConfigurator
{
    public class FilterSettings : EventLinkFilterBase
    {
        public override object Clone()
        {
            var clone = new FilterSettings();
            clone.Assign(this);
            return clone;
        }

        public override string[] GetAvailableEventSources()
        {
            return new string[] { TsGlobalConst.EventSource };
        }
    }
}
