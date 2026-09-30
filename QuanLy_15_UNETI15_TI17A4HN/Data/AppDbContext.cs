using Microsoft.EntityFrameworkCore;
using QuanLy_15_UNETI15_TI17A4HN.Models;

namespace QuanLy_15_UNETI15_TI17A4HN.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TaiKhoan> TaiKhoans => Set<TaiKhoan>();
    public DbSet<LoaiPhong> LoaiPhongs => Set<LoaiPhong>();
    // Các bạn khác thêm: Phong, KhachHang, DatPhong, ChiTietDatPhong, DichVu

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaiKhoan>()
            .HasIndex(t => t.TenDangNhap).IsUnique();

        modelBuilder.Entity<LoaiPhong>()
            .HasIndex(l => l.TenLoai).IsUnique();
    }
}