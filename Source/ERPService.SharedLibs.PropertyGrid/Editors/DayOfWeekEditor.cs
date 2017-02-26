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
        public override string[] Values
        {
            get
            {
                return new string[] {
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
        protected override int ObjectToIndex(Object value)
        {
            return (int)value - 1;
        }

        /// <summary>
        /// ������������ ��������� ��������� �������� � ������ ���
        /// </summary>
        /// <param name="selectedIndex">����� ������ � ������</param>
        protected override Object IndexToObject(int selectedIndex)
        {
            return selectedIndex + 1;
        }
    }
}
