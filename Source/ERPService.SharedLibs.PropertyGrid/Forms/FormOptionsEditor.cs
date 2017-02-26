using System;
using System.Collections.Generic;

namespace ERPService.SharedLibs.PropertyGrid.Forms
{
    /// <summary>
    /// Модальный редактор для опций
    /// </summary>
    /// <typeparam name="T">Тип, в котором хранятся опции (строка, перечисление и т.п.)</typeparam>
    public partial class FormOptionsEditor<T> : FormModalEditor
    {
        // интерфейс, реализующий набор опций
        private IOptionsProvider<T> _optionsProvider;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        public FormOptionsEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Редактируемое значение свойства
        /// </summary>
        public override Object Value
        {
            get
            {
                List<T> options = new List<T>();
                foreach (Object item in chlbOptions.CheckedItems)
                {
                    options.Add(((EditableOption<T>)item).Keyword);
                }
                return options.ToArray();
            }
            set
            {
                if (_optionsProvider == null)
                    throw new ArgumentNullException("_optionsProvider");

                chlbOptions.Items.Clear();
                foreach (EditableOption<T> option in _optionsProvider.Options)
                {
                    chlbOptions.Items.Add(option,
                        value == null ? false : Array.IndexOf<T>((T[])value, option.Keyword) != -1);
                }
            }
        }

        /// <summary>
        /// Интерфейс, реализующий набор опций
        /// </summary>
        public IOptionsProvider<T> OptionsProvider
        {
            get { return _optionsProvider; }
            set { _optionsProvider = value; }
        }
    }
}