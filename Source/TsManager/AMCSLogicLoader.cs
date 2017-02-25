using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace TsManager
{
    /// <summary>
    /// �������� ���������� ������ ������ ����
    /// </summary>
    public class AMCSLogicLoader
    {
        private Dictionary<String, Type> _logicTypes;
        private Dictionary<String, Type> _settingsTypes;

        private Type GetLogic(String name)
        {
            try
            {
                return _logicTypes[name];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(
                    String.Format("���������� ������ ���� \"{0}\" �� �������", name), e);
            }
        }

        private Type GetSettings(Type amcsType)
        {
            IAMCSLogic instance = (IAMCSLogic)Activator.CreateInstance(amcsType);
            return instance.Settings.GetType();
        }

        private Type GetSettings(String name)
        {
            try
            {
                return _settingsTypes[name];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(
                    String.Format("���������� ������ ���� \"{0}\" �� �������", name), e);
            }
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="directory">�����, � ������� ������� ������ ���������� ������ ������ ����</param>
        public AMCSLogicLoader(String directory)
        {
            _logicTypes = new Dictionary<String, Type>();
            _settingsTypes = new Dictionary<String, Type>();
            foreach (String fileName in Directory.GetFiles(directory, "*.dll", SearchOption.AllDirectories))
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
                                String amcsName = ((AMCSLogicAttrubute)attribute).AMCSName;
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
        public IAMCSLogic CreateLogic(String amcsName)
        {
            return (IAMCSLogic)Activator.CreateInstance(GetLogic(amcsName));
        }

        /// <summary>
        /// ������ ������������ ����������� ���������� ������ ������ ����
        /// </summary>
        public String[] GetLogicNames()
        {
            List<String> logicNames = new List<String>();
            foreach (KeyValuePair<String, Type> kvp in _logicTypes)
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
            foreach (KeyValuePair<String, Type> kvp in _settingsTypes)
            {
                settingsTypes.Add(kvp.Value);
            }
            return settingsTypes.ToArray();
        }

        /// <summary>
        /// ������� ��������� �������� ���������� ������ ������ ����
        /// </summary>
        /// <param name="amcsName">������������ ����</param>
        public Object CreateLogicSettings(String amcsName)
        {
            return Activator.CreateInstance(GetSettings(amcsName));
        }

        /// <summary>
        /// �������� �� ������ ���������� ������ ������ ���� ������
        /// </summary>
        public Boolean Empty
        {
            get { return _logicTypes.Count == 0; }
        }
    }
}
