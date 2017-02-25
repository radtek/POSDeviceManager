namespace ERPService.SharedLibs.PropertyGrid.Forms
{
    partial class FormTextEditor
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.tbText = new System.Windows.Forms.TextBox();
            this.lblPosition = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDescription
            // 
            this.lblDescription.Location = new System.Drawing.Point(12, 237);
            this.lblDescription.Size = new System.Drawing.Size(402, 33);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(258, 282);
            this.btnOk.TabIndex = 3;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(339, 282);
            this.btnCancel.TabIndex = 4;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.tbText);
            this.panel1.Controls.Add(this.lblPosition);
            this.panel1.Location = new System.Drawing.Point(12, 42);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(402, 182);
            this.panel1.TabIndex = 1;
            // 
            // tbText
            // 
            this.tbText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbText.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbText.Location = new System.Drawing.Point(0, 0);
            this.tbText.Multiline = true;
            this.tbText.Name = "tbText";
            this.tbText.Size = new System.Drawing.Size(402, 163);
            this.tbText.TabIndex = 0;
            this.tbText.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.tbText.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
            this.tbText.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // lblPosition
            // 
            this.lblPosition.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPosition.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblPosition.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblPosition.Location = new System.Drawing.Point(0, 163);
            this.lblPosition.Name = "lblPosition";
            this.lblPosition.Size = new System.Drawing.Size(402, 19);
            this.lblPosition.TabIndex = 1;
            this.lblPosition.Text = "lblPosition";
            this.lblPosition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(402, 30);
            this.label1.TabIndex = 0;
            this.label1.Text = "Для параметров, принимающих множество значений, каждое значение вводится в отдель" +
                "ной строке:";
            // 
            // FormTextEditor
            // 
            this.AcceptButton = null;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(426, 317);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Name = "FormTextEditor";
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.lblDescription, 0);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox tbText;
        private System.Windows.Forms.Label lblPosition;
        private System.Windows.Forms.Label label1;
    }
}