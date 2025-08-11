using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AceCook.Models;

[Table("TAIKHOAN")]
public partial class Taikhoan
{
    [Key]
    [Column("MaTK")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaTk { get; set; } = null!;

    [StringLength(30)]
    [Unicode(false)]
    public string? TenDangNhap { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? MatKhau { get; set; }

    [Column("TrangThaiTK")]
    [StringLength(15)]
    public string? TrangThaiTk { get; set; }

    [Column("MaNV")]
    [StringLength(10)]
    [Unicode(false)]
    public string? MaNv { get; set; }

    [Column("MaPQ")]
    [StringLength(5)]
    [Unicode(false)]
    public string? MaPq { get; set; }

    [ForeignKey("MaNv")]
    [InverseProperty("Taikhoans")]
    public virtual Nhanvien? MaNvNavigation { get; set; }

    [ForeignKey("MaPq")]
    [InverseProperty("Taikhoans")]
    public virtual Quyentruycap? MaPqNavigation { get; set; }
}
