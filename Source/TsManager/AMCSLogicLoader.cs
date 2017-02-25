using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace TsManager
{
    /// <summary>
    /// Загрузка реализаций логики работы СКУД
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
                    String.Format("Реализация логики СКУД \"{0}\" не найдена", name), e);
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
                    String.Format("Реализация логики СКУД \"{0}\" не найдена", name), e);
            }
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="directory">Папка, в которой следует искать реализации логики работы СКУД</param>
        public AMCSLogicLoader(String directory)
        {
            _logicTypes = new Dictionary<String, Type>();
            _settingsTypes = new Dictionary<String, Type>();
            foreach (String fileName in Directory.GetFiles(directory, "*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    // загружаем очередную сборку
                    Assembly asm = Assembly.LoadFrom(fileName);
                    // загружаем типы данных сборки
                    Type[] asmTypes = asm.GetTypes();

                    foreach (Type asmType in asmTypes)
                    {
                        // получаем список атрибутов для типа
                        Attribute[] attributes = Attribute.GetCustomAttributes(asmType);
                        foreach (Attribute attribute in attributes)
                        {
                            // проверяем, не определяет ли атрибут реализацию
                            // логики работы СКУД
                            if (attribute is AMCSLogicAttrubute)
                            {
                                // добавляем тип в словарь
                                String amcsName = ((AMCSLogicAttrubute)attribute).AMCSName;
                                _logicTypes.Add(amcsName, asmType);
                                _settingsTypes.Add(amcsName, GetSettings(asmType));
                                // переходим к следующему типу
                                break;
                            }
                        }
                    }
                }
                catch (FileLoadException)
                {
                    // пропускаем файл
                }
                catch (BadImageFormatException)
                {
                    // пропускаем файл
                }
            }
        }

        /// <summary>
        /// Создает реализацию логики работы СКУД по наименованию СКУД
        /// </summary>
        /// <param name="amcsName">Наименование СКУД</param>
        /// <returns>Реализация логики работы СКУД</returns>
        public IAMCSLogic CreateLogic(String amcsName)
        {
            return (IAMCSLogic)Activator.CreateInstance(GetLogic(amcsName));
        }

        /// <summary>
        /// Список наименований загруженных реализаций логики работы СКУД
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
        /// Список типов, хранящих настройки реализаций логики работы СКУД
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
        /// Создает экземпляр настроек реализации логики работы СКУД
        /// </summary>
        /// <param name="amcsName">Наименование СКУД</param>
        public Object CreateLogicSettings(String amcsName)
        {
            return Activator.CreateInstance(GetSettings(amcsName));
        }

        /// <summary>
        /// Является ли список реализации логики работы СКУД пустым
        /// </summary>
        public Boolean Empty
        {
            get { return _logicTypes.Count == 0; }
        }
    }
}
