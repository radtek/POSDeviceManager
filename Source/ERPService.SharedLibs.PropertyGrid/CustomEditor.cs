using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;

namespace ERPService.SharedLibs.PropertyGrid
{
    /// <summary>
    /// Ѕазовый класс дл€ всех редакторов свойств
    /// </summary>
    public abstract class CustomEditor : UITypeEditor
    {
        // ссылка на интерфейс дл€ отображени€ элементов управлени€ 
        // в property grid
        private IWindowsFormsEditorService _edSvc;
        // ссылка на контекст описани€ свойства
        private ITypeDescriptorContext _descriptorContext;
        // редактируемое значение
        private Object _value;

        /// <summary>
        /// Edits the value of the specified object using the editor style indicated by the GetEditStyle method
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that can be used to gain additional context information</param>
        /// <param name="provider">An IServiceProvider that this editor can use to obtain services</param>
        /// <param name="value">The object to edit</param>
        public override Object EditValue(ITypeDescriptorContext context, IServiceProvider provider,
            Object value)
        {
            // значение запоминаетс€ дл€ доступа из наследников
            _value = value;
            try
            {
                // получаем ссылку на интерфейс Windows Forms
                _edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                // инициализируем ссылку на описание редактируемого свойства
                _descriptorContext = context;

                // вызываем метод дл€ редактировани€, реализованный в потомке
                return _edSvc == null ? value : OnEdit(value);
            }
            finally
            {
                _value = null;
            }
        }

        /// <summary>
        /// ¬озвращает ссылку на интерфейс дл€ отображени€ элементов управлени€
        /// </summary>
        protected IWindowsFormsEditorService EdSvc
        {
            get { return _edSvc; }
        }

        /// <summary>
        /// ¬озвращает контекст описани€ редактируемого свойства
        /// </summary>
        protected ITypeDescriptorContext DescriptorContext
        {
            get { return _descriptorContext; }
        }

        /// <summary>
        /// –едактируемое значение
        /// </summary>
        protected Object Value
        {
            get { return _value; }
        }

        /// <summary>
        /// ¬озвращает измененное значение свойства
        /// </summary>
        /// <param name="value">»сходное значение</param>
        abstract protected Object OnEdit(Object value);
    }
}
