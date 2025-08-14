using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AceCook.Models;
using AceCook.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AceCook
{
    public partial class OrderAddEditForm : Form
    {
        private  AppDbContext _context;
        private  OrderRepository _orderRepository;
        private  CustomerRepository _customerRepository;
        private  ProductRepository _productRepository;
        private  InventoryRepository _inventoryRepository;

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
            InitializeRepositories();
            SetupForm();
        }

        public OrderAddEditForm(Dondathang order, bool isViewMode = false) : this()
        {
            _isEditMode = !isViewMode;
            _isViewMode = isViewMode;
            _editingOrder = order;
            _currentOrderId = order?.MaDdh;
            LoadOrderForEdit();
        }

        private void InitializeRepositories()
        {
            _context = new AppDbContext();
            _orderRepository = new OrderRepository(_context);
            _customerRepository = new CustomerRepository(_context);
            _productRepository = new ProductRepository(_context);
            _inventoryRepository = new InventoryRepository(_context);
            _orderItems = new List<OrderItem>();
        }

        private void SetupForm()
        {
            SetupComboBoxes();
            SetupDataGridView();
            SetupEventHandlers();
            SetDefaultValues();
            UpdateFormTitle();
        }

        private async void SetupComboBoxes()
        {
            try
            {
                _allCustomers = await _customerRepository.GetAllCustomersAsync();
                _allProducts = await _productRepository.GetAllProductsAsync();

                if (_allCustomers.Count == 0) await CreateSampleCustomers();
                if (_allProducts.Count == 0) await CreateSampleProducts();

                cboCustomer.DataSource = _allCustomers;
                cboCustomer.DisplayMember = "TenKh";
                cboCustomer.ValueMember = "MaKh";

                cboProduct.DataSource = _allProducts;
                cboProduct.DisplayMember = "TenSp";
                cboProduct.ValueMember = "MaSp";
                cboProduct.Format += FormatProductDisplay;

                cboStatus.Items.AddRange(new object[] { "Chờ xử lý", "Đang xử lý", "Đã giao", "Đã hủy" });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khởi tạo dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupDataGridView()
        {
            dgvOrderItems.AutoGenerateColumns = false;
            dgvOrderItems.Columns.Clear();

            var columns = new[]
            {
                new DataGridViewTextBoxColumn { Name = "ProductId", HeaderText = "Mã SP", DataPropertyName = "ProductId", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "ProductName", HeaderText = "Tên Sản Phẩm", DataPropertyName = "ProductName", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "Quantity", HeaderText = "Số Lượng", DataPropertyName = "Quantity", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "UnitPrice", HeaderText = "Đơn Giá", DataPropertyName = "UnitPrice", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "TotalPrice", HeaderText = "Thành Tiền", DataPropertyName = "TotalPrice", Width = 120 }
            };

            dgvOrderItems.Columns.AddRange(columns);
        }

        private void SetupEventHandlers()
        {
            cboProduct.SelectedIndexChanged += CboProduct_SelectedIndexChanged;
            dgvOrderItems.CellEndEdit += DgvOrderItems_CellEndEdit;
        }

        private void SetDefaultValues()
        {
            dtpOrderDate.Value = DateTime.Now;
            dtpDeliveryDate.Value = DateTime.Now.AddDays(7);
            cboStatus.SelectedItem = "Chờ xử lý";
            numQuantity.Value = 1;
        }

        private void UpdateFormTitle()
        {
            this.Text = _isViewMode ? "Xem chi tiết đơn hàng" : 
                       _isEditMode ? "Chỉnh sửa đơn hàng" : "Thêm đơn hàng mới";
            
            if (!string.IsNullOrEmpty(_currentOrderId))
                this.Text += $" - {_currentOrderId}";
        }

        private void DisableControlsForViewMode()
        {
            var controls = new Control[] { cboCustomer, dtpOrderDate, dtpDeliveryDate, cboStatus, cboProduct, numQuantity, btnAddProduct, btnRemoveProduct };
            foreach (var control in controls) control.Enabled = false;

            btnSave.Visible = false;
            btnCancel.Text = "Đóng";
            dgvOrderItems.ReadOnly = true;
        }

        private async void LoadOrderForEdit()
        {
            try
            {
                // Nếu có editingOrder, load dữ liệu trực tiếp
                if (_editingOrder != null)
                {
                    LoadOrderData();
                    LoadOrderItems();
                    RefreshOrderItemsGrid();
                    UpdateTotalAmount();
                }
                // Nếu là edit mode nhưng chưa có editingOrder, load từ database
                else if (_isEditMode && !string.IsNullOrEmpty(_currentOrderId))
                {
                    _editingOrder = await _orderRepository.GetOrderByIdAsync(_currentOrderId);
                    if (_editingOrder != null)
                    {
                        LoadOrderData();
                        LoadOrderItems();
                        RefreshOrderItemsGrid();
                        UpdateTotalAmount();
                    }
                }

                // Xử lý view mode và tạo mới
                if (_isViewMode)
                {
                    DisableControlsForViewMode();
                }
                else if (!_isEditMode)
                {
                    // Tạo mới - generate order ID
                    _currentOrderId = await _orderRepository.GenerateOrderIdAsync();
                    txtOrderId.Text = _currentOrderId;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải đơn hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadOrderData()
        {
            txtOrderId.Text = _editingOrder.MaDdh;
            cboCustomer.SelectedValue = _editingOrder.MaKh;
            dtpOrderDate.Value = _editingOrder.NgayDat?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Now;
            dtpDeliveryDate.Value = _editingOrder.NgayGiao?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Now.AddDays(7);
            cboStatus.SelectedItem = _editingOrder.TrangThai;
        }

        private void LoadOrderItems()
        {
            _orderItems.Clear();
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
                        UnitPrice = (double)(ct.DonGia ?? 0),
                        TotalPrice = (ct.SoLuong ?? 0) * (double)(ct.DonGia ?? 0)
                    });
                }
            }
        }

        private async void btnAddProduct_Click(object sender, EventArgs e)
        {
            if (_isViewMode) return;

            if (!ValidateProductSelection()) return;

            var selectedProduct = cboProduct.SelectedItem as Sanpham;
            var quantity = (int)numQuantity.Value;

            if (!await ValidateStock(selectedProduct.MaSp, quantity)) return;

            AddOrUpdateOrderItem(selectedProduct, quantity);
            RefreshOrderItemsGrid();
            UpdateTotalAmount();
            ClearProductSelection();
        }

        private bool ValidateProductSelection()
        {
            if (cboProduct.SelectedValue == null || numQuantity.Value <= 0)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm và nhập số lượng hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private async Task<bool> ValidateStock(string productId, int quantity)
        {
            var availableStock = await GetAvailableStock(productId);
            if (quantity > availableStock)
            {
                MessageBox.Show($"Số lượng vượt quá tồn kho! Tồn kho hiện tại: {availableStock}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void AddOrUpdateOrderItem(Sanpham product, int quantity)
        {
            var existingItem = _orderItems.FirstOrDefault(item => item.ProductId == product.MaSp);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.TotalPrice = existingItem.Quantity * existingItem.UnitPrice;
            }
            else
            {
                _orderItems.Add(new OrderItem
                {
                    ProductId = product.MaSp,
                    ProductName = product.TenSp ?? "",
                    Quantity = quantity,
                    UnitPrice = (double)(product.Gia ?? 0),
                    TotalPrice = quantity * (double)(product.Gia ?? 0)
                });
            }
        }

        private void btnRemoveProduct_Click(object sender, EventArgs e)
        {
            if (_isViewMode) return;

            if (dgvOrderItems.SelectedRows.Count > 0)
            {
                var productId = dgvOrderItems.SelectedRows[0].Cells["ProductId"].Value?.ToString();
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
            if (_isViewMode || e.ColumnIndex != dgvOrderItems.Columns["Quantity"].Index || e.RowIndex < 0) return;

            var row = dgvOrderItems.Rows[e.RowIndex];
            var productId = row.Cells["ProductId"].Value?.ToString();
            var newQuantity = Convert.ToInt32(row.Cells["Quantity"].Value ?? 0);

            if (!string.IsNullOrEmpty(productId) && newQuantity > 0)
            {
                if (!await ValidateStock(productId, newQuantity))
                {
                    var availableStock = await GetAvailableStock(productId);
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

        private async void CboProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isViewMode || cboProduct.SelectedValue == null) return;

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

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (_isViewMode || !ValidateForm()) return;

            try
            {
                btnSave.Enabled = false;
                btnSave.Text = "Đang lưu...";

                if (_isEditMode)
                    await UpdateExistingOrder();
                else
                    await CreateNewOrder();

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
                var order = new Dondathang
                {
                    MaDdh = _currentOrderId,
                    MaKh = cboCustomer.SelectedValue?.ToString(),
                    MaNv = "NV001",
                    NgayDat = DateOnly.FromDateTime(dtpOrderDate.Value),
                    NgayGiao = DateOnly.FromDateTime(dtpDeliveryDate.Value),
                    TrangThai = cboStatus.Text,
                    CtDhs = _orderItems.Select(item => new CtDh
                    {
                        MaDdh = _currentOrderId,
                        MaSp = item.ProductId,
                        SoLuong = item.Quantity,
                        DonGia = item.UnitPrice
                    }).ToList()
                };

                await _context.Dondathangs.AddAsync(order);
                await _context.SaveChangesAsync();

                foreach (var item in _orderItems)
                {
                    await UpdateInventory(item.ProductId, item.Quantity);
                }

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
                _editingOrder.MaKh = cboCustomer.SelectedValue?.ToString();
                _editingOrder.NgayDat = DateOnly.FromDateTime(dtpOrderDate.Value);
                _editingOrder.NgayGiao = DateOnly.FromDateTime(dtpDeliveryDate.Value);
                _editingOrder.TrangThai = cboStatus.Text;

                _context.CtDhs.RemoveRange(_editingOrder.CtDhs);
                _editingOrder.CtDhs = _orderItems.Select(item => new CtDh
                {
                    MaDdh = _editingOrder.MaDdh,
                    MaSp = item.ProductId,
                    SoLuong = item.Quantity,
                    DonGia = item.UnitPrice
                }).ToList();

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
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
                    .FirstOrDefaultAsync(ct => ct.MaSp == productId && ct.MaKho == "K01");
                return inventory?.SoLuongTonKho ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        private async Task UpdateInventory(string productId, int quantity)
        {
            var inventory = await _context.CtTons
                .FirstOrDefaultAsync(ct => ct.MaSp == productId && ct.MaKho == "K01");

            if (inventory != null)
            {
                inventory.SoLuongTonKho = Math.Max(0, (inventory.SoLuongTonKho ?? 0) - quantity);
            }
            else
            {
                inventory = new CtTon { MaSp = productId, MaKho = "K01", SoLuongTonKho = 0 };
                await _context.CtTons.AddAsync(inventory);
            }
        }

        private async Task CreateSampleCustomers()
        {
            var sampleCustomers = new List<Khachhang>
            {
                new Khachhang { MaKh = "KH001", TenKh = "Nguyễn Văn An", LoaiKh = "Cá nhân", Sdtkh = "0123456789", DiaChiKh = "123 Đường ABC, Quận 1, TP.HCM", EmailKh = "nguyenvanan@email.com" },
                new Khachhang { MaKh = "KH002", TenKh = "Trần Thị Bình", LoaiKh = "Cá nhân", Sdtkh = "0987654321", DiaChiKh = "456 Đường XYZ, Quận 2, TP.HCM", EmailKh = "tranthibinh@email.com" },
                new Khachhang { MaKh = "KH003", TenKh = "Công ty TNHH Minh Phát", LoaiKh = "Doanh nghiệp", Sdtkh = "0281234567", DiaChiKh = "789 Đường DEF, Quận 3, TP.HCM", EmailKh = "info@minhphat.com" }
            };

            foreach (var customer in sampleCustomers)
                await _context.Khachhangs.AddAsync(customer);
            
            await _context.SaveChangesAsync();
            _allCustomers = await _customerRepository.GetAllCustomersAsync();
            cboCustomer.DataSource = _allCustomers;
        }

        private async Task CreateSampleProducts()
        {
            var sampleProducts = new List<Sanpham>
            {
                new Sanpham { MaSp = "SP001", TenSp = "Bánh mì thịt nướng", MoTa = "Bánh mì thịt nướng thơm ngon", Gia = 25000, Dvtsp = "Cái", Loai = "Bánh mì" },
                new Sanpham { MaSp = "SP002", TenSp = "Phở bò", MoTa = "Phở bò truyền thống", Gia = 45000, Dvtsp = "Tô", Loai = "Phở" },
                new Sanpham { MaSp = "SP003", TenSp = "Cà phê sữa đá", MoTa = "Cà phê sữa đá Việt Nam", Gia = 15000, Dvtsp = "Ly", Loai = "Đồ uống" },
                new Sanpham { MaSp = "SP004", TenSp = "Bún chả", MoTa = "Bún chả Hà Nội", Gia = 35000, Dvtsp = "Phần", Loai = "Bún" },
                new Sanpham { MaSp = "SP005", TenSp = "Trà sữa trân châu", MoTa = "Trà sữa trân châu đường đen", Gia = 25000, Dvtsp = "Ly", Loai = "Đồ uống" }
            };

            foreach (var product in sampleProducts)
                await _context.Sanphams.AddAsync(product);
            
            await _context.SaveChangesAsync();

            foreach (var product in sampleProducts)
            {
                var inventory = new CtTon { MaSp = product.MaSp, MaKho = "K01", SoLuongTonKho = 100 };
                await _context.CtTons.AddAsync(inventory);
            }
            
            await _context.SaveChangesAsync();
            _allProducts = await _productRepository.GetAllProductsAsync();
            cboProduct.DataSource = _allProducts;
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
