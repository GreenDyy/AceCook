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
                MessageBox.Show("Đang load dữ liệu...");  // Debug message
                var startDate = dateTimePicker1.Value.Date;
                var endDate = dateTimePicker2.Value.Date.AddDays(1).AddTicks(-1); // Cuối ngày

                // Lấy tất cả đơn hàng trong khoảng thời gian
                var orders = await _orderRepository.GetOrdersByDateRangeAsync(startDate, endDate);
                MessageBox.Show($"Số đơn hàng lấy được: {orders.Count}");  // Debug message

                // Thống kê đơn hàng theo trạng thái (chỉ Mới/Hoàn thành như ASP.NET)
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

                MessageBox.Show($"Số trạng thái khác nhau: {statusStats.Count}, Danh sách trạng thái: {string.Join(", ", statusStats.Select(s => s.Status))}");  // Debug message

                // Cập nhật bảng thống kê trạng thái
                dgvOrderStatus.Rows.Clear();
                foreach (var stat in statusStats)
                {
                    try 
                    {
                        var rowIndex = dgvOrderStatus.Rows.Add(
                            stat.Status,
                            stat.Count,
                            string.Format("{0:N0}đ", stat.TotalValue)
                        );
                        MessageBox.Show($"Đã thêm dòng {rowIndex}: {stat.Status}, {stat.Count}, {string.Format("{0:N0}đ", stat.TotalValue)}");  // Debug message
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi thêm dòng: {ex.Message}");  // Debug message
                    }
                }

                // Top khách hàng theo logic ASP.NET
                var customerStats = orders
                    .Where(dh => dh.MaKhNavigation != null && !string.IsNullOrEmpty(dh.MaKhNavigation.TenKh))
                    .GroupBy(dh => new { dh.MaKh, dh.MaKhNavigation.TenKh })
                    .Select(g => new
                    {
                        MaKH = g.Key.MaKh,
                        Customer = g.Key.TenKh,
                        OrderCount = g.Count(),
                        TotalValue = g.SelectMany(dh => dh.CtDhs)
                                   .Sum(ct => (decimal)((ct.SoLuong ?? 0) * (ct.DonGia ?? 0)))
                    })
                    .OrderByDescending(x => x.OrderCount) // Sắp xếp theo số đơn hàng như ASP.NET
                    .Take(10)
                    .ToList();

                // Cập nhật bảng top khách hàng
                dgvTopCustomers.Rows.Clear();
                foreach (var stat in customerStats)
                {
                    dgvTopCustomers.Rows.Add(
                        stat.Customer,
                        stat.OrderCount,
                        string.Format("{0:N0}đ", stat.TotalValue)
                    );
                }

                // Hiển thị thông tin tổng quan (có thể thêm labels để hiển thị)
                var tongDonHang = orders.Count;
                var donHangHoanThanh = orders.Count(dh => dh.TrangThai == "Hoàn thành");
                var donHangMoi = orders.Count(dh => dh.TrangThai == "Mới");
                
                // Có thể thêm labels để hiển thị thống kê tổng quan này
                // lblTongDonHang.Text = tongDonHang.ToString();
                // lblDonHangHoanThanh.Text = donHangHoanThanh.ToString();
                // lblDonHangMoi.Text = donHangMoi.ToString();
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
