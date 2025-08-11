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
                .ToListAsync();
        }

        public async Task<CtTon?> GetInventoryByIdAsync(string maSP)
        {
            return await _context.CtTons
                .Include(c => c.MaSpNavigation)
                .FirstOrDefaultAsync(c => c.MaSp == maSP);
        }

        public async Task<List<CtTon>> SearchInventoryAsync(string searchTerm)
        {
            return await _context.CtTons
                .Include(c => c.MaSpNavigation)
                .Where(c => c.MaSpNavigation.TenSp.Contains(searchTerm) || c.MaSp.Contains(searchTerm))
                .ToListAsync();
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
                .Where(c => c.SoLuongTonKho <= threshold)
                .ToListAsync();
        }
    }
} 