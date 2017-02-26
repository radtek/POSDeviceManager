using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace ERPService.SharedLibs.Helpers
{
    /// <summary>
    /// ����������� �������� ��������� ���������
    /// </summary>
    public class ShortMessageSender : IDisposable
    {
        #region ����

        private EasyCommunicationPort _port;
        private string[] _finalResults;

        private const int _waitForCommand = 7000;
        private const string _opCancelled = 
            "�������� ��������� ���������� ��������� ��������. �������: {0}. ������: {1}";
        private const string _invalidIndexes = "�� �������� ���������� ������� ��������� ���������. �����: {0}";
        private const string _storageIndexesPattern =
            @"(?:\u002BCMGD\u003A\s\u0028)(\d+)(?:-)(\d+)(?:\u0029)";

        #endregion

        #region �������� ������

        /// <summary>
        /// ��������� ��������� ���������� ����������� ������
        /// </summary>
        private string OkResult
        {
            get { return _finalResults[0]; }
        }

        /// <summary>
        /// �������� ���������� ��������� ������ �� ��������
        /// </summary>
        /// <param name="answers">������ ��� ���������� �������</param>
        private bool FinalResultReceived(List<string> answers)
        {
            // ���������, ������� �� ���� �� ��������� �����������
            foreach (string answer in answers)
            {
                foreach (string finalResult in _finalResults)
                {
                    if (answer.Contains(finalResult))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// ������ ���������� ������, ����������� �� ��������, � ������
        /// </summary>
        /// <param name="sb">�����</param>
        /// <param name="answers">������ �������</param>
        private void SaveAnswer(StringBuilder sb, List<string> answers)
        {
            if (sb.Length > 0)
            {
                answers.Add(sb.ToString());
                sb.Length = 0;
            }
        }

        /// <summary>
        /// ������ ������ �� AT-�������
        /// </summary>
        /// <param name="answers">������ ��� �������� �������� �����</param>
        private void ReceiveATCommandResponse(List<string> answers)
        {
            // ����������� ������� �� ������ ��� ����, ����� ���� �����
            // �������� ���������� ������� � ������ �������� ����� �� ���
            _port.ReadTimeout = _waitForCommand;

            // ����� �������� ��������� ����������� ������ ������ �� ��������
            StringBuilder sb = new StringBuilder();

            // ������ �� ��������� ������-���� �� ��������� �����������
            do
            {
                // ��������� ����, ���������� �� ��������
                int nextByte;
                // ������ �� ������� ��������
                do
                {
                    nextByte = _port.ReadByte();
                    if (nextByte > 0)
                    {
                        // ���-�� ���������
                        // ������� ����� ���������� ����� �� �������, �������
                        // ���������� ������� �� �����
                        if (_port.ReadTimeout == _waitForCommand)
                            _port.ReadTimeout = 0;

                        // ��������, �� ������ �� ������-����������
                        if (nextByte == 0x0D)
                            // ��������� ��������� ������ ������
                            SaveAnswer(sb, answers);
                        else
                        {
                            if (nextByte >= 0x20)
                                // � ����� ��������� ������ �������� ������
                                sb.Append((Char)nextByte);
                        }
                    }
                }
                while (nextByte > 0);

                // ���������, �� ������ �� ������� ��� ������ ������� �����
                if (_port.ReadTimeout == _waitForCommand)
                    // ������� �� ������� �� �������, �������� �� ������
                    return;

                // �������� ������� ����������� ������ � ������
                SaveAnswer(sb, answers);
            }
            while (!FinalResultReceived(answers));
        }

        /// <summary>
        /// �������� AT-������� ��������
        /// </summary>
        /// <param name="command">����� �������</param>
        /// <param name="terminator">����������� ������</param>
        /// <returns>������ �����, ���������� � ����� �� �������</returns>
        private string[] SendATCommand(string command, byte terminator)
        {
            // ����� ������� � ����������� ������ � ����
            _port.DiscardBuffers();
            _port.Write(Encoding.ASCII.GetBytes(command));
            _port.WriteByte(terminator);

            // ��������� ��������� ���������� �� �������� (����������� AT-���������)
            _port.ThrowTimeoutExceptions = false;            
            try
            {
                // ������ ����� ������
                List<string> answers = new List<string>();
                ReceiveATCommandResponse(answers);
                return answers.ToArray();
            }
            finally
            {
                // ���������� ���� ��������� ����������
                _port.ThrowTimeoutExceptions = true;
            }
        }

        /// <summary>
        /// �������� AT-������� ��������. ������� ����������� �������� CR
        /// </summary>
        /// <param name="command">����� �������</param>
        private string[] SendATCommand(string command)
        {
            return SendATCommand(command, 0x0D);
        }

        /// <summary>
        /// �������� ������� ���������� �������
        /// </summary>
        /// <param name="answers">������ ������� �� �������</param>
        /// <param name="okValue">��������, ��������������� ��������� ���������� �������</param>
        private void CheckCommandStatus(string[] answers, string okValue)
        {
            if (answers.Length == 0)
                throw new InvalidOperationException("�������� ����� �� �������");

            if (IsFail(answers, okValue))
                throw new OperationCanceledException(
                    string.Format(_opCancelled, answers[0], answers[answers.Length - 1]));
        }

        /// <summary>
        /// �������� ������� ���������� �������, ��� ������� ��������� ����� "OK"
        /// </summary>
        /// <param name="answers">������ ������� �� �������</param>
        private void CheckCommandStatus(string[] answers)
        {
            CheckCommandStatus(answers, OkResult);
        }

        /// <summary>
        /// ���������� ������� ����������� ���������� �������
        /// </summary>
        /// <param name="answers">������ ������� �� �������</param>
        /// <param name="okValue">��������, ��������������� ��������� ���������� �������</param>
        private bool IsFail(string[] answers, string okValue)
        {
            return !answers[answers.Length - 1].Contains(okValue);
        }

        /// <summary>
        /// ���������� �������� �������� �������� ���������
        /// </summary>
        /// <param name="minValue">����������� ������</param>
        /// <param name="maxValue">������������ ������</param>
        private void GetStoragesRange(out int minValue, out int maxValue)
        {
            // ����������� �������� ��������
            string[] answers = SendATCommand("AT+CMGD=?");
            CheckCommandStatus(answers);

            // ��������� ���������� �����
            foreach (string answer in answers)
            {
                Match match = Regex.Match(answer, _storageIndexesPattern);
                if (match.Success)
                {
                    minValue = Convert.ToInt32(match.Groups[1].Value);
                    maxValue = Convert.ToInt32(match.Groups[2].Value);
                    return;
                }
            }
            throw new InvalidOperationException(string.Format(_invalidIndexes, answers[0]));
        }

        /// <summary>
        /// ���������� ����� � ������
        /// </summary>
        private void PreparePort()
        {
            if (!_port.IsOpen)
            {
                _port.Open();
                _port.WriteTimeout = 2000;
                // ����� �������������� ������ ������� ������ �����
                _port.ReadTimeout = 0;
            }
        }

        #endregion

        #region �����������

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="portName">��� ����� ������</param>
        /// <param name="baud">�������� ����� � �������</param>
        public ShortMessageSender(string portName, int baud)
        {
            _port = new EasyCommunicationPort();
            _port.PortName = portName;
            _port.BaudRate = baud;
            _port.DataBits = 8;
            _port.StopBits = StopBits.One;
            _port.Parity = Parity.None;
            _port.DsrFlow = false;

            _finalResults = new string[] { "OK", ">", "ERROR", "+CMS ERROR", "Call Ready" };
        }

        #endregion

        #region �������� ������

        /// <summary>
        /// ������� ������ �������� �� ���������
        /// </summary>
        public void CleanupMessages()
        {
            PreparePort();
            // ���������� �������� �������� ��������� ���������
            int minIndex, maxIndex;
            GetStoragesRange(out minIndex, out maxIndex);

            for (int i = minIndex; i <= maxIndex; i++)
                CheckCommandStatus(SendATCommand(string.Format("AT+CMGD={0}", i)));
        }

        /// <summary>
        /// �������� ���������
        /// </summary>
        /// <param name="message">���������, �������������� � ��������</param>
        public void Send(EncodedMessage message)
        {
            PreparePort();
            // �������� ����� �������� SMS � PDU-�������
            CheckCommandStatus(SendATCommand("AT+CMGF=0"));
            // ���������, ������������ �� ������� SMS-�������
            CheckCommandStatus(SendATCommand("AT+CSMS=0"));
            // ���������� ��������� ���������
            CheckCommandStatus(
                SendATCommand(string.Format("AT+CMGS={0}", message.NumberOfOctets)), ">");
            // ���������� ���� ���������
            CheckCommandStatus(SendATCommand(message.Message, 0x1A));
        }

        /// <summary>
        /// �������� ���������� ���������
        /// </summary>
        /// <param name="messages">���������, �������������� � ��������</param>
        public void Send(EncodedMessage[] messages)
        {
            Send(messages, true);
        }

        /// <summary>
        /// �������� ���������� ���������
        /// </summary>
        /// <param name="messages">���������, �������������� � ��������</param>
        /// <param name="reverseOrder">��������� ��������� � �������� �������</param>
        public void Send(EncodedMessage[] messages, bool reverseOrder)
        {
            if (reverseOrder)
            {
                for (int i = messages.Length; i > 0; i--)
                    Send(messages[i - 1]);
            }
            else
            {
                foreach (EncodedMessage message in messages)
                    Send(message);
            }
        }

        /// <summary>
        /// ������������ ���������� ��� �������� ���������
        /// </summary>
        public virtual void RebootGSMDevice()
        {
            PreparePort();
            // ���������� ������� ���������� ������� GPRS-������
            // ��� USB-������� Teleofis RX101 ��� ������� ��������� ������������
            CheckCommandStatus(SendATCommand("AT+CPOWD=1"), "Call Ready");
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// ������������ ��������
        /// </summary>
        public void Dispose()
        {
            if (_port != null)
            {
                _port.Dispose();
                _port = null;
            }
        }

        #endregion
    }
}
