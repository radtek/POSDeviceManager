namespace ERPService.SharedLibs.PropertyGrid
{
    /// <summary>
    /// ��������������� ����� ��� �������� ���� ������ � ������� ��������������
    /// �������, ���������� ������� ������
    /// </summary>
    public class FileType
    {
        private string _description;
        private string _extension;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="description">�������� ���� ������</param>
        /// <param name="extension">����������, �������������� ���� ������</param>
        public FileType(string description, string extension)
        {
            _description = description;
            _extension = extension;
        }

        /// <summary>
        /// �������� ���� ������
        /// </summary>
        public string Descpription
        {
            get { return _description; }
        }

        /// <summary>
        /// ����������, �������������� ���� ������
        /// </summary>
        public string Extension
        {
            get { return _extension; }
        }

        /// <summary>
        /// ��������� ������������� �������
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0} (*.{1})|*.{1}", _description, _extension);
        }
    }
}
