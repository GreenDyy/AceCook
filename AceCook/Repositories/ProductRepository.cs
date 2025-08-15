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
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Sanpham>> GetAllProductsAsync()
        {
            try
            {
                return await _context.Sanphams
                    .AsNoTracking()
                    .OrderBy(p => p.TenSp ?? "")
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetAllProductsAsync: {ex.Message}");
                throw new InvalidOperationException($"Không thể tải danh sách sản phẩm: {ex.Message}", ex);
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

                return await _context.Sanphams
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.MaSp == maSP);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetProductByIdAsync: {ex.Message}");
                throw new InvalidOperationException($"Không thể tải thông tin sản phẩm: {ex.Message}", ex);
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
                    .AsNoTracking()
                    .Where(p => p.Loai == loai)
                    .OrderBy(p => p.TenSp ?? "")
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetProductsByCategoryAsync: {ex.Message}");
                throw new InvalidOperationException($"Không thể tải sản phẩm theo loại: {ex.Message}", ex);
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

                var searchLower = searchTerm.ToLower();
                return await _context.Sanphams
                    .AsNoTracking()
                    .Where(p => (p.TenSp != null && p.TenSp.ToLower().Contains(searchLower)) || 
                               (p.MaSp != null && p.MaSp.ToLower().Contains(searchLower)) ||
                               (p.MoTa != null && p.MoTa.ToLower().Contains(searchLower)) ||
                               (p.Loai != null && p.Loai.ToLower().Contains(searchLower)))
                    .OrderBy(p => p.TenSp ?? "")
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SearchProductsAsync: {ex.Message}");
                throw new InvalidOperationException($"Không thể tìm kiếm sản phẩm: {ex.Message}", ex);
            }
        }

        public async Task<bool> AddProductAsync(Sanpham product)
        {
            try
            {
                if (product == null)
                {
                    throw new ArgumentNullException(nameof(product));
                }

                // Validation
                if (string.IsNullOrWhiteSpace(product.MaSp) || string.IsNullOrWhiteSpace(product.TenSp))
                {
                    throw new InvalidOperationException("Mã sản phẩm và tên sản phẩm không được để trống.");
                }

                // Check if product already exists
                var existingProduct = await _context.Sanphams
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.MaSp == product.MaSp);
                
                if (existingProduct != null)
                {
                    throw new InvalidOperationException($"Sản phẩm với mã '{product.MaSp}' đã tồn tại.");
                }

                // Ensure all required fields are set
                var newProduct = new Sanpham
                {
                    MaSp = product.MaSp.Trim(),
                    TenSp = product.TenSp?.Trim(),
                    MoTa = product.MoTa?.Trim(),
                    Gia = product.Gia,
                    Dvtsp = product.Dvtsp?.Trim(),
                    Loai = product.Loai?.Trim()
                };

                await _context.Sanphams.AddAsync(newProduct);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AddProductAsync: {ex.Message}");
                throw new InvalidOperationException($"Không thể thêm sản phẩm: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateProductAsync(Sanpham product)
        {
            try
            {
                if (product == null)
                {
                    throw new ArgumentNullException(nameof(product));
                }

                // Validation
                if (string.IsNullOrWhiteSpace(product.MaSp) || string.IsNullOrWhiteSpace(product.TenSp))
                {
                    throw new InvalidOperationException("Mã sản phẩm và tên sản phẩm không được để trống.");
                }

                // Check if product exists
                var existingProduct = await _context.Sanphams
                    .FirstOrDefaultAsync(p => p.MaSp == product.MaSp);
                
                if (existingProduct == null)
                {
                    throw new InvalidOperationException($"Không tìm thấy sản phẩm với mã '{product.MaSp}'.");
                }

                // Update properties
                existingProduct.TenSp = product.TenSp?.Trim();
                existingProduct.MoTa = product.MoTa?.Trim();
                existingProduct.Gia = product.Gia;
                existingProduct.Dvtsp = product.Dvtsp?.Trim();
                existingProduct.Loai = product.Loai?.Trim();

                _context.Sanphams.Update(existingProduct);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in UpdateProductAsync: {ex.Message}");
                throw new InvalidOperationException($"Không thể cập nhật sản phẩm: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteProductAsync(string maSP)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maSP))
                {
                    throw new ArgumentException("Mã sản phẩm không được để trống.");
                }

                var product = await _context.Sanphams
                    .FirstOrDefaultAsync(p => p.MaSp == maSP);
                
                if (product == null)
                {
                    throw new InvalidOperationException($"Không tìm thấy sản phẩm với mã '{maSP}'.");
                }

                // Check if product is referenced in other tables
                var hasReferences = await _context.CtDhs.AnyAsync(ct => ct.MaSp == maSP) ||
                                   await _context.CtTons.AnyAsync(ct => ct.MaSp == maSP) ||
                                   await _context.DinhMucs.AnyAsync(dm => dm.MaSp == maSP);

                if (hasReferences)
                {
                    throw new InvalidOperationException($"Không thể xóa sản phẩm '{product.TenSp}' vì đang được sử dụng trong hệ thống.");
                }

                _context.Sanphams.Remove(product);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in DeleteProductAsync: {ex.Message}");
                throw new InvalidOperationException($"Không thể xóa sản phẩm: {ex.Message}", ex);
            }
        }

        public async Task<List<string>> GetProductCategoriesAsync()
        {
            try
            {
                return await _context.Sanphams
                    .AsNoTracking()
                    .Where(p => !string.IsNullOrEmpty(p.Loai))
                    .Select(p => p.Loai!)
                    .Distinct()
                    .OrderBy(cat => cat)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetProductCategoriesAsync: {ex.Message}");
                throw new InvalidOperationException($"Không thể tải danh sách loại sản phẩm: {ex.Message}", ex);
            }
        }

        public async Task<bool> ProductExistsAsync(string maSP)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maSP))
                {
                    return false;
                }

                return await _context.Sanphams
                    .AsNoTracking()
                    .AnyAsync(p => p.MaSp == maSP);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ProductExistsAsync: {ex.Message}");
                return false;
            }
        }
    }
} 