using System;

namespace DevicesBase.Communicators
{
    /// <summary>
    /// ��������� ���������������� ���������� (���� RS-232, ����� � �.�.)
    /// </summary>
    public interface ICommunicator : IDisposable
    {
        /// <summary>
        /// ������� ������
        /// </summary>
        int ReadTimeout { get; set; }

        /// <summary>
        /// ������� ������
        /// </summary>
        int WriteTimeout { get; set; }

        /// <summary>
        /// ��������� ���������� � �����������
        /// </summary>
        void Open();

        /// <summary>
        /// ��������� ����������� � ����������
        /// </summary>
        void Close();

        /// <summary>
        /// ������ ������ �� �������������
        /// </summary>
        /// <param name="buffer">����� ������</param>
        /// <param name="offset">�������� �� ������ ������</param>
        /// <param name="size">������ ����������� ������</param>
        int Read(byte[] buffer, int offset, int size);

        /// <summary>
        /// ������ ������ � ������������
        /// </summary>
        /// <param name="buffer">����� ������</param>
        /// <param name="offset">�������� �� ������ ������</param>
        /// <param name="size">������ ������������ ������</param>
        int Write(byte[] buffer, int offset, int size);

        /// <summary>
        /// ������ ����� �� �������������
        /// </summary>
        byte ReadByte();

        /// <summary>
        /// ������ ����� � ������������
        /// </summary>
        /// <param name="b">���� ��� ������</param>
        void WriteByte(byte b);
    }
}
