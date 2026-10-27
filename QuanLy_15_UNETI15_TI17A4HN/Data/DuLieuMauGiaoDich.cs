// Họ và tên: Lê Văn Thanh
// Mã sinh viên: 23103100200
// Nội dung thực hiện: Module 5 - Dữ liệu mẫu khách hàng, đặt phòng và chi tiết đặt phòng
// (đủ trạng thái, đủ số lượng tối thiểu để kiểm thử Dashboard, thống kê và tính tiền).
using Microsoft.EntityFrameworkCore;
using QuanLy_15_UNETI15_TI17A4HN.Helpers;
using QuanLy_15_UNETI15_TI17A4HN.Models;

namespace QuanLy_15_UNETI15_TI17A4HN.Data;

/// <summary>
/// Bổ sung dữ liệu mẫu một cách idempotent (chạy nhiều lần không nhân đôi dữ liệu).
/// Mục tiêu: 30 khách hàng, 45 đặt phòng, 18 chi tiết đặt phòng.
/// </summary>
public static class DuLieuMauGiaoDich
{
    private const int SoKhachHangMau = 30;
    private const int SoDatPhongMau = 45;
    private const string TienToKhach = "[SEED-KH]";
    private const string TienToDatPhong = "[SEED-BK]";

    private static readonly string[] TenKhachHang =
    {
        "Nguyễn Văn An", "Trần Thị Bình", "Lê Hoàng Cường", "Phạm Thu Dung",
        "Hoàng Minh Đức", "Vũ Thị Hằng", "Đặng Quốc Huy", "Bùi Thanh Lan",
        "Ngô Tuấn Minh", "Đỗ Khánh Nam", "Lý Thu Oanh", "Trịnh Văn Phúc",
        "Phan Thị Quỳnh", "Tạ Minh Sơn", "Dương Thùy Trang", "Nguyễn Hữu Trí",
        "Lê Ngọc Tú", "Phạm Anh Tuấn", "Hoàng Mỹ Uyên", "Vũ Đức Việt",
        "Đặng Thị Xuân", "Bùi Văn Yên", "Ngô Bảo Châu", "Đỗ Minh Khoa",
        "Lý Gia Hân", "Trịnh Quốc Khánh", "Phan Thanh Mai", "Tạ Đức Long",
        "Dương Ngọc Ánh", "Nguyễn Văn Bình"
    };

    public static void Seed(AppDbContext db)
    {
        // DbSeeder tạo tài khoản, loại phòng và phòng trước khi hàm này được gọi.
        if (!db.LoaiPhongs.Any() || !db.Phongs.Any())
            return;

        SeedKhachHang(db);
        SeedDatPhong(db);
    }

    private static void SeedKhachHang(AppDbContext db)
    {
        var daDungCccd = new HashSet<string>(
            db.KhachHangs.Where(k => k.CCCD != null).Select(k => k.CCCD!).ToList());
        var daDungDienThoai = new HashSet<string>(
            db.KhachHangs.Where(k => k.SoDienThoai != null).Select(k => k.SoDienThoai!).ToList());
        var daDungEmail = new HashSet<string>(
            db.KhachHangs.Where(k => k.Email != null).Select(k => k.Email!).ToList(),
            StringComparer.OrdinalIgnoreCase);

        string TaoCccd(int stt)
        {
            var lan = 0;
            while (true)
            {
                var cccd = $"0792{(stt + lan * 1000):D8}";
                if (daDungCccd.Add(cccd)) return cccd;
                lan++;
            }
        }

        string TaoDienThoai(int stt)
        {
            var lan = 0;
            while (true)
            {
                var so = $"0902{(stt + lan * 1000):D6}";
                if (daDungDienThoai.Add(so)) return so;
                lan++;
            }
        }

        string TaoEmail(int stt)
        {
            var email = $"khachmau{stt:D2}@khachsan.vn";
            var lan = 1;
            while (!daDungEmail.Add(email))
            {
                email = $"khachmau{stt:D2}-{lan++}@khachsan.vn";
            }
            return email;
        }

        // Bảo đảm 4 tài khoản khách hàng mẫu luôn có hồ sơ để thử "Đặt phòng của tôi".
        var taiKhoanKhach = db.TaiKhoans
            .Where(t => t.VaiTro == VaiTroTaiKhoan.KhachHang && t.TrangThai)
            .OrderBy(t => t.TenDangNhap)
            .Take(4)
            .ToList();

        var stt = db.KhachHangs.Count();
        foreach (var taiKhoan in taiKhoanKhach)
        {
            if (db.KhachHangs.Any(k => k.MaTaiKhoan == taiKhoan.MaTaiKhoan))
                continue;

            stt++;
            db.KhachHangs.Add(new KhachHang
            {
                MaTaiKhoan = taiKhoan.MaTaiKhoan,
                HoTen = taiKhoan.HoTen,
                NgaySinh = new DateTime(1985 + stt % 15, 1 + stt % 12, 1 + stt % 27),
                GioiTinh = stt % 2 == 0 ? "Nam" : "Nữ",
                CCCD = TaoCccd(stt),
                SoDienThoai = TaoDienThoai(stt),
                Email = string.IsNullOrWhiteSpace(taiKhoan.Email) ? TaoEmail(stt) : taiKhoan.Email,
                DiaChi = $"Quận {stt % 12 + 1}, TP. Hồ Chí Minh",
                QuocTich = "Việt Nam",
                TrangThai = true,
                GhiChu = $"{TienToKhach} Hồ sơ liên kết tài khoản {taiKhoan.TenDangNhap}."
            });
        }
        db.SaveChanges();

        // Bổ sung khách hàng rời cho đủ 30 hồ sơ.
        while (db.KhachHangs.Count() < SoKhachHangMau)
        {
            stt++;
            var ten = TenKhachHang[(stt - 1) % TenKhachHang.Length];
            db.KhachHangs.Add(new KhachHang
            {
                HoTen = ten,
                NgaySinh = new DateTime(1980 + stt % 20, 1 + stt % 12, 1 + stt % 27),
                GioiTinh = stt % 2 == 0 ? "Nam" : "Nữ",
                CCCD = TaoCccd(stt),
                SoDienThoai = TaoDienThoai(stt),
                Email = TaoEmail(stt),
                DiaChi = new[] { "Hà Nội", "Hải Phòng", "Đà Nẵng", "TP. Hồ Chí Minh", "Cần Thơ" }[stt % 5],
                QuocTich = "Việt Nam",
                TrangThai = true,
                GhiChu = $"{TienToKhach} Khách hàng mẫu số {stt}."
            });
            db.SaveChanges();
        }
    }

