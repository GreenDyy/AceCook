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

        public OrderReportForm(OrderRepository orderRepository)
        {
            InitializeComponent();
            _orderRepository = orderRepository;
            
            InitializeGrids();
            _ = LoadDataAsync(); // Ignore the task since this is event-driven UI
        }

        private void InitializeGrids()
        {
            // Thêm cột cho bảng thống kê trạng thái
            dgvOrderStatus.Columns.Add("Status", "Trạng thái");
            dgvOrderStatus.Columns.Add("Count", "Số lượng");
            dgvOrderStatus.Columns.Add("TotalValue", "Tổng giá trị");

            // Thêm cột cho bảng top khách hàng
            dgvTopCustomers.Columns.Add("Customer", "Khách hàng");
            dgvTopCustomers.Columns.Add("OrderCount", "Số lượng đơn hàng");
            dgvTopCustomers.Columns.Add("TotalValue", "Tổng giá trị");
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var startDate = dateTimePicker1.Value;
                var endDate = dateTimePicker2.Value;

                // Lấy tất cả đơn hàng trong khoảng thời gian
                var orders = await _orderRepository.GetOrdersByDateRangeAsync(startDate, endDate);

                // Xử lý và hiển thị thống kê theo trạng thái
                var statusStats = orders
                    .GroupBy(o => o.TrangThai ?? "Không xác định")
                    .Select(g => new
                    {
                        Status = g.Key,
                        Count = g.Count(),
                        TotalValue = g.Sum(o => o.CtDhs.Sum(ct => (decimal)(ct.SoLuong * ct.DonGia ?? 0)))
                    })
                    .OrderByDescending(x => x.Count)
                    .ToList();

                dgvOrderStatus.Rows.Clear();
                foreach (var stat in statusStats)
                {
                    dgvOrderStatus.Rows.Add(
                        stat.Status,
                        stat.Count,
                        string.Format("{0:N0}đ", stat.TotalValue)
                    );
                }

                // Xử lý và hiển thị thống kê top khách hàng
                var customerStats = orders
                    .Where(o => o.MaKhNavigation != null)
                    .GroupBy(o => o.MaKhNavigation)
                    .Select(g => new
                    {
                        Customer = g.Key?.TenKh ?? "Không xác định",
                        OrderCount = g.Count(),
                        TotalValue = g.Sum(o => o.CtDhs.Sum(ct => (decimal)(ct.SoLuong * ct.DonGia ?? 0)))
                    })
                    .OrderByDescending(x => x.TotalValue)
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }
    }
}
