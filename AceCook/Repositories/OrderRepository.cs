using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AceCook.Models;

namespace AceCook.Repositories
{
    public class OrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Dondathang>> GetAllOrdersAsync()
        {
            return await _context.Dondathangs
                .Include(d => d.MaKhNavigation)
                .Include(d => d.MaNvNavigation)
                .Include(d => d.CtDhs)
                    .ThenInclude(ct => ct.MaSpNavigation)
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();
        }

        public async Task<Dondathang?> GetOrderByIdAsync(string maDDH)
        {
            try
            {
                var order = await _context.Dondathangs
                    .Include(d => d.MaKhNavigation)
                    .Include(d => d.MaNvNavigation)
                    .Include(d => d.CtDhs)
                        .ThenInclude(ct => ct.MaSpNavigation)
                    .FirstOrDefaultAsync(d => d.MaDdh == maDDH);

                // Đảm bảo các navigation properties không null
                if (order != null)
                {
                    if (order.CtDhs == null)
                        order.CtDhs = new List<CtDh>();
                    
                    if (order.MaKhNavigation == null)
                        order.MaKhNavigation = new Khachhang();
                    
                    if (order.MaNvNavigation == null)
                        order.MaNvNavigation = new Nhanvien();
                }

                return order;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetOrderByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Dondathang>> SearchOrdersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllOrdersAsync();

            return await _context.Dondathangs
                .Include(d => d.MaKhNavigation)
                .Include(d => d.MaNvNavigation)
                .Include(d => d.CtDhs)
                .Where(d => d.MaDdh.Contains(searchTerm) || 
                           (d.MaKhNavigation != null && d.MaKhNavigation.TenKh != null && d.MaKhNavigation.TenKh.Contains(searchTerm)) ||
                           d.TrangThai.Contains(searchTerm))
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();
        }

        public async Task<List<Dondathang>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            // Lấy tất cả đơn hàng để debug
            var allOrders = await _context.Dondathangs
                .Include(d => d.MaKhNavigation)
                .Include(d => d.MaNvNavigation)
                .Include(d => d.CtDhs)
                .ToListAsync();

            // In ra số lượng và ngày đặt của tất cả đơn hàng
            var orderDates = string.Join(", ", allOrders.Select(o => o.NgayDat?.ToString() ?? "null"));

            var startDateOnly = DateOnly.FromDateTime(startDate);
            var endDateOnly = DateOnly.FromDateTime(endDate);
            var filteredOrders = allOrders.Where(d => d.NgayDat.HasValue &&
                                                     d.NgayDat.Value >= startDateOnly &&
                                                     d.NgayDat.Value <= endDateOnly)
                                        .OrderByDescending(d => d.NgayDat)
                                        .ToList();

            return filteredOrders;
        }

        public async Task<List<Dondathang>> GetOrdersByStatusAsync(string status)
        {
            return await _context.Dondathangs
                .Include(d => d.MaKhNavigation)
                .Include(d => d.MaNvNavigation)
                .Include(d => d.CtDhs)
                .Where(d => d.TrangThai == status)
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();
        }

