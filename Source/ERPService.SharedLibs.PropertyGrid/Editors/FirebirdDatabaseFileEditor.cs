namespace ERPService.SharedLibs.PropertyGrid.Editors
{
    /// <summary>
    /// Редактор для свойств-имен файлов баз данных Firebird, Interbase
    /// </summary>
    public class FirebirdDatabaseFileEditor : CustomFileNameEditor
    {
        /// <summary>
        /// Поддерживаемые типы файлов
        /// </summary>
        protected override FileType[] SupportedFileTypes
        {
            get
            {
                return new FileType[] {
                    new FileType("Базы данных Firebird", "fdb"),
                    new FileType("Базы данных Interbase", "gdb"),
                    new FileType("Базы данных Interbase 7", "ib"),
                };
            }
        }
    }
}
