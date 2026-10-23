// Họ và tên: Lê Văn Thanh
// Mã sinh viên: 23103100200
// Nội dung thực hiện: Module 5 - Dữ liệu mẫu phòng, khách hàng, dịch vụ, đặt phòng và chi tiết đặt phòng
// (đủ trạng thái để kiểm thử Dashboard, thống kê và tính tiền).
using Microsoft.EntityFrameworkCore;
using QuanLy_15_UNETI15_TI17A4HN.Helpers;
using QuanLy_15_UNETI15_TI17A4HN.Models;

namespace QuanLy_15_UNETI15_TI17A4HN.Data;

public static class DuLieuMauGiaoDich
{
    public static void Seed(AppDbContext db)
    {
        SeedDichVu(db);
        SeedPhong(db);
        SeedKhachHang(db);
    }

    private static void SeedDichVu(AppDbContext db)
    {
        if (db.DichVus.Any()) return;
        db.DichVus.AddRange(
            new DichVu { TenDichVu = "Ăn sáng buffet", DonViTinh = "Suất", DonGia = 120_000 },
            new DichVu { TenDichVu = "Giặt ủi", DonViTinh = "Kg", DonGia = 40_000 },
            new DichVu { TenDichVu = "Đưa đón sân bay", DonViTinh = "Lượt", DonGia = 350_000 },
            new DichVu { TenDichVu = "Thuê xe máy", DonViTinh = "Ngày", DonGia = 150_000 },
            new DichVu { TenDichVu = "Spa & Massage", DonViTinh = "Giờ", DonGia = 400_000 });
        db.SaveChanges();
    }

    // 15 phòng: 5 tầng x 3 phòng, 5 loại phòng. Phòng 9 đang bảo trì, các phòng còn lại sẵn sàng.
    private static void SeedPhong(AppDbContext db)
    {
        if (db.Phongs.Any()) return;

        var loais = db.LoaiPhongs.OrderBy(l => l.MaLoaiPhong).ToList();
        if (loais.Count == 0) return;

        string[] huong = { "Đông", "Tây", "Nam", "Bắc" };
        string[] tienNghi =
        {
            "Điều hòa, TV, Wifi",
            "Điều hòa, TV, Wifi, Tủ lạnh mini",
            "Điều hòa, TV, Wifi, Ban công, Bồn tắm"
        };

        for (int i = 0; i < 15; i++)
        {
            var loai = loais[i % loais.Count];
            var tang = i / 3 + 1;
            var phong = new Phong
            {
                SoPhong = $"{tang}0{i % 3 + 1}",
                MaLoaiPhong = loai.MaLoaiPhong,
                Tang = tang,
                HuongPhong = huong[i % huong.Length],
                TienNghi = tienNghi[i % tienNghi.Length],
                DonGia = loai.DonGiaNgay,
                TrangThai = TrangThaiPhong.SanSang
            };

            if (i == 9)
            {
                phong.TrangThai = TrangThaiPhong.BaoTri;
                phong.NgayBaoTri = DateTime.Today.AddDays(-2);
                phong.GhiChuBaoTri = "Sửa điều hòa";
            }
            db.Phongs.Add(phong);
        }
        db.SaveChanges();
    }

    // Chỉ tạo khách hàng liên kết với tài khoản khach1..khach4 để thử "Xem kết quả của tôi".
    private static void SeedKhachHang(AppDbContext db)
    {
        if (db.KhachHangs.Any()) return;

        var taiKhoans = db.TaiKhoans
            .Where(t => t.VaiTro == VaiTroTaiKhoan.KhachHang && t.TrangThai)
            .OrderBy(t => t.TenDangNhap).Take(4).ToList();

        for (int i = 0; i < taiKhoans.Count; i++)
        {
            var tk = taiKhoans[i];
            db.KhachHangs.Add(new KhachHang
            {
                MaTaiKhoan = tk.MaTaiKhoan,
                HoTen = tk.HoTen,
                NgaySinh = new DateTime(1975 + i % 28, 1 + i % 12, 1 + i % 28),
                GioiTinh = i % 2 == 0 ? "Nam" : "Nữ",
                CCCD = $"0012{i + 1:D8}",
                SoDienThoai = $"09{i + 1:D8}",
                Email = tk.Email,
                DiaChi = i % 3 == 0 ? "Hà Nội" : i % 3 == 1 ? "Hải Phòng" : "Đà Nẵng",
                QuocTich = "Việt Nam",
                TrangThai = true
            });
        }
        db.SaveChanges();
    }
}
