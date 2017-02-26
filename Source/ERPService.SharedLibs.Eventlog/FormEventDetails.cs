using System;
using System.Text;
using System.Windows.Forms;

namespace ERPService.SharedLibs.Eventlog
{
    public partial class FormEventDetails : Form
    {
        private IEventsViewLink _viewLink;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        public FormEventDetails()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ���������� �������
        /// </summary>
        /// <param name="viewLink">��������� ���������� ������������ �� ������ �������</param>
        public void Execute(IEventsViewLink viewLink)
        {
            _viewLink = viewLink;

            // �������������� �������� ����������
            InitializeControls(_viewLink.Current);
            // ���������� ����
            ShowDialog();
        }

        /// <summary>
        /// ������������� ��������� ���������� ������� ������
        /// </summary>
        /// <param name="eventRecord">������ �� ��������� ������</param>
        private void InitializeControls(EventRecord eventRecord)
        {
            // ��������� ���������� ����������
            if (eventRecord == null)
                throw new ArgumentNullException("eventRecord");

            // ���� � �����
            lblDateTime.Text = string.Format("���� � �����: {0}", 
                eventRecord.Timestamp.ToString("dd MMM yyyy  HH:mm:ss"));
            // �������� �������
            lblSource.Text = string.Format("����������: {0}", eventRecord.Source);
            // ��� �������
            lblEventType.Text = string.Format("���: {0}", 
                EventTypeConvertor.ConvertFrom(eventRecord.EventType));
            // ����� �������
            tbText.Lines = eventRecord.Text.ToArray();
            tbText.SelectionStart = 0;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            // ��������� � ��������� ������
            InitializeControls(_viewLink.NextEvent());
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            // ��������� � ���������� ������
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