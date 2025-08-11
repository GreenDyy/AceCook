using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AceCook.Models;

[Table("KHACHHANG")]
public partial class Khachhang
{
    [Key]
    [Column("MaKH")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaKh { get; set; } = null!;

    [Column("TenKH")]
    [StringLength(50)]
    public string? TenKh { get; set; }

    [Column("LoaiKH")]
    [StringLength(30)]
    public string? LoaiKh { get; set; }

    [Column("SDTKH")]
    [StringLength(10)]
    [Unicode(false)]
    public string? Sdtkh { get; set; }

    [Column("DiaChiKH")]
    [StringLength(100)]
    public string? DiaChiKh { get; set; }

    [Column("EmailKH")]
    [StringLength(50)]
    [Unicode(false)]
    public string? EmailKh { get; set; }

    [InverseProperty("MaKhNavigation")]
    public virtual ICollection<Dondathang> Dondathangs { get; set; } = new List<Dondathang>();
}
