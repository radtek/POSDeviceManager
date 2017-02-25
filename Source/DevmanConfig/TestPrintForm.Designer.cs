namespace DevmanConfig
{
    partial class TestPrintForm
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
            this.lbFileName = new System.Windows.Forms.Label();
            this.numAmount = new System.Windows.Forms.NumericUpDown();
            this.tbFileName = new System.Windows.Forms.TextBox();
            this.cbDocType = new System.Windows.Forms.ComboBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.lbAmount = new System.Windows.Forms.Label();
            this.lbDocType = new System.Windows.Forms.Label();
            this.lbPosCount = new System.Windows.Forms.Label();
            this.numPosCount = new System.Windows.Forms.NumericUpDown();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.cbDrawer = new System.Windows.Forms.ComboBox();
            this.lbDrawer = new System.Windows.Forms.Label();
            this.rbFromFile = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.numAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPosCount)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(291, 177);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Закрыть";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lbFileName
            // 
            this.lbFileName.AutoSize = true;
            this.lbFileName.Location = new System.Drawing.Point(35, 38);
            this.lbFileName.Name = "lbFileName";
            this.lbFileName.Size = new System.Drawing.Size(39, 13);
            this.lbFileName.TabIndex = 1;
            this.lbFileName.Text = "Файл:";
            // 
            // numAmount
            // 
            this.numAmount.DecimalPlaces = 2;
            this.numAmount.Location = new System.Drawing.Point(307, 138);
            this.numAmount.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numAmount.Name = "numAmount";
            this.numAmount.Size = new System.Drawing.Size(59, 20);
            this.numAmount.TabIndex = 12;
            this.numAmount.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // tbFileName
            // 
            this.tbFileName.Location = new System.Drawing.Point(80, 35);
            this.tbFileName.Name = "tbFileName";
            this.tbFileName.Size = new System.Drawing.Size(259, 20);
            this.tbFileName.TabIndex = 2;
            // 
            // cbDocType
            // 
            this.cbDocType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDocType.FormattingEnabled = true;
            this.cbDocType.Items.AddRange(new object[] {
            "Продажа",
            "Возврат",
            "Нефискальный",
            "Внесение",
            "Выплата",
            "X-отчет",
            "Z-отчет",
            "Отчет по секциям"});
            this.cbDocType.Location = new System.Drawing.Point(155, 84);
            this.cbDocType.Name = "cbDocType";
            this.cbDocType.Size = new System.Drawing.Size(211, 21);
            this.cbDocType.TabIndex = 6;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(210, 177);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 13;
            this.btnOk.Text = "Печать";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // lbAmount
            // 
            this.lbAmount.AutoSize = true;
            this.lbAmount.Location = new System.Drawing.Point(257, 140);
            this.lbAmount.Name = "lbAmount";
            this.lbAmount.Size = new System.Drawing.Size(44, 13);
            this.lbAmount.TabIndex = 11;
            this.lbAmount.Text = "Сумма:";
            // 
            // lbDocType
            // 
            this.lbDocType.AutoSize = true;
            this.lbDocType.Location = new System.Drawing.Point(35, 87);
            this.lbDocType.Name = "lbDocType";
            this.lbDocType.Size = new System.Drawing.Size(86, 13);
            this.lbDocType.TabIndex = 5;
            this.lbDocType.Text = "Тип документа:";
            // 
            // lbPosCount
            // 
            this.lbPosCount.AutoSize = true;
            this.lbPosCount.Location = new System.Drawing.Point(35, 140);
            this.lbPosCount.Name = "lbPosCount";
            this.lbPosCount.Size = new System.Drawing.Size(114, 13);
            this.lbPosCount.TabIndex = 9;
            this.lbPosCount.Text = "Количество позиций:";
            // 
            // numPosCount
            // 
            this.numPosCount.Location = new System.Drawing.Point(155, 138);
            this.numPosCount.Name = "numPosCount";
            this.numPosCount.Size = new System.Drawing.Size(59, 20);
            this.numPosCount.TabIndex = 10;
            this.numPosCount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(345, 33);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(24, 23);
            this.btnOpenFile.TabIndex = 3;
            this.btnOpenFile.Text = "...";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // cbDrawer
            // 
            this.cbDrawer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDrawer.FormattingEnabled = true;
            this.cbDrawer.Items.AddRange(new object[] {
            "Не открывать",
            "Открыть до начала печати",
            "Открыть по завершению печати"});
            this.cbDrawer.Location = new System.Drawing.Point(155, 111);
            this.cbDrawer.Name = "cbDrawer";
            this.cbDrawer.Size = new System.Drawing.Size(211, 21);
            this.cbDrawer.TabIndex = 8;
            // 
            // lbDrawer
            // 
            this.lbDrawer.AutoSize = true;
            this.lbDrawer.Location = new System.Drawing.Point(35, 114);
            this.lbDrawer.Name = "lbDrawer";
            this.lbDrawer.Size = new System.Drawing.Size(95, 13);
            this.lbDrawer.TabIndex = 7;
            this.lbDrawer.Text = "Денежный ящик:";
            // 
            // rbFromFile
            // 
            this.rbFromFile.AutoSize = true;
            this.rbFromFile.Checked = true;
            this.rbFromFile.Location = new System.Drawing.Point(12, 12);
            this.rbFromFile.Name = "rbFromFile";
            this.rbFromFile.Size = new System.Drawing.Size(92, 17);
            this.rbFromFile.TabIndex = 0;
            this.rbFromFile.TabStop = true;
            this.rbFromFile.Text = "Из xml-файла";
            this.rbFromFile.UseVisualStyleBackColor = true;
            this.rbFromFile.CheckedChanged += new System.EventHandler(this.rbFromFile_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(12, 61);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(103, 17);
            this.radioButton2.TabIndex = 4;
            this.radioButton2.Text = "Автоматически";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.rbFromFile_CheckedChanged);
            // 
            // TestPrintForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(378, 212);
            this.ControlBox = false;
            this.Controls.Add(this.btnOpenFile);
            this.Controls.Add(this.cbDrawer);
            this.Controls.Add(this.cbDocType);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.rbFromFile);
            this.Controls.Add(this.tbFileName);
            this.Controls.Add(this.numPosCount);
            this.Controls.Add(this.numAmount);
            this.Controls.Add(this.lbPosCount);
            this.Controls.Add(this.lbDrawer);
            this.Controls.Add(this.lbDocType);
            this.Controls.Add(this.lbAmount);
            this.Controls.Add(this.lbFileName);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TestPrintForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Тестовая печать";
            ((System.ComponentModel.ISupportInitialize)(this.numAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPosCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lbFileName;
        private System.Windows.Forms.NumericUpDown numAmount;
        private System.Windows.Forms.TextBox tbFileName;
        private System.Windows.Forms.ComboBox cbDocType;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label lbAmount;
        private System.Windows.Forms.Label lbDocType;
        private System.Windows.Forms.Label lbPosCount;
        private System.Windows.Forms.NumericUpDown numPosCount;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.ComboBox cbDrawer;
        private System.Windows.Forms.Label lbDrawer;
        private System.Windows.Forms.RadioButton rbFromFile;
        private System.Windows.Forms.RadioButton radioButton2;
    }
}