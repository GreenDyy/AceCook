using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AceCook.Models;

namespace AceCook.Repositories
{
    public class SupplierRepository
    {
        private readonly AppDbContext _context;

        public SupplierRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Nhacungcap>> GetAllSuppliersAsync()
        {
            return await _context.Nhacungcaps
                .Include(n => n.Nguyenlieus)
                .ToListAsync();
        }

        public async Task<Nhacungcap?> GetSupplierByIdAsync(string maNCC)
        {
            return await _context.Nhacungcaps.FirstOrDefaultAsync(n => n.MaNcc == maNCC);
        }

        public async Task<List<Nhacungcap>> SearchSuppliersAsync(string searchTerm)
        {
            return await _context.Nhacungcaps
                .Where(n => n.TenNcc.Contains(searchTerm) || n.MaNcc.Contains(searchTerm) || n.Sdtncc.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<bool> AddSupplierAsync(Nhacungcap supplier)
        {
            try
            {
                await _context.Nhacungcaps.AddAsync(supplier);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateSupplierAsync(Nhacungcap supplier)
        {
            try
            {
                // Tìm nhà cung cấp hiện tại trong database
                var existingSupplier = await _context.Nhacungcaps
                    .FirstOrDefaultAsync(n => n.MaNcc == supplier.MaNcc);

                if (existingSupplier == null)
                {
                    return false;
                }

                // Cập nhật từng trường thông tin
                existingSupplier.TenNcc = supplier.TenNcc;
                existingSupplier.Sdtncc = supplier.Sdtncc;
                existingSupplier.EmailNcc = supplier.EmailNcc;
                existingSupplier.DiaChiNcc = supplier.DiaChiNcc;

                // Lưu thay đổi
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log lỗi để debug
                Console.WriteLine($"Error in UpdateSupplierAsync: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                throw; // Ném lại exception để form có thể xử lý
            }
        }

        public async Task<bool> DeleteSupplierAsync(string maNCC)
        {
            try
            {
                var supplier = await _context.Nhacungcaps.FirstOrDefaultAsync(n => n.MaNcc == maNCC);
                if (supplier != null)
                {
                    _context.Nhacungcaps.Remove(supplier);
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
    }
} 