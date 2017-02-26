using System;
using System.Collections.Generic;
using System.Text;
using ERPService.SharedLibs.Eventlog;

namespace TsManagerConfigurator
{
    public class TsManagerConfiguratorSettings
    {
        public FilterSettings FilterSettings { get; set; }
        public int Splitter1 { get; set; }
        public int Splitter2 { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public ListedEventsViewSettings LogColumns { get; set; }

        public TsManagerConfiguratorSettings()
        {
            FilterSettings = new FilterSettings();
            LogColumns = new ListedEventsViewSettings();
            Splitter1 = 235;
            Splitter2 = 240;
            Left = 10;
            Top = 10;
            Width = 750;
            Height = 520;
        }
    }
}
