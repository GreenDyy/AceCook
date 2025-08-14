using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AceCook.Models;

namespace AceCook.Repositories
{
    public class OrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Dondathang>> GetAllOrdersAsync()
        {
            return await _context.Dondathangs
                .Include(d => d.MaKhNavigation)
                .Include(d => d.MaNvNavigation)
                .Include(d => d.CtDhs)
                    .ThenInclude(ct => ct.MaSpNavigation)
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();
        }

        public async Task<Dondathang?> GetOrderByIdAsync(string maDDH)
        {
            return await _context.Dondathangs
                .Include(d => d.MaKhNavigation)
                .Include(d => d.MaNvNavigation)
                .Include(d => d.CtDhs)
                    .ThenInclude(ct => ct.MaSpNavigation)
                .FirstOrDefaultAsync(d => d.MaDdh == maDDH);
        }

        public async Task<List<Dondathang>> SearchOrdersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllOrdersAsync();

            return await _context.Dondathangs
                .Include(d => d.MaKhNavigation)
                .Include(d => d.MaNvNavigation)
                .Include(d => d.CtDhs)
                .Where(d => d.MaDdh.Contains(searchTerm) || 
                           (d.MaKhNavigation != null && d.MaKhNavigation.TenKh != null && d.MaKhNavigation.TenKh.Contains(searchTerm)) ||
                           d.TrangThai.Contains(searchTerm))
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();
        }

        public async Task<List<Dondathang>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Dondathangs
                .Include(d => d.MaKhNavigation)
                .Include(d => d.MaNvNavigation)
                .Include(d => d.CtDhs)
                .Where(d => d.NgayDat.HasValue &&
                            d.NgayDat.Value.ToDateTime(TimeOnly.MinValue) >= startDate &&
                            d.NgayDat.Value.ToDateTime(TimeOnly.MinValue) <= endDate)
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();
        }

        public async Task<List<Dondathang>> GetOrdersByStatusAsync(string status)
        {
            return await _context.Dondathangs
                .Include(d => d.MaKhNavigation)
                .Include(d => d.MaNvNavigation)
                .Include(d => d.CtDhs)
                .Where(d => d.TrangThai == status)
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();
        }

        public async Task<bool> AddOrderAsync(Dondathang order)
        {
            try
            {
                await _context.Dondathangs.AddAsync(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateOrderAsync(Dondathang order)
        {
            try
            {
                _context.Dondathangs.Update(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteOrderAsync(string maDDH)
        {
            try
            {
                var order = await _context.Dondathangs
                    .Include(d => d.CtDhs)
                    .FirstOrDefaultAsync(d => d.MaDdh == maDDH);
                
                if (order != null)
                {
                    // Xóa chi tiết đơn hàng trước
                    _context.CtDhs.RemoveRange(order.CtDhs);
                    _context.Dondathangs.Remove(order);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(string maDDH, string newStatus)
        {
            try
            {
                var order = await _context.Dondathangs.FirstOrDefaultAsync(d => d.MaDdh == maDDH);
                if (order != null)
                {
                    order.TrangThai = newStatus;
                    if (newStatus == "Đã giao")
                    {
                        order.NgayGiao = DateOnly.FromDateTime(DateTime.Now);
                    }
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Dondathangs
                .Where(d => d.NgayDat.HasValue &&
                            d.NgayDat.Value.ToDateTime(TimeOnly.MinValue) >= startDate &&
                            d.NgayDat.Value.ToDateTime(TimeOnly.MinValue) <= endDate &&
                            d.TrangThai == "Đã giao")
                .SelectMany(d => d.CtDhs)
                .SumAsync(ct => (decimal)((ct.SoLuong ?? 0) * (ct.DonGia ?? 0)));
        }

        public async Task<int> GetTotalOrdersAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Dondathangs
                .Where(d => d.NgayDat.HasValue &&
                            d.NgayDat.Value.ToDateTime(TimeOnly.MinValue) >= startDate &&
                            d.NgayDat.Value.ToDateTime(TimeOnly.MinValue) <= endDate)
                .CountAsync();
        }

        public async Task<List<Khachhang>> GetAllCustomersAsync()
        {
            return await _context.Khachhangs
                .OrderBy(k => k.TenKh)
                .ToListAsync();
        }

        public async Task<List<Sanpham>> GetAllProductsAsync()
        {
            return await _context.Sanphams
                .OrderBy(s => s.TenSp)
                .ToListAsync();
        }

        public async Task<string> GenerateOrderIdAsync()
        {
            var lastOrder = await _context.Dondathangs
                .OrderByDescending(d => d.MaDdh)
                .FirstOrDefaultAsync();

            if (lastOrder == null)
                return "DDH001";

            var lastNumber = int.Parse(lastOrder.MaDdh.Substring(3));
            return $"DDH{(lastNumber + 1):D3}";
        }
    }
}