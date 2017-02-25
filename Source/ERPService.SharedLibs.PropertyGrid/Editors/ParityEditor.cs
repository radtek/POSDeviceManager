using System;
using System.Collections.Generic;
using System.Text;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace ERPService.SharedLibs.PropertyGrid.Editors
{
    /// <summary>
    /// �������� �������� �������� ��������
    /// </summary>
    public class ParityEditor : CustomDropdownEditor
    {
        /// <summary>
        /// ���������� ������ ��������� �������� ��������
        /// </summary>
        public override String[] Values
        {
            get
            {
                return new String[] { 
                    "���", 
                    "��������",
                    "������",
                    "�� ���������� ���� ��������",
                    "�� �������� ���� ��������"
                };
            }
        }

        /// <summary>
        /// ���������� ������ ���������� ��������
        /// </summary>
        /// <param name="value">�������� �������� ��������</param>
        protected override Int32 ObjectToIndex(Object value)
        {
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// ������������ ��������� ��������� �������� � ������ ���
        /// </summary>
        /// <param name="selectedIndex">����� ������ � ������</param>
        protected override Object IndexToObject(Int32 selectedIndex)
        {
            return (Parity)selectedIndex;
        }
    }
}
