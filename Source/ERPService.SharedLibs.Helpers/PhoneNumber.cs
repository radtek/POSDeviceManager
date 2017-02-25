using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ERPService.SharedLibs.Helpers
{
    /// <summary>
    /// ����� ���������� ������������� ����������� ������
    /// </summary>
    public enum PhoneNumberFormat
    {
        /// <summary>
        /// ��� ������������� � �������� �������� SMS-���������
        /// </summary>
        SMS,

        /// <summary>
        /// �������� �����, ����� �������� � ������������� �������
        /// </summary>
        ReadableInternational,

        /// <summary>
        /// �������� �����, ����� �������� � ������� ��� ������
        /// </summary>
        ReadableRussian,

        /// <summary>
        /// �������� �����, ������ ����� ��������
        /// </summary>
        ReadableNumberOnly,

        /// <summary>
        /// ���������� ����� (��� ����� ������ ������)
        /// </summary>
        NonReadable
    }

    /// <summary>
    /// ����� �������� � ������������� �������
    /// </summary>
    public sealed class PhoneNumber
    {
        #region ���������

        private const String _russianCountryCode = "7";
        private const String _longDistancePrefix = "8";
        private const String _internationalPrefix = "10";
        private const String _phoneNoPattern = @"(?:\u002B?)(\d+)(?:\u0028)(\d{3})(?:\u0029)(\d{3})(?:\u002D?)(\d{2})(?:\u002D?)(\d{2})";
        private const String _phoneNoIncorrectMsg = "������������ ������ ������ ��������";

        #endregion

        #region ����

        private String _countryCode;
        private String _areaCode;
        private String[] _phoneNo;

        #endregion

        #region �����������

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="number">����� ��������</param>
        public PhoneNumber(String number)
        {
            Match match = Regex.Match(number, _phoneNoPattern);
            if (match.Success)
            {
                // ��� ������
                if (String.Compare(match.Groups[1].Value, _longDistancePrefix) == 0)
                    _countryCode = _russianCountryCode;
                else
                    _countryCode = match.Groups[1].Value;
                // ��� �������/����
                _areaCode = match.Groups[2].Value;
                // ����� ��������
                _phoneNo = new String[3] { match.Groups[3].Value, match.Groups[4].Value,
                    match.Groups[5].Value };
            }
            else
                throw new ArgumentOutOfRangeException("number", number, _phoneNoIncorrectMsg);
        }

        #endregion

        #region �������� �������� � ������

        /// <summary>
        /// �������� �� ������ ������� ��������
        /// </summary>
        /// <param name="testString">������, ��������� ��������</param>
        public static Boolean IsPhoneNumber(String testString)
        {
            return Regex.Match(testString, _phoneNoPattern).Success;
        }

        /// <summary>
        /// ��� ������
        /// </summary>
        public String CountryCode
        {
            get { return _countryCode; }
        }

        /// <summary>
        /// ��� �������, ���� (3 �����)
        /// </summary>
        public String AreaCode
        {
            get { return _areaCode; }
        }

        /// <summary>
        /// ����� �������� (7 ���� ��� ������������)
        /// </summary>
        public String PhoneNo
        {
            get 
            {
                StringBuilder sb = new StringBuilder();
                foreach (String s in _phoneNo)
                    sb.Append(s);
                return sb.ToString();
            }
        }

        /// <summary>
        /// ��������� ������������� ������ ��� ������������� � �������� 
        /// �������� SMS-���������
        /// </summary>
        public override String ToString()
        {
            return ToString(PhoneNumberFormat.SMS);
        }

        /// <summary>
        /// ��������� ������������� ������
        /// </summary>
        /// <param name="numberFormat">������ ���������� ������������� ������</param>
        public String ToString(PhoneNumberFormat numberFormat)
        {
            StringBuilder sb = new StringBuilder();
            switch (numberFormat)
            {
                case PhoneNumberFormat.ReadableInternational:
                    sb.Append('+');
                    sb.Append(_countryCode);
                    BuildPhoneNo(sb, true);
                    break;
                case PhoneNumberFormat.ReadableRussian:
                    sb.Append(_longDistancePrefix);
                    sb.Append('-');
                    if (String.Compare(_countryCode, _russianCountryCode) != 0)
                    {
                        sb.Append(_internationalPrefix);
                        sb.Append('-');
                        sb.Append(_countryCode);
                    }
                    BuildPhoneNo(sb, true);
                    break;
                case PhoneNumberFormat.ReadableNumberOnly:
                    BuildPhoneNo(sb, false);
                    break;
                case PhoneNumberFormat.NonReadable:
                    sb.Append(_countryCode);
                    sb.Append(_areaCode);
                    foreach (String s in _phoneNo)
                        sb.Append(s);
                    break;
                default:
                    ConvertToSMS(sb);
                    break;
            }
            return sb.ToString();
        }

        #endregion

        #region �������� ������

        private void ConvertToSMS(StringBuilder sb)
        {
            sb.Append(_countryCode);
            sb.Append(_areaCode);
            sb.Append(PhoneNo);

            if (sb.Length % 2 != 0)
                // �������� ����� ������
                sb.Append('F');

            // � ������ ���� ������ ����� �������
            Int32 hexBytes = sb.Length / 2;
            for (Int32 i = 0; i < hexBytes; i++)
            {
                Char swapChar = sb[i * 2];
                sb[i * 2] = sb[i * 2 + 1];
                sb[i * 2 + 1] = swapChar;
            }
        }

        private void BuildPhoneNo(StringBuilder sb, Boolean includeAreaCode)
        {
            if (includeAreaCode)
            {
                sb.Append('(');
                sb.Append(_areaCode);
                sb.Append(')');
            }
            sb.Append(_phoneNo[0]);
            sb.Append('-');
            sb.Append(_phoneNo[1]);
            sb.Append('-');
            sb.Append(_phoneNo[2]);
        }

        #endregion
    }
}
