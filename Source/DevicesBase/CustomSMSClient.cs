using System.Collections.Generic;
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

        private IDictionary<string, string> _connectivityParams;

        #endregion

        /// <summary>
        /// ������� ��������� SMS-�������
        /// </summary>
        protected CustomSMSClient()
            : base()
        {
            _connectivityParams = new Dictionary<string, string>();
        }

        /// <summary>
        /// ��������� ��� �����������, �������������� ��� �������� SMS
        /// </summary>
        protected IDictionary<string, string> ConnectivityParams
        {
            get { return _connectivityParams; }
        }

        #region ���������� ISMSClient Members

        /// <summary>
        /// �������� ��������� ���������� ���������
        /// </summary>
        /// <param name="recipientNumber">����� �������� ���������� ���������</param>
        /// <param name="messageText">����� ���������</param>
        public void Send(string recipientNumber, string messageText)
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
        public void SetConnectivityParam(string paramName, string paramValue)
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
        protected abstract EncodedMessage[] OnEncode(string messageText, 
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
