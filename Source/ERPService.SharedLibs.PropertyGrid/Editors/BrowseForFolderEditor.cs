using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Design;

namespace ERPService.SharedLibs.PropertyGrid.Editors
{
    /// <summary>
    /// �������� ��� �������-���� �����
    /// </summary>
    public class BrowseForFolderEditor : CustomEditor
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
        /// �������������� ��������
        /// </summary>
        /// <param name="value">�������� ��������</param>
        protected override Object OnEdit(Object value)
        {
            string selectedPath = (string)value;
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = Description;
                dialog.RootFolder = Root;
                dialog.ShowNewFolderButton = NewFolder;
                if (!string.IsNullOrEmpty(selectedPath))
                    dialog.SelectedPath = selectedPath;

                if (dialog.ShowDialog() == DialogResult.OK)
                    value = dialog.SelectedPath;
            }
            return value;
        }

        /// <summary>
        /// ��������� ������� ������ �����
        /// </summary>
        protected virtual string Description
        {
            get { return "�������� �����"; }
        }

        /// <summary>
        /// ��������� ��������� ����� �����
        /// </summary>
        protected virtual bool NewFolder
        {
            get { return true; }
        }

        /// <summary>
        /// �������� �����
        /// </summary>
        protected virtual Environment.SpecialFolder Root
        {
            get { return Environment.SpecialFolder.MyComputer; }
        }
    }
}
