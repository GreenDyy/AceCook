using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AceCook.Models;
using AceCook.Repositories;

namespace AceCook
{
    public partial class OrderAddEditForm : Form
    {
        private readonly AppDbContext _context;
        private readonly OrderRepository _orderRepository;
        private readonly Dondathang _order;
        private readonly bool _isEditMode;
        private readonly bool _isViewMode;

        // Controls
        private TextBox txtOrderId;
        private ComboBox cboCustomer;
        private ComboBox cboEmployee;
        private DateTimePicker dtpOrderDate;
        private DateTimePicker dtpDeliveryDate;
        private ComboBox cboStatus;
        private DataGridView dgvOrderDetails;
        private Button btnAddProduct;
        private Button btnRemoveProduct;
        private Button btnSave;
        private Button btnCancel;
        private Label lblTotalAmount;
        private TextBox txtTotalAmount;

        // Data sources
        private List<Khachhang> _customers;
        private List<Nhanvien> _employees;
        private List<Sanpham> _products;
        private List<CtDh> _orderDetails;

        public OrderAddEditForm(AppDbContext context, Dondathang order = null, bool isViewMode = false)
        {
            _context = context;
            _orderRepository = new OrderRepository(context);
            _order = order ?? new Dondathang();
            _isEditMode = order != null && !isViewMode;
            _isViewMode = isViewMode;
            _orderDetails = new List<CtDh>();

            if (_isEditMode && order.CtDhs != null)
            {
                _orderDetails = order.CtDhs.ToList();
            }
            else if (_isViewMode && order.CtDhs != null)
            {
                _orderDetails = order.CtDhs.ToList();
            }

            InitializeComponent();
            SetupUI();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            if (_isViewMode)
            {
                this.Text = "Xem chi tiết đơn hàng";
            }
            else
            {
                this.Text = _isEditMode ? "Chỉnh sửa đơn hàng" : "Thêm đơn hàng mới";
            }
            
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            this.ResumeLayout(false);
        }

        private void SetupUI()
        {
            // Order ID
            var lblOrderId = new Label
            {
                Text = "Mã đơn hàng:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(120, 25),
                Location = new Point(30, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtOrderId = new TextBox
            {
                Size = new Size(150, 30),
                Location = new Point(160, 27),
                Font = new Font("Segoe UI", 10),
                ReadOnly = true,
                BackColor = Color.LightGray
            };

            // Customer
            var lblCustomer = new Label
            {
                Text = "Khách hàng:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(120, 25),
                Location = new Point(30, 70),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cboCustomer = new ComboBox
            {
                Size = new Size(250, 30),
                Location = new Point(160, 67),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Employee
            var lblEmployee = new Label
            {
                Text = "Nhân viên:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(120, 25),
                Location = new Point(30, 110),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cboEmployee = new ComboBox
            {
                Size = new Size(250, 30),
                Location = new Point(160, 107),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Order Date
            var lblOrderDate = new Label
            {
                Text = "Ngày đặt:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(120, 25),
                Location = new Point(30, 150),
                TextAlign = ContentAlignment.MiddleLeft
            };

            dtpOrderDate = new DateTimePicker
            {
                Size = new Size(150, 30),
                Location = new Point(160, 147),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };

            // Delivery Date
            var lblDeliveryDate = new Label
            {
                Text = "Ngày giao:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(120, 25),
                Location = new Point(30, 190),
                TextAlign = ContentAlignment.MiddleLeft
            };

            dtpDeliveryDate = new DateTimePicker
            {
                Size = new Size(150, 30),
                Location = new Point(160, 187),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddDays(7)
            };

            // Status
            var lblStatus = new Label
            {
                Text = "Trạng thái:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(120, 25),
                Location = new Point(30, 230),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cboStatus = new ComboBox
            {
                Size = new Size(150, 30),
                Location = new Point(160, 227),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboStatus.Items.AddRange(new object[] { "Chờ xử lý", "Đang xử lý", "Đã giao", "Đã hủy" });
            cboStatus.SelectedIndex = 0;

            // Order Details Section
            var lblOrderDetails = new Label
            {
                Text = "CHI TIẾT ĐƠN HÀNG",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(200, 30),
                Location = new Point(30, 280),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Order Details Grid
            dgvOrderDetails = new DataGridView
            {
                Size = new Size(700, 200),
                Location = new Point(30, 320),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                GridColor = Color.LightGray,
                RowHeadersVisible = false,
                EditMode = DataGridViewEditMode.EditOnF2
            };

            // Setup grid columns
            dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ProductId",
                HeaderText = "Mã SP",
                Width = 80,
                ReadOnly = true
            });

            dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ProductName",
                HeaderText = "Tên sản phẩm",
                Width = 200,
                ReadOnly = true
            });

            dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Quantity",
                HeaderText = "Số lượng",
                Width = 100,
                ReadOnly = false
            });

            dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "UnitPrice",
                HeaderText = "Đơn giá",
                Width = 120,
                ReadOnly = false
            });

            dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Total",
                HeaderText = "Thành tiền",
                Width = 120,
                ReadOnly = true
            });

