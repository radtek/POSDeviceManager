using System;
using System.Collections.Generic;
using System.Text;

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
        Int32 ReadTimeout { get; set; }
        
        /// <summary>
        /// ������� ������
        /// </summary>
        Int32 WriteTimeout { get; set; }

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
        Int32 Read(Byte[] buffer, Int32 offset, Int32 size);

        /// <summary>
        /// ������ ������ � ������������
        /// </summary>
        /// <param name="buffer">����� ������</param>
        /// <param name="offset">�������� �� ������ ������</param>
        /// <param name="size">������ ������������ ������</param>
        Int32 Write(Byte[] buffer, Int32 offset, Int32 size);

        /// <summary>
        /// ������ ����� �� �������������
        /// </summary>
        Byte ReadByte();

        /// <summary>
        /// ������ ����� � ������������
        /// </summary>
        /// <param name="b">���� ��� ������</param>
        void WriteByte(Byte b);
    }
}
