using System;
using System.Text;
using System.Windows.Forms;

namespace ERPService.SharedLibs.Eventlog
{
    public partial class FormEventDetails : Form
    {
        private IEventsViewLink _viewLink;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        public FormEventDetails()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Выполнение диалога
        /// </summary>
        /// <param name="viewLink">Интерфейс управления перемещением по списку событий</param>
        public void Execute(IEventsViewLink viewLink)
        {
            _viewLink = viewLink;

            // инициализируем элементы управления
            InitializeControls(_viewLink.Current);
            // показываем окно
            ShowDialog();
        }

        /// <summary>
        /// Инициализация элементов управления данными записи
        /// </summary>
        /// <param name="eventRecord">Ссылка на интерфейс записи</param>
        private void InitializeControls(EventRecord eventRecord)
        {
            // проверяем валидность интерфейса
            if (eventRecord == null)
                throw new ArgumentNullException("eventRecord");

            // дата и время
            lblDateTime.Text = string.Format("Дата и время: {0}", 
                eventRecord.Timestamp.ToString("dd MMM yyyy  HH:mm:ss"));
            // источник события
            lblSource.Text = string.Format("Приложение: {0}", eventRecord.Source);
            // тип события
            lblEventType.Text = string.Format("Тип: {0}", 
                EventTypeConvertor.ConvertFrom(eventRecord.EventType));
            // текст события
            tbText.Lines = eventRecord.Text.ToArray();
            tbText.SelectionStart = 0;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            // переходим к следующей записи
            InitializeControls(_viewLink.NextEvent());
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            // переходим к предыдущей записи
            InitializeControls(_viewLink.PreviousEvent());
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(lblDateTime.Text);
            sb.AppendLine(lblSource.Text);
            sb.AppendLine(lblEventType.Text);
            sb.AppendLine();
            sb.AppendLine(tbText.Text);

            Clipboard.SetData(DataFormats.UnicodeText, sb.ToString());
        }
    }
}