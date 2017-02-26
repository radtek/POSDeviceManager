using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.Remoting
{
    /// <summary>
    /// ������� ����� ��� ��������, ��������������� ��� �������� �� ���������-�������
    /// </summary>
    public abstract class HostingTarget : MarshalByRefObject, IDisposable
    {
        /// <summary>
        /// "���" �������, ������������ ��� ������������ URI �������
        /// </summary>
        /// <example>http://127.0.0.1/someObject</example>
        public abstract string Name { get; }

        /// <summary>
        /// ����, �� ������� ����� �������� ������
        /// </summary>
        public abstract int Port { get; }

        #region ���������� IDisposable Members

        /// <summary>
        /// �������� �� ������������ ��������� ��������
        /// </summary>
        public virtual void Dispose()
        {
        }

        #endregion
    }
}
