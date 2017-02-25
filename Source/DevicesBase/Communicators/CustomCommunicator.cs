using System;
using System.IO;

namespace DevicesBase.Communicators
{
    /// <summary>
    /// ������� ����� ��� ����������������� ����������
    /// </summary>
    public abstract class CustomCommunicator : ICommunicator
    {
        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        protected CustomCommunicator()
        {
        }

        #region ���������� ICommunicator

        /// <summary>
        /// ������� ������
        /// </summary>
        public abstract Int32 ReadTimeout
        {
            get; set;
        }

        /// <summary>
        /// ������� ������
        /// </summary>
        public abstract Int32 WriteTimeout
        {
            get; set;
        }

        /// <summary>
        /// ��������� ���������� � �����������
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// ��������� ����������� � ����������
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// ������ ������ �� �������������
        /// </summary>
        /// <param name="buffer">����� ������</param>
        /// <param name="offset">�������� �� ������ ������</param>
        /// <param name="size">������ ����������� ������</param>
        public abstract Int32 Read(Byte[] buffer, Int32 offset, Int32 size);

        /// <summary>
        /// ������ ������ � ������������
        /// </summary>
        /// <param name="buffer">����� ������</param>
        /// <param name="offset">�������� �� ������ ������</param>
        /// <param name="size">������ ������������ ������</param>
        public abstract Int32 Write(Byte[] buffer, Int32 offset, Int32 size);

        /// <summary>
        /// ������ ����� �� �������������
        /// </summary>
        public Byte ReadByte()
        {
            var buf = new Byte[1];
            if (Read(buf, 0, buf.Length) != buf.Length)
                throw new IOException("�������� ������ ��������� � �������.");

            return buf[0];
        }

        /// <summary>
        /// ������ ����� � ������������
        /// </summary>
        /// <param name="b">���� ��� ������</param>
        public void WriteByte(Byte b)
        {
            var buf = new[] { b };
            if (Write(buf, 0, buf.Length) != buf.Length)
                throw new IOException("�������� ������ ��������� � �������.");
        }

        #endregion

        #region ���������� IDisposable

        /// <summary>
        /// ������������ ��������
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        #endregion
    }
}
