using System;
using System.Collections.Generic;
using System.Text;

namespace TsManager
{
    /// <summary>
    /// �������, ������� ������� �������� ���������� ������ ������ ����
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AMCSLogicAttrubute : Attribute
    {
        private String _amcsName;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="amcsName">������������ ����</param>
        public AMCSLogicAttrubute(String amcsName)
            : base()
        {
            _amcsName = amcsName;
        }

        /// <summary>
        /// ������������ ����
        /// </summary>
        public String AMCSName
        {
            get { return _amcsName; }
        }
    }
}
