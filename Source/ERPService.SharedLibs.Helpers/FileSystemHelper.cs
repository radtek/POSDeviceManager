using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ERPService.SharedLibs.Helpers
{
    /// <summary>
    /// Вспомогательный класс для работы с файловой системой
    /// </summary>
    public static class FileSystemHelper
    {
        /// <summary>
        /// Возвращает имя подкаталога. Создает подкаталог на диске при необходимости
        /// </summary>
        /// <param name="parentDirectory">Родительский каталог, полный путь</param>
        /// <param name="subDirectory">Подкаталог, без пути</param>
        /// <returns>Имя подкаталога, полный путь</returns>
        public static String GetSubDirectory(String parentDirectory, String subDirectory)
        {
            if (String.IsNullOrEmpty(parentDirectory))
                throw new ArgumentNullException("parentDirectory");
            if (String.IsNullOrEmpty(subDirectory))
                throw new ArgumentNullException("subDirectory");

            String dir = String.Format("{0}\\{1}", parentDirectory, subDirectory);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return dir;
        }
    }
}
