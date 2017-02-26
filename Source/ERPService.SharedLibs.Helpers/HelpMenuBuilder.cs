using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using ERPService.SharedLibs.Helpers.Properties;

namespace ERPService.SharedLibs.Helpers
{
    /// <summary>
    /// ��������������� ����� ��� ����������� ���� "�������"
    /// </summary>
    public class HelpMenuBuilder : AboutBox
    {
        #region ����

        private string _svnSubDirectory;
        private bool _showComponents;

        #endregion

        #region �������� �������� � ������

        /// <summary>
        /// ������� �� �������� ��������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnHomeClick(Object sender, EventArgs args)
        {
            Process.Start("http://www.erpservice.ru");
        }

        /// <summary>
        /// ������� � ���������� ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnUpdatesClick(Object sender, EventArgs args)
        {
            Process.Start(string.Format("http://www.erpservice.ru/svn/{0}", _svnSubDirectory));
        }

        /// <summary>
        /// ����� ������� "� ���������"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnAboutClick(Object sender, EventArgs args)
        {
            Show(_showComponents);
        }

        #endregion

        #region �����������

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="form">�����, � ������� ����� �������� ����</param>
        /// <param name="svnSubDirectory">���������� � ������� �������� ������ ��� ������������ ��������</param>
        public HelpMenuBuilder(Form form, string svnSubDirectory)
            : base()
        {
            if (form == null)
                throw new ArgumentNullException("form");
            if (string.IsNullOrEmpty(svnSubDirectory))
                throw new ArgumentNullException("svnSubDirectory");
            if (form.MainMenuStrip == null)
                throw new InvalidOperationException("����� �� �������� �������� ����");

            // ������� ����
            ToolStripMenuItem help = new ToolStripMenuItem("�������");
            help.DropDownItems.Add(new ToolStripMenuItem("�������� ��������", Resources.home.ToBitmap(),
                OnHomeClick));
            help.DropDownItems.Add(new ToolStripMenuItem("����������", null, OnUpdatesClick));
            help.DropDownItems.Add(new ToolStripSeparator());
            help.DropDownItems.Add(new ToolStripMenuItem("� ���������", Resources.help.ToBitmap(), 
                OnAboutClick));

            // ���������� ��� � ������� ���� �����
            form.MainMenuStrip.Items.Add(help);

            _svnSubDirectory = svnSubDirectory;
        }

        #endregion

        #region �������� �������� � ������

        /// <summary>
        /// ��������� ���������� � ����������
        /// </summary>
        /// <param name="componentName">������������ ����������</param>
        /// <param name="componentVersion">������ ����������</param>
        public override void AppendComponentInfo(string componentName, string componentVersion)
        {
            _showComponents = true;
            base.AppendComponentInfo(componentName, componentVersion);
        }

        #endregion
    }
}
