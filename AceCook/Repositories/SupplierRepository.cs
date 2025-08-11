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
            return await _context.Nhacungcaps.ToListAsync();
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
                _context.Nhacungcaps.Update(supplier);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
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