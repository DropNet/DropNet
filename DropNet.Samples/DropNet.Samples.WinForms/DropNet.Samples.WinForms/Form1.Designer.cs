namespace DropNet.Samples.WinForms
{
    partial class Form1
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
            this.btn_LoginEmbed = new System.Windows.Forms.Button();
            this.brwLogin = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // btn_LoginEmbed
            // 
            this.btn_LoginEmbed.Location = new System.Drawing.Point(12, 12);
            this.btn_LoginEmbed.Name = "btn_LoginEmbed";
            this.btn_LoginEmbed.Size = new System.Drawing.Size(122, 61);
            this.btn_LoginEmbed.TabIndex = 0;
            this.btn_LoginEmbed.Text = "Login (Embedded Browser)";
            this.btn_LoginEmbed.UseVisualStyleBackColor = true;
            this.btn_LoginEmbed.Click += new System.EventHandler(this.btn_LoginEmbed_Click);
            // 
            // brwLogin
            // 
            this.brwLogin.Location = new System.Drawing.Point(12, 79);
            this.brwLogin.MinimumSize = new System.Drawing.Size(20, 20);
            this.brwLogin.Name = "brwLogin";
            this.brwLogin.Size = new System.Drawing.Size(351, 391);
            this.brwLogin.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(576, 482);
            this.Controls.Add(this.brwLogin);
            this.Controls.Add(this.btn_LoginEmbed);
            this.Name = "Form1";
            this.Text = "DropNet.Sample.WinForms";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_LoginEmbed;
        private System.Windows.Forms.WebBrowser brwLogin;
    }
}

