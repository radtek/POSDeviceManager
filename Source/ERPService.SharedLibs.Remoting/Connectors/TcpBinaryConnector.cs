using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;

namespace ERPService.SharedLibs.Remoting.Connectors
{
    /// <summary>
    /// Класс для подключения к ремоутинг-объектам по TCP, бинарное форматирование сообщений
    /// </summary>
    /// <typeparam name="T">Интерфейс объекта</typeparam>
    public class TcpBinaryConnector<T> : CustomTcpConnector<T>
    {
        /// <summary>
        /// Создает провайдер для приемников, отвечающих за форматирование сообщений
        /// </summary>
        /// <returns>Провайдер для приемников, отвечающих за форматирование сообщений</returns>
        protected override IClientFormatterSinkProvider CreateFormatterSinkProvider()
        {
            return new BinaryClientFormatterSinkProvider();
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        public TcpBinaryConnector()
            : base()
        {
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="serverNameOrIp">Имя или IP-адрес сервера</param>
        /// <param name="port">Порт сервера</param>
        /// <param name="objectName">Имя объекта</param>
        public TcpBinaryConnector(String serverNameOrIp, Int32 port, String objectName)
            : base(serverNameOrIp, port, objectName)
        {
        }

        /// <summary>
        /// Создает экземпляр класса для подключения к локальному сервису
        /// </summary>
        /// <param name="port">Порт сервера</param>
        /// <param name="objectName">Имя объекта</param>
        public TcpBinaryConnector(Int32 port, String objectName)
            : this(CustomConnector<T>.Localhost, port, objectName)
        {
        }
    }
}
