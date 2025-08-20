using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QLTienLuong.Models;

public partial class QltienLuongContext : DbContext
{
    public QltienLuongContext()
    {
    }

    public QltienLuongContext(DbContextOptions<QltienLuongContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DanhMucPhuCap> DanhMucPhuCaps { get; set; }

    public virtual DbSet<HocVien> HocViens { get; set; }

    public virtual DbSet<HocVienTangLuong> HocVienTangLuongs { get; set; }

    public virtual DbSet<HuongPhuCap> HuongPhuCaps { get; set; }

    public virtual DbSet<KhenThuongKyLuat> KhenThuongKyLuats { get; set; }

    public virtual DbSet<PhuCapHocVien> PhuCapHocViens { get; set; }

    public virtual DbSet<QuocTich> QuocTiches { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TangLuong> TangLuongs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // This will be used only if the context is not configured through DI
            // The connection string should be configured in Program.cs
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DanhMucPhuCap>(entity =>
        {
            entity.HasKey(e => e.MaPhuCap).HasName("PK__DanhMucP__F97D9738843B4311");

            entity.ToTable("DanhMucPhuCap");

            entity.Property(e => e.MaPhuCap)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.MucPhuCapCoBan).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TenPhuCap).HasMaxLength(100);
        });

        modelBuilder.Entity<HocVien>(entity =>
        {
            entity.HasKey(e => e.MaHocVien).HasName("PK__HocVien__685B0E6AB3DFA5FC");

            entity.ToTable("HocVien");

            entity.Property(e => e.MaHocVien)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.DonVi).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.Khoa).HasMaxLength(50);
            entity.Property(e => e.Lop).HasMaxLength(20);
            entity.Property(e => e.MaQuocTich)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Nganh).HasMaxLength(100);

            entity.HasOne(d => d.MaQuocTichNavigation).WithMany(p => p.HocViens)
                .HasForeignKey(d => d.MaQuocTich)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__HocVien__MaQuocT__4BAC3F29");
        });

        modelBuilder.Entity<HocVienTangLuong>(entity =>
        {
            entity.HasKey(e => e.MaHvtl).HasName("PK__HocVienT__11B8F4E0DD42AC94");

            entity.ToTable("HocVienTangLuong");

            entity.Property(e => e.MaHvtl).HasColumnName("MaHVTL");
            entity.Property(e => e.GhiChu).HasMaxLength(100);
            entity.Property(e => e.MaHocVien)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.MaHocVienNavigation).WithMany(p => p.HocVienTangLuongs)
                .HasForeignKey(d => d.MaHocVien)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_HocVienTangLuong_HocVien");

            entity.HasOne(d => d.MaTangLuongNavigation).WithMany(p => p.HocVienTangLuongs)
                .HasForeignKey(d => d.MaTangLuong)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_HocVienTangLuong_TangLuong");
        });

        modelBuilder.Entity<HuongPhuCap>(entity =>
        {
            entity.HasKey(e => e.MaHuongPhuCap).HasName("PK__HuongPhu__4BDB2414F30D8D4E");

            entity.ToTable("HuongPhuCap");

            entity.Property(e => e.ConNhan).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.DoanPhi).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Ky)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.LopHoc).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.MaHocVien)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TongPhuCap).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TrUung)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("TrUUng");

            entity.HasOne(d => d.MaHocVienNavigation).WithMany(p => p.HuongPhuCaps)
                .HasForeignKey(d => d.MaHocVien)
                .HasConstraintName("FK__HuongPhuC__MaHoc__4E88ABD4");
        });

        modelBuilder.Entity<KhenThuongKyLuat>(entity =>
        {
            entity.HasKey(e => e.MaQuyetDinh).HasName("PK__KhenThuo__3F6D3FCB09E72D77");

            entity.ToTable("KhenThuongKyLuat");

            entity.Property(e => e.MaQuyetDinh)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LoaiHinh).HasMaxLength(50);
            entity.Property(e => e.LyDo).HasMaxLength(255);
            entity.Property(e => e.MaHocVien)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SoTien).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.MaHocVienNavigation).WithMany(p => p.KhenThuongKyLuats)
                .HasForeignKey(d => d.MaHocVien)
                .HasConstraintName("FK__KhenThuon__MaHoc__4F7CD00D");
        });

        modelBuilder.Entity<PhuCapHocVien>(entity =>
        {
            entity.HasKey(e => e.MaPhuCapHocVien).HasName("PK__PhuCapHo__AF769F56ECB4CDE4");

            entity.ToTable("PhuCapHocVien");

            entity.Property(e => e.MaHocVien)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MaPhuCap)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.MaHocVienNavigation).WithMany(p => p.PhuCapHocViens)
                .HasForeignKey(d => d.MaHocVien)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__PhuCapHoc__MaHoc__4E88ABD4");

            entity.HasOne(d => d.MaPhuCapNavigation).WithMany(p => p.PhuCapHocViens)
                .HasForeignKey(d => d.MaPhuCap)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__PhuCapHoc__MaPhu__4F7CD00D");
        });

        modelBuilder.Entity<QuocTich>(entity =>
        {
            entity.HasKey(e => e.MaQuocTich).HasName("PK__QuocTich__F7AD5B47CE7A2888");

            entity.ToTable("QuocTich");

            entity.Property(e => e.MaQuocTich)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenQuocTich).HasMaxLength(100);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.MaRole).HasName("PK__Roles__0639A0FD81C1A289");

            entity.HasIndex(e => e.TenRole, "UQ__Roles__37A723F3F3EDAD1C").IsUnique();

            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.TenRole).HasMaxLength(50);
        });

        modelBuilder.Entity<TangLuong>(entity =>
        {
            entity.HasKey(e => e.MaTangLuong).HasName("PK__TangLuon__07079EB9D78F9071");

            entity.ToTable("TangLuong");

            entity.Property(e => e.LyDoTangLuong).HasMaxLength(255);
            entity.Property(e => e.MucLuongTang).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.MaUser).HasName("PK__Users__55DAC4B71569871B");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4777D19AE").IsUnique();

            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.MaHocVien)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.MaHocVienNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.MaHocVien)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Users__MaHocVien__5070F446");

            entity.HasOne(d => d.MaRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.MaRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__MaRole__534D60F1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
