using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace ERPService.SharedLibs.PropertyGrid
{
    /// <summary>
    /// Базовый класс для редакторов свойств, отображающих диалоговые окна
    /// </summary>
    public abstract class CustomModalEditor : CustomEditor
    {
        /// <summary>
        /// Gets the editor style used by the EditValue method
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that can be used to gain additional context information</param>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            // реактирование будет производится в диалоговом окне
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        /// Возвращает измененное значение свойства
        /// </summary>
        /// <param name="value">Исходное значение</param>
        protected override Object OnEdit(Object value)
        {
            // получаем ссылку на интерфейс редактора
            IModalEditor editor = GetEditor();
            // инициализируем значение редактируемого свойства
            editor.Value = value;
            // показываем диалог
            if (editor.ShowEditor(DescriptorContext))
            {
                // меняем значение свойства
                value = editor.Value;
            }
            // возвращаем значение свойства
            return value;
        }

        /// <summary>
        /// Возвращает ссылку на интерфейс модального редактора
        /// </summary>
        abstract protected IModalEditor GetEditor();
    }
}
