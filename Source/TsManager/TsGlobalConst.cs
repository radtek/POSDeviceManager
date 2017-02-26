using System.IO;
using System.Reflection;
using ERPService.SharedLibs.Helpers;

namespace TsManager
{
    /// <summary>
    /// ���������� ���������
    /// </summary>
    public static class TsGlobalConst
    {
        /// <summary>
        /// �������� �������
        /// </summary>
        public const string EventSource = "��������";

        /// <summary>
        /// ������� �������
        /// </summary>
        public static string GetCurrentDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        /// ���� �������� ��������� ����������
        /// </summary>
        public static string GetSettingsFile()
        {
            return string.Format("{0}\\TsManagerSettings.xml", GetCurrentDirectory());
        }

        /// <summary>
        /// ������� ���������� ��������� ����������
        /// </summary>
        public static string GetLogDirectory()
        {
            return FileSystemHelper.GetSubDirectory(GetCurrentDirectory(), "TsManagerLog");
        }

        /// <summary>
        /// ������� ��� �������� ������ � ����������� ������ ������ ����
        /// </summary>
        public static string GetACMSLogicDirectory()
        {
            return FileSystemHelper.GetSubDirectory(GetCurrentDirectory(), "TsManagerACMS");
        }

        /// <summary>
        /// ���� �������� ������������� ��������� ����������
        /// </summary>
        public static string GetAppSettingsFile()
        {
            return string.Format("{0}\\TsManagerConfiguratorSettings.xml", GetCurrentDirectory());
        }
    }
}
