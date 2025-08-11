using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AceCook.Models;

[Table("PHIEUXUATKHO")]
public partial class Phieuxuatkho
{
    [Key]
    [Column("MaPXK")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaPxk { get; set; } = null!;

    public DateOnly? NgayXuat { get; set; }

    [Column("MaHDB")]
    [StringLength(10)]
    [Unicode(false)]
    public string? MaHdb { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? MaKho { get; set; }

    [Column("TrangThaiPXK")]
    [StringLength(20)]
    public string? TrangThaiPxk { get; set; }

    [ForeignKey("MaHdb")]
    [InverseProperty("Phieuxuatkhos")]
    public virtual Hoadonban? MaHdbNavigation { get; set; }

    [ForeignKey("MaKho")]
    [InverseProperty("Phieuxuatkhos")]
    public virtual Khohang? MaKhoNavigation { get; set; }
}
