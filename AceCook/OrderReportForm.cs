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
        private DataGridView dataGridViewOrderReport;
        private DataGridView dataGridViewOrderSummary;
        private DataGridView dataGridViewOrderStatistics; // Bảng mới
        private TextBox txtSearch;
        private ComboBox cboStatusFilter;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private Button btnSearch;
        private Button btnReset;
        private Button btnExportReport;
        private Button btnRefresh;
        private Label lblTitle;
        private Label lblSearch;
        private Label lblStatusFilter;
        private Label lblDateRange;
        private Label lblTotalOrders;
        private Label lblTotalRevenue;
        private Label lblCompletedOrders;
        private Label lblPendingOrders;
        private Panel pnlFilters;
        private Panel pnlSummary;
        private Panel pnlActions;

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
            cboStatusFilter.Items.AddRange(new object[] { "Tất cả", "Đơn hàng mới", "Đã hoàn thành" });
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

            btnExportReport = new Button
            {
                Text = "📊 Xuất báo cáo",
                Size = new Size(200, 60),
                Location = new Point(0, 0),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnExportReport.FlatAppearance.BorderSize = 0;
            btnExportReport.Click += BtnExportReport_Click;

            btnRefresh = new Button
            {
                Text = "🔄 Làm mới dữ liệu",
                Size = new Size(200, 60),
                Location = new Point(220, 0),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += BtnRefresh_Click;

            pnlActions.Controls.AddRange(new Control[] { btnExportReport, btnRefresh });

            // Main DataGridView for detailed report
            dataGridViewOrderReport = new DataGridView
            {
                Size = new Size(1340, 300),
                Location = new Point(30, 370),
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
                Size = new Size(1340, 120),
                Location = new Point(30, 690),
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

            // New Statistics DataGridView with 4 columns
            dataGridViewOrderStatistics = new DataGridView
            {
                Size = new Size(1340, 100),
                Location = new Point(30, 830), // Below the existing summary table
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

            // Style the DataGridViews
            StyleDataGridView(dataGridViewOrderReport);
            StyleDataGridView(dataGridViewOrderSummary);
            StyleDataGridView(dataGridViewOrderStatistics); // Style the new table

            // Add controls to form - update to include new table
            this.Controls.AddRange(new Control[] { 
                lblTitle, pnlFilters, pnlSummary, pnlActions, 
                dataGridViewOrderReport, dataGridViewOrderSummary, dataGridViewOrderStatistics // Add new table
            });

            // Increase form height to accommodate new table
            this.Size = new Size(1400, 980);
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

        private async Task RefreshReportData(List<Dondathang> orders)
        {
            // Update summary statistics
            var totalOrders = orders.Count;
            var completedOrders = orders.Count(o => o.TrangThai == "Đã giao");
            var pendingOrders = orders.Count(o => o.TrangThai == "Đang xử lý" || o.TrangThai == "Chờ xử lý");
            
            decimal totalRevenue = 0;
            foreach (var order in orders.Where(o => o.TrangThai == "Đã giao"))
            {
                if (order.CtDhs != null)
                {
                    totalRevenue += order.CtDhs.Sum(ct => (decimal)((ct.SoLuong ?? 0) * (ct.DonGia ?? 0)));
                }
            }

            lblTotalOrders.Text = $"Tổng đơn hàng: {totalOrders}";
            lblCompletedOrders.Text = $"Đã hoàn thành: {completedOrders}";
            lblPendingOrders.Text = $"Đang xử lý: {pendingOrders}";
            lblTotalRevenue.Text = $"Tổng doanh thu: {totalRevenue:N0} VNĐ";

            // Refresh detailed report
            RefreshDetailedReport(orders);
            
            // Refresh summary by status
            RefreshSummaryReport(orders);
            
            // Refresh new statistics report
            RefreshStatisticsReport(orders);
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
                row.Cells["TrangThai"].Value = order.TrangThai ?? "Đơn hàng mới";
                
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
                StyleStatusCell(row.Cells["TrangThai"], order.TrangThai);
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
            var statusGroups = orders.GroupBy(o => o.TrangThai ?? "Đơn hàng mới");
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
            if (status == "Đã hoàn thành")
            {
                cell.Style.ForeColor = Color.Green;
                cell.Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            else if (status == "Đơn hàng mới")
            {
                cell.Style.ForeColor = Color.Blue;
                cell.Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            else
            {
                // Default style for any other status
                cell.Style.ForeColor = Color.Black;
                cell.Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
        }

        private async void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                LoadReportData();
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
            txtSearch.Text = "";
            cboStatusFilter.SelectedIndex = 0;
            dtpStartDate.Value = DateTime.Now.AddDays(-30);
            dtpEndDate.Value = DateTime.Now;
            await ApplyFilters();
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadReportData();
        }

        private async void BtnExportReport_Click(object sender, EventArgs e)
        {
            try
            {
                // This would be implemented to export to Excel or PDF
                MessageBox.Show("Chức năng xuất báo cáo sẽ được triển khai trong phiên bản tiếp theo!", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất báo cáo: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ApplyFilters()
        {
            try
            {
                List<Dondathang> orders;

                // Apply status filter
                if (cboStatusFilter.SelectedIndex > 0)
                {
                    var status = cboStatusFilter.SelectedItem.ToString();
                    orders = await _orderRepository.GetOrdersByStatusAsync(status);
                }
                else
                {
                    orders = await _orderRepository.GetAllOrdersAsync();
                }

                // Apply date range filter
                if (dtpStartDate.Value <= dtpEndDate.Value)
                {
                    orders = orders.Where(o => o.NgayDat.HasValue &&
                        o.NgayDat.Value.ToDateTime(TimeOnly.MinValue) >= dtpStartDate.Value &&
                        o.NgayDat.Value.ToDateTime(TimeOnly.MinValue) <= dtpEndDate.Value.AddDays(1).AddSeconds(-1))
                        .ToList();
                }

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    var searchTerm = txtSearch.Text.ToLower();
                    orders = orders.Where(o => 
                        o.MaDdh.ToLower().Contains(searchTerm) ||
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

        // New method to populate the statistics table with 4 columns
        private void RefreshStatisticsReport(List<Dondathang> orders)
        {
            dataGridViewOrderStatistics.DataSource = null;
            dataGridViewOrderStatistics.Columns.Clear();

            // Create 4 columns for the new statistics table
            dataGridViewOrderStatistics.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TrangThai",
                HeaderText = "Trạng thái",
                Width = 335
            });

            dataGridViewOrderStatistics.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SoLuong",
                HeaderText = "Số lượng",
                Width = 335
            });

            dataGridViewOrderStatistics.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TyLe",
                HeaderText = "Tỷ lệ (%)",
                Width = 335
            });

            dataGridViewOrderStatistics.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DoanhThu",
                HeaderText = "Doanh thu",
                Width = 335
            });

            // Group orders by status and populate data
            var statusGroups = orders.GroupBy(o => o.TrangThai ?? "Đơn hàng mới");
            var totalOrders = orders.Count;

            foreach (var group in statusGroups)
            {
                var rowIndex = dataGridViewOrderStatistics.Rows.Add();
                var row = dataGridViewOrderStatistics.Rows[rowIndex];

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

                row.Cells["TrangThai"].Value = group.Key;
                row.Cells["SoLuong"].Value = count;
                row.Cells["TyLe"].Value = $"{percentage:F1}%";
                row.Cells["DoanhThu"].Value = revenue.ToString("N0") + " VNĐ";

                // Apply styling based on status
                StyleStatisticsRow(row, group.Key);
            }
        }

        // Method to style the new statistics table rows
        private void StyleStatisticsRow(DataGridViewRow row, string status)
        {
            if (status == "Hoàn thành")
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(240, 255, 240);
                row.Cells["TrangThai"].Style.ForeColor = Color.Green;
                row.Cells["TrangThai"].Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            else if (status == "Đơn hàng mới")
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
                row.Cells["TrangThai"].Style.ForeColor = Color.Blue;
                row.Cells["TrangThai"].Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            else
            {
                row.DefaultCellStyle.BackColor = Color.White;
                row.Cells["TrangThai"].Style.ForeColor = Color.Black;
                row.Cells["TrangThai"].Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
        }
    }
}