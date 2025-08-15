using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AceCook.Models;
using AceCook.Repositories;
using System.Globalization;

namespace AceCook
{
    public partial class OrderManagementForm : Form
    {
        private readonly AppDbContext _context;
        private readonly OrderRepository _orderRepository;
        private DataGridView dataGridViewOrders;
        private TextBox txtSearch;
        private ComboBox cboStatusFilter;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private Button btnSearch;
        private Button btnReset;
        private Button btnCreateOrder;
        private Button btnRefresh;
        private Label lblTitle;
        private Label lblSearch;
        private Label lblStatusFilter;
        private Label lblDateRange;
        private Panel pnlFilters;
        private Panel pnlActions;

        public OrderManagementForm(AppDbContext context)
        {
            _context = context;
            _orderRepository = new OrderRepository(context);
            InitializeComponent();
            SetupUI();
            LoadOrders();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Text = "Quản lý Đơn hàng";
            this.Size = new Size(1400, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            this.ResumeLayout(false);
        }

        private void SetupUI()
        {
            // Title
            lblTitle = new Label
            {
                Text = "QUẢN LÝ ĐƠN HÀNG",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Dock = DockStyle.Top,
                Height = 70, // tăng lên
                TextAlign = ContentAlignment.MiddleLeft,
            };

            // Filters Panel
            pnlFilters = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15),
                FlowDirection = FlowDirection.LeftToRight,
                AutoScroll = true,
                WrapContents = false,
                Margin = new Padding(0, 200, 0, 150) // Thêm dòng này để tạo khoảng cách phía trên

            };

            lblSearch = new Label
            {
                Text = "Tìm kiếm:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 8, 10, 0)
            };

            txtSearch = new TextBox
            {
                Width = 300,
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "Mã đơn hàng, tên KH, trạng thái...",
                Margin = new Padding(0, 5, 20, 0)
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            lblStatusFilter = new Label
            {
                Text = "Trạng thái:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 8, 10, 0)
            };

            cboStatusFilter = new ComboBox
            {
                Width = 180,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 5, 20, 0)
            };
            cboStatusFilter.Items.AddRange(new object[] { "Tất cả", "Chờ xử lý", "Đang xử lý", "Đã giao", "Đã hủy" });
            cboStatusFilter.SelectedIndex = 0;
            cboStatusFilter.SelectedIndexChanged += CboStatusFilter_SelectedIndexChanged;

            lblDateRange = new Label
            {
                Text = "Từ ngày:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 8, 10, 0)
            };

            dtpStartDate = new DateTimePicker
            {
                Width = 200,
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddDays(-30),
                Margin = new Padding(0, 5, 10, 0)
            };

            var lblToDate = new Label
            {
                Text = "đến:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 8, 10, 0)
            };

            dtpEndDate = new DateTimePicker
            {
                Width = 200,
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now,
                Margin = new Padding(0, 5, 10, 0)
            };

            btnSearch = new Button
            {
                Text = "🔍 Tìm kiếm",
               Width = 200,
                Height = 40,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 5, 10, 0)
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += BtnSearch_Click;

            btnReset = new Button
            {
                Text = "🔄 Làm mới",
                Width = 200,
                Height = 40,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(95, 95, 95),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 5, 10, 0)
            };
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.Click += BtnReset_Click;

            pnlFilters.Controls.AddRange(new Control[] {
        lblSearch, txtSearch, lblStatusFilter, cboStatusFilter,
        lblDateRange, dtpStartDate, lblToDate, dtpEndDate,
        btnSearch, btnReset
    });

