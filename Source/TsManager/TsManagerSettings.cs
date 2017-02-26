using System;
using System.Collections.Generic;

namespace TsManager
{
    /// <summary>
    /// Параметры менеджера турникетов
    /// </summary>
    [Serializable]
    public class TsManagerSettings
    {
        private List<AMCSLogicSettings> _logicSettings;

        /// <summary>
        /// Список настроек реализаций логики работы СКУД
        /// </summary>
        public List<AMCSLogicSettings> LogicSettings
        {
            get { return _logicSettings; }
            set { _logicSettings = value; }
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        public TsManagerSettings()
        {
            _logicSettings = new List<AMCSLogicSettings>();
        }
    }
}