    private static void SeedDatPhong(AppDbContext db)
    {
        var khachHangs = db.KhachHangs.OrderBy(k => k.MaKhachHang).ToList();
        var phongs = db.Phongs
            .Include(p => p.LoaiPhong)
            .OrderBy(p => p.MaPhong)
            .ToList();
        if (khachHangs.Count == 0 || phongs.Count == 0)
            return;

        var phongHoatDong = phongs.Where(LaPhongKinhDoanh).ToList();
        if (phongHoatDong.Count == 0)
            phongHoatDong = phongs;

        for (var i = 0; i < SoDatPhongMau; i++)
        {
            if (db.DatPhongs.Count() >= SoDatPhongMau)
                break;

            var marker = $"{TienToDatPhong}{i + 1:D2}]";
            if (db.DatPhongs.Any(d => d.GhiChu != null && d.GhiChu.Contains(marker)))
                continue;

            var kichBan = TaoKichBan(i, phongs, phongHoatDong);
            var phong = kichBan.Phong;
            var ngayNhan = kichBan.NgayNhan;
            var ngayTra = kichBan.NgayTra;

            // Nếu dữ liệu cũ đã chiếm lịch, dịch chuyển vài lần để tránh trùng.
            var buoc = ngayNhan.Date < DateTime.Today ? -30 : 14;
            var lan = 0;
            while (CoTrungLich(db, phong.MaPhong, ngayNhan, ngayTra, null) && lan++ < 12)
            {
                ngayNhan = ngayNhan.AddDays(buoc);
                ngayTra = ngayTra.AddDays(buoc);
            }

            var soNgay = TinhTienHelper.TinhSoNgay(ngayNhan, ngayTra);
            var datPhong = new DatPhong
            {
                MaKhachHang = khachHangs[i % khachHangs.Count].MaKhachHang,
                MaPhong = phong.MaPhong,
                NgayDat = ngayNhan.AddDays(-7),
                NgayNhan = ngayNhan,
                NgayTraDuKien = ngayTra,
                SoNguoi = Math.Clamp(1 + i % 3, 1, Math.Max(1, phong.LoaiPhong?.SoNguoiToiDa ?? 1)),
                TienCoc = Math.Round(phong.DonGia * soNgay * 0.3m, 0),
                TrangThai = kichBan.TrangThai,
                GhiChu = $"{marker} Dữ liệu mẫu tự sinh. Trạng thái: {kichBan.TrangThai}."
            };

            if (kichBan.TrangThai == TrangThaiDatPhong.DaNhanPhong)
            {
                datPhong.NgayNhanThucTe = ngayNhan.AddHours(14);
                phong.TrangThai = TrangThaiPhong.DangSuDung;
            }
            else if (kichBan.TrangThai == TrangThaiDatPhong.DaTraPhong)
            {
                datPhong.NgayNhanThucTe = ngayNhan.AddHours(14);
                datPhong.NgayTraThucTe = ngayTra.AddHours(12);
            }
            else if (kichBan.TrangThai is TrangThaiDatPhong.Huy or TrangThaiDatPhong.TuChoi)
            {
                datPhong.LyDoHuy = kichBan.TrangThai == TrangThaiDatPhong.Huy
                    ? "Khách thay đổi kế hoạch."
                    : "Không đủ điều kiện xác nhận.";
            }

            if (kichBan.CoChiTiet)
            {
                var trangThaiChiTiet = kichBan.TrangThai == TrangThaiDatPhong.DaTraPhong
                    ? TrangThaiChiTiet.DaTra
                    : TrangThaiChiTiet.DangO;

                datPhong.ChiTietDatPhongs.Add(new ChiTietDatPhong
                {
                    MaPhong = phong.MaPhong,
                    Phong = phong,
                    NgayNhan = ngayNhan,
                    NgayTra = ngayTra,
                    DonGia = phong.DonGia,
                    SoNgay = soNgay,
                    ThanhTien = TinhTienHelper.TinhThanhTien(phong.DonGia, soNgay),
                    TrangThai = trangThaiChiTiet,
                    NgayCapNhat = kichBan.TrangThai == TrangThaiDatPhong.DaTraPhong
                        ? ngayTra.AddHours(12)
                        : ngayNhan.AddHours(14)
                });
            }

            db.DatPhongs.Add(datPhong);
            db.SaveChanges();
        }
    }

