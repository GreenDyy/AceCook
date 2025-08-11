using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AceCook.Models;

[Table("PHIEUNHAPKHO")]
public partial class Phieunhapkho
{
    [Key]
    [Column("MaPNK")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaPnk { get; set; } = null!;

    public DateOnly? NgayNhap { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? MaKho { get; set; }

    [InverseProperty("MaPnkNavigation")]
    public virtual ICollection<ChitietPn> ChitietPns { get; set; } = new List<ChitietPn>();

    [ForeignKey("MaKho")]
    [InverseProperty("Phieunhapkhos")]
    public virtual Khohang? MaKhoNavigation { get; set; }
}
