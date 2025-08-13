using System;
using System.Collections.Generic;

namespace AceCook.Models.Reports
{
    public class RevenueReport : ReportBase
    {
        public decimal TotalRevenue { get; set; }
        public List<RevenueReportDetail> Details { get; set; }
    }

    public class RevenueReportDetail
    {
        public DateTime Date { get; set; }
        public string OrderId { get; set; }
        public string CustomerName { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
    }
}
