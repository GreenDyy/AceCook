using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AceCook.Models;

[PrimaryKey("MaPnk", "MaNl")]
[Table("CHITIET_PN")]
public partial class ChitietPn
{
    [Key]
    [Column("MaPNK")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaPnk { get; set; } = null!;

    [Key]
    [Column("MaNL")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaNl { get; set; } = null!;

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? SoLuongNhap { get; set; }

    [ForeignKey("MaNl")]
    [InverseProperty("ChitietPns")]
    public virtual Nguyenlieu MaNlNavigation { get; set; } = null!;

    [ForeignKey("MaPnk")]
    [InverseProperty("ChitietPns")]
    public virtual Phieunhapkho MaPnkNavigation { get; set; } = null!;
}
