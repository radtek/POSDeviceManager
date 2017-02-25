using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ERPService.SharedLibs.PropertyGrid.Forms
{
    /// <summary>
    /// Реализация модального редактора для текстовых блоков
    /// </summary>
    public partial class FormTextEditor : FormModalEditor
    {
        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        public FormTextEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Редактируемое свойство
        /// </summary>
        public override Object Value
        {
            get
            {
                return tbText.Lines;
            }
            set
            {
                tbText.Lines = (String[])value;
                tbText.SelectionStart = 0;
                WriteCursorLocation();
            }
        }

        /// <summary>
        /// Выводит строковое представление позиции курсора
        /// </summary>
        private void WriteCursorLocation()
        {
            // абсцисса и ордината
            Int32 X, Y;

            if (tbText.Text.Length == 0)
            {
                X = 1;
                Y = 1;
            }
            else
            {
                // абсцисса
                X = tbText.SelectionStart -
                    tbText.Text.Substring(0, tbText.SelectionStart).LastIndexOf("\n");

                // ордината
                Y = 1;
                Int32 nPos = tbText.Text.IndexOf("\n", 0); ;

                while ((nPos < tbText.SelectionStart) && (nPos >= 0))
                {
                    Y++;
                    nPos = tbText.Text.IndexOf("\n", nPos + 1);
                }
            }
            lblPosition.Text = String.Format("Позиция курсора: {0}/{1}", X, Y);
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            WriteCursorLocation();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            WriteCursorLocation();
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            WriteCursorLocation();
        }
    }
}