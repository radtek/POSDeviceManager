using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace ERPService.SharedLibs.Helpers.Security
{
    /// <summary>
    /// ����� ��� ������ � ��������
    /// </summary>
    public sealed class Password : IDisposable
    {
        #region ����

        private Byte[] _salt;
        private Byte[] _hash;
        private HashAlgorithm _algorithm;

        #endregion

        #region ������������

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="hashAlgorithm">�������� ����������� �������</param>
        private Password(HashAlgorithm hashAlgorithm)
        {
            _algorithm = hashAlgorithm;
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="salt">�������������</param>
        /// <param name="hash">��� ������</param>
        /// <param name="hashAlgorithm">�������� ����������� �������</param>
        public Password(String salt, String hash, HashAlgorithm hashAlgorithm)
            : this(hashAlgorithm)
        {
            _salt = Convert.FromBase64String(salt);
            _hash = Convert.FromBase64String(hash);
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="salt">�������������</param>
        /// <param name="hash">��� ������</param>
        public Password(String salt, String hash)
            : this(salt, hash, new MD5CryptoServiceProvider())
        {
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="salt">�������������</param>
        /// <param name="hash">��� ������</param>
        /// <param name="hashAlgorithm">�������� ����������� �������</param>
        public Password(Byte[] salt, Byte[] hash, HashAlgorithm hashAlgorithm)
            : this(hashAlgorithm)
        {
            _salt = (Byte[])salt.Clone();
            _hash = (Byte[])hash.Clone();
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="salt">�������������</param>
        /// <param name="hash">��� ������</param>
        public Password(Byte[] salt, Byte[] hash)
            : this(salt, hash, new MD5CryptoServiceProvider())
        {
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="clearText">������ � ����� ����</param>
        /// <param name="hashAlgorithm">�������� ����������� �������</param>
        public Password(Char[] clearText, HashAlgorithm hashAlgorithm)
            : this(hashAlgorithm)
        {
            _salt = GenerateRandom(6);
            _hash = HashPassword(clearText);
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="clearText">������ � ����� ����</param>
        public Password(Char[] clearText)
            : this(clearText, new MD5CryptoServiceProvider())
        {
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="clearText">������ � ����� ����</param>
        /// <param name="hashAlgorithm">�������� ����������� �������</param>
        public Password(String clearText, HashAlgorithm hashAlgorithm)
            : this(clearText.ToCharArray(), hashAlgorithm)
        {
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="clearText">������ � ����� ����</param>
        public Password(String clearText)
            : this(clearText, new MD5CryptoServiceProvider())
        {
        }

        #endregion

        #region ���������� IDisposable

        /// <summary>
        /// ������������ ��������
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

        #region �������� ������

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
                // �������� ������������� � ������� ������
                Array.Copy(_salt, 0, data, 0, _salt.Length);

                // �������� ������ � ������� ������, ���������� ��� � UTF-8
                Int32 byteCount = Encoding.UTF8.GetBytes(clearText, 0, clearText.Length, data, _salt.Length);

                // �������� ������ �������
                hash = _algorithm.ComputeHash(data, 0, _salt.Length + byteCount);
            }
            finally
            {
                // ������� ������� ������ � ����� ������, ����� ��������
                // ������ ��������� ������
                Array.Clear(data, 0, data.Length);
            }

            return hash;
        }

        #endregion

        #region �������� �������� � ������

        /// <summary>
        /// ������������� � Base64
        /// </summary>
        public String Salt
        {
            get { return Convert.ToBase64String(_salt); }
        }

        /// <summary>
        /// ��� � Base64
        /// </summary>
        public String Hash
        {
            get { return Convert.ToBase64String(_hash); }
        }

        /// <summary>
        /// �������������
        /// </summary>
        public Byte[] RawSalt
        {
            get { return (byte[])_salt.Clone(); }
        }

        /// <summary>
        /// ��� 
        /// </summary>
        public Byte[] RawHash
        {
            get { return (byte[])_hash.Clone(); }
        }

        /// <summary>
        /// �������� ���������� ������
        /// </summary>
        /// <param name="clearText">������</param>
        /// <returns>true, ���� ������ ���������, false - ���� ���</returns>
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
        /// �������� ���������� ������
        /// </summary>
        /// <param name="clearText">������</param>
        /// <returns>true, ���� ������ ���������, false - ���� ���</returns>
        public Boolean Verify(String clearText)
        {
            return Verify(clearText.ToCharArray());
        }

        #endregion
    }
}
