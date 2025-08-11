using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AceCook.Models;

[Table("SANPHAM")]
public partial class Sanpham
{
    [Key]
    [Column("MaSP")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaSp { get; set; } = null!;

    [Column("TenSP")]
    [StringLength(50)]
    public string? TenSp { get; set; }

    [StringLength(100)]
    public string? MoTa { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? Gia { get; set; }

    [Column("DVTSP")]
    [StringLength(20)]
    public string? Dvtsp { get; set; }

    [StringLength(20)]
    public string? Loai { get; set; }

    [InverseProperty("MaSpNavigation")]
    public virtual ICollection<CtDh> CtDhs { get; set; } = new List<CtDh>();

    [InverseProperty("MaSpNavigation")]
    public virtual ICollection<CtTon> CtTons { get; set; } = new List<CtTon>();

    [InverseProperty("MaSpNavigation")]
    public virtual ICollection<DinhMuc> DinhMucs { get; set; } = new List<DinhMuc>();
}
