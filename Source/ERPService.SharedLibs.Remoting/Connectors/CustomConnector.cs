using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using ERPService.SharedLibs.Remoting.Sinks;

namespace ERPService.SharedLibs.Remoting.Connectors
{
    /// <summary>
    /// Базовый класс для подключения к ремоутинг-объектам
    /// </summary>
    /// <typeparam name="T">Интерфейс объекта</typeparam>
    public abstract class CustomConnector<T>: RemotingBase
    {
        #region Поля

        private T _remoteObject;
        private IChannel _channel;
        private string _serverNameOrIp;
        private int _port;
        private string _objectName;
        private string _url;
        private int _timeout;

        #endregion

        #region Константы

        private const string readableServerNameOrIp = "Имя или IP-адрес сервера";
        private const string readableObjectName = "Имя объекта";
        
        /// <summary>
        /// Имя локального сервера
        /// </summary>
        protected const string Localhost = "localhost";

        #endregion

        /// <summary>
        /// Генерация исключения, если не инициализировано строковое свойство
        /// </summary>
        /// <param name="value">Значение свойства для проверки</param>
        /// <param name="readableParamName">Читаемое название свойства</param>
        protected void ThrowIfEmpty(string value, string readableParamName)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value",
                    string.Format("Не задано свойство \"{0}\"", readableParamName));
        }

        /// <summary>
        /// Генерация исключения, если значение TCP-порта вне диапазона
        /// </summary>
        /// <param name="value">Значение для проверки</param>
        protected void ThrowIfOutOfRange(int value)
        {
            if (value < 0 || value > UInt16.MaxValue)
                throw new ArgumentOutOfRangeException("value", value, "Значение TCP-порта вне диапазона");
        }

        /// <summary>
        /// Создает набор свойств канала, добавляет в него основные свойства,
        /// общие для всех типов каналов
        /// </summary>
        /// <param name="channelName">Имя канала</param>
        /// <returns>Набор свойств канала</returns>
        protected IDictionary GetBasicChannelProperties(string channelName)
        {
            IDictionary channelProps = new Hashtable();
            
            channelProps["name"] = channelName;
            channelProps["ServerNameOrIp"] = _serverNameOrIp;;
            channelProps["timeout"] = _timeout;
            
            return channelProps;
        }

        #region Конструкторы

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="serverNameOrIp">Имя или IP-адрес сервера</param>
        /// <param name="port">Порт сервера</param>
        /// <param name="objectName">Имя объекта</param>
        protected CustomConnector(string serverNameOrIp, int port, string objectName)
        {
            ServerNameOrIp = serverNameOrIp;
            Port = port;
            ObjectName = objectName;
            Timeout = -1;
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <remarks>Требует дополнительной инициализации свойств</remarks>
        protected CustomConnector()
        {
            Timeout = -1;
        }

        #endregion

        #region Открытые свойства и методы

        /// <summary>
        /// Таймаут ремоутинг-вызова
        /// </summary>
        public int Timeout
        {
            get { return _timeout; }
            set
            {
                if (value < -1 || value > int.MaxValue)
                    throw new ArgumentOutOfRangeException("value");

                _timeout = value;
            }
        }

        /// <summary>
        /// Имя или IP-адрес сервера
        /// </summary>
        public string ServerNameOrIp
        {
            get { return _serverNameOrIp; }
            set 
            {
                ThrowIfEmpty(value, readableServerNameOrIp);
                _serverNameOrIp = value; 
            }
        }

        /// <summary>
        /// Порт сервера
        /// </summary>
        public int Port
        {
            get { return _port; }
            set 
            {
                ThrowIfOutOfRange(value);
                _port = value;
            }
        }

        /// <summary>
        /// Имя объекта
        /// </summary>
        public string ObjectName
        {
            get { return _objectName; }
            set 
            {
                ThrowIfEmpty(value, readableObjectName);
                _objectName = value; 
            }
        }

        /// <summary>
        /// Ремоутинг-объект
        /// </summary>
        public T RemoteObject
        {
            get
            {
                if (_remoteObject == null)
                {
                    // проверка значений свойств
                    ThrowIfEmpty(_serverNameOrIp, readableServerNameOrIp);
                    ThrowIfEmpty(_objectName, readableObjectName);
                    ThrowIfOutOfRange(_port);

                    // создаем цепь провайдеров приемников
                    IpFixClientChannelSinkProvider customSinkProvider = 
                        new IpFixClientChannelSinkProvider(_serverNameOrIp);

                    // помещаем провайдер приемников, передающих имя серверного хоста или его IP-адрес, 
                    // видимые с клиента, после провайдера примеников, отвечающих за форматирование сообщений
                    IClientFormatterSinkProvider formatterSinkProvider = CreateFormatterSinkProvider();
                    formatterSinkProvider.Next = customSinkProvider;

                    // создаем и регистрируем канал
                    _channel = CreateChannel(formatterSinkProvider, GetChannelName("client"));
                    ChannelServices.RegisterChannel(_channel, false);

                    // формируем URL объекта
                    _url = string.Format("{0}://{1}:{2}/{3}", Protocol, _serverNameOrIp, _port, _objectName);

                    // создаем прокси объекта
                    _remoteObject = (T)Activator.GetObject(typeof(T), _url);
                }

                return _remoteObject;
            }
        }

        #endregion

        #region Для реализации в классах-наследниках

        /// <summary>
        /// Префикс протокола связи с объектом
        /// </summary>
        protected abstract string Protocol { get; }

        /// <summary>
        /// Создает провайдер для приемников, отвечающих за форматирование сообщений
        /// </summary>
        /// <returns>Провайдер для приемников, отвечающих за форматирование сообщений</returns>
        protected abstract IClientFormatterSinkProvider CreateFormatterSinkProvider();

        /// <summary>
        /// Создает клиентский канал
        /// </summary>
        /// <param name="sinkProvider">Провайдер приемников канала</param>
        /// <param name="channelName">Имя канала</param>
        /// <returns>Клиентский канал</returns>
        protected abstract IChannel CreateChannel(IClientChannelSinkProvider sinkProvider,
            string channelName);

        #endregion

        #region Перегрузка методов базового класса

        /// <summary>
        /// Закрытие подключения к ремоутинг-объекту
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            if (_remoteObject != null)
                _remoteObject = default(T);
            
            SafeUnregisterChannel(_channel);
        }

        #endregion
    }
}
