using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AceCook
{
    public partial class RevenueReportForm : Form
    {
        public RevenueReportForm()
        {
            InitializeComponent();
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            try
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
                        soHoaDon.ToString(),
                        tongTien.ToString("N0") + " ₫",
                        trungBinh.ToString("N0") + " ₫"
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            
            LoadSampleData();
        }
    }
}
