using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AceCook.Models;
using AceCook.Helpers;

namespace AceCook
{
    public partial class DashboardContent : UserControl
    {
        private AppDbContext _context;
        private Panel summaryPanel;
        private Panel chartPanel;
        private Panel detailPanel;

        public DashboardContent(AppDbContext context)
        {
            _context = context;
            SetupUI();
            LoadDashboardData();
        }

        private void SetupUI()
        {
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.Dock = DockStyle.Fill;

            // Summary Cards Panel
            summaryPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            CreateSummaryCards();

            // Chart Panel
            chartPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 400,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            CreateChartPanels();

            // Detail Panel
            detailPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            CreateDetailPanel();

            this.Controls.Add(detailPanel);
            this.Controls.Add(chartPanel);
            this.Controls.Add(summaryPanel);
        }

        private void CreateSummaryCards()
        {
            // Tạo FlowLayoutPanel để sắp xếp các cards tự động
            var flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(20, 10, 20, 10),
                BackColor = Color.FromArgb(248, 249, 250)
            };

            // Total Revenue Card
            var revenueCard = CreateSummaryCard("Tổng doanh thu", "960,000 ₫", Color.FromArgb(155, 89, 182));
            
            // Total Invoices Card
            var invoiceCard = CreateSummaryCard("Tổng hóa đơn", "1", Color.FromArgb(46, 204, 113));
            
            // Average per Invoice Card
            var avgCard = CreateSummaryCard("Trung bình/hóa đơn", "960,000 ₫", Color.FromArgb(52, 152, 219));
            
            // Days with Transactions Card
            var daysCard = CreateSummaryCard("Ngày có giao dịch", "1", Color.FromArgb(231, 76, 60));

            // Thêm các cards vào FlowLayoutPanel
            flowPanel.Controls.Add(revenueCard);
            flowPanel.Controls.Add(invoiceCard);
            flowPanel.Controls.Add(avgCard);
            flowPanel.Controls.Add(daysCard);

            summaryPanel.Controls.Add(flowPanel);
        }

        private Panel CreateSummaryCard(string title, string value, Color color)
        {
            var card = new Panel
            {
                Size = new Size(280, 100),
                Margin = new Padding(10, 5, 10, 5),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            // Icon
            var iconLabel = new Label
            {
                Text = "📊",
                Font = new Font("Segoe UI", 24),
                ForeColor = color,
                Size = new Size(50, 50),
                Location = new Point(20, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Value
            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(200, 30),
                Location = new Point(80, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Title
            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(127, 140, 141),
                Size = new Size(200, 20),
                Location = new Point(80, 55),
                TextAlign = ContentAlignment.MiddleLeft
            };

            card.Controls.Add(iconLabel);
            card.Controls.Add(valueLabel);
            card.Controls.Add(titleLabel);

            // Add shadow effect
            card.Paint += (sender, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, card.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid);
            };

            return card;
        }

        private void CreateChartPanels()
        {
            // Tạo TableLayoutPanel để sắp xếp 2 charts cạnh nhau
            var tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(20, 10, 20, 10)
            };

            // Cấu hình columns
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            // Daily Revenue Chart
            var dailyChartPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5, 5, 5, 5),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            var dailyChartTitle = new Label
            {
                Text = "Biểu đồ doanh thu theo ngày",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(300, 30),
                Location = new Point(20, 20)
            };

            var dailyChartSubtitle = new Label
            {
                Text = "Thống kê doanh thu từ 01/08/2025 đến 31/08/2025",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(127, 140, 141),
                Size = new Size(400, 20),
                Location = new Point(20, 50)
            };

            // Simple chart representation
            var chartArea = new Panel
            {
                Size = new Size(540, 280),
                Location = new Point(20, 80),
                BackColor = Color.FromArgb(248, 249, 250)
            };

            // Chart line
            var chartLine = new Panel
            {
                Size = new Size(2, 200),
                Location = new Point(50, 40),
                BackColor = Color.FromArgb(52, 152, 219)
            };

            // Data point
            var dataPoint = new Panel
            {
                Size = new Size(8, 8),
                Location = new Point(46, 40),
                BackColor = Color.FromArgb(52, 152, 219)
            };

            // Y-axis labels
            var yAxis1 = new Label { Text = "1,000,000 ₫", Location = new Point(10, 20), Size = new Size(80, 20), Font = new Font("Segoe UI", 8) };
            var yAxis2 = new Label { Text = "500,000 ₫", Location = new Point(10, 120), Size = new Size(80, 20), Font = new Font("Segoe UI", 8) };
            var yAxis3 = new Label { Text = "0 ₫", Location = new Point(10, 220), Size = new Size(80, 20), Font = new Font("Segoe UI", 8) };

            // X-axis label
            var xAxisLabel = new Label { Text = "05/08", Location = new Point(40, 250), Size = new Size(80, 20), Font = new Font("Segoe UI", 8) };

            chartArea.Controls.Add(chartLine);
            chartArea.Controls.Add(dataPoint);
            chartArea.Controls.Add(yAxis1);
            chartArea.Controls.Add(yAxis2);
            chartArea.Controls.Add(yAxis3);
            chartArea.Controls.Add(xAxisLabel);

            dailyChartPanel.Controls.Add(dailyChartTitle);
            dailyChartPanel.Controls.Add(dailyChartSubtitle);
            dailyChartPanel.Controls.Add(chartArea);

            // Monthly Revenue Chart
            var monthlyChartPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5, 5, 5, 5),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            var monthlyChartTitle = new Label
            {
                Text = "Doanh thu theo tháng",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(300, 30),
                Location = new Point(20, 20)
            };

