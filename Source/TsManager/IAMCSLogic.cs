using System;
using DevicesCommon;

namespace TsManager
{
    /// <summary>
    /// ���������, ����������� ������ ������ ����
    /// </summary>
    public interface IAMCSLogic
    {
        /// <summary>
        /// ���������, ����������� ��� ������
        /// </summary>
        Object Settings { get; set; }

        /// <summary>
        /// ��������� ����������� ������� � �������� �����������
        /// �� ����������������� ������
        /// </summary>
        /// <param name="direction">�����������</param>
        /// <param name="idData">����������������� ������</param>
        /// <param name="reason">������� ������ � �������</param>
        /// <returns>true, ���� ������ ��������</returns>
        bool IsAccessGranted(TurnstileDirection direction, string idData, 
            out string reason);

        /// <summary>
        /// ��������� ���� ������� � �������� �����������
        /// �� ����������������� ������
        /// </summary>
        /// <param name="direction">�����������</param>
        /// <param name="idData">����������������� ������</param>
        void OnAccessOccured(TurnstileDirection direction, string idData);
    }
}
