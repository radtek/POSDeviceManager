using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.Helpers
{
    /// <summary>
    /// ������ "� ���������"
    /// </summary>
    public class AboutBox
    {
        private FormAbout _formAbout;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        public AboutBox()
        {
            _formAbout = new FormAbout();
        }

        /// <summary>
        /// ��������� ���������� � ����������
        /// </summary>
        /// <param name="componentName">������������ ����������</param>
        /// <param name="componentVersion">������ ����������</param>
        public virtual void AppendComponentInfo(String componentName, String componentVersion)
        {
            _formAbout.AppendComponentInfo(componentName, componentVersion);
        }

        /// <summary>
        /// ���������� ������ "� ���������"
        /// </summary>
        /// <param name="showComponents">���������� ���������� � ������ �����������</param>
        public void Show(Boolean showComponents)
        {
            _formAbout.ShowDialog(showComponents);
        }

        /// <summary>
        /// ������������ ������������ ��������
        /// </summary>
        public String ProductName
        {
            get { return _formAbout.AppProductName; }
            set { _formAbout.AppProductName = value; }
        }

        /// <summary>
        /// ������ ������������ ��������
        /// </summary>
        public String Version
        {
            get { return _formAbout.AppProductVersion; }
            set { _formAbout.AppProductVersion = value; }
        }

        /// <summary>
        /// ��� ���������� ����� ��� �����������
        /// </summary>
        public Int32 CopyrightYear
        {
            get { return _formAbout.CopyrightYear; }
            set { _formAbout.CopyrightYear = value; }
        }

        /// <summary>
        /// ������������ ����������
        /// </summary>
        public String AppName
        {
            get { return _formAbout.AppName; }
            set { _formAbout.AppName = value; }
        }
    }
}
