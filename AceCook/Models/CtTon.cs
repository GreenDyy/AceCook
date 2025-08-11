using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AceCook.Models;

[PrimaryKey("MaSp", "MaKho")]
[Table("CT_TON")]
public partial class CtTon
{
    [Key]
    [Column("MaSP")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaSp { get; set; } = null!;

    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string MaKho { get; set; } = null!;

    public int? SoLuongTonKho { get; set; }

    [ForeignKey("MaKho")]
    [InverseProperty("CtTons")]
    public virtual Khohang MaKhoNavigation { get; set; } = null!;

    [ForeignKey("MaSp")]
    [InverseProperty("CtTons")]
    public virtual Sanpham MaSpNavigation { get; set; } = null!;
}
