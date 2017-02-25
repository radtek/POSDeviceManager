using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ERPService.SharedLibs.Helpers
{
    /// <summary>
    /// ��������������� ����� ��� �������� �������������� � �������� �������� ���������
    /// </summary>
    public sealed class EncodedMessage
    {
        private Int32 _numberOfOctets;
        private String _message;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="numberOfOctets">����� �������</param>
        /// <param name="message">�������������� ���������</param>
        public EncodedMessage(Int32 numberOfOctets, String message)
        {
            _numberOfOctets = numberOfOctets;
            _message = message;
        }

        /// <summary>
        /// ����� �������
        /// </summary>
        public Int32 NumberOfOctets
        {
            get { return _numberOfOctets; }
        }

        /// <summary>
        /// �������������� ���������
        /// </summary>
        public String Message
        {
            get { return _message; }
        }

        /// <summary>
        /// ��������� ������������� �������
        /// </summary>
        public override String ToString()
        {
            return _message;
        }
    }

    /// <summary>
    /// ������������ �������� ��������� ���������
    /// </summary>
    public class ShortMessageEncoder
    {
        #region ����

        private PhoneNumber _smsServer;
        private PhoneNumber _recipient;
        private String _messageText;
        private Int32 _validityPeriod = 2;
        private const Int32 _maxTextLength = 70;

        #endregion

        #region ��������

        /// <summary>
        /// ����� �������� SMS-�������
        /// </summary>
        public PhoneNumber SmsServer
        {
            get { return _smsServer; }
            set { _smsServer = value; }
        }

        /// <summary>
        /// ����� �������� ���������� ���������
        /// </summary>
        public PhoneNumber Recipient
        {
            get { return _recipient; }
            set { _recipient = value; }
        }

        /// <summary>
        /// ����� ���������
        /// </summary>
        public String MessageText
        {
            get { return _messageText; }
            set { _messageText = value; }
        }

        /// <summary>
        /// ������ ���������� ���������
        /// </summary>
        public Int32 ValidityPeriod
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

        #region �������� ������

        /// <summary>
        /// ������������ ��������� ���������� ��������� ��� ��������
        /// </summary>
        /// <returns>�������� ��������� ��������� � PDU-�������</returns>
        /// <remarks>��������� ������������ ����� ������ � ����� ��������� ����������,
        /// �� �������� ����� ��������� � ���������� ����� ���� ������ �� ��������� 
        /// ��������� � PDU-�������</remarks>
        public EncodedMessage[] Encode()
        {
            if (_recipient == null)
                throw new InvalidOperationException("�� ����� ����� ���������� ���������");
            if (String.IsNullOrEmpty(_messageText))
                throw new InvalidOperationException("�� ����� ����� ���������");

            // ��������� ����� �� ��������� � ������ ����. �����
            List<String> rawMessages = ParseMessageText();
            
            // ����� ����� ��������� �������������� ���������
            EncodedMessage[] messages = new EncodedMessage[rawMessages.Count];
            for (Int32 i = 0; i < rawMessages.Count; i++)
            {
                // �������� ��������� ���������
                messages[i] = Encode(rawMessages[i]);
            }
            return messages;
        }

        #endregion

        #region �������� ������

        private List<String> ParseMessageText()
        {
            // ��������� ����� �� ��������
            MatchCollection matches = Regex.Matches(_messageText, @"\S+\s*");
            if (matches.Count > 0)
            {
                // ��������� ������ ��������� � ������ ����. ����� ������ SMS
                List<String> rawMessages = new List<String>();
                StringBuilder sb = new StringBuilder();

                foreach (Match match in matches)
                {
                    if (match.Value.Length > _maxTextLength)
                        // ����� ������� ����������� ���������� ����� ���������
                        // �� ��������������
                        throw new InvalidOperationException(
                            String.Format("������� ������� ����� - \"{0}\"", match.Value));

                    // �� �������� �� ����� ��������� ����������
                    if (sb.Length + match.Value.Length > _maxTextLength)
                        // ����� ���������
                        SaveMessage(sb, rawMessages);

                    // ���������� ��������� �������� � ������ ���������
                    sb.Append(match.Value);
                }

                // ���� ������� "�����"
                SaveMessage(sb, rawMessages);
                return rawMessages;
            }
            else
                throw new InvalidOperationException("��������� ���������� �����������");
        }

        private void SaveMessage(StringBuilder sb, List<String> rawMessages)
        {
            if (sb.Length > 0)
            {
                rawMessages.Add(sb.ToString());
                sb.Length = 0;
            }
        }

        private EncodedMessage Encode(String sourceText)
        {
            StringBuilder sbMain = new StringBuilder();
            StringBuilder sbMessage = new StringBuilder();
            
            // ����� SMS-�������
            if (_smsServer == null)
                // ������������ ����������� ��������
                sbMain.Append("00");
            else
            {
                // ����� ����
                String smsServerNo = _smsServer.ToString();
                // ����� ������ (����� HEX-���� + 1)
                sbMain.Append((smsServerNo.Length / 2 + 1).ToString("X2"));
                // ��� ������ (�������������)
                sbMain.Append("91");
                // ����� 
                sbMain.Append(smsServerNo);
            }

            // ���������� ���������
            // ���� ������ ��������� (��� ��������� - ���������, ������ �������������
            // ����� ���������� ���������, � ����)
            sbMessage.Append("11");
            // ��������� ����� ��������� (����� ��������� ���������)
            sbMessage.Append("00");
            // ����� ������ ����������, ����� ���������� ����
            sbMessage.Append(
                _recipient.ToString(PhoneNumberFormat.NonReadable).Length.ToString("X2"));
            // ��� ������ (�������������)
            sbMessage.Append("91");
            // ����� ����������
            sbMessage.Append(_recipient.ToString(PhoneNumberFormat.SMS));
            // ������������� ��������� (������� ���������)
            sbMessage.Append("00");
            // ����� ����������� ������ � ���� ������ (��������� UCS2)
            sbMessage.Append("08");
            // ������ ���������� ���������
            sbMessage.Append((166 + _validityPeriod).ToString("X2"));

            // �������� ����� ��������� � UCS2
            String ucs2Text = EncodeUSC2String(sourceText);

            // ����� ������
            sbMessage.Append((ucs2Text.Length / 2).ToString("X2"));
            // ����� 
            sbMessage.Append(ucs2Text);

            // ���������� ��� ������
            sbMain.Append(sbMessage.ToString());
            EncodedMessage encMessage = new EncodedMessage(sbMessage.ToString().Length / 2, 
                sbMain.ToString());
            
            return encMessage;
        }

        private String EncodeUSC2String(String source)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Char c in source)
                // ��������� ������������� ������� ������-������� � HEX-�������
                sb.Append(((Int16)c).ToString("X4"));
            return sb.ToString();
        }

        #endregion
    }
}
