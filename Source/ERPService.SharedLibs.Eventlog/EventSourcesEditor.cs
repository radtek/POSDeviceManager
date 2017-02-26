using System.Collections.Generic;
using ERPService.SharedLibs.PropertyGrid;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// �������� ��� ���������� �������
    /// </summary>
    public class EventSourcesEditor : CustomOptionsEditor<string>
    {
        /// <summary>
        /// ������ ��������� ���������� �������
        /// </summary>
        public override EditableOption<string>[] Options
        {
            get 
            {
                List<EditableOption<string>> options = new List<EditableOption<string>>();
                foreach (string eventSource in 
                    ((EventLinkFilterBase)DescriptorContext.Instance).GetAvailableEventSources())
                {
                    options.Add(new EditableOption<string>(eventSource, eventSource));
                }
                return options.ToArray();
            }
        }
        }
}
