using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AceCook.Models;

namespace AceCook.Repositories
{
    public class InventoryRepository
    {
        private readonly AppDbContext _context;

        public InventoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CtTon>> GetAllInventoryAsync()
        {
            return await _context.CtTons
                .Include(c => c.MaSpNavigation)
                .Include(c => c.MaKhoNavigation)
                .ToListAsync();
        }

        public async Task<CtTon?> GetInventoryByIdAsync(string maSP)
        {
            return await _context.CtTons
                .Include(c => c.MaSpNavigation)
                .Include(c => c.MaKhoNavigation)
                .FirstOrDefaultAsync(c => c.MaSp == maSP);
        }

        public async Task<List<CtTon>> SearchInventoryAsync(string searchTerm)
        {
            return await _context.CtTons
                .Include(c => c.MaSpNavigation)
                .Include(c => c.MaKhoNavigation)
                .Where(c => c.MaSpNavigation.TenSp.Contains(searchTerm) || c.MaSp.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<bool> AddInventoryAsync(CtTon inventory)
        {
            try
            {
                await _context.CtTons.AddAsync(inventory);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateInventoryAsync(CtTon inventory)
        {
            try
            {
                _context.CtTons.Update(inventory);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteInventoryAsync(string maSP, string maKho)
        {
            try
            {
                var inventory = await _context.CtTons
                    .FirstOrDefaultAsync(c => c.MaSp == maSP && c.MaKho == maKho);
                
                if (inventory != null)
                {
                    _context.CtTons.Remove(inventory);
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

        public async Task<decimal> GetTotalInventoryValueAsync()
        {
            return await _context.CtTons
                .Include(c => c.MaSpNavigation)
                .SumAsync(c => (c.SoLuongTonKho ?? 0) * (c.MaSpNavigation.Gia ?? 0));
        }

        public async Task<int> GetTotalItemsAsync()
        {
            return await _context.CtTons.SumAsync(c => c.SoLuongTonKho ?? 0);
        }

        public async Task<List<CtTon>> GetLowStockItemsAsync(int threshold = 10)
        {
            return await _context.CtTons
                .Include(c => c.MaSpNavigation)
                .Include(c => c.MaKhoNavigation)
                .Where(c => c.SoLuongTonKho <= threshold)
                .ToListAsync();
        }

        // Thêm phương thức để lấy danh sách kho hàng
        public async Task<List<Khohang>> GetAllWarehousesAsync()
        {
            return await _context.Khohangs
                .OrderBy(k => k.TenKho)
                .ToListAsync();
        }

        // Thêm phương thức để lấy danh sách loại sản phẩm
        public async Task<List<string>> GetAllProductTypesAsync()
        {
            return await _context.Sanphams
                .Where(s => !string.IsNullOrEmpty(s.Loai))
                .Select(s => s.Loai)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();
        }

        // Thêm phương thức để lấy tồn kho theo kho hàng
        public async Task<List<CtTon>> GetInventoryByWarehouseAsync(string warehouseName)
        {
            return await _context.CtTons
                .Include(c => c.MaSpNavigation)
                .Include(c => c.MaKhoNavigation)
                .Where(c => c.MaKhoNavigation.TenKho == warehouseName)
                .ToListAsync();
        }

        // Thêm phương thức để lấy tồn kho theo loại sản phẩm
        public async Task<List<CtTon>> GetInventoryByProductTypeAsync(string productType)
        {
            return await _context.CtTons
                .Include(c => c.MaSpNavigation)
                .Include(c => c.MaKhoNavigation)
                .Where(c => c.MaSpNavigation.Loai == productType)
                .ToListAsync();
        }
    }
} 