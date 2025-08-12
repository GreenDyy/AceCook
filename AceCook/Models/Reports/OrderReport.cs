using System;
using System.Collections.Generic;

namespace AceCook.Models.Reports
{
    public class OrderReport : ReportBase
    {
        public int TotalOrders { get; set; }
        public decimal TotalValue { get; set; }
        public List<OrderReportDetail> Details { get; set; }
    }

    public class OrderReportDetail
    {
        public string OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string Status { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
