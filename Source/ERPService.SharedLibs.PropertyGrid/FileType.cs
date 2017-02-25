using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.PropertyGrid
{
    /// <summary>
    /// ��������������� ����� ��� �������� ���� ������ � ������� ��������������
    /// �������, ���������� ������� ������
    /// </summary>
    public class FileType
    {
        private String _description;
        private String _extension;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="description">�������� ���� ������</param>
        /// <param name="extension">����������, �������������� ���� ������</param>
        public FileType(String description, String extension)
        {
            _description = description;
            _extension = extension;
        }

        /// <summary>
        /// �������� ���� ������
        /// </summary>
        public String Descpription
        {
            get { return _description; }
        }

        /// <summary>
        /// ����������, �������������� ���� ������
        /// </summary>
        public String Extension
        {
            get { return _extension; }
        }

        /// <summary>
        /// ��������� ������������� �������
        /// </summary>
        public override String ToString()
        {
            return String.Format("{0} (*.{1})|*.{1}", _description, _extension);
        }
    }
}
