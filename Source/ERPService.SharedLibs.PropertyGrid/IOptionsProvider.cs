using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.PropertyGrid
{
    /// <summary>
    /// Интерфейс, реализующий набор опций
    /// </summary>
    /// <typeparam name="T">Тип, в котором хранятся опции (строка, перечисление и т.п.)</typeparam>
    public interface IOptionsProvider<T>
    {
        /// <summary>
        /// Возвращает набор опций с текстовым описанием
        /// </summary>
        EditableOption<T>[] Options { get; }
    }
}
