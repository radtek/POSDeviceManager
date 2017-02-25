using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceProcess;

namespace DevicesCommon.Helpers
{
    /// <summary>
    /// ����������� �������� ������������ ServiceControllerStatus � ������
    /// </summary>
    public class ServiceStatus
    {
        private ServiceControllerStatus _originalStatus;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="originalStatus">�������� �������� ������� �������</param>
        public ServiceStatus(ServiceControllerStatus originalStatus)
        {
            _originalStatus = originalStatus;
        }

        /// <summary>
        /// ���������� ��������� ������������� �������
        /// </summary>
        public override String ToString()
        {
 	        switch(_originalStatus)
            {
                case ServiceControllerStatus.ContinuePending:
                    return "����������� ������";
                case ServiceControllerStatus.Paused:
                    return "�������������";
                case ServiceControllerStatus.PausePending:
                    return "������������������";
                case ServiceControllerStatus.Running:
                    return "��������";
                case ServiceControllerStatus.StartPending:
                    return "�����������";
                case ServiceControllerStatus.Stopped:
                    return "����������";
                default:
                    return "���������������";
            }
        }
    }
}
