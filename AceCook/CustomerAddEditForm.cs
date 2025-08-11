using System;
using System.Drawing;
using System.Windows.Forms;
using AceCook.Models;

namespace AceCook
{
    public partial class CustomerAddEditForm : Form
    {
        private Khachhang _customer;
        private TextBox txtMaKH;
        private TextBox txtTenKH;
        private TextBox txtDiaChi;
        private TextBox txtSDT;
        private TextBox txtEmail;
        private Button btnSave;
        private Button btnCancel;
        private Label lblTitle;

        public Khachhang Customer => _customer;

        public CustomerAddEditForm()
        {
            _customer = new Khachhang();
            SetupUI();
        }

        public CustomerAddEditForm(Khachhang customer)
        {
            _customer = customer;
            SetupUI();
            LoadCustomerData();
        }

        private void SetupUI()
        {
            this.Text = _customer.MaKh == null ? "Thêm Khách Hàng Mới" : "Chỉnh Sửa Khách Hàng";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(248, 249, 250);

            // Title
            lblTitle = new Label
            {
                Text = _customer.MaKh == null ? "THÊM KHÁCH HÀNG MỚI" : "CHỈNH SỬA KHÁCH HÀNG",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(460, 30),
                Location = new Point(20, 20)
            };

            // Form Panel
            var formPanel = new Panel
            {
                Size = new Size(460, 280),
                Location = new Point(20, 60),
                BackColor = Color.White
            };

            // Mã KH
            var lblMaKH = new Label
            {
                Text = "Mã Khách Hàng:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(120, 25),
                Location = new Point(20, 30)
            };

            txtMaKH = new TextBox
            {
                Size = new Size(300, 25),
                Location = new Point(140, 28),
                Font = new Font("Segoe UI", 10),
                Enabled = _customer.MaKh == null // Only enable for new customer
            };

            // Tên KH
            var lblTenKH = new Label
            {
                Text = "Tên Khách Hàng:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(120, 25),
                Location = new Point(20, 70)
            };

            txtTenKH = new TextBox
            {
                Size = new Size(300, 25),
                Location = new Point(140, 68),
                Font = new Font("Segoe UI", 10)
            };

            // Địa chỉ
            var lblDiaChi = new Label
            {
                Text = "Địa Chỉ:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(120, 25),
                Location = new Point(20, 110)
            };

            txtDiaChi = new TextBox
            {
                Size = new Size(300, 25),
                Location = new Point(140, 108),
                Font = new Font("Segoe UI", 10)
            };

            // Số điện thoại
            var lblSDT = new Label
            {
                Text = "Số Điện Thoại:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(120, 25),
                Location = new Point(20, 150)
            };

            txtSDT = new TextBox
            {
                Size = new Size(300, 25),
                Location = new Point(140, 148),
                Font = new Font("Segoe UI", 10)
            };

            // Email
            var lblEmail = new Label
            {
                Text = "Email:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(120, 25),
                Location = new Point(20, 190)
            };

            txtEmail = new TextBox
            {
                Size = new Size(300, 25),
                Location = new Point(140, 188),
                Font = new Font("Segoe UI", 10)
            };

            // Buttons
            btnSave = new Button
            {
                Text = "Lưu",
                Size = new Size(100, 35),
                Location = new Point(140, 230),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Hủy",
                Size = new Size(100, 35),
                Location = new Point(260, 230),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += BtnCancel_Click;

            // Add controls to form panel
            formPanel.Controls.Add(lblMaKH);
            formPanel.Controls.Add(txtMaKH);
            formPanel.Controls.Add(lblTenKH);
            formPanel.Controls.Add(txtTenKH);
            formPanel.Controls.Add(lblDiaChi);
            formPanel.Controls.Add(txtDiaChi);
            formPanel.Controls.Add(lblSDT);
            formPanel.Controls.Add(txtSDT);
            formPanel.Controls.Add(lblEmail);
            formPanel.Controls.Add(txtEmail);
            formPanel.Controls.Add(btnSave);
            formPanel.Controls.Add(btnCancel);

            // Add controls to form
            this.Controls.Add(lblTitle);
            this.Controls.Add(formPanel);

            // Set default button
            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;

            // Add shadow effect to form panel
            formPanel.Paint += (sender, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, formPanel.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid);
            };
        }

        private void LoadCustomerData()
        {
            txtMaKH.Text = _customer.MaKh;
            txtTenKH.Text = _customer.TenKh;
            txtDiaChi.Text = _customer.DiaChiKh;
            txtSDT.Text = _customer.Sdtkh;
            txtEmail.Text = _customer.EmailKh;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                _customer.MaKh = txtMaKH.Text.Trim();
                _customer.TenKh = txtTenKH.Text.Trim();
                _customer.DiaChiKh = txtDiaChi.Text.Trim();
                _customer.Sdtkh = txtSDT.Text.Trim();
                _customer.EmailKh = txtEmail.Text.Trim();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtMaKH.Text))
            {
                MessageBox.Show("Vui lòng nhập mã khách hàng!", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaKH.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtTenKH.Text))
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng!", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenKH.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtSDT.Text))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại!", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSDT.Focus();
                return false;
            }

            // Validate phone number format
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtSDT.Text, @"^\d{10,11}$"))
            {
                MessageBox.Show("Số điện thoại không hợp lệ! Vui lòng nhập 10-11 chữ số.", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSDT.Focus();
                return false;
            }

            // Validate email format if provided
            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(txtEmail.Text, 
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    MessageBox.Show("Email không hợp lệ!", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    return false;
                }
            }

            return true;
        }
    }
} 