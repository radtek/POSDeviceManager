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
