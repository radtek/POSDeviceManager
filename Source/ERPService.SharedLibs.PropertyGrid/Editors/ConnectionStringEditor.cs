using ERPService.SharedLibs.PropertyGrid.Forms;

namespace ERPService.SharedLibs.PropertyGrid.Editors
{
    /// <summary>
    /// Редактор для свойств, являющихся строками подключения к базе данных
    /// </summary>
    public class ConnectionStringEditor : CustomModalEditor
    {
        /// <summary>
        /// Возвращает реализацию интерфейса для редактирования свойства
        /// </summary>
        protected override IModalEditor GetEditor()
        {
            return new FormConnectionStringEditor();
        }
    }
}
