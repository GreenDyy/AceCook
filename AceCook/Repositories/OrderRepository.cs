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
                .ToListAsync();
        }

        public async Task<Dondathang?> GetOrderByIdAsync(string maDDH)
        {
            return await _context.Dondathangs
                .Include(d => d.MaKhNavigation)
                .Include(d => d.MaNvNavigation)
                .Include(d => d.CtDhs)
                .FirstOrDefaultAsync(d => d.MaDdh == maDDH);
        }

        public async Task<List<Dondathang>> SearchOrdersAsync(string searchTerm)
        {
            return await _context.Dondathangs
                .Include(d => d.MaKhNavigation)
                .Include(d => d.MaNvNavigation)
                .Where(d => d.MaDdh.Contains(searchTerm) || d.MaKhNavigation.TenKh.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<List<Dondathang>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Dondathangs
                .Include(d => d.MaKhNavigation)
                .Include(d => d.MaNvNavigation)
                .Where(d => d.NgayDat.HasValue &&
                            d.NgayDat.Value.ToDateTime(TimeOnly.MinValue) >= startDate &&
                            d.NgayDat.Value.ToDateTime(TimeOnly.MinValue) <= endDate)
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
                var order = await _context.Dondathangs.FirstOrDefaultAsync(d => d.MaDdh == maDDH);
                if (order != null)
                {
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
    }
}