using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ERPService.SharedLibs.Helpers;

namespace TsManager
{
    /// <summary>
    /// Глобальные константы
    /// </summary>
    public static class TsGlobalConst
    {
        /// <summary>
        /// Источник событий
        /// </summary>
        public const String EventSource = "Турникет";

        /// <summary>
        /// Текущий каталог
        /// </summary>
        public static String GetCurrentDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        /// Файл настроек менеджера турникетов
        /// </summary>
        public static String GetSettingsFile()
        {
            return String.Format("{0}\\TsManagerSettings.xml", GetCurrentDirectory());
        }

        /// <summary>
        /// Каталог статистики менеджера турникетов
        /// </summary>
        public static String GetLogDirectory()
        {
            return FileSystemHelper.GetSubDirectory(GetCurrentDirectory(), "TsManagerLog");
        }

        /// <summary>
        /// Каталог для хранения сборок с реализацией логики работы СКУД
        /// </summary>
        public static String GetACMSLogicDirectory()
        {
            return FileSystemHelper.GetSubDirectory(GetCurrentDirectory(), "TsManagerACMS");
        }

        /// <summary>
        /// Файл настроек конфигуратора менеджера турникетов
        /// </summary>
        public static String GetAppSettingsFile()
        {
            return String.Format("{0}\\TsManagerConfiguratorSettings.xml", GetCurrentDirectory());
        }
    }
}
