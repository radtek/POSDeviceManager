using System;
using System.Linq;
using System.ServiceProcess;

namespace DevmanSvc
{
    /// <summary>
    /// Основная программа
    /// </summary>
    static class Program
    {
        private const string ConsoleApplicationSwitch = "/console";
        private const string POSDeviceManagerTitle = "POS Device Manager (отладочный режим).";
        private const string ServiceStarted = "Сервис запущен.";
        private const string ServiceStopped = "Сервис остановлен.";
        private const string PressEnterToStop = "Нажмите [ENTER], чтобы остановить сервис.";

        /// <summary>
        /// Точка входа в приложение
        /// </summary>
        static void Main(string[] args)
        {
            if (args.FirstOrDefault() == ConsoleApplicationSwitch)
            {
                RunAsConsoleApplication(args);
            }
            else
            {
                RunAsService();
            }
        }

        static void RunAsConsoleApplication(string[] args)
        {
            Console.WriteLine(POSDeviceManagerTitle);

            try
            {
                using (var deviceManagerService = new DeviceManagerService())
                {
                    deviceManagerService.StartApplication(args);

                    Console.WriteLine(ServiceStarted);
                    Console.WriteLine(PressEnterToStop);
                    Console.ReadLine();

                    deviceManagerService.StopApplication();

                    Console.WriteLine(ServiceStopped);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine();
                Console.WriteLine(PressEnterToStop);
            }
        }

        static void RunAsService()
        {
            ServiceBase.Run(new DeviceManagerService());
        }
    }
}