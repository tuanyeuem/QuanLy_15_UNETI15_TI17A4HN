
using Microsoft.EntityFrameworkCore;
using QuanLy_15_UNETI15_TI17A4HN.Models;

namespace QuanLy_15_UNETI15_TI17A4HN.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TaiKhoan> TaiKhoans => Set<TaiKhoan>();
    public DbSet<LoaiPhong> LoaiPhongs => Set<LoaiPhong>();
    // Các bạn khác thêm: Phong, KhachHang, DatPhong, ChiTietDatPhong, DichVu
    public DbSet<Phong>      Phongs      => Set<Phong>();
    public DbSet<KhachHang>  KhachHangs  => Set<KhachHang>();
    public DbSet<DatPhong>   DatPhongs   => Set<DatPhong>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaiKhoan>()
            .HasIndex(t => t.TenDangNhap).IsUnique();

        modelBuilder.Entity<LoaiPhong>()
            .HasIndex(l => l.TenLoai).IsUnique();
            // Phong: SoPhong không trùng
        modelBuilder.Entity<Phong>()
            .HasIndex(p => p.SoPhong).IsUnique();

        // Phong → LoaiPhong
        modelBuilder.Entity<Phong>()
            .HasOne(p => p.LoaiPhong)
            .WithMany()
            .HasForeignKey(p => p.MaLoaiPhong)
            .OnDelete(DeleteBehavior.Restrict);

        // KhachHang → TaiKhoan (nullable)
        modelBuilder.Entity<KhachHang>()
            .HasOne(k => k.TaiKhoan)
            .WithMany()
            .HasForeignKey(k => k.MaTaiKhoan)
            .OnDelete(DeleteBehavior.SetNull);

        // DatPhong → KhachHang
        modelBuilder.Entity<DatPhong>()
            .HasOne(d => d.KhachHang)
            .WithMany(k => k.DatPhongs)
            .HasForeignKey(d => d.MaKhachHang)
            .OnDelete(DeleteBehavior.Restrict);

        // DatPhong → Phong
        modelBuilder.Entity<DatPhong>()
            .HasOne(d => d.Phong)
            .WithMany(p => p.DatPhongs)
            .HasForeignKey(d => d.MaPhong)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
