using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using ERPService.SharedLibs.PropertyGrid;

namespace TsManager
{
    /// <summary>
    /// Редактор настроек реализации логики работы СКУД
    /// </summary>
    public class AMCSLogicEditor : CustomModalEditor
    {
        /// <summary>
        /// Ссылка на интерфейс редактора
        /// </summary>
        protected override IModalEditor GetEditor()
        {
            return new FormAMCSLogicEditor();
        }
    }
}
