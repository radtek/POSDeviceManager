using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;

namespace ERPService.SharedLibs.Remoting
{
    /// <summary>
    /// Базовый класс для хостинга и подключения
    /// </summary>
    public abstract class RemotingBase : IDisposable
    {
        /// <summary>
        /// Отмена регистрации канала
        /// </summary>
        /// <param name="channel">Канал</param>
        protected void SafeUnregisterChannel(IChannel channel)
        {
            if (channel == null)
                return;

            try
            {
                ChannelServices.UnregisterChannel(channel);
                channel = null;
            }
            catch (RemotingException)
            {
            }
        }

        /// <summary>
        /// Формирует имя канала
        /// </summary>
        /// <param name="prefix">Префикс</param>
        /// <returns>Имя серверного канала</returns>
        protected string GetChannelName(string prefix)
        {
            return string.Format("{1}Channel{0}", prefix, Guid.NewGuid());
        }

        #region Реализация IDisposable

        /// <summary>
        /// Освобождение ресурсов и т.п.
        /// </summary>
        public virtual void Dispose()
        {
        }

        #endregion
    }
}
