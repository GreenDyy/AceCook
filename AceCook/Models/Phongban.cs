using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AceCook.Models;

[Table("PHONGBAN")]
public partial class Phongban
{
    [Key]
    [Column("MaPB")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaPb { get; set; } = null!;

    [Column("TenPB")]
    [StringLength(30)]
    public string? TenPb { get; set; }

    [InverseProperty("MaPbNavigation")]
    public virtual ICollection<Nhanvien> Nhanviens { get; set; } = new List<Nhanvien>();
}
