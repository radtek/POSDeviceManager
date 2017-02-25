using System;
using System.Collections.Generic;
using System.Text;
using ERPService.SharedLibs.PropertyGrid.Forms;

namespace ERPService.SharedLibs.PropertyGrid
{
    /// <summary>
    /// Базовый класс для редакторов набора опций
    /// </summary>
    /// <typeparam name="T">Тип, в котором хранятся опции (строка, перечисление и т.п.)</typeparam>
    public abstract class CustomOptionsEditor<T> : CustomModalEditor, IOptionsProvider<T>
    {
        /// <summary>
        /// Возвращает ссылку на интерфейс модального редактора
        /// </summary>
        protected override IModalEditor GetEditor()
        {
            FormOptionsEditor<T> optionsEditor = new FormOptionsEditor<T>();
            optionsEditor.OptionsProvider = this;
            return optionsEditor;
        }

        /// <summary>
        /// Возвращает набор опций для редактирования
        /// </summary>
        public abstract EditableOption<T>[] Options { get; }
    }
}
