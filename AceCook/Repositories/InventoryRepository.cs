using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AceCook.Models;
using AceCook.Helpers;

namespace AceCook.Repositories
{
    public class InventoryRepository
    {
        private readonly AppDbContext _context;

        public InventoryRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<CtTon>> GetAllInventoryAsync()
        {
            return await DatabaseHelper.ExecuteWithRetryAsync(async () =>
            {
                // Kiểm tra kết nối database
                if (!await DatabaseHelper.TestConnectionAsync(_context))
                {
                    throw new InvalidOperationException("Không thể kết nối đến cơ sở dữ liệu");
                }

                var result = await _context.CtTons
                    .Include(c => c.MaSpNavigation)
                    .Include(c => c.MaKhoNavigation)
                    .AsNoTracking() // Tối ưu performance
                    .ToListAsync();

                return result ?? new List<CtTon>();
            }, "GetAllInventoryAsync");
        }

        public async Task<CtTon?> GetInventoryByIdAsync(string maSP, string maKho = null)
        {
            return await DatabaseHelper.ExecuteWithRetryAsync(async () =>
            {
                if (string.IsNullOrEmpty(maSP))
                    throw new ArgumentException("Mã sản phẩm không được để trống");

                var query = _context.CtTons
                    .Include(c => c.MaSpNavigation)
                    .Include(c => c.MaKhoNavigation)
                    .AsNoTracking()
                    .Where(c => c.MaSp == maSP);

                if (!string.IsNullOrEmpty(maKho))
                {
                    query = query.Where(c => c.MaKho == maKho);
                }

                return await query.FirstOrDefaultAsync();
            }, "GetInventoryByIdAsync");
        }

        public async Task<List<CtTon>> SearchInventoryAsync(string searchTerm)
        {
            return await DatabaseHelper.ExecuteWithRetryAsync(async () =>
            {
                if (string.IsNullOrEmpty(searchTerm))
                    return await GetAllInventoryAsync();

                var result = await _context.CtTons
                    .Include(c => c.MaSpNavigation)
                    .Include(c => c.MaKhoNavigation)
                    .AsNoTracking()
                    .Where(c => c.MaSpNavigation.TenSp.Contains(searchTerm) || c.MaSp.Contains(searchTerm))
                    .ToListAsync();

                return result ?? new List<CtTon>();
            }, "SearchInventoryAsync");
        }

        public async Task<bool> AddInventoryAsync(CtTon inventory)
        {
            return await DatabaseHelper.ExecuteWithRetryAsync(async () =>
            {
                if (inventory == null)
                    throw new ArgumentNullException(nameof(inventory));

                await _context.CtTons.AddAsync(inventory);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }, "AddInventoryAsync");
        }

        public async Task<bool> UpdateInventoryAsync(CtTon inventory)
        {
            return await DatabaseHelper.ExecuteWithRetryAsync(async () =>
            {
                if (inventory == null)
                    throw new ArgumentNullException(nameof(inventory));

                // Tìm entity trong context để update
                var existingInventory = await _context.CtTons
                    .FirstOrDefaultAsync(c => c.MaSp == inventory.MaSp && c.MaKho == inventory.MaKho);
                
                if (existingInventory == null)
                {
                    throw new InvalidOperationException($"Không tìm thấy tồn kho với mã sản phẩm {inventory.MaSp} và mã kho {inventory.MaKho}");
                }

                // Cập nhật các thuộc tính cần thiết
                existingInventory.SoLuongTonKho = inventory.SoLuongTonKho;
                
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }, "UpdateInventoryAsync");
        }

        public async Task<bool> DeleteInventoryAsync(string maSP, string maKho)
        {
            return await DatabaseHelper.ExecuteWithRetryAsync(async () =>
            {
                if (string.IsNullOrEmpty(maSP) || string.IsNullOrEmpty(maKho))
                    throw new ArgumentException("Mã sản phẩm và mã kho không được để trống");

                var inventory = await _context.CtTons
                    .FirstOrDefaultAsync(c => c.MaSp == maSP && c.MaKho == maKho);
                
                if (inventory != null)
                {
                    _context.CtTons.Remove(inventory);
                    var result = await _context.SaveChangesAsync();
                    return result > 0;
                }
                return false;
            }, "DeleteInventoryAsync");
        }

        public async Task<decimal> GetTotalInventoryValueAsync()
        {
            return await DatabaseHelper.ExecuteWithRetryAsync(async () =>
            {
                var result = await _context.CtTons
                    .Include(c => c.MaSpNavigation)
                    .AsNoTracking()
                    .SumAsync(c => (c.SoLuongTonKho ?? 0) * (c.MaSpNavigation.Gia ?? 0));

                return result;
            }, "GetTotalInventoryValueAsync");
        }