            var monthlyChartSubtitle = new Label
            {
                Text = "Thống kê năm 2025",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(127, 140, 141),
                Size = new Size(400, 20),
                Location = new Point(20, 50)
            };

            // Simple bar chart representation
            var barChartArea = new Panel
            {
                Size = new Size(540, 280),
                Location = new Point(20, 80),
                BackColor = Color.FromArgb(248, 249, 250)
            };

            // Bar for August
            var augustBar = new Panel
            {
                Size = new Size(30, 200),
                Location = new Point(200, 40),
                BackColor = Color.FromArgb(46, 204, 113)
            };

            // Month labels
            for (int i = 0; i < 12; i++)
            {
                var monthLabel = new Label
                {
                    Text = $"Tháng {i + 1}",
                    Location = new Point(50 + i * 40, 250),
                    Size = new Size(40, 20),
                    Font = new Font("Segoe UI", 7),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                barChartArea.Controls.Add(monthLabel);
            }

            barChartArea.Controls.Add(augustBar);

            monthlyChartPanel.Controls.Add(monthlyChartTitle);
            monthlyChartPanel.Controls.Add(monthlyChartSubtitle);
            monthlyChartPanel.Controls.Add(barChartArea);

            // Thêm charts vào TableLayoutPanel
            tableLayout.Controls.Add(dailyChartPanel, 0, 0);
            tableLayout.Controls.Add(monthlyChartPanel, 1, 0);

            // Thêm TableLayoutPanel vào chartPanel
            chartPanel.Controls.Add(tableLayout);

            // Add shadow effects
            dailyChartPanel.Paint += (sender, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, dailyChartPanel.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid);
            };

