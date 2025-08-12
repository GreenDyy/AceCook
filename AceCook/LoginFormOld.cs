using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AceCook.Models;
using AceCook.Repositories;

namespace AceCook
{
    public partial class LoginFormOld : Form
    {
        private AuthRepository _authRepository;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnExit;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblUsername;
        private Label lblPassword;
        private Panel panelLogin;
        private Panel panelHeader;
        private PictureBox pictureBoxLogo;
        private Panel panelUsername;
        private Panel panelPassword;

        public LoginFormOld()
        {
            //InitializeComponent();
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
            this.Size = new Size(800, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Paint += Form_Paint;

            // Header Panel with gradient
            panelHeader = new Panel
            {
                Size = new Size(450, 200),
                Location = new Point(0, 0),
                BackColor = Color.Transparent
            };
            panelHeader.Paint += PanelHeader_Paint;

            // Logo
            pictureBoxLogo = new PictureBox
            {
                Size = new Size(70, 70),
                Location = new Point(190, 40),
                BackColor = Color.Transparent,
                Image = null,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            // Title
            lblTitle = new Label
            {
                Text = "ACECOOK",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(450, 50),
                Location = new Point(0, 120)
            };

            // Subtitle
            lblSubtitle = new Label
            {
                Text = "Hệ thống quản lý bán hàng",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.FromArgb(255, 255, 255, 200),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(450, 25),
                Location = new Point(0, 170)
            };

            // Login Panel with shadow
            panelLogin = new Panel
            {
                Size = new Size(350, 380),
                Location = new Point(50, 230),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            panelLogin.Paint += PanelLogin_Paint;

            // Username Panel
            panelUsername = new Panel
            {
                Size = new Size(300, 60),
                Location = new Point(25, 40),
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.None
            };
            panelUsername.Paint += PanelInput_Paint;

            // Username Label
            lblUsername = new Label
            {
                Text = "Tên đăng nhập",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(108, 117, 125),
                Size = new Size(280, 20),
                Location = new Point(15, 8)
            };

            // Username TextBox
            txtUsername = new TextBox
            {
                Size = new Size(270, 30),
                Location = new Point(15, 25),
                Font = new Font("Segoe UI", 12),
                BorderStyle = BorderStyle.None,
                //BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(52, 73, 94)
            };
            txtUsername.Enter += TextBox_Enter;
            txtUsername.Leave += TextBox_Leave;

            // Password Panel
            panelPassword = new Panel
            {
                Size = new Size(300, 60),
                Location = new Point(25, 120),
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.None
            };
            panelPassword.Paint += PanelInput_Paint;

            // Password Label
            lblPassword = new Label
            {
                Text = "Mật khẩu",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(108, 117, 125),
                Size = new Size(280, 20),
                Location = new Point(15, 8)
            };

            // Password TextBox
            txtPassword = new TextBox
            {
                Size = new Size(270, 30),
                Location = new Point(15, 25),
                Font = new Font("Segoe UI", 12),
                BorderStyle = BorderStyle.None,
                ForeColor = Color.FromArgb(52, 73, 94),
                UseSystemPasswordChar = true
            };
            txtPassword.Enter += TextBox_Enter;
            txtPassword.Leave += TextBox_Leave;

            // Login Button
            btnLogin = new Button
            {
                Text = "ĐĂNG NHẬP",
                Size = new Size(300, 50),
                Location = new Point(25, 200),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Paint += Button_Paint;
            btnLogin.Click += BtnLogin_Click;

            // Exit Button
            btnExit = new Button
            {
                Text = "THOÁT",
                Size = new Size(300, 50),
                Location = new Point(25, 270),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Paint += Button_Paint;
            btnExit.Click += BtnExit_Click;

            // Close button
            var btnClose = new Button
            {
                Text = "×",
                Size = new Size(30, 30),
                Location = new Point(410, 10),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(108, 117, 125),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => Application.Exit();

            // Add controls to panels
            panelUsername.Controls.Add(lblUsername);
            panelUsername.Controls.Add(txtUsername);
            panelPassword.Controls.Add(lblPassword);
            panelPassword.Controls.Add(txtPassword);

            panelLogin.Controls.Add(panelUsername);
            panelLogin.Controls.Add(panelPassword);
            panelLogin.Controls.Add(btnLogin);
            panelLogin.Controls.Add(btnExit);

            // Add controls to form
            this.Controls.Add(panelHeader);
            this.Controls.Add(pictureBoxLogo);
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblSubtitle);
            this.Controls.Add(panelLogin);
            this.Controls.Add(btnClose);

            // Set default button
            this.AcceptButton = btnLogin;

            // Make form draggable
            this.MouseDown += Form_MouseDown;
            panelHeader.MouseDown += Form_MouseDown;
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            // Draw rounded corners for the form
            using (var path = new GraphicsPath())
            {
                path.AddArc(0, 0, 20, 20, 180, 90);
                path.AddArc(this.Width - 20, 0, 20, 20, 270, 90);
                path.AddArc(this.Width - 20, this.Height - 20, 20, 20, 0, 90);
                path.AddArc(0, this.Height - 20, 20, 20, 90, 90);
                path.CloseFigure();
                this.Region = new Region(path);
            }
        }

        private void PanelHeader_Paint(object sender, PaintEventArgs e)
        {
            // Draw gradient background for header
            using (var brush = new LinearGradientBrush(
                new Point(0, 0), new Point(0, 200),
                Color.FromArgb(52, 152, 219), Color.FromArgb(41, 128, 185)))
            {
                e.Graphics.FillRectangle(brush, panelHeader.ClientRectangle);
            }
        }

        private void PanelLogin_Paint(object sender, PaintEventArgs e)
        {
            // Draw shadow effect for login panel
            using (var brush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
            {
                e.Graphics.FillRoundedRectangle(brush, new Rectangle(0, 0, panelLogin.Width, panelLogin.Height), 15);
            }
        }

        private void PanelInput_Paint(object sender, PaintEventArgs e)
        {
            var panel = sender as Panel;
            if (panel != null)
            {
                // Draw rounded rectangle for input panels
                using (var brush = new SolidBrush(panel.BackColor))
                {
                    e.Graphics.FillRoundedRectangle(brush, panel.ClientRectangle, 8);
                }
            }
        }

        private void Button_Paint(object sender, PaintEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                // Draw rounded rectangle for buttons
                using (var brush = new SolidBrush(button.BackColor))
                {
                    e.Graphics.FillRoundedRectangle(brush, button.ClientRectangle, 8);
                }
            }
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            var panel = textBox.Parent as Panel;
            if (panel != null)
            {
                panel.BackColor = Color.FromArgb(255, 255, 255);
                panel.Invalidate();
            }
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            var panel = textBox.Parent as Panel;
            if (panel != null)
            {
                panel.BackColor = Color.FromArgb(248, 249, 250);
                panel.Invalidate();
            }
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(Handle, NativeMethods.WM_NCLBUTTONDOWN, NativeMethods.HT_CAPTION, 0);
            }
        }

        public int CalculateForm(int a, int b)
        {
            return a + b;
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

        //private void InitializeComponent()
        //{
        //    SuspendLayout();
        //    // 
        //    // LoginForm
        //    // 
        //    ClientSize = new Size(450, 650);
        //    Name = "LoginForm";
        //    ResumeLayout(false);
        //}

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            txtUsername.Focus();
        }
    }

    // Extension methods for rounded rectangles
    public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle rectangle, int cornerRadius)
        {
            using (var path = new GraphicsPath())
            {
                path.AddArc(rectangle.X, rectangle.Y, cornerRadius, cornerRadius, 180, 90);
                path.AddArc(rectangle.X + rectangle.Width - cornerRadius, rectangle.Y, cornerRadius, cornerRadius, 270, 90);
                path.AddArc(rectangle.X + rectangle.Width - cornerRadius, rectangle.Y + rectangle.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
                path.AddArc(rectangle.X, rectangle.Y + rectangle.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
                path.CloseFigure();
                graphics.FillPath(brush, path);
            }
        }
    }

    // Native methods for form dragging
    internal static class NativeMethods
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
    }
} 