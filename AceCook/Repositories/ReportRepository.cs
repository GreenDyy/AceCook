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
                Details = new List<RevenueReportDetail>()
            };

            var orders = _context.Hoadonban
                .Where(h => h.Ngaylap >= fromDate && h.Ngaylap <= toDate)
                .ToList();

            foreach (var order in orders)
            {
                var customer = _context.Khachhang.FirstOrDefault(k => k.Makh == order.Makh);
                report.Details.Add(new RevenueReportDetail
                {
                    Date = order.Ngaylap,
                    OrderId = order.Mahd,
                    CustomerName = customer?.Tenkh ?? "N/A",
                    Amount = order.Tongtien ?? 0,
                    PaymentMethod = order.Phuongthuctt,
                    Status = order.Trangthai
                });
            }

            report.TotalRevenue = report.Details.Sum(d => d.Amount);
            return report;
        }

        public InventoryReport GetInventoryReport(DateTime fromDate, DateTime toDate)
        {
            var report = new InventoryReport
            {
                FromDate = fromDate,
                ToDate = toDate,
                CreatedDate = DateTime.Now,
                Details = new List<InventoryReportDetail>()
            };

            var products = _context.Sanpham.ToList();
            foreach (var product in products)
            {
                var inStock = _context.ChitietPn
                    .Where(c => c.Phieunhapkho.Ngaynhap >= fromDate && c.Phieunhapkho.Ngaynhap <= toDate)
                    .Where(c => c.Masp == product.Masp)
                    .Sum(c => c.Soluong) ?? 0;

                var outStock = _context.Phieuxuatkho
                    .Where(p => p.Ngayxuat >= fromDate && p.Ngayxuat <= toDate)
                    .Where(p => p.Masp == product.Masp)
                    .Sum(p => p.Soluong) ?? 0;

                report.Details.Add(new InventoryReportDetail
                {
                    ProductId = product.Masp,
                    ProductName = product.Tensp,
                    BeginningQuantity = product.Soluongton ?? 0,
                    InQuantity = inStock,
                    OutQuantity = outStock,
                    EndingQuantity = (product.Soluongton ?? 0) + inStock - outStock,
                    UnitPrice = product.Dongia ?? 0,
                    TotalValue = ((product.Soluongton ?? 0) + inStock - outStock) * (product.Dongia ?? 0)
                });
            }

            report.TotalValue = report.Details.Sum(d => d.TotalValue);
            return report;
        }

        public OrderReport GetOrderReport(DateTime fromDate, DateTime toDate)
        {
            var report = new OrderReport
            {
                FromDate = fromDate,
                ToDate = toDate,
                CreatedDate = DateTime.Now,
                Details = new List<OrderReportDetail>()
            };

            var orders = _context.Dondathang
                .Where(d => d.Ngaydat >= fromDate && d.Ngaydat <= toDate)
                .ToList();

            foreach (var order in orders)
            {
                var customer = _context.Khachhang.FirstOrDefault(k => k.Makh == order.Makh);
                var orderDetails = _context.CtDh.Where(c => c.Madh == order.Madh).ToList();

                report.Details.Add(new OrderReportDetail
                {
                    OrderId = order.Madh,
                    OrderDate = order.Ngaydat,
                    CustomerName = customer?.Tenkh ?? "N/A",
                    Status = order.Trangthai,
                    TotalItems = orderDetails.Sum(d => d.Soluong ?? 0),
                    TotalAmount = orderDetails.Sum(d => (d.Soluong ?? 0) * (d.Dongia ?? 0))
                });
            }

            report.TotalOrders = report.Details.Count;
            report.TotalValue = report.Details.Sum(d => d.TotalAmount);
            return report;
        }
    }
}
