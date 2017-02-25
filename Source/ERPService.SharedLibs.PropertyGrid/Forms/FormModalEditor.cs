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
    /// ������� ����� ��� ��������� ����������
    /// </summary>
    public partial class FormModalEditor : Form, IModalEditor
    {
        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        public FormModalEditor()
        {
            InitializeComponent();
        }

        #region ���������� IModalEditor

        /// <summary>
        /// ����������� ���������� ���������
        /// </summary>
        /// <param name="descriptorContext">�������� ��� ��������� �������������� ���������� � ��������</param>
        public virtual Boolean ShowEditor(ITypeDescriptorContext descriptorContext)
        {
            Text = descriptorContext.PropertyDescriptor.DisplayName;
            lblDescription.Text = descriptorContext.PropertyDescriptor.Description;
            return ShowDialog() == DialogResult.OK;
        }

        /// <summary>
        /// ������������� ��������
        /// </summary>
        public virtual Object Value
        {
            get { return null; }
            set { }
        }

        #endregion
    }
}