using System;
using System.Collections.Generic;
using System.Text;
using ERPService.SharedLibs.PropertyGrid.Forms;

namespace ERPService.SharedLibs.PropertyGrid.Editors
{
    /// <summary>
    /// Редактор для свойств-текстовых блоков
    /// </summary>
    public sealed class TextEditor : CustomModalEditor
    {
        /// <summary>
        /// Возвращает ссылку на интерфейс модального редактора
        /// </summary>
        protected override IModalEditor GetEditor()
        {
            return new FormTextEditor();
        }
    }
}
