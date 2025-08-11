using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AceCook.Models;

namespace AceCook
{
    public partial class RevenueReportForm : Form
    {
        private AppDbContext _context;
        private DataGridView dataGridViewRevenue;
        private DateTimePicker dtpFrom;
        private DateTimePicker dtpTo;
        private Button btnGenerate;
        private Button btnExport;
        private Label lblTitle;
        private Label lblTotalRevenue;

        public RevenueReportForm(AppDbContext context)
        {
            _context = context;
            SetupUI();
            LoadRevenueData();
        }

        private void SetupUI()
        {
            this.Text = "Báo cáo Doanh thu";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);

            // Title
            lblTitle = new Label
            {
                Text = "BÁO CÁO DOANH THU",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(300, 40),
                Location = new Point(20, 20)
            };

            // Filter Panel
            var filterPanel = new Panel
            {
                Size = new Size(960, 80),
                Location = new Point(20, 70),
                BackColor = Color.White
            };

            var lblFrom = new Label
            {
                Text = "Từ ngày:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(80, 25),
                Location = new Point(20, 20)
            };

            dtpFrom = new DateTimePicker
            {
                Size = new Size(150, 25),
                Location = new Point(100, 18),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddDays(-30)
            };

            var lblTo = new Label
            {
                Text = "Đến ngày:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(80, 25),
                Location = new Point(270, 20)
            };

            dtpTo = new DateTimePicker
            {
                Size = new Size(150, 25),
                Location = new Point(350, 18),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today
            };

            btnGenerate = new Button
            {
                Text = "Tạo báo cáo",
                Size = new Size(100, 30),
                Location = new Point(520, 15),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnGenerate.FlatAppearance.BorderSize = 0;
            btnGenerate.Click += BtnGenerate_Click;

            btnExport = new Button
            {
                Text = "Xuất Excel",
                Size = new Size(100, 30),
                Location = new Point(640, 15),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnExport.FlatAppearance.BorderSize = 0;
            btnExport.Click += BtnExport_Click;

            filterPanel.Controls.Add(lblFrom);
            filterPanel.Controls.Add(dtpFrom);
            filterPanel.Controls.Add(lblTo);
            filterPanel.Controls.Add(dtpTo);
            filterPanel.Controls.Add(btnGenerate);
            filterPanel.Controls.Add(btnExport);

            // Total Revenue Label
            lblTotalRevenue = new Label
            {
                Text = "Tổng doanh thu: 0 ₫",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113),
                Size = new Size(300, 30),
                Location = new Point(20, 160)
            };

            // DataGridView
            dataGridViewRevenue = new DataGridView
            {
                Size = new Size(960, 350),
                Location = new Point(20, 200),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                GridColor = Color.LightGray,
                RowHeadersVisible = false
            };

            // Add controls to form
            this.Controls.Add(lblTitle);
            this.Controls.Add(filterPanel);
            this.Controls.Add(lblTotalRevenue);
            this.Controls.Add(dataGridViewRevenue);

            // Add shadow effect to filter panel
            filterPanel.Paint += (sender, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, filterPanel.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid);
            };
        }

        private async void LoadRevenueData()
        {
            await GenerateReport();
        }

        private async void BtnGenerate_Click(object sender, EventArgs e)
        {
            await GenerateReport();
        }

        private async Task GenerateReport()
        {
            try
            {
                var fromDate = DateOnly.FromDateTime(dtpFrom.Value.Date);
                var toDate = DateOnly.FromDateTime(dtpTo.Value.Date);

                var revenueData = await _context.Hoadonbans
                    .Where(h => h.NgayLap >= fromDate && h.NgayLap <= toDate)
                    .GroupBy(h => h.NgayLap.Value)
                    .Select(g => new
                    {
                        Ngay = g.Key,
                        SoHoaDon = g.Count(),
                        DoanhThu = g.Sum(h => h.TongTien ?? 0),
                        TrungBinh = g.Average(h => h.TongTien ?? 0)
                    })
                    .OrderByDescending(x => x.Ngay)
                    .ToListAsync();

                RefreshDataGridView(revenueData);

                var totalRevenue = revenueData.Sum(x => x.DoanhThu);
                lblTotalRevenue.Text = $"Tổng doanh thu: {totalRevenue:N0} ₫";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo báo cáo: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshDataGridView(dynamic revenueData)
        {
            dataGridViewRevenue.DataSource = null;
            dataGridViewRevenue.DataSource = revenueData;

            // Configure columns
            if (dataGridViewRevenue.Columns.Count > 0)
            {
                dataGridViewRevenue.Columns["Ngay"].HeaderText = "Ngày";
                dataGridViewRevenue.Columns["SoHoaDon"].HeaderText = "Số Hóa Đơn";
                dataGridViewRevenue.Columns["DoanhThu"].HeaderText = "Doanh Thu";
                dataGridViewRevenue.Columns["TrungBinh"].HeaderText = "Trung Bình/HD";

                // Format date and money columns
                if (dataGridViewRevenue.Columns["Ngay"] != null)
                {
                    dataGridViewRevenue.Columns["Ngay"].DefaultCellStyle.Format = "dd/MM/yyyy";
                }
                if (dataGridViewRevenue.Columns["DoanhThu"] != null)
                {
                    dataGridViewRevenue.Columns["DoanhThu"].DefaultCellStyle.Format = "N0";
                }
                if (dataGridViewRevenue.Columns["TrungBinh"] != null)
                {
                    dataGridViewRevenue.Columns["TrungBinh"].DefaultCellStyle.Format = "N0";
                }
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng xuất Excel sẽ được phát triển sau!", "Thông báo", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}