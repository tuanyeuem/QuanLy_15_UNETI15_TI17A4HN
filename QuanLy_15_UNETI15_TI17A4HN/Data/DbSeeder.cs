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

    

        // =========================
        // DỮ LIỆU LOẠI PHÒNG
        // =========================
        if (!db.LoaiPhongs.Any())
        {
            db.LoaiPhongs.AddRange(
                new LoaiPhong
                {
                    MaLoaiPhong = "LP01",
                    TenLoai = "Standard Đơn",
                    SoNguoiToiDa = 1,
                    DonGiaNgay = 400_000,
                    MoTa = "Phòng 1 giường đơn, phù hợp khách đi một mình."
                },

                new LoaiPhong
                {
                    MaLoaiPhong = "LP02",
                    TenLoai = "Standard Đôi",
                    SoNguoiToiDa = 2,
                    DonGiaNgay = 600_000,
                    MoTa = "Phòng 1 giường đôi hoặc 2 giường đơn."
                },

                new LoaiPhong
                {
                    MaLoaiPhong = "LP03",
                    TenLoai = "Deluxe",
                    SoNguoiToiDa = 3,
                    DonGiaNgay = 900_000,
                    MoTa = "Phòng rộng, có ban công và tầm nhìn đẹp."
                },

                new LoaiPhong
                {
                    MaLoaiPhong = "LP04",
                    TenLoai = "Family",
                    SoNguoiToiDa = 5,
                    DonGiaNgay = 1_400_000,
                    MoTa = "Phòng gia đình, 2 giường lớn."
                },

                new LoaiPhong
                {
                    MaLoaiPhong = "LP05",
                    TenLoai = "Suite",
                    SoNguoiToiDa = 4,
                    DonGiaNgay = 2_000_000,
                    MoTa = "Phòng cao cấp có phòng khách riêng."
                }
            );
        }

        // =========================
        // DỮ LIỆU PHÒNG
        // =========================
        if (!db.Phongs.Any())
        {
            db.Phongs.AddRange(

                // Tầng 1
                new Phong
                {
                    MaPhong = "P101",
                    SoPhong = "101",
                    MaLoaiPhong = "LP01",
                    Tang = 1,
                    HuongPhong = "Đông",
                    TienNghi = "Tivi, điều hòa, wifi, tủ lạnh, máy sấy tóc",
                    DonGia = 400_000,
                    TrangThai = "Trống",
                    GhiChu = "Phòng gần sảnh",
                    NgayBaoTri = null,
                    GhiChuBaoTri = null
                },

                new Phong
                {
                    MaPhong = "P102",
                    SoPhong = "102",
                    MaLoaiPhong = "LP02",
                    Tang = 1,
                    HuongPhong = "Tây",
                    TienNghi = "Tivi, điều hòa, wifi, tủ lạnh, máy sấy tóc",
                    DonGia = 600_000,
                    TrangThai = "Đang sử dụng",
                    GhiChu = "Khách đang lưu trú",
                    NgayBaoTri = null,
                    GhiChuBaoTri = null
                },

                new Phong
                {
                    MaPhong = "P103",
                    SoPhong = "103",
                    MaLoaiPhong = "LP02",
                    Tang = 1,
                    HuongPhong = "Nam",
                    TienNghi = "Tivi, điều hòa, wifi, tủ lạnh",
                    DonGia = 600_000,
                    TrangThai = "Trống",
                    GhiChu = "",
                    NgayBaoTri = null,
                    GhiChuBaoTri = null
                },

                // Tầng 2
                new Phong
                {
                    MaPhong = "P201",
                    SoPhong = "201",
                    MaLoaiPhong = "LP03",
                    Tang = 2,
                    HuongPhong = "Đông",
                    TienNghi = "Tivi, điều hòa, wifi, tủ lạnh, ban công, máy sấy tóc",
                    DonGia = 900_000,
                    TrangThai = "Trống",
                    GhiChu = "Phòng có ban công",
                    NgayBaoTri = null,
                    GhiChuBaoTri = null
                },

                new Phong
                {
                    MaPhong = "P202",
                    SoPhong = "202",
                    MaLoaiPhong = "LP03",
                    Tang = 2,
                    HuongPhong = "Nam",
                    TienNghi = "Tivi, điều hòa, wifi, tủ lạnh, ban công",
                    DonGia = 900_000,
                    TrangThai = "Đang sử dụng",
                    GhiChu = "Khách đang lưu trú",
                    NgayBaoTri = null,
                    GhiChuBaoTri = null
                },

                new Phong
                {
                    MaPhong = "P203",
                    SoPhong = "203",
                    MaLoaiPhong = "LP04",
                    Tang = 2,
                    HuongPhong = "Đông Nam",
                    TienNghi = "Tivi, điều hòa, wifi, tủ lạnh, 2 giường lớn, ban công",
                    DonGia = 1_400_000,
                    TrangThai = "Trống",
                    GhiChu = "Phòng dành cho gia đình",
                    NgayBaoTri = null,
                    GhiChuBaoTri = null
                },

                // Tầng 3
                new Phong
                {
                    MaPhong = "P301",
                    SoPhong = "301",
                    MaLoaiPhong = "LP04",
                    Tang = 3,
                    HuongPhong = "Tây",
                    TienNghi = "Tivi, điều hòa, wifi, tủ lạnh, 2 giường lớn",
                    DonGia = 1_400_000,
                    TrangThai = "Trống",
                    GhiChu = "Phòng gia đình",
                    NgayBaoTri = null,
                    GhiChuBaoTri = null
                },

                new Phong
                {
                    MaPhong = "P302",
                    SoPhong = "302",
                    MaLoaiPhong = "LP05",
                    Tang = 3,
                    HuongPhong = "Đông",
                    TienNghi = "Tivi, điều hòa, wifi, tủ lạnh, phòng khách riêng, ban công",
                    DonGia = 2_000_000,
                    TrangThai = "Trống",
                    GhiChu = "Phòng cao cấp",
                    NgayBaoTri = null,
                    GhiChuBaoTri = null
                },

                new Phong
                {
                    MaPhong = "P303",
                    SoPhong = "303",
                    MaLoaiPhong = "LP05",
                    Tang = 3,
                    HuongPhong = "Nam",
                    TienNghi = "Tivi, điều hòa, wifi, tủ lạnh, phòng khách riêng, ban công",
                    DonGia = 2_000_000,
                    TrangThai = "Bảo trì",
                    GhiChu = "Đang sửa chữa điều hòa",
                    NgayBaoTri = new DateTime(2026, 10, 1),
                    GhiChuBaoTri = "Bảo trì hệ thống điều hòa"
                }
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