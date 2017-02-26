using System;

namespace DevicesCommon
{
    /// <summary>
    /// Интерфейс для работы с SMS
    /// </summary>
    public interface ISMSClient : IDevice, IDisposable
    {
        /// <summary>
        /// Отправка короткого текстового сообщения
        /// </summary>
        /// <param name="recipientNumber">Номер телефона получателя</param>
        /// <param name="messageText">Текст сообщения</param>
        /// <remarks>Номер телефона задается в международном формате: +123(456)789-01-23
        /// Текст сообщения может быть многострочным.</remarks>
        void Send(string recipientNumber, string messageText);

        /// <summary>
        /// Инициализация параметров для подключения, использующихся для отправки SMS
        /// </summary>
        /// <param name="paramName">Имя параметра</param>
        /// <param name="paramValue">Значение параметра</param>
        void SetConnectivityParam(string paramName, string paramValue);
    }
}
