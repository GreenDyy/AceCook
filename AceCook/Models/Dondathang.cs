using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AceCook.Models;

[Table("DONDATHANG")]
public partial class Dondathang
{
    [Key]
    [Column("MaDDH")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaDdh { get; set; } = null!;

    public DateOnly? NgayDat { get; set; }

    public DateOnly? NgayGiao { get; set; }

    [StringLength(20)]
    public string? TrangThai { get; set; }

    [Column("MaKH")]
    [StringLength(10)]
    [Unicode(false)]
    public string? MaKh { get; set; }

    [Column("MaNV")]
    [StringLength(10)]
    [Unicode(false)]
    public string? MaNv { get; set; }

    [InverseProperty("MaDdhNavigation")]
    public virtual ICollection<CtDh> CtDhs { get; set; } = new List<CtDh>();

    [ForeignKey("MaKh")]
    [InverseProperty("Dondathangs")]
    public virtual Khachhang? MaKhNavigation { get; set; }

    [ForeignKey("MaNv")]
    [InverseProperty("Dondathangs")]
    public virtual Nhanvien? MaNvNavigation { get; set; }
}
