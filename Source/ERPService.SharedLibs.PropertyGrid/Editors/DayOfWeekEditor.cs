using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.PropertyGrid.Editors
{
    /// <summary>
    /// �������� �������-���� ������
    /// </summary>
    public class DayOfWeekEditor : CustomDropdownEditor
    {
        /// <summary>
        /// ���������� ������ ��������� �������� ��������
        /// </summary>
        public override String[] Values
        {
            get
            {
                return new String[] {
                    "�����������",
                    "�������",
                    "�����",
                    "�������",
                    "�������",
                    "�������",
                    "�����������"};
            }
        }

        /// <summary>
        /// ���������� ������ ���������� ��������
        /// </summary>
        /// <param name="value">�������� �������� ��������</param>
        protected override Int32 ObjectToIndex(Object value)
        {
            return (Int32)value - 1;
        }

        /// <summary>
        /// ������������ ��������� ��������� �������� � ������ ���
        /// </summary>
        /// <param name="selectedIndex">����� ������ � ������</param>
        protected override Object IndexToObject(Int32 selectedIndex)
        {
            return selectedIndex + 1;
        }
    }
}
