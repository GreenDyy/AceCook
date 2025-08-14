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

            var orders = _context.Hoadonbans
                .Where(h => h.NgayLap.HasValue && h.NgayLap.Value >= DateOnly.FromDateTime(fromDate) && h.NgayLap.Value <= DateOnly.FromDateTime(toDate))
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

        //public InventoryReport GetInventoryReport(DateTime fromDate, DateTime toDate)
        //{
        //    var report = new InventoryReport
        //    {
        //        FromDate = fromDate,
        //        ToDate = toDate,
        //        CreatedDate = DateTime.Now,
        //        CreatedBy = "System", // Add missing CreatedBy property
        //        Details = new List<InventoryReportDetail>()
        //    };

        //    var products = _context.Sanphams.ToList();
        //    foreach (var product in products)
        //    {
        //        // Get current stock from CtTon
        //        var currentStock = _context.CtTons
        //            .Where(c => c.MaSp == product.MaSp)
        //            .Sum(c => c.SoLuongTonKho) ?? 0;

        //        // Get incoming stock from ChitietPn (raw materials, not products)
        //        var inStock = _context.ChitietPns
        //            .Where(c => c.MaPnkNavigation.NgayNhap >= DateOnly.FromDateTime(fromDate) && c.MaPnkNavigation.NgayNhap <= DateOnly.FromDateTime(toDate))
        //            .Where(c => c.MaNl == product.MaSp) // This might not be correct - products vs raw materials
        //            .Sum(c => c.SoLuongNhap) ?? 0;

        //        // Phieuxuatkho doesn't have quantity information for products
        //        var outStock = 0;

        //        report.Details.Add(new InventoryReportDetail
        //        {
        //            ProductId = product.MaSp,
        //            ProductName = product.TenSp ?? "N/A",
        //            BeginningQuantity = (int)currentStock,
        //            InQuantity = (int)inStock,
        //            OutQuantity = (int)outStock,
        //            EndingQuantity = (int)currentStock + (int)inStock - (int)outStock,
        //            UnitPrice = product.Gia ?? 0,
        //            TotalValue = ((int)currentStock + (int)inStock - (int)outStock) * (product.Gia ?? 0)
        //        });
        //    }

        //    report.TotalValue = report.Details.Sum(d => d.TotalValue);
        //    return report;
        //}

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

            var orders = _context.Dondathangs
                .Where(d => d.NgayDat >= DateOnly.FromDateTime(fromDate) && d.NgayDat <= DateOnly.FromDateTime(toDate))
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
