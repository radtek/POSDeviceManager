using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using ERPService.SharedLibs.Remoting.Sinks;
using ERPService.SharedLibs.Eventlog;

namespace ERPService.SharedLibs.Remoting.Hosts
{
    /// <summary>
    /// Базовый класс для хостинга объектов, доступных через ремоутинг
    /// </summary>
    /// <typeparam name="T">Тип объекта, к которому нужно обеспечить доступ</typeparam>
    public abstract class CustomHost<T> : RemotingBase where T : HostingTarget
    {
        #region Поля

        // объект для публикации
        private T _target;
        // ссылка на объект, хранящий данные для генерации прокси к MBR-объекту
        private ObjRef _targetRef;
        // серверный канал
        private IChannel _channel;
        // логгер
        private IEventLink _eventLink;

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="target">Объект, к которому нужно обеспечить доступ</param>
        protected CustomHost(T target)
        {
            if (target == default(T))
                throw new ArgumentNullException("target");

            _target = target;
            _eventLink = null;
        }

        #endregion

        #region Открытые свойства и методы

        /// <summary>
        /// Объект для протоколирования работы хоста
        /// </summary>
        public IEventLink EventLink
        {
            get { return _eventLink; }
            set { _eventLink = value; }
        }

        /// <summary>
        /// Размещение объекта c использованием имени объекта и порта, определяемых пользователем
        /// </summary>
        /// <param name="objectName">Имя объекта</param>
        /// <param name="port">Порт</param>
        public void Marshal(string objectName, int port)
        {
            CheckCustomErrors();

            // регистрируем трэкер
            IpFixTrackingHandler.RegisterTracker(_eventLink);

            // создаем провайдера для приемников, отвечающего за форматирование сообщения
            IServerFormatterSinkProvider formatterSinkProvider = CreateFormatterSinkProvider();

            // создаем провайдера для приемников, принимающих имя серверного хоста 
            // или его IP-адрес, видимые с клиента
            IpFixServerChannelSinkProvider customSinkProvider = new IpFixServerChannelSinkProvider();
            customSinkProvider.EventLink = _eventLink;
            // связываем провайдеров в цепь
            customSinkProvider.Next = formatterSinkProvider;

            // создаем серверный канал
            _channel = CreateChannel(port, customSinkProvider, GetChannelName("server"));
            ChannelServices.RegisterChannel(_channel, false);

            // размещаем объект
            _targetRef = RemotingServices.Marshal(_target, objectName);
        }

        /// <summary>
        /// Размещение объекта c использованием имени объекта и порта, указанных в объекте
        /// </summary>
        public void Marshal()
        {
            Marshal(_target.Name, _target.Port);
        }


        /// <summary>
        /// Размещение объекта c использованием имени объекта, определяемого пользователем
        /// </summary>
        /// <param name="objectName">Имя объекта</param>
        /// <remarks>Используется порт, указанный в объекте</remarks>
        public void Marshal(string objectName)
        {
            Marshal(objectName, _target.Port);
        }

        /// <summary>
        /// Размещение объекта c использованием порта, определяемого пользователем
        /// </summary>
        /// <param name="port">Порт</param>
        /// <remarks>Используется имя объекта, указанное в объекте</remarks>
        public void Marshal(int port)
        {
            Marshal(_target.Name, port);
        }

        /// <summary>
        /// Прекращение доступа к объекту через ремоутинг
        /// </summary>
        /// <param name="disposeTarget">Вызывать IDisposable.Dispose у объекта, 
        /// к которому обеспечивался доступ<see cref="System.IDisposable"/></param>
        public void Unmarshal(bool disposeTarget)
        {
            if (_targetRef != null)
            {
                RemotingServices.Unmarshal(_targetRef);
                _targetRef = null;
            }

            SafeUnregisterChannel(_channel);

            if (_target != null && disposeTarget)
                _target.Dispose();
        }

        /// <summary>
        /// Прекращение доступа к объекту через ремоутинг c вызовом IDisposable.Dispose у объекта, 
        /// к которому обеспечивался доступ<see cref="System.IDisposable"/>
        /// </summary>
        public void Unmarshal()
        {
            Unmarshal(true);
        }

        /// <summary>
        /// Объект, к которому обеспечивается доступ
        /// </summary>
        public T Target
        {
            get { return _target; }
        }

        #endregion

        #region Для перегрузки в наследниках

        /// <summary>
        /// Создает провайдер для приемников, отвечающих за форматирование сообщений
        /// </summary>
        /// <returns>Провайдер для приемников, отвечающих за форматирование сообщений</returns>
        protected abstract IServerFormatterSinkProvider CreateFormatterSinkProvider();

        /// <summary>
        /// Создает серверный канал
        /// </summary>
        /// <param name="sinkProvider">Провайдер приемников канала</param>
        /// <param name="channelName">Имя канала</param>
        /// <param name="port">Порт для публикации объекта</param>
        /// <returns>Серверный канал</returns>
        protected abstract IChannel CreateChannel(int port, 
            IServerChannelSinkProvider sinkProvider, string channelName);

        #endregion

        #region Закрытые методы

        private void CheckCustomErrors()
        {
            if (RemotingConfiguration.CustomErrorsMode != CustomErrorsModes.Off)
                RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
        }

        #endregion

        #region Перегрузка методов базового класса

        /// <summary>
        /// Прекращение доступа к объекту через ремоутинг c вызовом IDisposable.Dispose у объекта, 
        /// к которому обеспечивался доступ<see cref="System.IDisposable"/>
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            Unmarshal();
        }

        #endregion
    }
}
