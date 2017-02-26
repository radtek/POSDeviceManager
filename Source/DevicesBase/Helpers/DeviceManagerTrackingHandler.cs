using System.Runtime.Remoting.Services;
using ERPService.SharedLibs.Eventlog;

namespace DevicesBase.Helpers
{
    /// <summary>
    /// ������
    /// </summary>
    internal class DeviceManagerTrackingHandler : ITrackingHandler
    {
        private const string _objDisconnected = "������ ���������� �� ������.\n��� �������: {0}";
        private const string _objMarshalled = "������ �����������.\n��� �������: {0}\nURI: {1}";
        private const string _objUnMarshalled = "���������� ������� ��������.\n��� �������: {0}\nURI: {1}";

        IEventLink _eventLink;
        bool _debugInfo;

        internal DeviceManagerTrackingHandler(IEventLink eventLink, bool debugInfo)
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
                    string.Format(_objDisconnected, obj.GetType()));
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
                    string.Format(_objMarshalled, obj.GetType(), or.URI));
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
                    string.Format(_objUnMarshalled, obj.GetType(), or.URI));
            }
        }

        #endregion
    }
}
