using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Security.Permissions;
using ERPService.SharedLibs.Eventlog;

namespace ERPService.SharedLibs.Remoting.Sinks
{
    /// <summary>
    /// Провайдер приемников, принимающих имя серверного хоста или его IP-адрес, 
    /// видимые с клиента
    /// </summary>
    public class IpFixServerChannelSinkProvider : IServerChannelSinkProvider
    {
        // ссылка на следующего провайдера в цепи
        private IServerChannelSinkProvider _nextProvider;
        // для протоколирования событий
        private IEventLink _eventLink;

        #region Открытые свойства

        /// <summary>
        /// Для протоколирования событий
        /// </summary>
        public IEventLink EventLink
        {
            get { return _eventLink; }
            set { _eventLink = value; }
        }

        #endregion

        #region Конструкторы

        /// <summary>
        /// Конструктор для случая, когда ремоутинг настраивается с помощью файла конфигурации
        /// </summary>
        /// <param name="properties">Свойства провайдера</param>
        /// <param name="providerData">Данные провайдера</param>
        public IpFixServerChannelSinkProvider(IDictionary properties, ICollection providerData) 
        { 
        }

        /// <summary>
        /// Создает экземпляр провайдера
        /// </summary>
        public IpFixServerChannelSinkProvider()
        {
        }

        #endregion

        #region Реализация IServerChannelSinkProvider

        /// <summary>
        /// Создает серверный приемник сообщений
        /// </summary>
        /// <param name="channel">Канал, для которого создается приемник</param>
        /// <returns>Серверный приемник сообщений</returns>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public IServerChannelSink CreateSink(IChannelReceiver channel)
        {
            IServerChannelSink nextSink = null;

            if (_nextProvider != null)
                nextSink = _nextProvider.CreateSink(channel);
            return new IpFixServerChannelSink(nextSink, _eventLink);
        }

        /// <summary>
        /// Возвращает данные канала
        /// </summary>
        /// <param name="channelData">Объект, в котором возвращаются данные канала</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public void GetChannelData(IChannelDataStore channelData)
        {
        }

        /// <summary>
        /// Следующий провайдер в цепи
        /// </summary>
        public IServerChannelSinkProvider Next
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
    }
}
