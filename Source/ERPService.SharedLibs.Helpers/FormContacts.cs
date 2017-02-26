using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ERPService.SharedLibs.Helpers
{
    /// <summary>
    /// �������� ERP Service
    /// </summary>
    internal partial class FormContacts : Form
    {
        private string _productName;
        private string _productVersion;

        /// <summary>
        /// ������������ ��������
        /// </summary>
        public string AppProductName
        {
            get { return _productName; }
            set { _productName = value; }
        }

        /// <summary>
        /// ������ ��������
        /// </summary>
        public string AppProductVersion
        {
            get { return _productVersion; }
            set { _productVersion = value; }
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        public FormContacts()
        {
            InitializeComponent();
        }

        private void label8_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.erpservice.ru");
        }

        private void label7_Click(object sender, EventArgs e)
        {
            Process.Start(string.Format("mailto:support@erpservice.ru?subject={0}, ������ {1}",
                _productName, _productVersion));
        }

        private void label13_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.erpservice.ru/svn");
        }

        private void label12_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.erpservice.ru/mantis");
        }
    }
}