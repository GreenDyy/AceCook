using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting; // Thêm namespace này

namespace AceCook
{
    public partial class RevenueReportForm : Form
    {
        private Chart dailyRevenueChart;
        private Chart monthlyRevenueChart;

        public RevenueReportForm()
        {
            InitializeComponent();
            InitializeCharts();
        }

        private void InitializeCharts()
        {
            // Biểu đồ doanh thu theo ngày
            dailyRevenueChart = new Chart();
            dailyRevenueChart.Dock = DockStyle.Fill;

            ChartArea dailyChartArea = new ChartArea();
            dailyRevenueChart.ChartAreas.Add(dailyChartArea);

            Series dailySeries = new Series();
            dailySeries.ChartType = SeriesChartType.Column;
            dailySeries.Name = "Doanh thu";
            dailySeries.Color = Color.FromArgb(107, 111, 213);
            dailyRevenueChart.Series.Add(dailySeries);

            // Cấu hình trục và tiêu đề cho biểu đồ ngày
            dailyChartArea.AxisX.Title = "Ngày";
            dailyChartArea.AxisY.Title = "Doanh thu (VNĐ)";
            dailyChartArea.AxisY.LabelStyle.Format = "N0";
            dailyChartArea.BackColor = Color.White;
            dailyChartArea.AxisX.Minimum = 1;
            dailyChartArea.AxisX.Maximum = 31;
            dailyChartArea.AxisX.Interval = 5;
            dailyChartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            dailyChartArea.AxisY.MajorGrid.LineColor = Color.LightGray;

            // Thêm biểu đồ vào panel3
            panel3.Controls.Clear();
            panel3.Controls.Add(dailyRevenueChart);

            // Biểu đồ doanh thu theo tháng
            monthlyRevenueChart = new Chart();
            monthlyRevenueChart.Dock = DockStyle.Fill;

            ChartArea monthlyChartArea = new ChartArea();
            monthlyRevenueChart.ChartAreas.Add(monthlyChartArea);

            Series monthlySeries = new Series();
            monthlySeries.ChartType = SeriesChartType.Column;
            monthlySeries.Name = "Doanh thu";
            monthlySeries.Color = Color.FromArgb(107, 111, 213);
            monthlyRevenueChart.Series.Add(monthlySeries);

            // Cấu hình trục và tiêu đề cho biểu đồ tháng
            monthlyChartArea.AxisX.Title = "Tháng";
            monthlyChartArea.AxisY.Title = "Doanh thu (VNĐ)";
            monthlyChartArea.AxisY.LabelStyle.Format = "N0";
            monthlyChartArea.BackColor = Color.White;
            monthlyChartArea.AxisX.Minimum = 1;
            monthlyChartArea.AxisX.Maximum = 12;
            monthlyChartArea.AxisX.Interval = 1;
            monthlyChartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            monthlyChartArea.AxisY.MajorGrid.LineColor = Color.LightGray;

            // Thêm biểu đồ vào panel4 (nằm trong groupBox7)
            panel4.Controls.Clear();
            panel4.Controls.Add(monthlyRevenueChart);

            // Load dữ liệu mẫu
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            // Dữ liệu mẫu cho biểu đồ theo ngày
            dailyRevenueChart.Series["Doanh thu"].Points.Clear();
            Random rnd = new Random();
            for (int day = 1; day <= 31; day++)
            {
                double value = rnd.Next(1000000, 10000000);
                dailyRevenueChart.Series["Doanh thu"].Points.AddXY(day, value);
            }

            // Dữ liệu mẫu cho biểu đồ theo tháng
            monthlyRevenueChart.Series["Doanh thu"].Points.Clear();
            for (int month = 1; month <= 12; month++)
            {
                double value = rnd.Next(2000000, 10000000);
                monthlyRevenueChart.Series["Doanh thu"].Points.AddXY(month, value);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime startDate = dateTimePicker1.Value;
            DateTime endDate = dateTimePicker2.Value;

            // Tạm thời load lại dữ liệu mẫu
            LoadSampleData();

            // Cập nhật label thời gian
            label14.Text = $"Thống kê doanh thu từ {startDate:d} đến {endDate:d}";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
