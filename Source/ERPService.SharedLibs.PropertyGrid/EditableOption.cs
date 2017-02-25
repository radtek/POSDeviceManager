using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.PropertyGrid
{
    /// <summary>
    /// Опция для редактирования
    /// </summary>
    /// <typeparam name="T">Тип, в котором хранятся опции (строка, перечисление и т.п.)</typeparam>
    public class EditableOption<T>
    {
        private String _displayName;
        private T _keyword;

        /// <summary>
        /// Отображаемое имя
        /// </summary>
        public String DisplayName
        {
            get { return _displayName; }
        }

        /// <summary>
        /// Значение для записи/чтения
        /// </summary>
        public T Keyword
        {
            get { return _keyword; }
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="displayName">Отображаемое имя</param>
        /// <param name="keyword">Значение для записи/чтения</param>
        public EditableOption(String displayName, T keyword)
        {
            _displayName = displayName;
            _keyword = keyword;
        }

        /// <summary>
        /// Строковое представление объекта
        /// </summary>
        public override String ToString()
        {
            return DisplayName;
        }
    }
}
