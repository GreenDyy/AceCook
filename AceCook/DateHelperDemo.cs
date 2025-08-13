using System;
using System.Windows.Forms;
using AceCook.Helpers;

namespace AceCook
{
    /// <summary>
    /// Demo class để test DateHelper và giải thích cách sửa lỗi DateOnly.ToDateTime
    /// </summary>
    public static class DateHelperDemo
    {
        /// <summary>
        /// Hiển thị thông tin về lỗi DateOnly.ToDateTime và cách sửa
        /// </summary>
        public static void ShowDateOnlyFixInfo()
        {
            var message = @"LỖI DATEONLY.TODATETIME VÀ CÁCH SỬA:

❌ VẤN ĐỀ:
- Lỗi: 'The LINQ expression could not be translated'
- Nguyên nhân: Entity Framework Core không thể dịch DateOnly.ToDateTime() sang SQL
- Dòng lỗi: h.NgayLap.Value.ToDateTime(TimeOnly.MinValue) >= DateTime.Today.AddDays(-30)

🔧 CÁCH SỬA:
1. Thay vì sử dụng DateOnly.ToDateTime():
   ❌ .Where(h => h.NgayLap.Value.ToDateTime(TimeOnly.MinValue) >= DateTime.Today.AddDays(-30))

2. Sử dụng DateOnly.FromDateTime():
   ✅ .Where(h => h.NgayLap.Value >= DateOnly.FromDateTime(DateTime.Today.AddDays(-30)))

3. Hoặc sử dụng DateHelper (Khuyến nghị):
   ✅ var (startDate, endDate) = DateHelper.GetLast30Days();
   ✅ .Where(h => h.NgayLap.Value >= startDate)

📝 LÝ DO:
- DateOnly.ToDateTime() là method của .NET 6+ không được EF Core hỗ trợ
- DateOnly.FromDateTime() được EF Core hỗ trợ và có thể dịch sang SQL
- DateHelper cung cấp các method tiện ích và an toàn

✅ KẾT QUẢ:
- LINQ query sẽ được dịch sang SQL thành công
- Dashboard sẽ load dữ liệu mà không bị lỗi
- Code sạch và dễ bảo trì hơn";

            MessageBox.Show(message, "Sửa lỗi DateOnly.ToDateTime", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Hiển thị hướng dẫn sử dụng DateHelper
        /// </summary>
        public static void ShowDateHelperUsage()
        {
            var message = @"HƯỚNG DẪN SỬ DỤNG DATEHELPER:

📅 CÁC METHOD CHÍNH:

1. Lấy khoảng thời gian:
   var (start, end) = DateHelper.GetLast30Days();
   var (start, end) = DateHelper.GetCurrentMonth();
   var (start, end) = DateHelper.GetCurrentQuarter();
   var (start, end) = DateHelper.GetCurrentYear();

2. Chuyển đổi DateTime ↔ DateOnly:
   var dateOnly = dateTime.ToDateOnly();
   var dateTime = dateOnly.ToDateTime();

3. Lấy ngày cụ thể:
   var daysAgo = DateHelper.GetDaysAgo(7);
   var firstDay = DateHelper.GetFirstDayOfMonth();
   var lastDay = DateHelper.GetLastDayOfMonth();

4. Kiểm tra khoảng thời gian:
   var isInRange = date.IsBetween(startDate, endDate);

💡 VÍ DỤ SỬ DỤNG:

// Trước khi sửa (Gây lỗi):
var query = _context.Hoadonbans
    .Where(h => h.NgayLap.Value.ToDateTime(TimeOnly.MinValue) >= DateTime.Today.AddDays(-30));

// Sau khi sửa (Sử dụng DateHelper):
var (startDate, endDate) = DateHelper.GetLast30Days();
var query = _context.Hoadonbans
    .Where(h => h.NgayLap.Value >= startDate);

// Hoặc sửa trực tiếp:
var thirtyDaysAgo = DateOnly.FromDateTime(DateTime.Today.AddDays(-30));
var query = _context.Hoadonbans
    .Where(h => h.NgayLap.Value >= thirtyDaysAgo);

🚀 LỢI ÍCH:
- Code sạch và dễ đọc
- Tái sử dụng logic thời gian
- Tránh lỗi EF Core translation
- Dễ bảo trì và mở rộng";

            MessageBox.Show(message, "Hướng dẫn sử dụng DateHelper", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Test các method của DateHelper
        /// </summary>
        public static void TestDateHelper()
        {
            try
            {
                var results = new System.Text.StringBuilder();
                results.AppendLine("🧪 TEST DATEHELPER METHODS:");
                results.AppendLine();

                // Test GetLast30Days
                var (start30, end30) = DateHelper.GetLast30Days();
                results.AppendLine($"📅 Last 30 days: {start30:dd/MM/yyyy} - {end30:dd/MM/yyyy}");

                // Test GetCurrentMonth
                var (startMonth, endMonth) = DateHelper.GetCurrentMonth();
                results.AppendLine($"📅 Current month: {startMonth:dd/MM/yyyy} - {endMonth:dd/MM/yyyy}");

                // Test GetCurrentQuarter
                var (startQuarter, endQuarter) = DateHelper.GetCurrentQuarter();
                results.AppendLine($"📅 Current quarter: {startQuarter:dd/MM/yyyy} - {endQuarter:dd/MM/yyyy}");

                // Test GetCurrentYear
                var (startYear, endYear) = DateHelper.GetCurrentYear();
                results.AppendLine($"📅 Current year: {startYear:dd/MM/yyyy} - {endYear:dd/MM/yyyy}");

                // Test conversion methods
                var today = DateTime.Today;
                var todayDateOnly = today.ToDateOnly();
                var todayDateTime = todayDateOnly.ToDateTime();
                results.AppendLine();
                results.AppendLine("🔄 CONVERSION TEST:");
                results.AppendLine($"DateTime.Today: {today:dd/MM/yyyy HH:mm:ss}");
                results.AppendLine($"ToDateOnly(): {todayDateOnly:dd/MM/yyyy}");
                results.AppendLine($"ToDateTime(): {todayDateTime:dd/MM/yyyy HH:mm:ss}");

                // Test IsBetween
                var testDate = DateOnly.FromDateTime(DateTime.Today);
                var isInCurrentMonth = testDate.IsBetween(startMonth, endMonth);
                results.AppendLine();
                results.AppendLine("✅ RANGE TEST:");
                results.AppendLine($"Today is in current month: {isInCurrentMonth}");

                MessageBox.Show(results.ToString(), "DateHelper Test Results", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi test DateHelper: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
