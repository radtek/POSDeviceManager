using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// ��������� ��� ���������� �������
    /// </summary>
    public interface IEventLink : IEventLinkBasics
    {
        /// <summary>
        /// ���������� �������
        /// </summary>
        /// <param name="sourceId">������������� ��������� �������</param>
        /// <param name="eventType">��� �������</param>
        /// <param name="text">��������� ������ �������</param>
        void Post(string sourceId, EventType eventType, string[] text);

        /// <summary>
        /// ���������� ��������������� ������� 
        /// </summary>
        /// <param name="sourceId">������������� ��������� �������</param>
        /// <param name="text">��������� ������ �������</param>
        void Post(string sourceId, string[] text);

        /// <summary>
        /// ���������� �������
        /// </summary>
        /// <param name="sourceId">������������� ��������� �������</param>
        /// <param name="eventType">��� �������</param>
        /// <param name="text">��������� ������ �������</param>
        void Post(string sourceId, EventType eventType, string text);

        /// <summary>
        /// ���������� ��������������� �������
        /// </summary>
        /// <param name="sourceId">������������� ��������� �������</param>
        /// <param name="text">��������� ������ �������</param>
        void Post(string sourceId, string text);

        /// <summary>
        /// ���������� ���������� �� ����������
        /// </summary>
        /// <param name="sourceId">������������� ��������� �������</param>
        /// <param name="checkPointId">�������� ����������� �����</param>
        /// <param name="e">����������</param>
        void Post(string sourceId, string checkPointId, Exception e);

        /// <summary>
        /// ���������� ���������� �� ����������
        /// </summary>
        /// <param name="sourceId">������������� ��������� �������</param>
        /// <param name="exceptionData">������ ����������, �������������� ��� ����������</param>
        void PostException(string sourceId, string[] exceptionData);
    }
}
