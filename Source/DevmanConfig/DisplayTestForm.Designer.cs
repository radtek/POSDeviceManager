namespace DevmanConfig
{
    partial class DisplayTestForm
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
            this.lbPosition = new System.Windows.Forms.Label();
            this.tbDisplayLines = new System.Windows.Forms.TextBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.cbSaveLines = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(257, 136);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Выход";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // lbPosition
            // 
            this.lbPosition.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbPosition.Location = new System.Drawing.Point(234, 105);
            this.lbPosition.Name = "lbPosition";
            this.lbPosition.Size = new System.Drawing.Size(98, 13);
            this.lbPosition.TabIndex = 1;
            this.lbPosition.Text = "Позиция курсора:";
            this.lbPosition.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbDisplayLines
            // 
            this.tbDisplayLines.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbDisplayLines.Location = new System.Drawing.Point(12, 12);
            this.tbDisplayLines.Multiline = true;
            this.tbDisplayLines.Name = "tbDisplayLines";
            this.tbDisplayLines.Size = new System.Drawing.Size(320, 86);
            this.tbDisplayLines.TabIndex = 0;
            this.tbDisplayLines.Text = "      Формула ИТ\r\n   (863) 242-62-05";
            this.tbDisplayLines.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbDisplayLines_KeyUp);
            this.tbDisplayLines.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tbDisplayLines_MouseClick);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(176, 136);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 2;
            this.btnTest.Text = "Тест";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.OnTest);
            // 
            // cbSaveLines
            // 
            this.cbSaveLines.AutoSize = true;
            this.cbSaveLines.Location = new System.Drawing.Point(12, 104);
            this.cbSaveLines.Name = "cbSaveLines";
            this.cbSaveLines.Size = new System.Drawing.Size(205, 17);
            this.cbSaveLines.TabIndex = 1;
            this.cbSaveLines.Text = "Записать во флэш-память дисплея";
            this.cbSaveLines.UseVisualStyleBackColor = true;
            // 
            // DisplayTestForm
            // 
            this.AcceptButton = this.btnTest;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(344, 171);
            this.ControlBox = false;
            this.Controls.Add(this.tbDisplayLines);
            this.Controls.Add(this.cbSaveLines);
            this.Controls.Add(this.lbPosition);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DisplayTestForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Тест дисплея покупателя";
            this.Shown += new System.EventHandler(this.DisplayTestForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lbPosition;
        private System.Windows.Forms.TextBox tbDisplayLines;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.CheckBox cbSaveLines;
    }
}