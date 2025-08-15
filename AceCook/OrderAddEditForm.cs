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
        private AppDbContext _context;
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
        private string _currentUserMaNv; // Thêm field để lưu MaNv của user đang đăng nhập
        private Nhanvien _currentEmployee; // Thêm field để lưu thông tin nhân viên

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
        }

        // Constructor mới để nhận thông tin nhân viên
        public OrderAddEditForm(Nhanvien currentEmployee) : this()
        {
            _currentEmployee = currentEmployee;
            _currentUserMaNv = currentEmployee?.MaNv ?? "NV001";
            _isEditMode = false; // Đảm bảo đây là chế độ tạo mới
            _isViewMode = false;
            SetupEmployeeInfo();
        }

        public OrderAddEditForm(Dondathang order, Nhanvien currentEmployee, bool isViewMode = false) : this()
        {
            _currentEmployee = currentEmployee;
            _currentUserMaNv = currentEmployee?.MaNv ?? "NV001";
            _isEditMode = !isViewMode;
            _isViewMode = isViewMode;
            _editingOrder = order;
            _currentOrderId = order?.MaDdh;
            SetupEmployeeInfo();
            if (_isViewMode)
            {
                grpProductSelection.Visible = false;
            }
            lbStatus.Text = "Hoàn thành";
            lbStatus.ForeColor = Color.Green;
            lbStatus.Font = new Font("Segoe UI", 10, FontStyle.Bold);
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

        private async void SetupForm()
        {
            await SetupComboBoxes();
            UpdateFormTitle();
            SetupActionControls();

            // Khởi tạo hiển thị tồn kho
            ResetInventoryDisplay();

            // Thiết lập thông tin nhân viên nếu có
            if (_currentEmployee != null)
            {
                SetupEmployeeInfo();
            }

            // Setup DataGridView ngay từ đầu
            SetupOrderItemsDataGridView();

            // Debug controls trong form
            DebugFormControls();

            // Kiểm tra và sửa chữa lblTotalAmount
            ValidateAndFixTotalAmountLabel();

            // Kiểm tra visibility của lblTotalAmount
            CheckTotalAmountVisibility();

            // Load dữ liệu đơn hàng sau khi đã setup xong ComboBoxes
            LoadOrderForEdit();

            // Thêm event handler để đảm bảo mã đơn hàng được sinh khi form được hiển thị
            this.Load += OrderAddEditForm_Load;
        }

        private async void OrderAddEditForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Nếu là chế độ tạo mới và chưa có mã đơn hàng, tự động sinh mã
                if (!_isEditMode && !_isViewMode && string.IsNullOrEmpty(_currentOrderId))
                {
                    _currentOrderId = await _orderRepository.GenerateOrderIdAsync();
                    if (txtOrderId != null)
                    {
                        txtOrderId.Text = _currentOrderId;
                        System.Diagnostics.Debug.WriteLine($"Generated new order ID: {_currentOrderId}");
                    }
                }

                // Force update tổng tiền sau khi form load
                System.Diagnostics.Debug.WriteLine("Form loaded, updating total amount...");
                UpdateTotalAmount();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OrderAddEditForm_Load: {ex.Message}");
            }
        }

        private void SetupEmployeeInfo()
        {
            try
            {
                if (_currentEmployee != null && txtTenNv != null)
                {
                    // Hiển thị tên nhân viên trong txtTenNv
                    txtTenNv.Text = _currentEmployee.HoTenNv ?? "Không xác định";
                    txtTenNv.ReadOnly = true; // Không cho phép chỉnh sửa

                    System.Diagnostics.Debug.WriteLine($"Employee info set: {_currentEmployee.HoTenNv} ({_currentEmployee.MaNv})");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Employee info not available or txtTenNv is null");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting up employee info: {ex.Message}");
            }
        }

        private void SetupActionControls()
        {
            try
            {
                // Sử dụng lblTotalAmount đã có sẵn từ Designer
                if (lblTotalAmount != null)
                {
                    // Cập nhật style cho lblTotalAmount
                    lblTotalAmount.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                    lblTotalAmount.ForeColor = Color.FromArgb(46, 204, 113);
                    lblTotalAmount.Text = "Tổng tiền: 0 VNĐ";

                    System.Diagnostics.Debug.WriteLine("Using existing lblTotalAmount from Designer");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: lblTotalAmount is null in Designer");
                }

                // Panel cho nút hành động (không cần panel tổng tiền nữa)
                // var pnlActions = new Panel
                // {
                //     Dock = DockStyle.Bottom,
                //     Height = 80,
                //     BackColor = Color.FromArgb(248, 249, 250),
                //     BorderStyle = BorderStyle.FixedSingle,
                //     Padding = new Padding(15)
                // };

                // // Nút Lưu
                // btnSave = new Button
                // {
                //     Text = "💾 Lưu đơn hàng",
                //     Location = new Point(400, 20),
                //     Size = new Size(150, 40),
                //     Font = new Font("Segoe UI", 10, FontStyle.Bold),
                //     BackColor = Color.FromArgb(46, 204, 113),
                //     ForeColor = Color.White,
                //     FlatStyle = FlatStyle.Flat,
                //     Cursor = Cursors.Hand
                // };
                // btnSave.FlatAppearance.BorderSize = 0;

                // // Nút Hủy/Đóng
                // btnCancel = new Button
                // {
                //     Text = "❌ Hủy",
                //     Location = new Point(570, 20),
                //     Size = new Size(120, 40),
                //     Font = new Font("Segoe UI", 10, FontStyle.Bold),
                //     BackColor = Color.FromArgb(231, 76, 60),
                //     ForeColor = Color.White,
                //     FlatStyle = FlatStyle.Flat,
                //     Cursor = Cursors.Hand
                // };
                // btnCancel.FlatAppearance.BorderSize = 0;

                // // Thêm controls vào panel
                // pnlActions.Controls.AddRange(new Control[]
                // {
                //     btnSave, btnCancel
                // });

                // // Thêm panel vào form
                // this.Controls.Add(pnlActions);

                // // Gắn event handlers
                // btnSave.Click += btnSave_Click;
                // btnCancel.Click += btnCancel_Click;

                // System.Diagnostics.Debug.WriteLine("Action controls setup completed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting up action controls: {ex.Message}");
                MessageBox.Show($"Lỗi khi thiết lập controls hành động: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task SetupComboBoxes()
        {
            try
            {
                _allCustomers = await _customerRepository.GetAllCustomersAsync();
                _allProducts = await _productRepository.GetAllProductsAsync();

                cboCustomer.DataSource = _allCustomers;
                cboCustomer.DisplayMember = "TenKh";
                cboCustomer.ValueMember = "MaKh";

                cboProduct.DataSource = _allProducts;
                cboProduct.DisplayMember = "TenSp";
                cboProduct.ValueMember = "MaSp";
                cboProduct.Format += FormatProductDisplay;

                // Thêm event handler để hiển thị tồn kho khi chọn sản phẩm
                cboProduct.SelectedIndexChanged += CboProduct_SelectedIndexChanged;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khởi tạo dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void UpdateFormTitle()
        {
            this.Text = _isViewMode ? "Xem chi tiết đơn hàng" :
                       _isEditMode ? "Chỉnh sửa đơn hàng" : "Thêm đơn hàng mới";

            if (!string.IsNullOrEmpty(_currentOrderId))
                this.Text += $" - {_currentOrderId}";

            // Nếu ở view mode, hiển thị thêm thông tin khách hàng và ngày
            if (_isViewMode && _editingOrder != null)
            {
                var customerName = _editingOrder.MaKhNavigation?.TenKh ?? "Không xác định";
                var orderDate = _editingOrder.NgayDat?.ToString("dd/MM/yyyy") ?? "N/A";
                this.Text += $" | KH: {customerName} | Ngày: {orderDate}";
            }
        }

        private void DisableControlsForViewMode()
        {
            try
            {
                // Disable tất cả controls input (không cần cboStatus nữa)
                var controls = new Control[] { cboCustomer, dtpOrderDate, dtpDeliveryDate, cboProduct, numQuantity, btnAddProduct, btnRemoveProduct, txtInventory };
                foreach (var control in controls)
                {
                    if (control != null) control.Enabled = false;
                }

                // Ẩn nút Save và đổi text nút Cancel
                if (btnSave != null) btnSave.Visible = false;
                if (btnCancel != null) btnCancel.Text = "Đóng";

                // Đảm bảo DataGridView không thể chỉnh sửa
                if (dgvOrderItems != null)
                {
                    dgvOrderItems.ReadOnly = true;
                    dgvOrderItems.AllowUserToAddRows = false;
                    dgvOrderItems.AllowUserToDeleteRows = false;
                    dgvOrderItems.AllowUserToOrderColumns = false;
                    dgvOrderItems.AllowUserToResizeRows = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error disabling controls for view mode: {ex.Message}");
            }
        }

        private async void LoadOrderForEdit()
        {
            try
            {
                // Nếu có editingOrder, load dữ liệu trực tiếp
                if (_editingOrder != null)
                {
                    // Kiểm tra trạng thái đơn hàng
                    if (_isEditMode && (_editingOrder.TrangThai == "Hoàn thành" || _editingOrder.TrangThai == "Đã giao"))
                    {
                        MessageBox.Show($"Không thể chỉnh sửa đơn hàng có trạng thái '{_editingOrder.TrangThai}'!\n\n" +
                                      "Chỉ có thể xem chi tiết đơn hàng này.",
                                      "Không thể chỉnh sửa",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Warning);

                        // Chuyển sang view mode
                        _isEditMode = false;
                        _isViewMode = true;
                        UpdateFormTitle();
                    }

                    LoadOrderData();
                    LoadOrderItems();
                    RefreshOrderItemsGrid();

                    // Cập nhật tổng tiền ngay lập tức
                    UpdateTotalAmount();

                    System.Diagnostics.Debug.WriteLine($"Order loaded successfully. Items: {_orderItems.Count}, Total: {_orderItems.Sum(item => item.TotalPrice):N0}");
                }
                // Nếu là edit mode nhưng chưa có editingOrder, load từ database
                else if (_isEditMode && !string.IsNullOrEmpty(_currentOrderId))
                {
                    _editingOrder = await _orderRepository.GetOrderByIdAsync(_currentOrderId);
                    if (_editingOrder != null)
                    {
                        // Kiểm tra trạng thái đơn hàng
                        if (_editingOrder.TrangThai == "Hoàn thành" || _editingOrder.TrangThai == "Đã giao")
                        {
                            MessageBox.Show($"Không thể chỉnh sửa đơn hàng có trạng thái '{_editingOrder.TrangThai}'!\n\n" +
                                          "Chỉ có thể xem chi tiết đơn hàng này.",
                                          "Không thể chỉnh sửa",
                                          MessageBoxButtons.OK,
                                          MessageBoxIcon.Warning);

                            // Chuyển sang view mode
                            _isEditMode = false;
                            _isViewMode = true;
                            UpdateFormTitle();
                        }
                        else
                        {
                            MessageBox.Show($"Đang chỉnh sửa đơn hàng: {_editingOrder.MaDdh}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        LoadOrderData();
                        LoadOrderItems();
                        RefreshOrderItemsGrid();

                        // Cập nhật tổng tiền
                        UpdateTotalAmount();

                        System.Diagnostics.Debug.WriteLine($"Order loaded from database. Items: {_orderItems.Count}, Total: {_orderItems.Sum(item => item.TotalPrice):N0}");
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

                    // Đảm bảo thông tin nhân viên được hiển thị
                    SetupEmployeeInfo();

                    System.Diagnostics.Debug.WriteLine($"New order mode - Generated ID: {_currentOrderId}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải đơn hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadOrderData()
        {
            try
            {
                txtOrderId.Text = _editingOrder.MaDdh;

                // Đảm bảo ComboBox đã có dữ liệu trước khi set SelectedValue
                if (cboCustomer.DataSource != null && _editingOrder.MaKh != null)
                {
                    cboCustomer.SelectedValue = _editingOrder.MaKh;
                    System.Diagnostics.Debug.WriteLine($"Set customer ComboBox to: {_editingOrder.MaKh}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Cannot set customer ComboBox - DataSource: {cboCustomer.DataSource != null}, MaKh: {_editingOrder.MaKh}");
                }

                dtpOrderDate.Value = _editingOrder.NgayDat?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Now;
                dtpDeliveryDate.Value = _editingOrder.NgayGiao?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Now.AddDays(7);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadOrderData: {ex.Message}");
            }
        }

        private void LoadOrderItems()
        {
            try
            {
                _orderItems.Clear();

                // Kiểm tra xem có dữ liệu chi tiết đơn hàng không
                if (_editingOrder?.CtDhs == null || !_editingOrder.CtDhs.Any())
                {
                    System.Diagnostics.Debug.WriteLine("Không có chi tiết đơn hàng để load");
                    if (_isViewMode)
                    {
                        MessageBox.Show("Đơn hàng này không có sản phẩm nào!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Loading {_editingOrder.CtDhs.Count} order items");

                foreach (var ct in _editingOrder.CtDhs)
                {
                    try
                    {
                        // Tìm sản phẩm tương ứng
                        var product = _allProducts?.FirstOrDefault(p => p.MaSp == ct.MaSp);

                        if (product != null)
                        {
                            var orderItem = new OrderItem
                            {
                                ProductId = ct.MaSp ?? "",
                                ProductName = product.TenSp ?? "Không xác định",
                                Quantity = ct.SoLuong ?? 0,
                                UnitPrice = (double)(ct.DonGia ?? 0),
                                TotalPrice = (ct.SoLuong ?? 0) * (double)(ct.DonGia ?? 0)
                            };

                            _orderItems.Add(orderItem);
                            System.Diagnostics.Debug.WriteLine($"Added item: {orderItem.ProductName} - Qty: {orderItem.Quantity} - Price: {orderItem.UnitPrice:N0} - Total: {orderItem.TotalPrice:N0}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Product not found for MaSp: {ct.MaSp}");
                            // Tạo item với thông tin cơ bản nếu không tìm thấy sản phẩm
                            var orderItem = new OrderItem
                            {
                                ProductId = ct.MaSp ?? "",
                                ProductName = $"Sản phẩm {ct.MaSp}",
                                Quantity = ct.SoLuong ?? 0,
                                UnitPrice = (double)(ct.DonGia ?? 0),
                                TotalPrice = (ct.SoLuong ?? 0) * (double)(ct.DonGia ?? 0)
                            };
                            _orderItems.Add(orderItem);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error processing order item {ct.MaSp}: {ex.Message}");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Successfully loaded {_orderItems.Count} order items");
                System.Diagnostics.Debug.WriteLine($"Total amount from loaded items: {_orderItems.Sum(item => item.TotalPrice):N0} VNĐ");

                // Hiển thị thông báo nếu load thành công và ở view mode
                if (_isViewMode && _orderItems.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"View mode: Loaded {_orderItems.Count} items successfully");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadOrderItems: {ex.Message}");
                MessageBox.Show($"Lỗi khi tải danh sách sản phẩm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnAddProduct_Click(object sender, EventArgs e)
        {
            if (_isViewMode) return;

            try
            {
                if (!ValidateProductSelection()) return;

                var selectedProduct = cboProduct.SelectedItem as Sanpham;
                var quantity = (int)numQuantity.Value;

                // Kiểm tra tồn kho trước khi thêm
                var availableStock = await GetAvailableStock(selectedProduct.MaSp);

                if (availableStock <= 0)
                {
                    MessageBox.Show($"Sản phẩm '{selectedProduct.TenSp}' hiện không có trong kho!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (quantity > availableStock)
                {
                    var result = MessageBox.Show(
                        $"Số lượng vượt quá tồn kho!\n\n" +
                        $"Tồn kho hiện tại: {availableStock}\n" +
                        $"Số lượng yêu cầu: {quantity}\n\n" +
                        $"Bạn có muốn đặt số lượng tối đa có thể ({availableStock}) không?",
                        "Cảnh báo tồn kho",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        quantity = availableStock;
                        numQuantity.Value = quantity;
                    }
                    else
                    {
                        return;
                    }
                }

                // Kiểm tra xem sản phẩm đã có trong đơn hàng chưa
                var existingItem = _orderItems.FirstOrDefault(item => item.ProductId == selectedProduct.MaSp);
                if (existingItem != null)
                {
                    var newTotalQuantity = existingItem.Quantity + quantity;
                    if (newTotalQuantity > availableStock)
                    {
                        MessageBox.Show($"Tổng số lượng vượt quá tồn kho!\n\n" +
                                      $"Đã có: {existingItem.Quantity}\n" +
                                      $"Thêm mới: {quantity}\n" +
                                      $"Tổng: {newTotalQuantity}\n" +
                                      $"Tồn kho: {availableStock}",
                                      "Cảnh báo tồn kho",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Thêm hoặc cập nhật sản phẩm
                AddOrUpdateOrderItem(selectedProduct, quantity);

                System.Diagnostics.Debug.WriteLine($"Product added/updated: {selectedProduct.TenSp}, Quantity: {quantity}, Total items: {_orderItems.Count}");

                RefreshOrderItemsGrid();

                // Cập nhật tổng tiền ngay lập tức
                UpdateTotalAmount();

                // Force refresh tổng tiền
                ForceRefreshTotalAmount();

                ClearProductSelection();

                // Hiển thị thông báo thành công
                MessageBox.Show($"Đã thêm '{selectedProduct.TenSp}' vào đơn hàng!\n" +
                              $"Số lượng: {quantity}\n" +
                              $"Tồn kho còn lại: {availableStock - quantity}",
                              "Thành công",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm sản phẩm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            try
            {
                var existingItem = _orderItems.FirstOrDefault(item => item.ProductId == product.MaSp);
                if (existingItem != null)
                {
                    // Cập nhật sản phẩm đã có
                    var oldTotal = existingItem.TotalPrice;
                    existingItem.Quantity += quantity;
                    existingItem.TotalPrice = existingItem.Quantity * existingItem.UnitPrice;

                    System.Diagnostics.Debug.WriteLine($"Updated existing item: {product.TenSp}, Old total: {oldTotal:N0}, New total: {existingItem.TotalPrice:N0}");
                }
                else
                {
                    // Thêm sản phẩm mới
                    var newItem = new OrderItem
                    {
                        ProductId = product.MaSp,
                        ProductName = product.TenSp ?? "",
                        Quantity = quantity,
                        UnitPrice = (double)(product.Gia ?? 0),
                        TotalPrice = quantity * (double)(product.Gia ?? 0)
                    };

                    _orderItems.Add(newItem);
                    System.Diagnostics.Debug.WriteLine($"Added new item: {product.TenSp}, Quantity: {quantity}, Unit price: {newItem.UnitPrice:N0}, Total: {newItem.TotalPrice:N0}");
                }

                System.Diagnostics.Debug.WriteLine($"Total order items: {_orderItems.Count}, Total amount: {_orderItems.Sum(item => item.TotalPrice):N0}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AddOrUpdateOrderItem: {ex.Message}");
                throw;
            }
        }

        private void btnRemoveProduct_Click(object sender, EventArgs e)
        {
            if (_isViewMode) return;

            try
            {
                if (dgvOrderItems.SelectedRows.Count > 0)
                {
                    var productId = dgvOrderItems.SelectedRows[0].Cells["ProductId"].Value?.ToString();
                    if (!string.IsNullOrEmpty(productId))
                    {
                        var removedItem = _orderItems.FirstOrDefault(item => item.ProductId == productId);
                        if (removedItem != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"Removing product: {removedItem.ProductName}, Quantity: {removedItem.Quantity}, Total: {removedItem.TotalPrice:N0}");
                        }

                        _orderItems.RemoveAll(item => item.ProductId == productId);

                        System.Diagnostics.Debug.WriteLine($"Product removed. Remaining items: {_orderItems.Count}");

                        RefreshOrderItemsGrid();

                        // Cập nhật tổng tiền sau khi xóa
                        UpdateTotalAmount();

                        // Force refresh tổng tiền
                        ForceRefreshTotalAmount();

                        MessageBox.Show("Đã xóa sản phẩm khỏi đơn hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa sản phẩm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshOrderItemsGrid()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Refreshing grid with {_orderItems.Count} items");

                // Clear và setup lại DataGridView với cột tùy chỉnh
                dgvOrderItems.DataSource = null;
                dgvOrderItems.Columns.Clear();

                // Setup DataGridView styling
                SetupOrderItemsDataGridView();

                // Tạo cột tùy chỉnh với tên tiếng Việt
                dgvOrderItems.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ProductId",
                    HeaderText = "Mã SP",
                    DataPropertyName = "ProductId",
                    Width = 80,
                    ReadOnly = true
                });

                dgvOrderItems.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ProductName",
                    HeaderText = "Tên sản phẩm",
                    DataPropertyName = "ProductName",
                    Width = 200,
                    ReadOnly = true
                });

                dgvOrderItems.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Quantity",
                    HeaderText = "Số lượng",
                    DataPropertyName = "Quantity",
                    Width = 80,
                    ReadOnly = _isViewMode
                });

                dgvOrderItems.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "UnitPrice",
                    HeaderText = "Đơn giá (VNĐ)",
                    DataPropertyName = "UnitPrice",
                    Width = 120,
                    ReadOnly = true,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }
                });

                dgvOrderItems.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalPrice",
                    HeaderText = "Thành tiền (VNĐ)",
                    DataPropertyName = "TotalPrice",
                    Width = 140,
                    ReadOnly = true,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }
                });

                // Set DataSource sau khi tạo cột
                dgvOrderItems.DataSource = _orderItems;

                // Đảm bảo DataGridView hiển thị đúng
                dgvOrderItems.Refresh();

                System.Diagnostics.Debug.WriteLine($"Grid refreshed successfully. Rows count: {dgvOrderItems.Rows.Count}");

                // Nếu ở view mode, đảm bảo không thể chỉnh sửa
                if (_isViewMode)
                {
                    dgvOrderItems.ReadOnly = true;
                    dgvOrderItems.AllowUserToAddRows = false;
                    dgvOrderItems.AllowUserToDeleteRows = false;
                }

                // Cập nhật thông tin tồn kho cho từng sản phẩm
                UpdateStockInfoInGrid();

                // Cập nhật tổng tiền sau khi refresh grid
                UpdateTotalAmount();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing grid: {ex.Message}");
                MessageBox.Show($"Lỗi khi cập nhật bảng dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupOrderItemsDataGridView()
        {
            try
            {
                // Cấu hình cơ bản
                dgvOrderItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvOrderItems.AllowUserToAddRows = false;
                dgvOrderItems.AllowUserToDeleteRows = false;
                dgvOrderItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvOrderItems.MultiSelect = false;
                dgvOrderItems.BackgroundColor = Color.White;
                dgvOrderItems.BorderStyle = BorderStyle.FixedSingle;
                dgvOrderItems.GridColor = Color.LightGray;
                dgvOrderItems.RowHeadersVisible = false;
                dgvOrderItems.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                dgvOrderItems.ColumnHeadersHeight = 45;
                dgvOrderItems.RowTemplate.Height = 40;

                // Font và style cho header
                dgvOrderItems.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                dgvOrderItems.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
                dgvOrderItems.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvOrderItems.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // Font và style cho cells
                dgvOrderItems.DefaultCellStyle.Font = new Font("Segoe UI", 9);
                dgvOrderItems.DefaultCellStyle.BackColor = Color.White;
                dgvOrderItems.DefaultCellStyle.ForeColor = Color.Black;
                dgvOrderItems.DefaultCellStyle.SelectionBackColor = Color.LightGray;
                dgvOrderItems.DefaultCellStyle.SelectionForeColor = Color.Black;
                dgvOrderItems.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                // Alternating row colors để dễ đọc
                dgvOrderItems.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);

                // Thêm event handler cho việc chỉnh sửa số lượng (chỉ thêm một lần)
                if (!_isViewMode)
                {
                    // Xóa event handler cũ trước khi thêm mới để tránh duplicate
                    dgvOrderItems.CellEndEdit -= DgvOrderItems_CellEndEdit;
                    dgvOrderItems.CellEndEdit += DgvOrderItems_CellEndEdit;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting up DataGridView: {ex.Message}");
            }
        }

        private async void UpdateStockInfoInGrid()
        {
            try
            {
                for (int i = 0; i < dgvOrderItems.Rows.Count; i++)
                {
                    var row = dgvOrderItems.Rows[i];
                    var productId = row.Cells["ProductId"].Value?.ToString();

                    if (!string.IsNullOrEmpty(productId))
                    {
                        var availableStock = await GetAvailableStock(productId);
                        var currentQuantity = Convert.ToInt32(row.Cells["Quantity"].Value ?? 0);

                        // Thay đổi màu sắc dựa trên tồn kho
                        if (availableStock <= 0)
                        {
                            row.DefaultCellStyle.BackColor = Color.LightCoral;
                            row.DefaultCellStyle.ForeColor = Color.DarkRed;
                        }
                        else if (availableStock < currentQuantity)
                        {
                            row.DefaultCellStyle.BackColor = Color.LightYellow;
                            row.DefaultCellStyle.ForeColor = Color.DarkOrange;
                        }
                        else
                        {
                            // Sử dụng màu alternating rows
                            if (i % 2 == 0)
                            {
                                row.DefaultCellStyle.BackColor = Color.White;
                            }
                            else
                            {
                                row.DefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
                            }
                            row.DefaultCellStyle.ForeColor = Color.Black;
                        }

                        // Thêm tooltip để hiển thị thông tin tồn kho
                        if (availableStock < currentQuantity)
                        {
                            row.Cells["Quantity"].ToolTipText = $"Cảnh báo: Tồn kho hiện tại chỉ có {availableStock} sản phẩm!";
                        }
                        else
                        {
                            row.Cells["Quantity"].ToolTipText = $"Tồn kho hiện tại: {availableStock} sản phẩm";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating stock info in grid: {ex.Message}");
            }
        }

        private void UpdateTotalAmount()
        {
            try
            {
                var total = _orderItems.Sum(item => item.TotalPrice);
                System.Diagnostics.Debug.WriteLine($"UpdateTotalAmount called - Total: {total:N0} VNĐ, Items count: {_orderItems.Count}");
                System.Diagnostics.Debug.WriteLine($"Order items: {string.Join(", ", _orderItems.Select(item => $"{item.ProductName}: {item.Quantity}x{item.UnitPrice:N0}={item.TotalPrice:N0}"))}");

                if (lblTotalAmount != null)
                {
                    var oldText = lblTotalAmount.Text;
                    lblTotalAmount.Text = $"Tổng tiền: {total:N0} VNĐ";
                    System.Diagnostics.Debug.WriteLine($"lblTotalAmount updated successfully: '{oldText}' -> '{lblTotalAmount.Text}'");

                    // Kiểm tra xem control có hiển thị không
                    System.Diagnostics.Debug.WriteLine($"lblTotalAmount visible: {lblTotalAmount.Visible}, Enabled: {lblTotalAmount.Enabled}");
                    System.Diagnostics.Debug.WriteLine($"lblTotalAmount location: {lblTotalAmount.Location}, Size: {lblTotalAmount.Size}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: lblTotalAmount is null - cannot update total amount");

                    // Tìm control trong form
                    var foundLabel = this.Controls.Find("lblTotalAmount", true).FirstOrDefault() as Label;
                    if (foundLabel != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Found lblTotalAmount in Controls.Find: {foundLabel.Text}");
                        foundLabel.Text = $"Tổng tiền: {total:N0} VNĐ";
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("lblTotalAmount not found in Controls.Find");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating total amount: {ex.Message}");
                if (lblTotalAmount != null)
                {
                    lblTotalAmount.Text = "Tổng tiền: Lỗi tính toán";
                }
            }
        }

        /// <summary>
        /// Cập nhật tổng tiền từ hóa đơn bán (async)
        /// </summary>
        private async Task UpdateTotalAmountFromInvoiceAsync()
        {
            try
            {
                if (_isViewMode && !string.IsNullOrEmpty(_currentOrderId))
                {
                    var total = await _orderRepository.GetOrderTotalFromInvoiceAsync(_currentOrderId);
                    if (total > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Total amount from invoice: {total:N0} VNĐ");
                        if (lblTotalAmount != null)
                        {
                            lblTotalAmount.Text = $"Tổng tiền: {total:N0} VNĐ";
                        }
                    }
                    else
                    {
                        // Nếu không có hóa đơn, tính từ order items
                        UpdateTotalAmount();
                    }
                }
                else
                {
                    // Tính toán từ order items hiện tại
                    UpdateTotalAmount();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating total amount from invoice: {ex.Message}");
                // Fallback về tính toán từ order items
                UpdateTotalAmount();
            }
        }

        private void ClearProductSelection()
        {
            cboProduct.SelectedIndex = -1;
            numQuantity.Value = 1;
            ResetInventoryDisplay();
        }

        private void ResetInventoryDisplay()
        {
            txtInventory.Text = "Vui lòng chọn sản phẩm";
            txtInventory.ForeColor = Color.Gray;
            txtInventory.BackColor = SystemColors.Window;
            numQuantity.Maximum = 999999; // Reset về giá trị mặc định
        }

        private async void DgvOrderItems_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (_isViewMode || e.ColumnIndex != dgvOrderItems.Columns["Quantity"].Index || e.RowIndex < 0) return;

            try
            {
                var row = dgvOrderItems.Rows[e.RowIndex];
                var productId = row.Cells["ProductId"].Value?.ToString();
                var newQuantity = Convert.ToInt32(row.Cells["Quantity"].Value ?? 0);

                if (!string.IsNullOrEmpty(productId) && newQuantity > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Editing quantity for product {productId}: {newQuantity}");

                    if (!await ValidateStock(productId, newQuantity))
                    {
                        var availableStock = await GetAvailableStock(productId);
                        row.Cells["Quantity"].Value = availableStock;
                        newQuantity = availableStock;
                    }

                    var item = _orderItems.FirstOrDefault(i => i.ProductId == productId);
                    if (item != null)
                    {
                        var oldTotal = item.TotalPrice;
                        item.Quantity = newQuantity;
                        item.TotalPrice = item.Quantity * item.UnitPrice;

                        System.Diagnostics.Debug.WriteLine($"Updated item: {item.ProductName}, Old total: {oldTotal:N0}, New total: {item.TotalPrice:N0}");

                        row.Cells["TotalPrice"].Value = item.TotalPrice;

                        // Cập nhật tổng tiền
                        UpdateTotalAmount();

                        // Force refresh tổng tiền
                        ForceRefreshTotalAmount();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in DgvOrderItems_CellEndEdit: {ex.Message}");
                MessageBox.Show($"Lỗi khi cập nhật số lượng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (_isViewMode || !ValidateForm()) return;

            try
            {
                // Kiểm tra trạng thái đơn hàng nếu đang edit
                if (_isEditMode && _editingOrder != null)
                {
                    if (_editingOrder.TrangThai == "Hoàn thành")
                    {
                        MessageBox.Show($"Không thể chỉnh sửa đơn hàng có trạng thái '{_editingOrder.TrangThai}'!\n\n" +
                                      "Chỉ có thể xem chi tiết đơn hàng này.",
                                      "Không thể chỉnh sửa",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Kiểm tra tồn kho một lần nữa trước khi lưu
                foreach (var item in _orderItems)
                {
                    var availableStock = await GetAvailableStock(item.ProductId);
                    if (availableStock < item.Quantity)
                    {
                        MessageBox.Show($"Sản phẩm '{item.ProductName}' không đủ tồn kho!\n\n" +
                                      $"Yêu cầu: {item.Quantity}\n" +
                                      $"Tồn kho hiện tại: {availableStock}\n\n" +
                                      "Vui lòng kiểm tra lại số lượng hoặc chọn sản phẩm khác!",
                                      "Cảnh báo tồn kho",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Warning);
                        return;
                    }
                }

                btnSave.Enabled = false;
                btnSave.Text = "Đang lưu...";
                this.Cursor = Cursors.WaitCursor;

                if (_isEditMode)
                {
                    await UpdateExistingOrder();
                }
                else
                {
                    await CreateNewOrder();
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving order: {ex.Message}");
                MessageBox.Show($"Lỗi lưu đơn hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSave.Enabled = true;
                btnSave.Text = "💾 Lưu đơn hàng";
                this.Cursor = Cursors.Default;
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

            // Không cần kiểm tra trạng thái vì đã có giá trị mặc định "Hoàn thành"
            return true;
        }

        private async Task CreateNewOrder()
        {
            try
            {
                // Tạo đơn hàng mới
                var order = new Dondathang
                {
                    MaDdh = _currentOrderId,
                    MaKh = cboCustomer.SelectedValue?.ToString(),
                    MaNv = _currentUserMaNv, // Sử dụng MaNv của user đang đăng nhập
                    NgayDat = DateOnly.FromDateTime(dtpOrderDate.Value),
                    NgayGiao = DateOnly.FromDateTime(dtpDeliveryDate.Value),
                    TrangThai = "Hoàn thành", // Trạng thái mặc định là "Hoàn thành"
                    CtDhs = _orderItems.Select(item => new CtDh
                    {
                        MaDdh = _currentOrderId,
                        MaSp = item.ProductId,
                        SoLuong = item.Quantity,
                        DonGia = item.UnitPrice
                    }).ToList()
                };
                // Tạo chuỗi log
                var orderLog = $"Mã ĐDH: {order.MaDdh}\n" +
                               $"Mã KH: {order.MaKh}\n" +
                               $"Mã NV: {order.MaNv}\n" +
                               $"Ngày đặt: {order.NgayDat}\n" +
                               $"Ngày giao: {order.NgayGiao}\n" +
                               $"Trạng thái: {order.TrangThai}\n\n" +
                               "Chi tiết đơn hàng:\n" +
                               string.Join("\n", order.CtDhs.Select((ct, index) =>
                                   $"{index + 1}. Mã SP: {ct.MaSp}, SL: {ct.SoLuong}, Đơn giá: {ct.DonGia}"
                               ));

                // Hiển thị MessageBox
                // MessageBox.Show(orderLog, "Thông tin đơn hàng", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Sử dụng repository để tạo đơn hàng
                var success = await _orderRepository.AddOrderAsync(order);

                if (success)
                {
                    System.Diagnostics.Debug.WriteLine($"Order {_currentOrderId} created successfully with {_orderItems.Count} items");

                    try
                    {
                        // Tự động tạo hóa đơn bán với tổng tiền
                        var totalAmount = (decimal)_orderItems.Sum(item => item.TotalPrice);
                        var invoiceId = await _orderRepository.CreateInvoiceForOrderAsync(_currentOrderId, totalAmount);
                        System.Diagnostics.Debug.WriteLine($"Invoice {invoiceId} created for order {_currentOrderId} with total amount: {totalAmount:N0} VNĐ");
                        MessageBox.Show($"Đơn hàng {_currentOrderId} đã được tạo thành công!\n\n" +
                                      $"Hóa đơn bán: {invoiceId}\n" +
                                      $"Tổng tiền: {totalAmount:N0} VNĐ",
                                      "Thành công",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Information);
                    }
                    catch (Exception invoiceEx)
                    {
                        // Nếu tạo hóa đơn thất bại, ghi log nhưng không làm fail việc tạo đơn hàng
                        System.Diagnostics.Debug.WriteLine($"Warning: Could not create invoice for order {_currentOrderId}: {invoiceEx.Message}");
                        // MessageBox.Show($"Đơn hàng {_currentOrderId} đã được tạo thành công!\n\n" +
                        //               $"Tuy nhiên, có vấn đề khi tạo hóa đơn bán:\n{invoiceEx.Message}\n\n" +
                        //               $"Bạn có thể tạo hóa đơn bán sau.",
                        //               "Thông báo",
                        //               MessageBoxButtons.OK,
                        //               MessageBoxIcon.Warning);
                       //trick ẩn =))
                    }
                }
                else
                {
                    throw new InvalidOperationException("Không thể tạo đơn hàng!");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating order: {ex.Message}");
                throw new InvalidOperationException($"Lỗi khi tạo đơn hàng: {ex.Message}");
            }
        }

        private async Task UpdateExistingOrder()
        {
            if (_editingOrder == null) return;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _editingOrder.MaKh = cboCustomer.SelectedValue?.ToString();
                _editingOrder.MaNv = _currentUserMaNv; // Cập nhật MaNv của user đang đăng nhập
                _editingOrder.NgayDat = DateOnly.FromDateTime(dtpOrderDate.Value);
                _editingOrder.NgayGiao = DateOnly.FromDateTime(dtpDeliveryDate.Value);
                _editingOrder.TrangThai = "Hoàn thành"; // Trạng thái mặc định

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

                try
                {
                    // Tự động tạo hóa đơn bán với tổng tiền cập nhật
                    var totalAmount = (decimal)_orderItems.Sum(item => item.TotalPrice);
                    var invoiceId = await _orderRepository.CreateInvoiceForOrderAsync(_editingOrder.MaDdh, totalAmount);
                    System.Diagnostics.Debug.WriteLine($"Invoice {invoiceId} created for updated order {_editingOrder.MaDdh} with total amount: {totalAmount:N0} VNĐ");
                }
                catch (Exception invoiceEx)
                {
                    // Nếu tạo hóa đơn thất bại, ghi log nhưng không làm fail việc cập nhật đơn hàng
                    System.Diagnostics.Debug.WriteLine($"Warning: Could not create invoice for updated order {_editingOrder.MaDdh}: {invoiceEx.Message}");
                }
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
                if (string.IsNullOrEmpty(productId))
                {
                    System.Diagnostics.Debug.WriteLine("ProductId is null or empty");
                    return 0;
                }

                // Sử dụng repository để lấy tồn kho
                var stock = await _orderRepository.GetAvailableStockAsync(productId);
                System.Diagnostics.Debug.WriteLine($"Available stock for {productId}: {stock}");

                return stock;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting available stock for {productId}: {ex.Message}");
                return 0;
            }
        }

        private async Task UpdateInventory(string productId, int quantity)
        {
            try
            {
                // Sử dụng repository để cập nhật tồn kho
                await _orderRepository.UpdateInventoryAsync(productId, quantity);
                System.Diagnostics.Debug.WriteLine($"Updated inventory for {productId} successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating inventory for {productId}: {ex.Message}");
                throw new InvalidOperationException($"Không thể cập nhật tồn kho cho sản phẩm {productId}: {ex.Message}");
            }
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

        private async void CboProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboProduct.SelectedValue == null)
                {
                    txtInventory.Text = "Vui lòng chọn sản phẩm";
                    txtInventory.ForeColor = Color.Gray;
                    return;
                }

                var selectedProduct = cboProduct.SelectedItem as Sanpham;
                if (selectedProduct == null) return;

                var availableStock = await GetAvailableStock(selectedProduct.MaSp);

                // Hiển thị thông tin tồn kho với màu sắc
                if (availableStock <= 0)
                {
                    txtInventory.Text = $"HẾT HÀNG - Tồn kho: 0";
                    txtInventory.ForeColor = Color.Red;
                    txtInventory.BackColor = Color.LightCoral;
                }
                else if (availableStock < 10)
                {
                    txtInventory.Text = $"SẮP HẾT - Tồn kho: {availableStock}";
                    txtInventory.ForeColor = Color.DarkOrange;
                    txtInventory.BackColor = Color.LightYellow;
                }
                else
                {
                    txtInventory.Text = $"CÓ HÀNG - Tồn kho: {availableStock}";
                    txtInventory.ForeColor = Color.DarkGreen;
                    txtInventory.BackColor = Color.LightGreen;
                }

                // Cập nhật số lượng tối đa có thể đặt
                numQuantity.Maximum = availableStock;
                if (numQuantity.Value > availableStock)
                {
                    numQuantity.Value = availableStock;
                }

                System.Diagnostics.Debug.WriteLine($"Selected product: {selectedProduct.TenSp}, Stock: {availableStock}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating inventory display: {ex.Message}");
                txtInventory.Text = "Lỗi khi tải thông tin tồn kho";
                txtInventory.ForeColor = Color.Red;
                txtInventory.BackColor = Color.LightCoral;
            }
        }

        private void txtTenNv_TextChanged(object sender, EventArgs e)
        {

        }

        private void ValidateAndFixTotalAmountLabel()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== Validating lblTotalAmount ===");

                // Kiểm tra lblTotalAmount từ Designer
                if (lblTotalAmount != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Designer lblTotalAmount: Text='{lblTotalAmount.Text}', Visible={lblTotalAmount.Visible}, Location={lblTotalAmount.Location}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Designer lblTotalAmount is NULL");
                }

                // Tìm tất cả Label có chứa "Tổng tiền" trong form
                var allLabels = this.Controls.Find("", true).OfType<Label>().ToList();
                var totalLabels = allLabels.Where(l => l.Text.Contains("Tổng tiền")).ToList();

                System.Diagnostics.Debug.WriteLine($"Found {totalLabels.Count} labels with 'Tổng tiền':");
                foreach (var label in totalLabels)
                {
                    System.Diagnostics.Debug.WriteLine($"  - Name: {label.Name}, Text: '{label.Text}', Location: {label.Location}");
                }

                // Nếu không tìm thấy lblTotalAmount, tạo mới
                if (lblTotalAmount == null || !lblTotalAmount.IsHandleCreated)
                {
                    System.Diagnostics.Debug.WriteLine("Creating new lblTotalAmount...");

                    // Tìm grpOrderItems để đặt label vào
                    var grpOrderItems = this.Controls.OfType<GroupBox>().FirstOrDefault(g => g.Name == "grpOrderItems");
                    if (grpOrderItems != null)
                    {
                        lblTotalAmount = new Label
                        {
                            Name = "lblTotalAmount",
                            Text = "Tổng tiền: 0 VNĐ",
                            Font = new Font("Segoe UI", 14, FontStyle.Bold),
                            ForeColor = Color.FromArgb(46, 204, 113),
                            AutoSize = true,
                            Location = new Point(37, 533)
                        };

                        grpOrderItems.Controls.Add(lblTotalAmount);
                        System.Diagnostics.Debug.WriteLine("New lblTotalAmount created and added to grpOrderItems");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("ERROR: grpOrderItems not found");
                    }
                }

                System.Diagnostics.Debug.WriteLine("=== End Validation ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ValidateAndFixTotalAmountLabel: {ex.Message}");
            }
        }

        private void DebugFormControls()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== DEBUG FORM CONTROLS ===");
                System.Diagnostics.Debug.WriteLine($"Form name: {this.Name}, Text: {this.Text}");
                System.Diagnostics.Debug.WriteLine($"Total controls: {this.Controls.Count}");

                foreach (Control control in this.Controls)
                {
                    System.Diagnostics.Debug.WriteLine($"Control: {control.GetType().Name} - Name: {control.Name}, Text: {control.Text}");

                    if (control is GroupBox grp)
                    {
                        System.Diagnostics.Debug.WriteLine($"  GroupBox '{grp.Name}' has {grp.Controls.Count} controls:");
                        foreach (Control subControl in grp.Controls)
                        {
                            System.Diagnostics.Debug.WriteLine($"    - {subControl.GetType().Name}: {subControl.Name} = '{subControl.Text}'");
                        }
                    }
                }

                // Tìm lblTotalAmount cụ thể
                var totalAmountLabel = this.Controls.Find("lblTotalAmount", true).FirstOrDefault();
                if (totalAmountLabel != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Found lblTotalAmount: {totalAmountLabel.GetType().Name} - Name: {totalAmountLabel.Name}, Text: '{totalAmountLabel.Text}'");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("lblTotalAmount NOT found in Controls.Find");
                }

                System.Diagnostics.Debug.WriteLine("=== END DEBUG ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in DebugFormControls: {ex.Message}");
            }
        }

        private void ForceRefreshTotalAmount()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("ForceRefreshTotalAmount called");

                // Cập nhật tổng tiền từ order items
                UpdateTotalAmount();

                // Force refresh UI
                if (lblTotalAmount != null)
                {
                    lblTotalAmount.Refresh();
                    System.Diagnostics.Debug.WriteLine($"Force refreshed lblTotalAmount: {lblTotalAmount.Text}");
                }

                // Cũng refresh form
                this.Refresh();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ForceRefreshTotalAmount: {ex.Message}");
            }
        }

        private void CheckTotalAmountVisibility()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== Checking Total Amount Visibility ===");

                if (lblTotalAmount != null)
                {
                    System.Diagnostics.Debug.WriteLine($"lblTotalAmount properties:");
                    System.Diagnostics.Debug.WriteLine($"  - Text: '{lblTotalAmount.Text}'");
                    System.Diagnostics.Debug.WriteLine($"  - Visible: {lblTotalAmount.Visible}");
                    System.Diagnostics.Debug.WriteLine($"  - Enabled: {lblTotalAmount.Enabled}");
                    System.Diagnostics.Debug.WriteLine($"  - Location: {lblTotalAmount.Location}");
                    System.Diagnostics.Debug.WriteLine($"  - Size: {lblTotalAmount.Size}");
                    System.Diagnostics.Debug.WriteLine($"  - Parent: {lblTotalAmount.Parent?.Name ?? "NULL"}");
                    System.Diagnostics.Debug.WriteLine($"  - Handle: {lblTotalAmount.IsHandleCreated}");

                    // Kiểm tra xem có bị ẩn bởi parent control không
                    var parent = lblTotalAmount.Parent;
                    while (parent != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"  - Parent {parent.GetType().Name}: Visible={parent.Visible}, Enabled={parent.Enabled}");
                        parent = parent.Parent;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("lblTotalAmount is NULL");
                }

                // Kiểm tra tất cả controls có chứa "Tổng tiền"
                var allControls = this.Controls.Find("", true);
                var totalLabels = allControls.OfType<Label>().Where(l => l.Text.Contains("Tổng tiền")).ToList();

                System.Diagnostics.Debug.WriteLine($"Found {totalLabels.Count} labels with 'Tổng tiền':");
                foreach (var label in totalLabels)
                {
                    System.Diagnostics.Debug.WriteLine($"  - {label.Name}: '{label.Text}' at {label.Location}, Visible={label.Visible}");
                }

                System.Diagnostics.Debug.WriteLine("=== End Check ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CheckTotalAmountVisibility: {ex.Message}");
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
