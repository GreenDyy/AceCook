namespace AceCook
{
    partial class LoginForm
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
            txtUsername = new TextBox();
            txtPassword = new TextBox();
            label1 = new Label();
            label2 = new Label();
            btnLogin = new Button();
            checkBox1 = new CheckBox();
            label3 = new Label();
            SuspendLayout();
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(543, 392);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(431, 35);
            txtUsername.TabIndex = 0;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(543, 478);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(431, 35);
            txtPassword.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(467, 395);
            label1.Name = "label1";
            label1.Size = new Size(68, 30);
            label1.TabIndex = 2;
            label1.Text = "label1";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(467, 518);
            label2.Name = "label2";
            label2.Size = new Size(68, 30);
            label2.TabIndex = 3;
            label2.Text = "label2";
            // 
            // btnLogin
            // 
            btnLogin.Location = new Point(543, 596);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(131, 40);
            btnLogin.TabIndex = 4;
            btnLogin.Text = "Đăng nhập";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(732, 596);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(137, 34);
            checkBox1.TabIndex = 5;
            checkBox1.Text = "checkBox1";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(543, 230);
            label3.Name = "label3";
            label3.Size = new Size(68, 30);
            label3.TabIndex = 6;
            label3.Text = "label3";
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1843, 972);
            Controls.Add(label3);
            Controls.Add(checkBox1);
            Controls.Add(btnLogin);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtPassword);
            Controls.Add(txtUsername);
            Name = "LoginForm";
            Text = "LoginForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtUsername;
        private TextBox txtPassword;
        private Label label1;
        private Label label2;
        private Button btnLogin;
        private CheckBox checkBox1;
        private Label label3;
    }
}