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
    /// Диалоговое окно "О программе"
    /// </summary>
    internal partial class FormAbout : Form
    {
        private String _appProductName;
        private String _appProductVersion;
        private String _appName;
        private Int32 _copyrightYear;
        private List<KeyValuePair<String, String>> _componentVersions;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        public FormAbout()
        {
            InitializeComponent();
            _componentVersions = new List<KeyValuePair<String, String>>();
            _copyrightYear = 2008;
            _appProductName = "Программный продукт";
            _appProductVersion = "Версия";
        }

        /// <summary>
        /// Показывает диалог "О программе"
        /// </summary>
        /// <param name="showComponents">Отображать информацию о версии компонентов</param>
        public void ShowDialog(Boolean showComponents)
        {
            labelProductName.Text = _appProductName;
            labelAppName.Text = _appName;
            labelProductVersion.Text = String.Format("Версия: {0}", _appProductVersion);
            labelCopyright.Text = String.Format("© ERP Service, {0}", _copyrightYear);

            if (showComponents)
                listView1.Select();
            else
            {
                if (this.Height == 347)
                {
                    // не даем уменьшать размеры формы больше одного раза
                    label3.Visible = false;
                    listView1.Visible = false;
                    this.Height -= 120;
                }
            }
            ShowDialog();
        }

        /// <summary>
        /// Имя приложения
        /// </summary>
        public String AppName
        {
            get { return _appName; }
            set { _appName = value; }
        }

        /// <summary>
        /// Год авторского права (либо модификации)
        /// </summary>
        public Int32 CopyrightYear
        {
            get { return _copyrightYear; }
            set { _copyrightYear = value; }
        }

        /// <summary>
        /// Версия продукта
        /// </summary>
        public String AppProductVersion
        {
            get { return _appProductVersion; }
            set { _appProductVersion = value; }
        }

        /// <summary>
        /// Имя продукта
        /// </summary>
        public String AppProductName
        {
            get { return _appProductName; }
            set { _appProductName = value; }
        }

        /// <summary>
        /// Добавляет информацию о версии компонента
        /// </summary>
        /// <param name="componentName">Имя компонента</param>
        /// <param name="componentVersion">Версия компонента</param>
        public void AppendComponentInfo(String componentName, String componentVersion)
        {
            ListViewItem item = new ListViewItem(componentName);
            item.SubItems.Add(componentVersion);
            listView1.Items.Add(item);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.erpservice.ru");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormContacts formContacts = new FormContacts();
            formContacts.AppProductName = _appProductName;
            formContacts.AppProductVersion = _appProductVersion;
            formContacts.ShowDialog();
        }


    }
}