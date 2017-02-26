using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.InteropServices;

namespace ERPService.SharedLibs.Helpers
{
    /// <summary>
    /// ��������������� ����� ��� ������������/�������������� ��������
    /// </summary>
    public static class GenericSerializer
    {
        private static XmlSerializer CreateSerializer(Type objType, Type[] extraTypes)
        {
            if (extraTypes == null)
                return new XmlSerializer(objType);
            else
                return new XmlSerializer(objType, extraTypes);
        }

        #region �������������� ������� �� �����

        /// <summary>
        /// �������������� ������� �� �����
        /// </summary>
        /// <typeparam name="T">��������������� ���</typeparam>
        /// <param name="fileName">��� �����, ����������� ��������������� ������</param>
        /// <param name="throwIfNotExists">������� ����������, ���� ����� �� ����������</param>
        /// <param name="extraTypes">�������������� ���� ������</param>
        /// <returns>����������������� ������ ���� ����� ������</returns>
        public static T Deserialize<T>(string fileName, bool throwIfNotExists, Type[] extraTypes)
            where T : new()
        {
            T result = DeserializeOrDefault<T>(fileName, throwIfNotExists, extraTypes);
            if (Object.Equals(result, default(T)))
                result = new T();
            return result;
        }

        /// <summary>
        /// �������������� ������� �� �����
        /// </summary>
        /// <typeparam name="T">��������������� ���</typeparam>
        /// <param name="fileName">��� �����, ����������� ��������������� ������</param>
        /// <param name="throwIfNotExists">������� ����������, ���� ����� �� ����������</param>
        /// <returns>����������������� ������ ���� ����� ������</returns>
        public static T Deserialize<T>(string fileName, bool throwIfNotExists)
            where T : new()
        {
            return Deserialize<T>(fileName, throwIfNotExists, null);
        }

        /// <summary>
        /// �������������� ������� �� �����
        /// </summary>
        /// <typeparam name="T">��������������� ���</typeparam>
        /// <param name="fileName">��� �����, ����������� ��������������� ������</param>
        /// <returns>����������������� ������ ���� ����� ������</returns>
        public static T Deserialize<T>(string fileName) where T : new()
        {
            return Deserialize<T>(fileName, false, null);
        }

        #endregion

        #region �������������� ������� �� �����, ���� ������� �������� �� ���������

        /// <summary>
        /// �������������� ������� �� �����, ���� ������� �������� �� ���������
        /// </summary>
        /// <typeparam name="T">��������������� ���</typeparam>
        /// <param name="fileName">��� �����, ����������� ��������������� ������</param>
        /// <param name="throwIfNotExists">������� ����������, ���� ����� �� ����������</param>
        /// <param name="extraTypes">�������������� ���� ������</param>
        /// <returns>����������������� ������ ���� �������� �� ���������</returns>
        public static T DeserializeOrDefault<T>(string fileName, bool throwIfNotExists, Type[] extraTypes)
        {
            if (File.Exists(fileName))
            {
                using (FileStream fs = new FileStream(fileName,
                    FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    XmlSerializer serializer = CreateSerializer(typeof(T), extraTypes);
                    return (T)serializer.Deserialize(fs);
                }
            }
            else
            {
                if (throwIfNotExists)
                    throw new FileNotFoundException(
                        "����, ���������� ��������������� ������, �� ������",
                        fileName);

                return default(T);
            }
        }

        /// <summary>
        /// �������������� ������� �� �����, ���� ������� �������� �� ���������
        /// </summary>
        /// <typeparam name="T">��������������� ���</typeparam>
        /// <param name="fileName">��� �����, ����������� ��������������� ������</param>
        /// <param name="throwIfNotExists">������� ����������, ���� ����� �� ����������</param>
        /// <returns>����������������� ������ ���� �������� �� ���������</returns>
        public static T DeserializeOrDefault<T>(string fileName, bool throwIfNotExists)
        {
            return DeserializeOrDefault<T>(fileName, throwIfNotExists, null);
        }

        /// <summary>
        /// �������������� ������� �� �����, ���� ������� �������� �� ���������
        /// </summary>
        /// <typeparam name="T">��������������� ���</typeparam>
        /// <param name="fileName">��� �����, ����������� ��������������� ������</param>
        /// <returns>����������������� ������ ���� �������� �� ���������</returns>
        public static T DeserializeOrDefault<T>(string fileName)
        {
            return DeserializeOrDefault<T>(fileName, false, null);
        }

        #endregion

        #region ������������ ������� � ����

        /// <summary>
        /// ������������ ������� � ����
        /// </summary>
        /// <typeparam name="T">������������� ���</typeparam>
        /// <param name="obj">������������� ������</param>
        /// <param name="fileName">��� �����</param>
        /// <param name="extraTypes">�������������� ���� ������</param>
        public static void Serialize<T>(T obj, string fileName, Type[] extraTypes)
        {
            string directory = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using (FileStream fs = new FileStream(fileName, FileMode.Create,
                FileAccess.Write, FileShare.None))
            {
                XmlSerializer serializer = CreateSerializer(typeof(T), extraTypes);
                serializer.Serialize(fs, obj);
            }
        }

        /// <summary>
        /// ������������ ������� � ����
        /// </summary>
        /// <typeparam name="T">������������� ���</typeparam>
        /// <param name="obj">������������� ������</param>
        /// <param name="fileName">��� �����</param>
        public static void Serialize<T>(T obj, string fileName)
        {
            Serialize<T>(obj, fileName, null);
        }

        #endregion

        #region ������������/�������������� �������� ��� �������������� � ������������ �����

        /// <summary>
        /// ������������ ��������� � ������ ����
        /// </summary>
        /// <typeparam name="T">��� ���������</typeparam>
        /// <param name="anyStruct">���������</param>
        /// <returns>��������������� ��������� � ���� ������� ����</returns>
        public static byte[] RawSerialize<T>(T anyStruct) where T : struct
        {
            byte[] rawdata = new byte[Marshal.SizeOf(anyStruct)];
            GCHandle handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
            try
            {
                Marshal.StructureToPtr(anyStruct, handle.AddrOfPinnedObject(), false);
                return rawdata;
            }
            finally
            {
                handle.Free();
            }
        }

        /// <summary>
        /// �������������� ��������� �� ������� ����
        /// </summary>
        /// <typeparam name="T">��� ���������</typeparam>
        /// <param name="rawSerializedStruct">������ ����, ���������� ������ ��������������� ���������</param>
        /// <returns>���������, ����������������� �� ������� ����</returns>
        public static T RawDeserialize<T>(byte[] rawSerializedStruct) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(rawSerializedStruct, GCHandleType.Pinned);
            try
            {
                T result = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
                return result;
            }
            finally
            {
                handle.Free();
            }
        }

        #endregion
    }
}
