using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AceCook.Models;
using AceCook.Repositories;

namespace AceCook
{
    public partial class InventoryAddEditForm : Form
    {
        private readonly AppDbContext _context;
        private readonly InventoryRepository _inventoryRepository;
        private readonly CtTon _inventoryItem;
        private readonly bool _isEditMode;
        private List<Sanpham> _products;
        private List<Khohang> _warehouses;

        public CtTon InventoryItem => _inventoryItem;

        public InventoryAddEditForm(AppDbContext context, CtTon inventoryItem = null)
        {
            _context = context;
            _inventoryRepository = new InventoryRepository(context);
            _inventoryItem = inventoryItem ?? new CtTon();
            _isEditMode = inventoryItem != null;
            _products = new List<Sanpham>();
            _warehouses = new List<Khohang>();
            
            InitializeComponent();
            SetupUI();
            _ = LoadDataAsync();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            if (_isEditMode)
            {
                this.Text = "Chỉnh sửa tồn kho";
            }
            else
            {
                this.Text = "Thêm tồn kho mới";
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
                Text = _isEditMode ? "CHỈNH SỬA TỒN KHO" : "THÊM TỒN KHO MỚI",
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

            // Sản phẩm
            var lblProduct = new Label
            {
                Text = "Sản phẩm:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(100, 25),
                Location = new Point(20, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var cboProduct = new ComboBox
            {
                Name = "cboProduct",
                Size = new Size(300, 30),
                Location = new Point(130, 28),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Kho hàng
            var lblWarehouse = new Label
            {
                Text = "Kho hàng:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(100, 25),
                Location = new Point(20, 80),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var cboWarehouse = new ComboBox
            {
                Name = "cboWarehouse",
                Size = new Size(300, 30),
                Location = new Point(130, 78),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Số lượng tồn
            var lblQuantity = new Label
            {
                Text = "Số lượng tồn:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(100, 25),
                Location = new Point(20, 130),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var numQuantity = new NumericUpDown
            {
                Name = "numQuantity",
                Size = new Size(150, 30),
                Location = new Point(130, 128),
                Font = new Font("Segoe UI", 10),
                Minimum = 0,
                Maximum = 999999,
                Value = _inventoryItem.SoLuongTonKho ?? 0
            };

            // Ghi chú
            var lblNote = new Label
            {
                Text = "Ghi chú:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(100, 25),
                Location = new Point(20, 180),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var txtNote = new TextBox
            {
                Name = "txtNote",
                Size = new Size(300, 60),
                Location = new Point(130, 178),
                Font = new Font("Segoe UI", 10),
                Multiline = true,
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
                lblProduct, cboProduct, lblWarehouse, cboWarehouse,
                lblQuantity, numQuantity, lblNote, txtNote, btnSave, btnCancel
            });

            // Add controls to form
            this.Controls.AddRange(new Control[] { lblTitle, formPanel });
        }

        private async Task LoadDataAsync()
        {
            try
            {
                // Load products
                var productRepo = new ProductRepository(_context);
                _products = await productRepo.GetAllProductsAsync();
                
                var cboProduct = GetControl<ComboBox>("cboProduct");
                cboProduct.DataSource = _products;
                cboProduct.DisplayMember = "TenSp";
                cboProduct.ValueMember = "MaSp";

                // Load warehouses
                _warehouses = await _inventoryRepository.GetAllWarehousesAsync();
                
                var cboWarehouse = GetControl<ComboBox>("cboWarehouse");
                cboWarehouse.DataSource = _warehouses;
                cboWarehouse.DisplayMember = "TenKho";
                cboWarehouse.ValueMember = "MaKho";

                // Set selected values if editing
                if (_isEditMode)
                {
                    cboProduct.SelectedValue = _inventoryItem.MaSp;
                    cboWarehouse.SelectedValue = _inventoryItem.MaKho;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            try
            {
                var cboProduct = GetControl<ComboBox>("cboProduct");
                var cboWarehouse = GetControl<ComboBox>("cboWarehouse");
                var numQuantity = GetControl<NumericUpDown>("numQuantity");
                var txtNote = GetControl<TextBox>("txtNote");

                // Update inventory object
                _inventoryItem.MaSp = cboProduct.SelectedValue?.ToString();
                _inventoryItem.MaKho = cboWarehouse.SelectedValue?.ToString();
                _inventoryItem.SoLuongTonKho = (int)numQuantity.Value;

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
            var cboProduct = GetControl<ComboBox>("cboProduct");
            var cboWarehouse = GetControl<ComboBox>("cboWarehouse");
            var numQuantity = GetControl<NumericUpDown>("numQuantity");

            // Validate required fields
            if (cboProduct.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboProduct.Focus();
                return false;
            }

            if (cboWarehouse.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn kho hàng!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboWarehouse.Focus();
                return false;
            }

            if (numQuantity.Value <= 0)
            {
                MessageBox.Show("Số lượng tồn phải lớn hơn 0!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numQuantity.Focus();
                return false;
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
