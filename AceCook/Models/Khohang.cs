using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AceCook.Models;

[Table("KHOHANG")]
public partial class Khohang
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string MaKho { get; set; } = null!;

    [StringLength(50)]
    public string? TenKho { get; set; }

    [StringLength(100)]
    public string? ViTri { get; set; }

    [InverseProperty("MaKhoNavigation")]
    public virtual ICollection<CtTon> CtTons { get; set; } = new List<CtTon>();

    [InverseProperty("MaKhoNavigation")]
    public virtual ICollection<Phieunhapkho> Phieunhapkhos { get; set; } = new List<Phieunhapkho>();

    [InverseProperty("MaKhoNavigation")]
    public virtual ICollection<Phieuxuatkho> Phieuxuatkhos { get; set; } = new List<Phieuxuatkho>();
}
