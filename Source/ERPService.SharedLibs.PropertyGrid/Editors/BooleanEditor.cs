using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.PropertyGrid.Editors
{
    /// <summary>
    /// �������� �������� ���� System.Boolean
    /// </summary>
    [Obsolete]
    public class BooleanEditor : CustomDropdownEditor
    {
        /// <summary>
        /// ���������� ������ ��������� �������� ��������
        /// </summary>
        public override string[] Values
        {
            get { return new string[] { "��", "���" }; }
        }

        /// <summary>
        /// ���������� ������ ���������� ��������
        /// </summary>
        /// <param name="value">�������� �������� ��������</param>
        protected override int ObjectToIndex(Object value)
        {
            return (bool)value ? 0 : 1;
        }

        /// <summary>
        /// ������������ ��������� ��������� �������� � ������ ���
        /// </summary>
        /// <param name="selectedIndex">����� ������ � ������</param>
        protected override Object IndexToObject(int selectedIndex)
        {
            return selectedIndex == 0;
        }
    }
}
