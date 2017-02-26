using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Design;

namespace ERPService.SharedLibs.PropertyGrid.Editors
{
    /// <summary>
    /// Редактор для свойств-имен папок
    /// </summary>
    public class BrowseForFolderEditor : CustomEditor
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
        /// Редактирование свойства
        /// </summary>
        /// <param name="value">Значение свойства</param>
        protected override Object OnEdit(Object value)
        {
            string selectedPath = (string)value;
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = Description;
                dialog.RootFolder = Root;
                dialog.ShowNewFolderButton = NewFolder;
                if (!string.IsNullOrEmpty(selectedPath))
                    dialog.SelectedPath = selectedPath;

                if (dialog.ShowDialog() == DialogResult.OK)
                    value = dialog.SelectedPath;
            }
            return value;
        }

        /// <summary>
        /// Заголовок диалога выбора папки
        /// </summary>
        protected virtual string Description
        {
            get { return "Выберите папку"; }
        }

        /// <summary>
        /// Разрешать создавать новые папки
        /// </summary>
        protected virtual bool NewFolder
        {
            get { return true; }
        }

        /// <summary>
        /// Корневая папка
        /// </summary>
        protected virtual Environment.SpecialFolder Root
        {
            get { return Environment.SpecialFolder.MyComputer; }
        }
    }
}
