namespace Common
{
    partial class TestMailSenderForm1
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
            this.textBoxSym = new System.Windows.Forms.TextBox();
            this.textBoxDir = new System.Windows.Forms.TextBox();
            this.textBoxLast = new System.Windows.Forms.TextBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxSym
            // 
            this.textBoxSym.Location = new System.Drawing.Point(76, 40);
            this.textBoxSym.Name = "textBoxSym";
            this.textBoxSym.Size = new System.Drawing.Size(100, 20);
            this.textBoxSym.TabIndex = 0;
            this.textBoxSym.Text = "ADU11";
            // 
            // textBoxDir
            // 
            this.textBoxDir.Location = new System.Drawing.Point(77, 80);
            this.textBoxDir.Name = "textBoxDir";
            this.textBoxDir.Size = new System.Drawing.Size(100, 20);
            this.textBoxDir.TabIndex = 1;
            this.textBoxDir.Text = "Buy";
            // 
            // textBoxLast
            // 
            this.textBoxLast.Location = new System.Drawing.Point(79, 120);
            this.textBoxLast.Name = "textBoxLast";
            this.textBoxLast.Size = new System.Drawing.Size(100, 20);
            this.textBoxLast.TabIndex = 2;
            this.textBoxLast.Text = "1.059";
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(79, 166);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(75, 23);
            this.buttonSend.TabIndex = 3;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // TestMailSenderForm1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.textBoxLast);
            this.Controls.Add(this.textBoxDir);
            this.Controls.Add(this.textBoxSym);
            this.Name = "TestMailSenderForm1";
            this.Text = "TestMailSenderForm1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxSym;
        private System.Windows.Forms.TextBox textBoxDir;
        private System.Windows.Forms.TextBox textBoxLast;
        private System.Windows.Forms.Button buttonSend;
    }
}