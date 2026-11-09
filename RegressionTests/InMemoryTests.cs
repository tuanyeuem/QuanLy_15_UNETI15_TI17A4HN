// Test in-memory — không cần SQL Server LocalDB.
// Chạy: dotnet run --project RegressionTests -- inmemory
using System.Security.Cryptography;
using System.Text;
using QuanLy_15_UNETI15_TI17A4HN.Helpers;
using QuanLy_15_UNETI15_TI17A4HN.Models;
using QuanLy_15_UNETI15_TI17A4HN.ViewModels;

namespace RegressionTests;

public static class InMemoryTests
{
    static int _pass, _fail;

    static void Check(bool condition, string message)
    {
        if (condition) { _pass++; Console.WriteLine("PASS: " + message); }
        else { _fail++; Console.WriteLine("FAIL: " + message); }
    }

    public static int Run()
    {
        _pass = 0; _fail = 0;
        TestMatKhauHelper();
        TestTinhTienHelper();
        TestTrangThaiUi();
        TestPhongAnhHelper();
        TestDatPhongListViewModel();
        TestChiTietFormViewModelValidation();
        TestKetQuaDatPhongViewModel();
        TestTrangThaiConstants();

        Console.WriteLine();
        Console.WriteLine($"=== Kết quả: {_pass} PASS, {_fail} FAIL ===");
        return _fail;
    }

