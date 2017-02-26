using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ERPService.SharedLibs.Helpers
{
    /// <summary>
    /// ��������������� ����� ��� ������ � �������� ��������
    /// </summary>
    public static class FileSystemHelper
    {
        /// <summary>
        /// ���������� ��� �����������. ������� ���������� �� ����� ��� �������������
        /// </summary>
        /// <param name="parentDirectory">������������ �������, ������ ����</param>
        /// <param name="subDirectory">����������, ��� ����</param>
        /// <returns>��� �����������, ������ ����</returns>
        public static string GetSubDirectory(string parentDirectory, string subDirectory)
        {
            if (string.IsNullOrEmpty(parentDirectory))
                throw new ArgumentNullException("parentDirectory");
            if (string.IsNullOrEmpty(subDirectory))
                throw new ArgumentNullException("subDirectory");

            string dir = string.Format("{0}\\{1}", parentDirectory, subDirectory);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return dir;
        }
    }
}
