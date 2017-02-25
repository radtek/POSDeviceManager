using System;
using System.Collections.Generic;
using System.Text;
using ERPService.SharedLibs.PropertyGrid.Forms;

namespace ERPService.SharedLibs.PropertyGrid.Editors
{
    /// <summary>
    /// Редкатор для свойств-паролей
    /// </summary>
    public class PasswordEditor : CustomModalEditor
    {
        /// <summary>
        /// Возвращает реализацию интерфейса для редактирования свойства
        /// </summary>
        protected override IModalEditor GetEditor()
        {
            return new FormPasswordEditor();
        }
    }
}