        public async Task<int> GetTotalItemsAsync()
        {
            return await DatabaseHelper.ExecuteWithRetryAsync(async () =>
            {
                var result = await _context.CtTons
                    .AsNoTracking()
                    .SumAsync(c => c.SoLuongTonKho ?? 0);

                return result;
            }, "GetTotalItemsAsync");
        }

        public async Task<List<CtTon>> GetLowStockItemsAsync(int threshold = 10)
        {
            return await DatabaseHelper.ExecuteWithRetryAsync(async () =>
            {
                var result = await _context.CtTons
                    .Include(c => c.MaSpNavigation)
                    .Include(c => c.MaKhoNavigation)
                    .AsNoTracking()
                    .Where(c => c.SoLuongTonKho <= threshold)
                    .ToListAsync();

                return result ?? new List<CtTon>();
            }, "GetLowStockItemsAsync");
        }

        public async Task<List<Khohang>> GetAllWarehousesAsync()
        {
            return await DatabaseHelper.ExecuteWithRetryAsync(async () =>
            {
                var result = await _context.Khohangs
                    .AsNoTracking()
                    .OrderBy(k => k.TenKho)
                    .ToListAsync();

                return result ?? new List<Khohang>();
            }, "GetAllWarehousesAsync");
        }

        public async Task<List<string>> GetAllProductTypesAsync()
        {
            return await DatabaseHelper.ExecuteWithRetryAsync(async () =>
            {
                var result = await _context.Sanphams
                    .AsNoTracking()
                    .Where(s => !string.IsNullOrEmpty(s.Loai))
                    .Select(s => s.Loai)
                    .Distinct()
                    .OrderBy(t => t)
                    .ToListAsync();

                return result ?? new List<string>();
            }, "GetAllProductTypesAsync");
        }

        public async Task<List<CtTon>> GetInventoryByWarehouseAsync(string warehouseName)
        {
            return await DatabaseHelper.ExecuteWithRetryAsync(async () =>
            {
                if (string.IsNullOrEmpty(warehouseName))
                    throw new ArgumentException("Tên kho không được để trống");

                var result = await _context.CtTons
                    .Include(c => c.MaSpNavigation)
                    .Include(c => c.MaKhoNavigation)
                    .AsNoTracking()
                    .Where(c => c.MaKhoNavigation.TenKho == warehouseName)
                    .ToListAsync();

                return result ?? new List<CtTon>();
            }, "GetInventoryByWarehouseAsync");
        }

        public async Task<List<CtTon>> GetInventoryByProductTypeAsync(string productType)
        {
            return await DatabaseHelper.ExecuteWithRetryAsync(async () =>
            {
                if (string.IsNullOrEmpty(productType))
                    throw new ArgumentException("Loại sản phẩm không được để trống");

                var result = await _context.CtTons
                    .Include(c => c.MaSpNavigation)
                    .Include(c => c.MaKhoNavigation)
                    .AsNoTracking()
                    .Where(c => c.MaSpNavigation.Loai == productType)
                    .ToListAsync();

                return result ?? new List<CtTon>();
            }, "GetInventoryByProductTypeAsync");
        }

        public async Task<List<CtTon>> GetFilteredInventoryAsync(string? searchTerm = null, string? warehouseName = null, string? productType = null)
        {
            return await DatabaseHelper.ExecuteWithRetryAsync(async () =>
            {
                var query = _context.CtTons
                    .Include(c => c.MaSpNavigation)
                    .Include(c => c.MaKhoNavigation)
                    .AsNoTracking()
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

                var result = await query.ToListAsync();
                return result ?? new List<CtTon>();
            }, "GetFilteredInventoryAsync");
        }

        public async Task<(int totalItems, decimal totalValue, int lowStockCount)> GetInventorySummaryAsync()
        {
            return await DatabaseHelper.ExecuteWithRetryAsync(async () =>
            {
                var totalItems = await _context.CtTons
                    .AsNoTracking()
                    .SumAsync(c => c.SoLuongTonKho ?? 0);

                var totalValue = await _context.CtTons
                    .Include(c => c.MaSpNavigation)
                    .AsNoTracking()
                    .SumAsync(c => (c.SoLuongTonKho ?? 0) * (c.MaSpNavigation.Gia ?? 0));

                var lowStockCount = await _context.CtTons
                    .AsNoTracking()
                    .CountAsync(c => (c.SoLuongTonKho ?? 0) <= 10);

                return (totalItems, totalValue, lowStockCount);
            }, "GetInventorySummaryAsync");
        }

        public async Task<List<object>> GetInventoryReportDataAsync(List<CtTon> inventory)
        {
            try
            {
                if (inventory == null)
                    return new List<object>();

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
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi tạo dữ liệu báo cáo: {ex.Message}", ex);
            }
        }

        private string GetStockStatus(int stockLevel)
        {
            if (stockLevel == 0) return "Hết hàng";
            if (stockLevel <= 10) return "Sắp hết";
            if (stockLevel <= 50) return "Trung bình";
            return "Còn hàng";
        }
    }
} 