        public async Task<bool> AddOrderAsync(Dondathang order)
        {
            try
            {
                await _context.Dondathangs.AddAsync(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateOrderAsync(Dondathang order)
        {
            try
            {
                // Kiểm tra trạng thái đơn hàng trước khi cho phép cập nhật
                var existingOrder = await _context.Dondathangs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.MaDdh == order.MaDdh);
                
                if (existingOrder != null)
                {
                    if (existingOrder.TrangThai == "Hoàn thành" || existingOrder.TrangThai == "Đã giao")
                    {
                        throw new InvalidOperationException($"Không thể cập nhật đơn hàng có trạng thái '{existingOrder.TrangThai}'!");
                    }
                }

                _context.Dondathangs.Update(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (InvalidOperationException)
            {
                throw; // Re-throw business logic exceptions
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating order: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteOrderAsync(string maDDH)
        {
            try
            {
                var order = await _context.Dondathangs
                    .Include(d => d.CtDhs)
                    .FirstOrDefaultAsync(d => d.MaDdh == maDDH);
                
                if (order != null)
                {
                    // Kiểm tra trạng thái đơn hàng trước khi cho phép xóa
                    if (order.TrangThai == "Hoàn thành" || order.TrangThai == "Đã giao")
                    {
                        throw new InvalidOperationException($"Không thể xóa đơn hàng có trạng thái '{order.TrangThai}'!");
                    }

                    // Xóa chi tiết đơn hàng trước
                    _context.CtDhs.RemoveRange(order.CtDhs);
                    _context.Dondathangs.Remove(order);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (InvalidOperationException)
            {
                throw; // Re-throw business logic exceptions
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting order: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(string maDDH, string newStatus)
        {
            try
            {
                var order = await _context.Dondathangs.FirstOrDefaultAsync(d => d.MaDdh == maDDH);
                if (order != null)
                {
                    // Kiểm tra trạng thái hiện tại
                    if (order.TrangThai == "Hoàn thành" || order.TrangThai == "Đã giao")
                    {
                        throw new InvalidOperationException($"Không thể thay đổi trạng thái đơn hàng đã hoàn thành!");
                    }

                    order.TrangThai = newStatus;
                    if (newStatus == "Đã giao" || newStatus == "Hoàn thành")
                    {
                        order.NgayGiao = DateOnly.FromDateTime(DateTime.Now);
                    }
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (InvalidOperationException)
            {
                throw; // Re-throw business logic exceptions
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating order status: {ex.Message}");
                return false;
            }
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Dondathangs
                .Where(d => d.NgayDat.HasValue &&
                            d.NgayDat.Value.ToDateTime(TimeOnly.MinValue) >= startDate &&
                            d.NgayDat.Value.ToDateTime(TimeOnly.MinValue) <= endDate &&
                            d.TrangThai == "Đã giao")
                .SelectMany(d => d.CtDhs)
                .SumAsync(ct => (decimal)((ct.SoLuong ?? 0) * (ct.DonGia ?? 0)));
        }

        public async Task<int> GetTotalOrdersAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Dondathangs
                .Where(d => d.NgayDat.HasValue &&
                            d.NgayDat.Value.ToDateTime(TimeOnly.MinValue) >= startDate &&
                            d.NgayDat.Value.ToDateTime(TimeOnly.MinValue) <= endDate)
                .CountAsync();
        }

        public async Task<List<Khachhang>> GetAllCustomersAsync()
        {
            return await _context.Khachhangs
                .OrderBy(k => k.TenKh)
                .ToListAsync();
        }

        public async Task<List<Sanpham>> GetAllProductsAsync()
        {
            return await _context.Sanphams
                .OrderBy(s => s.TenSp)
                .ToListAsync();
        }

        public async Task<string> GenerateOrderIdAsync()
        {
            var lastOrder = await _context.Dondathangs
                .OrderByDescending(d => d.MaDdh)
                .FirstOrDefaultAsync();

            if (lastOrder == null)
                return "DDH001";

            var lastNumber = int.Parse(lastOrder.MaDdh.Substring(3));
            return $"DDH{(lastNumber + 1):D3}";
        }

        public async Task<List<Dondathang>> GetFilteredOrdersAsync(string? searchTerm, string? status, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Dondathangs
                .Include(d => d.MaKhNavigation)
                .Include(d => d.MaNvNavigation)
                .Include(d => d.CtDhs)
                    .ThenInclude(ct => ct.MaSpNavigation)
                .AsQueryable();

            // Apply status filter
            if (!string.IsNullOrWhiteSpace(status) && status != "Tất cả")
            {
                query = query.Where(d => d.TrangThai == status);
            }

            // Apply date range filter - sử dụng DateTime để tránh lỗi LINQ translation
            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(d => d.NgayDat.HasValue &&
                                       d.NgayDat.Value.ToDateTime(TimeOnly.MinValue) >= startDate.Value &&
                                       d.NgayDat.Value.ToDateTime(TimeOnly.MinValue) <= endDate.Value);
            }

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchTermLower = searchTerm.ToLower();
                query = query.Where(d => 
                    (d.MaDdh != null && d.MaDdh.ToLower().Contains(searchTermLower)) ||
                    (d.MaKhNavigation != null && d.MaKhNavigation.TenKh != null && 
                     d.MaKhNavigation.TenKh.ToLower().Contains(searchTermLower)) ||
                    (d.TrangThai != null && d.TrangThai.ToLower().Contains(searchTermLower))
                );
            }

            return await query.OrderByDescending(d => d.NgayDat).ToListAsync();
        }

        // Class để chứa kết quả thống kê trạng thái
        public class OrderStatusStatistic
        {
            public string Status { get; set; } = string.Empty;
            public int Count { get; set; }
            public decimal TotalValue { get; set; }
        }

        // Class để chứa kết quả thống kê khách hàng
        public class TopCustomerStatistic
        {
            public string CustomerName { get; set; } = string.Empty;
            public int OrderCount { get; set; }
            public decimal TotalValue { get; set; }
        }

        // Phương thức lấy thống kê theo trạng thái
        public async Task<List<OrderStatusStatistic>> GetOrderStatusStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            var startDateOnly = DateOnly.FromDateTime(startDate);
            var endDateOnly = DateOnly.FromDateTime(endDate);

            var orders = await _context.Dondathangs
                .Include(d => d.CtDhs)
                .Where(d => d.NgayDat.HasValue &&
                            d.NgayDat.Value >= startDateOnly &&
                            d.NgayDat.Value <= endDateOnly)
                .ToListAsync();

            return orders
                .GroupBy(o => o.TrangThai ?? "Không xác định")
                .Select(g => new OrderStatusStatistic
                {
                    Status = g.Key,
                    Count = g.Count(),
                    TotalValue = g.Sum(o => o.CtDhs.Sum(ct => (decimal)(ct.SoLuong * ct.DonGia ?? 0)))
                })
                .OrderByDescending(x => x.Count)
                .ToList();
        }

        // Phương thức lấy thống kê top khách hàng
        public async Task<List<TopCustomerStatistic>> GetTopCustomersStatisticsAsync(DateTime startDate, DateTime endDate, int top = 10)
        {
            var startDateOnly = DateOnly.FromDateTime(startDate);
            var endDateOnly = DateOnly.FromDateTime(endDate);

            var orders = await _context.Dondathangs
                .Include(d => d.MaKhNavigation)
                .Include(d => d.CtDhs)
                .Where(d => d.NgayDat.HasValue &&
                            d.NgayDat.Value >= startDateOnly &&
                            d.NgayDat.Value <= endDateOnly &&
                            d.MaKhNavigation != null)
                .ToListAsync();

            return orders
                .GroupBy(o => o.MaKhNavigation)
                .Select(g => new TopCustomerStatistic
                {
                    CustomerName = g.Key.TenKh ?? "Không xác định",
                    OrderCount = g.Count(),
                    TotalValue = g.Sum(o => o.CtDhs.Sum(ct => (decimal)(ct.SoLuong * ct.DonGia ?? 0)))
                })
                .OrderByDescending(x => x.TotalValue)
                .Take(top)
                .ToList();
        }
    }
}