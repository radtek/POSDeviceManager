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

        private byte[] _salt;
        private byte[] _hash;
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
        public Password(string salt, string hash, HashAlgorithm hashAlgorithm)
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
        public Password(string salt, string hash)
            : this(salt, hash, new MD5CryptoServiceProvider())
        {
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="salt">�������������</param>
        /// <param name="hash">��� ������</param>
        /// <param name="hashAlgorithm">�������� ����������� �������</param>
        public Password(byte[] salt, byte[] hash, HashAlgorithm hashAlgorithm)
            : this(hashAlgorithm)
        {
            _salt = (byte[])salt.Clone();
            _hash = (byte[])hash.Clone();
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="salt">�������������</param>
        /// <param name="hash">��� ������</param>
        public Password(byte[] salt, byte[] hash)
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
        public Password(string clearText, HashAlgorithm hashAlgorithm)
            : this(clearText.ToCharArray(), hashAlgorithm)
        {
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="clearText">������ � ����� ����</param>
        public Password(string clearText)
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

        private static byte[] GenerateRandom(int size)
        {
            byte[] random = new byte[size];
            RandomNumberGenerator.Create().GetBytes(random);
            return random;
        }

        private byte[] HashPassword(Char[] clearText)
        {
            byte[] hash;
            byte[] data = new byte[_salt.Length + Encoding.UTF8.GetMaxByteCount(clearText.Length)];

            try
            {
                // �������� ������������� � ������� ������
                Array.Copy(_salt, 0, data, 0, _salt.Length);

                // �������� ������ � ������� ������, ���������� ��� � UTF-8
                int byteCount = Encoding.UTF8.GetBytes(clearText, 0, clearText.Length, data, _salt.Length);

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
        public string Salt
        {
            get { return Convert.ToBase64String(_salt); }
        }

        /// <summary>
        /// ��� � Base64
        /// </summary>
        public string Hash
        {
            get { return Convert.ToBase64String(_hash); }
        }

        /// <summary>
        /// �������������
        /// </summary>
        public byte[] RawSalt
        {
            get { return (byte[])_salt.Clone(); }
        }

        /// <summary>
        /// ��� 
        /// </summary>
        public byte[] RawHash
        {
            get { return (byte[])_hash.Clone(); }
        }

        /// <summary>
        /// �������� ���������� ������
        /// </summary>
        /// <param name="clearText">������</param>
        /// <returns>true, ���� ������ ���������, false - ���� ���</returns>
        public bool Verify(Char[] clearText)
        {
            byte[] hash = HashPassword(clearText);
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
        public bool Verify(string clearText)
        {
            return Verify(clearText.ToCharArray());
        }

        #endregion
    }
}
