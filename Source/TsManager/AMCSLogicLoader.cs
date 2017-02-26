using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace TsManager
{
    /// <summary>
    /// �������� ���������� ������ ������ ����
    /// </summary>
    public class AMCSLogicLoader
    {
        private Dictionary<string, Type> _logicTypes;
        private Dictionary<string, Type> _settingsTypes;

        private Type GetLogic(string name)
        {
            try
            {
                return _logicTypes[name];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(
                    string.Format("���������� ������ ���� \"{0}\" �� �������", name), e);
            }
        }

        private Type GetSettings(Type amcsType)
        {
            IAMCSLogic instance = (IAMCSLogic)Activator.CreateInstance(amcsType);
            return instance.Settings.GetType();
        }

        private Type GetSettings(string name)
        {
            try
            {
                return _settingsTypes[name];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(
                    string.Format("���������� ������ ���� \"{0}\" �� �������", name), e);
            }
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="directory">�����, � ������� ������� ������ ���������� ������ ������ ����</param>
        public AMCSLogicLoader(string directory)
        {
            _logicTypes = new Dictionary<string, Type>();
            _settingsTypes = new Dictionary<string, Type>();
            foreach (string fileName in Directory.GetFiles(directory, "*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    // ��������� ��������� ������
                    Assembly asm = Assembly.LoadFrom(fileName);
                    // ��������� ���� ������ ������
                    Type[] asmTypes = asm.GetTypes();

                    foreach (Type asmType in asmTypes)
                    {
                        // �������� ������ ��������� ��� ����
                        Attribute[] attributes = Attribute.GetCustomAttributes(asmType);
                        foreach (Attribute attribute in attributes)
                        {
                            // ���������, �� ���������� �� ������� ����������
                            // ������ ������ ����
                            if (attribute is AMCSLogicAttrubute)
                            {
                                // ��������� ��� � �������
                                string amcsName = ((AMCSLogicAttrubute)attribute).AMCSName;
                                _logicTypes.Add(amcsName, asmType);
                                _settingsTypes.Add(amcsName, GetSettings(asmType));
                                // ��������� � ���������� ����
                                break;
                            }
                        }
                    }
                }
                catch (FileLoadException)
                {
                    // ���������� ����
                }
                catch (BadImageFormatException)
                {
                    // ���������� ����
                }
            }
        }

        /// <summary>
        /// ������� ���������� ������ ������ ���� �� ������������ ����
        /// </summary>
        /// <param name="amcsName">������������ ����</param>
        /// <returns>���������� ������ ������ ����</returns>
        public IAMCSLogic CreateLogic(string amcsName)
        {
            return (IAMCSLogic)Activator.CreateInstance(GetLogic(amcsName));
        }

        /// <summary>
        /// ������ ������������ ����������� ���������� ������ ������ ����
        /// </summary>
        public string[] GetLogicNames()
        {
            List<string> logicNames = new List<string>();
            foreach (KeyValuePair<string, Type> kvp in _logicTypes)
            {
                logicNames.Add(kvp.Key);
            }
            return logicNames.ToArray();
        }

        /// <summary>
        /// ������ �����, �������� ��������� ���������� ������ ������ ����
        /// </summary>
        public Type[] GetLogicSettingsTypes()
        {
            List<Type> settingsTypes = new List<Type>();
            foreach (KeyValuePair<string, Type> kvp in _settingsTypes)
            {
                settingsTypes.Add(kvp.Value);
            }
            return settingsTypes.ToArray();
        }

        /// <summary>
        /// ������� ��������� �������� ���������� ������ ������ ����
        /// </summary>
        /// <param name="amcsName">������������ ����</param>
        public Object CreateLogicSettings(string amcsName)
        {
            return Activator.CreateInstance(GetSettings(amcsName));
        }

        /// <summary>
        /// �������� �� ������ ���������� ������ ������ ���� ������
        /// </summary>
        public bool Empty
        {
            get { return _logicTypes.Count == 0; }
        }
    }
}
