using System;

namespace TsManager
{
    /// <summary>
    /// �������, ������� ������� �������� ���������� ������ ������ ����
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AMCSLogicAttrubute : Attribute
    {
        private string _amcsName;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="amcsName">������������ ����</param>
        public AMCSLogicAttrubute(string amcsName)
            : base()
        {
            _amcsName = amcsName;
        }

        /// <summary>
        /// ������������ ����
        /// </summary>
        public string AMCSName
        {
            get { return _amcsName; }
        }
    }
}
