using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AceCook.Models;
using AceCook.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AceCook
{
    public partial class OrderAddEditForm : Form
    {
        private readonly AppDbContext _context;
        private readonly OrderRepository _orderRepository;
        private readonly CustomerRepository _customerRepository;
        private readonly ProductRepository _productRepository;
        private readonly InventoryRepository _inventoryRepository;
        
        private List<Sanpham> _allProducts;
        private List<Khachhang> _allCustomers;
        private List<OrderItem> _orderItems;
        private string _currentOrderId;
        private bool _isEditMode;
        private bool _isViewMode;
        private Dondathang _editingOrder;

        public OrderAddEditForm()
        {
            InitializeComponent();
            _context = new AppDbContext();
            _orderRepository = new OrderRepository(_context);
            _customerRepository = new CustomerRepository(_context);
            _productRepository = new ProductRepository(_context);
            _inventoryRepository = new InventoryRepository(_context);
            
            _orderItems = new List<OrderItem>();
            _isEditMode = false;
            
            InitializeForm();
        }

        private void InitializeRepositories()
        {
            if (_orderRepository == null)
                _orderRepository = new OrderRepository(_context);
            if (_customerRepository == null)
                _customerRepository = new CustomerRepository(_context);
            if (_productRepository == null)
                _productRepository = new ProductRepository(_context);
            if (_inventoryRepository == null)
                _inventoryRepository = new InventoryRepository(_context);
        }

        public OrderAddEditForm(string orderId) : this()
        {
            _currentOrderId = orderId;
            _isEditMode = true;
            LoadOrderForEdit();
        }

        public OrderAddEditForm(AppDbContext context, Dondathang? order, bool isViewMode) : this()
        {
            _context = context;
            _isEditMode = !isViewMode;
            _isViewMode = isViewMode;
            
            InitializeRepositories();
            
            if (order != null)
            {
                _currentOrderId = order.MaDdh;
                _editingOrder = order;
                LoadOrderForEdit();
            }
            else
            {
                // Tạo đơn hàng mới
                _isEditMode = false;
            }
        }

        private async void InitializeForm()
        {
            try
            {
                // Load customers and products
                _allCustomers = await _customerRepository.GetAllCustomersAsync();
                _allProducts = await _productRepository.GetAllProductsAsync();

                // Setup customer combo box
                cboCustomer.DataSource = _allCustomers;
                cboCustomer.DisplayMember = "TenKh";
                cboCustomer.ValueMember = "MaKh";

                // Setup product combo box
                cboProduct.DataSource = _allProducts;
                cboProduct.DisplayMember = "TenSp";
                cboProduct.ValueMember = "MaSp";
                
                // Add stock information to product display
                cboProduct.Format += async (sender, e) => await FormatProductDisplay(sender, e);

                // Setup DataGridView
                dgvOrderItems.AutoGenerateColumns = false;
                dgvOrderItems.Columns.Clear();
                
                dgvOrderItems.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ProductId",
                    HeaderText = "Mã SP",
                    DataPropertyName = "ProductId",
                    Width = 80
                });
                
                dgvOrderItems.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ProductName",
                    HeaderText = "Tên Sản Phẩm",
                    DataPropertyName = "ProductName",
                    Width = 200
                });
                
                dgvOrderItems.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Quantity",
                    HeaderText = "Số Lượng",
                    DataPropertyName = "Quantity",
                    Width = 100
                });
                
                // Add event handler for quantity changes
                dgvOrderItems.CellEndEdit += DgvOrderItems_CellEndEdit;
                
                // Add event handler for product selection
                cboProduct.SelectedIndexChanged += CboProduct_SelectedIndexChanged;
                
                dgvOrderItems.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "UnitPrice",
                    HeaderText = "Đơn Giá",
                    DataPropertyName = "UnitPrice",
                    Width = 120
                });
                
                dgvOrderItems.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalPrice",
                    HeaderText = "Thành Tiền",
                    DataPropertyName = "TotalPrice",
                    Width = 120
                });

                // Generate order ID
                if (!_isEditMode)
                {
                    _currentOrderId = await _orderRepository.GenerateOrderIdAsync();
                    txtOrderId.Text = _currentOrderId;
                }

                // Set default dates
                dtpOrderDate.Value = DateTime.Now;
                dtpDeliveryDate.Value = DateTime.Now.AddDays(7);

                            // Set default status
            cboStatus.SelectedItem = "Chờ xử lý";

                            // Set form title based on mode
                if (_isViewMode)
                {
                    this.Text = "Xem chi tiết đơn hàng";
                }
                else if (_isEditMode)
                {
                    this.Text = "Chỉnh sửa đơn hàng";
                }
                else
                {
                    this.Text = "Thêm đơn hàng mới";
                }

                UpdateTotalAmount();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khởi tạo form: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisableControlsForViewMode()
        {
            // Disable all input controls
            cboCustomer.Enabled = false;
            dtpOrderDate.Enabled = false;
            dtpDeliveryDate.Enabled = false;
            cboStatus.Enabled = false;
            cboProduct.Enabled = false;
            numQuantity.Enabled = false;
            btnAddProduct.Enabled = false;
            btnRemoveProduct.Enabled = false;
            
            // Hide Save button and show Close button for view mode
            btnSave.Visible = false;
            btnCancel.Text = "Đóng";
            
            // Make DataGridView read-only
            dgvOrderItems.ReadOnly = true;
        }

        private async void LoadOrderForEdit()
        {
            try
            {
                _editingOrder = await _orderRepository.GetOrderByIdAsync(_currentOrderId);
                if (_editingOrder != null)
                {
                    txtOrderId.Text = _editingOrder.MaDdh;
                    cboCustomer.SelectedValue = _editingOrder.MaKh;
                    dtpOrderDate.Value = _editingOrder.NgayDat?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Now;
                    dtpDeliveryDate.Value = _editingOrder.NgayGiao?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Now.AddDays(7);
                    cboStatus.SelectedItem = _editingOrder.TrangThai;

                    // Load order items
                    foreach (var ct in _editingOrder.CtDhs)
                    {
                        var product = _allProducts.FirstOrDefault(p => p.MaSp == ct.MaSp);
                        if (product != null)
                        {
                            _orderItems.Add(new OrderItem
                            {
                                ProductId = ct.MaSp,
                                ProductName = product.TenSp ?? "",
                                Quantity = ct.SoLuong ?? 0,
                                UnitPrice = (double)(ct.DonGia ?? 0)
                            });
                        }
                    }

                    RefreshOrderItemsGrid();
                    UpdateTotalAmount();
                }
                
                // Disable controls if in view mode
                if (_isViewMode)
                {
                    DisableControlsForViewMode();
                }
                
                // Update form title with order ID if available
                if (!string.IsNullOrEmpty(_currentOrderId))
                {
                    this.Text += $" - {_currentOrderId}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải đơn hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnAddProduct_Click(object sender, EventArgs e)
        {
            if (_isViewMode) return; // Không cho phép thêm sản phẩm khi ở view mode
            
            if (cboProduct.SelectedValue == null || numQuantity.Value <= 0)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm và nhập số lượng hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedProduct = cboProduct.SelectedItem as Sanpham;
            if (selectedProduct == null) return;

            var quantity = (int)numQuantity.Value;
            var unitPrice = (double)(selectedProduct.Gia ?? 0);

            // Kiểm tra tồn kho
            var availableStock = await GetAvailableStock(selectedProduct.MaSp);
            if (quantity > availableStock)
            {
                MessageBox.Show($"Số lượng vượt quá tồn kho! Tồn kho hiện tại: {availableStock}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if product already exists in order
            var existingItem = _orderItems.FirstOrDefault(item => item.ProductId == selectedProduct.MaSp);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.TotalPrice = existingItem.Quantity * existingItem.UnitPrice;
            }
            else
            {
                _orderItems.Add(new OrderItem
                {
                    ProductId = selectedProduct.MaSp,
                    ProductName = selectedProduct.TenSp ?? "",
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    TotalPrice = quantity * unitPrice
                });
            }

            RefreshOrderItemsGrid();
            UpdateTotalAmount();
            ClearProductSelection();
        }

        private void btnRemoveProduct_Click(object sender, EventArgs e)
        {
            if (_isViewMode) return; // Không cho phép xóa sản phẩm khi ở view mode
            
            if (dgvOrderItems.SelectedRows.Count > 0)
            {
                var selectedRow = dgvOrderItems.SelectedRows[0];
                var productId = selectedRow.Cells["ProductId"].Value?.ToString();
                
                if (!string.IsNullOrEmpty(productId))
                {
                    _orderItems.RemoveAll(item => item.ProductId == productId);
                    RefreshOrderItemsGrid();
                    UpdateTotalAmount();
                }
            }
        }

        private void RefreshOrderItemsGrid()
        {
            dgvOrderItems.DataSource = null;
            dgvOrderItems.DataSource = _orderItems;
        }

        private void UpdateTotalAmount()
        {
            var total = _orderItems.Sum(item => item.TotalPrice);
            lblTotalAmount.Text = $"Tổng tiền: {total:N0} VNĐ";
        }

        private void ClearProductSelection()
        {
            cboProduct.SelectedIndex = -1;
            numQuantity.Value = 1;
        }

        private async void DgvOrderItems_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (_isViewMode) return; // Không cho phép chỉnh sửa khi ở view mode
            
            if (e.ColumnIndex == dgvOrderItems.Columns["Quantity"].Index && e.RowIndex >= 0)
            {
                var row = dgvOrderItems.Rows[e.RowIndex];
                var productId = row.Cells["ProductId"].Value?.ToString();
                var newQuantity = Convert.ToInt32(row.Cells["Quantity"].Value ?? 0);
                
                if (!string.IsNullOrEmpty(productId) && newQuantity > 0)
                {
                    // Kiểm tra tồn kho
                    var availableStock = await GetAvailableStock(productId);
                    if (newQuantity > availableStock)
                    {
                        MessageBox.Show($"Số lượng vượt quá tồn kho! Tồn kho hiện tại: {availableStock}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        row.Cells["Quantity"].Value = availableStock;
                        newQuantity = availableStock;
                    }
                    
                    var item = _orderItems.FirstOrDefault(i => i.ProductId == productId);
                    if (item != null)
                    {
                        item.Quantity = newQuantity;
                        item.TotalPrice = item.Quantity * item.UnitPrice;
                        row.Cells["TotalPrice"].Value = item.TotalPrice;
                        UpdateTotalAmount();
                    }
                }
            }
        }

        private async void CboProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isViewMode) return; // Không cho phép thay đổi sản phẩm khi ở view mode
            
            if (cboProduct.SelectedValue != null)
            {
                var productId = cboProduct.SelectedValue.ToString();
                var availableStock = await GetAvailableStock(productId);
                var selectedProduct = cboProduct.SelectedItem as Sanpham;
                
                if (selectedProduct != null)
                {
                    var price = selectedProduct.Gia?.ToString("N0") ?? "0";
                    lblStockInfo.Text = $"Tồn kho: {availableStock} | Giá: {price} VNĐ";
                    numQuantity.Maximum = availableStock;
                }
            }
            else
            {
                lblStockInfo.Text = "";
                numQuantity.Maximum = 9999;
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (_isViewMode) return; // Không cho phép lưu khi ở view mode
            
            if (!ValidateForm()) return;

            try
            {
                btnSave.Enabled = false;
                btnSave.Text = "Đang lưu...";

                if (_isEditMode)
                {
                    await UpdateExistingOrder();
                }
                else
                {
                    await CreateNewOrder();
                }

                MessageBox.Show("Lưu đơn hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lưu đơn hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSave.Enabled = true;
                btnSave.Text = "Lưu";
            }
        }

        private bool ValidateForm()
        {
            if (cboCustomer.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (_orderItems.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm ít nhất một sản phẩm vào đơn hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(cboStatus.Text))
            {
                MessageBox.Show("Vui lòng chọn trạng thái đơn hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private async Task CreateNewOrder()
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Tạo đơn đặt hàng
                var order = new Dondathang
                {
                    MaDdh = _currentOrderId,
                    MaKh = cboCustomer.SelectedValue?.ToString(),
                    MaNv = "NV001", // TODO: Get current user ID
                    NgayDat = DateOnly.FromDateTime(dtpOrderDate.Value),
                    NgayGiao = DateOnly.FromDateTime(dtpDeliveryDate.Value),
                    TrangThai = cboStatus.Text,
                    CtDhs = new List<CtDh>()
                };

                // 2. Tạo chi tiết đơn hàng và cập nhật tồn kho
                foreach (var item in _orderItems)
                {
                    var ctDh = new CtDh
                    {
                        MaDdh = _currentOrderId,
                        MaSp = item.ProductId,
                        SoLuong = item.Quantity,
                        DonGia = item.UnitPrice
                    };
                    order.CtDhs.Add(ctDh);

                    // Cập nhật tồn kho (trừ số lượng)
                    await UpdateInventory(item.ProductId, item.Quantity);
                }

                // 3. Lưu đơn hàng
                await _context.Dondathangs.AddAsync(order);
                await _context.SaveChangesAsync();

                // 4. Tạo hóa đơn bán
                var totalAmount = _orderItems.Sum(item => item.TotalPrice);
                var invoice = new Hoadonban
                {
                    MaHdb = GenerateInvoiceId(),
                    NgayLap = DateOnly.FromDateTime(DateTime.Now),
                    TongTien = (decimal)totalAmount,
                    Vat = 0, // TODO: Add VAT calculation
                    TrangThaiThanhToan = "Chưa thanh toán",
                    MaNv = "NV001" // TODO: Get current user ID
                };

                await _context.Hoadonbans.AddAsync(invoice);
                await _context.SaveChangesAsync();

                // 5. Tạo phiếu xuất kho
                var exportNote = new Phieuxuatkho
                {
                    MaPxk = GenerateExportNoteId(),
                    NgayXuat = DateOnly.FromDateTime(DateTime.Now),
                    MaHdb = invoice.MaHdb,
                    MaKho = "KHO001", // Kho mặc định
                    TrangThaiPxk = "Đã xuất"
                };

                await _context.Phieuxuatkhos.AddAsync(exportNote);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task UpdateExistingOrder()
        {
            if (_editingOrder == null) return;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Cập nhật thông tin đơn hàng
                _editingOrder.MaKh = cboCustomer.SelectedValue?.ToString();
                _editingOrder.NgayDat = DateOnly.FromDateTime(dtpOrderDate.Value);
                _editingOrder.NgayGiao = DateOnly.FromDateTime(dtpDeliveryDate.Value);
                _editingOrder.TrangThai = cboStatus.Text;

                // 2. Xóa chi tiết cũ và tạo mới
                _context.CtDhs.RemoveRange(_editingOrder.CtDhs);
                _editingOrder.CtDhs.Clear();

                foreach (var item in _orderItems)
                {
                    var ctDh = new CtDh
                    {
                        MaDdh = _editingOrder.MaDdh,
                        MaSp = item.ProductId,
                        SoLuong = item.Quantity,
                        DonGia = item.UnitPrice
                    };
                    _editingOrder.CtDhs.Add(ctDh);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task FormatProductDisplay(object sender, ListControlConvertEventArgs e)
        {
            if (e.ListItem is Sanpham product)
            {
                var stock = await GetAvailableStock(product.MaSp);
                var price = product.Gia?.ToString("N0") ?? "0";
                e.Value = $"{product.TenSp} - Tồn: {stock} - Giá: {price} VNĐ";
            }
        }

        private async Task<int> GetAvailableStock(string productId)
        {
            var inventory = await _context.CtTons
                .FirstOrDefaultAsync(ct => ct.MaSp == productId && ct.MaKho == "KHO001");
            
            return inventory?.SoLuongTonKho ?? 0;
        }

        private async Task UpdateInventory(string productId, int quantity)
        {
            // Lấy tồn kho của sản phẩm (giả sử kho mặc định là "KHO001")
            var inventory = await _context.CtTons
                .FirstOrDefaultAsync(ct => ct.MaSp == productId && ct.MaKho == "KHO001");

            if (inventory != null)
            {
                inventory.SoLuongTonKho = Math.Max(0, (inventory.SoLuongTonKho ?? 0) - quantity);
            }
            else
            {
                // Tạo mới nếu chưa có
                inventory = new CtTon
                {
                    MaSp = productId,
                    MaKho = "KHO001",
                    SoLuongTonKho = 0
                };
                await _context.CtTons.AddAsync(inventory);
            }
        }

        private string GenerateInvoiceId()
        {
            return $"HDB{DateTime.Now:yyyyMMddHHmmss}";
        }

        private string GenerateExportNoteId()
        {
            return $"PXK{DateTime.Now:yyyyMMddHHmmss}";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void OrderAddEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _context?.Dispose();
        }
    }

    public class OrderItem
    {
        public string ProductId { get; set; } = "";
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }
    }
}
