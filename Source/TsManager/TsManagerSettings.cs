using System;
using System.Collections.Generic;

namespace TsManager
{
    /// <summary>
    /// ��������� ��������� ����������
    /// </summary>
    [Serializable]
    public class TsManagerSettings
    {
        private List<AMCSLogicSettings> _logicSettings;

        /// <summary>
        /// ������ �������� ���������� ������ ������ ����
        /// </summary>
        public List<AMCSLogicSettings> LogicSettings
        {
            get { return _logicSettings; }
            set { _logicSettings = value; }
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        public TsManagerSettings()
        {
            _logicSettings = new List<AMCSLogicSettings>();
        }
    }
}
