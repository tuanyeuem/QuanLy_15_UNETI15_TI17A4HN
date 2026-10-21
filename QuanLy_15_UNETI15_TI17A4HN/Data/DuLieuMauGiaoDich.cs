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
        SeedDatPhong(db);
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

    // 15 phòng: 5 tầng x 3 phòng, 5 loại phòng. Phòng 9 đang bảo trì, phòng 10-11 đang có khách.
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
            else if (i == 10 || i == 11)
            {
                phong.TrangThai = TrangThaiPhong.DangSuDung;
            }
            db.Phongs.Add(phong);
        }
        db.SaveChanges();
    }

    // 30 khách hàng; 4 khách đầu liên kết với tài khoản khach1..khach4 để thử "Xem kết quả của tôi".
    private static void SeedKhachHang(AppDbContext db)
    {
        if (db.KhachHangs.Any()) return;

        string[] ten =
        {
            "Hoàng Minh Đức", "Đỗ Thu Trang", "Bùi Anh Tú", "Ngô Mai Linh", "Nguyễn Văn An",
            "Trần Thị Bình", "Lê Quang Cường", "Phạm Thị Dung", "Vũ Hải Đăng", "Đặng Thị Hà",
            "Hồ Văn Hùng", "Dương Thị Lan", "Lý Quốc Khánh", "Trịnh Thị Mai", "Phan Văn Nam",
            "Mai Thị Oanh", "Tạ Văn Phúc", "Lương Thị Quỳnh", "Đinh Văn Sơn", "Châu Thị Thảo",
            "Kim Ji-ho", "Tanaka Yuki", "Nguyễn Thị Uyên", "Võ Văn Vinh", "Cao Thị Xuân",
            "Hà Văn Yên", "Tô Thị Ánh", "Lâm Văn Bảo", "Thái Thị Chi", "Huỳnh Văn Dũng"
        };

        var taiKhoans = db.TaiKhoans
            .Where(t => t.VaiTro == VaiTroTaiKhoan.KhachHang && t.TrangThai)
            .OrderBy(t => t.TenDangNhap).Take(4).ToList();

        for (int i = 0; i < ten.Length; i++)
        {
            var tk = i < taiKhoans.Count ? taiKhoans[i] : null;
            db.KhachHangs.Add(new KhachHang
            {
                MaTaiKhoan = tk?.MaTaiKhoan,
                HoTen = tk?.HoTen ?? ten[i],
                NgaySinh = new DateTime(1975 + i % 28, 1 + i % 12, 1 + i % 28),
                GioiTinh = i % 2 == 0 ? "Nam" : "Nữ",
                CCCD = $"0012{i + 1:D8}",
                SoDienThoai = $"09{i + 1:D8}",
                Email = tk?.Email ?? $"khachhang{i + 1}@example.com",
                DiaChi = i % 3 == 0 ? "Hà Nội" : i % 3 == 1 ? "Hải Phòng" : "Đà Nẵng",
                QuocTich = i == 20 ? "Hàn Quốc" : i == 21 ? "Nhật Bản" : "Việt Nam",
                TrangThai = true
            });
        }
        db.SaveChanges();
    }

    // 50 lượt đặt phòng, mỗi lượt 1 chi tiết. Các lượt trong quá khứ không trùng lịch cùng phòng.
    private static void SeedDatPhong(AppDbContext db)
    {
        if (db.DatPhongs.Any()) return;

        var today = DateTime.Today;
        var phongs = db.Phongs.Include(p => p.LoaiPhong).OrderBy(p => p.MaPhong).ToList();
        var khachs = db.KhachHangs.OrderBy(k => k.MaKhachHang).ToList();
        if (phongs.Count < 15 || khachs.Count == 0) return;

        for (int i = 0; i < 50; i++)
        {
            Phong phong;
            DateTime nhan, tra, ngayDat;
            string ttDat, ttCt;
            DateTime? capNhat = null;
            int soNgay;

            if (i < 45)
            {
                // Lượt đã kết thúc: Hoàn thành (phòng đã trả) hoặc Đã hủy.
                phong = phongs[i % 15];
                var vong = i / 15;
                nhan = today.AddDays(-130 + vong * 40 + i % 7);
                tra = nhan.AddDays(1 + i % 4);
                ngayDat = nhan.AddDays(-2).AddHours(9 + i % 8);

                var huy = i % 9 == 4;
                ttDat = huy ? TrangThaiDatPhong.DaHuy : TrangThaiDatPhong.HoanThanh;
                ttCt = huy ? TrangThaiChiTiet.DaHuy : TrangThaiChiTiet.DaTra;
                soNgay = huy ? 0 : TinhTienHelper.TinhSoNgay(nhan, tra);
                if (!huy) capNhat = tra.AddHours(12);
            }
            else
            {
                // 5 lượt gần đây: 2 đang xử lý (đang ở phòng 10, 11), 3 chờ xử lý.
                var k = i - 45;
                phong = phongs[10 + k];
                if (k < 2)
                {
                    nhan = today.AddDays(-1);
                    tra = today.AddDays(2);
                    ngayDat = today.AddDays(-3).AddHours(10 + k);
                    ttDat = TrangThaiDatPhong.DangXuLy;
                    ttCt = TrangThaiChiTiet.DangO;
                }
                else
                {
                    nhan = today.AddDays(3 + k);
                    tra = nhan.AddDays(2);
                    ngayDat = today.AddDays(-1).AddHours(9 + k);
                    ttDat = TrangThaiDatPhong.ChoXuLy;
                    ttCt = TrangThaiChiTiet.ChuaNhan;
                }
                soNgay = TinhTienHelper.TinhSoNgay(nhan, tra);   // tạm tính theo dự kiến
            }

            var thanhTien = ttCt == TrangThaiChiTiet.DaHuy ? 0 : TinhTienHelper.TinhThanhTien(phong.DonGia, soNgay);
            var duKien = TinhTienHelper.TinhThanhTien(phong.DonGia, TinhTienHelper.TinhSoNgay(nhan, tra));

            db.DatPhongs.Add(new DatPhong
            {
                MaKhachHang = khachs[i % khachs.Count].MaKhachHang,
                NgayDat = ngayDat,
                NgayNhan = nhan,
                NgayTraDuKien = tra,
                SoNguoi = 1 + i % phong.LoaiPhong.SoNguoiToiDa,
                TienCoc = Math.Round(duKien * 0.3m / 1000m) * 1000m,
                TrangThai = ttDat,
                GhiChu = i % 5 == 0 ? "Khách đặt qua điện thoại" : null,
                ChiTietDatPhongs =
                {
                    new ChiTietDatPhong
                    {
                        MaPhong = phong.MaPhong,
                        NgayNhan = nhan,
                        NgayTra = tra,
                        DonGia = phong.DonGia,
                        SoNgay = soNgay,
                        ThanhTien = thanhTien,
                        TrangThai = ttCt,
                        NgayCapNhat = capNhat
                    }
                }
            });
        }
        db.SaveChanges();
    }
}
