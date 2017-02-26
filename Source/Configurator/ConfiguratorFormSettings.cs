using System.Drawing;
using ERPService.SharedLibs.Eventlog;

namespace DevmanConfig
{
    public class ConfiguratorFormSettings
    {
        public int ConfigWidth { get; set; }

        public int PropertiesHeight { get; set; }

        public int DetailedViewHeight { get; set; }

        public int Mode { get; set; }

        public EventFilterParams Filter { get; set; }

        public Size Size { get; set; }

        public Point Location { get; set; }

        public ListedEventsViewSettings Columns { get; set; }

        public ConfiguratorFormSettings()
        {
            Columns = new ListedEventsViewSettings();
            Location = new Point(200, 100);
            Size = new Size(800, 600);
            PropertiesHeight = 300;
            ConfigWidth = 300;
            Filter = new EventFilterParams();
            DetailedViewHeight = 380;
        }
    }
}
