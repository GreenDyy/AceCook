# AceCook - Hệ thống quản lý nhà hàng

## Mô tả
AceCook là một ứng dụng WinForm được xây dựng bằng C# và .NET 8, sử dụng Entity Framework Core để quản lý cơ sở dữ liệu SQL Server.

## Tính năng chính
- **Quản lý tài khoản**: Đăng nhập, phân quyền người dùng
- **Quản lý nhân viên**: Thông tin nhân viên, phòng ban
- **Quản lý khách hàng**: Thông tin khách hàng, đơn đặt hàng
- **Quản lý sản phẩm**: Danh sách sản phẩm, nguyên liệu
- **Quản lý kho**: Nhập/xuất kho, tồn kho
- **Quản lý đơn hàng**: Đơn đặt hàng, hóa đơn bán

## Cấu trúc dự án
```
AceCook/
├── Models/           # Entity models
├── Repositories/     # Data access layer
├── Forms/            # Windows Forms
├── Assets/           # Resources, images
└── appsettings.json  # Cấu hình database
```

## Yêu cầu hệ thống
- Windows 10/11
- .NET 8.0 Runtime
- SQL Server 2019+
- Visual Studio 2022 (để phát triển)

## Cài đặt và chạy

### 1. Clone repository
```bash
git clone [repository-url]
cd AceCook
```

### 2. Cấu hình database
Chỉnh sửa file `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "Default": "Server=your-server;Database=your-database;User Id=your-user;Password=your-password;TrustServerCertificate=true"
  }
}
```

### 3. Build và chạy
```bash
dotnet build
dotnet run
```

Hoặc mở solution trong Visual Studio và nhấn F5.

## Đăng nhập
- **Form đăng nhập**: `LoginForm.cs`
- **Xác thực**: Sử dụng `AuthRepository` với Entity Framework
- **Phân quyền**: Dựa trên bảng `Quyentruycap` và `Taikhoan`

## Tính năng LoginForm
- ✅ Giao diện đẹp, responsive
- ✅ Validation input
- ✅ Xử lý lỗi database
- ✅ Hiển thị/ẩn mật khẩu
- ✅ Navigation bằng Enter key
- ✅ Loading state khi đăng nhập
- ✅ Chuyển hướng đến Dashboard sau khi đăng nhập thành công

## Database Schema
- **TAIKHOAN**: Quản lý tài khoản người dùng
- **NHANVIEN**: Thông tin nhân viên
- **QUYENTRUYCAP**: Phân quyền hệ thống
- **KHACHHANG**: Thông tin khách hàng
- **SANPHAM**: Danh sách sản phẩm
- **NGUYENLIEU**: Nguyên liệu sản xuất
- **KHO**: Quản lý kho hàng

## Phát triển
- Sử dụng Repository pattern
- Entity Framework Core với SQL Server
- Windows Forms với .NET 8
- Dependency Injection với Microsoft.Extensions.Configuration

## Tác giả
AceCook Development Team

## License
[Thêm thông tin license nếu cần]
