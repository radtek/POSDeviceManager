using System;
using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Security.Permissions;

namespace ERPService.SharedLibs.Remoting.Sinks
{
    /// <summary>
    /// Провайдер для приемников, передающих имя серверного хоста или его IP-адрес, 
    /// видимые с клиента
    /// </summary>
    public class IpFixClientChannelSinkProvider : IClientChannelSinkProvider
    {
        #region Поля

        // ссылка на следующего провайдера в цепи
        private IClientChannelSinkProvider _nextProvider;
        // имя серверного хоста или его IP-адрес, видимые с клиента
        private string _serverHostNameOrIp;

        #endregion

        #region Конструкторы

        /// <summary>
        /// Конструктор для случая, когда ремоутинг настраивается с помощью файла конфигурации
        /// </summary>
        /// <param name="properties">Свойства провайдера</param>
        /// <param name="providerData">Данные провайдера</param>
        public IpFixClientChannelSinkProvider(IDictionary properties, ICollection providerData)
        {
        }

        /// <summary>
        /// Создает экземпляр провайдера
        /// </summary>
        /// <param name="serverHostNameOrIp">Имя серверного хоста или его IP-адрес, видимые с клиента</param>
        public IpFixClientChannelSinkProvider(string serverHostNameOrIp)
        {
            if (string.IsNullOrEmpty(serverHostNameOrIp))
                throw new ArgumentNullException("serverHostNameOrIp");

            _serverHostNameOrIp = serverHostNameOrIp;
        }

        #endregion

        #region Реализация IClientChannelSinkProvider

        /// <summary>
        /// Создает клиентский приемник сообщений
        /// </summary>
        /// <param name="channel">Канал, для которого создается приемник</param>
        /// <param name="url">Адрес удаленного объекта, к которому будет выполняться подключение</param>
        /// <param name="remoteChannelData">Данные канала</param>
        /// <returns>Клиентский приемник сообщений</returns>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public IClientChannelSink CreateSink(IChannelSender channel, string url, object remoteChannelData)
        {
            IClientChannelSink nextSink = null;

            if (_nextProvider != null)
                // создаем следующий приемник в цепи
                nextSink = _nextProvider.CreateSink(channel, url, remoteChannelData);

            // добавляем наш приемник в цепь
            return new IpFixClientChannelSink(nextSink, _serverHostNameOrIp);
        }

        /// <summary>
        /// Следующий провайдер в цепи
        /// </summary>
        public IClientChannelSinkProvider Next
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
            get
            {
                return _nextProvider;
            }
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
            set
            {
                _nextProvider = value;
            }
        }

        #endregion

        #region Прочие свойства

        /// <summary>
        /// Имя серверного хоста или его IP-адрес, видимые с клиента
        /// </summary>
        public string ServerHostNameOrIp
        {
            get
            {
                return _serverHostNameOrIp;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value");

                _serverHostNameOrIp = value;
            }
        }

        #endregion
    }
}
