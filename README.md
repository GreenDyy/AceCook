# AceCook - Ứng dụng Quản lý Sản phẩm

## Mô tả
AceCook là một ứng dụng Windows Forms được viết bằng C# để quản lý danh sách sản phẩm. Ứng dụng sử dụng Entity Framework Core để kết nối với cơ sở dữ liệu SQL Server.

## Tính năng chính

### 1. Hiển thị danh sách sản phẩm
- Hiển thị tất cả sản phẩm trong DataGridView
- Thông tin hiển thị: Mã SP, Tên SP, Mô tả, Giá, Đơn vị, Loại

### 2. Tìm kiếm và lọc
- Tìm kiếm theo tên sản phẩm, mã sản phẩm hoặc loại
- Lọc theo loại sản phẩm (Mì tô, Mì gói, Mỳ ly, Miến, Bún, Phở, Hủ tiếu)
- Nút "Xóa lọc" để reset về danh sách ban đầu

### 3. Quản lý sản phẩm
- **Thêm sản phẩm mới**: Mở form nhập thông tin sản phẩm
- **Sửa sản phẩm**: Chọn sản phẩm và chỉnh sửa (chưa implement)
- **Xóa sản phẩm**: Xóa sản phẩm với xác nhận
- **Làm mới**: Tải lại danh sách từ database

## Cấu trúc Database

### Bảng SANPHAM
```sql
CREATE TABLE SANPHAM 
(
    MaSP CHAR(10) NOT NULL,
    TenSP NVARCHAR(50),
    MoTa NVARCHAR(100),
    Gia DECIMAL CHECK (Gia >= 0),
    DVTSP NVARCHAR(20),
    Loai NVARCHAR(20),
    CONSTRAINT pk_SANPHAM PRIMARY KEY (MaSP),
    CONSTRAINT chk_SANPHAM_Loai CHECK (Loai IN (N'Mì tô', N'Mì gói', N'Mỳ ly', N'Miến', N'Bún', N'Phở', N'Hủ tiếu'))
);
```

## Cấu trúc Project

```
AceCook/
├── Models/
│   └── SANPHAM.cs              # Model cho bảng SANPHAM
├── Data/
│   └── AppContext.cs           # DbContext cho Entity Framework
├── Repositories/
│   └── ProductRepository.cs    # Repository pattern cho thao tác CRUD
├── Form1.cs                    # Form chính hiển thị danh sách
├── Form1.Designer.cs           # Designer cho Form1
├── AddProductForm.cs           # Form thêm sản phẩm
├── AddProductForm.Designer.cs  # Designer cho AddProductForm
├── appsettings.json            # Cấu hình connection string
└── Program.cs                  # Entry point của ứng dụng
```

## Cài đặt và Chạy

### Yêu cầu hệ thống
- .NET 8.0 hoặc cao hơn
- SQL Server (có thể là LocalDB hoặc SQL Server Express)
- Visual Studio 2022 hoặc VS Code

### Các bước cài đặt

1. **Clone hoặc download project**
2. **Cập nhật connection string** trong file `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "Default": "Server=your_server;Database=your_database;User Id=your_user;Password=your_password;TrustServerCertificate=true;"
     }
   }
   ```

3. **Tạo database và bảng**:
   - Tạo database mới
   - Chạy script SQL để tạo bảng SANPHAM

4. **Build và chạy project**:
   ```bash
   dotnet build
   dotnet run
   ```

## Sử dụng

### Giao diện chính
- **DataGridView**: Hiển thị danh sách sản phẩm
- **Ô tìm kiếm**: Nhập từ khóa để tìm kiếm
- **ComboBox Loại**: Chọn loại sản phẩm để lọc
- **Nút Xóa lọc**: Reset về danh sách ban đầu

### Các nút chức năng
- **Làm mới**: Tải lại dữ liệu từ database
- **Thêm**: Mở form thêm sản phẩm mới
- **Sửa**: Chỉnh sửa sản phẩm đã chọn (chưa implement)
- **Xóa**: Xóa sản phẩm đã chọn với xác nhận

### Form thêm sản phẩm
- Nhập đầy đủ thông tin: Mã SP, Tên SP, Mô tả, Giá, Đơn vị, Loại
- Validation tự động kiểm tra dữ liệu nhập
- Nút Lưu để thêm sản phẩm, Hủy để đóng form

## Lưu ý
- Mã sản phẩm phải là duy nhất (Primary Key)
- Giá sản phẩm phải >= 0
- Loại sản phẩm phải thuộc danh sách cho phép
- Ứng dụng sử dụng async/await để không block UI khi thao tác database

## Phát triển tiếp theo
- Form chỉnh sửa sản phẩm
- Export dữ liệu ra Excel/PDF
- Thêm hình ảnh sản phẩm
- Quản lý nhà cung cấp
- Báo cáo thống kê 

## Scaffold