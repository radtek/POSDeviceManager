using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon.Connectors;
using DevicesCommon.Helpers;
using DevicesCommon;
using System.Xml;

namespace DevmanTest
{
    class Program
    {
        static void ViewHelp()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  DevmanTest [server] [deviceId] [docName] [repeatCount]");
            Console.WriteLine();
        }

        static Boolean ProcessErrorCode(ErrorCode errorCode, Boolean writeIfSuccess)
        {
            if ((errorCode.Succeeded && writeIfSuccess) || errorCode.Failed)
                Console.WriteLine(String.Format("Error code: {0}", errorCode.ToString()));

            if (errorCode.Failed)
            {
                Console.WriteLine("Error. View full description (y/n)?");
                ConsoleKeyInfo info = Console.ReadKey(true);
                Console.WriteLine();
                if (info.Key == ConsoleKey.Y)
                    Console.WriteLine(errorCode.FullDescription);
            }

            return errorCode.Value == GeneralError.Success;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("POS device manager test utility");
            Console.WriteLine("version 2.0");
            Console.WriteLine();

            try
            {
                if (args.Length == 0)
                    ViewHelp();
                else
                {
                    String host = args.Length > 0 ? args[0] : "localhost";
                    using (DeviceManagerClient dmc = new DeviceManagerClient(host))
                    {
                        dmc.Login();
                        String deviceID = args.Length > 1 ? args[1] : "Устройство1";
                        dmc.Capture(deviceID, WaitConstant.Infinite);
                        try
                        {
                            IPrintableDevice device = (IPrintableDevice)dmc[deviceID];
                            Console.WriteLine(String.Format("Device status: {0}",
                                device.Active ? "active" : "inactive"));

                            if (!device.Active)
                            {
                                Console.WriteLine("Trying to re-activate.");
                                device.Active = true;
                                if (!device.Active)
                                    Console.WriteLine("Re-activate failed.");
                                else
                                    Console.WriteLine("Re-activate succeeded.");
                            }

                            if (device.Active)
                            {
                                PaperOutStatus paperStatus = device.PrinterStatus.PaperOut;
                                if (ProcessErrorCode(device.ErrorCode, false))
                                {
                                    Console.WriteLine(String.Format("Paper: {0}", paperStatus));
                                    if (paperStatus == PaperOutStatus.Present || paperStatus == PaperOutStatus.OutAfterActive)
                                    {
                                        XmlDocument xmlDoc = new XmlDocument();
                                        xmlDoc.Load(args.Length > 2 ? args[2] : "receipt.xml");
                                        device.Print(xmlDoc.OuterXml);
                                        ProcessErrorCode(device.ErrorCode, true);
                                    }
                                    else
                                        Console.WriteLine("Printer not ready.");
                                }
                            }
                        }
                        finally
                        {
                            dmc.Release(deviceID);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception.");
                Console.WriteLine(e.Message);
                Console.WriteLine("View stack trace (y/n)?:");
                ConsoleKeyInfo info = Console.ReadKey(true);
                Console.WriteLine();
                if (info.Key == ConsoleKey.Y)
                    Console.WriteLine(e.StackTrace);
            }

            Console.WriteLine("Done. Press any key.");
            Console.ReadKey(true);
        }
    }
}
