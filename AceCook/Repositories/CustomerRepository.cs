using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AceCook.Models;

namespace AceCook.Repositories
{
    public class CustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Khachhang>> GetAllCustomersAsync()
        {
            return await _context.Khachhangs.ToListAsync();
        }

        public async Task<Khachhang?> GetCustomerByIdAsync(string maKH)
        {
            return await _context.Khachhangs.FirstOrDefaultAsync(k => k.MaKh == maKH);
        }

        public async Task<List<Khachhang>> SearchCustomersAsync(string searchTerm)
        {
            return await _context.Khachhangs
                .Where(k => k.TenKh.Contains(searchTerm) || k.MaKh.Contains(searchTerm) || k.Sdtkh.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<bool> AddCustomerAsync(Khachhang customer)
        {
            try
            {
                await _context.Khachhangs.AddAsync(customer);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateCustomerAsync(Khachhang customer)
        {
            try
            {
                _context.Khachhangs.Update(customer);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteCustomerAsync(string maKH)
        {
            try
            {
                var customer = await _context.Khachhangs.FirstOrDefaultAsync(k => k.MaKh == maKH);
                if (customer != null)
                {
                    _context.Khachhangs.Remove(customer);
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