    // ======================== MatKhauHelper ========================
    static void TestMatKhauHelper()
    {
        Console.WriteLine("\n--- MatKhauHelper ---");

        // 1. Hash dùng random salt → 2 lần hash cùng mật khẩu cho kết quả khác nhau
        var h1 = MatKhauHelper.BamMatKhau("Test@123");
        var h2 = MatKhauHelper.BamMatKhau("Test@123");
        Check(h1 != h2, "Hai lần hash cùng mật khẩu cho kết quả khác nhau (random salt)");

        // 2. Xác thực đúng
        Check(MatKhauHelper.KiemTraMatKhau("Test@123", h1), "Xác thực đúng mật khẩu");

        // 3. Xác thực sai
        Check(!MatKhauHelper.KiemTraMatKhau("Wrong@123", h1), "Từ chối mật khẩu sai");

        // 4. Empty/null edge cases
        Check(MatKhauHelper.BamMatKhau("") == string.Empty, "Hash mật khẩu rỗng trả về chuỗi rỗng");
        Check(MatKhauHelper.BamMatKhau(null!) == string.Empty, "Hash null trả về chuỗi rỗng");
        Check(!MatKhauHelper.KiemTraMatKhau("", h1), "Mật khẩu rỗng bị từ chối");
        Check(!MatKhauHelper.KiemTraMatKhau(null!, h1), "Mật khẩu null bị từ chối");
        Check(!MatKhauHelper.KiemTraMatKhau("Test@123", ""), "Hash rỗng bị từ chối");

        // 5. Legacy SHA-256 tương thích
        var oldHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes("Test@123")));
        Check(MatKhauHelper.KiemTraMatKhau("Test@123", oldHash, out var upgrade) && upgrade,
            "Legacy SHA-256 hash vẫn xác thực được và đánh dấu cần nâng cấp");

        // 6. Legacy SHA-256 sai mật khẩu
        Check(!MatKhauHelper.KiemTraMatKhau("Wrong", oldHash), "Legacy SHA-256 từ chối mật khẩu sai");

        // 7. Hash format sai
        Check(!MatKhauHelper.KiemTraMatKhau("Test@123", "invalid-hash-format"),
            "Hash format không hợp lệ bị từ chối mà không throw");

        // 8. Whitespace-only password
        Check(MatKhauHelper.BamMatKhau("   ") == string.Empty, "Hash mật khẩu chỉ khoảng trắng trả về rỗng");
    }

    // ======================== TinhTienHelper ========================
    static void TestTinhTienHelper()
    {
        Console.WriteLine("\n--- TinhTienHelper ---");

        // 1. Ở 1 đêm
        Check(TinhTienHelper.TinhSoNgay(new DateTime(2024, 1, 1), new DateTime(2024, 1, 2)) == 1,
            "1 đêm = 1 ngày");

        // 2. Ở 3 đêm
        Check(TinhTienHelper.TinhSoNgay(new DateTime(2024, 1, 1), new DateTime(2024, 1, 4)) == 3,
            "3 đêm = 3 ngày");

        // 3. Ở trong ngày (nhận và trả cùng ngày) → tối thiểu 1 ngày
        Check(TinhTienHelper.TinhSoNgay(new DateTime(2024, 1, 1), new DateTime(2024, 1, 1)) == 1,
            "Ở trong ngày vẫn tính 1 ngày (tối thiểu)");

        // 4. Tính thành tiền
        Check(TinhTienHelper.TinhThanhTien(500_000m, 3) == 1_500_000m,
            "500.000đ × 3 đêm = 1.500.000đ");

        // 5. Đơn giá 0
        Check(TinhTienHelper.TinhThanhTien(0m, 5) == 0m,
            "Đơn giá 0 → thành tiền 0");

        // 6. Đơn giá âm → trả về 0
        Check(TinhTienHelper.TinhThanhTien(-100m, 2) == 0m,
            "Đơn giá âm → thành tiền 0 (bảo vệ)");

        // 7. Số ngày âm → trả về 0
        Check(TinhTienHelper.TinhThanhTien(100m, -1) == 0m,
            "Số ngày âm → thành tiền 0 (bảo vệ)");

        // 8. Ngày trả trước ngày nhận → Math.Max(1,...) = 1
        Check(TinhTienHelper.TinhSoNgay(new DateTime(2024, 1, 5), new DateTime(2024, 1, 3)) == 1,
            "Ngày trả trước ngày nhận → vẫn tối thiểu 1 ngày");
    }

    // ======================== TrangThaiUi ========================
    static void TestTrangThaiUi()
    {
        Console.WriteLine("\n--- TrangThaiUi ---");

        // 1. Badge - Chờ duyệt
        Check(TrangThaiUi.Badge(TrangThaiDatPhong.ChoDuyet) == "bg-warning text-dark",
            "Badge ChoDuyet = bg-warning text-dark");

        // 2. Badge - Đã duyệt (DangXuLy alias)
        Check(TrangThaiUi.Badge(TrangThaiDatPhong.DaDuyet) == "bg-primary",
            "Badge DaDuyet = bg-primary");

        // 3. Badge - Đã nhận phòng
        Check(TrangThaiUi.Badge(TrangThaiDatPhong.DaNhanPhong) == "bg-primary",
            "Badge DaNhanPhong = bg-primary");

        // 4. Badge - Hoàn thành
        Check(TrangThaiUi.Badge(TrangThaiDatPhong.DaTraPhong) == "bg-success",
            "Badge DaTraPhong = bg-success");

        // 5. Badge - Đã hủy
        Check(TrangThaiUi.Badge(TrangThaiDatPhong.DaHuy) == "bg-secondary",
            "Badge Huy = bg-secondary");

        // 6. Badge - Từ chối  (BUG-08: trước đây rơi vào default)
        Check(TrangThaiUi.Badge(TrangThaiDatPhong.TuChoi) == "bg-secondary",
            "Badge TuChoi = bg-secondary (BUG-08 đã fix)");

        // 7. Badge - null/unknown
        Check(TrangThaiUi.Badge(null) == "bg-light text-dark",
            "Badge null = bg-light text-dark (default)");
        Check(TrangThaiUi.Badge("UnknownStatus") == "bg-light text-dark",
            "Badge unknown = bg-light text-dark (default)");

        // 8. Badge - ChiTiet trạng thái
        Check(TrangThaiUi.Badge(TrangThaiChiTiet.ChuaNhan) == "bg-warning text-dark",
            "Badge ChiTiet ChuaNhan = bg-warning text-dark");
        Check(TrangThaiUi.Badge(TrangThaiChiTiet.DangO) == "bg-primary",
            "Badge ChiTiet DangO = bg-primary");
        Check(TrangThaiUi.Badge(TrangThaiChiTiet.DaTra) == "bg-success",
            "Badge ChiTiet DaTra = bg-success");

        // 9. Tien - định dạng tiền
        Check(TrangThaiUi.Tien(1_500_000m) == "1.500.000 đ",
            "Định dạng tiền: 1.500.000 đ");
        Check(TrangThaiUi.Tien(0m) == "0 đ", "Định dạng tiền: 0 đ");

        // 10. NhanPhong
        Check(TrangThaiUi.NhanPhong("TrongRanh") == "Còn trống", "NhanPhong TrongRanh");
        Check(TrangThaiUi.NhanPhong("DaDat") == "Đã đặt", "NhanPhong DaDat");
        Check(TrangThaiUi.NhanPhong("DangSuDung") == "Đang sử dụng", "NhanPhong DangSuDung");
        Check(TrangThaiUi.NhanPhong("BaoTri") == "Bảo trì", "NhanPhong BaoTri");
        Check(TrangThaiUi.NhanPhong("KhongKinhDoanh") == "Ngừng kinh doanh", "NhanPhong KhongKinhDoanh");
        Check(TrangThaiUi.NhanPhong(null) == "Chưa rõ", "NhanPhong null");
        Check(TrangThaiUi.NhanPhong("") == "Chưa rõ", "NhanPhong rỗng");

        // 11. LopPhong
        Check(TrangThaiUi.LopPhong("TrongRanh") == "tt-ok", "LopPhong TrongRanh = tt-ok");
        Check(TrangThaiUi.LopPhong("DaDat") == "tt-info", "LopPhong DaDat = tt-info");
        Check(TrangThaiUi.LopPhong("BaoTri") == "tt-warn", "LopPhong BaoTri = tt-warn");
        Check(TrangThaiUi.LopPhong("KhongKinhDoanh") == "tt-off", "LopPhong KhongKinhDoanh = tt-off");

        // 12. TienNghi
        var tn = TrangThaiUi.TienNghi("WiFi, TV, Máy lạnh, Tủ lạnh, Bồn tắm");
        Check(tn.Length == 4, "TienNghi tối đa 4 mục (mặc định)");
        Check(tn[0] == "WiFi", "TienNghi mục đầu = WiFi");
        Check(TrangThaiUi.TienNghi(null).Length == 0, "TienNghi null → mảng rỗng");
        Check(TrangThaiUi.TienNghi("").Length == 0, "TienNghi rỗng → mảng rỗng");
        var tn2 = TrangThaiUi.TienNghi("A;B;C", 2);
        Check(tn2.Length == 2, "TienNghi giới hạn 2 mục");
    }

    // ======================== PhongAnhHelper ========================
    static void TestPhongAnhHelper()
    {
        Console.WriteLine("\n--- PhongAnhHelper ---");

        // 1. AnhMacDinh theo tên loại
        Check(PhongAnhHelper.AnhMacDinh("Phòng Suite cao cấp", 1) == "/images/phong/suite.jpg",
            "AnhMacDinh Suite");
        Check(PhongAnhHelper.AnhMacDinh("Phòng gia đình", 1) == "/images/phong/family.jpg",
            "AnhMacDinh gia đình");
        Check(PhongAnhHelper.AnhMacDinh("Family Room", 1) == "/images/phong/family.jpg",
            "AnhMacDinh Family");
        Check(PhongAnhHelper.AnhMacDinh("Deluxe Room", 1) == "/images/phong/deluxe.jpg",
            "AnhMacDinh Deluxe");
        Check(PhongAnhHelper.AnhMacDinh("Phòng đôi", 1) == "/images/phong/doi.jpg",
            "AnhMacDinh đôi");
        Check(PhongAnhHelper.AnhMacDinh("Double Room", 1) == "/images/phong/doi.jpg",
            "AnhMacDinh Double");
        Check(PhongAnhHelper.AnhMacDinh("Twin Room", 1) == "/images/phong/doi.jpg",
            "AnhMacDinh Twin");
        Check(PhongAnhHelper.AnhMacDinh("Phòng đơn", 1) == "/images/phong/don.jpg",
            "AnhMacDinh đơn");
        Check(PhongAnhHelper.AnhMacDinh("Single Room", 1) == "/images/phong/don.jpg",
            "AnhMacDinh Single");

        // 2. AnhMacDinh fallback theo mã loại phòng
        Check(PhongAnhHelper.AnhMacDinh("Phòng XYZ", 1) == "/images/phong/don.jpg",
            "AnhMacDinh fallback maLoai=1 → don");
        Check(PhongAnhHelper.AnhMacDinh("Phòng XYZ", 2) == "/images/phong/doi.jpg",
            "AnhMacDinh fallback maLoai=2 → doi");
        Check(PhongAnhHelper.AnhMacDinh("Phòng XYZ", 3) == "/images/phong/deluxe.jpg",
            "AnhMacDinh fallback maLoai=3 → deluxe");
        Check(PhongAnhHelper.AnhMacDinh("Phòng XYZ", 4) == "/images/phong/family.jpg",
            "AnhMacDinh fallback maLoai=4 → family");
        Check(PhongAnhHelper.AnhMacDinh("Phòng XYZ", 5) == "/images/phong/suite.jpg",
            "AnhMacDinh fallback maLoai=5 → suite");
        Check(PhongAnhHelper.AnhMacDinh("Phòng XYZ", 6) == "/images/phong/don.jpg",
            "AnhMacDinh fallback maLoai=6 wrap → don");

        // 3. AnhMacDinh null tên loại
        Check(PhongAnhHelper.AnhMacDinh(null, 1) == "/images/phong/don.jpg",
            "AnhMacDinh null tên loại → fallback theo mã");

        // 4. NhanDangDuoi - nhận dạng file format
        // JPEG
        var jpegBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0, 0, 0, 0, 0, 0, 0, 0 };
        using (var ms = new MemoryStream(jpegBytes))
            Check(PhongAnhHelper.NhanDangDuoi(ms) == ".jpg", "NhanDangDuoi JPEG");

        // PNG
        var pngBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0, 0, 0, 0 };
        using (var ms = new MemoryStream(pngBytes))
            Check(PhongAnhHelper.NhanDangDuoi(ms) == ".png", "NhanDangDuoi PNG");

        // WebP
        var webpBytes = new byte[] { 0x52, 0x49, 0x46, 0x46, 0, 0, 0, 0, 0x57, 0x45, 0x42, 0x50 };
        using (var ms = new MemoryStream(webpBytes))
            Check(PhongAnhHelper.NhanDangDuoi(ms) == ".webp", "NhanDangDuoi WebP");

        // Unknown
        var unknownBytes = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        using (var ms = new MemoryStream(unknownBytes))
            Check(PhongAnhHelper.NhanDangDuoi(ms) == null, "NhanDangDuoi file không nhận dạng → null");

        // Empty stream
        using (var ms = new MemoryStream(Array.Empty<byte>()))
            Check(PhongAnhHelper.NhanDangDuoi(ms) == null, "NhanDangDuoi stream rỗng → null");

        // 5. Constants
        Check(PhongAnhHelper.KichThuocToiDa == 5 * 1024 * 1024, "KichThuocToiDa = 5MB");
        Check(PhongAnhHelper.DuoiChoPhep.Length == 3, "3 loại đuôi cho phép");
    }

    // ======================== DatPhongListViewModel ========================
    static void TestDatPhongListViewModel()
    {
        Console.WriteLine("\n--- DatPhongListViewModel ---");

        var vm = new DatPhongListViewModel();

        // Dropdown trạng thái phải có đủ 6 giá trị
        Check(vm.DanhSachTrangThai.Count == 6, "Dropdown trạng thái có 6 giá trị");
        Check(vm.DanhSachTrangThai.Contains(TrangThaiDatPhong.ChoDuyet), "Dropdown chứa ChoDuyet");
        Check(vm.DanhSachTrangThai.Contains(TrangThaiDatPhong.DaDuyet), "Dropdown chứa DaDuyet");
        Check(vm.DanhSachTrangThai.Contains(TrangThaiDatPhong.DaNhanPhong), "Dropdown chứa DaNhanPhong");
        Check(vm.DanhSachTrangThai.Contains(TrangThaiDatPhong.DaTraPhong), "Dropdown chứa DaTraPhong");
        Check(vm.DanhSachTrangThai.Contains(TrangThaiDatPhong.Huy), "Dropdown chứa Huy");
        Check(vm.DanhSachTrangThai.Contains(TrangThaiDatPhong.TuChoi),
            "Dropdown chứa TuChoi (BUG-09: trước đây bị thiếu)");

        // Defaults
        Check(vm.TrangHienTai == 1, "TrangHienTai mặc định = 1");
        Check(vm.SapXep == "ngaydat_moi", "SapXep mặc định = ngaydat_moi");
    }

    // ======================== ChiTietDatPhongFormViewModel validation ========================
    static void TestChiTietFormViewModelValidation()
    {
        Console.WriteLine("\n--- ChiTietDatPhongFormViewModel validation ---");

        // NgayTra <= NgayNhan → có lỗi
        var vm1 = new ChiTietDatPhongFormViewModel
        {
            MaDatPhong = 1, MaPhong = 1,
            NgayNhan = new DateTime(2024, 5, 10), NgayTra = new DateTime(2024, 5, 10)
        };
        var errors1 = vm1.Validate(new System.ComponentModel.DataAnnotations.ValidationContext(vm1)).ToList();
        Check(errors1.Count == 1, "NgayTra == NgayNhan → lỗi validation");

        var vm2 = new ChiTietDatPhongFormViewModel
        {
            MaDatPhong = 1, MaPhong = 1,
            NgayNhan = new DateTime(2024, 5, 10), NgayTra = new DateTime(2024, 5, 8)
        };
        var errors2 = vm2.Validate(new System.ComponentModel.DataAnnotations.ValidationContext(vm2)).ToList();
        Check(errors2.Count == 1, "NgayTra < NgayNhan → lỗi validation");

        // NgayTra > NgayNhan → OK
        var vm3 = new ChiTietDatPhongFormViewModel
        {
            MaDatPhong = 1, MaPhong = 1,
            NgayNhan = new DateTime(2024, 5, 10), NgayTra = new DateTime(2024, 5, 12)
        };
        var errors3 = vm3.Validate(new System.ComponentModel.DataAnnotations.ValidationContext(vm3)).ToList();
        Check(errors3.Count == 0, "NgayTra > NgayNhan → không lỗi");

        // Null dates → bỏ qua validation custom
        var vm4 = new ChiTietDatPhongFormViewModel { MaDatPhong = 1, MaPhong = 1 };
        var errors4 = vm4.Validate(new System.ComponentModel.DataAnnotations.ValidationContext(vm4)).ToList();
        Check(errors4.Count == 0, "NgayNhan/NgayTra null → không lỗi custom validation");
    }

    // ======================== KetQuaDatPhongViewModel ========================
    static void TestKetQuaDatPhongViewModel()
    {
        Console.WriteLine("\n--- KetQuaDatPhongViewModel ---");

       var dp = new DatPhong { TrangThai = TrangThaiDatPhong.DaTraPhong, TienCoc = 200_000m, TienCocDaXacNhan = true };
        var chiTiets = new List<ChiTietDatPhong>
        {
            new() { ThanhTien = 500_000m, TrangThai = TrangThaiChiTiet.DaTra },
            new() { ThanhTien = 300_000m, TrangThai = TrangThaiChiTiet.DaTra },
            new() { ThanhTien = 100_000m, TrangThai = TrangThaiChiTiet.DaHuy },  // đã hủy → không tính
        };

        var vm = new KetQuaDatPhongViewModel { DatPhong = dp, ChiTiets = chiTiets };

        Check(vm.TongTien == 800_000m, "TongTien chỉ tính chi tiết chưa hủy (500k + 300k)");
        Check(vm.ConPhaiThu == 600_000m, "ConPhaiThu = TongTien - TienCoc (800k - 200k)");
        Check(vm.DaHoanTat, "DaHoanTat = true khi TrangThai = DaTraPhong");

        // Chưa hoàn thành
        dp.TrangThai = TrangThaiDatPhong.DaNhanPhong;
        Check(!vm.DaHoanTat, "DaHoanTat = false khi TrangThai != DaTraPhong");
    }

    // ======================== TrangThaiDatPhong constants ========================
    static void TestTrangThaiConstants()
    {
        Console.WriteLine("\n--- TrangThaiDatPhong / TrangThaiChiTiet constants ---");

        // Aliases
        Check(TrangThaiDatPhong.ChoXuLy == TrangThaiDatPhong.ChoDuyet, "ChoXuLy == ChoDuyet");
        Check(TrangThaiDatPhong.DangXuLy == TrangThaiDatPhong.DaDuyet, "DangXuLy == DaDuyet");
        Check(TrangThaiDatPhong.HoanThanh == TrangThaiDatPhong.DaTraPhong, "HoanThanh == DaTraPhong");
        Check(TrangThaiDatPhong.DaHuy == TrangThaiDatPhong.Huy, "DaHuy == Huy");

        // Distinct values
        var trangThais = new HashSet<string>
        {
            TrangThaiDatPhong.ChoDuyet, TrangThaiDatPhong.DaDuyet,
            TrangThaiDatPhong.DaNhanPhong, TrangThaiDatPhong.DaTraPhong,
            TrangThaiDatPhong.Huy, TrangThaiDatPhong.TuChoi
        };
        Check(trangThais.Count == 6, "6 trạng thái đặt phòng khác nhau");

        // ChiTiet
Check(TrangThaiChiTiet.ChuaNhan == TrangThaiChiTietDatPhong.DaDat, "TrangThaiChiTiet.ChuaNhan");
Check(TrangThaiChiTiet.DangO == TrangThaiChiTietDatPhong.DangSuDung, "TrangThaiChiTiet.DangO");
Check(TrangThaiChiTiet.DaTra == TrangThaiChiTietDatPhong.DaTraPhong, "TrangThaiChiTiet.DaTra");
Check(TrangThaiChiTiet.DaHuy == TrangThaiChiTietDatPhong.Huy, "TrangThaiChiTiet.DaHuy");
    }
}
