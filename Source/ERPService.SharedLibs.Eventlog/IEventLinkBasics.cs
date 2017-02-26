using System;

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
            string[] sourceFilter, EventType[] eventFilter);

        /// <summary>
        /// �������� �������
        /// </summary>
        /// <param name="fromDate">������ �� ����-�������, ��������� ��������</param>
        /// <param name="toDate">������ �� ����-�������, �������� ��������</param>
        /// <param name="sourceFilter">������ �� ��������� �������</param>
        /// <param name="eventFilter">������ �� ����� �������</param>
        /// <param name="maxEvents">������������ ���������� �������</param>
        EventRecord[] GetLog(DateTime fromDate, DateTime toDate,
            string[] sourceFilter, EventType[] eventFilter, int maxEvents);

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
        string BeginGetLog(DateTime fromDate, DateTime toDate, string[] sourceFilter, 
            EventType[] eventFilter, int maxEvents, int eventPerIteration);

        /// <summary>
        /// ��������� ��������� ���� ������� �� �������
        /// </summary>
        /// <param name="iteratorId">������������� ���������</param>
        /// <returns>
        /// ��������� ���� ������� �� ������� ��� null, ���� ��������� ����� �������
        /// </returns>
        EventRecord[] GetLog(string iteratorId);

        /// <summary>
        /// ��������� ���������������� ������ � �������� 
        /// </summary>
        /// <param name="iteratorId">������������� ���������</param>
        void EndGetLog(string iteratorId);

        #endregion
    }
}
