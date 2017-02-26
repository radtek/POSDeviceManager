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
        public override string[] Values
        {
            get
            {
                return new string[] { 
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
        protected override int ObjectToIndex(Object value)
        {
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// ������������ ��������� ��������� �������� � ������ ���
        /// </summary>
        /// <param name="selectedIndex">����� ������ � ������</param>
        protected override Object IndexToObject(int selectedIndex)
        {
            return (Parity)selectedIndex;
        }
    }
}
