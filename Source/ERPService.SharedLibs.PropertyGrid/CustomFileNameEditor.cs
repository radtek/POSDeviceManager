using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ERPService.SharedLibs.PropertyGrid
{
    /// <summary>
    /// ������� ����� ��� ���������� �������, ���������� ������� ������
    /// </summary>
    public class CustomFileNameEditor : CustomEditor
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
            string fileName = (string)value;
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = DialogTitle;

                StringBuilder sb = new StringBuilder();
                foreach (FileType fileType in SupportedFileTypes)
                {
                    if (sb.Length != 0)
                        sb.Append('|');

                    sb.Append(fileType.ToString());
                }
                dialog.Filter = sb.ToString();

                if (!string.IsNullOrEmpty(fileName))
                    dialog.InitialDirectory = Path.GetDirectoryName(fileName);
                
                dialog.CheckFileExists = CheckFileExists;
                dialog.CheckPathExists = CheckPathExists;
                dialog.Multiselect = false;

                // ���������� ������ ������ �����
                if (dialog.ShowDialog() == DialogResult.OK)
                    value = dialog.FileName;
            }
            return value;
        }

        /// <summary>
        /// ��������� ������� ������ �����
        /// </summary>
        protected virtual string DialogTitle
        {
            get { return "������� ����"; }
        }

        /// <summary>
        /// ���������, ���������� �� ��������� ����
        /// </summary>
        protected virtual bool CheckFileExists
        {
            get { return false; }
        }

        /// <summary>
        /// ���������, ���������� �� ��������� ����
        /// </summary>
        protected virtual bool CheckPathExists
        {
            get { return true; }
        }

        /// <summary>
        /// �������������� ���� ������
        /// </summary>
        protected virtual FileType[] SupportedFileTypes 
        {
            get
            {
                return new FileType[] { new FileType("��� �����", "*") };
            }
        }
    }
}
