using System;

namespace AceCook.Models.Reports
{
    public class InventoryReportItem
    {
        public string MaSP { get; set; }
        public string TenSanPham { get; set; }
        public string Loai { get; set; }
        public decimal Gia { get; set; }
        public int TonKho { get; set; }
        public decimal GiaTri { get; set; }
        public string ChiTietTheoKho { get; set; }
    }
}
