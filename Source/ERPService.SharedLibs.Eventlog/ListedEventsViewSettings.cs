using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// Ширина колонок в списке просмотра событий
    /// </summary>
    [Serializable]
    public class ListedEventsViewSettings
    {
        private Int32[] _columnWidth;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        public ListedEventsViewSettings()
        {
            _columnWidth = new Int32[] { 100, 100, 100, 250 };
        }

        /// <summary>
        /// Возвращает ширину колонки
        /// </summary>
        /// <param name="index">Индекс</param>
        public Int32 this[Int32 index]
        {
            get { return _columnWidth[index]; }
            set { _columnWidth[index] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32[] ColumnWidth
        {
            get { return _columnWidth; }
            set { _columnWidth = value; }
        }
    }
}
