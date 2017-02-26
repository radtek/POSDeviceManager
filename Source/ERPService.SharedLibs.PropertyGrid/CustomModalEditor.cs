using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace ERPService.SharedLibs.PropertyGrid
{
    /// <summary>
    /// ������� ����� ��� ���������� �������, ������������ ���������� ����
    /// </summary>
    public abstract class CustomModalEditor : CustomEditor
    {
        /// <summary>
        /// Gets the editor style used by the EditValue method
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that can be used to gain additional context information</param>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            // ������������� ����� ������������ � ���������� ����
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        /// ���������� ���������� �������� ��������
        /// </summary>
        /// <param name="value">�������� ��������</param>
        protected override Object OnEdit(Object value)
        {
            // �������� ������ �� ��������� ���������
            IModalEditor editor = GetEditor();
            // �������������� �������� �������������� ��������
            editor.Value = value;
            // ���������� ������
            if (editor.ShowEditor(DescriptorContext))
            {
                // ������ �������� ��������
                value = editor.Value;
            }
            // ���������� �������� ��������
            return value;
        }

        /// <summary>
        /// ���������� ������ �� ��������� ���������� ���������
        /// </summary>
        abstract protected IModalEditor GetEditor();
    }
}
