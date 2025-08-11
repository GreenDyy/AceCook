using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AceCook.Models;

[PrimaryKey("MaDdh", "MaSp")]
[Table("CT_DH")]
public partial class CtDh
{
    [Key]
    [Column("MaDDH")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaDdh { get; set; } = null!;

    [Key]
    [Column("MaSP")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaSp { get; set; } = null!;

    public int? SoLuong { get; set; }

    public double? DonGia { get; set; }

    [ForeignKey("MaDdh")]
    [InverseProperty("CtDhs")]
    public virtual Dondathang MaDdhNavigation { get; set; } = null!;

    [ForeignKey("MaSp")]
    [InverseProperty("CtDhs")]
    public virtual Sanpham MaSpNavigation { get; set; } = null!;
}
