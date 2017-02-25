using System;
using System.Threading;
using System.Collections.Generic;

namespace ERPService.SharedLibs.Eventlog
{
    internal sealed class EventsQueue : IDisposable
    {
        #region Константы

        private const Int32 maxBufferSize = 10;

        #endregion

        #region Поля

        private readonly Boolean bufferedOutput;
        private readonly Action<IEnumerable<EventRecord>> flushAction;
        private readonly Object syncObject;
        private readonly List<EventRecord> eventsList;
        private readonly AutoResetEvent flushEvent;
        private readonly RegisteredWaitHandle registeredWaitHandle;
        private Boolean disposed;

        #endregion

        #region Конструктор

        public EventsQueue(Boolean bufferedOutput, Int32 autoFlushPeriod, 
            Action<IEnumerable<EventRecord>> flushAction)
        {
            this.bufferedOutput = bufferedOutput;
            this.flushAction = flushAction;
            this.syncObject = new Object();
            this.eventsList = new List<EventRecord>();
            this.flushEvent = new AutoResetEvent(false);
            this.registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(
                this.flushEvent, FlushEventSignaledCallback, null, autoFlushPeriod * 1000, false);
        }

        #endregion

        #region Открытые методы

        public void Enqueue(EventRecord eventRecord)
        {
            Boolean mustFlush;

            lock (syncObject)
            {
                eventsList.Add(eventRecord);

                mustFlush = !bufferedOutput || (bufferedOutput && eventsList.Count >= maxBufferSize);
            }

            if (mustFlush)
                flushEvent.Set();
        }

        #endregion

        #region Закрытые методы

        private void FlushEventSignaledCallback(Object state, Boolean timedOut)
        {
            EventRecord[] eventRecords;

            lock (syncObject)
            {
                eventRecords = eventsList.ToArray();
                eventsList.Clear();
            }

            if (eventRecords.Length > 0)
            {
                flushAction(eventRecords);
            }
        }

        #endregion

        #region Реализация IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(Boolean disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                if (registeredWaitHandle != null)
                {
                    registeredWaitHandle.Unregister(flushEvent);
                }

                if (flushEvent != null)
                {
                    flushEvent.Close();
                }
            }

            if (eventsList != null && eventsList.Count > 0 && flushAction != null)
            {
                // сбрасываем остаток буфера
                FlushEventSignaledCallback(null, false);
            }

            disposed = true;
        }

        #endregion
    }
}
