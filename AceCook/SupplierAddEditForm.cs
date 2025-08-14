using System;
using System.Drawing;
using System.Windows.Forms;
using AceCook.Models;
using AceCook.Repositories;
using System.Linq; // Added for FirstOrDefault

namespace AceCook
{
    public partial class SupplierAddEditForm : Form
    {
        private readonly AppDbContext _context;
        private readonly SupplierRepository _supplierRepository;
        private readonly Nhacungcap _supplier;
        private readonly bool _isEditMode;
        private TextBox txtMaNcc;
        private TextBox txtTenNcc;
        private TextBox txtSoDienThoai;
        private TextBox txtEmail;
        private TextBox txtDiaChi;

        public Nhacungcap Supplier => _supplier;

        public SupplierAddEditForm(AppDbContext context, Nhacungcap supplier = null)
        {
            _context = context;
            _supplierRepository = new SupplierRepository(context);
            _supplier = supplier ?? new Nhacungcap();
            _isEditMode = supplier != null;
            InitializeComponent();
            SetupUI();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            if (_isEditMode)
            {
                this.Text = "Chỉnh sửa nhà cung cấp";
            }
            else
            {
                this.Text = "Thêm nhà cung cấp mới";
            }
            
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            this.ResumeLayout(false);
        }

        private void SetupUI()
        {
            // Title
            var lblTitle = new Label
            {
                Text = _isEditMode ? "CHỈNH SỬA NHÀ CUNG CẤP" : "THÊM NHÀ CUNG CẤP MỚI",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(400, 40),
                Location = new Point(30, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Form Panel
            var formPanel = new Panel
            {
                Size = new Size(540, 350),
                Location = new Point(30, 80),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Mã NCC
            var lblMaNcc = new Label
            {
                Text = "Mã nhà cung cấp:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(120, 25),
                Location = new Point(20, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtMaNcc = new TextBox
            {
                Name = "txtMaNcc",
                Size = new Size(350, 30),
                Location = new Point(150, 28),
                Font = new Font("Segoe UI", 10),
                Enabled = !_isEditMode
            };

            // Tên NCC
            var lblTenNcc = new Label
            {
                Text = "Tên nhà cung cấp:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(120, 25),
                Location = new Point(20, 80),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtTenNcc = new TextBox
            {
                Name = "txtTenNcc",
                Size = new Size(350, 30),
                Location = new Point(150, 78),
                Font = new Font("Segoe UI", 10)
            };

            // Số điện thoại
            var lblSoDienThoai = new Label
            {
                Text = "Số điện thoại:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(120, 25),
                Location = new Point(20, 130),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtSoDienThoai = new TextBox
            {
                Name = "txtSoDienThoai",
                Size = new Size(350, 30),
                Location = new Point(150, 128),
                Font = new Font("Segoe UI", 10)
            };

            // Email
            var lblEmail = new Label
            {
                Text = "Email:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(120, 25),
                Location = new Point(20, 180),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtEmail = new TextBox
            {
                Name = "txtEmail",
                Size = new Size(350, 30),
                Location = new Point(150, 178),
                Font = new Font("Segoe UI", 10)
            };

            // Địa chỉ
            var lblDiaChi = new Label
            {
                Text = "Địa chỉ:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(120, 25),
                Location = new Point(20, 230),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtDiaChi = new TextBox
            {
                Name = "txtDiaChi",
                Size = new Size(350, 60),
                Location = new Point(150, 228),
                Font = new Font("Segoe UI", 10),
                Multiline = true
            };

            // Buttons
            var btnSave = new Button
            {
                Text = "Lưu",
                Size = new Size(120, 40),
                Location = new Point(250, 290),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            btnSave.Click += BtnSave_Click;

            var btnCancel = new Button
            {
                Text = "Hủy",
                Size = new Size(120, 40),
                Location = new Point(380, 290),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };

            // Add controls to form panel
            formPanel.Controls.AddRange(new Control[] {
                lblMaNcc, txtMaNcc,
                lblTenNcc, txtTenNcc,
                lblSoDienThoai, txtSoDienThoai,
                lblEmail, txtEmail,
                lblDiaChi, txtDiaChi,
                btnSave, btnCancel
            });

            // Add controls to form
            this.Controls.AddRange(new Control[] { lblTitle, formPanel });
        }

        private void LoadData()
        {
            if (_isEditMode)
            {
                txtMaNcc.Text = _supplier.MaNcc;
                txtTenNcc.Text = _supplier.TenNcc;
                txtSoDienThoai.Text = _supplier.Sdtncc;  // Changed from SoDienThoai to Sdtncc
                txtEmail.Text = _supplier.EmailNcc;       // Changed from Email to EmailNcc
                txtDiaChi.Text = _supplier.DiaChiNcc;     // Changed from DiaChi to DiaChiNcc
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                _supplier.MaNcc = txtMaNcc.Text.Trim();
                _supplier.TenNcc = txtTenNcc.Text.Trim();
                _supplier.Sdtncc = txtSoDienThoai.Text.Trim();
                _supplier.EmailNcc = txtEmail.Text.Trim();
                _supplier.DiaChiNcc = txtDiaChi.Text.Trim();

                // Chỉ cần set DialogResult và đóng form
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtMaNcc.Text))
            {
                MessageBox.Show("Vui lòng nhập mã nhà cung cấp", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtTenNcc.Text))
            {
                MessageBox.Show("Vui lòng nhập tên nhà cung cấp", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtSoDienThoai.Text))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
    }
}
