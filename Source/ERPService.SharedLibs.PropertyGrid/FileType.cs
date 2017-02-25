using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.PropertyGrid
{
    /// <summary>
    /// Вспомогательный класс для описания типа файлов в диалоге редактирования
    /// свойств, являющихся именами файлов
    /// </summary>
    public class FileType
    {
        private String _description;
        private String _extension;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="description">Описание типа файлов</param>
        /// <param name="extension">Расширение, сопоставленное типу файлов</param>
        public FileType(String description, String extension)
        {
            _description = description;
            _extension = extension;
        }

        /// <summary>
        /// Описание типа файлов
        /// </summary>
        public String Descpription
        {
            get { return _description; }
        }

        /// <summary>
        /// Расширение, сопоставленное типу файлов
        /// </summary>
        public String Extension
        {
            get { return _extension; }
        }

        /// <summary>
        /// Строковое представление объекта
        /// </summary>
        public override String ToString()
        {
            return String.Format("{0} (*.{1})|*.{1}", _description, _extension);
        }
    }
}
