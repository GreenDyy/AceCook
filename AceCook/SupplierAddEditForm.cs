using System;
using System.Drawing;
using System.Windows.Forms;
using AceCook.Models;

namespace AceCook
{
    public partial class SupplierAddEditForm : Form
    {
        private readonly Nhacungcap _supplier;
        private readonly bool _isEditMode;

        public Nhacungcap Supplier => _supplier;

        public SupplierAddEditForm(Nhacungcap supplier = null)
        {
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
                Text = "Mã NCC:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(100, 25),
                Location = new Point(20, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var txtMaNcc = new TextBox
            {
                Name = "txtMaNcc",
                Size = new Size(150, 30),
                Location = new Point(130, 28),
                Font = new Font("Segoe UI", 10),
                Text = _supplier.MaNcc ?? "",
                Enabled = !_isEditMode // Không cho phép sửa mã khi edit
            };

            // Tên NCC
            var lblTenNcc = new Label
            {
                Text = "Tên NCC:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(100, 25),
                Location = new Point(20, 80),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var txtTenNcc = new TextBox
            {
                Name = "txtTenNcc",
                Size = new Size(300, 30),
                Location = new Point(130, 78),
                Font = new Font("Segoe UI", 10),
                Text = _supplier.TenNcc ?? ""
            };

            // SĐT NCC
            var lblSdtncc = new Label
            {
                Text = "Số điện thoại:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(100, 25),
                Location = new Point(20, 130),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var txtSdtncc = new TextBox
            {
                Name = "txtSdtncc",
                Size = new Size(200, 30),
                Location = new Point(130, 128),
                Font = new Font("Segoe UI", 10),
                Text = _supplier.Sdtncc ?? ""
            };

            // Email NCC
            var lblEmailNcc = new Label
            {
                Text = "Email:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(100, 25),
                Location = new Point(20, 180),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var txtEmailNcc = new TextBox
            {
                Name = "txtEmailNcc",
                Size = new Size(300, 30),
                Location = new Point(130, 178),
                Font = new Font("Segoe UI", 10),
                Text = _supplier.EmailNcc ?? ""
            };

            // Địa chỉ NCC
            var lblDiaChiNcc = new Label
            {
                Text = "Địa chỉ:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(100, 25),
                Location = new Point(20, 230),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var txtDiaChiNcc = new TextBox
            {
                Name = "txtDiaChiNcc",
                Size = new Size(300, 30),
                Location = new Point(130, 228),
                Font = new Font("Segoe UI", 10),
                Text = _supplier.DiaChiNcc ?? ""
            };

            // Buttons
            var btnSave = new Button
            {
                Name = "btnSave",
                Text = _isEditMode ? "💾 Cập nhật" : "💾 Lưu",
                Size = new Size(120, 40),
                Location = new Point(200, 280),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            var btnCancel = new Button
            {
                Text = "❌ Hủy",
                Size = new Size(120, 40),
                Location = new Point(340, 280),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += BtnCancel_Click;

            // Add controls to form panel
            formPanel.Controls.AddRange(new Control[] { 
                lblMaNcc, txtMaNcc, lblTenNcc, txtTenNcc,
                lblSdtncc, txtSdtncc, lblEmailNcc, txtEmailNcc,
                lblDiaChiNcc, txtDiaChiNcc, btnSave, btnCancel
            });

            // Add controls to form
            this.Controls.AddRange(new Control[] { lblTitle, formPanel });
        }

        private void LoadData()
        {
            if (_isEditMode)
            {
                // Data is already loaded in constructor
                return;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            try
            {
                // Update supplier object
                _supplier.MaNcc = GetControl<TextBox>("txtMaNcc").Text.Trim();
                _supplier.TenNcc = GetControl<TextBox>("txtTenNcc").Text.Trim();
                _supplier.Sdtncc = GetControl<TextBox>("txtSdtncc").Text.Trim();
                _supplier.EmailNcc = GetControl<TextBox>("txtEmailNcc").Text.Trim();
                _supplier.DiaChiNcc = GetControl<TextBox>("txtDiaChiNcc").Text.Trim();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool ValidateForm()
        {
            var txtMaNcc = GetControl<TextBox>("txtMaNcc");
            var txtTenNcc = GetControl<TextBox>("txtTenNcc");
            var txtSdtncc = GetControl<TextBox>("txtSdtncc");

            // Validate required fields
            if (string.IsNullOrWhiteSpace(txtMaNcc.Text))
            {
                MessageBox.Show("Vui lòng nhập mã nhà cung cấp!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaNcc.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtTenNcc.Text))
            {
                MessageBox.Show("Vui lòng nhập tên nhà cung cấp!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenNcc.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtSdtncc.Text))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSdtncc.Focus();
                return false;
            }

            // Validate phone number format (basic validation)
            if (txtSdtncc.Text.Length < 10 || txtSdtncc.Text.Length > 11)
            {
                MessageBox.Show("Số điện thoại phải có 10-11 chữ số!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSdtncc.Focus();
                return false;
            }

            // Validate email format if provided
            var txtEmailNcc = GetControl<TextBox>("txtEmailNcc");
            if (!string.IsNullOrWhiteSpace(txtEmailNcc.Text))
            {
                try
                {
                    var email = new System.Net.Mail.MailAddress(txtEmailNcc.Text);
                }
                catch
                {
                    MessageBox.Show("Email không đúng định dạng!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmailNcc.Focus();
                    return false;
                }
            }

            return true;
        }

        private T GetControl<T>(string name) where T : Control
        {
            foreach (Control control in this.Controls)
            {
                if (control is Panel panel)
                {
                    foreach (Control c in panel.Controls)
                    {
                        if (c.Name == name && c is T result)
                            return result;
                    }
                }
            }
            throw new InvalidOperationException($"Control '{name}' not found");
        }
    }
}
