using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AceCook.Models;

[Table("NHACUNGCAP")]
public partial class Nhacungcap
{
    [Key]
    [Column("MaNCC")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaNcc { get; set; } = null!;

    [Column("TenNCC")]
    [StringLength(50)]
    public string? TenNcc { get; set; }

    [Column("SDTNCC")]
    [StringLength(10)]
    [Unicode(false)]
    public string? Sdtncc { get; set; }

    [Column("EmailNCC")]
    [StringLength(50)]
    [Unicode(false)]
    public string? EmailNcc { get; set; }

    [Column("DiaChiNCC")]
    [StringLength(100)]
    public string? DiaChiNcc { get; set; }

    [InverseProperty("MaNccNavigation")]
    public virtual ICollection<Nguyenlieu> Nguyenlieus { get; set; } = new List<Nguyenlieu>();
}
