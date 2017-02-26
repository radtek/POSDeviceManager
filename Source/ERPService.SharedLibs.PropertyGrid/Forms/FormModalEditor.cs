using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ERPService.SharedLibs.PropertyGrid.Forms
{
    /// <summary>
    /// Базовый класс для модальных редакторов
    /// </summary>
    public partial class FormModalEditor : Form, IModalEditor
    {
        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        public FormModalEditor()
        {
            InitializeComponent();
        }

        #region Реализация IModalEditor

        /// <summary>
        /// Отображение модального редактора
        /// </summary>
        /// <param name="descriptorContext">Контекст для получения дополнительной информации о свойстве</param>
        public virtual bool ShowEditor(ITypeDescriptorContext descriptorContext)
        {
            Text = descriptorContext.PropertyDescriptor.DisplayName;
            lblDescription.Text = descriptorContext.PropertyDescriptor.Description;
            return ShowDialog() == DialogResult.OK;
        }

        /// <summary>
        /// Редактируемое значение
        /// </summary>
        public virtual Object Value
        {
            get { return null; }
            set { }
        }

        #endregion
    }
}