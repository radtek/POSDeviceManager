using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.PropertyGrid.Converters
{
    /// <summary>
    /// ��������� ��� <see cref="System.Boolean"/>
    /// </summary>
    public class RussianBooleanConverter : CustomEnumConverter
    {
        /// <summary>
        /// ����� ��������� ��������, �������������� ��������� ������������
        /// </summary>
        protected override string[] StringValues
        {
            get 
            {
                return new string[] { "��", "���" };
            }
        }

        /// <summary>
        /// ����� �������� ��������� ������������
        /// </summary>
        protected override Object[] ObjectValues
        {
            get 
            {
                return new Object[] { true, false };
            }
        }
    }
}
