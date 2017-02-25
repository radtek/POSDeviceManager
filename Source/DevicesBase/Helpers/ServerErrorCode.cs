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
        private const String noErrors = "������ ���";
        // ������ �������� ������, ������������� ��� ��������� ������ � �����������
        private const String specificError = "������ {0}. {1}";
        // ������ ��� �������� ���������� �� ����������
        private const String exceptionText = "���������� {0}. �����: \"{1}\". ����: \"{2}\"";

        #endregion

        #region ����

        private String _commandDump;

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
        public ServerErrorCode(IDevice sender, GeneralError value, String description,
            Int16 specificValue, String specificDescription, String commandDump)
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
            sb.AppendLine(String.IsNullOrEmpty(_commandDump) ? "��� ������" : _commandDump);
            sender.Logger.WriteEntry(sb.ToString(), entryType);
        }

        /// <summary>
        /// ������� ��������� ������ ��� �������� ������ �� ��������� 
        /// ����� ����� ������
        /// </summary>
        /// <param name="value">����� ��� ������</param>
        /// <param name="sender">����������-����������� ���� ������</param>
        public ServerErrorCode(IDevice sender, GeneralError value)
            : this(sender, value, GetGeneralDescription(value), 0, noErrors, String.Empty)
        {
        }

        /// <summary>
        /// ������� ��������� ������ ��� �������� ������ �� ��������� 
        /// ����� ����� ������, ������� ���� ������� ��� ������
        /// </summary>
        /// <param name="value">����� ��� ������</param>
        /// <param name="sender">����������-����������� ���� ������</param>
        /// <param name="commandDump">���� ������� ��� ������</param>
        public ServerErrorCode(IDevice sender, GeneralError value, String commandDump)
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
        public ServerErrorCode(IDevice sender, Int16 specificValue, String specificDescription)
            : this(sender, GeneralError.Specific, GetGeneralDescription(GeneralError.Specific),
            specificValue, specificDescription, String.Empty)
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
        public ServerErrorCode(IDevice sender, Int16 specificValue, String specificDescription,
            String commandDump)
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
            String.Format(exceptionText, ex.GetType().Name, ex.Message, ex.StackTrace), 0, 
            ex.Message, String.Empty)
        {
        }

        /// <summary>
        /// ������� ��������� ������ ��� �������� ������, ���� ��� ����� ������ - ����������
        /// </summary>
        /// <param name="sender">����������-����������� ���� ������</param>
        /// <param name="ex">����������</param>
        /// <param name="commandDump">���� ������� ��� ������</param>
        public ServerErrorCode(IDevice sender, Exception ex, String commandDump)
            : this(sender, GeneralError.Exception,
            String.Format(exceptionText, ex.GetType().Name, ex.Message, ex.StackTrace), 0,
            ex.Message, commandDump)
        {
        }

        #endregion

        #region �������� ��������

        /// <summary>
        /// ���� ������� ��� ������
        /// </summary>
        public String CommandDump
        {
            get { return _commandDump; }
        }

        #endregion
    }
}
