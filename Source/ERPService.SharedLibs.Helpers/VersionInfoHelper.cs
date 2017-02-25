using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.IO;

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
        public static String GetVersion(String fileName)
        {
            try
            {
                return GetVersion(Assembly.LoadFrom(fileName));
            }
            catch (BadImageFormatException)
            {
                return String.Empty;
            }
            catch (FileLoadException)
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Возвращает версию файла
        /// </summary>
        /// <param name="asm">Сборка</param>
        public static String GetVersion(Assembly asm)
        {
            return FileVersionInfo.GetVersionInfo(asm.Location).FileVersion;
        }
    }
}
