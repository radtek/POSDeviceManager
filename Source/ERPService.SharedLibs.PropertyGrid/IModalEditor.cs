using System;
using System.ComponentModel;

namespace ERPService.SharedLibs.PropertyGrid
{
    /// <summary>
    /// Интерфейс, реализуемый модальными редакторами свойств
    /// </summary>
    public interface IModalEditor
    {
        /// <summary>
        /// Отображение модального редактора
        /// </summary>
        /// <param name="descriptorContext">Контекст для получения дополнительной информации о свойстве</param>
        bool ShowEditor(ITypeDescriptorContext descriptorContext);

        /// <summary>
        /// Редактируемое значение
        /// </summary>
        Object Value { get; set; }
    }
}
