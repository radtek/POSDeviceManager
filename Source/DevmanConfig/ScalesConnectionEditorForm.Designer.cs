namespace DevmanConfig
{
    partial class ScalesConnectionEditorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbProtocol = new System.Windows.Forms.ComboBox();
            this.tbHost = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.lbParam1 = new System.Windows.Forms.Label();
            this.lbParam2 = new System.Windows.Forms.Label();
            this.cbBaudRate = new System.Windows.Forms.ComboBox();
            this.cbComPort = new System.Windows.Forms.ComboBox();
            this.numTcpPort = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numTcpPort)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(213, 101);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Протокол:";
            // 
            // cbProtocol
            // 
            this.cbProtocol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProtocol.FormattingEnabled = true;
            this.cbProtocol.Items.AddRange(new object[] {
            "TCP",
            "UDP",
            "Последовательный порт"});
            this.cbProtocol.Location = new System.Drawing.Point(77, 12);
            this.cbProtocol.Name = "cbProtocol";
            this.cbProtocol.Size = new System.Drawing.Size(211, 21);
            this.cbProtocol.TabIndex = 1;
            this.cbProtocol.SelectedIndexChanged += new System.EventHandler(this.OnSelectProtocol);
            // 
            // tbHost
            // 
            this.tbHost.Location = new System.Drawing.Point(77, 39);
            this.tbHost.Name = "tbHost";
            this.tbHost.Size = new System.Drawing.Size(211, 20);
            this.tbHost.TabIndex = 4;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(132, 101);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "ОК";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // lbParam1
            // 
            this.lbParam1.AutoSize = true;
            this.lbParam1.Location = new System.Drawing.Point(12, 42);
            this.lbParam1.Name = "lbParam1";
            this.lbParam1.Size = new System.Drawing.Size(34, 13);
            this.lbParam1.TabIndex = 2;
            this.lbParam1.Text = "Хост:";
            // 
            // lbParam2
            // 
            this.lbParam2.AutoSize = true;
            this.lbParam2.Location = new System.Drawing.Point(12, 69);
            this.lbParam2.Name = "lbParam2";
            this.lbParam2.Size = new System.Drawing.Size(35, 13);
            this.lbParam2.TabIndex = 5;
            this.lbParam2.Text = "Порт:";
            // 
            // cbBaudRate
            // 
            this.cbBaudRate.FormattingEnabled = true;
            this.cbBaudRate.Items.AddRange(new object[] {
            "2400",
            "4800",
            "9600",
            "19200",
            "38400",
            "57600",
            "115200"});
            this.cbBaudRate.Location = new System.Drawing.Point(76, 66);
            this.cbBaudRate.Name = "cbBaudRate";
            this.cbBaudRate.Size = new System.Drawing.Size(83, 21);
            this.cbBaudRate.TabIndex = 3;
            this.cbBaudRate.Visible = false;
            // 
            // cbComPort
            // 
            this.cbComPort.FormattingEnabled = true;
            this.cbComPort.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "LPT1",
            "LPT2",
            "LPT3",
            "LPT4"});
            this.cbComPort.Location = new System.Drawing.Point(77, 39);
            this.cbComPort.Name = "cbComPort";
            this.cbComPort.Size = new System.Drawing.Size(82, 21);
            this.cbComPort.TabIndex = 3;
            this.cbComPort.Visible = false;
            // 
            // numTcpPort
            // 
            this.numTcpPort.Location = new System.Drawing.Point(76, 67);
            this.numTcpPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numTcpPort.Name = "numTcpPort";
            this.numTcpPort.Size = new System.Drawing.Size(83, 20);
            this.numTcpPort.TabIndex = 6;
            // 
            // ScalesConnectionEditorForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(300, 136);
            this.ControlBox = false;
            this.Controls.Add(this.cbComPort);
            this.Controls.Add(this.cbProtocol);
            this.Controls.Add(this.lbParam2);
            this.Controls.Add(this.lbParam1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.tbHost);
            this.Controls.Add(this.numTcpPort);
            this.Controls.Add(this.cbBaudRate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ScalesConnectionEditorForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Параметры соединения с весами";
            ((System.ComponentModel.ISupportInitialize)(this.numTcpPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbProtocol;
        private System.Windows.Forms.TextBox tbHost;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label lbParam1;
        private System.Windows.Forms.Label lbParam2;
        private System.Windows.Forms.ComboBox cbBaudRate;
        private System.Windows.Forms.ComboBox cbComPort;
        private System.Windows.Forms.NumericUpDown numTcpPort;
    }
}