using System;
using System.Collections.Generic;
using System.Text;
using ERPService.SharedLibs.PropertyGrid.Forms;

namespace ERPService.SharedLibs.PropertyGrid
{
    /// <summary>
    /// ������� ����� ��� ���������� ������ �����
    /// </summary>
    /// <typeparam name="T">���, � ������� �������� ����� (������, ������������ � �.�.)</typeparam>
    public abstract class CustomOptionsEditor<T> : CustomModalEditor, IOptionsProvider<T>
    {
        /// <summary>
        /// ���������� ������ �� ��������� ���������� ���������
        /// </summary>
        protected override IModalEditor GetEditor()
        {
            FormOptionsEditor<T> optionsEditor = new FormOptionsEditor<T>();
            optionsEditor.OptionsProvider = this;
            return optionsEditor;
        }

        /// <summary>
        /// ���������� ����� ����� ��� ��������������
        /// </summary>
        public abstract EditableOption<T>[] Options { get; }
    }
}
