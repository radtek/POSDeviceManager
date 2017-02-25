using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Services;
using ERPService.SharedLibs.Eventlog;

namespace DevicesBase.Helpers
{
    /// <summary>
    /// ������
    /// </summary>
    internal class DeviceManagerTrackingHandler : ITrackingHandler
    {
        private const String _objDisconnected = "������ ���������� �� ������.\n��� �������: {0}";
        private const String _objMarshalled = "������ �����������.\n��� �������: {0}\nURI: {1}";
        private const String _objUnMarshalled = "���������� ������� ��������.\n��� �������: {0}\nURI: {1}";

        IEventLink _eventLink;
        Boolean _debugInfo;

        internal DeviceManagerTrackingHandler(IEventLink eventLink, Boolean debugInfo)
        {
            _eventLink = eventLink;
            _debugInfo = debugInfo;
            TrackingServices.RegisterTrackingHandler(this);
        }

        #region ���������� ITrackingHandler

        /// <summary>
        /// ������������ ������� �� ��� ������
        /// </summary>
        /// <param name="obj">������</param>
        public void DisconnectedObject(object obj)
        {
            if (_debugInfo)
            {
                _eventLink.Post(DeviceManager.EventSource, 
                    String.Format(_objDisconnected, obj.GetType()));
            }
        }

        /// <summary>
        /// ���������� �������
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
        /// ������ ���������� �������
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
