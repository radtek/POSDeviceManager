using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DevmanConfig
{
    public partial class KeyValueItemForm : Form
    {
        /// <summary>
        /// —оздает экземплл€р класса
        /// </summary>
        public KeyValueItemForm()
        {
            InitializeComponent();
        }

        internal static bool Edit(IWin32Window owner, ref string key, ref string value)
        {
            using (KeyValueItemForm dlgForm = new KeyValueItemForm())
            {
                dlgForm.Text = string.Format("{0}: {1}", key, value);
                dlgForm.tbKey.Text = key;
                dlgForm.tbValue.Text = value;
                if (dlgForm.ShowDialog(owner) == DialogResult.OK)
                {
                    key = dlgForm.tbKey.Text;
                    value = dlgForm.tbValue.Text;
                    return true;
                }
                return false;
            }
        }
    }
}