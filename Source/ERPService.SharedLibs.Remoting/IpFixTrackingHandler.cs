using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Services;
using ERPService.SharedLibs.Eventlog;

namespace ERPService.SharedLibs.Remoting
{
    /// <summary>
    /// Объект, получающий уведомления об размешении, удалении и отключении объектов,
    /// доступных через ремоутинг-инфраструктуру
    /// </summary>
    public class IpFixTrackingHandler : ITrackingHandler
    {
        #region Реализация синглтона

        // имя источника событий
        private const string EventSource = "Tracking handler";

        // для синхронизации
        private static Object _syncObject = new Object();
        // экземпляр 
        private static IpFixTrackingHandler _instance = null;
        // логгер
        private IEventLink _eventLink;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="eventLink">Логгер</param>
        /// <remarks>Потрбуется всего один трэкер данного типа, поэтому конструктор закрыт</remarks>
        private IpFixTrackingHandler(IEventLink eventLink)
        {
            _eventLink = eventLink;
        }

        /// <summary>
        /// Регистрация трэкера в ремоутинг-инфраструктуре
        /// </summary>
        public static void RegisterTracker(IEventLink eventLink)
        {
            lock (_syncObject)
            {
                if (_instance == null)
                    // создаем трэкер, если он еще не создан
                    _instance = new IpFixTrackingHandler(eventLink);

                // проверяем, не зарегистрирован ли еще один такой же трэкер
                foreach (ITrackingHandler thrackingHandler in TrackingServices.RegisteredHandlers)
                {
                    if (thrackingHandler is IpFixTrackingHandler)
                        // второй такой же объект регистрировать не нужно
                        return;
                }

                // регистрируем его
                TrackingServices.RegisterTrackingHandler(_instance);
            }
        }

        #endregion

        /// <summary>
        /// Замена имени хоста или IP-адреса
        /// </summary>
        /// <param name="dataStore">Данные канала</param>
        /// <param name="serverHostNameOrIp">Имя или адрес хоста, переданное с клиента</param>
        private void ReplaceHostNameOrIp(ChannelDataStore dataStore, string serverHostNameOrIp)
        {
            for (int i = 0; i < dataStore.ChannelUris.Length; i++)
            {
                if (_eventLink != null)
                {
                    _eventLink.Post(EventSource, string.Format(
                        "Исходный URI в данных канала связи: {0}", dataStore.ChannelUris[i]));
                }

                UriBuilder ub = new UriBuilder(dataStore.ChannelUris[i]);
                
                // сравниваем имя хоста в URI канала и то же, переданное с клиента
                if (string.Compare(ub.Host, serverHostNameOrIp, true) != 0)
                {
                    // меняем на значение, переданное с клиента
                    ub.Host = serverHostNameOrIp;
                    dataStore.ChannelUris[i] = ub.ToString();

                    if (_eventLink != null)
                    {
                        _eventLink.Post(EventSource, string.Format(
                            "Хост изменен. Новый URI: {0}", dataStore.ChannelUris[i]));
                    }
                }
            }
        }

        #region Реализация ITrackingHandler

        /// <summary>
        /// Отключение объекта от его прокси
        /// </summary>
        /// <param name="obj">Отключенный объект</param>
        public void DisconnectedObject(object obj)
        {
        }

        /// <summary>
        /// Размещение объекта
        /// </summary>
        /// <param name="obj">Объект</param>
        /// <param name="or">ObjRef объекта<see cref="System.Runtime.Remoting.ObjRef"/></param>
        public void MarshaledObject(object obj, ObjRef or)
        {
            // пытаемся получить имя или адрес серверного хоста из контекста вызова
            Object serverHostNameOrIp = CallContext.GetData("serverHostNameOrIp");
            if (_eventLink != null)
            {
                _eventLink.Post(EventSource, string.Format("Публикация объекта {0}, URI {1}", 
                    obj.GetType(), or.URI));
                _eventLink.Post(EventSource, string.Format(
                    "Имя или IP-адрес сервера, полученное из контекста вызова: [{0}]", serverHostNameOrIp));
            }

            if (serverHostNameOrIp != null)
            {
                // проверим, есть ли необходимость в подмене имени хоста или адреса для 
                // размещаемого объекта
                foreach (Object channelData in or.ChannelInfo.ChannelData)
                {
                    if (channelData is ChannelDataStore)
                    {
                        if (_eventLink != null)
                            _eventLink.Post(EventSource, "Обнаружены данные канала связи");

                        // URI, по которому доступен размешаемый объект, 
                        // хранится в ChannelDataStore. Объектов типа ChannelDataStore
                        // может быть несколько, в зависимости от числа серверных каналов,
                        // по которым можно получить доступ к объектам
                        ReplaceHostNameOrIp((ChannelDataStore)channelData, serverHostNameOrIp.ToString());
                    }
                }
            }

            if (_eventLink != null)
            {
                _eventLink.Post(EventSource, string.Format("Объект {0}, URI {1} опубликован",
                    obj.GetType(), or.URI));
            }
        }

        /// <summary>
        /// Удаление объекта
        /// </summary>
        /// <param name="obj">Объект</param>
        /// <param name="or">ObjRef объекта<see cref="System.Runtime.Remoting.ObjRef"/></param>
        public void UnmarshaledObject(object obj, ObjRef or)
        {
        }

        #endregion
    }
}
