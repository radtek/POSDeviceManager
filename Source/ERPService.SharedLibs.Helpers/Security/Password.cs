using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace ERPService.SharedLibs.Helpers.Security
{
    /// <summary>
    /// Класс для работы с паролями
    /// </summary>
    public sealed class Password : IDisposable
    {
        #region Поля

        private Byte[] _salt;
        private Byte[] _hash;
        private HashAlgorithm _algorithm;

        #endregion

        #region Конструкторы

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="hashAlgorithm">Алгоритм хэширования паролей</param>
        private Password(HashAlgorithm hashAlgorithm)
        {
            _algorithm = hashAlgorithm;
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="salt">Синхропосылка</param>
        /// <param name="hash">Хэш пароля</param>
        /// <param name="hashAlgorithm">Алгоритм хэширования паролей</param>
        public Password(String salt, String hash, HashAlgorithm hashAlgorithm)
            : this(hashAlgorithm)
        {
            _salt = Convert.FromBase64String(salt);
            _hash = Convert.FromBase64String(hash);
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="salt">Синхропосылка</param>
        /// <param name="hash">Хэш пароля</param>
        public Password(String salt, String hash)
            : this(salt, hash, new MD5CryptoServiceProvider())
        {
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="salt">Синхропосылка</param>
        /// <param name="hash">Хэш пароля</param>
        /// <param name="hashAlgorithm">Алгоритм хэширования паролей</param>
        public Password(Byte[] salt, Byte[] hash, HashAlgorithm hashAlgorithm)
            : this(hashAlgorithm)
        {
            _salt = (Byte[])salt.Clone();
            _hash = (Byte[])hash.Clone();
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="salt">Синхропосылка</param>
        /// <param name="hash">Хэш пароля</param>
        public Password(Byte[] salt, Byte[] hash)
            : this(salt, hash, new MD5CryptoServiceProvider())
        {
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="clearText">Пароль в явном виде</param>
        /// <param name="hashAlgorithm">Алгоритм хэширования паролей</param>
        public Password(Char[] clearText, HashAlgorithm hashAlgorithm)
            : this(hashAlgorithm)
        {
            _salt = GenerateRandom(6);
            _hash = HashPassword(clearText);
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="clearText">Пароль в явном виде</param>
        public Password(Char[] clearText)
            : this(clearText, new MD5CryptoServiceProvider())
        {
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="clearText">Пароль в явном виде</param>
        /// <param name="hashAlgorithm">Алгоритм хэширования паролей</param>
        public Password(String clearText, HashAlgorithm hashAlgorithm)
            : this(clearText.ToCharArray(), hashAlgorithm)
        {
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="clearText">Пароль в явном виде</param>
        public Password(String clearText)
            : this(clearText, new MD5CryptoServiceProvider())
        {
        }

        #endregion

        #region Реализация IDisposable

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public void Dispose()
        {
            if (_algorithm != null)
            {
                _algorithm.Clear();
                _algorithm = null;
            }
        }

        #endregion

        #region Закрытые методы

        private static Byte[] GenerateRandom(Int32 size)
        {
            Byte[] random = new Byte[size];
            RandomNumberGenerator.Create().GetBytes(random);
            return random;
        }

        private Byte[] HashPassword(Char[] clearText)
        {
            Byte[] hash;
            Byte[] data = new Byte[_salt.Length + Encoding.UTF8.GetMaxByteCount(clearText.Length)];

            try
            {
                // копируем синхропосылку в рабочий массив
                Array.Copy(_salt, 0, data, 0, _salt.Length);

                // копируем пароль в рабочий массив, преобразуя его в UTF-8
                Int32 byteCount = Encoding.UTF8.GetBytes(clearText, 0, clearText.Length, data, _salt.Length);

                // хэшируем данные массива
                hash = _algorithm.ComputeHash(data, 0, _salt.Length + byteCount);
            }
            finally
            {
                // очищаем рабочий массив в конце работы, чтобы избежать
                // утечки открытого пароля
                Array.Clear(data, 0, data.Length);
            }

            return hash;
        }

        #endregion

        #region Открытые свойства и методы

        /// <summary>
        /// Синхропосылка в Base64
        /// </summary>
        public String Salt
        {
            get { return Convert.ToBase64String(_salt); }
        }

        /// <summary>
        /// Хэш в Base64
        /// </summary>
        public String Hash
        {
            get { return Convert.ToBase64String(_hash); }
        }

        /// <summary>
        /// Синхропосылка
        /// </summary>
        public Byte[] RawSalt
        {
            get { return (byte[])_salt.Clone(); }
        }

        /// <summary>
        /// Хэш 
        /// </summary>
        public Byte[] RawHash
        {
            get { return (byte[])_hash.Clone(); }
        }

        /// <summary>
        /// Проверка совпадения пароля
        /// </summary>
        /// <param name="clearText">Пароль</param>
        /// <returns>true, если пароли совпадают, false - если нет</returns>
        public Boolean Verify(Char[] clearText)
        {
            Byte[] hash = HashPassword(clearText);
            if (hash.Length == _hash.Length)
            {
                for (int i = 0; i < hash.Length; i++)
                {
                    if (hash[i] != _hash[i])
                        return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Проверка совпадения пароля
        /// </summary>
        /// <param name="clearText">Пароль</param>
        /// <returns>true, если пароли совпадают, false - если нет</returns>
        public Boolean Verify(String clearText)
        {
            return Verify(clearText.ToCharArray());
        }

        #endregion
    }
}
