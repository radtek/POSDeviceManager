namespace DevmanConfig
{
    partial class TestBilliardForm
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
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.numTable = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbTurnOff = new System.Windows.Forms.RadioButton();
            this.rbTurnOn = new System.Windows.Forms.RadioButton();
            this.btnTest = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numTable)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(193, 127);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Закрыть";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Номер стола:";
            // 
            // numTable
            // 
            this.numTable.Location = new System.Drawing.Point(181, 12);
            this.numTable.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTable.Name = "numTable";
            this.numTable.Size = new System.Drawing.Size(87, 20);
            this.numTable.TabIndex = 1;
            this.numTable.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbTurnOff);
            this.groupBox1.Controls.Add(this.rbTurnOn);
            this.groupBox1.Location = new System.Drawing.Point(12, 38);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(256, 75);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Освещение";
            // 
            // rbTurnOff
            // 
            this.rbTurnOff.AutoSize = true;
            this.rbTurnOff.Location = new System.Drawing.Point(6, 42);
            this.rbTurnOff.Name = "rbTurnOff";
            this.rbTurnOff.Size = new System.Drawing.Size(82, 17);
            this.rbTurnOff.TabIndex = 1;
            this.rbTurnOff.TabStop = true;
            this.rbTurnOff.Text = "Выключить";
            this.rbTurnOff.UseVisualStyleBackColor = true;
            // 
            // rbTurnOn
            // 
            this.rbTurnOn.AutoSize = true;
            this.rbTurnOn.Checked = true;
            this.rbTurnOn.Location = new System.Drawing.Point(6, 19);
            this.rbTurnOn.Name = "rbTurnOn";
            this.rbTurnOn.Size = new System.Drawing.Size(74, 17);
            this.rbTurnOn.TabIndex = 0;
            this.rbTurnOn.TabStop = true;
            this.rbTurnOn.Text = "Включить";
            this.rbTurnOn.UseVisualStyleBackColor = true;
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest.Location = new System.Drawing.Point(112, 127);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 3;
            this.btnTest.Text = "Тест";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // TestBilliardForm
            // 
            this.AcceptButton = this.btnTest;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(280, 162);
            this.ControlBox = false;
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.numTable);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TestBilliardForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Тест бильярда";
            ((System.ComponentModel.ISupportInitialize)(this.numTable)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numTable;
        private System.Windows.Forms.RadioButton rbTurnOff;
        private System.Windows.Forms.RadioButton rbTurnOn;
        private System.Windows.Forms.Button btnTest;
    }
}