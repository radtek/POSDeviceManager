using System;
using System.Collections.Generic;

namespace ERPService.SharedLibs.PropertyGrid.Forms
{
    /// <summary>
    /// ��������� �������� ��� �����
    /// </summary>
    /// <typeparam name="T">���, � ������� �������� ����� (������, ������������ � �.�.)</typeparam>
    public partial class FormOptionsEditor<T> : FormModalEditor
    {
        // ���������, ����������� ����� �����
        private IOptionsProvider<T> _optionsProvider;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        public FormOptionsEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ������������� �������� ��������
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
        /// ���������, ����������� ����� �����
        /// </summary>
        public IOptionsProvider<T> OptionsProvider
        {
            get { return _optionsProvider; }
            set { _optionsProvider = value; }
        }
    }
}