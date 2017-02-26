using System;
using System.Windows.Forms;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// Диалог для редактирования фильтра событий
    /// </summary>
    public partial class FormEventsFilterEdit : Form
    {
        private EventLinkFilterBase _filter;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        public FormEventsFilterEdit()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Редактирование фильтра событий
        /// </summary>
        /// <param name="filter">Текущее значение фильтра</param>
        /// <returns>Новое значение фильтра</returns>
        public bool Execute(EventLinkFilterBase filter)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");

            // работаем с копией фильтра
            _filter = (EventLinkFilterBase)filter.Clone();
            propertyGrid1.SelectedObject = _filter;
            return ShowDialog() == DialogResult.OK;
        }

        /// <summary>
        /// Новое значение фильтра
        /// </summary>
        public EventLinkFilterBase NewValue
        {
            get { return _filter; }
        }
    }
}