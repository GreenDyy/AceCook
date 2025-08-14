using System;
using System.Collections.Generic;

namespace AceCook.Models.Reports
{
    public class RevenueReport : ReportBase
    {
        public RevenueReport()
        {
            Details = new List<RevenueReportDetail>();
        }
        
        public decimal TotalRevenue { get; set; }
        public List<RevenueReportDetail> Details { get; set; }
    }

    public class RevenueReportDetail
    {
        public DateTime Date { get; set; }
        public string OrderId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
