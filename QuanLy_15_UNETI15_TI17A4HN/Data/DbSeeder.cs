// Họ và tên: Nguyễn Đình Quang
// Mã sinh viên: 23103100259
// Nội dung thực hiện: Hợp nhất dữ liệu mẫu đăng nhập, phòng, khách hàng, đặt phòng và chi tiết lưu trú.
using Microsoft.EntityFrameworkCore;
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
                TaoTaiKhoan("khachkhoa", "KhachHang@123", "Tài khoản bị khóa", "khoa@gmail.com", VaiTroTaiKhoan.KhachHang, false));
            db.SaveChanges();
        }

        if (!db.LoaiPhongs.Any())
        {
            db.LoaiPhongs.AddRange(
                new LoaiPhong { TenLoai = "Standard Đơn", SoNguoiToiDa = 1, DonGiaNgay = 400_000, MoTa = "Phòng đơn." },
                new LoaiPhong { TenLoai = "Standard Đôi", SoNguoiToiDa = 2, DonGiaNgay = 600_000, MoTa = "Phòng đôi." },
                new LoaiPhong { TenLoai = "Deluxe", SoNguoiToiDa = 3, DonGiaNgay = 900_000, MoTa = "Phòng Deluxe." },
                new LoaiPhong { TenLoai = "Family", SoNguoiToiDa = 5, DonGiaNgay = 1_400_000, MoTa = "Phòng gia đình." },
                new LoaiPhong { TenLoai = "Suite", SoNguoiToiDa = 4, DonGiaNgay = 2_000_000, MoTa = "Phòng Suite." });
            db.SaveChanges();
        }

        if (!db.Phongs.Any())
        {
            var loaiPhongs = db.LoaiPhongs.OrderBy(x => x.MaLoaiPhong).ToList();
            var rooms = new List<Phong>();
            for (var i = 0; i < 15; i++)
            {
                var loai = loaiPhongs[i % loaiPhongs.Count];
                rooms.Add(new Phong
                {
                    SoPhong = $"{(i / 3 + 1) * 100 + (i % 3 + 1)}",
                    MaLoaiPhong = loai.MaLoaiPhong,
                    Tang = i / 3 + 1,
                    DonGia = loai.DonGiaNgay,
                    HuongPhong = i % 2 == 0 ? "Đông" : "Tây",
                    TienNghi = "Wi-Fi, điều hòa",
                    TrangThai = i == 5 ? TrangThaiPhong.BaoTri :
                                i == 13 ? TrangThaiPhong.KhongKinhDoanh : TrangThaiPhong.TrongRanh
                });
            }
            db.Phongs.AddRange(rooms);
            db.SaveChanges();
        }

        if (!db.KhachHangs.Any())
        {
            var taiKhoanKhach = db.TaiKhoans.Where(t => t.VaiTro == VaiTroTaiKhoan.KhachHang)
                .OrderBy(t => t.TenDangNhap).Take(4).ToList();
            for (var i = 0; i < 30; i++)
            {
                var account = i < taiKhoanKhach.Count ? taiKhoanKhach[i] : null;
                db.KhachHangs.Add(new KhachHang
                {
                    MaTaiKhoan = account?.MaTaiKhoan,
                    HoTen = account?.HoTen ?? $"Khách hàng {i + 1:00}",
                    CCCD = $"079{i + 1:000000000}",
                    SoDienThoai = $"090{i + 1:0000000}",
                    Email = account?.Email ?? $"khach{i + 1:00}@gmail.com",
                    NgaySinh = new DateTime(1980 + i % 20, 1 + i % 12, 1 + i % 27),
                    GioiTinh = i % 2 == 0 ? "Nam" : "Nữ",
                    DiaChi = $"Quận {i % 12 + 1}, TP. Hồ Chí Minh",
                    QuocTich = "Việt Nam",
                    TrangThai = i != 29
                });
            }
            db.SaveChanges();
        }

        if (!db.DichVus.Any())
        {
            db.DichVus.AddRange(
                new DichVu { TenDichVu = "Bữa sáng", DonViTinh = "suất", DonGia = 100_000 },
                new DichVu { TenDichVu = "Giặt ủi", DonViTinh = "kg", DonGia = 50_000 },
                new DichVu { TenDichVu = "Đưa đón sân bay", DonViTinh = "lượt", DonGia = 250_000 },
                new DichVu { TenDichVu = "Nước uống", DonViTinh = "chai", DonGia = 20_000 },
                new DichVu { TenDichVu = "Minibar", DonViTinh = "lần", DonGia = 80_000 });
            db.SaveChanges();
        }

        if (!db.DatPhongs.Any())
        {
            var customerIds = db.KhachHangs.OrderBy(k => k.MaKhachHang).Select(k => k.MaKhachHang).ToList();
            var roomIds = db.Phongs.OrderBy(p => p.MaPhong).Select(p => p.MaPhong).ToList();
            var today = DateTime.Today;
            var bookings = new List<DatPhong>();
            for (var i = 0; i < 50; i++)
            {
                var isCompleted = i >= 30 && i < 40;
                var isCancelled = i >= 40;
                var arrival = isCompleted ? today.AddDays(-20 + i % 5) : today.AddDays(2 + i % 14);
                bookings.Add(new DatPhong
                {
                    MaKhachHang = customerIds[i % customerIds.Count],
                    MaPhong = roomIds[i % roomIds.Count],
                    NgayDat = today.AddDays(-i - 1),
                    NgayNhan = arrival,
                    NgayTraDuKien = arrival.AddDays(2),
                    SoNguoi = 1,
                    TienCoc = isCancelled ? 0 : 200_000,
                    TrangThai = isCancelled ? TrangThaiDatPhong.Huy :
                                isCompleted ? TrangThaiDatPhong.DaTraPhong :
                                i < 15 ? TrangThaiDatPhong.ChoDuyet : TrangThaiDatPhong.DaDuyet,
                    NgayNhanThucTe = isCompleted ? arrival : null,
                    NgayTraThucTe = isCompleted ? arrival.AddDays(2) : null,
                    LyDoHuy = isCancelled ? "Khách hủy vì lý do cá nhân." : null
                });
            }
            db.DatPhongs.AddRange(bookings);
            db.SaveChanges();
        }

        if (!db.ChiTietDatPhongs.Any())
        {
            var bookings = db.DatPhongs.OrderBy(d => d.MaDatPhong).Take(20).ToList();
            foreach (var booking in bookings)
            {
                var nights = Math.Max(1, (booking.NgayTraDuKien.Date - booking.NgayNhan.Date).Days);
                var room = db.Phongs.First(p => p.MaPhong == booking.MaPhong);
                var status = booking.TrangThai == TrangThaiDatPhong.Huy
                    ? TrangThaiChiTietDatPhong.Huy
                    : booking.TrangThai == TrangThaiDatPhong.DaTraPhong
                        ? TrangThaiChiTietDatPhong.DaTraPhong
                        : TrangThaiChiTietDatPhong.DaDat;
                db.ChiTietDatPhongs.Add(new ChiTietDatPhong
                {
                    MaDatPhong = booking.MaDatPhong,
                    MaPhong = booking.MaPhong,
                    NgayNhan = booking.NgayNhan,
                    NgayTra = booking.NgayTraDuKien,
                    DonGia = room.DonGia,
                    SoNgay = nights,
                    ThanhTien = room.DonGia * nights,
                    TrangThai = status
                });
            }
            db.SaveChanges();
        }
    }

    private static TaiKhoan TaoTaiKhoan(
        string tenDangNhap, string matKhau, string hoTen, string email, string vaiTro, bool trangThai = true) => new()
    {
        TenDangNhap = tenDangNhap,
        MatKhau = MatKhauHelper.BamMatKhau(matKhau),
        HoTen = hoTen,
        Email = email,
        VaiTro = vaiTro,
        TrangThai = trangThai
    };
}
