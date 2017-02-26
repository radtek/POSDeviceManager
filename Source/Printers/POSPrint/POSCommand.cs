using System;
using System.Text;
using DevicesCommon.Helpers;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace POSPrint
{
    internal class POSCommand
    {
        private const int MAX_CMD_LEN = 1024;
        private byte[] m_Cmd = null;
        private int m_CmdLen;
        private int m_FullLen;

        static byte[] CRC8_DATA = new byte[] {
            0x00, 0x5e, 0xbc, 0xe2, 0x61, 0x3f, 0xdd, 0x83,
            0xc2, 0x9c, 0x7e, 0x20, 0xa3, 0xfd, 0x1f, 0x41,
            0x9d, 0xc3, 0x21, 0x7f, 0xfc, 0xa2, 0x40, 0x1e,
            0x5f, 0x01, 0xe3, 0xbd, 0x3e, 0x60, 0x82, 0xdc,
            0x23, 0x7d, 0x9f, 0xc1, 0x42, 0x1c, 0xfe, 0xa0,
            0xe1, 0xbf, 0x5d, 0x03, 0x80, 0xde, 0x3c, 0x62,
            0xbe, 0xe0, 0x02, 0x5c, 0xdf, 0x81, 0x63, 0x3d,
            0x7c, 0x22, 0xc0, 0x9e, 0x1d, 0x43, 0xa1, 0xff,
            0x46, 0x18, 0xfa, 0xa4, 0x27, 0x79, 0x9b, 0xc5,
            0x84, 0xda, 0x38, 0x66, 0xe5, 0xbb, 0x59, 0x07,
            0xdb, 0x85, 0x67, 0x39, 0xba, 0xe4, 0x06, 0x58,
            0x19, 0x47, 0xa5, 0xfb, 0x78, 0x26, 0xc4, 0x9a,
            0x65, 0x3b, 0xd9, 0x87, 0x04, 0x5a, 0xb8, 0xe6,
            0xa7, 0xf9, 0x1b, 0x45, 0xc6, 0x98, 0x7a, 0x24,
            0xf8, 0xa6, 0x44, 0x1a, 0x99, 0xc7, 0x25, 0x7b,
            0x3a, 0x64, 0x86, 0xd8, 0x5b, 0x05, 0xe7, 0xb9,
            0x8c, 0xd2, 0x30, 0x6e, 0xed, 0xb3, 0x51, 0x0f,
            0x4e, 0x10, 0xf2, 0xac, 0x2f, 0x71, 0x93, 0xcd,
            0x11, 0x4f, 0xad, 0xf3, 0x70, 0x2e, 0xcc, 0x92,
            0xd3, 0x8d, 0x6f, 0x31, 0xb2, 0xec, 0x0e, 0x50,
            0xaf, 0xf1, 0x13, 0x4d, 0xce, 0x90, 0x72, 0x2c,
            0x6d, 0x33, 0xd1, 0x8f, 0x0c, 0x52, 0xb0, 0xee,
            0x32, 0x6c, 0x8e, 0xd0, 0x53, 0x0d, 0xef, 0xb1,
            0xf0, 0xae, 0x4c, 0x12, 0x91, 0xcf, 0x2d, 0x73,
            0xca, 0x94, 0x76, 0x28, 0xab, 0xf5, 0x17, 0x49,
            0x08, 0x56, 0xb4, 0xea, 0x69, 0x37, 0xd5, 0x8b,
            0x57, 0x09, 0xeb, 0xb5, 0x36, 0x68, 0x8a, 0xd4,
            0x95, 0xcb, 0x29, 0x77, 0xf4, 0xaa, 0x48, 0x16,
            0xe9, 0xb7, 0x55, 0x0b, 0x88, 0xd6, 0x34, 0x6a,
            0x2b, 0x75, 0x97, 0xc9, 0x4a, 0x14, 0xf6, 0xa8,
            0x74, 0x2a, 0xc8, 0x96, 0x15, 0x4b, 0xa9, 0xf7,
            0xb6, 0xe8, 0x0a, 0x54, 0xd7, 0x89, 0x6b, 0x35};

        // максимальное количество попыток связи
        private const int MAX_RETRIES_COUNT = 5;

        public const byte ENQ = 0x05;
        public const byte STX = 0x02;
        public const byte ACK = 0x06;
        public const byte NACK = 0x15;
        public const byte ETX = 0x03;
        public const byte SYN = 0x16;
        public const byte DLE = 0x10;

        public POSCommand(int nCmd)
        {
            m_Cmd = new byte[256];
            Array.Copy(Encoding.ASCII.GetBytes(nCmd.ToString()), m_Cmd, 3);
            m_CmdLen = 3;
        }

        private byte CalcBCC(byte[] nRequest, int nStart, int nLength)
        {
            byte bcc = 0;
            byte b = 0;

            for (int i = nStart; i < nStart + nLength; i++)
            {
                b = nRequest[i];
                bcc = CRC8_DATA[b ^ bcc];
            }

            return bcc;
        }

        public void AddNumeric(int nValue, int nLength)
        {
            string s = nValue.ToString("d" + nLength);
            Array.Copy(Encoding.ASCII.GetBytes(s), 0, m_Cmd, m_CmdLen, nLength);
            m_CmdLen += nLength;
        }

        public void AddChar(string sValue, int nLength)
        {            
            Array.Copy(Encoding.GetEncoding(866).GetBytes(sValue), 0, m_Cmd, m_CmdLen, nLength);
            m_CmdLen += nLength;
        }

        public void AddBChar(string sValue)
        {
            AddNumeric(sValue.Length, 3);
            AddChar(sValue, sValue.Length);
        }

        public byte[] GetCommandRequest()
        {
            byte STX = 0x02;
            byte ETX = 0x03;
            byte[] nRequest = new byte[256];

            m_FullLen = 0;
            nRequest[m_FullLen++] = STX;
            Array.Copy(m_Cmd, 0, nRequest, m_FullLen, m_CmdLen);
            m_FullLen += m_CmdLen;
            nRequest[m_FullLen++] = ETX;
            nRequest[m_FullLen++] = CalcBCC(nRequest, 1, m_FullLen - 2);

            return nRequest;
        }

        public short Execute(EasyCommunicationPort Port)
        {
            return Execute(Port, new byte[] { });
        }

        public short Execute(EasyCommunicationPort Port, byte[] nRsp)
        {
            byte nResponse;
            int nRetries = 0;

            try
            {
                Port.DiscardBuffers();

                // отправка запроса на передачу данных
                do
                {
                    nRetries++;
                    // если максимальное число попыток исчерпано, считаем, что вышел таймаут
                    if (nRetries > MAX_RETRIES_COUNT)
                        throw new TimeoutException();

                    Port.WriteByte(ENQ);
                    nResponse = (byte)Port.ReadByte();
                }
                while (nResponse != ACK);

                // передача данных и ожидание ответа
                nRetries = 0;
                do
                {
                    nRetries++;
                    // если максимальное число попыток исчерпано, считаем, что вышел таймаут
                    if (nRetries > MAX_RETRIES_COUNT)
                        throw new TimeoutException();

                    Port.Write(GetCommandRequest(), 0, m_FullLen);
                    nResponse = (byte)Port.ReadByte();
                }
                while (nResponse != ACK);
            }
            catch (TimeoutException)
            {
                return (short)GeneralError.Timeout;
            }

            return ReadResponse(Port, nRsp);
        }

        private short ReadResponse(EasyCommunicationPort Port, byte[] nRsp)
        {
            short nErrorCode = 0;
            bool bSuccess = false;
            byte[] nRequest = new byte[MAX_CMD_LEN];
            int nRetries = 0;
            short dwPrefixSize = 3 + 3;
            //                 |CMD| RC|
            byte nResponse = 0;
            int nRequestLen = 0;

            short dwDataSize = (short)nRsp.Length;

            try
            {
                do
                {
                    nRetries++;
                    // если максимальное число попыток исчерпано, считаем, что вышел таймаут
                    if (nRetries > MAX_RETRIES_COUNT)
                        throw new TimeoutException();
                    
                    Port.ReadTimeout = 600;
                    do
                        nResponse = (byte)Port.ReadByte();
                    while (nResponse == SYN);

                    nRequestLen = 0;
                    // чтение полей CMD и RC - 6 байт
                    nRequestLen += Port.Read(nRequest, nRequestLen, 6);
                    nErrorCode = Convert.ToInt16(Encoding.ASCII.GetString(nRequest, 3, 3));

                    // если ошибок не было, читаем данные ответа
                    if (nErrorCode == 0)
                        nRequestLen += Port.Read(nRequest, nRequestLen, nRsp.Length);

                    // читаем ETX и BCC
                    nRequestLen += Port.Read(nRequest, nRequestLen, 2);
                    bSuccess = CalcBCC(nRequest, 0, nRequestLen - 1) == nRequest[nRequestLen - 1];

                    // если контрольная сумма не сошлась, нужно запросить повтора отправки данных
                    if (!bSuccess)
                        Port.WriteByte(NACK);
                    // если все успешно, скопировать данные в nRsp
                    else
                        Array.Copy(nRequest, dwPrefixSize, nRsp, 0, nRequestLen - dwPrefixSize - 2);
                }
                while (!bSuccess);
            }
            catch (TimeoutException)
            {
                nErrorCode = (short)GeneralError.Timeout;
            }

            if (nErrorCode == 0)
                nErrorCode = (short)GeneralError.Success;

            return nErrorCode;
        }

    }
}
