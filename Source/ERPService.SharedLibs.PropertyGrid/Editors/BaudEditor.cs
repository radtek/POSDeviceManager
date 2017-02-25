using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.PropertyGrid.Editors
{
    /// <summary>
    /// �������� �������� �������-��������� �������� ������
    /// </summary>
    public class BaudEditor : CustomDropdownEditor
    {
        /// <summary>
        /// ���������� ������ ��������� �������� ��������
        /// </summary>
        public override String[] Values
        {
            get
            {
                return new String[] { 
                    "4800", 
                    "9600",
                    "19200",
                    "38400",
                    "57600",
                    "115200"
                };
            }
        }

        /// <summary>
        /// ���������� ������ ���������� ��������
        /// </summary>
        /// <param name="value">�������� �������� ��������</param>
        protected override Int32 ObjectToIndex(Object value)
        {
            return Array.IndexOf<String>(Values, value.ToString());
        }

        /// <summary>
        /// ������������ ��������� ��������� �������� � ������ ���
        /// </summary>
        /// <param name="selectedIndex">����� ������ � ������</param>
        protected override Object IndexToObject(Int32 selectedIndex)
        {
            return Convert.ToInt32(Values[selectedIndex]);
        }
    }
}
