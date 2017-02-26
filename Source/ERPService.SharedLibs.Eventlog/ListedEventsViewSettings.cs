using System;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// ������ ������� � ������ ��������� �������
    /// </summary>
    [Serializable]
    public class ListedEventsViewSettings
    {
        private int[] _columnWidth;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        public ListedEventsViewSettings()
        {
            _columnWidth = new int[] { 100, 100, 100, 250 };
        }

        /// <summary>
        /// ���������� ������ �������
        /// </summary>
        /// <param name="index">������</param>
        public int this[int index]
        {
            get { return _columnWidth[index]; }
            set { _columnWidth[index] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int[] ColumnWidth
        {
            get { return _columnWidth; }
            set { _columnWidth = value; }
        }
    }
}
