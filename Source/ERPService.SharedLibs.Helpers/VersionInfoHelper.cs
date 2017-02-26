using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ERPService.SharedLibs.Helpers
{
    /// <summary>
    /// ��������������� ����� ��� ������ � �������� ������
    /// </summary>
    public static class VersionInfoHelper
    {
        /// <summary>
        /// ���������� ������ �����
        /// </summary>
        /// <param name="fileName">��� �����</param>
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
        /// ���������� ������ �����
        /// </summary>
        /// <param name="asm">������</param>
        public static string GetVersion(Assembly asm)
        {
            return FileVersionInfo.GetVersionInfo(asm.Location).FileVersion;
        }
    }
}
