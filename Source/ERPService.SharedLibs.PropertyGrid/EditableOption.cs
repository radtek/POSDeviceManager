using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.PropertyGrid
{
    /// <summary>
    /// ����� ��� ��������������
    /// </summary>
    /// <typeparam name="T">���, � ������� �������� ����� (������, ������������ � �.�.)</typeparam>
    public class EditableOption<T>
    {
        private String _displayName;
        private T _keyword;

        /// <summary>
        /// ������������ ���
        /// </summary>
        public String DisplayName
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
        public EditableOption(String displayName, T keyword)
        {
            _displayName = displayName;
            _keyword = keyword;
        }

        /// <summary>
        /// ��������� ������������� �������
        /// </summary>
        public override String ToString()
        {
            return DisplayName;
        }
    }
}
