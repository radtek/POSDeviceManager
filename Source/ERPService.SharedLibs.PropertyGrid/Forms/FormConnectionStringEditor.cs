using System;
using System.Text;

namespace ERPService.SharedLibs.PropertyGrid.Forms
{
    /// <summary>
    /// ������ ��� �������������� ����� ����������� � ���� ������
    /// </summary>
    public partial class FormConnectionStringEditor : FormModalEditor
    {
        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        public FormConnectionStringEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ������������� ��������
        /// </summary>
        public override Object Value
        {
            get
            {
                if (string.IsNullOrEmpty(tbServer.Text))
                    throw new InvalidOperationException("������� ��� ������� ��� ������");
                if (string.IsNullOrEmpty(tbDatabase.Text))
                    throw new InvalidOperationException("������� ��� ���� ������");
                if (!cbWindowsIdent.Checked && string.IsNullOrEmpty(tbUser.Text))
                    throw new InvalidOperationException("������� ��� ������������ ��� �������� ����� Windows-�������������");

                StringBuilder sb = new StringBuilder("Data Source=");
                sb.Append(tbServer.Text);
                if (cbSqlExpress.Checked)
                    sb.Append("\\SQLEXPRESS");
                sb.Append(";Initial Catalog=");
                sb.Append(tbDatabase.Text);

                if (cbWindowsIdent.Checked)
                    sb.Append(";Integrated Security=SSPI;");
                else
                {
                    sb.Append(";User Id=");
                    sb.Append(tbUser.Text);
                    sb.Append(";Password=");
                    sb.Append(tbPassword.Text);
                }
                return sb.ToString();
            }
            set
            {
                // ��������� ������ ����������� �� ���� ��������-��������
                string[] connStrParts = value.ToString().Split(new Char[] { ';' }, 
                    StringSplitOptions.RemoveEmptyEntries);

                foreach (string part in connStrParts)
                {
                    // ��������� ������� �� �������� � ��������
                    string[] partItems = part.Split(new Char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (partItems.Length == 2)
                    {
                        // �������������� ������ �������� � ��������� ����������
                        switch (partItems[0])
                        {
                            case "Data Source":
                                // ���������, �� �������� �� ������������ � ������������ ����������
                                string[] serverParts = partItems[1].Split(new Char[] { '\\' }, 
                                    StringSplitOptions.RemoveEmptyEntries);

                                switch (serverParts.Length)
                                {
                                    case 1:
                                        // ������� ��� �������
                                        tbServer.Text = partItems[1];
                                        break;
                                    case 2:
                                        // �� �������� �� ������������ � SQL Express
                                        cbSqlExpress.Checked = string.Compare(serverParts[1], "SQLEXPRESS") == 0;
                                        tbServer.Text = cbSqlExpress.Checked ? serverParts[0] : partItems[1];
                                        break;
                                    default:
                                        // ��� ������� ������ �����������
                                        tbServer.Text = string.Empty;
                                        break;
                                }
                                break;
                            case "Initial Catalog":
                                tbDatabase.Text = partItems[1];
                                break;
                            case "User Id":
                                tbUser.Text = partItems[1];
                                break;
                            case "Password":
                                tbPassword.Text = partItems[1];
                                break;
                            case "Integrated Security":
                                cbWindowsIdent.Checked = true;
                                break;
                        }
                    }
                }
            }
        }

        private void cbWindowsIdent_CheckedChanged(object sender, EventArgs e)
        {
            tbUser.Enabled = !cbWindowsIdent.Checked;
            tbPassword.Enabled = !cbWindowsIdent.Checked;
        }
    }
}