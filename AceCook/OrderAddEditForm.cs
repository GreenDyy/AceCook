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
        private OrderRepository _orderRepository;
        private CustomerRepository _customerRepository;
        private ProductRepository _productRepository;
        private InventoryRepository _inventoryRepository;

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
            _isViewMode = false;
            
            SetupFormForOrder();
        }

        private void InitializeRepositories()
        {
            _orderRepository = new OrderRepository(_context);
            _customerRepository = new CustomerRepository(_context);
            _productRepository = new ProductRepository(_context);
            _inventoryRepository = new InventoryRepository(_context);
        }

        public OrderAddEditForm(string orderId) : this()
        {
            _currentOrderId = orderId;
            _isEditMode = true;
            _isViewMode = false;
            // LoadOrderForEdit sẽ được gọi sau khi SetupFormForOrder hoàn thành
        }

        public OrderAddEditForm(Dondathang order, bool isViewMode = false)
        {
            InitializeComponent();
            _context = new AppDbContext();
            _orderRepository = new OrderRepository(_context);
            _customerRepository = new CustomerRepository(_context);
            _productRepository = new ProductRepository(_context);
            _inventoryRepository = new InventoryRepository(_context);
            
            _orderItems = new List<OrderItem>();
            _isEditMode = !isViewMode;
            _isViewMode = isViewMode;
            _editingOrder = order;
            _currentOrderId = order?.MaDdh;
            
            // Không gọi InitializeForm() ở đây, sẽ gọi sau khi setup repositories
            SetupFormForOrder();
        }

                public OrderAddEditForm(AppDbContext context, Dondathang? order, bool isViewMode)
        {
            InitializeComponent();
            // Tạo context mới để tránh vấn đề disposed context
            _context = new AppDbContext();
            _isEditMode = !isViewMode;
            _isViewMode = isViewMode;
            
            InitializeRepositories();
            
            _orderItems = new List<OrderItem>();
            
            if (order != null)
            {
                _currentOrderId = order.MaDdh;
                _editingOrder = order;
                SetupFormForOrder();
            }
            else
            {
                // Tạo đơn hàng mới
                _isEditMode = false;
                SetupFormForOrder();
            }
        }



        private void DisableControlsForViewMode()
        {
            System.Diagnostics.Debug.WriteLine("DisableControlsForViewMode called");
            
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
            
            System.Diagnostics.Debug.WriteLine("All controls disabled for view mode");
        }



        private async void LoadOrderForEdit()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"LoadOrderForEdit called - _editingOrder: {_editingOrder?.MaDdh}, _currentOrderId: {_currentOrderId}, _isViewMode: {_isViewMode}");
                
                // Nếu _editingOrder chưa được set, load từ database
                if (_editingOrder == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Loading order from database with ID: {_currentOrderId}");
                    _editingOrder = await _orderRepository.GetOrderByIdAsync(_currentOrderId).ConfigureAwait(false);
                    System.Diagnostics.Debug.WriteLine($"Loaded order: {_editingOrder?.MaDdh}, CtDhs count: {_editingOrder?.CtDhs?.Count ?? 0}");
                }

                if (_editingOrder != null)
                {
                    txtOrderId.Text = _editingOrder.MaDdh;
                    cboCustomer.SelectedValue = _editingOrder.MaKh;
                    dtpOrderDate.Value = _editingOrder.NgayDat?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Now;
                    dtpDeliveryDate.Value = _editingOrder.NgayGiao?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Now.AddDays(7);
                    cboStatus.SelectedItem = _editingOrder.TrangThai;

                    // Load order items
                    System.Diagnostics.Debug.WriteLine($"Loading {_editingOrder.CtDhs?.Count ?? 0} order items");
                    foreach (var ct in _editingOrder.CtDhs)
                    {
                        System.Diagnostics.Debug.WriteLine($"Processing order item: MaSp={ct.MaSp}, SoLuong={ct.SoLuong}, DonGia={ct.DonGia}");
                        var product = _allProducts.FirstOrDefault(p => p.MaSp == ct.MaSp);
                        if (product != null)
                        {
                            var quantity = ct.SoLuong ?? 0;
                            var unitPrice = (double)(ct.DonGia ?? 0);
                            var orderItem = new OrderItem
                            {
                                ProductId = ct.MaSp,
                                ProductName = product.TenSp ?? "",
                                Quantity = quantity,
                                UnitPrice = unitPrice,
                                TotalPrice = quantity * unitPrice
                            };
                            _orderItems.Add(orderItem);
                            System.Diagnostics.Debug.WriteLine($"Added order item: {orderItem.ProductId} - {orderItem.ProductName} - Qty: {orderItem.Quantity} - Price: {orderItem.UnitPrice}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Product not found for MaSp: {ct.MaSp}");
                        }
                    }
                    System.Diagnostics.Debug.WriteLine($"Total order items loaded: {_orderItems.Count}");

                    System.Diagnostics.Debug.WriteLine("Refreshing order items grid...");
                    RefreshOrderItemsGrid();
                    System.Diagnostics.Debug.WriteLine("Updating total amount...");
                    UpdateTotalAmount();
                }

                // Disable controls if in view mode
                if (_isViewMode)
                {
                    System.Diagnostics.Debug.WriteLine("Disabling controls for view mode...");
                    DisableControlsForViewMode();
                }

                // Update form title with order ID if available
                if (!string.IsNullOrEmpty(_currentOrderId))
                {
                    this.Text += $" - {_currentOrderId}";
                    System.Diagnostics.Debug.WriteLine($"Form title updated with order ID: {this.Text}");
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
            var availableStock = await GetAvailableStock(selectedProduct.MaSp).ConfigureAwait(false);
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
            System.Diagnostics.Debug.WriteLine($"RefreshOrderItemsGrid called - _orderItems count: {_orderItems.Count}");
            dgvOrderItems.DataSource = null;
            dgvOrderItems.DataSource = _orderItems;
            System.Diagnostics.Debug.WriteLine($"DataGridView rows count after refresh: {dgvOrderItems.Rows.Count}");
        }

        private void UpdateTotalAmount()
        {
            var total = _orderItems.Sum(item => item.TotalPrice);
            System.Diagnostics.Debug.WriteLine($"UpdateTotalAmount - Total: {total:N0} VNĐ");
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
                    var availableStock = await GetAvailableStock(productId).ConfigureAwait(false);
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
                var availableStock = await GetAvailableStock(productId).ConfigureAwait(false);
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

        private void CboCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isViewMode) return; // Không cho phép thay đổi khách hàng khi ở view mode

            // Có thể thêm logic xử lý khi khách hàng thay đổi ở đây
            // Ví dụ: load thông tin khách hàng, cập nhật form, v.v.
            System.Diagnostics.Debug.WriteLine($"Customer changed to: {cboCustomer.SelectedValue}");
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
            using var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false);
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
                    await UpdateInventory(item.ProductId, item.Quantity).ConfigureAwait(false);
                }

                // 3. Lưu đơn hàng
                await _context.Dondathangs.AddAsync(order);
                await _context.SaveChangesAsync().ConfigureAwait(false);

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
                await _context.SaveChangesAsync().ConfigureAwait(false);

                // 5. Tạo phiếu xuất kho
                var exportNote = new Phieuxuatkho
                {
                    MaPxk = GenerateExportNoteId(),
                    NgayXuat = DateOnly.FromDateTime(DateTime.Now),
                    MaHdb = invoice.MaHdb,
                    MaKho = "K01", // Kho mặc định
                    TrangThaiPxk = "Đã xuất"
                };

                await _context.Phieuxuatkhos.AddAsync(exportNote);
                await _context.SaveChangesAsync().ConfigureAwait(false);

                await transaction.CommitAsync().ConfigureAwait(false);
            }
            catch
            {
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        private async Task UpdateExistingOrder()
        {
            if (_editingOrder == null) return;

            using var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false);
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

                await _context.SaveChangesAsync().ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
            }
            catch
            {
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        private void FormatProductDisplay(object sender, ListControlConvertEventArgs e)
        {
            if (e.ListItem is Sanpham product)
            {
                var price = product.Gia?.ToString("N0") ?? "0";
                e.Value = $"{product.TenSp} - Giá: {price} VNĐ";
            }
        }

        private async Task<int> GetAvailableStock(string productId)
        {
            try
            {
                var inventory = await _context.CtTons
                    .FirstOrDefaultAsync(ct => ct.MaSp == productId && ct.MaKho == "K01")
                    .ConfigureAwait(false);

                return inventory?.SoLuongTonKho ?? 0;
            }
            catch (Exception ex)
            {
                // Log error and return 0 as default
                System.Diagnostics.Debug.WriteLine($"Error getting stock for product {productId}: {ex.Message}");
                return 0;
            }
        }

        private async Task UpdateInventory(string productId, int quantity)
        {
            try
            {
                // Lấy tồn kho của sản phẩm (giả sử kho mặc định là "K01")
                var inventory = await _context.CtTons
                    .FirstOrDefaultAsync(ct => ct.MaSp == productId && ct.MaKho == "K01")
                    .ConfigureAwait(false);

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
                        MaKho = "K01",
                        SoLuongTonKho = 0
                    };
                    await _context.CtTons.AddAsync(inventory).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating inventory for product {productId}: {ex.Message}");
                throw; // Re-throw để transaction có thể rollback
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

        private async Task CreateSampleCustomers()
        {
            try
            {
                var sampleCustomers = new List<Khachhang>
                {
                    new Khachhang
                    {
                        MaKh = "KH001",
                        TenKh = "Nguyễn Văn An",
                        LoaiKh = "Cá nhân",
                        Sdtkh = "0123456789",
                        DiaChiKh = "123 Đường ABC, Quận 1, TP.HCM",
                        EmailKh = "nguyenvanan@email.com"
                    },
                    new Khachhang
                    {
                        MaKh = "KH002",
                        TenKh = "Trần Thị Bình",
                        LoaiKh = "Cá nhân",
                        Sdtkh = "0987654321",
                        DiaChiKh = "456 Đường XYZ, Quận 2, TP.HCM",
                        EmailKh = "tranthibinh@email.com"
                    },
                    new Khachhang
                    {
                        MaKh = "KH003",
                        TenKh = "Công ty TNHH Minh Phát",
                        LoaiKh = "Doanh nghiệp",
                        Sdtkh = "0281234567",
                        DiaChiKh = "789 Đường DEF, Quận 3, TP.HCM",
                        EmailKh = "info@minhphat.com"
                    }
                };

                foreach (var customer in sampleCustomers)
                {
                    await _context.Khachhangs.AddAsync(customer);
                }
                await _context.SaveChangesAsync().ConfigureAwait(false);

                // Reload customers
                _allCustomers = await _customerRepository.GetAllCustomersAsync().ConfigureAwait(false);
                cboCustomer.DataSource = _allCustomers;

                MessageBox.Show("Đã tạo dữ liệu mẫu cho khách hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating sample customers: {ex.Message}");
                MessageBox.Show($"Lỗi tạo dữ liệu mẫu khách hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

                private void ShowDataInfo()
        {
            var customerCount = _allCustomers?.Count ?? 0;
            var productCount = _allProducts?.Count ?? 0;
            
            var info = $"Dữ liệu đã load:\n" +
                      $"• Khách hàng: {customerCount}\n" +
                      $"• Sản phẩm: {productCount}";
            
            if (customerCount == 0 || productCount == 0)
            {
                info += "\n\nLưu ý: Nếu không có dữ liệu, hệ thống sẽ tự động tạo dữ liệu mẫu.";
            }
            
            System.Diagnostics.Debug.WriteLine(info);
        }

        private async Task<bool> CheckDatabaseConnection()
        {
            try
            {
                // Kiểm tra kết nối bằng cách thực hiện một query đơn giản
                var canConnect = await _context.Database.CanConnectAsync();
                if (!canConnect)
                {
                    System.Diagnostics.Debug.WriteLine("Cannot connect to database");
                    return false;
                }

                // Kiểm tra xem các bảng có tồn tại không
                var hasCustomers = await _context.Khachhangs.AnyAsync();
                var hasProducts = await _context.Sanphams.AnyAsync();
                
                System.Diagnostics.Debug.WriteLine($"Database connection OK. Has customers: {hasCustomers}, Has products: {hasProducts}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database connection error: {ex.Message}");
                return false;
            }
        }

        private async Task CreateSampleProducts()
        {
            try
            {
                var sampleProducts = new List<Sanpham>
                {
                    new Sanpham
                    {
                        MaSp = "SP001",
                        TenSp = "Bánh mì thịt nướng",
                        MoTa = "Bánh mì thịt nướng thơm ngon",
                        Gia = 25000,
                        Dvtsp = "Cái",
                        Loai = "Bánh mì"
                    },
                    new Sanpham
                    {
                        MaSp = "SP002",
                        TenSp = "Phở bò",
                        MoTa = "Phở bò truyền thống",
                        Gia = 45000,
                        Dvtsp = "Tô",
                        Loai = "Phở"
                    },
                    new Sanpham
                    {
                        MaSp = "SP003",
                        TenSp = "Cà phê sữa đá",
                        MoTa = "Cà phê sữa đá Việt Nam",
                        Gia = 15000,
                        Dvtsp = "Ly",
                        Loai = "Đồ uống"
                    },
                    new Sanpham
                    {
                        MaSp = "SP004",
                        TenSp = "Bún chả",
                        MoTa = "Bún chả Hà Nội",
                        Gia = 35000,
                        Dvtsp = "Phần",
                        Loai = "Bún"
                    },
                    new Sanpham
                    {
                        MaSp = "SP005",
                        TenSp = "Trà sữa trân châu",
                        MoTa = "Trà sữa trân châu đường đen",
                        Gia = 25000,
                        Dvtsp = "Ly",
                        Loai = "Đồ uống"
                    }
                };

                foreach (var product in sampleProducts)
                {
                    await _context.Sanphams.AddAsync(product);
                }
                await _context.SaveChangesAsync().ConfigureAwait(false);

                // Tạo dữ liệu tồn kho mẫu
                foreach (var product in sampleProducts)
                {
                    var inventory = new CtTon
                    {
                        MaSp = product.MaSp,
                        MaKho = "K01",
                        SoLuongTonKho = 100
                    };
                    await _context.CtTons.AddAsync(inventory);
                }
                await _context.SaveChangesAsync().ConfigureAwait(false);

                // Reload products
                _allProducts = await _productRepository.GetAllProductsAsync().ConfigureAwait(false);
                cboProduct.DataSource = _allProducts;

                // Reload customer combobox nếu cần
                if (_allCustomers?.Count == 0)
                {
                    await CreateSampleCustomers();
                }

                MessageBox.Show("Đã tạo dữ liệu mẫu cho sản phẩm và tồn kho!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating sample products: {ex.Message}");
                MessageBox.Show($"Lỗi tạo dữ liệu mẫu sản phẩm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void SetupFormForOrder()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"SetupFormForOrder called - _isEditMode: {_isEditMode}, _isViewMode: {_isViewMode}, _editingOrder: {_editingOrder?.MaDdh}");
                
                // Kiểm tra kết nối database
                if (!await CheckDatabaseConnection())
                {
                    MessageBox.Show("Không thể kết nối đến database. Vui lòng kiểm tra kết nối!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Load customers and products
                System.Diagnostics.Debug.WriteLine("Loading customers and products...");
                _allCustomers = await _customerRepository.GetAllCustomersAsync().ConfigureAwait(false);
                _allProducts = await _productRepository.GetAllProductsAsync().ConfigureAwait(false);

                // Debug: Kiểm tra dữ liệu
                System.Diagnostics.Debug.WriteLine($"Loaded {_allCustomers?.Count ?? 0} customers");
                System.Diagnostics.Debug.WriteLine($"Loaded {_allProducts?.Count ?? 0} products");

                // Hiển thị thông tin dữ liệu
                ShowDataInfo();

                // Kiểm tra dữ liệu trước khi setup
                if (_allCustomers == null || _allCustomers.Count == 0)
                {
                    MessageBox.Show("Không thể load danh sách khách hàng. Vui lòng kiểm tra database!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _allCustomers = new List<Khachhang>();

                    // Tạo dữ liệu mẫu cho khách hàng
                    await CreateSampleCustomers();
                }

                if (_allProducts == null || _allProducts.Count == 0)
                {
                    MessageBox.Show("Không thể load danh sách sản phẩm. Vui lòng kiểm tra database!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _allProducts = new List<Sanpham>();

                    // Tạo dữ liệu mẫu cho sản phẩm
                    await CreateSampleProducts();
                }

                // Setup customer combo box
                System.Diagnostics.Debug.WriteLine("Setting up customer combo box...");
                cboCustomer.DataSource = _allCustomers;
                cboCustomer.DisplayMember = "TenKh";
                cboCustomer.ValueMember = "MaKh";
                System.Diagnostics.Debug.WriteLine($"Customer combo box setup complete - Items count: {cboCustomer.Items.Count}");

                // Setup product combo box
                System.Diagnostics.Debug.WriteLine("Setting up product combo box...");
                cboProduct.DataSource = _allProducts;
                cboProduct.DisplayMember = "TenSp";
                cboProduct.ValueMember = "MaSp";
                System.Diagnostics.Debug.WriteLine($"Product combo box setup complete - Items count: {cboProduct.Items.Count}");

                // Add stock information to product display
                cboProduct.Format += FormatProductDisplay;

                // Setup DataGridView
                System.Diagnostics.Debug.WriteLine("Setting up DataGridView...");
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

                System.Diagnostics.Debug.WriteLine($"DataGridView setup complete - Columns count: {dgvOrderItems.Columns.Count}");

                // Set default dates
                System.Diagnostics.Debug.WriteLine("Setting up default dates...");
                dtpOrderDate.Value = DateTime.Now;
                dtpDeliveryDate.Value = DateTime.Now.AddDays(7);
                System.Diagnostics.Debug.WriteLine($"Default dates set - Order: {dtpOrderDate.Value:dd/MM/yyyy}, Delivery: {dtpDeliveryDate.Value:dd/MM/yyyy}");

                // Set default status
                System.Diagnostics.Debug.WriteLine("Setting up status combo box...");
                cboStatus.Items.Clear();
                cboStatus.Items.AddRange(new object[] { "Chờ xử lý", "Đang xử lý", "Đã giao", "Đã hủy" });
                cboStatus.SelectedItem = "Chờ xử lý";
                System.Diagnostics.Debug.WriteLine($"Status combo box setup complete - Selected: {cboStatus.SelectedItem}");

                // Set form title based on mode
                System.Diagnostics.Debug.WriteLine("Setting form title...");
                if (_isViewMode)
                {
                    this.Text = "Xem chi tiết đơn hàng";
                    System.Diagnostics.Debug.WriteLine("Form title set to: Xem chi tiết đơn hàng");
                }
                else if (_isEditMode)
                {
                    this.Text = "Chỉnh sửa đơn hàng";
                    System.Diagnostics.Debug.WriteLine("Form title set to: Chỉnh sửa đơn hàng");
                }
                else
                {
                    this.Text = "Thêm đơn hàng mới";
                    System.Diagnostics.Debug.WriteLine("Form title set to: Thêm đơn hàng mới");
                }

                UpdateTotalAmount();

                // Generate order ID cho trường hợp tạo mới
                if (!_isEditMode && !_isViewMode)
                {
                    _currentOrderId = await _orderRepository.GenerateOrderIdAsync().ConfigureAwait(false);
                    txtOrderId.Text = _currentOrderId;
                }

                // Bây giờ mới load dữ liệu đơn hàng
                if (_editingOrder != null || (!string.IsNullOrEmpty(_currentOrderId) && _isEditMode))
                {
                    System.Diagnostics.Debug.WriteLine("Calling LoadOrderForEdit...");
                    LoadOrderForEdit();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Not calling LoadOrderForEdit - conditions not met");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khởi tạo form: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
