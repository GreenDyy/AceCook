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

namespace AceCook
{
    public partial class OrderReportForm : Form
    {
        private readonly OrderRepository _orderRepository;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private Button btnApplyFilter;
        private Button btnReset;
        private Label lblTitle;
        private Label lblDateRange;
        private Panel pnlFilters;
        private Panel pnlSummary;
        private DataGridView dgvOrderStatus;
        private DataGridView dgvTopCustomers;
        private Label lblTotalOrders;
        private Label lblCompletedOrders;
        private Label lblNewOrders;
        private Label lblOrdersByStatus;
        private Label lblTopCustomers;

        public OrderReportForm(OrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            InitializeComponent();
            SetupUI();
            InitializeGrids();
            _ = LoadDataAsync();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Text = "Báo cáo Đơn hàng";
            this.Size = new Size(1400, 900);
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

            // Date range controls
            lblDateRange = new Label
            {
                Text = "Khoảng thời gian:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(120, 25),
                Location = new Point(20, 15),
                TextAlign = ContentAlignment.MiddleLeft
            };

            dtpStartDate = new DateTimePicker
            {
                Size = new Size(150, 30),
                Location = new Point(150, 12),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddDays(-30)
            };

            Label lblTo = new Label
            {
                Text = "đến",
                Font = new Font("Segoe UI", 10),
                Size = new Size(40, 25),
                Location = new Point(310, 15),
                TextAlign = ContentAlignment.MiddleCenter
            };

            dtpEndDate = new DateTimePicker
            {
                Size = new Size(150, 30),
                Location = new Point(360, 12),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };

            btnApplyFilter = new Button
            {
                Text = "🔍 Áp dụng",
                Size = new Size(100, 35),
                Location = new Point(530, 12),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnApplyFilter.FlatAppearance.BorderSize = 0;
            btnApplyFilter.Click += BtnApplyFilter_Click;

            btnReset = new Button
            {
                Text = "🔄 Làm mới",
                Size = new Size(100, 35),
                Location = new Point(640, 12),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(95, 95, 95),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.Click += BtnReset_Click;

            pnlFilters.Controls.AddRange(new Control[] {
                lblDateRange, dtpStartDate, lblTo, dtpEndDate,
                btnApplyFilter, btnReset
            });

            // Summary Panel
            pnlSummary = new Panel
            {
                Size = new Size(1340, 100),
                Location = new Point(30, 190),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Summary Labels
            lblTotalOrders = CreateSummaryLabel("Tổng số đơn hàng", "0", 20);
            lblCompletedOrders = CreateSummaryLabel("Đơn hàng hoàn thành", "0", 240);
            lblNewOrders = CreateSummaryLabel("Đơn hàng mới", "0", 460);

            pnlSummary.Controls.AddRange(new Control[] {
                lblTotalOrders, lblCompletedOrders, lblNewOrders
            });

            // Grid Labels
            lblOrdersByStatus = new Label
            {
                Text = "THỐNG KÊ THEO TRẠNG THÁI",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(400, 30),
                Location = new Point(30, 310),
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblTopCustomers = new Label
            {
                Text = "TOP 10 KHÁCH HÀNG",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(400, 30),
                Location = new Point(720, 310),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Setup DataGridViews
            dgvOrderStatus = CreateDataGridView(new Point(30, 350), new Size(650, 480));
            dgvTopCustomers = CreateDataGridView(new Point(720, 350), new Size(650, 480));

            // Add all controls to form
            this.Controls.AddRange(new Control[] {
                lblTitle, pnlFilters, pnlSummary,
                lblOrdersByStatus, lblTopCustomers,
                dgvOrderStatus, dgvTopCustomers
            });
        }

        private Label CreateSummaryLabel(string title, string value, int x)
        {
            var container = new Label
            {
                Size = new Size(200, 70),
                Location = new Point(x, 15),
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10),
                Size = new Size(200, 25),
                Location = new Point(0, 0),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Size = new Size(200, 35),
                Location = new Point(0, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            container.Controls.AddRange(new Control[] { titleLabel, valueLabel });
            return container;
        }

        private DataGridView CreateDataGridView(Point location, Size size)
        {
            var dgv = new DataGridView
            {
                Location = location,
                Size = size,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                GridColor = Color.LightGray,
                RowHeadersVisible = false,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersHeight = 50,
                RowTemplate = { Height = 40 }
            };

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;

            return dgv;
        }

        private void InitializeGrids()
        {
            // Thống kê trạng thái
            dgvOrderStatus.Columns.Add("Status", "Trạng thái");
            dgvOrderStatus.Columns.Add("Count", "Số lượng");
            dgvOrderStatus.Columns.Add("TotalValue", "Tổng giá trị");

            // Top khách hàng
            dgvTopCustomers.Columns.Add("Customer", "Khách hàng");
            dgvTopCustomers.Columns.Add("OrderCount", "Số đơn hàng");
            dgvTopCustomers.Columns.Add("TotalValue", "Tổng giá trị");
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var startDate = dtpStartDate.Value.Date;
                var endDate = dtpEndDate.Value.Date.AddDays(1).AddTicks(-1);

                var orders = await _orderRepository.GetOrdersByDateRangeAsync(startDate, endDate);

                // Thống kê theo trạng thái
                var statusStats = orders
                    .GroupBy(dh => dh.TrangThai ?? "Không xác định")
                    .Select(g => new
                    {
                        Status = g.Key,
                        Count = g.Count(),
                        TotalValue = g.SelectMany(dh => dh.CtDhs)
                                   .Sum(ct => (decimal)((ct.SoLuong ?? 0) * (ct.DonGia ?? 0)))
                    })
                    .OrderByDescending(x => x.Count)
                    .ToList();

                dgvOrderStatus.Rows.Clear();
                foreach (var stat in statusStats)
                {
                    var rowIndex = dgvOrderStatus.Rows.Add(
                        stat.Status,
                        stat.Count,
                        string.Format("{0:N0}đ", stat.TotalValue)
                    );
                    StyleStatusRow(dgvOrderStatus.Rows[rowIndex], stat.Status);
                }

                // Top khách hàng
                var customerStats = orders
                    .Where(dh => dh.MaKhNavigation != null && !string.IsNullOrEmpty(dh.MaKhNavigation.TenKh))
                    .GroupBy(dh => new { dh.MaKh, dh.MaKhNavigation.TenKh })
                    .Select(g => new
                    {
                        Customer = g.Key.TenKh,
                        OrderCount = g.Count(),
                        TotalValue = g.SelectMany(dh => dh.CtDhs)
                                   .Sum(ct => (decimal)((ct.SoLuong ?? 0) * (ct.DonGia ?? 0)))
                    })
                    .OrderByDescending(x => x.OrderCount)
                    .Take(10)
                    .ToList();

                dgvTopCustomers.Rows.Clear();
                foreach (var stat in customerStats)
                {
                    dgvTopCustomers.Rows.Add(
                        stat.Customer,
                        stat.OrderCount,
                        string.Format("{0:N0}đ", stat.TotalValue)
                    );
                }

                // Cập nhật thống kê tổng quan
                UpdateSummaryStats(orders);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateSummaryStats(List<Dondathang> orders)
        {
            var totalOrders = orders.Count;
            var completedOrders = orders.Count(dh => dh.TrangThai == "Hoàn thành" || dh.TrangThai == "Đã giao");
            var newOrders = orders.Count(dh => dh.TrangThai == "Mới" || dh.TrangThai == "Chờ xử lý");

            ((Label)lblTotalOrders.Controls[1]).Text = totalOrders.ToString("N0");
            ((Label)lblCompletedOrders.Controls[1]).Text = completedOrders.ToString("N0");
            ((Label)lblNewOrders.Controls[1]).Text = newOrders.ToString("N0");
        }

        private void StyleStatusRow(DataGridViewRow row, string status)
        {
            Color statusColor = status.ToLower() switch
            {
                "hoàn thành" or "đã giao" => Color.Green,
                "đang xử lý" => Color.Orange,
                "đã hủy" => Color.Red,
                _ => Color.Blue
            };
            row.Cells["Status"].Style.ForeColor = statusColor;
        }

        private async void BtnApplyFilter_Click(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        private async void BtnReset_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Now.AddDays(-30);
            dtpEndDate.Value = DateTime.Now;
            await LoadDataAsync();
        }
    }
}
