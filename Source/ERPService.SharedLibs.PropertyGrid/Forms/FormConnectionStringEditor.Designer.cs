namespace ERPService.SharedLibs.PropertyGrid.Forms
{
    partial class FormConnectionStringEditor
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbServer = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbDatabase = new System.Windows.Forms.TextBox();
            this.cbWindowsIdent = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbUser = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbSqlExpress = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblDescription
            // 
            this.lblDescription.Location = new System.Drawing.Point(12, 220);
            this.lblDescription.TabIndex = 10;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(217, 270);
            this.btnOk.TabIndex = 11;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(298, 270);
            this.btnCancel.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Сервер:";
            // 
            // tbServer
            // 
            this.tbServer.Location = new System.Drawing.Point(98, 13);
            this.tbServer.Name = "tbServer";
            this.tbServer.Size = new System.Drawing.Size(275, 20);
            this.tbServer.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "База данных:";
            // 
            // tbDatabase
            // 
            this.tbDatabase.Location = new System.Drawing.Point(98, 46);
            this.tbDatabase.Name = "tbDatabase";
            this.tbDatabase.Size = new System.Drawing.Size(275, 20);
            this.tbDatabase.TabIndex = 3;
            // 
            // cbWindowsIdent
            // 
            this.cbWindowsIdent.AutoSize = true;
            this.cbWindowsIdent.Location = new System.Drawing.Point(12, 113);
            this.cbWindowsIdent.Name = "cbWindowsIdent";
            this.cbWindowsIdent.Size = new System.Drawing.Size(152, 17);
            this.cbWindowsIdent.TabIndex = 5;
            this.cbWindowsIdent.Text = "Windows-идентификация";
            this.cbWindowsIdent.UseVisualStyleBackColor = true;
            this.cbWindowsIdent.CheckedChanged += new System.EventHandler(this.cbWindowsIdent_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 149);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Пользователь:";
            // 
            // tbUser
            // 
            this.tbUser.Location = new System.Drawing.Point(98, 146);
            this.tbUser.Name = "tbUser";
            this.tbUser.Size = new System.Drawing.Size(275, 20);
            this.tbUser.TabIndex = 7;
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(98, 181);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(275, 20);
            this.tbPassword.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 184);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Пароль:";
            // 
            // cbSqlExpress
            // 
            this.cbSqlExpress.AutoSize = true;
            this.cbSqlExpress.Location = new System.Drawing.Point(12, 81);
            this.cbSqlExpress.Name = "cbSqlExpress";
            this.cbSqlExpress.Size = new System.Drawing.Size(148, 17);
            this.cbSqlExpress.TabIndex = 4;
            this.cbSqlExpress.Text = "SQL Server 2005 Express";
            this.cbSqlExpress.UseVisualStyleBackColor = true;
            // 
            // FormConnectionStringEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(385, 305);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbDatabase);
            this.Controls.Add(this.tbServer);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbSqlExpress);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.cbWindowsIdent);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbUser);
            this.Name = "FormConnectionStringEditor";
            this.Text = "FormConnectionStringEditor";
            this.Controls.SetChildIndex(this.tbUser, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.cbWindowsIdent, 0);
            this.Controls.SetChildIndex(this.tbPassword, 0);
            this.Controls.SetChildIndex(this.cbSqlExpress, 0);
            this.Controls.SetChildIndex(this.label4, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.tbServer, 0);
            this.Controls.SetChildIndex(this.tbDatabase, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.lblDescription, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbServer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbDatabase;
        private System.Windows.Forms.CheckBox cbWindowsIdent;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbUser;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cbSqlExpress;

    }
}