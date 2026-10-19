// Họ và tên: Lê Văn Thanh
// Mã sinh viên: 23103100200
// Nội dung thực hiện: Module 5 - DbContext và các DbSet (ChiTietDatPhong cùng các entity liên quan) dùng cho Dashboard, thống kê, tính tiền.
// Họ và tên: Nguyễn Đình Quang
// Mã sinh viên: 23103100259
// Nội dung thực hiện: Module 3 - Giữ CCCD là duy nhất nhưng cho phép nhiều hồ sơ chưa khai báo CCCD.
using Microsoft.EntityFrameworkCore;
using QuanLy_15_UNETI15_TI17A4HN.Models;

namespace QuanLy_15_UNETI15_TI17A4HN.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TaiKhoan> TaiKhoans => Set<TaiKhoan>();
    public DbSet<LoaiPhong> LoaiPhongs => Set<LoaiPhong>();
    public DbSet<Phong> Phongs => Set<Phong>();
    public DbSet<KhachHang> KhachHangs => Set<KhachHang>();
    public DbSet<DatPhong> DatPhongs => Set<DatPhong>();
    public DbSet<ChiTietDatPhong> ChiTietDatPhongs => Set<ChiTietDatPhong>();
    public DbSet<DichVu> DichVus => Set<DichVu>();
    public DbSet<SuDungDichVu> SuDungDichVus => Set<SuDungDichVu>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaiKhoan>()
            .HasIndex(t => t.TenDangNhap).IsUnique();

        modelBuilder.Entity<LoaiPhong>()
            .HasIndex(l => l.TenLoai).IsUnique();

        modelBuilder.Entity<Phong>(e =>
        {
            e.HasIndex(p => p.SoPhong).IsUnique();
            e.HasOne(p => p.LoaiPhong).WithMany(l => l.Phongs)
                .HasForeignKey(p => p.MaLoaiPhong)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<KhachHang>(e =>
        {
            e.HasIndex(k => k.CCCD).IsUnique().HasFilter("[CCCD] IS NOT NULL");
            e.HasOne(k => k.TaiKhoan).WithMany()
                .HasForeignKey(k => k.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DatPhong>(e =>
        {
            e.HasOne(d => d.KhachHang).WithMany(k => k.DatPhongs)
                .HasForeignKey(d => d.MaKhachHang)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(d => d.Phong).WithMany(p => p.DatPhongs)
                .HasForeignKey(d => d.MaPhong)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Module 5: chi tiết gắn bắt buộc với DatPhong và Phong; không xóa dây chuyền để giữ lịch sử.
        modelBuilder.Entity<ChiTietDatPhong>(e =>
        {
            e.HasOne(c => c.DatPhong).WithMany(d => d.ChiTietDatPhongs)
                .HasForeignKey(c => c.MaDatPhong)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(c => c.Phong).WithMany(p => p.ChiTietDatPhongs)
                .HasForeignKey(c => c.MaPhong)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(c => new { c.MaPhong, c.NgayNhan, c.NgayTra });
            e.ToTable(t => t.HasCheckConstraint("CK_ChiTietDatPhong_GiaTri",
                "[DonGia] >= 0 AND [SoNgay] >= 0 AND [ThanhTien] >= 0 AND [NgayTra] >= [NgayNhan]"));
        });

        modelBuilder.Entity<DichVu>()
            .HasIndex(d => d.TenDichVu).IsUnique();

        modelBuilder.Entity<SuDungDichVu>(e =>
        {
            e.ToTable("SuDungDichVus", t =>
            {
                t.HasCheckConstraint("CK_SuDungDichVu_SoLuong", "[SoLuong] > 0");
                t.HasCheckConstraint("CK_SuDungDichVu_Tien", "[DonGia] >= 0 AND [ThanhTien] >= 0");
            });
            e.HasOne(s => s.ChiTietDatPhong).WithMany(c => c.SuDungDichVus)
                .HasForeignKey(s => s.MaChiTiet)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(s => s.DichVu).WithMany(d => d.SuDungDichVus)
                .HasForeignKey(s => s.MaDichVu)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
