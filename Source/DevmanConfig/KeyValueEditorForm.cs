using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace DevmanConfig
{
    public partial class KeyValueEditorForm : Form
    {
        /// <summary>
        /// Коллекция пар "ключ-значение"
        /// </summary>
        public IDictionary<string, string> Collection
        {
            get
            {
                IDictionary<string, string> collection = new SerializableDictionary<string, string>();

                foreach (ListViewItem lvItem in lvCollection.Items)
                    collection.Add(lvItem.Text, lvItem.SubItems[1].Text);

                return collection;
            }
            set
            {
                UpdateView(value);
            }
        }

        private void UpdateView(IDictionary<string, string> collection)
        {
            lvCollection.BeginUpdate();
            try
            {
                lvCollection.Items.Clear();

                foreach (KeyValuePair<string, string> keyValuePair in collection)
                {
                    ListViewItem lvItem = lvCollection.Items.Add(keyValuePair.Key, keyValuePair.Key, 0);
                    lvItem.SubItems.Add(keyValuePair.Value);
                }
            }
            finally
            {
                lvCollection.EndUpdate();
            }
        }

        /// <summary>
        /// Создает экземплляр класса
        /// </summary>
        public KeyValueEditorForm()
        {
            InitializeComponent();
        }

        private void OnAddItem(object sender, EventArgs e)
        {
            int i = 1;
            string autoName;
            do
            {
                autoName = String.Format("Параметр {0}", i++);
            }
            while (lvCollection.Items.ContainsKey(autoName));

            ListViewItem lvItem = lvCollection.Items.Add(autoName, autoName, 0);
            lvItem.SubItems.Add("Значение");

            lvItem.Selected = true;
        }

        private void OnDeleteItem(object sender, EventArgs e)
        {
            if (lvCollection.SelectedItems.Count > 0)
            {
                int index = lvCollection.SelectedItems[0].Index;
                lvCollection.Items.Remove(lvCollection.SelectedItems[0]);
                if (index >= lvCollection.Items.Count)
                    index = lvCollection.Items.Count - 1;
                if (index >= 0)
                    lvCollection.Items[index].Selected = true;
            }
        }

        private void lvCollection_DoubleClick(object sender, EventArgs e)
        {
            if (lvCollection.SelectedItems.Count > 0)
            {
                ListViewItem lvItem = lvCollection.SelectedItems[0];
                string key = lvItem.Text;
                string value = lvItem.SubItems[1].Text;
                if (KeyValueItemForm.Edit(this, ref key, ref value))
                {
                    if (lvCollection.SelectedItems[0].Text != key && lvCollection.Items.ContainsKey(key))
                    {
                        MessageBox.Show(this, String.Format("Параметр \"{0}\" уже имеется в списке", key), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (lvCollection.SelectedItems[0].Text != key)
                    {
                        lvCollection.Items.Remove(lvItem);
                        lvItem = lvCollection.Items.Add(key, key, 0);
                        lvItem.SubItems.Add(value);
                    }
                    else
                        lvItem.SubItems[1].Text = value;
                }
            }
        }
    }
}