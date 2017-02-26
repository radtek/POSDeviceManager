using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using DevicesCommon;
using DevicesCommon.Helpers;

namespace DevicesBase.Helpers
{
    /// <summary>
    /// ��������� ��� ������
    /// </summary>
    [Serializable]
    public class ServerErrorCode : ErrorCode
    {
        #region ���������

        // ������ ��������� ��� ��������� ���������� ��������
        private const string noErrors = "������ ���";
        // ������ �������� ������, ������������� ��� ��������� ������ � �����������
        private const string specificError = "������ {0}. {1}";
        // ������ ��� �������� ���������� �� ����������
        private const string exceptionText = "���������� {0}. �����: \"{1}\". ����: \"{2}\"";

        #endregion

        #region ����

        private string _commandDump;

        #endregion

        #region ������������

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="value">����� ��� ������</param>
        /// <param name="description">�������� ������ ���� ������</param>
        /// <param name="specificValue">��� ������ ��������� ������ � �����������</param>
        /// <param name="specificDescription">�������� ���� ������</param>
        /// <param name="sender">����������-����������� ���� ������</param>
        /// <param name="commandDump"></param>
        public ServerErrorCode(IDevice sender, GeneralError value, string description,
            Int16 specificValue, string specificDescription, string commandDump)
            : base(sender.DeviceId, value, description, specificValue, specificDescription)
        {
            _commandDump = commandDump;
            if (Value == GeneralError.Success)
                return;

            // ��������� ������ � ����
            EventLogEntryType entryType = EventLogEntryType.Information;
            switch (Value)
            {
                // ������ ������
                case GeneralError.Busy:
                case GeneralError.Exception:
                case GeneralError.Specific:
                case GeneralError.Timeout:
                case GeneralError.Inactive:
                    entryType = EventLogEntryType.Error;
                    break;

                // ��������������
                case GeneralError.CurrentlyUnsupported:
                case GeneralError.Unsupported:
                    entryType = EventLogEntryType.Warning;
                    break;
            }

            StringBuilder sb = new StringBuilder(FullDescription);
            sb.AppendLine("���� ������� ��� ������:");
            sb.AppendLine(string.IsNullOrEmpty(_commandDump) ? "��� ������" : _commandDump);
            sender.Logger.WriteEntry(sb.ToString(), entryType);
        }

        /// <summary>
        /// ������� ��������� ������ ��� �������� ������ �� ��������� 
        /// ����� ����� ������
        /// </summary>
        /// <param name="value">����� ��� ������</param>
        /// <param name="sender">����������-����������� ���� ������</param>
        public ServerErrorCode(IDevice sender, GeneralError value)
            : this(sender, value, GetGeneralDescription(value), 0, noErrors, string.Empty)
        {
        }

        /// <summary>
        /// ������� ��������� ������ ��� �������� ������ �� ��������� 
        /// ����� ����� ������, ������� ���� ������� ��� ������
        /// </summary>
        /// <param name="value">����� ��� ������</param>
        /// <param name="sender">����������-����������� ���� ������</param>
        /// <param name="commandDump">���� ������� ��� ������</param>
        public ServerErrorCode(IDevice sender, GeneralError value, string commandDump)
            : this(sender, value, GetGeneralDescription(value), 0, noErrors, commandDump)
        {
        }

        /// <summary>
        /// ������� ��������� ������ ��� �������� ������,
        /// ������������� ��� ��������� ������ � �����������
        /// </summary>
        /// <param name="specificValue">��� ������ ��������� ������ � �����������</param>
        /// <param name="specificDescription">�������� ���� ������</param>
        /// <param name="sender">����������-����������� ���� ������</param>
        public ServerErrorCode(IDevice sender, Int16 specificValue, string specificDescription)
            : this(sender, GeneralError.Specific, GetGeneralDescription(GeneralError.Specific),
            specificValue, specificDescription, string.Empty)
        {
        }

        /// <summary>
        /// ������� ��������� ������ ��� �������� ������,
        /// ������������� ��� ��������� ������ � �����������, ������� ���� �������
        /// </summary>
        /// <param name="specificValue">��� ������ ��������� ������ � �����������</param>
        /// <param name="specificDescription">�������� ���� ������</param>
        /// <param name="sender">����������-����������� ���� ������</param>
        /// <param name="commandDump">���� ������� ��� ������</param>
        public ServerErrorCode(IDevice sender, Int16 specificValue, string specificDescription,
            string commandDump)
            : this(sender, GeneralError.Specific, GetGeneralDescription(GeneralError.Specific),
            specificValue, specificDescription, commandDump)
        {
        }

        /// <summary>
        /// ������� ��������� ������ ��� �������� ������, ���� ��� ����� ������ - ����������
        /// </summary>
        /// <param name="sender">����������-����������� ���� ������</param>
        /// <param name="ex">����������</param>
        public ServerErrorCode(IDevice sender, Exception ex)
            : this(sender, GeneralError.Exception,
            string.Format(exceptionText, ex.GetType().Name, ex.Message, ex.StackTrace), 0, 
            ex.Message, string.Empty)
        {
        }

        /// <summary>
        /// ������� ��������� ������ ��� �������� ������, ���� ��� ����� ������ - ����������
        /// </summary>
        /// <param name="sender">����������-����������� ���� ������</param>
        /// <param name="ex">����������</param>
        /// <param name="commandDump">���� ������� ��� ������</param>
        public ServerErrorCode(IDevice sender, Exception ex, string commandDump)
            : this(sender, GeneralError.Exception,
            string.Format(exceptionText, ex.GetType().Name, ex.Message, ex.StackTrace), 0,
            ex.Message, commandDump)
        {
        }

        #endregion

        #region �������� ��������

        /// <summary>
        /// ���� ������� ��� ������
        /// </summary>
        public string CommandDump
        {
            get { return _commandDump; }
        }

        #endregion
    }
}
