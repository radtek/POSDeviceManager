using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// ���������, ������������� �������� ���������������� ��� ������ � �������� �������
    /// </summary>
    public interface IEventLinkBasics
    {
        /// <summary>
        /// �������� �������
        /// </summary>
        /// <param name="fromDate">������ �� ����-�������, ��������� ��������</param>
        /// <param name="toDate">������ �� ����-�������, �������� ��������</param>
        /// <param name="sourceFilter">������ �� ��������� �������</param>
        /// <param name="eventFilter">������ �� ����� �������</param>
        EventRecord[] GetLog(DateTime fromDate, DateTime toDate,
            String[] sourceFilter, EventType[] eventFilter);

        /// <summary>
        /// �������� �������
        /// </summary>
        /// <param name="fromDate">������ �� ����-�������, ��������� ��������</param>
        /// <param name="toDate">������ �� ����-�������, �������� ��������</param>
        /// <param name="sourceFilter">������ �� ��������� �������</param>
        /// <param name="eventFilter">������ �� ����� �������</param>
        /// <param name="maxEvents">������������ ���������� �������</param>
        EventRecord[] GetLog(DateTime fromDate, DateTime toDate,
            String[] sourceFilter, EventType[] eventFilter, Int32 maxEvents);

        /// <summary>
        /// �������������� ������� �������
        /// </summary>
        /// <param name="fromDate">������ �� ����-�������, ��������� ��������</param>
        /// <param name="toDate">������ �� ����-�������, �������� ��������</param>
        void TruncLog(DateTime fromDate, DateTime toDate);

        #region ���������������� ������ � ��������

        /// <summary>
        /// ������ ���������������� ������ � ��������
        /// </summary>
        /// <param name="fromDate">������ �� ����-�������, ��������� ��������</param>
        /// <param name="toDate">������ �� ����-�������, �������� ��������</param>
        /// <param name="sourceFilter">������ �� ��������� �������</param>
        /// <param name="eventFilter">������ �� ����� �������</param>
        /// <param name="maxEvents">������������ ���������� �������</param>
        /// <param name="eventPerIteration">
        /// ������������ ���������� �������, ������������ �� ������ �����
        /// ������ IEventLinkBasics.GetLog
        /// </param>
        /// <returns>������������� ���������</returns>
        String BeginGetLog(DateTime fromDate, DateTime toDate, String[] sourceFilter, 
            EventType[] eventFilter, Int32 maxEvents, Int32 eventPerIteration);

        /// <summary>
        /// ��������� ��������� ���� ������� �� �������
        /// </summary>
        /// <param name="iteratorId">������������� ���������</param>
        /// <returns>
        /// ��������� ���� ������� �� ������� ��� null, ���� ��������� ����� �������
        /// </returns>
        EventRecord[] GetLog(String iteratorId);

        /// <summary>
        /// ��������� ���������������� ������ � �������� 
        /// </summary>
        /// <param name="iteratorId">������������� ���������</param>
        void EndGetLog(String iteratorId);

        #endregion
    }
}
