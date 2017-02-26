using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ERPService.SharedLibs.Helpers
{
    /// <summary>
    /// Вспомогательный класс для работы с версиями файлов
    /// </summary>
    public static class VersionInfoHelper
    {
        /// <summary>
        /// Возвращает версию файла
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        public static string GetVersion(string fileName)
        {
            try
            {
                return GetVersion(Assembly.LoadFrom(fileName));
            }
            catch (BadImageFormatException)
            {
                return string.Empty;
            }
            catch (FileLoadException)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Возвращает версию файла
        /// </summary>
        /// <param name="asm">Сборка</param>
        public static string GetVersion(Assembly asm)
        {
            return FileVersionInfo.GetVersionInfo(asm.Location).FileVersion;
        }
    }
}
