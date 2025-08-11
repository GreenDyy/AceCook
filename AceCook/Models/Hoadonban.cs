using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AceCook.Models;

[Table("HOADONBAN")]
public partial class Hoadonban
{
    [Key]
    [Column("MaHDB")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaHdb { get; set; } = null!;

    public DateOnly? NgayLap { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? TongTien { get; set; }

    [Column("VAT", TypeName = "decimal(18, 0)")]
    public decimal? Vat { get; set; }

    [StringLength(30)]
    public string? TrangThaiThanhToan { get; set; }

    [Column("MaNV")]
    [StringLength(10)]
    [Unicode(false)]
    public string? MaNv { get; set; }

    [ForeignKey("MaNv")]
    [InverseProperty("Hoadonbans")]
    public virtual Nhanvien? MaNvNavigation { get; set; }

    [InverseProperty("MaHdbNavigation")]
    public virtual ICollection<Phieuthanhtoan> Phieuthanhtoans { get; set; } = new List<Phieuthanhtoan>();

    [InverseProperty("MaHdbNavigation")]
    public virtual ICollection<Phieuxuatkho> Phieuxuatkhos { get; set; } = new List<Phieuxuatkho>();
}
