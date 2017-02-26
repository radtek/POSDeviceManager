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
        public const string EventSource = "Турникет";

        /// <summary>
        /// Текущий каталог
        /// </summary>
        public static string GetCurrentDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        /// Файл настроек менеджера турникетов
        /// </summary>
        public static string GetSettingsFile()
        {
            return string.Format("{0}\\TsManagerSettings.xml", GetCurrentDirectory());
        }

        /// <summary>
        /// Каталог статистики менеджера турникетов
        /// </summary>
        public static string GetLogDirectory()
        {
            return FileSystemHelper.GetSubDirectory(GetCurrentDirectory(), "TsManagerLog");
        }

        /// <summary>
        /// Каталог для хранения сборок с реализацией логики работы СКУД
        /// </summary>
        public static string GetACMSLogicDirectory()
        {
            return FileSystemHelper.GetSubDirectory(GetCurrentDirectory(), "TsManagerACMS");
        }

        /// <summary>
        /// Файл настроек конфигуратора менеджера турникетов
        /// </summary>
        public static string GetAppSettingsFile()
        {
            return string.Format("{0}\\TsManagerConfiguratorSettings.xml", GetCurrentDirectory());
        }
    }
}
