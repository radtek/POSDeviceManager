using System;
using System.Windows.Forms;

namespace ERPService.SharedLibs.PropertyGrid.Forms
{
    /// <summary>
    /// ������ ��� �������������� �������
    /// </summary>
    public partial class FormPasswordEditor : ERPService.SharedLibs.PropertyGrid.Forms.FormModalEditor
    {
        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        public FormPasswordEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ������������� ��������
        /// </summary>
        public override Object Value
        {
            get
            {
                return tbPassword.Text;
            }
            set
            {
                tbPassword.Text = value == null ? string.Empty : value.ToString();
                tbConfirm.Text = tbPassword.Text;
            }
        }

        private void FormPasswordEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = (DialogResult == DialogResult.OK &&
                string.Compare(tbPassword.Text, tbConfirm.Text, false) != 0);
            if (e.Cancel)
            {
                MessageBox.Show("��������� �������� ������ �� ��������� � ��������������",
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

