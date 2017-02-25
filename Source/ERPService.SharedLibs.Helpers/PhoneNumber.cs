using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ERPService.SharedLibs.Helpers
{
    /// <summary>
    /// Форма строкового представления телефонного номера
    /// </summary>
    public enum PhoneNumberFormat
    {
        /// <summary>
        /// Для использования в протколе передачи SMS-сообщений
        /// </summary>
        SMS,

        /// <summary>
        /// Читаемая форма, номер задается в международном формате
        /// </summary>
        ReadableInternational,

        /// <summary>
        /// Читаемая форма, номер задается в формате для России
        /// </summary>
        ReadableRussian,

        /// <summary>
        /// Читаемая форма, только номер телефона
        /// </summary>
        ReadableNumberOnly,

        /// <summary>
        /// Нечитаемая форма (все части номера слитно)
        /// </summary>
        NonReadable
    }

    /// <summary>
    /// Номер телефона в международном формате
    /// </summary>
    public sealed class PhoneNumber
    {
        #region Константы

        private const String _russianCountryCode = "7";
        private const String _longDistancePrefix = "8";
        private const String _internationalPrefix = "10";
        private const String _phoneNoPattern = @"(?:\u002B?)(\d+)(?:\u0028)(\d{3})(?:\u0029)(\d{3})(?:\u002D?)(\d{2})(?:\u002D?)(\d{2})";
        private const String _phoneNoIncorrectMsg = "Некорректный формат номера телефона";

        #endregion

        #region Поля

        private String _countryCode;
        private String _areaCode;
        private String[] _phoneNo;

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="number">Номер телефона</param>
        public PhoneNumber(String number)
        {
            Match match = Regex.Match(number, _phoneNoPattern);
            if (match.Success)
            {
                // код страны
                if (String.Compare(match.Groups[1].Value, _longDistancePrefix) == 0)
                    _countryCode = _russianCountryCode;
                else
                    _countryCode = match.Groups[1].Value;
                // код региона/сети
                _areaCode = match.Groups[2].Value;
                // номер телефона
                _phoneNo = new String[3] { match.Groups[3].Value, match.Groups[4].Value,
                    match.Groups[5].Value };
            }
            else
                throw new ArgumentOutOfRangeException("number", number, _phoneNoIncorrectMsg);
        }

        #endregion

        #region Открытые свойства и методы

        /// <summary>
        /// Является ли строка номером телефона
        /// </summary>
        /// <param name="testString">Строка, требующая проверки</param>
        public static Boolean IsPhoneNumber(String testString)
        {
            return Regex.Match(testString, _phoneNoPattern).Success;
        }

        /// <summary>
        /// Код страны
        /// </summary>
        public String CountryCode
        {
            get { return _countryCode; }
        }

        /// <summary>
        /// Код региона, сети (3 цифры)
        /// </summary>
        public String AreaCode
        {
            get { return _areaCode; }
        }

        /// <summary>
        /// Номер телефона (7 цифр без разделителей)
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
        /// Строковое представление номера для использования в протколе 
        /// передачи SMS-сообщений
        /// </summary>
        public override String ToString()
        {
            return ToString(PhoneNumberFormat.SMS);
        }

        /// <summary>
        /// Строковое представление номера
        /// </summary>
        /// <param name="numberFormat">Формат строкового представления номера</param>
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

        #region Закрытые методы

        private void ConvertToSMS(StringBuilder sb)
        {
            sb.Append(_countryCode);
            sb.Append(_areaCode);
            sb.Append(PhoneNo);

            if (sb.Length % 2 != 0)
                // нечетная длина номера
                sb.Append('F');

            // в каждой паре меняем цифры местами
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
