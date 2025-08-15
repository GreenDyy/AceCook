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
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Khachhang>> GetAllCustomersAsync()
        {
            try
            {
                return await _context.Khachhangs
                    .AsNoTracking()
                    .OrderBy(k => k.TenKh ?? "")
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetAllCustomersAsync: {ex.Message}");
                throw new InvalidOperationException($"Không thể tải danh sách khách hàng: {ex.Message}", ex);
            }
        }

        public async Task<Khachhang?> GetCustomerByIdAsync(string maKH)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maKH))
                {
                    return null;
                }

                return await _context.Khachhangs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(k => k.MaKh == maKH);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetCustomerByIdAsync: {ex.Message}");
                throw new InvalidOperationException($"Không thể tải thông tin khách hàng: {ex.Message}", ex);
            }
        }

        public async Task<List<Khachhang>> SearchCustomersAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetAllCustomersAsync();
                }

                var searchLower = searchTerm.ToLower();
                return await _context.Khachhangs
                    .AsNoTracking()
                    .Where(k => (k.TenKh != null && k.TenKh.ToLower().Contains(searchLower)) || 
                               (k.MaKh != null && k.MaKh.ToLower().Contains(searchLower)) ||
                               (k.Sdtkh != null && k.Sdtkh.ToLower().Contains(searchLower)) ||
                               (k.EmailKh != null && k.EmailKh.ToLower().Contains(searchLower)))
                    .OrderBy(k => k.TenKh ?? "")
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SearchCustomersAsync: {ex.Message}");
                throw new InvalidOperationException($"Không thể tìm kiếm khách hàng: {ex.Message}", ex);
            }
        }

        public async Task<bool> AddCustomerAsync(Khachhang customer)
        {
            try
            {
                if (customer == null)
                {
                    throw new ArgumentNullException(nameof(customer));
                }

                // Validation
                if (string.IsNullOrWhiteSpace(customer.MaKh) || string.IsNullOrWhiteSpace(customer.TenKh))
                {
                    throw new InvalidOperationException("Mã khách hàng và tên khách hàng không được để trống.");
                }

                // Check if customer already exists
                var existingCustomer = await _context.Khachhangs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(k => k.MaKh == customer.MaKh);
                
                if (existingCustomer != null)
                {
                    throw new InvalidOperationException($"Khách hàng với mã '{customer.MaKh}' đã tồn tại.");
                }

                // Ensure all required fields are set
                var newCustomer = new Khachhang
                {
                    MaKh = customer.MaKh.Trim(),
                    TenKh = customer.TenKh?.Trim(),
                    LoaiKh = customer.LoaiKh?.Trim(),
                    Sdtkh = customer.Sdtkh?.Trim(),
                    DiaChiKh = customer.DiaChiKh?.Trim(),
                    EmailKh = customer.EmailKh?.Trim()
                };

                await _context.Khachhangs.AddAsync(newCustomer);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AddCustomerAsync: {ex.Message}");
                throw new InvalidOperationException($"Không thể thêm khách hàng: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateCustomerAsync(Khachhang customer)
        {
            try
            {
                if (customer == null)
                {
                    throw new ArgumentNullException(nameof(customer));
                }

                // Validation
                if (string.IsNullOrWhiteSpace(customer.MaKh) || string.IsNullOrWhiteSpace(customer.TenKh))
                {
                    throw new InvalidOperationException("Mã khách hàng và tên khách hàng không được để trống.");
                }

                // Check if customer exists
                var existingCustomer = await _context.Khachhangs
                    .FirstOrDefaultAsync(k => k.MaKh == customer.MaKh);
                
                if (existingCustomer == null)
                {
                    throw new InvalidOperationException($"Không tìm thấy khách hàng với mã '{customer.MaKh}'.");
                }

                // Update properties
                existingCustomer.TenKh = customer.TenKh?.Trim();
                existingCustomer.LoaiKh = customer.LoaiKh?.Trim();
                existingCustomer.Sdtkh = customer.Sdtkh?.Trim();
                existingCustomer.DiaChiKh = customer.DiaChiKh?.Trim();
                existingCustomer.EmailKh = customer.EmailKh?.Trim();

                _context.Khachhangs.Update(existingCustomer);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in UpdateCustomerAsync: {ex.Message}");
                throw new InvalidOperationException($"Không thể cập nhật khách hàng: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteCustomerAsync(string maKH)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maKH))
                {
                    throw new ArgumentException("Mã khách hàng không được để trống.");
                }

                var customer = await _context.Khachhangs
                    .FirstOrDefaultAsync(k => k.MaKh == maKH);
                
                if (customer == null)
                {
                    throw new InvalidOperationException($"Không tìm thấy khách hàng với mã '{maKH}'.");
                }

                // Check if customer is referenced in other tables
                var hasReferences = await _context.Dondathangs.AnyAsync(dd => dd.MaKh == maKH);

                if (hasReferences)
                {
                    throw new InvalidOperationException($"Không thể xóa khách hàng '{customer.TenKh}' vì đang được sử dụng trong hệ thống.");
                }

                _context.Khachhangs.Remove(customer);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in DeleteCustomerAsync: {ex.Message}");
                throw new InvalidOperationException($"Không thể xóa khách hàng: {ex.Message}", ex);
            }
        }

        public async Task<bool> CustomerExistsAsync(string maKH)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maKH))
                {
                    return false;
                }

                return await _context.Khachhangs
                    .AsNoTracking()
                    .AnyAsync(k => k.MaKh == maKH);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CustomerExistsAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<List<string>> GetCustomerTypesAsync()
        {
            try
            {
                return await _context.Khachhangs
                    .AsNoTracking()
                    .Where(k => !string.IsNullOrEmpty(k.LoaiKh))
                    .Select(k => k.LoaiKh!)
                    .Distinct()
                    .OrderBy(type => type)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetCustomerTypesAsync: {ex.Message}");
                throw new InvalidOperationException($"Không thể tải danh sách loại khách hàng: {ex.Message}", ex);
            }
        }
    }
} 