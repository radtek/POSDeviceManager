using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;

namespace DevmanSvc
{
    /// <summary>
    /// �������� ���������
    /// </summary>
	static class Program
	{
		/// <summary>
		/// ����� ����� � ����������
		/// </summary>
		static void Main()
		{
            ServiceBase.Run(new DeviceManagerService());
		}
	}
}