using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERPService.SharedLibs.PropertyGrid.Forms;

namespace ERPService.SharedLibs.PropertyGrid.Editors
{
    /// <summary>
    /// Редактор для свойств, представляющих сложные типы
    /// </summary>
    public class ComplexPropertyEditor : CustomModalEditor
    {
        /// <summary>
        /// Возвращает ссылку на интерфейс модального редактора
        /// </summary>
        protected override IModalEditor GetEditor()
        {
            return new FormComplexPropertyEditor();
        }
    }
}
