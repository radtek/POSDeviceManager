using System;
using System.Collections.Generic;
using System.Text;
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
        Boolean IsAccessGranted(TurnstileDirection direction, String idData, 
            out String reason);

        /// <summary>
        /// ��������� ���� ������� � �������� �����������
        /// �� ����������������� ������
        /// </summary>
        /// <param name="direction">�����������</param>
        /// <param name="idData">����������������� ������</param>
        void OnAccessOccured(TurnstileDirection direction, String idData);
    }
}
