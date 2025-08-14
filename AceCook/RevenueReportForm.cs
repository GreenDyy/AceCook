using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace AceCook
{
    public partial class RevenueReportForm : Form
    {
        private Chart dailyRevenueChart;
        private Chart monthlyRevenueChart;

        public RevenueReportForm()
        {
            InitializeComponent();
            InitializeModernUI();
            LoadSampleData();
        }

        private void InitializeModernUI()
        {
            // Set form properties for modern look
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            // Initialize daily revenue chart
            InitializeDailyChart();
            
            // Initialize monthly revenue chart
            InitializeMonthlyChart();

            // Update summary cards with sample data
            UpdateSummaryCards();
        }

        private void InitializeDailyChart()
        {
            dailyRevenueChart = new Chart();
            dailyRevenueChart.Size = new Size(580, 300);
            dailyRevenueChart.Location = new Point(10, 10);
            dailyRevenueChart.BackColor = Color.White;

            ChartArea chartArea = new ChartArea();
            chartArea.Name = "DailyArea";
            chartArea.BackColor = Color.White;
            chartArea.AxisX.Title = "Ngày";
            chartArea.AxisY.Title = "Doanh thu (VNĐ)";
            chartArea.AxisX.TitleFont = new Font("Segoe UI", 9F, FontStyle.Bold);
            chartArea.AxisY.TitleFont = new Font("Segoe UI", 9F, FontStyle.Bold);
            chartArea.AxisY.LabelStyle.Format = "N0";
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisX.LineColor = Color.Gray;
            chartArea.AxisY.LineColor = Color.Gray;

            dailyRevenueChart.ChartAreas.Add(chartArea);

            Series series = new Series();
            series.Name = "Doanh thu";
            series.ChartType = SeriesChartType.Line;
            series.Color = Color.FromArgb(59, 130, 246);
            series.BorderWidth = 3;
            series.MarkerStyle = MarkerStyle.Circle;
            series.MarkerSize = 6;
            series.MarkerColor = Color.FromArgb(59, 130, 246);

            dailyRevenueChart.Series.Add(series);

            // Add title
            Title title = new Title("Biểu đồ doanh thu theo ngày");
            title.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(44, 62, 80);
            dailyRevenueChart.Titles.Add(title);

            panelDailyChart.Controls.Add(dailyRevenueChart);
        }

        private void InitializeMonthlyChart()
        {
            monthlyRevenueChart = new Chart();
            monthlyRevenueChart.Size = new Size(580, 300);
            monthlyRevenueChart.Location = new Point(10, 10);
            monthlyRevenueChart.BackColor = Color.White;

            ChartArea chartArea = new ChartArea();
            chartArea.Name = "MonthlyArea";
            chartArea.BackColor = Color.White;
            chartArea.AxisX.Title = "Tháng";
            chartArea.AxisY.Title = "Doanh thu (VNĐ)";
            chartArea.AxisX.TitleFont = new Font("Segoe UI", 9F, FontStyle.Bold);
            chartArea.AxisY.TitleFont = new Font("Segoe UI", 9F, FontStyle.Bold);
            chartArea.AxisY.LabelStyle.Format = "N0";
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisX.LineColor = Color.Gray;
            chartArea.AxisY.LineColor = Color.Gray;
            chartArea.AxisX.Minimum = 1;
            chartArea.AxisX.Maximum = 12;
            chartArea.AxisX.Interval = 1;

            monthlyRevenueChart.ChartAreas.Add(chartArea);

            Series series = new Series();
            series.Name = "Doanh thu";
            series.ChartType = SeriesChartType.Column;
            series.Color = Color.FromArgb(34, 197, 94);
            series.BorderWidth = 1;

            monthlyRevenueChart.Series.Add(series);

            // Add title
            Title title = new Title($"Doanh thu theo tháng năm {DateTime.Now.Year}");
            title.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(44, 62, 80);
            monthlyRevenueChart.Titles.Add(title);

            panelMonthlyChart.Controls.Add(monthlyRevenueChart);
        }

        private void LoadSampleData()
        {
            Random rnd = new Random();

            // Load daily data
            dailyRevenueChart.Series["Doanh thu"].Points.Clear();
            for (int day = 1; day <= 31; day++)
            {
                double value = rnd.Next(1000000, 10000000);
                dailyRevenueChart.Series["Doanh thu"].Points.AddXY(day, value);
            }

            // Load monthly data
            monthlyRevenueChart.Series["Doanh thu"].Points.Clear();
            for (int month = 1; month <= 12; month++)
            {
                double value = rnd.Next(20000000, 100000000);
                monthlyRevenueChart.Series["Doanh thu"].Points.AddXY(month, value);
            }

            // Load daily details table
            LoadDailyDetailsTable();
        }

        private void LoadDailyDetailsTable()
        {
            dataGridViewDetails.Rows.Clear();
            Random rnd = new Random();

            DateTime startDate = dateTimePickerFrom.Value.Date;
            DateTime endDate = dateTimePickerTo.Value.Date;

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                int soHoaDon = rnd.Next(1, 20);
                decimal tongTien = rnd.Next(1000000, 10000000);
                decimal trungBinh = tongTien / soHoaDon;

                dataGridViewDetails.Rows.Add(
                    date.ToString("dd/MM/yyyy"),
                    soHoaDon,
                    tongTien.ToString("N0") + " ₫",
                    trungBinh.ToString("N0") + " ₫"
                );
            }
        }

        private void UpdateSummaryCards()
        {
            Random rnd = new Random();
            
            // Calculate sample totals
            decimal tongDoanhThu = rnd.Next(50000000, 200000000);
            int tongSoHoaDon = rnd.Next(50, 200);
            decimal trungBinhHoaDon = tongDoanhThu / tongSoHoaDon;
            int ngayGiaoDich = rnd.Next(20, 31);

            lblTotalRevenue.Text = tongDoanhThu.ToString("N0") + " ₫";
            lblTotalInvoices.Text = tongSoHoaDon.ToString();
            lblAverageInvoice.Text = trungBinhHoaDon.ToString("N0") + " ₫";
            lblTransactionDays.Text = ngayGiaoDich.ToString();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            DateTime startDate = dateTimePickerFrom.Value;
            DateTime endDate = dateTimePickerTo.Value;

            if (startDate > endDate)
            {
                MessageBox.Show("Ngày bắt đầu không thể lớn hơn ngày kết thúc!", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Update period label
            lblPeriod.Text = $"Thống kê doanh thu từ {startDate:dd/MM/yyyy} đến {endDate:dd/MM/yyyy}";

            // Reload data
            LoadSampleData();
            UpdateSummaryCards();

            MessageBox.Show("Đã cập nhật báo cáo theo khoảng thời gian đã chọn!", "Thông báo", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng xuất Excel sẽ được triển khai sau!", "Thông báo", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng in báo cáo sẽ được triển khai sau!", "Thông báo", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RevenueReportForm_Load(object sender, EventArgs e)
        {
            // Set default date range (current month)
            dateTimePickerFrom.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dateTimePickerTo.Value = DateTime.Now;
            
            lblPeriod.Text = $"Thống kê doanh thu từ {dateTimePickerFrom.Value:dd/MM/yyyy} đến {dateTimePickerTo.Value:dd/MM/yyyy}";
        }
    }
}
