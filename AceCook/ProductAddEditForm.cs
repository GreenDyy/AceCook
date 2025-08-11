using System;
using System.Drawing;
using System.Windows.Forms;
using AceCook.Models;

namespace AceCook
{
    public partial class ProductAddEditForm : Form
    {
        private Sanpham _product;
        private TextBox txtMaSP;
        private TextBox txtTenSP;
        private TextBox txtMoTa;
        private TextBox txtGia;
        private TextBox txtDVT;
        private ComboBox cboLoai;
        private Button btnSave;
        private Button btnCancel;
        private Label lblTitle;

        public Sanpham Product => _product;

        public ProductAddEditForm()
        {
            _product = new Sanpham();
            SetupUI();
        }

        public ProductAddEditForm(Sanpham product)
        {
            _product = product;
            SetupUI();
            LoadProductData();
        }

        private void SetupUI()
        {
            this.Text = _product.MaSp == null ? "Thêm Sản Phẩm Mới" : "Chỉnh Sửa Sản Phẩm";
            this.Size = new Size(500, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(248, 249, 250);

            // Title
            lblTitle = new Label
            {
                Text = _product.MaSp == null ? "THÊM SẢN PHẨM MỚI" : "CHỈNH SỬA SẢN PHẨM",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(460, 30),
                Location = new Point(20, 20)
            };

            // Form Panel
            var formPanel = new Panel
            {
                Size = new Size(460, 330),
                Location = new Point(20, 60),
                BackColor = Color.White
            };

            // Mã SP
            var lblMaSP = new Label
            {
                Text = "Mã Sản Phẩm:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(120, 25),
                Location = new Point(20, 30)
            };

            txtMaSP = new TextBox
            {
                Size = new Size(300, 25),
                Location = new Point(140, 28),
                Font = new Font("Segoe UI", 10),
                Enabled = _product.MaSp == null // Only enable for new product
            };

            // Tên SP
            var lblTenSP = new Label
            {
                Text = "Tên Sản Phẩm:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(120, 25),
                Location = new Point(20, 70)
            };

            txtTenSP = new TextBox
            {
                Size = new Size(300, 25),
                Location = new Point(140, 68),
                Font = new Font("Segoe UI", 10)
            };

            // Mô tả
            var lblMoTa = new Label
            {
                Text = "Mô Tả:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(120, 25),
                Location = new Point(20, 110)
            };

            txtMoTa = new TextBox
            {
                Size = new Size(300, 25),
                Location = new Point(140, 108),
                Font = new Font("Segoe UI", 10)
            };

            // Giá
            var lblGia = new Label
            {
                Text = "Giá:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(120, 25),
                Location = new Point(20, 150)
            };

            txtGia = new TextBox
            {
                Size = new Size(300, 25),
                Location = new Point(140, 148),
                Font = new Font("Segoe UI", 10)
            };

            // Đơn vị
            var lblDVT = new Label
            {
                Text = "Đơn Vị:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(120, 25),
                Location = new Point(20, 190)
            };

            txtDVT = new TextBox
            {
                Size = new Size(300, 25),
                Location = new Point(140, 188),
                Font = new Font("Segoe UI", 10)
            };

            // Loại
            var lblLoai = new Label
            {
                Text = "Loại:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(120, 25),
                Location = new Point(20, 230)
            };

            cboLoai = new ComboBox
            {
                Size = new Size(300, 25),
                Location = new Point(140, 228),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Load categories
            cboLoai.Items.AddRange(new string[] { "Mì ăn liền", "Gia vị", "Nước chấm", "Đồ khô", "Khác" });

            // Buttons
            btnSave = new Button
            {
                Text = "Lưu",
                Size = new Size(100, 35),
                Location = new Point(140, 280),
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
                Location = new Point(260, 280),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += BtnCancel_Click;

            // Add controls to form panel
            formPanel.Controls.Add(lblMaSP);
            formPanel.Controls.Add(txtMaSP);
            formPanel.Controls.Add(lblTenSP);
            formPanel.Controls.Add(txtTenSP);
            formPanel.Controls.Add(lblMoTa);
            formPanel.Controls.Add(txtMoTa);
            formPanel.Controls.Add(lblGia);
            formPanel.Controls.Add(txtGia);
            formPanel.Controls.Add(lblDVT);
            formPanel.Controls.Add(txtDVT);
            formPanel.Controls.Add(lblLoai);
            formPanel.Controls.Add(cboLoai);
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

        private void LoadProductData()
        {
            txtMaSP.Text = _product.MaSp;
            txtTenSP.Text = _product.TenSp;
            txtMoTa.Text = _product.MoTa;
            txtGia.Text = _product.Gia?.ToString();
            txtDVT.Text = _product.Dvtsp;
            cboLoai.Text = _product.Loai;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                _product.MaSp = txtMaSP.Text.Trim();
                _product.TenSp = txtTenSP.Text.Trim();
                _product.MoTa = txtMoTa.Text.Trim();
                
                if (decimal.TryParse(txtGia.Text, out decimal gia))
                {
                    _product.Gia = gia;
                }
                
                _product.Dvtsp = txtDVT.Text.Trim();
                _product.Loai = cboLoai.Text;

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
            if (string.IsNullOrWhiteSpace(txtMaSP.Text))
            {
                MessageBox.Show("Vui lòng nhập mã sản phẩm!", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaSP.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtTenSP.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm!", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenSP.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtGia.Text))
            {
                MessageBox.Show("Vui lòng nhập giá sản phẩm!", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGia.Focus();
                return false;
            }

            if (!decimal.TryParse(txtGia.Text, out decimal gia) || gia < 0)
            {
                MessageBox.Show("Giá sản phẩm không hợp lệ! Vui lòng nhập số dương.", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGia.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDVT.Text))
            {
                MessageBox.Show("Vui lòng nhập đơn vị sản phẩm!", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDVT.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(cboLoai.Text))
            {
                MessageBox.Show("Vui lòng chọn loại sản phẩm!", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboLoai.Focus();
                return false;
            }

            return true;
        }
    }
} 