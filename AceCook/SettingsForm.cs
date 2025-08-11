using System;
using System.Drawing;
using System.Windows.Forms;
using AceCook.Models;

namespace AceCook
{
    public partial class SettingsForm : Form
    {
        private Taikhoan _currentAccount;
        private AppDbContext _context;
        private TextBox txtOldPassword;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private Button btnChangePassword;
        private Button btnSaveSettings;
        private Label lblTitle;

        public SettingsForm(Taikhoan account, AppDbContext context)
        {
            _currentAccount = account;
            _context = context;
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Cài đặt Hệ thống";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);

            // Title
            lblTitle = new Label
            {
                Text = "CÀI ĐẶT HỆ THỐNG",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(560, 40),
                Location = new Point(20, 20)
            };

            // Change Password Panel
            var passwordPanel = new Panel
            {
                Size = new Size(560, 200),
                Location = new Point(20, 80),
                BackColor = Color.White
            };

            var lblPasswordTitle = new Label
            {
                Text = "Đổi mật khẩu",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(200, 30),
                Location = new Point(20, 20)
            };

            var lblOldPassword = new Label
            {
                Text = "Mật khẩu cũ:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(120, 25),
                Location = new Point(20, 60)
            };

            txtOldPassword = new TextBox
            {
                Size = new Size(300, 25),
                Location = new Point(150, 58),
                Font = new Font("Segoe UI", 10),
                UseSystemPasswordChar = true
            };

            var lblNewPassword = new Label
            {
                Text = "Mật khẩu mới:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(120, 25),
                Location = new Point(20, 95)
            };

            txtNewPassword = new TextBox
            {
                Size = new Size(300, 25),
                Location = new Point(150, 93),
                Font = new Font("Segoe UI", 10),
                UseSystemPasswordChar = true
            };

            var lblConfirmPassword = new Label
            {
                Text = "Xác nhận mật khẩu:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(120, 25),
                Location = new Point(20, 130)
            };

            txtConfirmPassword = new TextBox
            {
                Size = new Size(300, 25),
                Location = new Point(150, 128),
                Font = new Font("Segoe UI", 10),
                UseSystemPasswordChar = true
            };

            btnChangePassword = new Button
            {
                Text = "Đổi mật khẩu",
                Size = new Size(120, 35),
                Location = new Point(150, 160),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnChangePassword.FlatAppearance.BorderSize = 0;
            btnChangePassword.Click += BtnChangePassword_Click;

            passwordPanel.Controls.Add(lblPasswordTitle);
            passwordPanel.Controls.Add(lblOldPassword);
            passwordPanel.Controls.Add(txtOldPassword);
            passwordPanel.Controls.Add(lblNewPassword);
            passwordPanel.Controls.Add(txtNewPassword);
            passwordPanel.Controls.Add(lblConfirmPassword);
            passwordPanel.Controls.Add(txtConfirmPassword);
            passwordPanel.Controls.Add(btnChangePassword);

            // System Settings Panel
            var systemPanel = new Panel
            {
                Size = new Size(560, 150),
                Location = new Point(20, 300),
                BackColor = Color.White
            };

            var lblSystemTitle = new Label
            {
                Text = "Cài đặt hệ thống",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(200, 30),
                Location = new Point(20, 20)
            };

            var lblBackup = new Label
            {
                Text = "Sao lưu dữ liệu:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(120, 25),
                Location = new Point(20, 60)
            };

            var btnBackup = new Button
            {
                Text = "Sao lưu",
                Size = new Size(100, 30),
                Location = new Point(150, 58),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnBackup.FlatAppearance.BorderSize = 0;
            btnBackup.Click += BtnBackup_Click;

            var lblRestore = new Label
            {
                Text = "Khôi phục dữ liệu:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(120, 25),
                Location = new Point(20, 100)
            };

            var btnRestore = new Button
            {
                Text = "Khôi phục",
                Size = new Size(100, 30),
                Location = new Point(150, 98),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(241, 196, 15),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRestore.FlatAppearance.BorderSize = 0;
            btnRestore.Click += BtnRestore_Click;

            systemPanel.Controls.Add(lblSystemTitle);
            systemPanel.Controls.Add(lblBackup);
            systemPanel.Controls.Add(btnBackup);
            systemPanel.Controls.Add(lblRestore);
            systemPanel.Controls.Add(btnRestore);

            // Add controls to form
            this.Controls.Add(lblTitle);
            this.Controls.Add(passwordPanel);
            this.Controls.Add(systemPanel);

            // Add shadow effects
            passwordPanel.Paint += (sender, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, passwordPanel.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid);
            };

            systemPanel.Paint += (sender, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, systemPanel.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid);
            };
        }

        private async void BtnChangePassword_Click(object sender, EventArgs e)
        {
            if (ValidatePasswordChange())
            {
                try
                {
                    var authRepository = new Repositories.AuthRepository(_context);
                    bool success = await authRepository.ChangePasswordAsync(
                        _currentAccount.MaTk, 
                        txtOldPassword.Text, 
                        txtNewPassword.Text);

                    if (success)
                    {
                        MessageBox.Show("Đổi mật khẩu thành công!", "Thông báo", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearPasswordFields();
                    }
                    else
                    {
                        MessageBox.Show("Mật khẩu cũ không đúng!", "Lỗi", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi đổi mật khẩu: {ex.Message}", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidatePasswordChange()
        {
            if (string.IsNullOrWhiteSpace(txtOldPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu cũ!", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtOldPassword.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu mới!", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPassword.Focus();
                return false;
            }

            if (txtNewPassword.Text.Length < 6)
            {
                MessageBox.Show("Mật khẩu mới phải có ít nhất 6 ký tự!", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPassword.Focus();
                return false;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                return false;
            }

            return true;
        }

        private void ClearPasswordFields()
        {
            txtOldPassword.Clear();
            txtNewPassword.Clear();
            txtConfirmPassword.Clear();
        }

        private void BtnBackup_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng sao lưu dữ liệu sẽ được phát triển sau!", "Thông báo", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnRestore_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng khôi phục dữ liệu sẽ được phát triển sau!", "Thông báo", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
} 