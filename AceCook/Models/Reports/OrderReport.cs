using System;
using System.Collections.Generic;

namespace AceCook.Models.Reports
{
    public class OrderReport : ReportBase
    {
        public OrderReport()
        {
            Details = new List<OrderReportDetail>();
        }
        
        public int TotalOrders { get; set; }
        public decimal TotalValue { get; set; }
        public List<OrderReportDetail> Details { get; set; }
    }

    public class OrderReportDetail
    {
        public string OrderId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
