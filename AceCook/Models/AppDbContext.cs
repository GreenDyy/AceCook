using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AceCook.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChitietPn> ChitietPns { get; set; }

    public virtual DbSet<CtDh> CtDhs { get; set; }

    public virtual DbSet<CtTon> CtTons { get; set; }

    public virtual DbSet<DinhMuc> DinhMucs { get; set; }

    public virtual DbSet<Dondathang> Dondathangs { get; set; }

    public virtual DbSet<Hoadonban> Hoadonbans { get; set; }

    public virtual DbSet<Khachhang> Khachhangs { get; set; }

    public virtual DbSet<Khohang> Khohangs { get; set; }

    public virtual DbSet<Nguyenlieu> Nguyenlieus { get; set; }

    public virtual DbSet<Nhacungcap> Nhacungcaps { get; set; }

    public virtual DbSet<Nhanvien> Nhanviens { get; set; }

    public virtual DbSet<Phieunhapkho> Phieunhapkhos { get; set; }

    public virtual DbSet<Phieuthanhtoan> Phieuthanhtoans { get; set; }

    public virtual DbSet<Phieuxuatkho> Phieuxuatkhos { get; set; }

    public virtual DbSet<Phongban> Phongbans { get; set; }

    public virtual DbSet<Quyentruycap> Quyentruycaps { get; set; }

    public virtual DbSet<Sanpham> Sanphams { get; set; }

    public virtual DbSet<Taikhoan> Taikhoans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Chỉ cấu hình nếu chưa được cấu hình từ bên ngoài
        if (!optionsBuilder.IsConfigured)
        {
            // Sử dụng connection string từ appsettings.json thay vì hardcode
            optionsBuilder.UseSqlServer("Server=14.161.21.15,1433;Database=QLBH_ACECOOK;User Id=sa;Password=Ezin@123;TrustServerCertificate=true;MultipleActiveResultSets=true");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChitietPn>(entity =>
        {
            entity.HasKey(e => new { e.MaPnk, e.MaNl }).HasName("pk_CHITIET_PN");

            entity.Property(e => e.MaPnk).IsFixedLength();
            entity.Property(e => e.MaNl).IsFixedLength();

            entity.HasOne(d => d.MaNlNavigation).WithMany(p => p.ChitietPns)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_NGUYENLIEU_CTPN");

            entity.HasOne(d => d.MaPnkNavigation).WithMany(p => p.ChitietPns)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_PHIEUNHAPKHO_CTPN");
        });

        modelBuilder.Entity<CtDh>(entity =>
        {
            entity.HasKey(e => new { e.MaDdh, e.MaSp }).HasName("pk_CT_DH");

            entity.Property(e => e.MaDdh).IsFixedLength();
            entity.Property(e => e.MaSp).IsFixedLength();

            entity.HasOne(d => d.MaDdhNavigation).WithMany(p => p.CtDhs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_DONDATHANG_CT_DH");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.CtDhs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_SANPHAM_CT_DH");
        });

        modelBuilder.Entity<CtTon>(entity =>
        {
            entity.HasKey(e => new { e.MaSp, e.MaKho }).HasName("pk_CT_TON");

            entity.Property(e => e.MaSp).IsFixedLength();
            entity.Property(e => e.MaKho).IsFixedLength();

            entity.HasOne(d => d.MaKhoNavigation).WithMany(p => p.CtTons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_KHOHANG_CTTON");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.CtTons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_SANPHAM_CTTON");
        });

        modelBuilder.Entity<DinhMuc>(entity =>
        {
            entity.HasKey(e => new { e.MaSp, e.MaNl }).HasName("pk_DINH_MUC");

            entity.Property(e => e.MaSp).IsFixedLength();
            entity.Property(e => e.MaNl).IsFixedLength();

            entity.HasOne(d => d.MaNlNavigation).WithMany(p => p.DinhMucs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_NGUYENLIEU_DINH_MUC");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.DinhMucs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_SANPHAM_DINH_MUC");
        });

        modelBuilder.Entity<Dondathang>(entity =>
        {
            entity.HasKey(e => e.MaDdh).HasName("pk_DONDATHANG");

            entity.Property(e => e.MaDdh).IsFixedLength();
            entity.Property(e => e.MaKh).IsFixedLength();
            entity.Property(e => e.MaNv).IsFixedLength();

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.Dondathangs).HasConstraintName("fk_KHACHHANG_DONDATHANG");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.Dondathangs).HasConstraintName("fk_NHANVIEN_DONDATHANG");
        });

        modelBuilder.Entity<Hoadonban>(entity =>
        {
            entity.HasKey(e => e.MaHdb).HasName("pk_HOADONBAN");

            entity.Property(e => e.MaHdb).IsFixedLength();
            entity.Property(e => e.MaNv).IsFixedLength();

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.Hoadonbans).HasConstraintName("fk_NHANVIEN_HOADONBAN");
        });

        modelBuilder.Entity<Khachhang>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("pk_KHACHHANG");

            entity.Property(e => e.MaKh).IsFixedLength();
            entity.Property(e => e.Sdtkh).IsFixedLength();
        });

        modelBuilder.Entity<Khohang>(entity =>
        {
            entity.HasKey(e => e.MaKho).HasName("pk_KHOHANG");

            entity.Property(e => e.MaKho).IsFixedLength();
        });

        modelBuilder.Entity<Nguyenlieu>(entity =>
        {
            entity.HasKey(e => e.MaNl).HasName("pk_NGUYENLIEU");

            entity.Property(e => e.MaNl).IsFixedLength();
            entity.Property(e => e.MaNcc).IsFixedLength();

            entity.HasOne(d => d.MaNccNavigation).WithMany(p => p.Nguyenlieus).HasConstraintName("fk_NHACUNGCAP_NGUYENLIEU");
        });

        modelBuilder.Entity<Nhacungcap>(entity =>
        {
            entity.HasKey(e => e.MaNcc).HasName("pk_NHACUNGCAP");

            entity.Property(e => e.MaNcc).IsFixedLength();
            entity.Property(e => e.Sdtncc).IsFixedLength();
        });

        modelBuilder.Entity<Nhanvien>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("pk_NHANVIEN");

            entity.Property(e => e.MaNv).IsFixedLength();
            entity.Property(e => e.MaPb).IsFixedLength();
            entity.Property(e => e.Sdtnv).IsFixedLength();

            entity.HasOne(d => d.MaPbNavigation).WithMany(p => p.Nhanviens).HasConstraintName("fk_PHONGBAN_NHANVIEN");
        });

        modelBuilder.Entity<Phieunhapkho>(entity =>
        {
            entity.HasKey(e => e.MaPnk).HasName("pk_PHIEUNHAPKHO");

            entity.Property(e => e.MaPnk).IsFixedLength();
            entity.Property(e => e.MaKho).IsFixedLength();

            entity.HasOne(d => d.MaKhoNavigation).WithMany(p => p.Phieunhapkhos).HasConstraintName("fk_KHOHANG_PHIEUNHAPKHO");
        });

        modelBuilder.Entity<Phieuthanhtoan>(entity =>
        {
            entity.HasKey(e => e.MaPhieuTt).HasName("pk_PHIEUTHANHTOAN");

            entity.Property(e => e.MaPhieuTt).IsFixedLength();
            entity.Property(e => e.MaHdb).IsFixedLength();
            entity.Property(e => e.MaNv).IsFixedLength();

            entity.HasOne(d => d.MaHdbNavigation).WithMany(p => p.Phieuthanhtoans).HasConstraintName("fk_HOADONBAN_PTT");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.Phieuthanhtoans).HasConstraintName("fk_NHANVIEN_PTT");
        });

        modelBuilder.Entity<Phieuxuatkho>(entity =>
        {
            entity.HasKey(e => e.MaPxk).HasName("pk_PHIEUXUATKHO");

            entity.Property(e => e.MaPxk).IsFixedLength();
            entity.Property(e => e.MaHdb).IsFixedLength();
            entity.Property(e => e.MaKho).IsFixedLength();

            entity.HasOne(d => d.MaHdbNavigation).WithMany(p => p.Phieuxuatkhos).HasConstraintName("fk_HOADONBAN_PXK");

            entity.HasOne(d => d.MaKhoNavigation).WithMany(p => p.Phieuxuatkhos).HasConstraintName("fk_KHOHANG_PXK");
        });

        modelBuilder.Entity<Phongban>(entity =>
        {
            entity.HasKey(e => e.MaPb).HasName("pk_PHONGBAN");

            entity.Property(e => e.MaPb).IsFixedLength();
        });

        modelBuilder.Entity<Quyentruycap>(entity =>
        {
            entity.HasKey(e => e.MaPq).HasName("pk_QUYENTRUYCAP");

            entity.Property(e => e.MaPq).IsFixedLength();
        });

        modelBuilder.Entity<Sanpham>(entity =>
        {
            entity.HasKey(e => e.MaSp).HasName("pk_SANPHAM");

            entity.Property(e => e.MaSp).IsFixedLength();
        });

        modelBuilder.Entity<Taikhoan>(entity =>
        {
            entity.HasKey(e => e.MaTk).HasName("pk_TAIKHOAN");

            entity.Property(e => e.MaTk).IsFixedLength();
            entity.Property(e => e.MaNv).IsFixedLength();
            entity.Property(e => e.MaPq).IsFixedLength();

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.Taikhoans).HasConstraintName("fk_NHANVIEN_TAIKHOAN");

            entity.HasOne(d => d.MaPqNavigation).WithMany(p => p.Taikhoans).HasConstraintName("fk_QUYENTRUYCAP_TAIKHOAN");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
