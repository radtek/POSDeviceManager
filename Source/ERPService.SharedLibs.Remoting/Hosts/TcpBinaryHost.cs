using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Serialization.Formatters;

namespace ERPService.SharedLibs.Remoting.Hosts
{
    /// <summary>
    /// Класс для хостинга объектов с помощью TCP-каналов и 
    /// бинарным форматированием сообщений
    /// </summary>
    /// <typeparam name="T">Тип объекта, к которому нужно обеспечить доступ</typeparam>
    public class TcpBinaryHost<T> : CustomTcpHost<T> where T : HostingTarget
    {
        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="target">Объект, к которому нужно обеспечить доступ</param>
        public TcpBinaryHost(T target)
            : base(target)
        {
        }

        /// <summary>
        /// Создает провайдер для приемников, отвечающих за форматирование сообщений
        /// </summary>
        /// <returns>Провайдер для приемников, отвечающих за форматирование сообщений</returns>
        protected override IServerFormatterSinkProvider CreateFormatterSinkProvider()
        {
            BinaryServerFormatterSinkProvider sinkProvider =
                new BinaryServerFormatterSinkProvider();
            sinkProvider.TypeFilterLevel = TypeFilterLevel.Full;

            return sinkProvider;
        }
    }
}
