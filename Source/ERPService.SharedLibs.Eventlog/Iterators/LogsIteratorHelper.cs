using System;
using System.IO;
using ERPService.SharedLibs.Eventlog.FileLink;

namespace ERPService.SharedLibs.Eventlog.Iterators
{
    /// <summary>
    /// Вспомогательный класс для чтения логов в процессе перебора
    /// </summary>
    internal class LogsIteratorHelper : IDisposable
    {
        private FileStream _stream;

        internal LineReader Reader { get; private set; }
        internal Index Index { get; private set; }

        internal LogsIteratorHelper(string logFile)
        {
            _stream = new FileStream(logFile, FileMode.Open, FileAccess.Read,
                FileShare.ReadWrite);
            Reader = new LineReader(_stream);
            Index = new Index(logFile);
        }

        #region Реализация IDisposable

        public void Dispose()
        {
            if (Reader != null)
                Reader = null;

            if (_stream != null)
            {
                _stream.Close();
                _stream = null;
            }

            if (Index != null)
            {
                Index.Dispose();
                Index = null;
            }
        }

        #endregion
    }
}
