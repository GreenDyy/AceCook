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
    public partial class OrderReportForm : Form
    {
        private readonly AppDbContext _context;
        private readonly OrderRepository _orderRepository;
        private readonly ReportRepository _reportRepository;
        private DataGridView dataGridViewOrderReport = null!;
        private DataGridView dataGridViewOrderSummary = null!;
        private DataGridView dataGridViewOrderStatistics = null!; // Bảng mới
        private TextBox txtSearch = null!;
        private ComboBox cboStatusFilter = null!;
        private DateTimePicker dtpStartDate = null!;
        private DateTimePicker dtpEndDate = null!;
        private Button btnSearch = null!;
        private Button btnReset = null!;
        private Label lblTitle = null!;
        private Label lblSearch = null!;
        private Label lblStatusFilter = null!;
        private Label lblDateRange = null!;
        private Label lblTotalOrders = null!;
        private Label lblTotalRevenue = null!;
        private Label lblCompletedOrders = null!;
        private Label lblPendingOrders = null!;
        private Panel pnlFilters = null!;
        private Panel pnlSummary = null!;
        private Panel pnlActions = null!;

        public OrderReportForm(OrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            _context = new AppDbContext();
            _reportRepository = new ReportRepository(_context);
            InitializeComponent();
            SetupUI();
            LoadReportData();
        }



        private void SetupUI()
        {
            // Title
            lblTitle = new Label
            {
                Text = "BÁO CÁO ĐƠN HÀNG",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(800, 50),
                Location = new Point(30, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Filters Panel
            pnlFilters = new Panel
            {
                Size = new Size(1340, 80),
                Location = new Point(30, 90),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Search controls
            lblSearch = new Label
            {
                Text = "Tìm kiếm:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(80, 25),
                Location = new Point(20, 15),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtSearch = new TextBox
            {
                Size = new Size(200, 30),
                Location = new Point(110, 12),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "Mã đơn hàng, tên KH..."
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            // Status filter
            lblStatusFilter = new Label
            {
                Text = "Trạng thái:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(80, 25),
                Location = new Point(330, 15),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cboStatusFilter = new ComboBox
            {
                Size = new Size(150, 30),
                Location = new Point(420, 12),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboStatusFilter.Items.AddRange(new object[] { "Tất cả", "Đang xử lý", "Chờ xử lý", "Đã giao" });
            cboStatusFilter.SelectedIndex = 0;
            cboStatusFilter.SelectedIndexChanged += CboStatusFilter_SelectedIndexChanged;

            // Date range
            lblDateRange = new Label
            {
                Text = "Từ ngày:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(70, 25),
                Location = new Point(590, 15),
                TextAlign = ContentAlignment.MiddleLeft
            };

            dtpStartDate = new DateTimePicker
            {
                Size = new Size(130, 30),
                Location = new Point(670, 12),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddDays(-30)
            };

            Label lblToDate = new Label
            {
                Text = "đến:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(40, 25),
                Location = new Point(820, 15),
                TextAlign = ContentAlignment.MiddleLeft
            };

            dtpEndDate = new DateTimePicker
            {
                Size = new Size(130, 30),
                Location = new Point(870, 12),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };

            // Filter buttons
            btnSearch = new Button
            {
                Text = "🔍 Tìm kiếm",
                Size = new Size(100, 35),
                Location = new Point(1020, 12),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += BtnSearch_Click;

            btnReset = new Button
            {
                Text = "🔄 Làm mới",
                Size = new Size(100, 35),
                Location = new Point(1130, 12),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(95, 95, 95),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.Click += BtnReset_Click;

            // Add controls to filters panel
            pnlFilters.Controls.AddRange(new Control[] { 
                lblSearch, txtSearch, lblStatusFilter, cboStatusFilter,
                lblDateRange, dtpStartDate, lblToDate, dtpEndDate,
                btnSearch, btnReset
            });

            // Summary Panel
            pnlSummary = new Panel
            {
                Size = new Size(1340, 80),
                Location = new Point(30, 190),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Summary labels
            lblTotalOrders = new Label
            {
                Text = "Tổng đơn hàng: 0",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(200, 30),
                Location = new Point(20, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblCompletedOrders = new Label
            {
                Text = "Đã hoàn thành: 0",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Green,
                Size = new Size(200, 30),
                Location = new Point(240, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblPendingOrders = new Label
            {
                Text = "Đơn hàng mới: 0",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Orange,
                Size = new Size(200, 30),
                Location = new Point(460, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblTotalRevenue = new Label
            {
                Text = "Tổng doanh thu: 0 VNĐ",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113),
                Size = new Size(300, 30),
                Location = new Point(680, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            pnlSummary.Controls.AddRange(new Control[] { 
                lblTotalOrders, lblCompletedOrders, lblPendingOrders, lblTotalRevenue
            });

            // Actions Panel
            pnlActions = new Panel
            {
                Size = new Size(1340, 60),
                Location = new Point(30, 290),
                BackColor = Color.Transparent
            };

            // Main DataGridView for detailed report
            dataGridViewOrderReport = new DataGridView
            {
                Size = new Size(1340, 250), // Giảm từ 300 xuống 250
                Location = new Point(30, 290), // Thay đổi từ 370 xuống 290 để loại bỏ khoảng trống
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

            // Summary DataGridView for statistics (existing)
            dataGridViewOrderSummary = new DataGridView
            {
                Size = new Size(1340, 100), // Giảm từ 120 xuống 100
                Location = new Point(30, 560), // Điều chỉnh: 290 + 250 + 20 = 560
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
                ColumnHeadersHeight = 40,
                RowTemplate = { Height = 40 }
            };

            // New Statistics DataGridView - Top Customers
            dataGridViewOrderStatistics = new DataGridView
            {
                Size = new Size(1340, 150), // Giảm từ 200 xuống 150
                Location = new Point(30, 680), // Điều chỉnh: 560 + 100 + 20 = 680
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
                ColumnHeadersHeight = 35, // Giảm từ 40 xuống 35
                RowTemplate = { Height = 35 } // Giảm từ 40 xuống 35
            };

            // Style the DataGridViews
            StyleDataGridView(dataGridViewOrderReport);
            StyleDataGridView(dataGridViewOrderSummary);
            StyleDataGridView(dataGridViewOrderStatistics); // Style the new table

            // Add controls to form - update to include new table
            this.Controls.AddRange(new Control[] { 
                lblTitle, pnlFilters, pnlSummary, // pnlActions, // Comment out để ẩn panel
                dataGridViewOrderReport, dataGridViewOrderSummary, dataGridViewOrderStatistics // Add new table
            });

            // Adjust form height to fit screen better
            this.Size = new Size(1400, 870); // Điều chỉnh từ 950 xuống 870 do đã loại bỏ khoảng trống
        }

        private void StyleDataGridView(DataGridView dgv)
        {
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        private async void LoadReportData()
        {
            try
            {
                var orders = await _orderRepository.GetAllOrdersAsync();
                await RefreshReportData(orders);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu báo cáo: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Task RefreshReportData(List<Dondathang> orders)
        {
            // Update summary statistics
            var totalOrders = orders.Count;
            var completedOrders = orders.Count(o => o.TrangThai == "Đã giao");
            var pendingOrders = orders.Count(o => o.TrangThai == "Đang xử lý" || o.TrangThai == "Chờ xử lý");
            var newOrders = orders.Count(o => string.IsNullOrEmpty(o.TrangThai) || o.TrangThai == "Mới");
            
            decimal totalRevenue = 0;
            foreach (var order in orders.Where(o => o.TrangThai == "Đã giao"))
            {
                if (order.CtDhs != null)
                {
                    totalRevenue += order.CtDhs.Sum(ct => (decimal)((ct.SoLuong ?? 0) * (ct.DonGia ?? 0)));
                }
            }

            lblTotalOrders.Text = $"Tổng đơn hàng: {totalOrders}";
            lblCompletedOrders.Text = $"Đã giao: {completedOrders}";
            lblPendingOrders.Text = $"Đang xử lý: {pendingOrders}";
            lblTotalRevenue.Text = $"Tổng doanh thu: {totalRevenue:N0} VNĐ";

            // Refresh detailed report
            RefreshDetailedReport(orders);
            
            // Refresh summary by status
            RefreshSummaryReport(orders);
            
            // Refresh new statistics report
            RefreshStatisticsReport(orders);
            
            return Task.CompletedTask;
        }

        private void RefreshDetailedReport(List<Dondathang> orders)
        {
            dataGridViewOrderReport.DataSource = null;
            dataGridViewOrderReport.Columns.Clear();

            // Create custom columns for detailed report
            dataGridViewOrderReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaDdh",
                HeaderText = "Mã đơn hàng",
                Width = 120
            });

            dataGridViewOrderReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CustomerInfo",
                HeaderText = "Khách hàng",
                Width = 200
            });

            dataGridViewOrderReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NgayDat",
                HeaderText = "Ngày đặt",
                Width = 120
            });

            dataGridViewOrderReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NgayGiao",
                HeaderText = "Ngày giao",
                Width = 120
            });

            dataGridViewOrderReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TrangThai",
                HeaderText = "Trạng thái",
                Width = 120
            });

            dataGridViewOrderReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalAmount",
                HeaderText = "Tổng tiền",
                Width = 120
            });

            dataGridViewOrderReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ItemCount",
                HeaderText = "Số mặt hàng",
                Width = 100
            });

            // Populate data
            foreach (var order in orders)
            {
                var rowIndex = dataGridViewOrderReport.Rows.Add();
                var row = dataGridViewOrderReport.Rows[rowIndex];

                row.Cells["MaDdh"].Value = order.MaDdh;
                row.Cells["CustomerInfo"].Value = order.MaKhNavigation?.TenKh ?? "N/A";
                row.Cells["NgayDat"].Value = order.NgayDat?.ToString("dd/MM/yyyy");
                row.Cells["NgayGiao"].Value = order.NgayGiao?.ToString("dd/MM/yyyy") ?? "Chưa giao";
                row.Cells["TrangThai"].Value = order.TrangThai ?? "Mới";
                
                // Calculate total amount
                decimal totalAmount = 0;
                int itemCount = 0;
                if (order.CtDhs != null)
                {
                    totalAmount = order.CtDhs.Sum(ct => (decimal)((ct.SoLuong ?? 0) * (ct.DonGia ?? 0)));
                    itemCount = order.CtDhs.Count;
                }
                row.Cells["TotalAmount"].Value = totalAmount.ToString("N0") + " VNĐ";
                row.Cells["ItemCount"].Value = itemCount;

                // Style status column
                StyleStatusCell(row.Cells["TrangThai"], order.TrangThai ?? "Mới");
            }
        }

        private void RefreshSummaryReport(List<Dondathang> orders)
        {
            dataGridViewOrderSummary.DataSource = null;
            dataGridViewOrderSummary.Columns.Clear();

            // Create columns for summary report
            dataGridViewOrderSummary.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Trạng thái",
                Width = 200
            });

            dataGridViewOrderSummary.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Count",
                HeaderText = "Số lượng",
                Width = 150
            });

            dataGridViewOrderSummary.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Percentage",
                HeaderText = "Tỷ lệ (%)",
                Width = 150
            });

            dataGridViewOrderSummary.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Revenue",
                HeaderText = "Doanh thu",
                Width = 200
            });

            // Group by status
            var statusGroups = orders.GroupBy(o => o.TrangThai ?? "Mới");
            var totalOrders = orders.Count;

            foreach (var group in statusGroups)
            {
                var rowIndex = dataGridViewOrderSummary.Rows.Add();
                var row = dataGridViewOrderSummary.Rows[rowIndex];

                var count = group.Count();
                var percentage = totalOrders > 0 ? (count * 100.0 / totalOrders) : 0;
                
                decimal revenue = 0;
                foreach (var order in group)
                {
                    if (order.CtDhs != null)
                    {
                        revenue += order.CtDhs.Sum(ct => (decimal)((ct.SoLuong ?? 0) * (ct.DonGia ?? 0)));
                    }
                }

                row.Cells["Status"].Value = group.Key;
                row.Cells["Count"].Value = count;
                row.Cells["Percentage"].Value = $"{percentage:F1}%";
                row.Cells["Revenue"].Value = revenue.ToString("N0") + " VNĐ";

                // Style status column
                StyleStatusCell(row.Cells["Status"], group.Key);
            }
        }

        private void StyleStatusCell(DataGridViewCell cell, string status)
        {
            if (status == "Đã giao")
            {
                cell.Style.ForeColor = Color.Green;
                cell.Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            else if (status == "Đang xử lý")
            {
                cell.Style.ForeColor = Color.Orange;
                cell.Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            else if (status == "Chờ xử lý")
            {
                cell.Style.ForeColor = Color.Blue;
                cell.Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            else if (string.IsNullOrEmpty(status) || status == "Mới")
            {
                cell.Style.ForeColor = Color.DarkBlue;
                cell.Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            else
            {
                // Default style for any other status
                cell.Style.ForeColor = Color.Black;
                cell.Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                LoadReportData();
            }
        }

        private async void CboStatusFilter_SelectedIndexChanged(object? sender, EventArgs e)
        {
            await ApplyFilters();
        }

        private async void BtnSearch_Click(object? sender, EventArgs e)
        {
            await ApplyFilters();
        }

        private async void BtnReset_Click(object? sender, EventArgs e)
        {
            txtSearch.Text = "";
            cboStatusFilter.SelectedIndex = 0;
            dtpStartDate.Value = DateTime.Now.AddDays(-30);
            dtpEndDate.Value = DateTime.Now;
            await ApplyFilters();
        }

        private async Task ApplyFilters()
        {
            try
            {
                // Validate date range first
                if (dtpStartDate.Value > dtpEndDate.Value)
                {
                    MessageBox.Show("Ngày bắt đầu không thể lớn hơn ngày kết thúc!", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<Dondathang> orders;

                // Apply status filter
                if (cboStatusFilter.SelectedIndex > 0)
                {
                    var status = cboStatusFilter.SelectedItem?.ToString();
                    if (!string.IsNullOrEmpty(status))
                    {
                        orders = await _orderRepository.GetOrdersByStatusAsync(status);
                    }
                    else
                    {
                        orders = await _orderRepository.GetAllOrdersAsync();
                    }
                }
                else
                {
                    orders = await _orderRepository.GetAllOrdersAsync();
                }

                // Apply date range filter - Alternative approach
                if (dtpStartDate.Value <= dtpEndDate.Value)
                {
                    var startDate = dtpStartDate.Value.Date;
                    var endDate = dtpEndDate.Value.Date;
                    
                    // Filter in memory after converting to list
                    orders = orders.ToList().Where(o => o.NgayDat.HasValue)
                        .Where(o => 
                        {
                            // Convert DateOnly to DateTime for comparison
                            var orderDateTime = new DateTime(o.NgayDat.Value.Year, 
                                                           o.NgayDat.Value.Month, 
                                                           o.NgayDat.Value.Day);
                            return orderDateTime >= startDate && orderDateTime <= endDate;
                        })
                        .ToList();
                }

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    var searchTerm = txtSearch.Text.ToLower();
                    orders = orders.Where(o => 
                        (o.MaDdh?.ToLower().Contains(searchTerm) ?? false) ||
                        (o.MaKhNavigation?.TenKh?.ToLower().Contains(searchTerm) ?? false) ||
                        (o.TrangThai?.ToLower().Contains(searchTerm) ?? false)
                    ).ToList();
                }

                await RefreshReportData(orders);
                
                // Show result count
                var resultCount = orders.Count;
                var totalCount = await _orderRepository.GetTotalOrdersAsync(DateTime.MinValue, DateTime.MaxValue);
                this.Text = $"Báo cáo Đơn hàng - Hiển thị {resultCount}/{totalCount} đơn hàng";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi áp dụng bộ lọc: {ex.Message}", 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Method to populate the statistics table with Top Customers (3 columns)
        private void RefreshStatisticsReport(List<Dondathang> orders)
        {
            dataGridViewOrderStatistics.DataSource = null;
            dataGridViewOrderStatistics.Columns.Clear();

            // Create 3 columns for Top Customers table
            dataGridViewOrderStatistics.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenKhachHang",
                HeaderText = "Khách hàng",
                Width = 447  // 1340/3 ≈ 447
            });

            dataGridViewOrderStatistics.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SoDonHang",
                HeaderText = "Số đơn hàng",
                Width = 447
            });

            dataGridViewOrderStatistics.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TongGiaTri",
                HeaderText = "Tổng giá trị",
                Width = 446
            });

            // Note: orders parameter already contains filtered data from ApplyFilters(),
            // so we don't need to apply date filter again here
            var filteredOrders = orders;

            // Group orders by customer and calculate statistics
            var topCustomers = filteredOrders
                .Where(o => o.MaKhNavigation != null) // Only orders with customer info
                .GroupBy(o => new { 
                    MaKH = o.MaKh, 
                    TenKH = o.MaKhNavigation?.TenKh ?? "Khách hàng không xác định" 
                })
                .Select(g => new
                {
                    MaKH = g.Key.MaKH,
                    TenKH = g.Key.TenKH,
                    SoDonHang = g.Count(),
                    TongGiaTri = g.Sum(order => 
                        order.CtDhs?.Sum(ct => (decimal)((ct.SoLuong ?? 0) * (ct.DonGia ?? 0))) ?? 0)
                })
                .OrderByDescending(x => x.SoDonHang) // Sort by number of orders descending
                .Take(10) // Take top 10 customers
                .ToList();

            // Populate data rows
            foreach (var customer in topCustomers)
            {
                var rowIndex = dataGridViewOrderStatistics.Rows.Add();
                var row = dataGridViewOrderStatistics.Rows[rowIndex];

                row.Cells["TenKhachHang"].Value = customer.TenKH;
                row.Cells["SoDonHang"].Value = customer.SoDonHang;
                row.Cells["TongGiaTri"].Value = customer.TongGiaTri.ToString("N0") + " VNĐ";

                // Apply styling for top customers
                StyleTopCustomerRow(row, customer.SoDonHang);
            }
        }

        // Method to style the top customer table rows
        private void StyleTopCustomerRow(DataGridViewRow row, int orderCount)
        {
            // Style based on number of orders
            if (orderCount >= 5)
            {
                // VIP customers (5+ orders) - Gold background
                row.DefaultCellStyle.BackColor = Color.FromArgb(255, 248, 220);
                row.Cells["TenKhachHang"].Style.ForeColor = Color.FromArgb(184, 134, 11);
                row.Cells["TenKhachHang"].Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                row.Cells["SoDonHang"].Style.ForeColor = Color.FromArgb(184, 134, 11);
                row.Cells["SoDonHang"].Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            else if (orderCount >= 3)
            {
                // Good customers (3-4 orders) - Light blue background
                row.DefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
                row.Cells["TenKhachHang"].Style.ForeColor = Color.FromArgb(37, 99, 235);
                row.Cells["TenKhachHang"].Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                row.Cells["SoDonHang"].Style.ForeColor = Color.FromArgb(37, 99, 235);
                row.Cells["SoDonHang"].Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            else
            {
                // Regular customers (1-2 orders) - White background
                row.DefaultCellStyle.BackColor = Color.White;
                row.Cells["TenKhachHang"].Style.ForeColor = Color.Black;
                row.Cells["TenKhachHang"].Style.Font = new Font("Segoe UI", 9, FontStyle.Regular);
                row.Cells["SoDonHang"].Style.ForeColor = Color.Black;
                row.Cells["SoDonHang"].Style.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            }

            // Always style the total value in green
            row.Cells["TongGiaTri"].Style.ForeColor = Color.FromArgb(34, 197, 94);
            row.Cells["TongGiaTri"].Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        }
    }
}