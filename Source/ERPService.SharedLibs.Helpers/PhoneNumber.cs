using System;
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

        private const string _russianCountryCode = "7";
        private const string _longDistancePrefix = "8";
        private const string _internationalPrefix = "10";
        private const string _phoneNoPattern = @"(?:\u002B?)(\d+)(?:\u0028)(\d{3})(?:\u0029)(\d{3})(?:\u002D?)(\d{2})(?:\u002D?)(\d{2})";
        private const string _phoneNoIncorrectMsg = "Некорректный формат номера телефона";

        #endregion

        #region Поля

        private string _countryCode;
        private string _areaCode;
        private string[] _phoneNo;

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="number">Номер телефона</param>
        public PhoneNumber(string number)
        {
            Match match = Regex.Match(number, _phoneNoPattern);
            if (match.Success)
            {
                // код страны
                if (string.Compare(match.Groups[1].Value, _longDistancePrefix) == 0)
                    _countryCode = _russianCountryCode;
                else
                    _countryCode = match.Groups[1].Value;
                // код региона/сети
                _areaCode = match.Groups[2].Value;
                // номер телефона
                _phoneNo = new string[3] { match.Groups[3].Value, match.Groups[4].Value,
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
        public static bool IsPhoneNumber(string testString)
        {
            return Regex.Match(testString, _phoneNoPattern).Success;
        }

        /// <summary>
        /// Код страны
        /// </summary>
        public string CountryCode
        {
            get { return _countryCode; }
        }

        /// <summary>
        /// Код региона, сети (3 цифры)
        /// </summary>
        public string AreaCode
        {
            get { return _areaCode; }
        }

        /// <summary>
        /// Номер телефона (7 цифр без разделителей)
        /// </summary>
        public string PhoneNo
        {
            get 
            {
                StringBuilder sb = new StringBuilder();
                foreach (string s in _phoneNo)
                    sb.Append(s);
                return sb.ToString();
            }
        }

        /// <summary>
        /// Строковое представление номера для использования в протколе 
        /// передачи SMS-сообщений
        /// </summary>
        public override string ToString()
        {
            return ToString(PhoneNumberFormat.SMS);
        }

        /// <summary>
        /// Строковое представление номера
        /// </summary>
        /// <param name="numberFormat">Формат строкового представления номера</param>
        public string ToString(PhoneNumberFormat numberFormat)
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
                    if (string.Compare(_countryCode, _russianCountryCode) != 0)
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
                    foreach (string s in _phoneNo)
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
            int hexBytes = sb.Length / 2;
            for (int i = 0; i < hexBytes; i++)
            {
                Char swapChar = sb[i * 2];
                sb[i * 2] = sb[i * 2 + 1];
                sb[i * 2 + 1] = swapChar;
            }
        }

        private void BuildPhoneNo(StringBuilder sb, bool includeAreaCode)
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
