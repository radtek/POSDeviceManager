using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
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
                tbPassword.Text = value == null ? String.Empty : value.ToString();
                tbConfirm.Text = tbPassword.Text;
            }
        }

        private void FormPasswordEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = (DialogResult == DialogResult.OK && 
                String.Compare(tbPassword.Text, tbConfirm.Text, false) != 0);
            if (e.Cancel)
            {
                MessageBox.Show("��������� �������� ������ �� ��������� � ��������������",
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

