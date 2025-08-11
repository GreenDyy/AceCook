using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AceCook.Models;

namespace AceCook.Repositories
{
    public class AuthRepository
    {
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool success, Taikhoan? account, Nhanvien? employee, Quyentruycap? permission)> 
            AuthenticateAsync(string username, string password)
        {
            try
            {
                var account = await _context.Taikhoans
                    .Include(t => t.MaNvNavigation)
                    .Include(t => t.MaPqNavigation)
                    .FirstOrDefaultAsync(t => t.TenDangNhap == username && t.MatKhau == password);

                if (account != null && account.TrangThaiTk == "Hoạt động")
                {
                    return (true, account, account.MaNvNavigation, account.MaPqNavigation);
                }

                return (false, null, null, null);
            }
            catch (Exception)
            {
                return (false, null, null, null);
            }
        }

        public async Task<bool> ChangePasswordAsync(string maTK, string oldPassword, string newPassword)
        {
            try
            {
                var account = await _context.Taikhoans.FindAsync(maTK);
                if (account != null && account.MatKhau == oldPassword)
                {
                    account.MatKhau = newPassword;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
} 