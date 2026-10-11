using Microsoft.EntityFrameworkCore;
using QuanLy_15_UNETI15_TI17A4HN.Models;

namespace QuanLy_15_UNETI15_TI17A4HN.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TaiKhoan> TaiKhoans => Set<TaiKhoan>();
    public DbSet<LoaiPhong> LoaiPhongs => Set<LoaiPhong>();
    public DbSet<Phong> Phongs => Set<Phong>();

    // Các bạn khác thêm: KhachHang, DatPhong, ChiTietDatPhong, DichVu

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaiKhoan>()
            .HasIndex(t => t.TenDangNhap)
            .IsUnique();

        modelBuilder.Entity<LoaiPhong>()
            .HasIndex(l => l.TenLoai)
            .IsUnique();

        modelBuilder.Entity<Phong>()
            .HasIndex(p => p.SoPhong)
            .IsUnique();

        modelBuilder.Entity<Phong>()
            .HasIndex(p => p.MaLoaiPhong);

        modelBuilder.Entity<Phong>()
            .HasKey(p => p.MaPhong);

        modelBuilder.Entity<LoaiPhong>()
            .HasKey(l => l.MaLoaiPhong);

        modelBuilder.Entity<Phong>()
            .HasOne(p => p.LoaiPhong)
            .WithMany()
            .HasForeignKey(p => p.MaLoaiPhong)
            .HasPrincipalKey(l => l.MaLoaiPhong)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Phong>()
            .Property(p => p.DonGia)
            .HasPrecision(18, 2);
    }
}