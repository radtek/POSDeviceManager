using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using ERPService.SharedLibs.Helpers.Properties;

namespace ERPService.SharedLibs.Helpers
{
    /// <summary>
    /// Вспомогательный класс для встраивания меню "Справка"
    /// </summary>
    public class HelpMenuBuilder : AboutBox
    {
        #region Поля

        private string _svnSubDirectory;
        private bool _showComponents;

        #endregion

        #region Закрытые свойства и методы

        /// <summary>
        /// Переход на домашнюю страницу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnHomeClick(Object sender, EventArgs args)
        {
            Process.Start("http://www.erpservice.ru");
        }

        /// <summary>
        /// Переход к обновлению ПО
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnUpdatesClick(Object sender, EventArgs args)
        {
            Process.Start(string.Format("http://www.erpservice.ru/svn/{0}", _svnSubDirectory));
        }

        /// <summary>
        /// Вызов диалога "О программе"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnAboutClick(Object sender, EventArgs args)
        {
            Show(_showComponents);
        }

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="form">Форма, в которую нужно встроить меню</param>
        /// <param name="svnSubDirectory">Подкаталог в системе контроля версий для программного продукта</param>
        public HelpMenuBuilder(Form form, string svnSubDirectory)
            : base()
        {
            if (form == null)
                throw new ArgumentNullException("form");
            if (string.IsNullOrEmpty(svnSubDirectory))
                throw new ArgumentNullException("svnSubDirectory");
            if (form.MainMenuStrip == null)
                throw new InvalidOperationException("Форма не содержит главного меню");

            // создаем меню
            ToolStripMenuItem help = new ToolStripMenuItem("Справка");
            help.DropDownItems.Add(new ToolStripMenuItem("Домашняя страница", Resources.home.ToBitmap(),
                OnHomeClick));
            help.DropDownItems.Add(new ToolStripMenuItem("Обновления", null, OnUpdatesClick));
            help.DropDownItems.Add(new ToolStripSeparator());
            help.DropDownItems.Add(new ToolStripMenuItem("О программе", Resources.help.ToBitmap(), 
                OnAboutClick));

            // встраиваем его в главное меню формы
            form.MainMenuStrip.Items.Add(help);

            _svnSubDirectory = svnSubDirectory;
        }

        #endregion

        #region Открытые свойства и методы

        /// <summary>
        /// Добавляет информацию о компоненте
        /// </summary>
        /// <param name="componentName">Наименование компонента</param>
        /// <param name="componentVersion">Версия компонента</param>
        public override void AppendComponentInfo(string componentName, string componentVersion)
        {
            _showComponents = true;
            base.AppendComponentInfo(componentName, componentVersion);
        }

        #endregion
    }
}
