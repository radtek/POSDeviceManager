using System;
using System.Collections.Generic;
using System.Text;

namespace DevicesCommon
{
    /// <summary>
    /// �����������, � ������� �������� ��������
    /// </summary>
    public enum TurnstileDirection
    {
        /// <summary>
        /// �� ����
        /// </summary>
        Entry,

        /// <summary>
        /// �� �����
        /// </summary>
        Exit
    }

    /// <summary>
    /// ��������� ��������� ��� ���������� ����������
    /// </summary>
    public interface ITurnstileDevice : IRS485Device
    {
        #region ���������� ���������� ���������

        /// <summary>
        /// �����������, � ������� �������� ��������
        /// </summary>
        TurnstileDirection Direction { get; set; }

        /// <summary>
        /// ������� �������� ���������
        /// </summary>
        Int32 Timeout { get; set; }

        /// <summary>
        /// ������� ��������
        /// </summary>
        /// <returns>true, ���� � ������� �������� ����� �������� �������� ������</returns>
        Boolean Open();

        /// <summary>
        /// ������� ��������
        /// </summary>
        /// <param name="accessDenied">������ ��������</param>
        void Close(Boolean accessDenied);

        #endregion

        #region ������ � ����������������� �����������

        /// <summary>
        /// ��������� ���� ����������������� ������ �� ����������
        /// </summary>
        String IdentificationData { get; }

        #endregion
    }
}
