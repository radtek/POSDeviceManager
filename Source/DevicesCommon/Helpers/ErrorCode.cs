using System;
using System.Collections.Generic;
using System.Text;

namespace DevicesCommon.Helpers
{
    /// <summary>
    /// ��� ������, ������������ �����������
    /// </summary>
    [Serializable]
    public class ErrorCode
    {
        #region ���������

        // ������ ������� �������� ���� ������
        private const String fullDescriptionTemplate = "����������: \"{0}\".\n����� ��� ������: {1} ({2}).\n�������������� ��� ������: {3} ({4})";

        #endregion

        #region ����

        // ������������� ����������, �������� ����������� ��� ������
        private String _sender;
        // ����� ��� ������
        private GeneralError _value;
        // �������� ������ ���� ������
        private String _description;
        // ������������� ��� ��������� ������ � ����������� ��� ������
        private Int16 _specificValue;
        // �������� �������������� ��� ��������� ������ � ����������� ���� ������
        private String _specificDescription;

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
        public ErrorCode(String sender, GeneralError value, String description,
            Int16 specificValue, String specificDescription)
        {
            _sender = sender;
            _value = value;
            _description = description;
            _specificValue = specificValue;
            _specificDescription = specificDescription;
        }

        #endregion

        #region ������, ��������� �� ��������

        /// <summary>
        /// ���������� �������� ������ ���� ������ �� ��� ��������
        /// </summary>
        /// <param name="value">�������� ���� ������</param>
        protected static String GetGeneralDescription(GeneralError value)
        {
            switch (value)
            {
                case GeneralError.Success:
                    return "������� (������ ���)";
                case GeneralError.Busy:
                    return "���������� ������ ������ �����������";
                case GeneralError.Timeout:
                    return "������� �� ����� ������ ������� � �����������";
                case GeneralError.Inactive:
                    return "���������� ���������. ��������� ������������ ���������� ����� ���������� � ��� ������������ �������";
                case GeneralError.Exception:
                    return "�������������� �������� �� ����� ������ ������� � �����������";
                case GeneralError.Unsupported:
                    return "������� �� �������������� �����������";
                case GeneralError.CurrentlyUnsupported:
                    return "������� �� �������������� ����������� ��� ������ ���������� �������";
                default:
                    return "������, ������������� ��� ��������� ������ � �����������";
            }
        }

        #endregion

        #region ������� ��������

        /// <summary>
        /// ������������� ����������, �������� ����������� ��� ������
        /// </summary>
        public String Sender
        {
            get { return _sender; }
        }

        /// <summary>
        /// ����� ��� ������
        /// </summary>
        public GeneralError Value
        {
            get { return _value; }
        }

        /// <summary>
        /// �������� ������ ���� ������
        /// </summary>
        public String Description 
        {
            get { return _description; }
        }

        /// <summary>
        /// ������������� ��� ��������� ������ � ����������� ��� ������
        /// </summary>
        public short SpecificValue
        {
            get { return _specificValue; }
        }

        /// <summary>
        /// �������� �������������� ��� ��������� ������ � ����������� ���� ������
        /// </summary>
        public String SpecificDescription
        {
            get { return _specificDescription; }
        }

        /// <summary>
        /// ���������� ������ �������� ���� ������
        /// </summary>
        public string FullDescription
        {
            get
            {
                return String.Format(
                    fullDescriptionTemplate,
                    _sender,
                    _value,
                    _description,
                    _specificValue,
                    _specificDescription);
            }
        }

        /// <summary>
        /// �������� ���������� ���������� ��������
        /// </summary>
        public Boolean Succeeded
        {
            get { return _value == GeneralError.Success; }
        }

        /// <summary>
        /// ���������� ���������� ���������� ��������
        /// </summary>
        public Boolean Failed
        {
            get { return _value != GeneralError.Success; }
        }

        #endregion

        #region ���������� ������� �������� ������

        /// <summary>
        /// ���������� ��������� ������������� ������� (������� �������� ���� ������)
        /// </summary>
        public override String ToString()
        {
            switch (_value)
            {
                case GeneralError.Specific:
                    return _specificDescription;
                case GeneralError.Exception:
                    return _specificDescription;
                default:
                    return GetGeneralDescription(Value);
            }
        }

        #endregion
    }
}