            // Actions Panel
            pnlActions = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 90,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(15),
                BackColor = Color.Transparent
            };

            btnCreateOrder = CreateActionButton("➕ Tạo đơn", Color.FromArgb(46, 204, 113));
            btnCreateOrder.Click += BtnCreateOrder_Click;

            btnRefresh = CreateActionButton("🔄 Làm mới", Color.FromArgb(52, 152, 219));
            btnRefresh.Click += BtnRefresh_Click;

            var btnEditOrder = CreateActionButton("✏️ Chỉnh sửa", Color.FromArgb(255, 193, 7));
            btnEditOrder.Click += BtnEditOrder_Click;

            var btnDeleteOrder = CreateActionButton("🗑️ Xóa", Color.FromArgb(231, 76, 60));
            btnDeleteOrder.Click += BtnDeleteOrder_Click;

            pnlActions.Controls.AddRange(new Control[] { btnCreateOrder, btnRefresh, btnEditOrder, btnDeleteOrder });

            // DataGridView
            dataGridViewOrders = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
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
                ColumnHeadersHeight = 50,
                RowTemplate = { Height = 50 }
            };
            dataGridViewOrders.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dataGridViewOrders.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridViewOrders.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dataGridViewOrders.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewOrders.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewOrders.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dataGridViewOrders.DefaultCellStyle.SelectionForeColor = Color.White;

            // Add all to form
            this.Controls.AddRange(new Control[] { dataGridViewOrders, pnlActions, pnlFilters, lblTitle });
        }

        // Helper to create buttons
        private Button CreateActionButton(string text, Color backColor)
        {
            var btn = new Button
            {
                Text = text,
                Width = 250,
                Height = 60,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 0, 15, 0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }


        private async void LoadOrders()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Loading orders...");
                var orders = await _orderRepository.GetAllOrdersAsync();
                System.Diagnostics.Debug.WriteLine($"Loaded {orders?.Count ?? 0} orders");
                
                if (orders == null)
                {
                    MessageBox.Show("Không thể tải danh sách đơn hàng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                RefreshDataGridView(orders);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadOrders: {ex.Message}");
                MessageBox.Show($"Lỗi khi tải dữ liệu đơn hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshDataGridView(List<Dondathang> orders)
        {
            dataGridViewOrders.DataSource = null;
            dataGridViewOrders.Columns.Clear();

            // Create custom columns
            dataGridViewOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaDdh",
                HeaderText = "Mã đơn hàng",
                Width = 120
            });

            dataGridViewOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CustomerInfo",
                HeaderText = "Thông tin khách hàng",
                Width = 200
            });

            dataGridViewOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NgayDat",
                HeaderText = "Ngày đặt",
                Width = 120
            });

            dataGridViewOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NgayGiao",
                HeaderText = "Ngày giao",
                Width = 120
            });

            dataGridViewOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "EmployeeInfo",
                HeaderText = "Nhân viên xử lý",
                Width = 150
            });

            dataGridViewOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TrangThai",
                HeaderText = "Trạng thái",
                Width = 120
            });

            dataGridViewOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalAmount",
                HeaderText = "Tổng tiền",
                Width = 120
            });

            dataGridViewOrders.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "ViewDetails",
                HeaderText = "Hành động",
                Width = 100,
                Text = "👁️ Xem",
                UseColumnTextForButtonValue = true
            });

            // Populate data
            foreach (var order in orders)
            {
                try
                {
                    var rowIndex = dataGridViewOrders.Rows.Add();
                    var row = dataGridViewOrders.Rows[rowIndex];

                    row.Cells["MaDdh"].Value = order.MaDdh ?? "N/A";
                    
                    // Xử lý thông tin khách hàng an toàn
                    var customerName = order.MaKhNavigation?.TenKh ?? "N/A";
                    var customerPhone = order.MaKhNavigation?.Sdtkh ?? "N/A";
                    row.Cells["CustomerInfo"].Value = $"{customerName}\n{customerPhone}";
                    
                    row.Cells["NgayDat"].Value = order.NgayDat?.ToString("dd/MM/yyyy") ?? "N/A";
                    row.Cells["NgayGiao"].Value = order.NgayGiao?.ToString("dd/MM/yyyy") ?? "Chưa giao";
                    
                    // Xử lý thông tin nhân viên an toàn
                    row.Cells["EmployeeInfo"].Value = order.MaNvNavigation?.HoTenNv ?? "N/A";
                    row.Cells["TrangThai"].Value = order.TrangThai ?? "Chờ xử lý";
                    
                    // Calculate total amount an toàn
                    decimal totalAmount = 0;
                    if (order.CtDhs != null && order.CtDhs.Any())
                    {
                        totalAmount = order.CtDhs.Sum(ct => (decimal)((ct.SoLuong ?? 0) * (ct.DonGia ?? 0)));
                    }
                    row.Cells["TotalAmount"].Value = totalAmount.ToString("N0") + " VNĐ";

                    // Style status column
                    StyleStatusCell(row.Cells["TrangThai"], order.TrangThai);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error processing order {order.MaDdh}: {ex.Message}");
                    // Bỏ qua dòng lỗi và tiếp tục
                }
            }

            // Handle button clicks
            dataGridViewOrders.CellClick += DataGridViewOrders_CellClick;
            
            // Add double-click to view details
            dataGridViewOrders.CellDoubleClick += DataGridViewOrders_CellDoubleClick;
        }

        private void StyleStatusCell(DataGridViewCell cell, string status)
        {
            if (status == "Hoàn thành" || status == "Đã giao")
            {
                cell.Style.ForeColor = Color.Green;
            }
            else if (status == "Đang xử lý")
            {
                cell.Style.ForeColor = Color.Orange;
            }
            else if (status == "Đã hủy")
            {
                cell.Style.ForeColor = Color.Red;
            }
            else
            {
                cell.Style.ForeColor = Color.Blue;
            }
        }

        private bool _isProcessing = false;

        private async void DataGridViewOrders_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && !_isProcessing)
            {
                try
                {
                    _isProcessing = true;
                    
                    // Kiểm tra dữ liệu trước khi xử lý
                    if (dataGridViewOrders.Rows[e.RowIndex].Cells["MaDdh"].Value == null)
                    {
                        MessageBox.Show("Không thể đọc mã đơn hàng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    
                    var orderId = dataGridViewOrders.Rows[e.RowIndex].Cells["MaDdh"].Value.ToString();
                    
                    if (e.ColumnIndex == dataGridViewOrders.Columns["ViewDetails"].Index)
                    {
                        var order = await _orderRepository.GetOrderByIdAsync(orderId);
                        if (order != null)
                        {
                            ViewOrderDetails(order);
                        }
                        else
                        {
                            MessageBox.Show($"Không thể tải thông tin đơn hàng {orderId}!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xử lý thao tác: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    _isProcessing = false;
                }
            }
        }

        private void ViewOrderDetails(Dondathang order)
        {
            try
            {
                // Kiểm tra dữ liệu trước khi mở form
                if (order == null)
                {
                    MessageBox.Show("Không thể tải thông tin đơn hàng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Kiểm tra các navigation properties
                if (order.CtDhs == null)
                {
                    order.CtDhs = new List<CtDh>();
                }

                var viewForm = new OrderAddEditForm(order, true); // true = view mode
                viewForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form xem chi tiết: {ex.Message}\n\nChi tiết: {ex.StackTrace}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditOrder(Dondathang order)
        {
            try
            {
                var editForm = new OrderAddEditForm(order, false); // false = edit mode
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadOrders(); // Reload data after edit
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form chỉnh sửa: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ChangeOrderStatus(string orderId, string newStatus)
        {
            try
            {
                var success = await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus);
                if (success)
                {
                    MessageBox.Show($"Đã cập nhật trạng thái đơn hàng {orderId} thành '{newStatus}'", 
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadOrders(); // Reload data
                }
                else
                {
                    MessageBox.Show("Không thể cập nhật trạng thái đơn hàng!", 
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật trạng thái: {ex.Message}", 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void DeleteOrder(Dondathang order)
        {
            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa đơn hàng {order.MaDdh}?\n\n" +
                                       $"Khách hàng: {order.MaKhNavigation?.TenKh ?? "N/A"}\n" +
                                       $"Ngày đặt: {order.NgayDat?.ToString("dd/MM/yyyy") ?? "N/A"}\n" +
                                       $"Trạng thái: {order.TrangThai ?? "N/A"}\n\n" +
                                       "Hành động này không thể hoàn tác!", 
                                       "Xác nhận xóa", 
                                       MessageBoxButtons.YesNo, 
                                       MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var success = await _orderRepository.DeleteOrderAsync(order.MaDdh);
                    if (success)
                    {
                        MessageBox.Show($"Đã xóa đơn hàng {order.MaDdh} thành công!", 
                            "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadOrders(); // Reload data
                    }
                    else
                    {
                        MessageBox.Show("Không thể xóa đơn hàng!", 
                            "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa đơn hàng: {ex.Message}", 
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private System.Windows.Forms.Timer _searchTimer;
        private bool _isSearching = false;

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                // Tạo timer để tránh tìm kiếm quá nhiều lần
                if (_searchTimer == null)
                {
                    _searchTimer = new System.Windows.Forms.Timer();
                    _searchTimer.Interval = 500; // Delay 500ms
                    _searchTimer.Tick += async (s, args) =>
                    {
                        _searchTimer.Stop();
                        if (!_isSearching)
                        {
                            await ApplySearchOnly();
                        }
                    };
                }
                
                // Reset timer
                _searchTimer.Stop();
                _searchTimer.Start();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in TxtSearch_TextChanged: {ex.Message}");
            }
        }

        private async Task ApplySearchOnly()
        {
            if (_isSearching) return;
            
            try
            {
                _isSearching = true;
                string searchText = txtSearch.Text.Trim();
                
                if (!string.IsNullOrEmpty(searchText))
                {
                    // Sử dụng SearchOrdersAsync method đã có sẵn
                    var filteredOrders = await _orderRepository.SearchOrdersAsync(searchText);
                    RefreshDataGridView(filteredOrders);
                    
                    // Update title với số lượng kết quả
                    var resultCount = filteredOrders.Count;
                    this.Text = $"Quản lý Đơn hàng - Tìm thấy {resultCount} đơn hàng";
                }
                else
                {
                    // Nếu không có text tìm kiếm, load lại tất cả đơn hàng
                    LoadOrders();
                    this.Text = "Quản lý Đơn hàng";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ApplySearchOnly: {ex.Message}");
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isSearching = false;
            }
        }

        private async void CboStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            await ApplyFilters();
        }

        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            await ApplyFilters();
        }

        private async void BtnReset_Click(object sender, EventArgs e)
        {
            try
            {
                // Reset tất cả filters
                txtSearch.Text = "";
                cboStatusFilter.SelectedIndex = 0;
                dtpStartDate.Value = DateTime.Now.AddDays(-30);
                dtpEndDate.Value = DateTime.Now;
                
                // Reset title
                this.Text = "Quản lý Đơn hàng";
                
                // Load lại tất cả đơn hàng
                LoadOrders();
                
                MessageBox.Show("Đã reset bộ lọc và tải lại dữ liệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi reset bộ lọc: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadOrders();
            MessageBox.Show("Đã tải lại dữ liệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void DataGridViewOrders_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && 
                e.ColumnIndex != dataGridViewOrders.Columns["ViewDetails"].Index)
            {
                try
                {
                    var orderId = dataGridViewOrders.Rows[e.RowIndex].Cells["MaDdh"].Value.ToString();
                    var order = await _orderRepository.GetOrderByIdAsync(orderId);
                    if (order != null)
                    {
                        ViewOrderDetails(order);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xem chi tiết: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async Task ApplyFilters()
        {
            if (_isSearching) return;
            
            try
            {
                // Get filter values
                var searchTerm = txtSearch.Text.Trim();
                var status = cboStatusFilter.SelectedIndex > 0 ? cboStatusFilter.SelectedItem.ToString() : null;
                
                // Validation ngày tháng
                if (dtpStartDate.Value > dtpEndDate.Value)
                {
                    MessageBox.Show("Ngày bắt đầu không thể lớn hơn ngày kết thúc!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Sử dụng DateTime thay vì DateOnly để tránh lỗi LINQ translation
                var startDate = dtpStartDate.Value.Date;
                var endDate = dtpEndDate.Value.Date.AddDays(1).AddSeconds(-1); // Đến cuối ngày

                // Hiển thị thông báo đang tìm kiếm
                this.Cursor = Cursors.WaitCursor;
                btnSearch.Enabled = false;
                btnSearch.Text = "Đang tìm...";

                // Get filtered orders from repository - sử dụng method có sẵn
                var orders = await _orderRepository.GetOrdersByDateRangeAsync(startDate, endDate);
                
                // Nếu có search term, filter thêm theo text
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    orders = orders.Where(d => 
                        d.MaDdh.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (d.MaKhNavigation?.TenKh?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true) ||
                        (d.TrangThai?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true)
                    ).ToList();
                }
                
                // Nếu có status filter, filter thêm theo status
                if (!string.IsNullOrEmpty(status))
                {
                    orders = orders.Where(d => d.TrangThai == status).ToList();
                }

                RefreshDataGridView(orders);
                
                // Show result count
                var resultCount = orders.Count;
                this.Text = $"Quản lý Đơn hàng - Hiển thị {resultCount} đơn hàng";
                
                // Hiển thị thông báo kết quả
                if (resultCount == 0)
                {
                    MessageBox.Show("Không tìm thấy đơn hàng nào phù hợp với điều kiện tìm kiếm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi áp dụng bộ lọc: {ex.Message}", 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Khôi phục trạng thái
                this.Cursor = Cursors.Default;
                btnSearch.Enabled = true;
                btnSearch.Text = "🔍 Tìm kiếm";
            }
        }

        private async void BtnCreateOrder_Click(object sender, EventArgs e)
        {
            if (_isProcessing) return;
            
            try
            {
                _isProcessing = true;
                var addForm = new OrderAddEditForm(); // Sử dụng constructor mặc định để tạo đơn hàng mới
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    LoadOrders(); // Reload data after adding
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form thêm đơn hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private async void BtnEditOrder_Click(object sender, EventArgs e)
        {
            if (_isProcessing) return;
            
            if (dataGridViewOrders.SelectedRows.Count > 0)
            {
                try
                {
                    _isProcessing = true;
                    var selectedRow = dataGridViewOrders.SelectedRows[0];
                    var orderId = selectedRow.Cells["MaDdh"].Value?.ToString();
                    var order = await _orderRepository.GetOrderByIdAsync(orderId);
                    
                    if (order != null)
                    {
                        // Kiểm tra trạng thái đơn hàng
                        if (order.TrangThai == "Hoàn thành" || order.TrangThai == "Đã giao")
                        {
                            MessageBox.Show($"Không thể chỉnh sửa đơn hàng có trạng thái '{order.TrangThai}'!\n\n" +
                                          "Chỉ có thể xem chi tiết đơn hàng này.", 
                                          "Không thể chỉnh sửa", 
                                          MessageBoxButtons.OK, 
                                          MessageBoxIcon.Warning);
                            return;
                        }
                        
                        EditOrder(order);
                    }
                    else
                    {
                        MessageBox.Show("Không thể tải thông tin đơn hàng!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi tải thông tin đơn hàng: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    _isProcessing = false;
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn đơn hàng cần chỉnh sửa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void BtnDeleteOrder_Click(object sender, EventArgs e)
        {
            if (_isProcessing) return;

            if (dataGridViewOrders.SelectedRows.Count > 0)
            {
                try
                {
                    _isProcessing = true;
                    var selectedRow = dataGridViewOrders.SelectedRows[0];
                    var orderId = selectedRow.Cells["MaDdh"].Value?.ToString();
                    var order = await _orderRepository.GetOrderByIdAsync(orderId);

                    if (order != null)
                    {
                        // Kiểm tra trạng thái đơn hàng
                        if (order.TrangThai == "Hoàn thành" || order.TrangThai == "Đã giao")
                        {
                            MessageBox.Show($"Không thể xóa đơn hàng có trạng thái '{order.TrangThai}'!\n\n" +
                                          "Chỉ có thể xem chi tiết đơn hàng này.", 
                                          "Không thể xóa", 
                                          MessageBoxButtons.OK, 
                                          MessageBoxIcon.Warning);
                            return;
                        }
                        
                        DeleteOrder(order);
                    }
                    else
                    {
                        MessageBox.Show("Không thể tải thông tin đơn hàng để xóa!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi tải thông tin đơn hàng để xóa: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    _isProcessing = false;
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn đơn hàng cần xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ShowStatusChangeDialog(Dondathang order)
        {
            var statuses = new[] { "Chờ xử lý", "Đang xử lý", "Đã giao", "Đã hủy" };
            var currentStatus = order.TrangThai ?? "Chờ xử lý";
            
            var availableStatuses = statuses.Where(s => s != currentStatus).ToArray();
            
            if (availableStatuses.Length == 0)
            {
                MessageBox.Show("Đơn hàng này đã ở trạng thái cuối cùng!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var statusForm = new Form
            {
                Text = "Thay đổi trạng thái đơn hàng",
                Size = new Size(400, 200),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedSingle,
                MaximizeBox = false,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            var lblCurrent = new Label
            {
                Text = $"Trạng thái hiện tại: {currentStatus}",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(350, 25),
                Location = new Point(20, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblNew = new Label
            {
                Text = "Chọn trạng thái mới:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(200, 25),
                Location = new Point(20, 60),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var cboNewStatus = new ComboBox
            {
                Size = new Size(200, 30),
                Location = new Point(20, 90),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboNewStatus.Items.AddRange(availableStatuses);
            cboNewStatus.SelectedIndex = 0;

            var btnOK = new Button
            {
                Text = "OK",
                Size = new Size(80, 35),
                Location = new Point(200, 120),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            btnOK.FlatAppearance.BorderSize = 0;

            var btnCancel = new Button
            {
                Text = "Hủy",
                Size = new Size(80, 35),
                Location = new Point(300, 120),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            statusForm.Controls.AddRange(new Control[] { lblCurrent, lblNew, cboNewStatus, btnOK, btnCancel });

            if (statusForm.ShowDialog() == DialogResult.OK)
            {
                var newStatus = cboNewStatus.SelectedItem.ToString();
                _ = ChangeOrderStatus(order.MaDdh, newStatus);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _searchTimer?.Dispose();
                _context?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}