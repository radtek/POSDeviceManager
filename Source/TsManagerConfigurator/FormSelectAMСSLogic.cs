using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TsManagerConfigurator
{
    public partial class FormSelectAMÑSLogic : Form
    {
        public FormSelectAMÑSLogic()
        {
            InitializeComponent();
        }

        public Boolean Execute(String[] acmsNames)
        {
            if (acmsNames == null || acmsNames.Length == 0)
            {
                MessageBox.Show("Íåò äîñòóïíûõ ğåàëèçàöèé ëîãèêè ğàáîòû ÑÊÓÄ", Text, MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return false;
            }

            listBox1.BeginUpdate();
            try
            {
                listBox1.Items.Clear();
                foreach (String acmsName in acmsNames)
                {
                    listBox1.Items.Add(acmsName);
                }
                listBox1.SelectedIndex = 0;
                return ShowDialog() == DialogResult.OK;
            }
            finally
            {
                listBox1.EndUpdate();
            }
        }

        public String ACMSName
        {
            get 
            { 
                return listBox1.SelectedItem == null ? 
                    String.Empty : listBox1.SelectedItem.ToString(); 
            }
        }
    }
}