    private static (Phong Phong, string TrangThai, bool CoChiTiet, DateTime NgayNhan, DateTime NgayTra)
        TaoKichBan(int i, List<Phong> tatCa, List<Phong> hoatDong)
    {
        var nhom = i / 15;
        var r = i % 15;

        // Nhóm 1: 10 lượt đã trả phòng (có chi tiết) + 5 lượt đã hủy (không chi tiết).
        if (nhom == 0)
        {
            var phong = tatCa[r % tatCa.Count];
            var nhan = new DateTime(2025, 1, 6).AddDays(r * 9);
            var tra = nhan.AddDays(2 + r % 3);
            return r < 10
                ? (phong, TrangThaiDatPhong.DaTraPhong, true, nhan, tra)
                : (phong, TrangThaiDatPhong.Huy, false, nhan, tra);
        }

        // Nhóm 2: 8 lượt đang ở (có chi tiết) + 7 lượt đã duyệt (chưa có chi tiết).
        if (nhom == 1)
        {
            if (r < 8)
            {
                var phong = hoatDong[r % hoatDong.Count];
                var nhan = DateTime.Today.AddDays(-1 + r % 2);
                var tra = nhan.AddDays(3 + r % 2);
                return (phong, TrangThaiDatPhong.DaNhanPhong, true, nhan, tra);
            }

            var phongDuyet = hoatDong[(r - 8) % hoatDong.Count];
            var nhanDuyet = DateTime.Today.AddDays(7 + r - 8);
            var traDuyet = nhanDuyet.AddDays(3);
            return (phongDuyet, TrangThaiDatPhong.DaDuyet, false, nhanDuyet, traDuyet);
        }

        // Nhóm 3: 12 lượt chờ duyệt + 3 lượt bị từ chối.
        if (r < 12)
        {
            var phong = hoatDong[r % hoatDong.Count];
            var nhan = DateTime.Today.AddDays(20 + r);
            var tra = nhan.AddDays(2 + r % 3);
            return (phong, TrangThaiDatPhong.ChoDuyet, false, nhan, tra);
        }

        var phongTuChoi = tatCa[r % tatCa.Count];
        var nhanTuChoi = DateTime.Today.AddDays(20 + r);
        var traTuChoi = nhanTuChoi.AddDays(2);
        return (phongTuChoi, TrangThaiDatPhong.TuChoi, false, nhanTuChoi, traTuChoi);
    }

    private static bool LaPhongKinhDoanh(Phong phong) =>
        phong.TrangThai != TrangThaiPhong.BaoTri
        && phong.TrangThai != TrangThaiPhong.KhongKinhDoanh
        && phong.TrangThai != TrangThaiPhong.NgungKinhDoanh;

    private static bool CoTrungLich(AppDbContext db, int maPhong, DateTime ngayNhan, DateTime ngayTra, int? boQua)
    {
        var trungDatPhong = db.DatPhongs.Any(d =>
            d.MaPhong == maPhong
            && d.MaDatPhong != boQua
            && d.TrangThai != TrangThaiDatPhong.Huy
            && d.TrangThai != TrangThaiDatPhong.TuChoi
            && d.NgayNhan < ngayTra
            && d.NgayTraDuKien > ngayNhan);

        if (trungDatPhong) return true;

        return db.ChiTietDatPhongs.Any(c =>
            c.MaPhong == maPhong
            && c.MaDatPhong != boQua
            && c.TrangThai != TrangThaiChiTiet.DaHuy
            && c.NgayNhan < ngayTra
            && c.NgayTra > ngayNhan);
    }
}
