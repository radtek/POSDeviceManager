using System.Collections.Generic;
using Microsoft.Win32;

namespace ERPService.SharedLibs.Helpers.SerialCommunications
{
    /// <summary>
    /// ¬спомогательный класс дл€ определени€ списка доступных COM и LPT-портов
    /// </summary>
    public static class SerialPortsEnumerator
    {
        private static string[] Enumerate(string registryKeyName, string portNamePattern)
        {
            List<string> values = new List<string>();

            RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKeyName);
            if (key != null)
            {
                try
                {
                    foreach (string valueName in key.GetValueNames())
                    {
                        string value = key.GetValue(valueName).ToString();

                        if (string.IsNullOrEmpty(portNamePattern))
                            values.Add(value);
                        else
                        {
                            int index = value.IndexOf(portNamePattern);
                            if (index != -1)
                                values.Add(value.Substring(index));
                        }
                    }
                    values.Sort();
                }
                finally
                {
                    key.Close();
                }
            }

            return values.ToArray();
        }

        /// <summary>
        /// ¬озвращает список доступных COM-портов
        /// </summary>
        public static string[] Enumerate()
        {
            return Enumerate(@"HARDWARE\DEVICEMAP\SERIALCOMM", string.Empty);
        }

        /// <summary>
        /// ¬озвращает список доступных LPT-портов
        /// </summary>
        public static string[] EnumerateLPT()
        {
            return Enumerate(@"HARDWARE\DEVICEMAP\PARALLEL PORTS", "LPT");
        }
    }
}
