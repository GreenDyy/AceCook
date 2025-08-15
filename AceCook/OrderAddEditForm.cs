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
            SetupBasicControls();
            SetupComboBoxes();
            SetupDataGridView();
            SetupEventHandlers();
            SetDefaultValues();
            UpdateFormTitle();
            SetupProductControls();
            SetupActionControls();
        }

        private void SetupBasicControls()
        {
            try
            {
                // Panel cho thông tin cơ bản
                var pnlBasicInfo = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 150,
                    BackColor = Color.FromArgb(248, 249, 250),
                    BorderStyle = BorderStyle.FixedSingle,
                    Padding = new Padding(15)
                };

                // Label tiêu đề
                var lblBasicInfo = new Label
                {
                    Text = "THÔNG TIN ĐƠN HÀNG",
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.FromArgb(52, 73, 94),
                    AutoSize = true,
                    Location = new Point(15, 15)
                };

                // Label và TextBox cho Mã đơn hàng
                var lblOrderId = new Label
                {
                    Text = "Mã đơn hàng:",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(15, 50)
                };

                txtOrderId = new TextBox
                {
                    Location = new Point(120, 48),
                    Size = new Size(150, 25),
                    Font = new Font("Segoe UI", 10),
                    ReadOnly = true,
                    BackColor = Color.LightGray
                };

                // Label và ComboBox cho Khách hàng
                var lblCustomer = new Label
                {
                    Text = "Khách hàng:",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(290, 50)
                };

                cboCustomer = new ComboBox
                {
                    Location = new Point(380, 48),
                    Size = new Size(250, 25),
                    Font = new Font("Segoe UI", 10),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                // Label và DateTimePicker cho Ngày đặt
                var lblOrderDate = new Label
                {
                    Text = "Ngày đặt:",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(15, 85)
                };

                dtpOrderDate = new DateTimePicker
                {
                    Location = new Point(120, 83),
                    Size = new Size(150, 25),
                    Font = new Font("Segoe UI", 10),
                    Format = DateTimePickerFormat.Short
                };

                // Label và DateTimePicker cho Ngày giao
                var lblDeliveryDate = new Label
                {
                    Text = "Ngày giao:",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(290, 85)
                };

                dtpDeliveryDate = new DateTimePicker
                {
                    Location = new Point(380, 83),
                    Size = new Size(150, 25),
                    Font = new Font("Segoe UI", 10),
                    Format = DateTimePickerFormat.Short
                };

                // Label và ComboBox cho Trạng thái
                var lblStatus = new Label
                {
                    Text = "Trạng thái:",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(550, 85)
                };

                cboStatus = new ComboBox
                {
                    Location = new Point(630, 83),
                    Size = new Size(150, 25),
                    Font = new Font("Segoe UI", 10),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                // Thêm controls vào panel
                pnlBasicInfo.Controls.AddRange(new Control[] 
                {
                    lblBasicInfo, lblOrderId, txtOrderId, lblCustomer, cboCustomer,
                    lblOrderDate, dtpOrderDate, lblDeliveryDate, dtpDeliveryDate,
                    lblStatus, cboStatus
                });

                // Thêm panel vào form
                this.Controls.Add(pnlBasicInfo);

                System.Diagnostics.Debug.WriteLine("Basic controls setup completed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting up basic controls: {ex.Message}");
                MessageBox.Show($"Lỗi khi thiết lập controls cơ bản: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupProductControls()
        {
            try
            {
                // Tạo panel cho việc thêm sản phẩm
                var pnlAddProduct = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 120,
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    Padding = new Padding(15)
                };

                // Label "Thêm sản phẩm"
                var lblAddProduct = new Label
                {
                    Text = "THÊM SẢN PHẨM VÀO ĐƠN HÀNG",
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.FromArgb(52, 73, 94),
                    AutoSize = true,
                    Location = new Point(15, 15)
                };

                // Label "Sản phẩm"
                var lblProduct = new Label
                {
                    Text = "Sản phẩm:",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(15, 50)
                };

                // ComboBox sản phẩm
                cboProduct = new ComboBox
                {
                    Location = new Point(100, 48),
                    Size = new Size(250, 25),
                    Font = new Font("Segoe UI", 10),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                // Label "Số lượng"
                var lblQuantity = new Label
                {
                    Text = "Số lượng:",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(370, 50)
                };

                // NumericUpDown cho số lượng
                numQuantity = new NumericUpDown
                {
                    Location = new Point(450, 48),
                    Size = new Size(100, 25),
                    Font = new Font("Segoe UI", 10),
                    Minimum = 1,
                    Maximum = 9999,
                    Value = 1
                };

                // Label hiển thị thông tin tồn kho
                lblStockInfo = new Label
                {
                    Text = "Chọn sản phẩm để xem thông tin tồn kho",
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Location = new Point(100, 80)
                };

                // Nút thêm sản phẩm
                btnAddProduct = new Button
                {
                    Text = "➕ Thêm vào đơn hàng",
                    Location = new Point(570, 45),
                    Size = new Size(150, 30),
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    BackColor = Color.FromArgb(46, 204, 113),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnAddProduct.FlatAppearance.BorderSize = 0;

                // Nút xóa sản phẩm
                btnRemoveProduct = new Button
                {
                    Text = "🗑️ Xóa sản phẩm",
                    Location = new Point(730, 45),
                    Size = new Size(130, 30),
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    BackColor = Color.FromArgb(231, 76, 60),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnRemoveProduct.FlatAppearance.BorderSize = 0;

                // Thêm controls vào panel
                pnlAddProduct.Controls.AddRange(new Control[] 
                {
                    lblAddProduct, lblProduct, cboProduct, lblQuantity, numQuantity,
                    lblStockInfo, btnAddProduct, btnRemoveProduct
                });

                // Thêm panel vào form
                this.Controls.Add(pnlAddProduct);

                // Gắn event handlers
                btnAddProduct.Click += btnAddProduct_Click;
                btnRemoveProduct.Click += btnRemoveProduct_Click;
                cboProduct.SelectedIndexChanged += CboProduct_SelectedIndexChanged;

                System.Diagnostics.Debug.WriteLine("Product controls setup completed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting up product controls: {ex.Message}");
                MessageBox.Show($"Lỗi khi thiết lập controls sản phẩm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupActionControls()
        {
            try
            {
                // Panel cho tổng tiền và nút hành động
                var pnlActions = new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 80,
                    BackColor = Color.FromArgb(248, 249, 250),
                    BorderStyle = BorderStyle.FixedSingle,
                    Padding = new Padding(15)
                };

                // Label tổng tiền
                var lblTotal = new Label
                {
                    Text = "Tổng tiền:",
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.FromArgb(52, 73, 94),
                    AutoSize = true,
                    Location = new Point(15, 25)
                };

                // Label hiển thị tổng tiền
                lblTotalAmount = new Label
                {
                    Text = "0 VNĐ",
                    Font = new Font("Segoe UI", 14, FontStyle.Bold),
                    ForeColor = Color.FromArgb(46, 204, 113),
                    AutoSize = true,
                    Location = new Point(120, 22)
                };

                // Nút Lưu
                btnSave = new Button
                {
                    Text = "💾 Lưu đơn hàng",
                    Location = new Point(400, 20),
                    Size = new Size(150, 40),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    BackColor = Color.FromArgb(46, 204, 113),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnSave.FlatAppearance.BorderSize = 0;

                // Nút Hủy/Đóng
                btnCancel = new Button
                {
                    Text = "❌ Hủy",
                    Location = new Point(570, 20),
                    Size = new Size(120, 40),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    BackColor = Color.FromArgb(231, 76, 60),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnCancel.FlatAppearance.BorderSize = 0;

                // Thêm controls vào panel
                pnlActions.Controls.AddRange(new Control[] 
                {
                    lblTotal, lblTotalAmount, btnSave, btnCancel
                });

                // Thêm panel vào form
                this.Controls.Add(pnlActions);

                // Gắn event handlers
                btnSave.Click += btnSave_Click;
                btnCancel.Click += btnCancel_Click;

                System.Diagnostics.Debug.WriteLine("Action controls setup completed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting up action controls: {ex.Message}");
                MessageBox.Show($"Lỗi khi thiết lập controls hành động: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            try
            {
                // Tạo DataGridView
                dgvOrderItems = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    AutoGenerateColumns = false,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    ReadOnly = true,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    MultiSelect = false,
                    BackgroundColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    GridColor = Color.LightGray,
                    RowHeadersVisible = false,
                    CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                    ColumnHeadersHeight = 40,
                    RowTemplate = { Height = 35 }
                };

                // Thiết lập style cho DataGridView
                dgvOrderItems.DefaultCellStyle.Font = new Font("Segoe UI", 9);
                dgvOrderItems.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                dgvOrderItems.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
                dgvOrderItems.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvOrderItems.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvOrderItems.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
                dgvOrderItems.DefaultCellStyle.SelectionForeColor = Color.White;

                // Tạo các cột
                var columns = new[]
                {
                    new DataGridViewTextBoxColumn { Name = "ProductId", HeaderText = "Mã SP", DataPropertyName = "ProductId", Width = 80 },
                    new DataGridViewTextBoxColumn { Name = "ProductName", HeaderText = "Tên Sản Phẩm", DataPropertyName = "ProductName", Width = 250 },
                    new DataGridViewTextBoxColumn { Name = "Quantity", HeaderText = "Số Lượng", DataPropertyName = "Quantity", Width = 100 },
                    new DataGridViewTextBoxColumn { Name = "UnitPrice", HeaderText = "Đơn Giá", DataPropertyName = "UnitPrice", Width = 120 },
                    new DataGridViewTextBoxColumn { Name = "TotalPrice", HeaderText = "Thành Tiền", DataPropertyName = "TotalPrice", Width = 150 }
                };

                dgvOrderItems.Columns.AddRange(columns);

                // Tạo panel chứa DataGridView
                var pnlDataGrid = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    Padding = new Padding(10)
                };

                // Label tiêu đề cho danh sách sản phẩm
                var lblOrderItems = new Label
                {
                    Text = "DANH SÁCH SẢN PHẨM TRONG ĐƠN HÀNG",
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.FromArgb(52, 73, 94),
                    AutoSize = true,
                    Location = new Point(10, 10)
                };

                pnlDataGrid.Controls.Add(lblOrderItems);
                pnlDataGrid.Controls.Add(dgvOrderItems);
                dgvOrderItems.Location = new Point(10, 40);
                dgvOrderItems.Size = new Size(pnlDataGrid.Width - 20, pnlDataGrid.Height - 50);

                // Thêm panel vào form
                this.Controls.Add(pnlDataGrid);

                System.Diagnostics.Debug.WriteLine("DataGridView setup completed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting up DataGridView: {ex.Message}");
                MessageBox.Show($"Lỗi khi thiết lập DataGridView: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                
            // Nếu ở view mode, hiển thị thêm thông tin
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
                // Disable tất cả controls input
                var controls = new Control[] { cboCustomer, dtpOrderDate, dtpDeliveryDate, cboStatus, cboProduct, numQuantity, btnAddProduct, btnRemoveProduct };
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
                
                // Ẩn các controls thêm sản phẩm
                if (lblStockInfo != null) lblStockInfo.Visible = false;

                System.Diagnostics.Debug.WriteLine("View mode controls disabled successfully");
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
                        MessageBox.Show($"Đang chỉnh sửa đơn hàng: {_editingOrder.MaDdh}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                            System.Diagnostics.Debug.WriteLine($"Added item: {orderItem.ProductName} - Qty: {orderItem.Quantity} - Price: {orderItem.UnitPrice:N0}");
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
                RefreshOrderItemsGrid();
                UpdateTotalAmount();
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
            try
            {
                System.Diagnostics.Debug.WriteLine($"Refreshing grid with {_orderItems.Count} items");
                
                // Clear và set lại DataSource
                dgvOrderItems.DataSource = null;
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing grid: {ex.Message}");
                MessageBox.Show($"Lỗi khi cập nhật bảng dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                            row.DefaultCellStyle.BackColor = Color.White;
                            row.DefaultCellStyle.ForeColor = Color.Black;
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
                System.Diagnostics.Debug.WriteLine($"Total amount: {total:N0} VNĐ");
                
                if (lblTotalAmount != null)
                {
                    lblTotalAmount.Text = $"Tổng tiền: {total:N0} VNĐ";
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("lblTotalAmount is null");
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

            try
            {
                var productId = cboProduct.SelectedValue.ToString();
                var selectedProduct = cboProduct.SelectedItem as Sanpham;

                if (selectedProduct != null)
                {
                    // Lấy thông tin tồn kho
                    var availableStock = await GetAvailableStock(productId);
                    var price = selectedProduct.Gia?.ToString("N0") ?? "0";
                    
                    // Cập nhật thông tin hiển thị
                    lblStockInfo.Text = $"Tồn kho: {availableStock} | Giá: {price} VNĐ";
                    lblStockInfo.Visible = true;
                    
                    // Cập nhật số lượng tối đa có thể đặt
                    numQuantity.Maximum = availableStock;
                    numQuantity.Value = Math.Min((int)numQuantity.Value, availableStock);
                    
                    // Hiển thị thông báo nếu hết hàng
                    if (availableStock <= 0)
                    {
                        lblStockInfo.Text = "HẾT HÀNG | Giá: " + price + " VNĐ";
                        lblStockInfo.ForeColor = Color.Red;
                        btnAddProduct.Enabled = false;
                    }
                    else if (availableStock <= 10)
                    {
                        lblStockInfo.ForeColor = Color.Orange;
                        btnAddProduct.Enabled = true;
                    }
                    else
                    {
                        lblStockInfo.ForeColor = Color.Green;
                        btnAddProduct.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CboProduct_SelectedIndexChanged: {ex.Message}");
                lblStockInfo.Text = "Lỗi khi tải thông tin sản phẩm";
                lblStockInfo.ForeColor = Color.Red;
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
                // Kiểm tra tồn kho một lần nữa trước khi tạo đơn hàng
                foreach (var item in _orderItems)
                {
                    var availableStock = await GetAvailableStock(item.ProductId);
                    if (availableStock < item.Quantity)
                    {
                        throw new InvalidOperationException(
                            $"Sản phẩm '{item.ProductName}' không đủ tồn kho!\n" +
                            $"Yêu cầu: {item.Quantity}, Tồn kho: {availableStock}");
                    }
                }

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

                // Lưu đơn hàng
                await _context.Dondathangs.AddAsync(order);
                await _context.SaveChangesAsync();

                // Cập nhật tồn kho
                foreach (var item in _orderItems)
                {
                    await UpdateInventory(item.ProductId, item.Quantity);
                }

                // Commit transaction
                await transaction.CommitAsync();
                
                System.Diagnostics.Debug.WriteLine($"Order {_currentOrderId} created successfully with {_orderItems.Count} items");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                System.Diagnostics.Debug.WriteLine($"Error creating order: {ex.Message}");
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
                if (string.IsNullOrEmpty(productId))
                {
                    System.Diagnostics.Debug.WriteLine("ProductId is null or empty");
                    return 0;
                }

                var inventory = await _context.CtTons
                    .FirstOrDefaultAsync(ct => ct.MaSp == productId && ct.MaKho == "K01");
                
                var stock = inventory?.SoLuongTonKho ?? 0;
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
                // Tìm inventory hiện tại
                var inventory = await _context.CtTons
                    .FirstOrDefaultAsync(ct => ct.MaSp == productId && ct.MaKho == "K01");

                if (inventory != null)
                {
                    // Kiểm tra tồn kho trước khi trừ
                    var currentStock = inventory.SoLuongTonKho ?? 0;
                    if (currentStock < quantity)
                    {
                        throw new InvalidOperationException($"Tồn kho không đủ! Yêu cầu: {quantity}, Hiện có: {currentStock}");
                    }

                    // Trừ tồn kho
                    inventory.SoLuongTonKho = Math.Max(0, currentStock - quantity);
                    
                    System.Diagnostics.Debug.WriteLine($"Updated inventory for {productId}: {currentStock} -> {inventory.SoLuongTonKho}");
                }
                else
                {
                    // Tạo mới inventory nếu chưa có
                    inventory = new CtTon 
                    { 
                        MaSp = productId, 
                        MaKho = "K01", 
                        SoLuongTonKho = 0 
                    };
                    await _context.CtTons.AddAsync(inventory);
                    System.Diagnostics.Debug.WriteLine($"Created new inventory for {productId}");
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating inventory for {productId}: {ex.Message}");
                throw new InvalidOperationException($"Không thể cập nhật tồn kho cho sản phẩm {productId}: {ex.Message}");
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
