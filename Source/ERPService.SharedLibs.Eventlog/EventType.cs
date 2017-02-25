using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// ���� ������� 
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// ��� �� ���������
        /// </summary>
        Undefined,

        /// <summary>
        /// ����������
        /// </summary>
        Information,

        /// <summary>
        /// ������
        /// </summary>
        Error,

        /// <summary>
        /// ��������������
        /// </summary>
        Warning
    }
}
