using System;
using System.Collections.Generic;
using System.Linq;
using AceCook.Models;
using AceCook.Models.Reports;

namespace AceCook.Repositories
{
    public class ReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        public RevenueReport GetRevenueReport(DateTime fromDate, DateTime toDate)
        {
            var report = new RevenueReport
            {
                FromDate = fromDate,
                ToDate = toDate,
                CreatedDate = DateTime.Now,
                CreatedBy = "System", // Add missing CreatedBy property
                Details = new List<RevenueReportDetail>()
            };

            // Get all orders first, then filter in memory to avoid EF translation issues
            var startDateOnly = DateOnly.FromDateTime(fromDate);
            var endDateOnly = DateOnly.FromDateTime(toDate);
            
            var orders = _context.Hoadonbans
                .Where(h => h.NgayLap.HasValue)
                .ToList()
                .Where(h => h.NgayLap.Value >= startDateOnly && h.NgayLap.Value <= endDateOnly)
                .ToList();

            foreach (var order in orders)
            {
                // Note: Hoadonban doesn't have Makh or Phuongthuctt properties
                // Using available properties instead
                report.Details.Add(new RevenueReportDetail
                {
                    Date = order.NgayLap?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue,
                    OrderId = order.MaHdb,
                    CustomerName = "N/A", // Customer info not directly available in Hoadonban
                    Amount = order.TongTien ?? 0,
                    PaymentMethod = "N/A", // Payment method not available in Hoadonban
                    Status = order.TrangThaiThanhToan ?? "N/A"
                });
            }

            report.TotalRevenue = report.Details.Sum(d => d.Amount);
            return report;
        }

        public class InventoryReportItem
        {
            public string MaSP { get; set; }
            public string TenSanPham { get; set; }
            public string Loai { get; set; }
            public decimal Gia { get; set; }
            public int TonKho { get; set; }
            public decimal GiaTri { get; set; }
            public string ChiTietTheoKho { get; set; }
        }

        public List<InventoryReportItem> GetInventoryReport(DateTime fromDate, DateTime toDate)
        {
            var result = new List<InventoryReportItem>();
            
            var products = _context.Sanphams.ToList();
            foreach (var product in products)
            {
                // Lấy thông tin tồn kho theo từng kho
                var stockByWarehouse = _context.CtTons
                    .Where(c => c.MaSp == product.MaSp)
                    .Join(_context.Khohangs,
                        ct => ct.MaKho,
                        kh => kh.MaKho,
                        (ct, kh) => new { 
                            Warehouse = kh.TenKho, 
                            Quantity = ct.SoLuongTonKho ?? 0
                        })
                    .ToList();

                // Tính tổng tồn kho
                var totalStock = stockByWarehouse.Sum(s => s.Quantity);

                // Tạo chuỗi chi tiết theo kho
                var warehouseDetails = string.Join(", ", 
                    stockByWarehouse.Select(s => $"{s.Warehouse}: {s.Quantity}"));

                result.Add(new InventoryReportItem
                {
                    MaSP = product.MaSp,
                    TenSanPham = product.TenSp ?? "N/A",
                    Loai = product.Loai ?? "N/A",
                    Gia = product.Gia ?? 0,
                    TonKho = (int)totalStock,
                    GiaTri = totalStock * (product.Gia ?? 0),
                    ChiTietTheoKho = warehouseDetails
                });
            }

            return result;
        }

        public OrderReport GetOrderReport(DateTime fromDate, DateTime toDate)
        {
            var report = new OrderReport
            {
                FromDate = fromDate,
                ToDate = toDate,
                CreatedDate = DateTime.Now,
                CreatedBy = "System", // Add missing CreatedBy property
                Details = new List<OrderReportDetail>()
            };

            // Get all orders first, then filter in memory to avoid EF translation issues
            var startDateOnly = DateOnly.FromDateTime(fromDate);
            var endDateOnly = DateOnly.FromDateTime(toDate);
            
            var orders = _context.Dondathangs
                .ToList()
                .Where(d => d.NgayDat.HasValue && d.NgayDat.Value >= startDateOnly && d.NgayDat.Value <= endDateOnly)
                .ToList();

            foreach (var order in orders)
            {
                var customer = _context.Khachhangs.FirstOrDefault(k => k.MaKh == order.MaKh);
                var orderDetails = _context.CtDhs.Where(c => c.MaDdh == order.MaDdh).ToList();

                report.Details.Add(new OrderReportDetail
                {
                    OrderId = order.MaDdh,
                    OrderDate = order.NgayDat?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue,
                    CustomerName = customer?.TenKh ?? "N/A",
                    Status = order.TrangThai ?? "N/A",
                    TotalItems = orderDetails.Sum(d => d.SoLuong ?? 0),
                    TotalAmount = orderDetails.Sum(d => (d.SoLuong ?? 0) * (decimal)(d.DonGia ?? 0.0))
                });
            }

            report.TotalOrders = report.Details.Count;
            report.TotalValue = report.Details.Sum(d => d.TotalAmount);
            return report;
        }
    }
}
