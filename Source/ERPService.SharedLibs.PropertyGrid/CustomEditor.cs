using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;

namespace ERPService.SharedLibs.PropertyGrid
{
    /// <summary>
    /// ������� ����� ��� ���� ���������� �������
    /// </summary>
    public abstract class CustomEditor : UITypeEditor
    {
        // ������ �� ��������� ��� ����������� ��������� ���������� 
        // � property grid
        private IWindowsFormsEditorService _edSvc;
        // ������ �� �������� �������� ��������
        private ITypeDescriptorContext _descriptorContext;
        // ������������� ��������
        private Object _value;

        /// <summary>
        /// Edits the value of the specified object using the editor style indicated by the GetEditStyle method
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that can be used to gain additional context information</param>
        /// <param name="provider">An IServiceProvider that this editor can use to obtain services</param>
        /// <param name="value">The object to edit</param>
        public override Object EditValue(ITypeDescriptorContext context, IServiceProvider provider,
            Object value)
        {
            // �������� ������������ ��� ������� �� �����������
            _value = value;
            try
            {
                // �������� ������ �� ��������� Windows Forms
                _edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                // �������������� ������ �� �������� �������������� ��������
                _descriptorContext = context;

                // �������� ����� ��� ��������������, ������������� � �������
                return _edSvc == null ? value : OnEdit(value);
            }
            finally
            {
                _value = null;
            }
        }

        /// <summary>
        /// ���������� ������ �� ��������� ��� ����������� ��������� ����������
        /// </summary>
        protected IWindowsFormsEditorService EdSvc
        {
            get { return _edSvc; }
        }

        /// <summary>
        /// ���������� �������� �������� �������������� ��������
        /// </summary>
        protected ITypeDescriptorContext DescriptorContext
        {
            get { return _descriptorContext; }
        }

        /// <summary>
        /// ������������� ��������
        /// </summary>
        protected Object Value
        {
            get { return _value; }
        }

        /// <summary>
        /// ���������� ���������� �������� ��������
        /// </summary>
        /// <param name="value">�������� ��������</param>
        abstract protected Object OnEdit(Object value);
    }
}