            monthlyChartPanel.Paint += (sender, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, monthlyChartPanel.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid);
            };
        }

        private void CreateDetailPanel()
        {
            // Tạo Panel chính cho detail
            var detailMainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 20, 20, 20),
                BackColor = Color.FromArgb(248, 249, 250)
            };

            var detailTitle = new Label
            {
                Text = "Chi tiết doanh thu theo ngày",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(300, 30),
                Location = new Point(0, 0),
                AutoSize = true
            };

            var detailSubtitle = new Label
            {
                Text = "Danh sách doanh thu từng ngày trong khoảng thời gian",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(127, 140, 141),
                Size = new Size(400, 20),
                Location = new Point(0, 40),
                AutoSize = true
            };

            // DataGridView for daily revenue details
            var dataGridView = new DataGridView
            {
                Dock = DockStyle.Bottom,
                Height = 300,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                GridColor = Color.LightGray
            };

            // Add sample data
            dataGridView.Columns.Add("Ngay", "NGÀY");
            dataGridView.Columns.Add("SoHoaDon", "SỐ HÓA ĐƠN");
            dataGridView.Columns.Add("DoanhThu", "DOANH THU");
            dataGridView.Columns.Add("TrungBinh", "TRUNG BÌNH/HD");

            dataGridView.Rows.Add("05/08/2025", "1", "960,000 ₫", "960,000 ₫");

            // Thêm controls vào detailMainPanel
            detailMainPanel.Controls.Add(detailTitle);
            detailMainPanel.Controls.Add(detailSubtitle);
            detailMainPanel.Controls.Add(dataGridView);

            // Thêm detailMainPanel vào detailPanel
            detailPanel.Controls.Add(detailMainPanel);
        }

        private async void LoadDashboardData()
        {
            try
            {
                // Load real data from database
                await LoadSummaryData();
                await LoadChartData();
                await LoadDetailData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu dashboard: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadSummaryData()
        {
            try
            {
                // Load total revenue - Using DateHelper for safe DateOnly operations
                var (startDate, endDate) = DateHelper.GetLast30Days();
                var totalRevenue = await _context.Hoadonbans
                    .Where(h => h.NgayLap.HasValue && h.NgayLap.Value >= startDate)
                    .SumAsync(h => h.TongTien ?? 0);

                // Load total invoices
                var totalInvoices = await _context.Hoadonbans
                    .Where(h => h.NgayLap.HasValue && h.NgayLap.Value >= startDate)
                    .CountAsync();

                // Update summary cards
                UpdateSummaryCard(0, $"{totalRevenue:N0} ₫");
                UpdateSummaryCard(1, totalInvoices.ToString());
                UpdateSummaryCard(2, totalInvoices > 0 ? $"{totalRevenue / totalInvoices:N0} ₫" : "0 ₫");
            }
            catch (Exception ex)
            {
                // Fallback to default values if database query fails
                UpdateSummaryCard(0, "0 ₫");
                UpdateSummaryCard(1, "0");
                UpdateSummaryCard(2, "0 ₫");
                
                // Log error for debugging
                System.Diagnostics.Debug.WriteLine($"LoadSummaryData error: {ex.Message}");
            }
        }

        private async Task LoadChartData()
        {
            try
            {
                // Load daily revenue data for the last 30 days - Using DateHelper
                var (startDate, endDate) = DateHelper.GetLast30Days();
                var dailyRevenue = await _context.Hoadonbans
                    .Where(h => h.NgayLap.HasValue && h.NgayLap.Value >= startDate)
                    .GroupBy(h => h.NgayLap.Value)
                    .Select(g => new { Date = g.Key, Revenue = g.Sum(h => h.TongTien ?? 0) })
                    .ToListAsync();

                // Update chart data
                UpdateChartData(dailyRevenue);
            }
            catch (Exception ex)
            {
                // Log error for debugging
                System.Diagnostics.Debug.WriteLine($"LoadChartData error: {ex.Message}");
            }
        }

        private async Task LoadDetailData()
        {
            try
            {
                // Load detailed daily revenue - Using DateHelper
                var (startDate, endDate) = DateHelper.GetLast30Days();
                var dailyDetails = await _context.Hoadonbans
                    .Where(h => h.NgayLap.HasValue && h.NgayLap.Value >= startDate)
                    .GroupBy(h => h.NgayLap.Value)
                    .Select(g => new 
                    { 
                        Date = g.Key, 
                        InvoiceCount = g.Count(),
                        TotalRevenue = g.Sum(h => h.TongTien ?? 0),
                        AverageRevenue = g.Average(h => h.TongTien ?? 0)
                    })
                    .OrderByDescending(x => x.Date)
                    .ToListAsync();

                // Update detail grid
                UpdateDetailGrid(dailyDetails);
            }
            catch (Exception ex)
            {
                // Log error for debugging
                System.Diagnostics.Debug.WriteLine($"LoadDetailData error: {ex.Message}");
            }
        }

        private void UpdateSummaryCard(int index, string value)
        {
            try
            {
                // Tìm FlowLayoutPanel trong summaryPanel
                var flowPanel = summaryPanel.Controls.OfType<FlowLayoutPanel>().FirstOrDefault();
                if (flowPanel != null && flowPanel.Controls.Count > index)
                {
                    var card = flowPanel.Controls[index] as Panel;
                    if (card?.Controls.Count > 1)
                    {
                        var valueLabel = card.Controls.OfType<Label>().Skip(1).FirstOrDefault();
                        if (valueLabel != null)
                        {
                            valueLabel.Text = value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateSummaryCard error: {ex.Message}");
            }
        }

        private void UpdateChartData(dynamic dailyRevenue)
        {
            // Update chart with real data
            // This would involve updating the chart controls with actual data
        }

        private void UpdateDetailGrid(dynamic dailyDetails)
        {
            // Update detail grid with real data
            // This would involve updating the DataGridView with actual data
        }
    }
}