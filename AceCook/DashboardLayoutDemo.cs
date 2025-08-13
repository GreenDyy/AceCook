using System;
using System.Windows.Forms;

namespace AceCook
{
    /// <summary>
    /// Demo class để test layout mới của Dashboard và giải thích cách sửa vấn đề đè nhau
    /// </summary>
    public static class DashboardLayoutDemo
    {
        /// <summary>
        /// Hiển thị thông tin về vấn đề layout và cách sửa
        /// </summary>
        public static void ShowLayoutFixInfo()
        {
            var message = @"VẤN ĐỀ LAYOUT DASHBOARD VÀ CÁCH SỬA:

❌ VẤN ĐỀ TRƯỚC ĐÂY:
- Các controls bị đè nhau vì sử dụng Location cố định
- Summary cards không responsive khi thay đổi kích thước
- Charts bị overlap khi form resize
- DataGridView bị cắt khi không đủ không gian

🔧 CÁCH SỬA ĐÃ ÁP DỤNG:

1. SUMMARY CARDS:
   ✅ Sử dụng FlowLayoutPanel thay vì Location cố định
   ✅ Cards tự động sắp xếp theo chiều ngang
   ✅ Responsive khi thay đổi kích thước form
   ✅ AutoScroll khi cần thiết

2. CHART PANELS:
   ✅ Sử dụng TableLayoutPanel với 2 columns
   ✅ Mỗi chart chiếm 50% chiều rộng
   ✅ Dock = DockStyle.Fill để lấp đầy không gian
   ✅ Margin để tạo khoảng cách giữa các charts

3. DETAIL PANEL:
   ✅ Sử dụng Dock = DockStyle.Bottom cho DataGridView
   ✅ Labels sử dụng AutoSize thay vì Size cố định
   ✅ Padding để tạo khoảng cách đẹp mắt

4. LAYOUT PRINCIPLES:
   ✅ Sử dụng Dock thay vì Location
   ✅ TableLayoutPanel cho layout phức tạp
   ✅ FlowLayoutPanel cho danh sách items
   ✅ Margin và Padding cho spacing

✅ KẾT QUẢ:
- Giao diện responsive và không bị đè nhau
- Layout tự động điều chỉnh theo kích thước form
- Code dễ bảo trì và mở rộng
- UX tốt hơn trên các màn hình khác nhau";

            MessageBox.Show(message, "Sửa vấn đề Layout Dashboard", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Hiển thị hướng dẫn sử dụng layout controls
        /// </summary>
        public static void ShowLayoutControlsGuide()
        {
            var message = @"HƯỚNG DẪN SỬ DỤNG LAYOUT CONTROLS:

📱 FLOWLAYOUTPANEL:
- Sắp xếp controls theo flow (trái sang phải hoặc trên xuống dưới)
- Tự động wrap khi hết không gian
- Tốt cho danh sách items (cards, buttons, etc.)
- AutoScroll khi cần thiết

📊 TABLELAYOUTPANEL:
- Sắp xếp controls theo grid (rows và columns)
- Mỗi cell có thể chứa 1 control
- Hỗ trợ SizeType.Percent để chia tỷ lệ
- Tốt cho layout phức tạp (charts, forms, etc.)

🔧 DOCK PROPERTY:
- Dock = DockStyle.Fill: Lấp đầy toàn bộ parent container
- Dock = DockStyle.Top: Gắn vào phía trên
- Dock = DockStyle.Bottom: Gắn vào phía dưới
- Dock = DockStyle.Left: Gắn vào bên trái
- Dock = DockStyle.Right: Gắn vào bên phải

📏 MARGIN VÀ PADDING:
- Margin: Khoảng cách giữa control và container
- Padding: Khoảng cách giữa border và content
- Giúp tạo spacing đẹp mắt giữa các controls

💡 VÍ DỤ SỬ DỤNG:

// FlowLayoutPanel cho summary cards
var flowPanel = new FlowLayoutPanel
{
    Dock = DockStyle.Fill,
    FlowDirection = FlowDirection.LeftToRight,
    WrapContents = false,
    AutoScroll = true
};

// TableLayoutPanel cho 2 charts cạnh nhau
var tableLayout = new TableLayoutPanel
{
    Dock = DockStyle.Fill,
    ColumnCount = 2,
    RowCount = 1
};
tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

// Dock cho DataGridView
var dataGridView = new DataGridView
{
    Dock = DockStyle.Bottom,
    Height = 300
};";

            MessageBox.Show(message, "Hướng dẫn Layout Controls", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Hiển thị thông tin về responsive design
        /// </summary>
        public static void ShowResponsiveDesignInfo()
        {
            var message = @"RESPONSIVE DESIGN TRONG WINFORMS:

🎯 NGUYÊN TẮC CHÍNH:

1. KHÔNG SỬ DỤNG LOCATION CỐ ĐỊNH:
   ❌ new Point(100, 200) - Không responsive
   ✅ Dock = DockStyle.Fill - Responsive

2. SỬ DỤNG LAYOUT CONTROLS:
   ✅ FlowLayoutPanel: Tự động sắp xếp items
   ✅ TableLayoutPanel: Chia tỷ lệ không gian
   ✅ SplitContainer: Chia màn hình thành các phần

3. SỬ DỤNG DOCK PROPERTY:
   ✅ DockStyle.Fill: Lấp đầy container
   ✅ DockStyle.Top/Bottom: Gắn vào cạnh
   ✅ DockStyle.Left/Right: Gắn vào bên

4. SỬ DỤNG ANCHOR PROPERTY:
   ✅ Anchor = AnchorStyles.All: Stretch theo cả 4 hướng
   ✅ Anchor = AnchorStyles.Top | AnchorStyles.Left: Giữ vị trí tương đối

🚀 LỢI ÍCH RESPONSIVE DESIGN:

- Giao diện đẹp trên mọi kích thước màn hình
- Tự động điều chỉnh khi user resize form
- Không bị đè nhau hoặc cắt controls
- UX tốt hơn trên các thiết bị khác nhau
- Dễ bảo trì và mở rộng code

💻 TEST RESPONSIVE DESIGN:

1. Chạy ứng dụng
2. Thay đổi kích thước cửa sổ
3. Kiểm tra xem controls có bị đè nhau không
4. Kiểm tra xem layout có tự động điều chỉnh không
5. Test trên các độ phân giải màn hình khác nhau";

            MessageBox.Show(message, "Responsive Design trong WinForms", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
