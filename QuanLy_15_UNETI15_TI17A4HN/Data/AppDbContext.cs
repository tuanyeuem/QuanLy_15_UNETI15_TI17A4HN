// Họ và tên: Nguyễn Đình Tuấn
// Mã sinh viên: 23103100235
// Nội dung thực hiện: DbContext, khóa, chỉ mục, quan hệ và ràng buộc dữ liệu khách sạn.
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
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<TaiKhoan>().HasIndex(x => x.TenDangNhap).IsUnique();
        modelBuilder.Entity<LoaiPhong>().HasIndex(x => x.TenLoai).IsUnique();
        modelBuilder.Entity<Phong>().HasIndex(x => x.SoPhong).IsUnique();
        modelBuilder.Entity<DichVu>().HasIndex(x => x.TenDichVu).IsUnique();
        modelBuilder.Entity<KhachHang>().HasIndex(x => x.MaTaiKhoan).IsUnique().HasFilter("[MaTaiKhoan] IS NOT NULL");
        modelBuilder.Entity<KhachHang>().HasIndex(x => x.CCCD).IsUnique().HasFilter("[CCCD] IS NOT NULL");
        modelBuilder.Entity<ChiTietDatPhong>().HasIndex(x => new { x.MaDatPhong, x.MaPhong }).IsUnique();
        modelBuilder.Entity<DatPhong>().HasIndex(x => new { x.TrangThai, x.NgayNhan });
        modelBuilder.Entity<DatPhong>().HasIndex(x => new { x.MaKhachHang, x.NgayDat });
        modelBuilder.Entity<ChiTietDatPhong>().HasIndex(x => new { x.MaPhong, x.NgayNhan, x.NgayTra });

        modelBuilder.Entity<KhachHang>()
            .HasOne(x => x.TaiKhoan).WithOne(x => x.KhachHang)
            .HasForeignKey<KhachHang>(x => x.MaTaiKhoan).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Phong>()
            .HasOne(x => x.LoaiPhong).WithMany(x => x.Phongs)
            .HasForeignKey(x => x.MaLoaiPhong).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<DatPhong>()
            .HasOne(x => x.KhachHang).WithMany(x => x.DatPhongs)
            .HasForeignKey(x => x.MaKhachHang).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<DatPhong>()
            .HasOne(x => x.NhanVienTiepNhan).WithMany()
            .HasForeignKey(x => x.MaNhanVienTiepNhan).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ChiTietDatPhong>()
            .HasOne(x => x.DatPhong).WithMany(x => x.ChiTietDatPhongs)
            .HasForeignKey(x => x.MaDatPhong).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ChiTietDatPhong>()
            .HasOne(x => x.Phong).WithMany(x => x.ChiTietDatPhongs)
            .HasForeignKey(x => x.MaPhong).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<SuDungDichVu>()
            .HasOne(x => x.ChiTietDatPhong).WithMany(x => x.SuDungDichVus)
            .HasForeignKey(x => x.MaChiTiet).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<SuDungDichVu>()
            .HasOne(x => x.DichVu).WithMany(x => x.SuDungDichVus)
            .HasForeignKey(x => x.MaDichVu).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Phong>(e => e.ToTable(t => {
            t.HasCheckConstraint("CK_Phong_Tang", "[Tang] > 0");
            t.HasCheckConstraint("CK_Phong_DonGia", "[DonGia] >= 0");
            t.HasCheckConstraint("CK_Phong_TrangThai", "[TrangThai] IN ('Trong','DaDat','DangSuDung','BaoTri','NgungKinhDoanh')");
        }));
        modelBuilder.Entity<DatPhong>(e => e.ToTable(t => {
            t.HasCheckConstraint("CK_DatPhong_Ngay", "[NgayTraDuKien] > [NgayNhan]");
            t.HasCheckConstraint("CK_DatPhong_SoNguoi", "[SoNguoi] > 0");
            t.HasCheckConstraint("CK_DatPhong_TienCoc", "[TienCoc] >= 0");
            t.HasCheckConstraint("CK_DatPhong_TrangThai", "[TrangThai] IN ('ChoXuLy','DangXuLy','HoanThanh','Huy','TuChoi')");
        }));
        modelBuilder.Entity<ChiTietDatPhong>(e => e.ToTable(t => {
            t.HasCheckConstraint("CK_ChiTietDatPhong_Ngay", "[NgayTra] > [NgayNhan]");
            t.HasCheckConstraint("CK_ChiTietDatPhong_Tien", "[DonGia] >= 0 AND [SoNgay] > 0 AND [ThanhTien] >= 0");
            t.HasCheckConstraint("CK_ChiTietDatPhong_TrangThai", "[TrangThai] IN ('DaDat','DangSuDung','DaTraPhong','Huy')");
        }));
        modelBuilder.Entity<DichVu>().ToTable(t => t.HasCheckConstraint("CK_DichVu_DonGia", "[DonGia] >= 0"));
        modelBuilder.Entity<SuDungDichVu>(e => e.ToTable(t => {
            t.HasCheckConstraint("CK_SuDungDichVu_SoLuong", "[SoLuong] > 0");
            t.HasCheckConstraint("CK_SuDungDichVu_Tien", "[DonGia] >= 0 AND [ThanhTien] >= 0");
        }));
    }
}
