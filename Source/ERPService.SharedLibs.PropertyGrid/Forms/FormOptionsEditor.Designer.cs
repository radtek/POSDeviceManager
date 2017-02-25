namespace ERPService.SharedLibs.PropertyGrid.Forms
{
    partial class FormOptionsEditor<T>
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
            this.chlbOptions = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // chlbOptions
            // 
            this.chlbOptions.FormattingEnabled = true;
            this.chlbOptions.Location = new System.Drawing.Point(12, 12);
            this.chlbOptions.Name = "chlbOptions";
            this.chlbOptions.Size = new System.Drawing.Size(361, 154);
            this.chlbOptions.TabIndex = 3;
            // 
            // FormOptionsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(385, 258);
            this.Controls.Add(this.chlbOptions);
            this.Name = "FormOptionsEditor";
            this.Controls.SetChildIndex(this.chlbOptions, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.lblDescription, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox chlbOptions;
    }
}