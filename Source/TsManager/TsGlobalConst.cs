using System;
using System.Collections.Generic;
using System.Text;
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
        public const String EventSource = "��������";

        /// <summary>
        /// ������� �������
        /// </summary>
        public static String GetCurrentDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        /// ���� �������� ��������� ����������
        /// </summary>
        public static String GetSettingsFile()
        {
            return String.Format("{0}\\TsManagerSettings.xml", GetCurrentDirectory());
        }

        /// <summary>
        /// ������� ���������� ��������� ����������
        /// </summary>
        public static String GetLogDirectory()
        {
            return FileSystemHelper.GetSubDirectory(GetCurrentDirectory(), "TsManagerLog");
        }

        /// <summary>
        /// ������� ��� �������� ������ � ����������� ������ ������ ����
        /// </summary>
        public static String GetACMSLogicDirectory()
        {
            return FileSystemHelper.GetSubDirectory(GetCurrentDirectory(), "TsManagerACMS");
        }

        /// <summary>
        /// ���� �������� ������������� ��������� ����������
        /// </summary>
        public static String GetAppSettingsFile()
        {
            return String.Format("{0}\\TsManagerConfiguratorSettings.xml", GetCurrentDirectory());
        }
    }
}
