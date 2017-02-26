namespace ERPService.SharedLibs.PropertyGrid
{
    /// <summary>
    /// ����� ��� ��������������
    /// </summary>
    /// <typeparam name="T">���, � ������� �������� ����� (������, ������������ � �.�.)</typeparam>
    public class EditableOption<T>
    {
        private string _displayName;
        private T _keyword;

        /// <summary>
        /// ������������ ���
        /// </summary>
        public string DisplayName
        {
            get { return _displayName; }
        }

        /// <summary>
        /// �������� ��� ������/������
        /// </summary>
        public T Keyword
        {
            get { return _keyword; }
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="displayName">������������ ���</param>
        /// <param name="keyword">�������� ��� ������/������</param>
        public EditableOption(string displayName, T keyword)
        {
            _displayName = displayName;
            _keyword = keyword;
        }

        /// <summary>
        /// ��������� ������������� �������
        /// </summary>
        public override string ToString()
        {
            return DisplayName;
        }
    }
}
