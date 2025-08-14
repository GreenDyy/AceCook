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

        public async Task<CtTon?> GetInventoryByIdAsync(string maSP, string maKho = null)
        {
            var query = _context.CtTons
                .Include(c => c.MaSpNavigation)
                .Include(c => c.MaKhoNavigation)
                .Where(c => c.MaSp == maSP);

            if (!string.IsNullOrEmpty(maKho))
            {
                query = query.Where(c => c.MaKho == maKho);
            }

            return await query.FirstOrDefaultAsync();
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

        // Thêm phương thức để lấy tồn kho với bộ lọc kết hợp
        public async Task<List<CtTon>> GetFilteredInventoryAsync(string? searchTerm = null, string? warehouseName = null, string? productType = null)
        {
            var query = _context.CtTons
                .Include(c => c.MaSpNavigation)
                .Include(c => c.MaKhoNavigation)
                .AsQueryable();

            // Apply warehouse filter
            if (!string.IsNullOrEmpty(warehouseName) && warehouseName != "Tất cả kho")
            {
                query = query.Where(c => c.MaKhoNavigation.TenKho == warehouseName);
            }

            // Apply product type filter
            if (!string.IsNullOrEmpty(productType) && productType != "Tất cả loại")
            {
                query = query.Where(c => c.MaSpNavigation.Loai == productType);
            }

            // Apply search filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchLower = searchTerm.ToLower();
                query = query.Where(c => 
                    c.MaSp.ToLower().Contains(searchLower) ||
                    (c.MaSpNavigation.TenSp != null && c.MaSpNavigation.TenSp.ToLower().Contains(searchLower)) ||
                    (c.MaKhoNavigation.TenKho != null && c.MaKhoNavigation.TenKho.ToLower().Contains(searchLower))
                );
            }

            return await query.ToListAsync();
        }

        // Thêm phương thức để lấy thống kê tổng quan
        public async Task<(int totalItems, decimal totalValue, int lowStockCount)> GetInventorySummaryAsync()
        {
            var totalItems = await _context.CtTons.SumAsync(c => c.SoLuongTonKho ?? 0);
            var totalValue = await _context.CtTons
                .Include(c => c.MaSpNavigation)
                .SumAsync(c => (c.SoLuongTonKho ?? 0) * (c.MaSpNavigation.Gia ?? 0));
            var lowStockCount = await _context.CtTons.CountAsync(c => (c.SoLuongTonKho ?? 0) <= 10);

            return (totalItems, totalValue, lowStockCount);
        }

        // Thêm phương thức để lấy dữ liệu cho báo cáo
        public async Task<List<object>> GetInventoryReportDataAsync(List<CtTon> inventory)
        {
            var reportData = new List<object>();
            
            foreach (var item in inventory)
            {
                var status = GetStockStatus(item.SoLuongTonKho ?? 0);
                var thanhTien = (item.SoLuongTonKho ?? 0) * (item.MaSpNavigation?.Gia ?? 0);
                
                reportData.Add(new
                {
                    MaSp = item.MaSp,
                    TenSp = item.MaSpNavigation?.TenSp ?? "",
                    Loai = item.MaSpNavigation?.Loai ?? "",
                    MaKho = item.MaKho,
                    TenKho = item.MaKhoNavigation?.TenKho ?? "",
                    ViTri = item.MaKhoNavigation?.ViTri ?? "",
                    SoLuongTon = item.SoLuongTonKho ?? 0,
                    DonGia = item.MaSpNavigation?.Gia ?? 0,
                    ThanhTien = thanhTien,
                    TrangThai = status
                });
            }
            
            return reportData;
        }

        private string GetStockStatus(int stockLevel)
        {
            if (stockLevel == 0) return "Hết hàng";
            if (stockLevel <= 10) return "Sắp hết";
            if (stockLevel <= 50) return "Trung bình";
            return "Đủ hàng";
        }
    }
} 