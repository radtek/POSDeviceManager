using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.InteropServices;

namespace ERPService.SharedLibs.Helpers
{
    /// <summary>
    /// Вспомогательный класс для сериализации/десериализации объектов
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

        #region Десериализация объекта из файла

        /// <summary>
        /// Десериализация объекта из файла
        /// </summary>
        /// <typeparam name="T">Десериализуемый тип</typeparam>
        /// <param name="fileName">Имя файла, содержащего сериализованный объект</param>
        /// <param name="throwIfNotExists">Бросать исключение, если файла не существует</param>
        /// <param name="extraTypes">Дополнительные типы данных</param>
        /// <returns>Десериализованный объект либо новый объект</returns>
        public static T Deserialize<T>(string fileName, bool throwIfNotExists, Type[] extraTypes)
            where T : new()
        {
            T result = DeserializeOrDefault<T>(fileName, throwIfNotExists, extraTypes);
            if (Object.Equals(result, default(T)))
                result = new T();
            return result;
        }

        /// <summary>
        /// Десериализация объекта из файла
        /// </summary>
        /// <typeparam name="T">Десериализуемый тип</typeparam>
        /// <param name="fileName">Имя файла, содержащего сериализованный объект</param>
        /// <param name="throwIfNotExists">Бросать исключение, если файла не существует</param>
        /// <returns>Десериализованный объект либо новый объект</returns>
        public static T Deserialize<T>(string fileName, bool throwIfNotExists)
            where T : new()
        {
            return Deserialize<T>(fileName, throwIfNotExists, null);
        }

        /// <summary>
        /// Десериализация объекта из файла
        /// </summary>
        /// <typeparam name="T">Десериализуемый тип</typeparam>
        /// <param name="fileName">Имя файла, содержащего сериализованный объект</param>
        /// <returns>Десериализованный объект либо новый объект</returns>
        public static T Deserialize<T>(string fileName) where T : new()
        {
            return Deserialize<T>(fileName, false, null);
        }

        #endregion

        #region Десериализация объекта из файла, либо возврат значения по умолчанию

        /// <summary>
        /// Десериализация объекта из файла, либо возврат значения по умолчанию
        /// </summary>
        /// <typeparam name="T">Десериализуемый тип</typeparam>
        /// <param name="fileName">Имя файла, содержащего сериализованный объект</param>
        /// <param name="throwIfNotExists">Бросать исключение, если файла не существует</param>
        /// <param name="extraTypes">Дополнительные типы данных</param>
        /// <returns>Десериализованный объект либо значение по умолчанию</returns>
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
                        "Файл, содержащий сериализованный объект, не найден",
                        fileName);

                return default(T);
            }
        }

        /// <summary>
        /// Десериализация объекта из файла, либо возврат значения по умолчанию
        /// </summary>
        /// <typeparam name="T">Десериализуемый тип</typeparam>
        /// <param name="fileName">Имя файла, содержащего сериализованный объект</param>
        /// <param name="throwIfNotExists">Бросать исключение, если файла не существует</param>
        /// <returns>Десериализованный объект либо значение по умолчанию</returns>
        public static T DeserializeOrDefault<T>(string fileName, bool throwIfNotExists)
        {
            return DeserializeOrDefault<T>(fileName, throwIfNotExists, null);
        }

        /// <summary>
        /// Десериализация объекта из файла, либо возврат значения по умолчанию
        /// </summary>
        /// <typeparam name="T">Десериализуемый тип</typeparam>
        /// <param name="fileName">Имя файла, содержащего сериализованный объект</param>
        /// <returns>Десериализованный объект либо значение по умолчанию</returns>
        public static T DeserializeOrDefault<T>(string fileName)
        {
            return DeserializeOrDefault<T>(fileName, false, null);
        }

        #endregion

        #region Сериализация объекта в файл

        /// <summary>
        /// Сериализация объекта в файл
        /// </summary>
        /// <typeparam name="T">Сериализуемый тип</typeparam>
        /// <param name="obj">Сериализуемый объект</param>
        /// <param name="fileName">Имя файла</param>
        /// <param name="extraTypes">Дополнительные типы данных</param>
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
        /// Сериализация объекта в файл
        /// </summary>
        /// <typeparam name="T">Сериализуемый тип</typeparam>
        /// <param name="obj">Сериализуемый объект</param>
        /// <param name="fileName">Имя файла</param>
        public static void Serialize<T>(T obj, string fileName)
        {
            Serialize<T>(obj, fileName, null);
        }

        #endregion

        #region Сериализация/десериализация структур для взаимодействия с неуправяемым кодом

        /// <summary>
        /// Сериализация структуры в массив байт
        /// </summary>
        /// <typeparam name="T">Тип структуры</typeparam>
        /// <param name="anyStruct">Структура</param>
        /// <returns>Сериализованная структура в виде массива байт</returns>
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
        /// Десериализация структуры из массива байт
        /// </summary>
        /// <typeparam name="T">Тип структуры</typeparam>
        /// <param name="rawSerializedStruct">Массив байт, содержащий данные сериализованной структуры</param>
        /// <returns>Структура, десериализованная из массива байт</returns>
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
