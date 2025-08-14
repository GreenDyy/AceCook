using System;

namespace AceCook.Models.Reports
{
    public class ReportBase
    {
        public ReportBase()
        {
            CreatedBy = "System";
        }
        
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
