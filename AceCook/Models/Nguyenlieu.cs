using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AceCook.Models;

[Table("NGUYENLIEU")]
public partial class Nguyenlieu
{
    [Key]
    [Column("MaNL")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaNl { get; set; } = null!;

    [Column("TenNL")]
    [StringLength(50)]
    public string? TenNl { get; set; }

    [Column("DVTNL")]
    [StringLength(20)]
    public string? Dvtnl { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? TonKho { get; set; }

    [Column("MaNCC")]
    [StringLength(10)]
    [Unicode(false)]
    public string? MaNcc { get; set; }

    [InverseProperty("MaNlNavigation")]
    public virtual ICollection<ChitietPn> ChitietPns { get; set; } = new List<ChitietPn>();

    [InverseProperty("MaNlNavigation")]
    public virtual ICollection<DinhMuc> DinhMucs { get; set; } = new List<DinhMuc>();

    [ForeignKey("MaNcc")]
    [InverseProperty("Nguyenlieus")]
    public virtual Nhacungcap? MaNccNavigation { get; set; }
}
