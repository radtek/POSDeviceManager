using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using DevicesBase;
using DevicesCommon;
using DevicesCommon.Helpers;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace T283DualTripod
{
    /// <summary>
    /// ���������� ������ ���������� ���������� �283 � ������� ����������� NL-16D0-DI3
    /// </summary>
    [TurnstileDevice(DeviceNames.t283dualTripod)]
    public class T283TurnstileDevice : CustomTurnstileDevice
    {
        private const String RfidMask =
            @"(?:\w*\u002D\w*)(?:\u005B\w+\u005D)(?:\s*)(\d+)(?:\u002C)(\d+)";

        private UInt16 _outStatus;

        public T283TurnstileDevice()
            : base()
        {
            _outStatus = 0;
        }

        protected override void OnAfterActivate()
        {
            Port.DataBits = 8;
            Port.StopBits = StopBits.One;
            Port.DsrFlow = false;
            Port.WriteTimeout = 2000;
            Port.ReadTimeout = 2000;
            Port.DiscardBuffers();
            base.OnAfterActivate();
        }

        #region �������� ������

        /// <summary>
        /// ������� ������
        /// </summary>
        /// <param name="pow">���������� �������</param>
        private UInt16 Pow2(UInt16 pow)
        {
            UInt16 result = 1;
            if (pow != 0)
            {
                for (Byte i = 0; i < pow; i++)
                    result *= 2;
            }
            return result;
        }

        /// <summary>
        /// ��������� ��������� ����������� ������
        /// </summary>
        /// <param name="dOut">����� ������</param>
        /// <param name="outOn">����� �������</param>
        private void SetOutStatus(Int32 dOut, Boolean outOn)
        {
            // ����� ��������� �������
            UInt16 newOutStatus = _outStatus;
            if (outOn)
                newOutStatus |= Pow2((UInt16)dOut);
            else
                newOutStatus &= (UInt16)~Pow2((UInt16)dOut);

            Byte[] statusBytes = BitConverter.GetBytes(newOutStatus);

            // ��������� ������� ��� ������ � ����������
            String command = String.Format("@{0:X2}{1:X2}{2:X2}", Address, statusBytes[1], statusBytes[0]);
            ExecuteCommand(command, 2, true, false, ">");

            _outStatus = newOutStatus;
        }

        /// <summary>
        /// �������� ������ �� ������������
        /// </summary>
        /// <param name="answer">�����</param>
        /// <param name="valueToCompareWith">���������� �����</param>
        /// <param name="command">�������</param>
        private void TestAnswer(String command, String answer, String valueToCompareWith)
        {
            if (String.Compare(answer, valueToCompareWith) != 0)
                throw new OperationCanceledException(
                    String.Format("������� \"{0}\" �� ���������. �����: \"{1}\"", command, answer));
        }

        /// <summary>
        /// ���������� �������
        /// </summary>
        /// <param name="command">����� �������</param>
        /// <param name="answerLen">��������� ����� ������</param>
        /// <param name="cr">�������� ������� CR</param>
        /// <param name="lf">�������� ������� LF</param>
        /// <returns>����� �� �������</returns>
        private String ExecuteCommand(String command, Int32 answerLen, Boolean cr, Boolean lf)
        {
            // ���������� �������
            StringBuilder preparedCommand = new StringBuilder(command);
            if (cr)
                preparedCommand.Append("\r");
            if (lf)
                preparedCommand.Append("\n");

            Int32 retryCount = 5;
            do
            {
                try
                {
                    // �������� �������
                    Byte[] commandBytes = Encoding.Default.GetBytes(preparedCommand.ToString());
                    Port.DiscardBuffers();
                    Port.Write(commandBytes);

                    // ������ �����
                    Byte[] answer = new Byte[answerLen];
                    Port.Read(answer, 0, answerLen);

                    // ���������� ����� � ��������� ����
                    StringBuilder sb = new StringBuilder();
                    foreach (Byte b in answer)
                    {
                        if (b > 0x20)
                            sb.Append((Char)b);
                    }
                    return sb.ToString();
                }
                catch (TimeoutException)
                {
                    retryCount--;
                    Port.ClearError();
                }
                catch (Win32Exception)
                {
                    retryCount--;
                    Port.ClearError();
                }
            }
            while (retryCount > 0);
            return String.Empty;
        }

        private void ExecuteCommand(String command, Int32 answerLen, Boolean cr, Boolean lf, 
            String answerToCompareWith)
        {
            Int32 retryCount = 50;
            String answer = String.Empty;
            do
            {
                answer = ExecuteCommand(command, answerLen, cr, lf);
                if (String.Compare(answer, answerToCompareWith) == 0)
                    return;
                else
                {
                    retryCount--;
                    System.Threading.Thread.Sleep(50);
                }
            }
            while (retryCount > 0);
            TestAnswer(command, answer, answerToCompareWith);
        }

        #endregion

        #region ���������� ������� � ������� ������������ ������

        protected override Byte TermChar
        {
            get { return 0x0D; }
        }

        protected override String OnReadIdData()
        {
            Byte[] idData = new Byte[1024];
            Int32 zeroReads = 0;

            Port.ReadTimeout = -1;
            try
            {
                StringBuilder rawData = new StringBuilder();
                Boolean complete = false;
                do
                {
                    Array.Clear(idData, 0, idData.Length);
                    Int32 bytesRead = Port.Read(idData, 0, idData.Length);

                    if (bytesRead > 0)
                    {
                        for (Int32 i = 0; i < bytesRead; i++)
                        {
                            complete = idData[i] == 0x0A;
                            if (complete)
                                break;
                            else if (idData[i] >= 0x20)
                            {
                                rawData.Append((Char)idData[i]);
                            }
                        }
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(100);
                        zeroReads++;
                    }
                }
                while (!complete && zeroReads < 10);
                
                Match match = Regex.Match(rawData.ToString(), RfidMask);
                return match.Success ? 
                    String.Concat(match.Groups[1].Value, match.Groups[2].Value) : String.Empty;
            }
            finally
            {
                Port.ReadTimeout = 2000;
                Port.DiscardBuffers();
            }
        }

        protected override void OnRed(Boolean flashOn)
        {
            SetOutStatus(14, flashOn);
        }

        protected override void OnGreen(bool flashOn)
        {
            SetOutStatus(13, flashOn);
        }

        protected override void OnBeep(bool beepOn)
        {
            SetOutStatus(15, beepOn);
        }

        protected override void OnOpen()
        {
            SetOutStatus(Direction == TurnstileDirection.Entry ? 0 : 1, true);
        }

        protected override void OnClose()
        {
            SetOutStatus(Direction == TurnstileDirection.Entry ? 0 : 1, false);
        }

        protected override bool OnPassComplete()
        {
            String answer = String.Empty;
            Boolean complete = false;
            do
            {
                answer = ExecuteCommand(String.Format("^{0:X2}DI", Address), 6, true, false);
                complete = answer.Length == 6 && answer[0] == '!';
            }
            while (!complete);
            return answer[5] == '0';
        }

        #endregion
    }
}
