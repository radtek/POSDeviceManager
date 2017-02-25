using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace DevmanConfig
{
    internal partial class ScalesConnectionEditorForm : Form
    {
        private string Protocol
        {
            get 
            {
                // протокол связи
                switch (cbProtocol.SelectedIndex)
                {
                    case 0:
                        return "tcp";
                    case 1:
                        return "udp";
                    default:
                        return "rs";
                }
            }
            set 
            {
                // протокол связи
                switch (value)
                {
                    case "tcp":
                        cbProtocol.SelectedIndex = 0;
                        break;
                    case "udp":
                        cbProtocol.SelectedIndex = 1;
                        break;
                    default:
                        cbProtocol.SelectedIndex = 2;
                        break;
                }
            }
        }

        private string Param1
        {
            get 
            {
                if (cbProtocol.SelectedIndex < 2)
                    return tbHost.Text;
                else
                    return cbComPort.Text.ToUpper();
            }
            set 
            {
                if (cbProtocol.SelectedIndex < 2)
                    tbHost.Text = value;
                else
                    cbComPort.Text = value;
            }
        }

        private string Param2
        {
            get 
            {
                if (cbProtocol.SelectedIndex < 2)
                    return numTcpPort.Value.ToString();
                else
                    return cbBaudRate.Text;
            }
            set 
            {
                if (cbProtocol.SelectedIndex < 2)
                {
                    ushort portNo = 0;
                    UInt16.TryParse(value, out portNo);
                    numTcpPort.Value = portNo;
                }
                else
                    cbBaudRate.Text = value;
            }
        }

        public string ConnectionString
        {
            get { return String.Format("{0}://{1}:{2}", Protocol, Param1, Param2); }
            set
            {
                if (String.IsNullOrEmpty(value))
                    return;

                Regex connectionString = new Regex(@"(?<Protocol>\w+):\/\/(?<Param1>[\w.]+\/?):(?<Param2>\d+)",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase);

                Match matches = connectionString.Match(value);
                if (matches.Success)
                {
                    Protocol = matches.Groups["Protocol"].Value;
                    Param1 = matches.Groups["Param1"].Value;
                    Param2 = matches.Groups["Param2"].Value;
                }
            }
        }

        public ScalesConnectionEditorForm()
        {
            InitializeComponent();
            
            cbComPort.Items.Clear();
            cbComPort.Items.AddRange(SerialPortsEnumerator.Enumerate());
            cbComPort.Items.AddRange(SerialPortsEnumerator.EnumerateLPT());
        }

        private void OnSelectProtocol(object sender, EventArgs e)
        {
            lbParam1.Text = cbProtocol.SelectedIndex < 2 ? "Хост" : "Порт";
            lbParam2.Text = cbProtocol.SelectedIndex < 2 ? "Порт" : "Скорость";
            tbHost.Visible = cbProtocol.SelectedIndex < 2;
            numTcpPort.Visible = cbProtocol.SelectedIndex < 2;

            cbComPort.Visible = cbProtocol.SelectedIndex >= 2;
            cbBaudRate.Visible = cbProtocol.SelectedIndex >= 2;
        }
    }
}