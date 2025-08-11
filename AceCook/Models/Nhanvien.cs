using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AceCook.Models;

[Table("NHANVIEN")]
public partial class Nhanvien
{
    [Key]
    [Column("MaNV")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaNv { get; set; } = null!;

    [Column("HoTenNV")]
    [StringLength(50)]
    public string? HoTenNv { get; set; }

    [Column("SDTNV")]
    [StringLength(10)]
    [Unicode(false)]
    public string? Sdtnv { get; set; }

    [Column("EmailNV")]
    [StringLength(50)]
    [Unicode(false)]
    public string? EmailNv { get; set; }

    [Column("MaPB")]
    [StringLength(10)]
    [Unicode(false)]
    public string? MaPb { get; set; }

    [InverseProperty("MaNvNavigation")]
    public virtual ICollection<Dondathang> Dondathangs { get; set; } = new List<Dondathang>();

    [InverseProperty("MaNvNavigation")]
    public virtual ICollection<Hoadonban> Hoadonbans { get; set; } = new List<Hoadonban>();

    [ForeignKey("MaPb")]
    [InverseProperty("Nhanviens")]
    public virtual Phongban? MaPbNavigation { get; set; }

    [InverseProperty("MaNvNavigation")]
    public virtual ICollection<Phieuthanhtoan> Phieuthanhtoans { get; set; } = new List<Phieuthanhtoan>();

    [InverseProperty("MaNvNavigation")]
    public virtual ICollection<Taikhoan> Taikhoans { get; set; } = new List<Taikhoan>();
}
