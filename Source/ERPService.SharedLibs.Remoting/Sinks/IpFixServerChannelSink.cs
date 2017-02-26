using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Security.Permissions;
using System.IO;
using ERPService.SharedLibs.Eventlog;

namespace ERPService.SharedLibs.Remoting.Sinks
{
    /// <summary>
    /// Серверный приемник, принимающий имя серверного хоста или его IP-адрес, 
    /// видимые с клиента
    /// </summary>
    public class IpFixServerChannelSink : BaseChannelSinkWithProperties, IServerChannelSink
    {
        #region Поля

        private const string _eventSource = "Channel Sink";

        // ссылка на следующий приемник в цепи
        private IServerChannelSink _nextSink;
        // имя серверного хоста или его IP-адрес, видимые с клиента
        private Object _serverHostNameOrIp;
        // для протоколирования событий
        private IEventLink _eventLink;

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает экземпляр приемника
        /// </summary>
        /// <param name="nextSink">Ссылка на следующий приемник в цепи</param>
        /// <param name="eventLink">Для протоколирования событий</param>
        [SecurityPermission(SecurityAction.LinkDemand)]
        public IpFixServerChannelSink(IServerChannelSink nextSink, IEventLink eventLink)
        {
            if (nextSink == null) 
                throw new ArgumentNullException("nextSink");

            _nextSink = nextSink;
            _eventLink = eventLink;
        }

        #endregion

        #region Реализация IServerChannelSink

        /// <summary>
        /// Асинхронная обработка ответа
        /// </summary>
        /// <param name="sinkStack">Стек приемников</param>
        /// <param name="state">Информация, связанная с этим приемником</param>
        /// <param name="msg">Сообщение</param>
        /// <param name="headers">Заголовки сообщения</param>
        /// <param name="stream">Поток сообщения</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public void AsyncProcessResponse(IServerResponseChannelSinkStack sinkStack, object state, 
            IMessage msg, ITransportHeaders headers, Stream stream)
        {
            // перенаправляем вызов следующему приемнику в цепи
            sinkStack.AsyncProcessResponse(msg, headers, stream);
        }

        /// <summary>
        /// Возвращает поток, в который будет сериализовано ответное сообщение
        /// </summary>
        /// <param name="sinkStack">Стек приемников</param>
        /// <param name="state">Состояние, помещенное в стек этим приемником</param>
        /// <param name="msg">Сообщение</param>
        /// <param name="headers">Заголовки сообщения</param>
        /// <returns>Поток, в который будет сериализовано ответное сообщение</returns>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public Stream GetResponseStream(IServerResponseChannelSinkStack sinkStack, object state, 
            IMessage msg, ITransportHeaders headers)
        {
            return null;
        }

        /// <summary>
        /// Следующий приемник в цепи
        /// </summary>
        public IServerChannelSink NextChannelSink
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
            get 
            { 
                return _nextSink; 
            }
        }

        /// <summary>
        /// Обработка сообщения
        /// </summary>
        /// <param name="sinkStack">Стек приемников</param>
        /// <param name="requestMsg">Запрос</param>
        /// <param name="requestHeaders">Заголовки запроса</param>
        /// <param name="requestStream">Поток запроса</param>
        /// <param name="responseMsg">Ответ</param>
        /// <param name="responseHeaders">Заголовки ответа</param>
        /// <param name="responseStream">Поток ответа</param>
        /// <returns>Состояние обработки сообщения</returns>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public ServerProcessing ProcessMessage(IServerChannelSinkStack sinkStack, IMessage requestMsg, 
            ITransportHeaders requestHeaders, Stream requestStream, out IMessage responseMsg, 
            out ITransportHeaders responseHeaders, out Stream responseStream)
        {
            // проверяем, задано ли имя серверного хоста или его адрес
            _serverHostNameOrIp = requestHeaders["serverHostNameOrIp"];
            if (_serverHostNameOrIp != null)
            {
                // помещаем его в контекст вызова
                CallContext.SetData("serverHostNameOrIp", _serverHostNameOrIp);
                // протоколируем
                if (_eventLink != null)
                {
                    _eventLink.Post(_eventSource, string.Format("Имя или IP-адрес сервера ЗАДАНО: [{0}]",
                        _serverHostNameOrIp));
                }
            }
            else
            {
                // очишаем значение, оставшееся от предыдущих вызовов
                CallContext.FreeNamedDataSlot("serverHostNameOrIp");
                // протоколируем
                if (_eventLink != null)
                {
                    _eventLink.Post(_eventSource, EventType.Warning, "Имя или IP-адрес сервера НЕ задано");
                }
            }

            // перенаправляем обработку сообщения следующему приемнику в цепи
            sinkStack.Push(this, null);
            ServerProcessing status = _nextSink.ProcessMessage(sinkStack, requestMsg, requestHeaders, 
                requestStream, out responseMsg, out responseHeaders, out responseStream);

            return status;
        }

        #endregion
    }
}
