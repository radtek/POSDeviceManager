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

        private void ThrowArgException(string connStr)
        {
            throw new ArgumentException(
                string.Format("�������� ������ ����� �����������: {0}", connStr));
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="connStr">������ �����������</param>
        public ConnStrHelper(string connStr)
        {
            if (string.IsNullOrEmpty(connStr))
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
        public string this[Int32 itemNo]
        {
            get
            {
                return _connStrMatch.Groups[itemNo].Value;
            }
        }
    }
}
