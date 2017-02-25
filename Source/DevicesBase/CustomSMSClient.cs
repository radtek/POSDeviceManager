using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon;
using ERPService.SharedLibs.Helpers;

namespace DevicesBase
{
    /// <summary>
    /// ������� ����� ��� ������ � SMS
    /// </summary>
    public abstract class CustomSMSClient : CustomDevice, ISMSClient
    {
        #region ����

        private IDictionary<String, String> _connectivityParams;

        #endregion

        /// <summary>
        /// ������� ��������� SMS-�������
        /// </summary>
        protected CustomSMSClient()
            : base()
        {
            _connectivityParams = new Dictionary<String, String>();
        }

        /// <summary>
        /// ��������� ��� �����������, �������������� ��� �������� SMS
        /// </summary>
        protected IDictionary<String, String> ConnectivityParams
        {
            get { return _connectivityParams; }
        }

        #region ���������� ISMSClient Members

        /// <summary>
        /// �������� ��������� ���������� ���������
        /// </summary>
        /// <param name="recipientNumber">����� �������� ���������� ���������</param>
        /// <param name="messageText">����� ���������</param>
        public void Send(String recipientNumber, String messageText)
        {
            // �������� ��������� �/��� ��������� �� �����
            EncodedMessage[] messages = OnEncode(messageText, 
                new PhoneNumber(recipientNumber));

            // ���������� ���������
            OnSend(messages);
        }

        /// <summary>
        /// ������������� ���������� ��� �����������, �������������� ��� �������� SMS
        /// </summary>
        /// <param name="paramName">��� ���������</param>
        /// <param name="paramValue">�������� ���������</param>
        public void SetConnectivityParam(String paramName, String paramValue)
        {
            if (_connectivityParams.ContainsKey(paramName))
                _connectivityParams[paramName] = paramValue;
            else
                _connectivityParams.Add(paramName, paramValue);
        }

        #endregion

        #region ������ ��� ���������� � �������-��������

        /// <summary>
        /// ����������� ��������� ����� ���������
        /// </summary>
        /// <param name="messageText">����� ���������</param>
        /// <param name="recipient">����� �������� �����������</param>
        /// <returns>�������������� ����� ���������</returns>
        protected abstract EncodedMessage[] OnEncode(String messageText, 
            PhoneNumber recipient);

        /// <summary>
        /// �������� ���������
        /// </summary>
        /// <param name="messages">�������� ���������</param>
        protected abstract void OnSend(EncodedMessage[] messages);

        #endregion

        #region ���������� IDisposable

        /// <summary>
        /// ������������ ��������
        /// </summary>
        public virtual void Dispose()
        {
        }

        #endregion
    }
}
