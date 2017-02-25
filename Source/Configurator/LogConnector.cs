using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ERPService.SharedLibs.Eventlog;
using DevicesBase;

namespace DevmanConfig
{
    internal class LogConnector : IEventSourceConnector, IDisposable
    {
        private EventLink _log;
        private ToolStripStatusLabel _tsslReloadProgress;

        internal LogConnector(ToolStripStatusLabel tsslReloadProgress)
        {
            _tsslReloadProgress = tsslReloadProgress;
        }

        #region IEventSourceConnector Members

        public void CloseConnector()
        {
            Dispose();
        }

        public void OpenConnector()
        {
            Dispose();
            _log = new EventLink(DeviceManager.GetDeviceManagerLogDirectory(), true);
        }

        public void ReloadProgress(int eventsLoaded)
        {
            _tsslReloadProgress.Text = String.Format("Событий загружено: {0}", eventsLoaded);
        }

        public IEventLinkBasics Source
        {
            get { return _log; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_log != null)
            {
                _log.Dispose();
                _log = null;
            }
        }

        #endregion
    }
}
