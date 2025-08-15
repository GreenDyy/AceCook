# Hướng dẫn khắc phục lỗi InventoryManagementForm

## Các lỗi thường gặp và cách khắc phục

### 1. Lỗi "Không thể kết nối đến cơ sở dữ liệu"

**Nguyên nhân:**
- Mất kết nối mạng
- Database server không hoạt động
- Connection string không đúng
- Firewall chặn kết nối

**Cách khắc phục:**
1. Kiểm tra kết nối mạng
2. Kiểm tra database server có đang chạy không
3. Kiểm tra connection string trong `AppDbContext.cs`
4. Kiểm tra firewall settings

**Connection string hiện tại:**
```
Server=14.161.21.15,1433;Database=QLBH_ACECOOK;User Id=sa;Password=Ezin@123;TrustServerCertificate=true;MultipleActiveResultSets=true
```

### 2. Lỗi "Timeout khi load dữ liệu tồn kho"

**Nguyên nhân:**
- Database server quá tải
- Query phức tạp mất nhiều thời gian
- Kết nối mạng chậm

**Cách khắc phục:**
1. Đợi một lúc rồi thử lại
2. Kiểm tra performance của database server
3. Tối ưu hóa query (đã được thực hiện với `AsNoTracking()`)

### 3. Lỗi "Lỗi cập nhật cơ sở dữ liệu"

**Nguyên nhân:**
- Không có quyền ghi vào database
- Constraint violation
- Database schema thay đổi

**Cách khắc phục:**
1. Kiểm tra quyền của user database
2. Kiểm tra dữ liệu đầu vào có hợp lệ không
3. Kiểm tra cấu trúc database

### 4. Lỗi "Lỗi thao tác dữ liệu"

**Nguyên nhân:**
- Navigation properties null
- Dữ liệu không nhất quán
- Lỗi mapping Entity Framework

**Cách khắc phục:**
1. Kiểm tra dữ liệu trong database
2. Kiểm tra foreign key relationships
3. Chạy lại migration nếu cần

### 5. Lỗi "Xuất kho không thành công"

**Nguyên nhân:**
- Entity tracking issues với AsNoTracking
- Dữ liệu tồn kho không được refresh trước khi update
- Validation logic không đầy đủ

**Cách khắc phục:**
1. **Đã sửa**: Cập nhật `UpdateInventoryAsync` để tìm entity trong context
2. **Đã sửa**: Thêm refresh dữ liệu trước khi thực hiện thao tác
3. **Đã sửa**: Cải thiện validation logic cho xuất kho
4. Kiểm tra logs trong Output window để debug

**Chi tiết sửa lỗi:**
- Trong `InventoryRepository.UpdateInventoryAsync`: Tìm entity trong context thay vì update trực tiếp
- Trong `InventoryAddEditForm`: Refresh dữ liệu tồn kho trước khi thực hiện thao tác
- Thêm validation: Kiểm tra số lượng âm, hết hàng, v.v.

## Các cải tiến đã thực hiện

### 1. Error Handling
- Thêm try-catch blocks cho tất cả database operations
- Thông báo lỗi chi tiết và thân thiện với người dùng
- Phân loại lỗi để xử lý phù hợp

### 2. Retry Logic
- Tự động thử lại khi gặp lỗi database
- Tối đa 3 lần thử lại với delay tăng dần
- Sử dụng `DatabaseHelper.ExecuteWithRetryAsync()`

### 3. Connection Testing
- Kiểm tra kết nối database trước khi thực hiện operations
- Timeout handling cho các operations dài
- Connection status monitoring

### 4. Performance Optimization
- Sử dụng `AsNoTracking()` cho read-only operations
- Async/await pattern để không block UI thread
- Lazy loading cho navigation properties

### 5. UI Improvements
- Loading indicators (cursor, disable controls)
- Progress feedback cho user
- Non-blocking UI updates

### 6. Inventory Operations Fixes
- Sửa lỗi entity tracking trong UpdateInventoryAsync
- Thêm refresh dữ liệu trước khi thực hiện thao tác
- Cải thiện validation logic cho xuất kho
- Thêm logging để debug

## Cách debug

### 1. Kiểm tra logs
- Xem Output window trong Visual Studio
- Kiểm tra Debug.WriteLine trong DatabaseHelper và InventoryAddEditForm

### 2. Test database connection
```csharp
var canConnect = await context.Database.CanConnectAsync();
```

### 3. Kiểm tra dữ liệu
```sql
SELECT COUNT(*) FROM CT_TON;
SELECT TOP 10 * FROM CT_TON;
```

### 4. Kiểm tra relationships
```sql
SELECT * FROM CT_TON ct
JOIN SANPHAM sp ON ct.MaSP = sp.MaSP
JOIN KHOHANG kh ON ct.MaKho = kh.MaKho;
```

### 5. Test inventory operations
```csharp
// Sử dụng TestInventoryOperations.TestInventoryUpdate(context)
await TestInventoryOperations.TestInventoryUpdate(context);
```

## Cấu hình database

### 1. Connection String
Đảm bảo connection string có các tham số:
- `TrustServerCertificate=true` - Bỏ qua SSL certificate validation
- `MultipleActiveResultSets=true` - Cho phép multiple connections
- `Connection Timeout=30` - Timeout cho connection

### 2. Database Permissions
User cần có quyền:
- SELECT trên tất cả tables
- INSERT, UPDATE, DELETE trên CT_TON
- EXECUTE trên stored procedures (nếu có)

### 3. Network Configuration
- Port 1433 (SQL Server) phải được mở
- Firewall cho phép kết nối từ client
- DNS resolution cho server name

## Kiểm tra chức năng xuất kho

### 1. Các bước test
1. Mở InventoryManagementForm
2. Chọn một sản phẩm có tồn kho > 0
3. Click "Xuất kho"
4. Nhập số lượng xuất (nhỏ hơn tồn kho hiện tại)
5. Click "Xuất kho"

### 2. Kiểm tra logs
- Xem Output window cho debug messages
- Kiểm tra thông báo lỗi nếu có

### 3. Kiểm tra database
```sql
-- Kiểm tra tồn kho trước và sau khi xuất
SELECT MaSP, MaKho, SoLuongTonKho FROM CT_TON WHERE MaSP = 'SP001' AND MaKho = 'KHO001';
```

## Liên hệ hỗ trợ

Nếu vẫn gặp vấn đề, vui lòng:
1. Chụp màn hình lỗi
2. Ghi lại thời điểm xảy ra lỗi
3. Mô tả các bước thực hiện trước khi lỗi
4. Kiểm tra logs và error messages
5. Chạy test case từ TestInventoryOperations