            // Add event handlers for editing
            dgvOrderDetails.CellEndEdit += DgvOrderDetails_CellEndEdit;
            dgvOrderDetails.CellDoubleClick += DgvOrderDetails_CellDoubleClick;

            // Buttons for order details
            btnAddProduct = new Button
            {
                Text = "➕ Thêm sản phẩm",
                Size = new Size(140, 35),
                Location = new Point(750, 320),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAddProduct.FlatAppearance.BorderSize = 0;
            btnAddProduct.Click += BtnAddProduct_Click;

            btnRemoveProduct = new Button
            {
                Text = "➖ Xóa sản phẩm",
                Size = new Size(140, 35),
                Location = new Point(750, 365),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRemoveProduct.FlatAppearance.BorderSize = 0;
            btnRemoveProduct.Click += BtnRemoveProduct_Click;

            var btnViewSummary = new Button
            {
                Text = "📋 Xem tổng quan",
                Size = new Size(140, 35),
                Location = new Point(750, 410),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnViewSummary.FlatAppearance.BorderSize = 0;
            btnViewSummary.Click += BtnViewSummary_Click;

            // Total Amount
            var lblTotal = new Label
            {
                Text = "Tổng tiền:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(100, 30),
                Location = new Point(30, 540),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtTotalAmount = new TextBox
            {
                Size = new Size(200, 30),
                Location = new Point(140, 537),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ReadOnly = true,
                BackColor = Color.LightYellow,
                TextAlign = HorizontalAlignment.Right
            };

            // Action Buttons
            btnSave = new Button
            {
                Text = "💾 Lưu đơn hàng",
                Size = new Size(150, 45),
                Location = new Point(30, 590),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "❌ Hủy bỏ",
                Size = new Size(120, 45),
                Location = new Point(200, 590),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += BtnCancel_Click;

            // Add controls to form
            this.Controls.AddRange(new Control[] {
                lblOrderId, txtOrderId,
                lblCustomer, cboCustomer,
                lblEmployee, cboEmployee,
                lblOrderDate, dtpOrderDate,
                lblDeliveryDate, dtpDeliveryDate,
                lblStatus, cboStatus,
                lblOrderDetails, dgvOrderDetails,
                btnAddProduct, btnRemoveProduct, btnViewSummary,
                lblTotal, txtTotalAmount,
                btnSave, btnCancel
            });
        }

        private async void LoadData()
        {
            try
            {
                // Load customers
                _customers = await _orderRepository.GetAllCustomersAsync();
                cboCustomer.DataSource = _customers;
                cboCustomer.DisplayMember = "TenKh";
                cboCustomer.ValueMember = "MaKh";

                // Load employees
                _employees = await _context.Nhanviens.ToListAsync();
                cboEmployee.DataSource = _employees;
                cboEmployee.DisplayMember = "HoTenNv";
                cboEmployee.ValueMember = "MaNv";

                // Load products
                _products = await _orderRepository.GetAllProductsAsync();

                if (_isEditMode || _isViewMode)
                {
                    // Load existing order data
                    txtOrderId.Text = _order.MaDdh;
                    cboCustomer.SelectedValue = _order.MaKh;
                    cboEmployee.SelectedValue = _order.MaNv;
                    dtpOrderDate.Value = _order.NgayDat?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Now;
                    if (_order.NgayGiao.HasValue)
                    {
                        dtpDeliveryDate.Value = _order.NgayGiao.Value.ToDateTime(TimeOnly.MinValue);
                    }
                    cboStatus.Text = _order.TrangThai ?? "Chờ xử lý";
                }
                else
                {
                    // Generate new order ID
                    var newOrderId = await _orderRepository.GenerateOrderIdAsync();
                    txtOrderId.Text = newOrderId;
                }

                RefreshOrderDetailsGrid();
                CalculateTotalAmount();
                
                // Apply edit restrictions based on order status
                ApplyEditRestrictions();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshOrderDetailsGrid()
        {
            dgvOrderDetails.Rows.Clear();
            foreach (var detail in _orderDetails)
            {
                var rowIndex = dgvOrderDetails.Rows.Add();
                var row = dgvOrderDetails.Rows[rowIndex];

                row.Cells["ProductId"].Value = detail.MaSp;
                row.Cells["ProductName"].Value = detail.MaSpNavigation?.TenSp ?? "N/A";
                row.Cells["Quantity"].Value = detail.SoLuong;
                row.Cells["UnitPrice"].Value = detail.DonGia?.ToString("N0") + " VNĐ";
                row.Cells["Total"].Value = ((detail.SoLuong ?? 0) * (detail.DonGia ?? 0)).ToString("N0") + " VNĐ";
            }
        }

        private void CalculateTotalAmount()
        {
            decimal total = _orderDetails.Sum(d => (decimal)((d.SoLuong ?? 0) * (d.DonGia ?? 0)));
            txtTotalAmount.Text = total.ToString("N0") + " VNĐ";
        }

        private void ApplyEditRestrictions()
        {
            if (_isViewMode)
            {
                // Disable all editing controls in view mode
                DisableAllControls();
                return;
            }

            // Check if order is completed and disable editing
            var orderStatus = _order.TrangThai?.ToLower();
            bool isCompleted = orderStatus == "hoàn thành" || orderStatus == "đã giao" || orderStatus == "đã hủy";
            
            if (isCompleted)
            {
                DisableAllControls();
                MessageBox.Show("Đơn hàng này đã hoàn thành và không thể chỉnh sửa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DisableAllControls()
        {
            // Disable input controls
            cboCustomer.Enabled = false;
            cboEmployee.Enabled = false;
            dtpOrderDate.Enabled = false;
            dtpDeliveryDate.Enabled = false;
            cboStatus.Enabled = false;
            
            // Disable order details editing
            dgvOrderDetails.ReadOnly = true;
            dgvOrderDetails.EditMode = DataGridViewEditMode.EditProgrammatically;
            
            // Disable action buttons
            btnAddProduct.Enabled = false;
            btnRemoveProduct.Enabled = false;
            btnSave.Enabled = false;
            
            // Change save button text
            btnSave.Text = "🔒 Chỉ xem";
            btnSave.BackColor = Color.FromArgb(149, 165, 166);
            
            // Disable view summary button in view mode
            if (_isViewMode)
            {
                // Find the view summary button by its text
                foreach (Control control in this.Controls)
                {
                    if (control is Button button && button.Text.Contains("Xem tổng quan"))
                    {
                        button.Enabled = false;
                        break;
                    }
                }
            }
        }

        private void BtnAddProduct_Click(object sender, EventArgs e)
        {
            if (_products == null || _products.Count == 0)
            {
                MessageBox.Show("Không có sản phẩm nào để chọn!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var form = new ProductSelectionForm(_products))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var selectedProduct = form.SelectedProduct;
                    var quantity = form.Quantity;
                    var unitPrice = form.UnitPrice;

                    // Check if product already exists
                    var existingDetail = _orderDetails.FirstOrDefault(d => d.MaSp == selectedProduct.MaSp);
                    if (existingDetail != null)
                    {
                        var result = MessageBox.Show(
                            $"Sản phẩm '{selectedProduct.TenSp}' đã có trong đơn hàng với số lượng {existingDetail.SoLuong}.\n\n" +
                            "Bạn có muốn cộng thêm số lượng mới không?\n" +
                            "Chọn 'Có' để cộng thêm, 'Không' để thay thế số lượng cũ.",
                            "Sản phẩm đã tồn tại",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            existingDetail.SoLuong = (existingDetail.SoLuong ?? 0) + quantity;
                        }
                        else
                        {
                            existingDetail.SoLuong = quantity;
                            existingDetail.DonGia = unitPrice;
                        }
                    }
                    else
                    {
                        var newDetail = new CtDh
                        {
                            MaSp = selectedProduct.MaSp,
                            MaSpNavigation = selectedProduct,
                            SoLuong = quantity,
                            DonGia = unitPrice
                        };
                        _orderDetails.Add(newDetail);
                    }

                    RefreshOrderDetailsGrid();
                    CalculateTotalAmount();
                }
            }
        }

        private void BtnRemoveProduct_Click(object sender, EventArgs e)
        {
            if (dgvOrderDetails.SelectedRows.Count > 0)
            {
                var selectedRows = dgvOrderDetails.SelectedRows.Cast<DataGridViewRow>().ToList();
                var productNames = selectedRows.Select(r => r.Cells["ProductName"].Value.ToString()).ToList();
                
                var message = selectedRows.Count == 1 
                    ? $"Bạn có chắc chắn muốn xóa sản phẩm '{productNames[0]}' khỏi đơn hàng?"
                    : $"Bạn có chắc chắn muốn xóa {selectedRows.Count} sản phẩm sau khỏi đơn hàng?\n\n{string.Join("\n", productNames)}";
                
                var result = MessageBox.Show(message, "Xác nhận xóa",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    foreach (var row in selectedRows)
                    {
                        var productId = row.Cells["ProductId"].Value.ToString();
                        var detailToRemove = _orderDetails.FirstOrDefault(d => d.MaSp == productId);
                        if (detailToRemove != null)
                        {
                            _orderDetails.Remove(detailToRemove);
                        }
                    }
                    
                    RefreshOrderDetailsGrid();
                    CalculateTotalAmount();
                    
                    MessageBox.Show($"Đã xóa {selectedRows.Count} sản phẩm khỏi đơn hàng!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            // If in view mode, just close the form
            if (_isViewMode)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
                return;
            }

            try
            {
                if (!ValidateForm())
                    return;

                // Update order object
                _order.MaDdh = txtOrderId.Text;
                _order.MaKh = cboCustomer.SelectedValue?.ToString();
                _order.MaNv = cboEmployee.SelectedValue?.ToString();
                _order.NgayDat = DateOnly.FromDateTime(dtpOrderDate.Value);
                _order.NgayGiao = DateOnly.FromDateTime(dtpDeliveryDate.Value);
                _order.TrangThai = cboStatus.Text;

                // Clear existing order details
                if (_isEditMode)
                {
                    _context.CtDhs.RemoveRange(_order.CtDhs);
                }

                // Add new order details
                foreach (var detail in _orderDetails)
                {
                    detail.MaDdh = _order.MaDdh;
                    if (_isEditMode)
                    {
                        _context.CtDhs.Add(detail);
                    }
                }

                if (_isEditMode)
                {
                    _context.Dondathangs.Update(_order);
                }
                else
                {
                    _order.CtDhs = _orderDetails;
                    _context.Dondathangs.Add(_order);
                }

                await _context.SaveChangesAsync();

                MessageBox.Show($"Đã {(this._isEditMode ? "cập nhật" : "tạo")} đơn hàng thành công!", 
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu đơn hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtOrderId.Text))
            {
                MessageBox.Show("Mã đơn hàng không được để trống!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cboCustomer.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cboEmployee.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (_orderDetails.Count == 0)
            {
                MessageBox.Show("Đơn hàng phải có ít nhất một sản phẩm!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (dtpOrderDate.Value > dtpDeliveryDate.Value)
            {
                MessageBox.Show("Ngày đặt không thể sau ngày giao!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (_isViewMode)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
            }
            this.Close();
        }

        private void DgvOrderDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var row = dgvOrderDetails.Rows[e.RowIndex];
                var productId = row.Cells["ProductId"].Value?.ToString();
                
                if (!string.IsNullOrEmpty(productId))
                {
                    var detail = _orderDetails.FirstOrDefault(d => d.MaSp == productId);
                    if (detail != null)
                    {
                        // Update quantity
                        if (e.ColumnIndex == dgvOrderDetails.Columns["Quantity"].Index)
                        {
                            if (int.TryParse(row.Cells["Quantity"].Value?.ToString(), out int newQuantity))
                            {
                                if (newQuantity > 0)
                                {
                                    detail.SoLuong = newQuantity;
                                }
                                else
                                {
                                    MessageBox.Show("Số lượng phải lớn hơn 0!", "Lỗi",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    row.Cells["Quantity"].Value = detail.SoLuong;
                                    return;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Số lượng không hợp lệ!", "Lỗi",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                row.Cells["Quantity"].Value = detail.SoLuong;
                                return;
                            }
                        }
                        
                        // Update unit price
                        if (e.ColumnIndex == dgvOrderDetails.Columns["UnitPrice"].Index)
                        {
                            var unitPriceText = row.Cells["UnitPrice"].Value?.ToString()?.Replace(" VNĐ", "");
                            if (double.TryParse(unitPriceText, out double newUnitPrice))
                            {
                                if (newUnitPrice >= 0)
                                {
                                    detail.DonGia = newUnitPrice;
                                }
                                else
                                {
                                    MessageBox.Show("Đơn giá không được âm!", "Lỗi",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    row.Cells["UnitPrice"].Value = detail.DonGia?.ToString("N0") + " VNĐ";
                                    return;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Đơn giá không hợp lệ!", "Lỗi",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                row.Cells["UnitPrice"].Value = detail.DonGia?.ToString("N0") + " VNĐ";
                                return;
                            }
                        }
                        
                        // Refresh the grid to show updated values
                        RefreshOrderDetailsGrid();
                        CalculateTotalAmount();
                    }
                }
            }
        }

        private void DgvOrderDetails_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Allow editing quantity and unit price columns
                if (e.ColumnIndex == dgvOrderDetails.Columns["Quantity"].Index ||
                    e.ColumnIndex == dgvOrderDetails.Columns["UnitPrice"].Index)
                {
                    dgvOrderDetails.BeginEdit(true);
                }
            }
        }

        private void BtnViewSummary_Click(object sender, EventArgs e)
        {
            if (_orderDetails.Count == 0)
            {
                MessageBox.Show("Đơn hàng chưa có sản phẩm nào!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var summary = $"=== TỔNG QUAN ĐƠN HÀNG ===\n\n" +
                         $"Mã đơn hàng: {txtOrderId.Text}\n" +
                         $"Khách hàng: {cboCustomer.Text}\n" +
                         $"Nhân viên: {cboEmployee.Text}\n" +
                         $"Ngày đặt: {dtpOrderDate.Value:dd/MM/yyyy}\n" +
                         $"Ngày giao: {dtpDeliveryDate.Value:dd/MM/yyyy}\n" +
                         $"Trạng thái: {cboStatus.Text}\n\n" +
                         $"=== CHI TIẾT SẢN PHẨM ===\n";

            decimal totalAmount = 0;
            foreach (var detail in _orderDetails)
            {
                var amount = (detail.SoLuong ?? 0) * (detail.DonGia ?? 0);
                totalAmount += (decimal)amount;
                summary += $"\n{detail.MaSpNavigation?.TenSp ?? "N/A"}\n" +
                          $"  Số lượng: {detail.SoLuong}\n" +
                          $"  Đơn giá: {detail.DonGia:N0} VNĐ\n" +
                          $"  Thành tiền: {amount:N0} VNĐ\n";
            }

            summary += $"\n=== TỔNG TIỀN: {totalAmount:N0} VNĐ ===";
            summary += $"\n=== TỔNG SẢN PHẨM: {_orderDetails.Count} loại ===";

            MessageBox.Show(summary, "Tổng quan đơn hàng", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    // Helper form for product selection
    public class ProductSelectionForm : Form
    {
        private ComboBox cboProduct;
        private NumericUpDown nudQuantity;
        private NumericUpDown nudUnitPrice;
        private Button btnOK;
        private Button btnCancel;

        public Sanpham SelectedProduct { get; private set; }
        public int Quantity { get; private set; }
        public double UnitPrice { get; private set; }

        public ProductSelectionForm(List<Sanpham> products)
        {
            this.Text = "Chọn sản phẩm";
            this.Size = new Size(400, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            var lblProduct = new Label
            {
                Text = "Sản phẩm:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(100, 25),
                Location = new Point(30, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cboProduct = new ComboBox
            {
                Size = new Size(250, 30),
                Location = new Point(30, 60),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboProduct.DataSource = products;
            cboProduct.DisplayMember = "TenSp";
            cboProduct.ValueMember = "MaSp";
            cboProduct.SelectedIndexChanged += CboProduct_SelectedIndexChanged;

            var lblQuantity = new Label
            {
                Text = "Số lượng:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(100, 25),
                Location = new Point(30, 100),
                TextAlign = ContentAlignment.MiddleLeft
            };

            nudQuantity = new NumericUpDown
            {
                Size = new Size(100, 30),
                Location = new Point(30, 130),
                Font = new Font("Segoe UI", 10),
                Minimum = 1,
                Maximum = 9999,
                Value = 1
            };

            var lblUnitPrice = new Label
            {
                Text = "Đơn giá:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(100, 25),
                Location = new Point(180, 100),
                TextAlign = ContentAlignment.MiddleLeft
            };

            nudUnitPrice = new NumericUpDown
            {
                Size = new Size(150, 30),
                Location = new Point(180, 130),
                Font = new Font("Segoe UI", 10),
                Minimum = 0,
                Maximum = 999999999,
                DecimalPlaces = 0,
                Value = 0
            };

            btnOK = new Button
            {
                Text = "OK",
                Size = new Size(80, 35),
                Location = new Point(180, 170),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Click += BtnOK_Click;

            btnCancel = new Button
            {
                Text = "Hủy",
                Size = new Size(80, 35),
                Location = new Point(280, 170),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            this.Controls.AddRange(new Control[] {
                lblProduct, cboProduct,
                lblQuantity, nudQuantity,
                lblUnitPrice, nudUnitPrice,
                btnOK, btnCancel
            });

            // Bây giờ mới set SelectedIndex sau khi controls đã được thêm vào form
            if (products.Count > 0)
            {
                cboProduct.SelectedIndex = 0;
            }
        }

        // test

        private void CboProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboProduct.SelectedItem is Sanpham product)
            {
                nudUnitPrice.Value = product.Gia ?? 0;
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (cboProduct.SelectedItem is Sanpham product)
            {
                SelectedProduct = product;
                Quantity = (int)nudQuantity.Value;
                UnitPrice = (double)nudUnitPrice.Value;
            }
        }
    }
}
