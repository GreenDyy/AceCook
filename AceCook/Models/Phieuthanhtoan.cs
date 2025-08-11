using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AceCook.Models;

[Table("PHIEUTHANHTOAN")]
public partial class Phieuthanhtoan
{
    [Key]
    [Column("MaPhieuTT")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaPhieuTt { get; set; } = null!;

    [Column("NgayTT")]
    public DateOnly? NgayTt { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? SoTien { get; set; }

    [Column("HinhThucTT")]
    [StringLength(30)]
    public string? HinhThucTt { get; set; }

    [StringLength(20)]
    public string? TrangThai { get; set; }

    [Column("MaHDB")]
    [StringLength(10)]
    [Unicode(false)]
    public string? MaHdb { get; set; }

    [Column("MaNV")]
    [StringLength(10)]
    [Unicode(false)]
    public string? MaNv { get; set; }

    [ForeignKey("MaHdb")]
    [InverseProperty("Phieuthanhtoans")]
    public virtual Hoadonban? MaHdbNavigation { get; set; }

    [ForeignKey("MaNv")]
    [InverseProperty("Phieuthanhtoans")]
    public virtual Nhanvien? MaNvNavigation { get; set; }
}
