using System.IO;
using System.Threading;

namespace ERPService.SharedLibs.Eventlog
{
    internal static class MutexHelper
	{
		private const string _syncFilesMutexNameFmt = "Global\\{0}_sync_files";
		private const string _syncIndexMutexNameFmt = "Global\\{0}_sync_index";
		private static Mutex CreateNamedMutex(string mutexNameFormat, string mutexId)
		{
			mutexId = mutexId.Replace(Path.DirectorySeparatorChar, '_');
			return new Mutex(false, string.Format(Thread.CurrentThread.CurrentCulture, mutexNameFormat, new object[]
			{
				mutexId
			}));
		}
		internal static Mutex CreateSyncFilesMutex(string logFilePrefix)
		{
			return MutexHelper.CreateNamedMutex("Global\\{0}_sync_files", logFilePrefix);
		}
		internal static Mutex CreateSyncIndexMutex(string logFileName)
		{
			return MutexHelper.CreateNamedMutex("Global\\{0}_sync_index", Path.GetFileNameWithoutExtension(logFileName));
		}
		internal static void WaitMutex(this Mutex mutex)
		{
			try
			{
				mutex.WaitOne();
			}
			catch (AbandonedMutexException)
			{
			}
		}
	}
}
