using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AceCook.Models;

[PrimaryKey("MaSp", "MaNl")]
[Table("DINH_MUC")]
public partial class DinhMuc
{
    [Key]
    [Column("MaSP")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaSp { get; set; } = null!;

    [Key]
    [Column("MaNL")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaNl { get; set; } = null!;

    [Column("SoLuongNL", TypeName = "decimal(18, 0)")]
    public decimal? SoLuongNl { get; set; }

    [ForeignKey("MaNl")]
    [InverseProperty("DinhMucs")]
    public virtual Nguyenlieu MaNlNavigation { get; set; } = null!;

    [ForeignKey("MaSp")]
    [InverseProperty("DinhMucs")]
    public virtual Sanpham MaSpNavigation { get; set; } = null!;
}
