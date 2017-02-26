using System;
using System.Windows.Forms;
using ERPService.SharedLibs.Eventlog;
using TsManager;

namespace TsManagerConfigurator
{
    internal class LogConnector : IEventSourceConnector, IDisposable
    {
        private EventLink _log;
        private ToolStripStatusLabel _tsslUpdateState;

        internal LogConnector(ToolStripStatusLabel tsslUpdateState)
        {
            _tsslUpdateState = tsslUpdateState;
        }

        #region IEventSourceConnector Members

        public void CloseConnector()
        {
            Dispose();
        }

        public void OpenConnector()
        {
            Dispose();
            _log = new EventLink(TsGlobalConst.GetLogDirectory(), true);
        }

        public void ReloadProgress(int eventsLoaded)
        {
            _tsslUpdateState.Text = string.Format("Событий загружено: {0}", eventsLoaded);
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
