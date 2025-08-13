using System;

namespace AceCook.Helpers
{
    /// <summary>
    /// Helper class để xử lý DateOnly và DateTime một cách an toàn với Entity Framework
    /// </summary>
    public static class DateHelper
    {
        /// <summary>
        /// Chuyển đổi DateTime sang DateOnly một cách an toàn
        /// </summary>
        /// <param name="dateTime">DateTime cần chuyển đổi</param>
        /// <returns>DateOnly tương ứng</returns>
        public static DateOnly ToDateOnly(this DateTime dateTime)
        {
            return DateOnly.FromDateTime(dateTime);
        }

        /// <summary>
        /// Chuyển đổi DateOnly sang DateTime một cách an toàn
        /// </summary>
        /// <param name="dateOnly">DateOnly cần chuyển đổi</param>
        /// <returns>DateTime tương ứng (00:00:00)</returns>
        public static DateTime ToDateTime(this DateOnly dateOnly)
        {
            return dateOnly.ToDateTime(TimeOnly.MinValue);
        }

        /// <summary>
        /// Lấy ngày cách đây n ngày
        /// </summary>
        /// <param name="days">Số ngày trước</param>
        /// <returns>DateOnly của ngày đó</returns>
        public static DateOnly GetDaysAgo(int days)
        {
            return DateTime.Today.AddDays(-days).ToDateOnly();
        }

        /// <summary>
        /// Lấy ngày đầu tháng hiện tại
        /// </summary>
        /// <returns>DateOnly của ngày đầu tháng</returns>
        public static DateOnly GetFirstDayOfMonth()
        {
            return new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToDateOnly();
        }

        /// <summary>
        /// Lấy ngày cuối tháng hiện tại
        /// </summary>
        /// <returns>DateOnly của ngày cuối tháng</returns>
        public static DateOnly GetLastDayOfMonth()
        {
            return new DateTime(DateTime.Today.Year, DateTime.Today.Month, 
                DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month)).ToDateOnly();
        }

        /// <summary>
        /// Kiểm tra xem DateOnly có nằm trong khoảng thời gian không
        /// </summary>
        /// <param name="date">DateOnly cần kiểm tra</param>
        /// <param name="startDate">Ngày bắt đầu</param>
        /// <param name="endDate">Ngày kết thúc</param>
        /// <returns>True nếu nằm trong khoảng</returns>
        public static bool IsBetween(this DateOnly date, DateOnly startDate, DateOnly endDate)
        {
            return date >= startDate && date <= endDate;
        }

        /// <summary>
        /// Lấy khoảng thời gian 30 ngày gần nhất
        /// </summary>
        /// <returns>Tuple chứa ngày bắt đầu và kết thúc</returns>
        public static (DateOnly StartDate, DateOnly EndDate) GetLast30Days()
        {
            var endDate = DateTime.Today.ToDateOnly();
            var startDate = DateTime.Today.AddDays(-30).ToDateOnly();
            return (startDate, endDate);
        }

        /// <summary>
        /// Lấy khoảng thời gian tháng hiện tại
        /// </summary>
        /// <returns>Tuple chứa ngày bắt đầu và kết thúc</returns>
        public static (DateOnly StartDate, DateOnly EndDate) GetCurrentMonth()
        {
            var startDate = GetFirstDayOfMonth();
            var endDate = GetLastDayOfMonth();
            return (startDate, endDate);
        }

        /// <summary>
        /// Lấy khoảng thời gian quý hiện tại
        /// </summary>
        /// <returns>Tuple chứa ngày bắt đầu và kết thúc</returns>
        public static (DateOnly StartDate, DateOnly EndDate) GetCurrentQuarter()
        {
            var currentMonth = DateTime.Today.Month;
            var quarter = (currentMonth - 1) / 3 + 1;
            var startMonth = (quarter - 1) * 3 + 1;
            var endMonth = startMonth + 2;

            var startDate = new DateTime(DateTime.Today.Year, startMonth, 1).ToDateOnly();
            var endDate = new DateTime(DateTime.Today.Year, endMonth, 
                DateTime.DaysInMonth(DateTime.Today.Year, endMonth)).ToDateOnly();

            return (startDate, endDate);
        }

        /// <summary>
        /// Lấy khoảng thời gian năm hiện tại
        /// </summary>
        /// <returns>Tuple chứa ngày bắt đầu và kết thúc</returns>
        public static (DateOnly StartDate, DateOnly EndDate) GetCurrentYear()
        {
            var startDate = new DateTime(DateTime.Today.Year, 1, 1).ToDateOnly();
            var endDate = new DateTime(DateTime.Today.Year, 12, 31).ToDateOnly();
            return (startDate, endDate);
        }
    }
}
