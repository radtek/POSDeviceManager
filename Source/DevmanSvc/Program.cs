using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;

namespace DevmanSvc
{
    /// <summary>
    /// Основная программа
    /// </summary>
	static class Program
	{
		/// <summary>
		/// Точка входа в приложение
		/// </summary>
		static void Main()
		{
            ServiceBase.Run(new DeviceManagerService());
		}
	}
}