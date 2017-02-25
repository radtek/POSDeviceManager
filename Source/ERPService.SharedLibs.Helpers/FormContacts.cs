using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace ERPService.SharedLibs.Helpers
{
    /// <summary>
    /// Контакты ERP Service
    /// </summary>
    internal partial class FormContacts : Form
    {
        private String _productName;
        private String _productVersion;

        /// <summary>
        /// Наименование продукта
        /// </summary>
        public String AppProductName
        {
            get { return _productName; }
            set { _productName = value; }
        }

        /// <summary>
        /// Версия продукта
        /// </summary>
        public String AppProductVersion
        {
            get { return _productVersion; }
            set { _productVersion = value; }
        }

        /// <summary>
        /// Создает экземпляр класса
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
            Process.Start(String.Format("mailto:support@erpservice.ru?subject={0}, версия {1}",
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