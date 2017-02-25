using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;

namespace ERPService.SharedLibs.PropertyGrid
{
    /// <summary>
    /// ������� ����� ��� ���������� �������, ������������ ���������� ������
    /// </summary>
    public abstract class CustomDropdownEditor : CustomEditor
    {
        /// <summary>
        /// Gets the editor style used by the EditValue method
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that can be used to gain additional context information</param>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            // ������������� ����� ������������ � ���������� ����
            return UITypeEditorEditStyle.DropDown;
        }

        /// <summary>
        /// ���������� ���������� �������� ��������
        /// </summary>
        /// <param name="value">�������� ��������</param>
        protected override Object OnEdit(Object value)
        {
            // ������� ���������� ������ ��������
            ListBox valuesList = new ListBox();
            valuesList.BorderStyle = BorderStyle.None;
            valuesList.BeginUpdate();
            try
            {
                // ��������� ������ ����������
                valuesList.Items.AddRange(Values);
            }
            finally
            {
                valuesList.EndUpdate();
            }
            // ���������� ������ ������
            Int32 heightMultiplier = valuesList.Items.Count > 7 ? 7 : valuesList.Items.Count;
            valuesList.Height = valuesList.ItemHeight * (heightMultiplier + 1);
            // �������� ������ � ������ � ����������� �� �������� ��������
            valuesList.SelectedIndex = ObjectToIndex(value);
            // ��������� ��������� �������� �� ������ ����
            valuesList.Click += new EventHandler(valuesList_Click);
            // ��������� ������ ��������
            EdSvc.DropDownControl(valuesList);
            // ���������� ��������� �������� ��������
            return IndexToObject(valuesList.SelectedIndex);
        }

        /// <summary>
        /// �������� ������ �� �������� ������ ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void valuesList_Click(Object sender, EventArgs e)
        {
            EdSvc.CloseDropDown();
        }

        /// <summary>
        /// ���������� ������ ��������� �������� ��������
        /// </summary>
        abstract public String[] Values { get; }

        /// <summary>
        /// ���������� ������ ���������� ��������
        /// </summary>
        /// <param name="value">�������� �������� ��������</param>
        abstract protected Int32 ObjectToIndex(Object value);

        /// <summary>
        /// ������������ ��������� ��������� �������� � ������ ���
        /// </summary>
        /// <param name="selectedIndex">����� ������ � ������</param>
        abstract protected Object IndexToObject(Int32 selectedIndex);
    }
}
