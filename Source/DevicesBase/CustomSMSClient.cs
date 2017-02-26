using System.Collections.Generic;
using DevicesCommon;
using ERPService.SharedLibs.Helpers;

namespace DevicesBase
{
    /// <summary>
    /// Базовый класс для работы с SMS
    /// </summary>
    public abstract class CustomSMSClient : CustomDevice, ISMSClient
    {
        #region Поля

        private IDictionary<string, string> _connectivityParams;

        #endregion

        /// <summary>
        /// Создает экземпляр SMS-клиента
        /// </summary>
        protected CustomSMSClient()
            : base()
        {
            _connectivityParams = new Dictionary<string, string>();
        }

        /// <summary>
        /// Параметры для подключения, использующиеся для отправки SMS
        /// </summary>
        protected IDictionary<string, string> ConnectivityParams
        {
            get { return _connectivityParams; }
        }

        #region Реализация ISMSClient Members

        /// <summary>
        /// Отправка короткого текстового сообщения
        /// </summary>
        /// <param name="recipientNumber">Номер телефона получаетля сообщения</param>
        /// <param name="messageText">Текст сообщения</param>
        public void Send(string recipientNumber, string messageText)
        {
            // кодируем сообщения и/или разбиваем на части
            EncodedMessage[] messages = OnEncode(messageText, 
                new PhoneNumber(recipientNumber));

            // отправляем сообщения
            OnSend(messages);
        }

        /// <summary>
        /// Инициализация параметров для подключения, использующихся для отправки SMS
        /// </summary>
        /// <param name="paramName">Имя параметра</param>
        /// <param name="paramValue">Значение параметра</param>
        public void SetConnectivityParam(string paramName, string paramValue)
        {
            if (_connectivityParams.ContainsKey(paramName))
                _connectivityParams[paramName] = paramValue;
            else
                _connectivityParams.Add(paramName, paramValue);
        }

        #endregion

        #region Методы для реализации в классах-потомках

        /// <summary>
        /// Кодирование сообщения перед отправкой
        /// </summary>
        /// <param name="messageText">Текст сообщения</param>
        /// <param name="recipient">Номер телефона отправителя</param>
        /// <returns>Закодированные части сообщения</returns>
        protected abstract EncodedMessage[] OnEncode(string messageText, 
            PhoneNumber recipient);

        /// <summary>
        /// Отправка сообщений
        /// </summary>
        /// <param name="messages">Отправка сообщений</param>
        protected abstract void OnSend(EncodedMessage[] messages);

        #endregion

        #region Реализация IDisposable

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public virtual void Dispose()
        {
        }

        #endregion
    }
}
