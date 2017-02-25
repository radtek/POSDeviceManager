using System;
using System.Collections.Generic;
using System.Text;
using ERPService.SharedLibs.PropertyGrid;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// Редактор для источников событий
    /// </summary>
    public class EventSourcesEditor : CustomOptionsEditor<String>
    {
        /// <summary>
        /// Список доступных источников событий
        /// </summary>
        public override EditableOption<String>[] Options
        {
            get 
            {
                List<EditableOption<String>> options = new List<EditableOption<String>>();
                foreach (String eventSource in 
                    ((EventLinkFilterBase)DescriptorContext.Instance).GetAvailableEventSources())
                {
                    options.Add(new EditableOption<String>(eventSource, eventSource));
                }
                return options.ToArray();
            }
        }
        }
}
