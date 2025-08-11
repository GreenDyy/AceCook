using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AceCook.Models;

[Table("QUYENTRUYCAP")]
public partial class Quyentruycap
{
    [Key]
    [Column("MaPQ")]
    [StringLength(5)]
    [Unicode(false)]
    public string MaPq { get; set; } = null!;

    [StringLength(30)]
    public string? QuyenTruyCap { get; set; }

    [StringLength(20)]
    public string? ChucNang { get; set; }

    [InverseProperty("MaPqNavigation")]
    public virtual ICollection<Taikhoan> Taikhoans { get; set; } = new List<Taikhoan>();
}
