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
            // Khởi tạo biểu đồ doanh thu theo ngày
            dailyRevenueChart = new Chart();
            dailyRevenueChart.Size = new Size(600, 300);
            dailyRevenueChart.Location = new Point(15, 450);
            
            ChartArea dailyChartArea = new ChartArea();
            dailyRevenueChart.ChartAreas.Add(dailyChartArea);
            
            Series dailySeries = new Series();
            dailySeries.ChartType = SeriesChartType.Column;
            dailySeries.Name = "Doanh thu";
            dailyRevenueChart.Series.Add(dailySeries);
            
            // Thêm tiêu đề và định dạng trục
            dailyChartArea.AxisX.Title = "Ngày";
            dailyChartArea.AxisY.Title = "Doanh thu (VNĐ)";
            dailyChartArea.AxisY.LabelStyle.Format = "N0";
            
            panel3.Controls.Add(dailyRevenueChart);

            // Khởi tạo biểu đồ doanh thu theo tháng
            monthlyRevenueChart = new Chart();
            monthlyRevenueChart.Size = new Size(600, 300);
            monthlyRevenueChart.Location = new Point(15, 100);
            
            ChartArea monthlyChartArea = new ChartArea();
            monthlyRevenueChart.ChartAreas.Add(monthlyChartArea);
            
            Series monthlySeries = new Series();
            monthlySeries.ChartType = SeriesChartType.Column;
            monthlySeries.Name = "Doanh thu";
            monthlyRevenueChart.Series.Add(monthlySeries);
            
            // Thêm tiêu đề và định dạng trục
            monthlyChartArea.AxisX.Title = "Tháng";
            monthlyChartArea.AxisY.Title = "Doanh thu (VNĐ)";
            monthlyChartArea.AxisY.LabelStyle.Format = "N0";
            
            panel1.Controls.Add(monthlyRevenueChart);

            // Load dữ liệu mẫu
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            // Dữ liệu mẫu cho biểu đồ theo ngày
            dailyRevenueChart.Series["Doanh thu"].Points.Clear();
            for (int day = 1; day <= 31; day++)
            {
                dailyRevenueChart.Series["Doanh thu"].Points.AddXY(day, new Random().Next(100000, 1000000));
            }

            // Dữ liệu mẫu cho biểu đồ theo tháng
            monthlyRevenueChart.Series["Doanh thu"].Points.Clear();
            for (int month = 1; month <= 12; month++)
            {
                monthlyRevenueChart.Series["Doanh thu"].Points.AddXY(month, new Random().Next(1000000, 10000000));
            }
        }

        // Thêm phương thức để cập nhật dữ liệu khi nhấn nút Lọc
        private void button1_Click(object sender, EventArgs e)
        {
            DateTime startDate = dateTimePicker1.Value;
            DateTime endDate = dateTimePicker2.Value;
            
            // TODO: Lấy dữ liệu thực từ cơ sở dữ liệu dựa trên khoảng thời gian đã chọn
            // UpdateChartData(startDate, endDate);
            
            // Tạm thời load lại dữ liệu mẫu
            LoadSampleData();
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
    }
}
