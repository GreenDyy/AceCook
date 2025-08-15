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
                // Validation trước khi thêm
                if (string.IsNullOrWhiteSpace(order.MaDdh))
                    throw new InvalidOperationException("Mã đơn hàng không được để trống!");
                
                if (string.IsNullOrWhiteSpace(order.MaKh))
                    throw new InvalidOperationException("Khách hàng không được để trống!");
                
                if (string.IsNullOrWhiteSpace(order.MaNv))
                    throw new InvalidOperationException("Mã nhân viên không được để trống!");
                
                if (order.CtDhs == null || !order.CtDhs.Any())
                    throw new InvalidOperationException("Đơn hàng phải có ít nhất một sản phẩm!");
                
                // Trạng thái mặc định là "Hoàn thành"
                if (string.IsNullOrWhiteSpace(order.TrangThai))
                    order.TrangThai = "Hoàn thành";

                // Kiểm tra mã đơn hàng đã tồn tại chưa
                var existingOrder = await _context.Dondathangs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.MaDdh == order.MaDdh);
                
                if (existingOrder != null)
                    throw new InvalidOperationException($"Mã đơn hàng {order.MaDdh} đã tồn tại!");

                // Kiểm tra tồn kho trước khi tạo đơn hàng
                foreach (var ct in order.CtDhs)
                {
                    var availableStock = await GetAvailableStockAsync(ct.MaSp);
                    if (availableStock < ct.SoLuong)
                    {
                        throw new InvalidOperationException(
                            $"Sản phẩm {ct.MaSp} không đủ tồn kho! Yêu cầu: {ct.SoLuong}, Tồn kho: {availableStock}");
                    }
                }

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Thêm đơn hàng
                    await _context.Dondathangs.AddAsync(order);
                    await _context.SaveChangesAsync();

                    // Cập nhật tồn kho
                    foreach (var ct in order.CtDhs)
                    {
                        await UpdateInventoryAsync(ct.MaSp, ct.SoLuong.Value);
                    }

                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error during transaction in AddOrderAsync: {ex.Message}");
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AddOrderAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        // Thêm method để kiểm tra tồn kho
        public async Task<int> GetAvailableStockAsync(string productId)
        {
            try
            {
                if (string.IsNullOrEmpty(productId))
                    return 0;

                var inventory = await _context.CtTons
                    .FirstOrDefaultAsync(ct => ct.MaSp == productId && ct.MaKho == "K01");
                
                return inventory?.SoLuongTonKho ?? 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting available stock for {productId}: {ex.Message}");
                return 0;
            }
        }

        // Thêm method để cập nhật tồn kho
        public async Task UpdateInventoryAsync(string productId, int quantity)
        {
            try
            {
                var inventory = await _context.CtTons
                    .FirstOrDefaultAsync(ct => ct.MaSp == productId && ct.MaKho == "K01");

                if (inventory != null)
                {
                    var currentStock = inventory.SoLuongTonKho ?? 0;
                    if (currentStock < quantity)
                    {
                        throw new InvalidOperationException($"Tồn kho không đủ! Yêu cầu: {quantity}, Hiện có: {currentStock}");
                    }

                    inventory.SoLuongTonKho = Math.Max(0, currentStock - quantity);
                }
                else
                {
                    inventory = new CtTon 
                    { 
                        MaSp = productId, 
                        MaKho = "K01", 
                        SoLuongTonKho = 0 
                    };
                    await _context.CtTons.AddAsync(inventory);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating inventory for {productId}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Tự động sinh mã đơn hàng tiếp theo theo format DH + số
        /// </summary>
        /// <returns>Mã đơn hàng mới (ví dụ: DH008)</returns>
        public async Task<string> GenerateNextOrderIdAsync()
        {
            try
            {
                // Lấy tất cả mã đơn hàng hiện có
                var existingOrderIds = await _context.Dondathangs
                    .AsNoTracking()
                    .Where(d => d.MaDdh.StartsWith("DH"))
                    .Select(d => d.MaDdh)
                    .ToListAsync();

                if (!existingOrderIds.Any())
                {
                    return "DH001"; // Nếu chưa có đơn hàng nào
                }

                // Tìm số lớn nhất trong các mã hiện có
                var maxNumber = 0;
                foreach (var orderId in existingOrderIds)
                {
                    if (orderId.Length >= 3 && int.TryParse(orderId.Substring(2), out int number))
                    {
                        maxNumber = Math.Max(maxNumber, number);
                    }
                }

                // Trả về mã tiếp theo
                return $"DH{(maxNumber + 1):D3}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating next order ID: {ex.Message}");
                // Fallback: trả về mã với timestamp nếu có lỗi
                return $"DH{DateTime.Now:yyyyMMddHHmmss}";
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
            var startDateOnly = DateOnly.FromDateTime(startDate);
            var endDateOnly = DateOnly.FromDateTime(endDate);
            
            // Materialize the query first to avoid EF translation issues
            var orders = await _context.Dondathangs
                .Include(d => d.CtDhs)
                .Where(d => d.NgayDat.HasValue && d.TrangThai == "Đã giao")
                .ToListAsync();
                
            return orders
                .Where(d => d.NgayDat.HasValue &&
                           d.NgayDat.Value >= startDateOnly &&
                           d.NgayDat.Value <= endDateOnly)
                .SelectMany(d => d.CtDhs)
                .Sum(ct => (decimal)((ct.SoLuong ?? 0) * (ct.DonGia ?? 0)));
        }

        public async Task<int> GetTotalOrdersAsync(DateTime startDate, DateTime endDate)
        {
            var startDateOnly = DateOnly.FromDateTime(startDate);
            var endDateOnly = DateOnly.FromDateTime(endDate);
            
            // Materialize the query first to avoid EF translation issues
            var orders = await _context.Dondathangs
                .Where(d => d.NgayDat.HasValue)
                .ToListAsync();
                
            return orders
                .Where(d => d.NgayDat.HasValue &&
                           d.NgayDat.Value >= startDateOnly &&
                           d.NgayDat.Value <= endDateOnly)
                .Count();
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
            try
            {
                // Lấy tất cả mã đơn hàng hiện có bắt đầu bằng "DH"
                var existingOrderIds = await _context.Dondathangs
                    .AsNoTracking()
                    .Where(d => d.MaDdh.StartsWith("DH"))
                    .Select(d => d.MaDdh)
                    .ToListAsync();

                if (!existingOrderIds.Any())
                {
                    return "DH001"; // Nếu chưa có đơn hàng nào
                }

                // Tìm số lớn nhất trong các mã hiện có
                var maxNumber = 0;
                foreach (var orderId in existingOrderIds)
                {
                    if (orderId.Length >= 3 && int.TryParse(orderId.Substring(2), out int number))
                    {
                        maxNumber = Math.Max(maxNumber, number);
                    }
                }

                // Trả về mã tiếp theo theo format DH + số 3 chữ số
                return $"DH{(maxNumber + 1):D3}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating next order ID: {ex.Message}");
                // Fallback: trả về mã với timestamp nếu có lỗi
                return $"DH{DateTime.Now:yyyyMMddHHmmss}";
            }
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

            // Get all orders first, then apply date filter in memory to avoid EF translation issues
            var allOrders = await query.ToListAsync();
            
            // Apply date range filter in memory
            if (startDate.HasValue && endDate.HasValue)
            {
                var startDateOnly = DateOnly.FromDateTime(startDate.Value);
                var endDateOnly = DateOnly.FromDateTime(endDate.Value);
                
                allOrders = allOrders.Where(d => d.NgayDat.HasValue &&
                                               d.NgayDat.Value >= startDateOnly &&
                                               d.NgayDat.Value <= endDateOnly)
                                   .ToList();
            }

            return allOrders.OrderByDescending(d => d.NgayDat).ToList();
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

        /// <summary>
        /// Lấy tổng tiền đơn hàng từ bảng HOADONBAN.TongTien
        /// </summary>
        /// <param name="maDDH">Mã đơn đặt hàng</param>
        /// <returns>Tổng tiền đơn hàng, 0 nếu không tìm thấy hóa đơn</returns>
        public async Task<decimal> GetOrderTotalFromInvoiceAsync(string maDDH)
        {
            try
            {
                // Vì không có liên kết trực tiếp giữa DONDATHANG và HOADONBAN,
                // nên tính toán tổng tiền từ chi tiết đơn hàng
                var order = await _context.Dondathangs
                    .Include(d => d.CtDhs)
                    .FirstOrDefaultAsync(d => d.MaDdh == maDDH);

                if (order?.CtDhs != null)
                {
                    return (decimal)order.CtDhs.Sum(ct => (ct.SoLuong ?? 0) * (ct.DonGia ?? 0));
                }

                return 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting order total from invoice: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Tạo hóa đơn bán khi tạo đơn hàng
        /// </summary>
        /// <param name="maDDH">Mã đơn đặt hàng</param>
        /// <param name="totalAmount">Tổng tiền đơn hàng</param>
        /// <returns>Mã hóa đơn bán được tạo</returns>
        public async Task<string> CreateInvoiceForOrderAsync(string maDDH, decimal totalAmount)
        {
            try
            {
                // Kiểm tra xem đơn hàng có tồn tại không
                var existingOrder = await _context.Dondathangs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.MaDdh == maDDH);
                
                if (existingOrder == null)
                {
                    throw new InvalidOperationException($"Không tìm thấy đơn hàng {maDDH}");
                }

                // Tạo mã hóa đơn bán mới
                var invoiceId = await GenerateInvoiceIdAsync();
                
                // Tạo hóa đơn bán
                var invoice = new Hoadonban
                {
                    MaHdb = invoiceId,
                    NgayLap = DateOnly.FromDateTime(DateTime.Now),
                    TongTien = totalAmount,
                    Vat = 0, // Có thể cập nhật sau
                    TrangThaiThanhToan = "Chưa thanh toán",
                    MaNv = existingOrder.MaNv ?? "NV001"
                };

                await _context.Hoadonbans.AddAsync(invoice);

                // Tạo phiếu xuất kho liên kết với hóa đơn
                var phieuXuatKho = new Phieuxuatkho
                {
                    MaPxk = await GeneratePhieuXuatKhoIdAsync(),
                    NgayXuat = DateOnly.FromDateTime(DateTime.Now),
                    MaHdb = invoiceId,
                    MaKho = "K01" // Kho mặc định
                };

                await _context.Phieuxuatkhos.AddAsync(phieuXuatKho);
                await _context.SaveChangesAsync();

                return invoiceId;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating invoice for order {maDDH}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Sinh mã hóa đơn bán mới
        /// </summary>
        private async Task<string> GenerateInvoiceIdAsync()
        {
            try
            {
                var existingIds = await _context.Hoadonbans
                    .AsNoTracking()
                    .Where(h => h.MaHdb.StartsWith("HD"))
                    .Select(h => h.MaHdb)
                    .ToListAsync();

                if (!existingIds.Any())
                    return "HD001";

                var maxNumber = 0;
                foreach (var id in existingIds)
                {
                    if (id.Length >= 3 && int.TryParse(id.Substring(2), out int number))
                    {
                        maxNumber = Math.Max(maxNumber, number);
                    }
                }

                return $"HD{(maxNumber + 1):D3}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating invoice ID: {ex.Message}");
                return $"HD{DateTime.Now:yyyyMMddHHmmss}";
            }
        }

        /// <summary>
        /// Sinh mã phiếu xuất kho mới
        /// </summary>
        private async Task<string> GeneratePhieuXuatKhoIdAsync()
        {
            try
            {
                var existingIds = await _context.Phieuxuatkhos
                    .AsNoTracking()
                    .Where(p => p.MaPxk.StartsWith("PX"))
                    .Select(p => p.MaPxk)
                    .ToListAsync();

                if (!existingIds.Any())
                    return "PX001";

                var maxNumber = 0;
                foreach (var id in existingIds)
                {
                    if (id.Length >= 3 && int.TryParse(id.Substring(2), out int number))
                    {
                        maxNumber = Math.Max(maxNumber, number);
                    }
                }

                return $"PX{(maxNumber + 1):D3}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating phieu xuat kho ID: {ex.Message}");
                return $"PX{DateTime.Now:yyyyMMddHHmmss}";
            }
        }

        /// <summary>
        /// Lấy mã nhân viên từ đơn hàng
        /// </summary>
        private async Task<string> GetOrderEmployeeAsync(string maDDH)
        {
            var order = await _context.Dondathangs
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.MaDdh == maDDH);
            
            return order?.MaNv ?? "NV001";
        }
    }
}