using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.Helpers.SerialCommunications
{
    /// <summary>
    /// ��������������� ����� ��� ������ � ���������� ����������� ����� ���������������� ����
    /// </summary>
    public sealed class TextProtocol
    {
        private EasyCommunicationPort _port;
        private Encoding _encoding;
        private Int32 _receiveTimeout;

        private Boolean IsStopByte(Byte b)
        {
            return (b == 10 || b == 13);
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="encoding">��������� ��� �������������� �����</param>
        /// <param name="port">���������������� ����</param>
        public TextProtocol(Encoding encoding, EasyCommunicationPort port)
        {
            _encoding = encoding;
            _port = port;
            _port.WriteTimeout = 1000;
            _port.ReadTimeout = -1;
            _receiveTimeout = 5000;
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="port">���������������� ����</param>
        public TextProtocol(EasyCommunicationPort port)
            : this(Encoding.Default, port)
        {
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        public Int32 ReceiveTimeout
        {
            get { return _receiveTimeout; }
            set { _receiveTimeout = value; }
        }

        /// <summary>
        /// �������� �������
        /// </summary>
        /// <param name="command">�������</param>
        /// <returns>�����</returns>
        public String Send(String command)
        {
            // ��������� ����������� ������ � �������
            String preparedCommand = String.Concat(command, "\r");

            // ����� ������� � ����
            _port.ClearError();
            _port.DiscardBuffers();
            _port.Write(_encoding.GetBytes(preparedCommand));

            // �������� �����
            DateTime fixedTime = DateTime.Now;

            // ����� ��� �������� ������
            List<Byte> answer = new List<Byte>();
            // ��������� ���� ������
            Byte nextByte = 0;
            // ��������� ����� ������
            Byte[] buf = new Byte[1024];

            // ������ �����
            do
            {
                // ������ ��������� ������ ������ � � �����
                Array.Clear(buf, 0, buf.Length);
                Int32 bytesRead = _port.Read(buf, 0, buf.Length);
                
                // ��������� ���������� ������
                for (Int32 i = 0; i < bytesRead; i++)
                {
                    nextByte = buf[i];

                    if (IsStopByte(nextByte))
                        // �������� ����
                        break;

                    if (nextByte >= 0x20)
                        // �������� ������ ��� ��� �����
                        answer.Add(nextByte);
                }

                if (IsStopByte(nextByte))
                    break;
                else
                {
                    TimeSpan elapsedTime = DateTime.Now - fixedTime;
                    if (elapsedTime.TotalMilliseconds >= _receiveTimeout)
                        throw new TimeoutException("����� �������� ������ �������");
                }
            }
            while (!IsStopByte(nextByte));

            // ���������� ����� � ���� ������ � ������� ���������
            return _encoding.GetString(answer.ToArray());
        }
    }
}
