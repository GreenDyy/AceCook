using System;
using System.Windows.Forms;

namespace AceCook
{
    /// <summary>
    /// Demo class để test DashboardForm
    /// Chứa thông tin mẫu và hướng dẫn sử dụng
    /// </summary>
    public static class DashboardDemo
    {
        /// <summary>
        /// Hiển thị hướng dẫn sử dụng DashboardForm
        /// </summary>
        public static void ShowUsageGuide()
        {
            var message = @"HƯỚNG DẪN SỬ DỤNG DASHBOARDFORM

1. GIAO DIỆN:
   - Sidebar bên trái: Menu điều hướng với icons và màu sắc đẹp
   - Header: Tiêu đề hệ thống và các nút điều khiển cửa sổ
   - Main Content: Khu vực hiển thị nội dung chính
   - User Info: Thông tin người dùng đang đăng nhập

2. TÍNH NĂNG:
   - Menu điều hướng với TreeView đẹp mắt
   - Icons emoji cho từng mục menu
   - Màu sắc phân biệt cho từng nhóm chức năng
   - Hover effects cho các nút
   - Responsive layout

3. MENU CHÍNH:
   📊 Dashboard: Tổng quan hệ thống
   💼 Kinh doanh: Quản lý khách hàng, đơn hàng
   🏪 Kho hàng: Quản lý sản phẩm, tồn kho
   🚚 Nhà cung cấp: Quản lý nhà cung cấp
   📈 Báo cáo: Các báo cáo thống kê
   ⚙️ Cài đặt: Cấu hình hệ thống

4. ĐIỀU KHIỂN CỬA SỔ:
   - Nút ─ : Thu nhỏ cửa sổ
   - Nút □/❐ : Phóng to/thu nhỏ
   - Nút × : Đóng ứng dụng (có xác nhận)

5. TÍNH NĂNG ĐẶC BIỆT:
   - Form không có viền (FormBorderStyle.None)
   - Cửa sổ mặc định phóng to toàn màn hình
   - Sidebar cố định bên trái
   - Content area responsive
   - Error handling cho các form con

6. LƯU Ý:
   - Các form con được load vào panelContent
   - Tính năng chưa hoàn thiện sẽ hiển thị 'Coming Soon'
   - Có xử lý lỗi khi load các form con
   - Tự động chọn Dashboard khi khởi động";

            MessageBox.Show(message, "Hướng dẫn sử dụng DashboardForm", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Hiển thị thông tin về cải tiến giao diện
        /// </summary>
        public static void ShowImprovements()
        {
            var message = @"CẢI TIẾN GIAO DIỆN DASHBOARDFORM:

1. THIẾT KẾ MỚI:
   ✅ Sử dụng Windows Forms Designer thay vì code tạo UI
   ✅ Layout responsive và chuyên nghiệp
   ✅ Màu sắc hiện đại và nhất quán
   ✅ Typography rõ ràng với Segoe UI font

2. SIDEBAR:
   ✅ Panel thông tin người dùng đẹp mắt
   ✅ TreeView menu với icons emoji
   ✅ Màu sắc phân biệt cho từng nhóm chức năng
   ✅ Hover effects và selection states

3. HEADER:
   ✅ Tiêu đề hệ thống nổi bật
   ✅ Nút điều khiển cửa sổ đẹp mắt
   ✅ Hover effects cho các nút
   ✅ Layout cân đối và chuyên nghiệp

4. MAIN CONTENT:
   ✅ Panel nội dung responsive
   ✅ Background color nhẹ nhàng
   ✅ Loading states cho các form con
   ✅ Error handling và coming soon messages

5. TÍNH NĂNG MỚI:
   ✅ Custom window controls (minimize, maximize, close)
   ✅ Form state management
   ✅ Menu navigation với TreeView
   ✅ Content loading system
   ✅ Error handling system

6. UX IMPROVEMENTS:
   ✅ Smooth navigation giữa các chức năng
   ✅ Visual feedback cho user actions
   ✅ Consistent color scheme
   ✅ Professional appearance
   ✅ Easy-to-use interface";

            MessageBox.Show(message, "Cải tiến giao diện", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Hiển thị thông tin về cách tùy chỉnh
        /// </summary>
        public static void ShowCustomizationInfo()
        {
            var message = @"TÙY CHỈNH DASHBOARDFORM:

1. MÀU SẮC:
   - Sidebar: Color.FromArgb(44, 62, 80)
   - Header: Color.FromArgb(52, 73, 94)
   - User Info: Color.FromArgb(52, 73, 94)
   - Main Content: Color.FromArgb(248, 249, 250)

2. FONT:
   - System Title: Segoe UI, 16pt, Bold
   - User Name: Segoe UI, 12pt, Bold
   - User Role: Segoe UI, 9pt, Regular
   - Menu Items: Segoe UI, 10pt, Regular

3. KÍCH THƯỚC:
   - Sidebar Width: 280px
   - Header Height: 60px
   - User Info Height: 120px
   - Form Minimum Size: 1000x600

4. LAYOUT:
   - Sidebar: Dock = DockStyle.Left
   - Header: Dock = DockStyle.Top
   - Main Content: Dock = DockStyle.Fill
   - Content Panel: Dock = DockStyle.Fill

5. TÙY CHỈNH MENU:
   - Thêm/sửa menu items trong SetupMenuItems()
   - Thay đổi icons emoji
   - Điều chỉnh màu sắc cho từng nhóm
   - Thêm sub-menu items

6. TÙY CHỈNH CONTENT:
   - Thêm form mới trong LoadContent()
   - Tạo custom content panels
   - Thêm loading states
   - Custom error handling";

            MessageBox.Show(message, "Thông tin tùy chỉnh", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
