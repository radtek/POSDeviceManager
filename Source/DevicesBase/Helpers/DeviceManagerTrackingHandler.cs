using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Services;
using ERPService.SharedLibs.Eventlog;

namespace DevicesBase.Helpers
{
    /// <summary>
    /// Трэкер
    /// </summary>
    internal class DeviceManagerTrackingHandler : ITrackingHandler
    {
        private const String _objDisconnected = "Объект отсоединен от прокси.\nТип объекта: {0}";
        private const String _objMarshalled = "Объект опубликован.\nТип объекта: {0}\nURI: {1}";
        private const String _objUnMarshalled = "Публикация объекта отменена.\nТип объекта: {0}\nURI: {1}";

        IEventLink _eventLink;
        Boolean _debugInfo;

        internal DeviceManagerTrackingHandler(IEventLink eventLink, Boolean debugInfo)
        {
            _eventLink = eventLink;
            _debugInfo = debugInfo;
            TrackingServices.RegisterTrackingHandler(this);
        }

        #region Реализация ITrackingHandler

        /// <summary>
        /// Отсоединение объекта от его прокси
        /// </summary>
        /// <param name="obj">Объект</param>
        public void DisconnectedObject(object obj)
        {
            if (_debugInfo)
            {
                _eventLink.Post(DeviceManager.EventSource, 
                    String.Format(_objDisconnected, obj.GetType()));
            }
        }

        /// <summary>
        /// Публикация объекта
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="or"></param>
        public void MarshaledObject(object obj, System.Runtime.Remoting.ObjRef or)
        {
            if (_debugInfo)
            {
                _eventLink.Post(DeviceManager.EventSource, 
                    String.Format(_objMarshalled, obj.GetType(), or.URI));
            }
        }

        /// <summary>
        /// Отмена публикации объекта
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="or"></param>
        public void UnmarshaledObject(object obj, System.Runtime.Remoting.ObjRef or)
        {
            if (_debugInfo)
            {
                _eventLink.Post(DeviceManager.EventSource, 
                    String.Format(_objUnMarshalled, obj.GetType(), or.URI));
            }
        }

        #endregion
    }
}
