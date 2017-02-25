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
        public override String[] Values
        {
            get { return new String[] { "��", "���" }; }
        }

        /// <summary>
        /// ���������� ������ ���������� ��������
        /// </summary>
        /// <param name="value">�������� �������� ��������</param>
        protected override Int32 ObjectToIndex(Object value)
        {
            return (Boolean)value ? 0 : 1;
        }

        /// <summary>
        /// ������������ ��������� ��������� �������� � ������ ���
        /// </summary>
        /// <param name="selectedIndex">����� ������ � ������</param>
        protected override Object IndexToObject(Int32 selectedIndex)
        {
            return selectedIndex == 0;
        }
    }
}
