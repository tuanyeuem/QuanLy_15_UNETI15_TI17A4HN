// Họ và tên: Nguyễn Đình Tuấn
// Mã sinh viên: 23103100235
// Nội dung thực hiện: Dữ liệu mẫu tài khoản (Admin, Nhân viên, Khách hàng) và loại phòng.
using QuanLy_15_UNETI15_TI17A4HN.Helpers;
using QuanLy_15_UNETI15_TI17A4HN.Models;

namespace QuanLy_15_UNETI15_TI17A4HN.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (!db.TaiKhoans.Any())
        {
            db.TaiKhoans.AddRange(
                TaoTaiKhoan("admin1", "Admin@123", "Nguyễn Đình Tuấn", "admin1@khachsan.vn", VaiTroTaiKhoan.Admin),
                TaoTaiKhoan("admin2", "Admin@123", "Trần Quốc Bảo", "admin2@khachsan.vn", VaiTroTaiKhoan.Admin),
                TaoTaiKhoan("nhanvien1", "NhanVien@123", "Lê Thị Hoa", "nv1@khachsan.vn", VaiTroTaiKhoan.NhanVien),
                TaoTaiKhoan("nhanvien2", "NhanVien@123", "Phạm Văn Nam", "nv2@khachsan.vn", VaiTroTaiKhoan.NhanVien),
                TaoTaiKhoan("nhanvien3", "NhanVien@123", "Vũ Thị Lan", "nv3@khachsan.vn", VaiTroTaiKhoan.NhanVien),
                TaoTaiKhoan("khach1", "KhachHang@123", "Hoàng Minh Đức", "khach1@gmail.com", VaiTroTaiKhoan.KhachHang),
                TaoTaiKhoan("khach2", "KhachHang@123", "Đỗ Thu Trang", "khach2@gmail.com", VaiTroTaiKhoan.KhachHang),
                TaoTaiKhoan("khach3", "KhachHang@123", "Bùi Anh Tú", "khach3@gmail.com", VaiTroTaiKhoan.KhachHang),
                TaoTaiKhoan("khach4", "KhachHang@123", "Ngô Mai Linh", "khach4@gmail.com", VaiTroTaiKhoan.KhachHang),
                TaoTaiKhoan("khachkhoa", "KhachHang@123", "Tài Khoản Bị Khóa", "khoa@gmail.com", VaiTroTaiKhoan.KhachHang, false)
            );
        }

        if (!db.LoaiPhongs.Any())
        {
            db.LoaiPhongs.AddRange(
                new LoaiPhong { TenLoai = "Standard Đơn", SoNguoiToiDa = 1, DonGiaNgay = 400_000, MoTa = "Phòng 1 giường đơn, phù hợp khách đi một mình." },
                new LoaiPhong { TenLoai = "Standard Đôi", SoNguoiToiDa = 2, DonGiaNgay = 600_000, MoTa = "Phòng 1 giường đôi hoặc 2 giường đơn." },
                new LoaiPhong { TenLoai = "Deluxe", SoNguoiToiDa = 3, DonGiaNgay = 900_000, MoTa = "Phòng rộng, có ban công và tầm nhìn đẹp." },
                new LoaiPhong { TenLoai = "Family", SoNguoiToiDa = 5, DonGiaNgay = 1_400_000, MoTa = "Phòng gia đình, 2 giường lớn." },
                new LoaiPhong { TenLoai = "Suite", SoNguoiToiDa = 4, DonGiaNgay = 2_000_000, MoTa = "Phòng cao cấp có phòng khách riêng." }
            );
        }

        db.SaveChanges();
    }

    private static TaiKhoan TaoTaiKhoan(string tenDangNhap, string matKhau, string hoTen,
        string email, string vaiTro, bool trangThai = true) => new()
    {
        TenDangNhap = tenDangNhap,
        MatKhau = MatKhauHelper.BamMatKhau(matKhau),
        HoTen = hoTen,
        Email = email,
        VaiTro = vaiTro,
        TrangThai = trangThai
    };
}