<<<<<<< HEAD:QuanLy_15_UNETI15_TI17A4HN/Data/DbSeeder.cs
// Họ và tên: Vũ Xuân Tuấn
// Mã sinh viên: 23103100035
// Nội dung thực hiện: Thêm Phòng , Khách hàng, Đặt phòng mẫu vào cơ sở dữ liệu khi khởi tạo ứng dụng (DbSeeder.cs)
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
        // ── Phòng (15 phòng, nhiều trạng thái) ──
        if (!db.Phongs.Any())
        {
            var lp = db.LoaiPhongs.ToList();
            int std1 = lp.First(l => l.TenLoai == "Standard Đơn").MaLoaiPhong;
            int std2 = lp.First(l => l.TenLoai == "Standard Đôi").MaLoaiPhong;
            int dlx  = lp.First(l => l.TenLoai == "Deluxe").MaLoaiPhong;
            int fam  = lp.First(l => l.TenLoai == "Family").MaLoaiPhong;
            int suit = lp.First(l => l.TenLoai == "Suite").MaLoaiPhong;
            db.Phongs.AddRange(
                new Phong { SoPhong = "101", MaLoaiPhong = std1, Tang = 1, HuongPhong = "Đông", DonGia = 400_000, TrangThai = "TrongRanh" },
                new Phong { SoPhong = "102", MaLoaiPhong = std1, Tang = 1, HuongPhong = "Tây",  DonGia = 400_000, TrangThai = "TrongRanh" },
                new Phong { SoPhong = "103", MaLoaiPhong = std2, Tang = 1, HuongPhong = "Nam",  DonGia = 600_000, TrangThai = "TrongRanh" },
                new Phong { SoPhong = "201", MaLoaiPhong = std2, Tang = 2, HuongPhong = "Đông", DonGia = 600_000, TrangThai = "DangSuDung" },
                new Phong { SoPhong = "202", MaLoaiPhong = dlx,  Tang = 2, HuongPhong = "Nam",  DonGia = 900_000, TrangThai = "TrongRanh" },
                new Phong { SoPhong = "203", MaLoaiPhong = dlx,  Tang = 2, HuongPhong = "Tây",  DonGia = 900_000, TrangThai = "BaoTri", NgayBaoTri = DateTime.Today },
                new Phong { SoPhong = "301", MaLoaiPhong = dlx,  Tang = 3, HuongPhong = "Đông", DonGia = 900_000, TrangThai = "TrongRanh" },
                new Phong { SoPhong = "302", MaLoaiPhong = fam,  Tang = 3, HuongPhong = "Nam",  DonGia = 1_400_000, TrangThai = "TrongRanh" },
                new Phong { SoPhong = "303", MaLoaiPhong = fam,  Tang = 3, HuongPhong = "Tây",  DonGia = 1_400_000, TrangThai = "DangSuDung" },
                new Phong { SoPhong = "401", MaLoaiPhong = fam,  Tang = 4, HuongPhong = "Đông", DonGia = 1_400_000, TrangThai = "TrongRanh" },
                new Phong { SoPhong = "402", MaLoaiPhong = suit, Tang = 4, HuongPhong = "Toàn cảnh", DonGia = 2_000_000, TrangThai = "TrongRanh" },
                new Phong { SoPhong = "403", MaLoaiPhong = suit, Tang = 4, HuongPhong = "Toàn cảnh", DonGia = 2_000_000, TrangThai = "DangSuDung" },
                new Phong { SoPhong = "501", MaLoaiPhong = suit, Tang = 5, HuongPhong = "Toàn cảnh", DonGia = 2_000_000, TrangThai = "TrongRanh" },
                new Phong { SoPhong = "104", MaLoaiPhong = std1, Tang = 1, HuongPhong = "Bắc",  DonGia = 400_000, TrangThai = "KhongKinhDoanh" },
                new Phong { SoPhong = "204", MaLoaiPhong = std2, Tang = 2, HuongPhong = "Bắc",  DonGia = 600_000, TrangThai = "TrongRanh" }
            );
            db.SaveChanges();
        }
        // ── Khách hàng (30 khách) ──
        if (!db.KhachHangs.Any())
        {
            db.KhachHangs.AddRange(
                new KhachHang { HoTen = "Nguyễn Văn An",     GioiTinh = "Nam",  SoDienThoai = "0901111001", Email = "an@gmail.com",     CCCD = "001001000001", QuocTich = "Việt Nam", TrangThai = true },
                new KhachHang { HoTen = "Trần Thị Bình",     GioiTinh = "Nữ",   SoDienThoai = "0901111002", Email = "binh@gmail.com",   CCCD = "001001000002", QuocTich = "Việt Nam", TrangThai = true },
                new KhachHang { HoTen = "Lê Văn Cường",      GioiTinh = "Nam",  SoDienThoai = "0901111003", Email = "cuong@gmail.com",  CCCD = "001001000003", QuocTich = "Việt Nam", TrangThai = true },
                new KhachHang { HoTen = "Phạm Thị Dung",     GioiTinh = "Nữ",   SoDienThoai = "0901111004", Email = "dung@gmail.com",   CCCD = "001001000004", QuocTich = "Việt Nam", TrangThai = true },
                new KhachHang { HoTen = "Hoàng Văn Em",      GioiTinh = "Nam",  SoDienThoai = "0901111005", Email = "em@gmail.com",     CCCD = "001001000005", QuocTich = "Việt Nam", TrangThai = true },
                new KhachHang { HoTen = "Vũ Thị Phương",     GioiTinh = "Nữ",   SoDienThoai = "0901111006", Email = "phuong@gmail.com", CCCD = "001001000006", QuocTich = "Việt Nam", TrangThai = true },
                new KhachHang { HoTen = "Đặng Văn Giang",    GioiTinh = "Nam",  SoDienThoai = "0901111007", Email = "giang@gmail.com",  CCCD = "001001000007", QuocTich = "Việt Nam", TrangThai = true },
                new KhachHang { HoTen = "Bùi Thị Hà",        GioiTinh = "Nữ",   SoDienThoai = "0901111008", Email = "ha@gmail.com",     CCCD = "001001000008", QuocTich = "Việt Nam", TrangThai = true },
                new KhachHang { HoTen = "Ngô Văn Ích",       GioiTinh = "Nam",  SoDienThoai = "0901111009", Email = "ich@gmail.com",    CCCD = "001001000009", QuocTich = "Việt Nam", TrangThai = true },
                new KhachHang { HoTen = "Đinh Thị Kim",      GioiTinh = "Nữ",   SoDienThoai = "0901111010", Email = "kim@gmail.com",    CCCD = "001001000010", QuocTich = "Việt Nam", TrangThai = true },
                new KhachHang { HoTen = "Lý Văn Long",       GioiTinh = "Nam",  SoDienThoai = "0901111011", Email = "long@gmail.com",   CCCD = "001001000011", QuocTich = "Việt Nam", TrangThai = true },
                new KhachHang { HoTen = "Phan Thị Mai",      GioiTinh = "Nữ",   SoDienThoai = "0901111012", Email = "mai@gmail.com",    CCCD = "001001000012", QuocTich = "Việt Nam", TrangThai = true },
                new KhachHang { HoTen = "Trương Văn Nam",    GioiTinh = "Nam",  SoDienThoai = "0901111013", Email = "nam@gmail.com",    CCCD = "001001000013", QuocTich = "Việt Nam", TrangThai = true },
                new KhachHang { HoTen = "Dương Thị Oanh",    GioiTinh = "Nữ",   SoDienThoai = "0901111014", Email = "oanh@gmail.com",   CCCD = "001001000014", QuocTich = "Việt Nam", TrangThai = true },
                new KhachHang { HoTen = "Hồ Văn Phúc",      GioiTinh = "Nam",  SoDienThoai = "0901111015", Email = "phuc@gmail.com",   CCCD = "001001000015", QuocTich = "Việt Nam", TrangThai = true },
                new KhachHang { HoTen = "Tô Thị Quỳnh",     GioiTinh = "Nữ",   SoDienThoai = "0901111016", Email = "quynh@gmail.com",  CCCD = "001001000016", QuocTich = "Việt Nam", TrangThai = true },
                new KhachHang { HoTen = "Cao Văn Rồng",     GioiTinh = "Nam",  SoDienThoai = "0901111017", Email = "rong@gmail.com",   CCCD = "001001000017", QuocTich = "Việt Nam", TrangThai = true },
                new KhachHang { HoTen = "Mai Thị Sen",       GioiTinh = "Nữ",   SoDienThoai = "0901111018", Email = "sen@gmail.com",    CCCD = "001001000018", QuocTich = "Việt Nam", TrangThai = true },
                new KhachHang { HoTen = "Kiều Văn Thắng",   GioiTinh = "Nam",  SoDienThoai = "0901111019", Email = "thang@gmail.com",  CCCD = "001001000019", QuocTich = "Việt Nam", TrangThai = true },
                new KhachHang { HoTen = "Lưu Thị Uyên",     GioiTinh = "Nữ",   SoDienThoai = "0901111020", Email = "uyen@gmail.com",   CCCD = "001001000020", QuocTich = "Việt Nam", TrangThai = true },
                new KhachHang { HoTen = "John Smith",        GioiTinh = "Nam",  SoDienThoai = "0901111021", Email = "john@mail.com",    CCCD = "PASS001",      QuocTich = "Mỹ",       TrangThai = true },
                new KhachHang { HoTen = "Emma Johnson",      GioiTinh = "Nữ",   SoDienThoai = "0901111022", Email = "emma@mail.com",    CCCD = "PASS002",      QuocTich = "Anh",      TrangThai = true },
                new KhachHang { HoTen = "Zhang Wei",         GioiTinh = "Nam",  SoDienThoai = "0901111023", Email = "zhang@mail.com",   CCCD = "PASS003",      QuocTich = "Trung Quốc", TrangThai = true },
                new KhachHang { HoTen = "Yuki Tanaka",       GioiTinh = "Nữ",   SoDienThoai = "0901111024", Email = "yuki@mail.com",    CCCD = "PASS004",      QuocTich = "Nhật Bản", TrangThai = true },
                new KhachHang { HoTen = "Park Jiyeon",       GioiTinh = "Nữ",   SoDienThoai = "0901111025", Email = "park@mail.com",    CCCD = "PASS005",      QuocTich = "Hàn Quốc", TrangThai = true },
                new KhachHang { HoTen = "Nguyễn Thị Vân",   GioiTinh = "Nữ",   SoDienThoai = "0901111026", Email = "van@gmail.com",    CCCD = "001001000026", QuocTich = "Việt Nam", TrangThai = true },
                new KhachHang { HoTen = "Trần Văn Xinh",    GioiTinh = "Nam",  SoDienThoai = "0901111027", Email = "xinh@gmail.com",   CCCD = "001001000027", QuocTich = "Việt Nam", TrangThai = true },
                new KhachHang { HoTen = "Lê Thị Yến",       GioiTinh = "Nữ",   SoDienThoai = "0901111028", Email = "yen@gmail.com",    CCCD = "001001000028", QuocTich = "Việt Nam", TrangThai = true },
                new KhachHang { HoTen = "Phạm Văn Zũng",    GioiTinh = "Nam",  SoDienThoai = "0901111029", Email = "zung@gmail.com",   CCCD = "001001000029", QuocTich = "Việt Nam", TrangThai = true },
                new KhachHang { HoTen = "Hoàng Thị Ánh",    GioiTinh = "Nữ",   SoDienThoai = "0901111030", Email = "anh@gmail.com",    CCCD = "001001000030", QuocTich = "Việt Nam", TrangThai = false }
            );
            db.SaveChanges();
        }
        // ── Đặt phòng mẫu (50 bản ghi, nhiều trạng thái) ──
        if (!db.DatPhongs.Any())
        {
            var phongs = db.Phongs.ToList();
            var khs    = db.KhachHangs.ToList();

            var khList = khs.ToList();
            int KhId(int idx) => khList[idx % khList.Count].MaKhachHang;
            int PhId(string so) => phongs.First(p => p.SoPhong == so).MaPhong;
            var list = new List<DatPhong>();
            // 10 ChoDuyet
            for (int i = 0; i < 10; i++)
                list.Add(new DatPhong { MaKhachHang = KhId(i),    MaPhong = PhId("101"), NgayDat = DateTime.Now.AddDays(-i),      NgayNhan = DateTime.Now.AddDays(i+1),  NgayTraDuKien = DateTime.Now.AddDays(i+3),  SoNguoi=1, TienCoc=200_000, TrangThai = TrangThaiDatPhong.ChoDuyet });
            // 10 DaDuyet
            for (int i = 0; i < 10; i++)
                list.Add(new DatPhong { MaKhachHang = KhId(i+10), MaPhong = PhId("102"), NgayDat = DateTime.Now.AddDays(-i-5),   NgayNhan = DateTime.Now.AddDays(i+1),  NgayTraDuKien = DateTime.Now.AddDays(i+4),  SoNguoi=1, TienCoc=200_000, TrangThai = TrangThaiDatPhong.DaDuyet });
            // 10 DaNhanPhong
            for (int i = 0; i < 10; i++)
                list.Add(new DatPhong { MaKhachHang = KhId(i+5),  MaPhong = PhId("103"), NgayDat = DateTime.Now.AddDays(-i-10),  NgayNhan = DateTime.Now.AddDays(-i),   NgayTraDuKien = DateTime.Now.AddDays(i+2),  SoNguoi=1, TienCoc=200_000, TrangThai = TrangThaiDatPhong.DaNhanPhong, NgayNhanThucTe = DateTime.Now.AddDays(-i) });
            // 10 DaTraPhong
            for (int i = 0; i < 10; i++)
                list.Add(new DatPhong { MaKhachHang = KhId(i+15), MaPhong = PhId("201"), NgayDat = DateTime.Now.AddDays(-i-20), NgayNhan = DateTime.Now.AddDays(-i-10), NgayTraDuKien = DateTime.Now.AddDays(-i-5), SoNguoi=1, TienCoc=200_000, TrangThai = TrangThaiDatPhong.DaTraPhong, NgayNhanThucTe = DateTime.Now.AddDays(-i-10), NgayTraThucTe = DateTime.Now.AddDays(-i-5) });
            // 10 Huy
            for (int i = 0; i < 10; i++)
                list.Add(new DatPhong { MaKhachHang = KhId(i+20), MaPhong = PhId("204"), NgayDat = DateTime.Now.AddDays(-i-3),  NgayNhan = DateTime.Now.AddDays(i+2),  NgayTraDuKien = DateTime.Now.AddDays(i+5),  SoNguoi=1, TienCoc=200_000, TrangThai = TrangThaiDatPhong.Huy, LyDoHuy = "Khách hủy vì lý do cá nhân" });
            db.DatPhongs.AddRange(list);
            db.SaveChanges();
        }
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
=======
// Họ và tên: Lê Văn Thanh
// Mã sinh viên: 23103100200
// Nội dung thực hiện: Module 5 - Dữ liệu mẫu tài khoản (Admin, Nhân viên, Khách hàng) và loại phòng phục vụ kiểm thử.
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
                TaoTaiKhoan("admin1", "Admin@123", "Lê Văn Thanh", "admin1@khachsan.vn", VaiTroTaiKhoan.Admin),
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

        // Dữ liệu giao dịch (phòng, khách hàng, đặt phòng, chi tiết) phục vụ Module 5.
        DuLieuMauGiaoDich.Seed(db);
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
>>>>>>> origin/md5:Data/DbSeeder.cs
}