using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.IO;

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
        /// ���������� ������ �����
        /// </summary>
        /// <param name="asm">������</param>
        public static String GetVersion(Assembly asm)
        {
            return FileVersionInfo.GetVersionInfo(asm.Location).FileVersion;
        }
    }
}
