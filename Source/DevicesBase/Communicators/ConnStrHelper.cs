using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DevicesBase.Communicators
{
    /// <summary>
    /// ��������������� ����� ��� ������� ��������� ������ �����������
    /// </summary>
    public sealed class ConnStrHelper
    {
        private Match _connStrMatch;

        private void ThrowArgException(String connStr)
        {
            throw new ArgumentException(
                String.Format("�������� ������ ����� �����������: {0}", connStr));
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="connStr">������ �����������</param>
        public ConnStrHelper(String connStr)
        {
            if (String.IsNullOrEmpty(connStr))
                ThrowArgException(connStr);

            Regex cstr = new Regex(@"(?<Protocol>\w+):\/\/(?<Host>[\w.]+\/?):(?<Port>\d+)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

            _connStrMatch = cstr.Match(connStr);
            if (!_connStrMatch.Success)
                ThrowArgException(connStr);
        }

        /// <summary>
        /// ���������� ������� ������ ����������� �� ������
        /// </summary>
        /// <param name="itemNo">����� �������� ������ �����������</param>
        public String this[Int32 itemNo]
        {
            get
            {
                return _connStrMatch.Groups[itemNo].Value;
            }
        }
    }
}
