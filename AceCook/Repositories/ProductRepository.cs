using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AceCook.Models;

namespace AceCook.Repositories
{
    public class ProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Sanpham>> GetAllProductsAsync()
        {
            return await _context.Sanphams.ToListAsync();
        }

        public async Task<Sanpham?> GetProductByIdAsync(string maSP)
        {
            return await _context.Sanphams.FirstOrDefaultAsync(p => p.MaSp == maSP);
        }

        public async Task<List<Sanpham>> GetProductsByCategoryAsync(string loai)
        {
            return await _context.Sanphams.Where(p => p.Loai == loai).ToListAsync();
        }

        public async Task<List<Sanpham>> SearchProductsAsync(string searchTerm)
        {
            return await _context.Sanphams
                .Where(p => p.TenSp.Contains(searchTerm) || p.MaSp.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<bool> AddProductAsync(Sanpham product)
        {
            try
            {
                await _context.Sanphams.AddAsync(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateProductAsync(Sanpham product)
        {
            try
            {
                _context.Sanphams.Update(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(string maSP)
        {
            try
            {
                var product = await _context.Sanphams.FirstOrDefaultAsync(p => p.MaSp == maSP);
                if (product != null)
                {
                    _context.Sanphams.Remove(product);
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

        public async Task<List<string>> GetProductCategoriesAsync()
        {
            return await _context.Sanphams?
                .Select(p => p.Loai)
                ?.Distinct()
                ?.ToListAsync();
        }
    }
} 