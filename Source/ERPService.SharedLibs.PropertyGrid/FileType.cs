namespace ERPService.SharedLibs.PropertyGrid
{
    /// <summary>
    /// Вспомогательный класс для описания типа файлов в диалоге редактирования
    /// свойств, являющихся именами файлов
    /// </summary>
    public class FileType
    {
        private string _description;
        private string _extension;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="description">Описание типа файлов</param>
        /// <param name="extension">Расширение, сопоставленное типу файлов</param>
        public FileType(string description, string extension)
        {
            _description = description;
            _extension = extension;
        }

        /// <summary>
        /// Описание типа файлов
        /// </summary>
        public string Descpription
        {
            get { return _description; }
        }

        /// <summary>
        /// Расширение, сопоставленное типу файлов
        /// </summary>
        public string Extension
        {
            get { return _extension; }
        }

        /// <summary>
        /// Строковое представление объекта
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0} (*.{1})|*.{1}", _description, _extension);
        }
    }
}
