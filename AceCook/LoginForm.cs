using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AceCook.Models;
using AceCook.Repositories;

namespace AceCook
{
    public partial class LoginForm : Form
    {
        private AuthRepository _authRepository;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnExit;
        private Label lblTitle;
        private Label lblUsername;
        private Label lblPassword;
        private Panel panelLogin;
        private PictureBox pictureBoxLogo;

        public LoginForm()
        {
            InitializeRepository();
            SetupUI();
        }

        private void InitializeRepository()
        {
            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                var connectionString = configuration.GetConnectionString("Default");
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseSqlServer(connectionString);
                var context = new AppDbContext(optionsBuilder.Options);
                _authRepository = new AuthRepository(context);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối database: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupUI()
        {
            // Form settings
            this.Text = "ACECOOK - Đăng nhập";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Logo
            pictureBoxLogo = new PictureBox
            {
                Size = new Size(80, 80),
                Location = new Point(160, 30),
                BackColor = Color.Transparent,
                Image = null, // Sẽ thêm icon sau
                SizeMode = PictureBoxSizeMode.Zoom
            };

            // Title
            lblTitle = new Label
            {
                Text = "ACECOOK SALES MANAGEMENT",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(350, 40),
                Location = new Point(25, 120)
            };

            // Login Panel
            panelLogin = new Panel
            {
                Size = new Size(320, 280),
                Location = new Point(40, 180),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            // Username Label
            lblUsername = new Label
            {
                Text = "Tên đăng nhập:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(100, 20),
                Location = new Point(20, 30)
            };

            // Username TextBox
            txtUsername = new TextBox
            {
                Size = new Size(280, 35),
                Location = new Point(20, 55),
                Font = new Font("Segoe UI", 12),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            // Password Label
            lblPassword = new Label
            {
                Text = "Mật khẩu:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(100, 20),
                Location = new Point(20, 100)
            };

            // Password TextBox
            txtPassword = new TextBox
            {
                Size = new Size(280, 35),
                Location = new Point(20, 125),
                Font = new Font("Segoe UI", 12),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 250),
                UseSystemPasswordChar = true
            };

            // Login Button
            btnLogin = new Button
            {
                Text = "ĐĂNG NHẬP",
                Size = new Size(280, 40),
                Location = new Point(20, 180),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            // Exit Button
            btnExit = new Button
            {
                Text = "THOÁT",
                Size = new Size(280, 40),
                Location = new Point(20, 230),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Click += BtnExit_Click;

            // Add controls to panel
            panelLogin.Controls.Add(lblUsername);
            panelLogin.Controls.Add(txtUsername);
            panelLogin.Controls.Add(lblPassword);
            panelLogin.Controls.Add(txtPassword);
            panelLogin.Controls.Add(btnLogin);
            panelLogin.Controls.Add(btnExit);

            // Add controls to form
            this.Controls.Add(pictureBoxLogo);
            this.Controls.Add(lblTitle);
            this.Controls.Add(panelLogin);

            // Set default button
            this.AcceptButton = btnLogin;

            // Add shadow effect to panel
            panelLogin.Paint += (sender, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, panelLogin.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid);
            };
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin đăng nhập!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnLogin.Enabled = false;
            btnLogin.Text = "ĐANG XỬ LÝ...";

            try
            {
                var result = await _authRepository.AuthenticateAsync(txtUsername.Text.Trim(), txtPassword.Text);
                
                if (result.success)
                {
                    MessageBox.Show($"Chào mừng {result.employee?.HoTenNv}!", "Đăng nhập thành công", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Mở Dashboard
                    var dashboardForm = new DashboardForm(result.account, result.employee, result.permission);
                    this.Hide();
                    dashboardForm.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!", "Đăng nhập thất bại", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi đăng nhập: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "ĐĂNG NHẬP";
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            txtUsername.Focus();
        }
    }
} 