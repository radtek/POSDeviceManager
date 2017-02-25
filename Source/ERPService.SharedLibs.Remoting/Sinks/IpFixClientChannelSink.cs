using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Security.Permissions;
using System.IO;
using System.Collections;

namespace ERPService.SharedLibs.Remoting.Sinks
{
    /// <summary>
    /// Клиентский приемник, передающий имя серверного хоста или его IP-адрес, 
    /// видимые с клиента
    /// </summary>
    public class IpFixClientChannelSink : BaseChannelSinkWithProperties, IClientChannelSink
    {
        #region Поля

        // ссылка на следующий приемник в цепи
        private IClientChannelSink _nextSink;
        // имя серверного хоста или его IP-адрес, выдимые с клиента
        private String _serverHostNameOrIp;

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает клиентский приемник
        /// </summary>
        /// <param name="nextSink">Ссылка на следующий приемник в цепи</param>
        /// <param name="serverHostNameOrIp">Имя серверного хоста или его IP-адрес, видимые с клиента</param>
        [SecurityPermission(SecurityAction.LinkDemand)]
        public IpFixClientChannelSink(IClientChannelSink nextSink, String serverHostNameOrIp)
        {
            if (nextSink == null)
                throw new ArgumentNullException("nextSink");
            if (String.IsNullOrEmpty(serverHostNameOrIp))
                throw new ArgumentNullException("serverHostNameOrIp");

            _nextSink = nextSink;
            _serverHostNameOrIp = serverHostNameOrIp;
        }

        #endregion

        #region Реализация IClientChannelSink

        /// <summary>
        /// Асинхронная обработка запроса
        /// </summary>
        /// <param name="sinkStack">Стек приемников</param>
        /// <param name="msg">Сообщение</param>
        /// <param name="headers">Заголовки запроса</param>
        /// <param name="stream">Поток запроса</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public void AsyncProcessRequest(IClientChannelSinkStack sinkStack, IMessage msg, 
            ITransportHeaders headers, Stream stream)
        {
            // перенаправляем вызов следующему приемнику в стеке
            sinkStack.Push(this, null);
            _nextSink.AsyncProcessRequest(sinkStack, msg, headers, stream);
        }

        /// <summary>
        /// Асинхронная обработка ответа
        /// </summary>
        /// <param name="sinkStack">Стек приемников</param>
        /// <param name="state">Информация, связанная с этим приемником</param>
        /// <param name="headers">Заголовки ответа</param>
        /// <param name="stream">Поток ответа</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public void AsyncProcessResponse(IClientResponseChannelSinkStack sinkStack, object state, 
            ITransportHeaders headers, Stream stream)
        {
            // перенаправляем вызов следующему приемнику в стеке
            sinkStack.AsyncProcessResponse(headers, stream);
        }

        /// <summary>
        /// Возвращает поток, в который будет сериализовано сообщение
        /// </summary>
        /// <param name="msg">Сообщение</param>
        /// <param name="headers">Заголовки сообщения</param>
        /// <returns>Поток, в который будет сериализовано сообщение</returns>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public Stream GetRequestStream(IMessage msg, ITransportHeaders headers)
        {
            // перенаправляем вызов следующему приемнику в стеке
            return _nextSink.GetRequestStream(msg, headers);
        }

        /// <summary>
        /// Ссылка на следующий приемник в цепи
        /// </summary>
        public IClientChannelSink NextChannelSink
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
            get 
            { 
                return _nextSink; 
            }
        }

        /// <summary>
        /// Обработка сообщения приемником
        /// </summary>
        /// <param name="msg">Сообщение</param>
        /// <param name="requestHeaders">Заголовки запроса</param>
        /// <param name="requestStream">Поток запроса</param>
        /// <param name="responseHeaders">Заголовки ответа</param>
        /// <param name="responseStream">Поток ответа</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public void ProcessMessage(IMessage msg, ITransportHeaders requestHeaders, Stream requestStream, 
            out ITransportHeaders responseHeaders, out Stream responseStream)
        {
            // помещаем имя серверного хоста или его IP-адрес в заголовки запроса
            requestHeaders["serverHostNameOrIp"] = _serverHostNameOrIp;

            // перенаправляем вызов следующему приемнику в стеке
            _nextSink.ProcessMessage(msg, requestHeaders, requestStream, 
                out responseHeaders, out responseStream);
        }

        #endregion
    }
}
