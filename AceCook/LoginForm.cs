using AceCook.Models;
using AceCook.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AceCook
{
    public partial class LoginForm : Form
    {
        private AuthRepository _authRepository;
        private bool _isLoading = false;

        public LoginForm()
        {
            InitializeComponent();
            InitializeRepository();
            SetupForm();
            txtUsername.Text = "kd1";
            txtPassword.Text = "123456";
        }

        private void SetupForm()
        {
            // Cấu hình form
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Text = "Đăng nhập - AceCook";

            // Cấu hình controls
            txtUsername.PlaceholderText = "Nhập tên đăng nhập";
            txtPassword.PlaceholderText = "Nhập mật khẩu";
            txtPassword.UseSystemPasswordChar = true;

            // Cập nhật labels
            label1.Text = "Tên đăng nhập:";
            label2.Text = "Mật khẩu:";
            label3.Text = "HỆ THỐNG QUẢN LÝ NHÀ HÀNG ACECOOK";
            label3.Font = new Font(label3.Font.FontFamily, 18, FontStyle.Bold);
            label3.ForeColor = Color.FromArgb(64, 64, 64);

            // Cập nhật checkbox
            checkBoxShowPass.Text = "Hiển thị mật khẩu";
            checkBoxShowPass.CheckedChanged += CheckBox1_CheckedChanged;

            // Cấu hình button
            btnLogin.Text = "ĐĂNG NHẬP";
            btnLogin.Font = new Font(btnLogin.Font.FontFamily, 12, FontStyle.Bold);
            btnLogin.BackColor = Color.FromArgb(0, 123, 255);
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Cursor = Cursors.Hand;

            // Thêm event handlers
            txtUsername.KeyPress += TxtUsername_KeyPress;
            txtPassword.KeyPress += TxtPassword_KeyPress;
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !checkBoxShowPass.Checked;
        }

        private void TxtUsername_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                txtPassword.Focus();
                e.Handled = true;
            }
        }

        private void TxtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnLogin.PerformClick();
                e.Handled = true;
            }
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

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (_isLoading) return;

            // Validation
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            // Đăng nhập
            await PerformLogin();
        }

        private async Task PerformLogin()
        {
            try
            {
                _isLoading = true;
                btnLogin.Enabled = false;
                btnLogin.Text = "ĐANG XỬ LÝ...";

                var (success, account, employee, permission) = await _authRepository.AuthenticateAsync(
                    txtUsername.Text.Trim(),
                    txtPassword.Text.Trim()
                );

                if (success && account != null)
                {

                    MessageBox.Show($"Đăng nhập thành công!\nChào mừng {employee?.HoTenNv ?? account.TenDangNhap}",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Mở Dashboard hoặc form chính
                    OpenMainForm(account, employee, permission);
                }
                else
                {
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!\nVui lòng kiểm tra lại.",
                        "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.SelectAll();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi đăng nhập: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isLoading = false;
                btnLogin.Enabled = true;
                btnLogin.Text = "ĐĂNG NHẬP";
            }
        }

        private void OpenMainForm(Taikhoan account, Nhanvien employee, Quyentruycap permission)
        {
            try
            {
                // Log thông tin trước khi tạo DashboardForm
                Console.WriteLine("=== MỞ DASHBOARD FORM ===");
                Console.WriteLine($"ACCOUNT INFO:");
                Console.WriteLine($"  - MaTk: {account?.MaTk ?? "NULL"}");
                Console.WriteLine($"  - TenDangNhap: {account?.TenDangNhap ?? "NULL"}");
                Console.WriteLine($"  - MaNv: {account?.MaNv ?? "NULL"}");
                Console.WriteLine($"  - MaPq: {account?.MaPq ?? "NULL"}");
                
                Console.WriteLine($"EMPLOYEE INFO:");
                Console.WriteLine($"  - MaNv: {employee?.MaNv ?? "NULL"}");
                Console.WriteLine($"  - HoTenNv: {employee?.HoTenNv ?? "NULL"}");
                Console.WriteLine($"  - MaPb: {employee?.MaPb ?? "NULL"}");
                
                Console.WriteLine($"PERMISSION INFO:");
                Console.WriteLine($"  - MaPq: {permission?.MaPq ?? "NULL"}");
                Console.WriteLine($"  - QuyenTruyCap: {permission?.QuyenTruyCap ?? "NULL"}");
                Console.WriteLine("==========================");

                // Tạo và hiển thị form chính (Dashboard)
                var dashboardForm = new DashboardForm(account, employee, permission);
                dashboardForm.Show();

                // Ẩn form đăng nhập
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi mở form chính: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkBoxShowPass_CheckedChanged(object sender, EventArgs e)
        {
           if( checkBoxShowPass.Checked)
            {
                txtPassword.UseSystemPasswordChar = false;
            }
            else
            {
                txtPassword.UseSystemPasswordChar = true;
            }
        }
    }
}
