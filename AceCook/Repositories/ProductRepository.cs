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
            try
            {
                return await _context.Sanphams
                    .OrderBy(p => p.TenSp)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetAllProductsAsync: {ex.Message}");
                return new List<Sanpham>();
            }
        }

        public async Task<Sanpham?> GetProductByIdAsync(string maSP)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maSP))
                {
                    return null;
                }

                return await _context.Sanphams.FirstOrDefaultAsync(p => p.MaSp == maSP);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetProductByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Sanpham>> GetProductsByCategoryAsync(string loai)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loai))
                {
                    return new List<Sanpham>();
                }

                return await _context.Sanphams
                    .Where(p => p.Loai == loai)
                    .OrderBy(p => p.TenSp)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetProductsByCategoryAsync: {ex.Message}");
                return new List<Sanpham>();
            }
        }

        public async Task<List<Sanpham>> SearchProductsAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetAllProductsAsync();
                }

                return await _context.Sanphams
                    .Where(p => (p.TenSp != null && p.TenSp.Contains(searchTerm)) || 
                               (p.MaSp != null && p.MaSp.Contains(searchTerm)) ||
                               (p.MoTa != null && p.MoTa.Contains(searchTerm)) ||
                               (p.Loai != null && p.Loai.Contains(searchTerm)))
                    .OrderBy(p => p.TenSp)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SearchProductsAsync: {ex.Message}");
                return new List<Sanpham>();
            }
        }

        public async Task<bool> AddProductAsync(Sanpham product)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(product.MaSp) || string.IsNullOrWhiteSpace(product.TenSp))
                {
                    return false;
                }

                // Check if product already exists
                var existingProduct = await _context.Sanphams.FirstOrDefaultAsync(p => p.MaSp == product.MaSp);
                if (existingProduct != null)
                {
                    return false; // Product already exists
                }

                await _context.Sanphams.AddAsync(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AddProductAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateProductAsync(Sanpham product)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(product.MaSp) || string.IsNullOrWhiteSpace(product.TenSp))
                {
                    return false;
                }

                // Check if product exists
                var existingProduct = await _context.Sanphams.FirstOrDefaultAsync(p => p.MaSp == product.MaSp);
                if (existingProduct == null)
                {
                    return false; // Product not found
                }

                // Update properties
                existingProduct.TenSp = product.TenSp;
                existingProduct.MoTa = product.MoTa;
                existingProduct.Gia = product.Gia;
                existingProduct.Dvtsp = product.Dvtsp;
                existingProduct.Loai = product.Loai;

                _context.Sanphams.Update(existingProduct);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in UpdateProductAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(string maSP)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maSP))
                {
                    return false;
                }

                var product = await _context.Sanphams.FirstOrDefaultAsync(p => p.MaSp == maSP);
                if (product != null)
                {
                    _context.Sanphams.Remove(product);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in DeleteProductAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<List<string>> GetProductCategoriesAsync()
        {
            try
            {
                return await _context.Sanphams
                    .Where(p => !string.IsNullOrEmpty(p.Loai))
                    .Select(p => p.Loai)
                    .Distinct()
                    .OrderBy(cat => cat)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetProductCategoriesAsync: {ex.Message}");
                return new List<string>();
            }
        }
    }
} 