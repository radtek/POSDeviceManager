using System;
using System.Windows.Forms;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// ������ ��� �������������� ������� �������
    /// </summary>
    public partial class FormEventsFilterEdit : Form
    {
        private EventLinkFilterBase _filter;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        public FormEventsFilterEdit()
        {
            InitializeComponent();
        }

        /// <summary>
        /// �������������� ������� �������
        /// </summary>
        /// <param name="filter">������� �������� �������</param>
        /// <returns>����� �������� �������</returns>
        public bool Execute(EventLinkFilterBase filter)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");

            // �������� � ������ �������
            _filter = (EventLinkFilterBase)filter.Clone();
            propertyGrid1.SelectedObject = _filter;
            return ShowDialog() == DialogResult.OK;
        }

        /// <summary>
        /// ����� �������� �������
        /// </summary>
        public EventLinkFilterBase NewValue
        {
            get { return _filter; }
        }
    }
}