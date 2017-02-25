using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace ERPService.SharedLibs.Helpers.SerialCommunications
{
    /// <summary>
    /// ��������������� ����� ��� ����������� ������ ��������� COM � LPT-������
    /// </summary>
    public static class SerialPortsEnumerator
    {
        private static String[] Enumerate(String registryKeyName, String portNamePattern)
        {
            List<String> values = new List<String>();

            RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKeyName);
            if (key != null)
            {
                try
                {
                    foreach (String valueName in key.GetValueNames())
                    {
                        String value = key.GetValue(valueName).ToString();

                        if (String.IsNullOrEmpty(portNamePattern))
                            values.Add(value);
                        else
                        {
                            Int32 index = value.IndexOf(portNamePattern);
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
        /// ���������� ������ ��������� COM-������
        /// </summary>
        public static String[] Enumerate()
        {
            return Enumerate(@"HARDWARE\DEVICEMAP\SERIALCOMM", String.Empty);
        }

        /// <summary>
        /// ���������� ������ ��������� LPT-������
        /// </summary>
        public static String[] EnumerateLPT()
        {
            return Enumerate(@"HARDWARE\DEVICEMAP\PARALLEL PORTS", "LPT");
        }
    }
}
