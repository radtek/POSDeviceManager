using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ERPService.SharedLibs.Helpers
{
    /// <summary>
    /// Вспомогательный класс для хранения подготовленных к отправке коротких сообщений
    /// </summary>
    public sealed class EncodedMessage
    {
        private int _numberOfOctets;
        private string _message;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="numberOfOctets">Число октетов</param>
        /// <param name="message">Закодированное сообщение</param>
        public EncodedMessage(int numberOfOctets, string message)
        {
            _numberOfOctets = numberOfOctets;
            _message = message;
        }

        /// <summary>
        /// Число октетов
        /// </summary>
        public int NumberOfOctets
        {
            get { return _numberOfOctets; }
        }

        /// <summary>
        /// Закодированное сообщение
        /// </summary>
        public string Message
        {
            get { return _message; }
        }

        /// <summary>
        /// Строковое представление объекта
        /// </summary>
        public override string ToString()
        {
            return _message;
        }
    }

    /// <summary>
    /// Формирование коротких текстовых сообщений
    /// </summary>
    public class ShortMessageEncoder
    {
        #region Поля

        private PhoneNumber _smsServer;
        private PhoneNumber _recipient;
        private string _messageText;
        private int _validityPeriod = 2;
        private const int _maxTextLength = 70;

        #endregion

        #region Свойства

        /// <summary>
        /// Номер телефона SMS-сервера
        /// </summary>
        public PhoneNumber SmsServer
        {
            get { return _smsServer; }
            set { _smsServer = value; }
        }

        /// <summary>
        /// Номер телефона получателя сообщения
        /// </summary>
        public PhoneNumber Recipient
        {
            get { return _recipient; }
            set { _recipient = value; }
        }

        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string MessageText
        {
            get { return _messageText; }
            set { _messageText = value; }
        }

        /// <summary>
        /// Период валидности сообщений
        /// </summary>
        public int ValidityPeriod
        {
            get { return _validityPeriod; }
            set 
            {
                if (value < 2 || value > 30)
                    throw new ArgumentOutOfRangeException("value");
                _validityPeriod = value;
            }
        }

        #endregion

        #region Открытые методы

        /// <summary>
        /// Формирование короткого текстового сообщения для отправки
        /// </summary>
        /// <returns>Короткое текстовое сообщение в PDU-формате</returns>
        /// <remarks>Поскольку максимальная длина текста в одном сообщении ограничена,
        /// то исходный текст сообщения в результате может быть разбит на несколько 
        /// сообщений в PDU-формате</remarks>
        public EncodedMessage[] Encode()
        {
            if (_recipient == null)
                throw new InvalidOperationException("Не задан номер получателя сообщений");
            if (string.IsNullOrEmpty(_messageText))
                throw new InvalidOperationException("Не задан текст сообщения");

            // разбиваем текст на сообщения с учетом макс. длины
            List<string> rawMessages = ParseMessageText();
            
            // здесь будут сохранены закодированные сообщения
            EncodedMessage[] messages = new EncodedMessage[rawMessages.Count];
            for (int i = 0; i < rawMessages.Count; i++)
            {
                // кодируем очередное сообщение
                messages[i] = Encode(rawMessages[i]);
            }
            return messages;
        }

        #endregion

        #region Закрытые методы

        private List<string> ParseMessageText()
        {
            // разбиваем текст на лексеммы
            MatchCollection matches = Regex.Matches(_messageText, @"\S+\s*");
            if (matches.Count > 0)
            {
                // формируем список сообщений с учетом макс. длины данных SMS
                List<string> rawMessages = new List<string>();
                StringBuilder sb = new StringBuilder();

                foreach (Match match in matches)
                {
                    if (match.Value.Length > _maxTextLength)
                        // слова длиннее макисмально допустимой длины сообщения
                        // не обрабатываются
                        throw new InvalidOperationException(
                            string.Format("Слишком длинное слово - \"{0}\"", match.Value));

                    // не превысит ли длина сообщения допустимую
                    if (sb.Length + match.Value.Length > _maxTextLength)
                        // новое сообщение
                        SaveMessage(sb, rawMessages);

                    // прибавляем очередную лексемму к тексту сообщения
                    sb.Append(match.Value);
                }

                // если остался "хвост"
                SaveMessage(sb, rawMessages);
                return rawMessages;
            }
            else
                throw new InvalidOperationException("Сообщение составлено некорректно");
        }

        private void SaveMessage(StringBuilder sb, List<string> rawMessages)
        {
            if (sb.Length > 0)
            {
                rawMessages.Add(sb.ToString());
                sb.Length = 0;
            }
        }

        private EncodedMessage Encode(string sourceText)
        {
            StringBuilder sbMain = new StringBuilder();
            StringBuilder sbMessage = new StringBuilder();
            
            // номер SMS-сервера
            if (_smsServer == null)
                // определяется настройками телефона
                sbMain.Append("00");
            else
            {
                // задан явно
                string smsServerNo = _smsServer.ToString();
                // длина номера (число HEX-байт + 1)
                sbMain.Append((smsServerNo.Length / 2 + 1).ToString("X2"));
                // тип номера (международный)
                sbMain.Append("91");
                // номер 
                sbMain.Append(smsServerNo);
            }

            // собственно сообщение
            // поле данных протокола (тип сообщения - исходящее, задано относительное
            // время валидности сообщения, в днях)
            sbMessage.Append("11");
            // ссылочный номер сообщения (будет возвращен телефоном)
            sbMessage.Append("00");
            // длина номера получателя, число ДЕСЯТИЧНЫХ цифр
            sbMessage.Append(
                _recipient.ToString(PhoneNumberFormat.NonReadable).Length.ToString("X2"));
            // тип номера (международный)
            sbMessage.Append("91");
            // номер получателя
            sbMessage.Append(_recipient.ToString(PhoneNumberFormat.SMS));
            // идентификатор протокола (обычное сообщение)
            sbMessage.Append("00");
            // схема кодирования данных в поле данных (кодировка UCS2)
            sbMessage.Append("08");
            // период валидности сообщения
            sbMessage.Append((166 + _validityPeriod).ToString("X2"));

            // кодируем текст сообщения в UCS2
            string ucs2Text = EncodeUSC2String(sourceText);

            // длина текста
            sbMessage.Append((ucs2Text.Length / 2).ToString("X2"));
            // текст 
            sbMessage.Append(ucs2Text);

            // объединяем две строки
            sbMain.Append(sbMessage.ToString());
            EncodedMessage encMessage = new EncodedMessage(sbMessage.ToString().Length / 2, 
                sbMain.ToString());
            
            return encMessage;
        }

        private string EncodeUSC2String(string source)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Char c in source)
                // строковое представление каждого юникод-символа в HEX-формате
                sb.Append(((Int16)c).ToString("X4"));
            return sb.ToString();
        }

        #endregion
    }
}
