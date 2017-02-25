using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.Helpers
{
    /// <summary>
    /// Диалог "О программе"
    /// </summary>
    public class AboutBox
    {
        private FormAbout _formAbout;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        public AboutBox()
        {
            _formAbout = new FormAbout();
        }

        /// <summary>
        /// Добавляет информацию о компоненте
        /// </summary>
        /// <param name="componentName">Наименование компонента</param>
        /// <param name="componentVersion">Версия компонента</param>
        public virtual void AppendComponentInfo(String componentName, String componentVersion)
        {
            _formAbout.AppendComponentInfo(componentName, componentVersion);
        }

        /// <summary>
        /// Показывает диалог "О программе"
        /// </summary>
        /// <param name="showComponents">Отображать информацию о версии компонентов</param>
        public void Show(Boolean showComponents)
        {
            _formAbout.ShowDialog(showComponents);
        }

        /// <summary>
        /// Наименование программного продукта
        /// </summary>
        public String ProductName
        {
            get { return _formAbout.AppProductName; }
            set { _formAbout.AppProductName = value; }
        }

        /// <summary>
        /// Версия программного продукта
        /// </summary>
        public String Version
        {
            get { return _formAbout.AppProductVersion; }
            set { _formAbout.AppProductVersion = value; }
        }

        /// <summary>
        /// Год авторского права или модификации
        /// </summary>
        public Int32 CopyrightYear
        {
            get { return _formAbout.CopyrightYear; }
            set { _formAbout.CopyrightYear = value; }
        }

        /// <summary>
        /// Наименование приложения
        /// </summary>
        public String AppName
        {
            get { return _formAbout.AppName; }
            set { _formAbout.AppName = value; }
        }
    }
}
