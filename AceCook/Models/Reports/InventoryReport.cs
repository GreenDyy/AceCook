using System;
using System.Collections.Generic;

namespace AceCook.Models.Reports
{
    public class InventoryReport : ReportBase
    {
        public decimal TotalValue { get; set; }
        public List<InventoryReportDetail> Details { get; set; }
    }

    public class InventoryReportDetail
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int BeginningQuantity { get; set; }
        public int InQuantity { get; set; }
        public int OutQuantity { get; set; }
        public int EndingQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalValue { get; set; }
    }
}
