// Họ và tên: Thành viên Module 2 (Git: tkv261005)
// Mã sinh viên: (bổ sung MSSV khi nộp bài)
// Nội dung thực hiện: Module 2 - Xác định đường dẫn ảnh phòng và ảnh minh họa mặc định.
using Microsoft.AspNetCore.Hosting;
using QuanLy_15_UNETI15_TI17A4HN.Models;

namespace QuanLy_15_UNETI15_TI17A4HN.Helpers;

// Quản lý ảnh phòng bằng tệp trong wwwroot (không cần thay đổi cơ sở dữ liệu).
//  - Ảnh riêng của phòng: wwwroot/uploads/phong/phong-{MaPhong}.jpg|png|webp
//  - Chưa có ảnh riêng: dùng ảnh minh họa mặc định theo loại phòng (wwwroot/images/phong/*.svg).
public static class PhongAnhHelper
{
    public const string ThuMucUrl = "/uploads/phong";
    public const long KichThuocToiDa = 5 * 1024 * 1024;
    public static readonly string[] DuoiChoPhep = { ".jpg", ".png", ".webp" };

    private static readonly string[] AnhTheoThuTu = { "don", "doi", "deluxe", "family", "suite" };

    public static string ThuMucVatLy(IWebHostEnvironment env)
    {
        var goc = string.IsNullOrEmpty(env.WebRootPath)
            ? Path.Combine(env.ContentRootPath, "wwwroot")
            : env.WebRootPath;
        return Path.Combine(goc, "uploads", "phong");
    }

    // Đường dẫn ảnh riêng (kèm dấu thời gian để trình duyệt tải lại khi đổi ảnh), hoặc null nếu chưa có.
    public static string? AnhRieng(IWebHostEnvironment env, int maPhong)
    {
        var thuMuc = ThuMucVatLy(env);
        foreach (var duoi in DuoiChoPhep)
        {
            var duongDan = Path.Combine(thuMuc, $"phong-{maPhong}{duoi}");
            if (System.IO.File.Exists(duongDan))
            {
                var dau = System.IO.File.GetLastWriteTimeUtc(duongDan).Ticks;
                return $"{ThuMucUrl}/phong-{maPhong}{duoi}?v={dau}";
            }
        }
        return null;
    }

    public static string AnhMacDinh(Phong phong)
        => AnhMacDinh(phong.LoaiPhong?.TenLoai, phong.MaLoaiPhong);

    // Chọn ảnh mặc định theo tên loại phòng; không nhận ra tên thì dùng mã loại (1..5 theo dữ liệu mẫu).
    public static string AnhMacDinh(string? tenLoai, int maLoaiPhong)
    {
        var ten = tenLoai ?? string.Empty;
        string tep;
        if (ten.Contains("suite", StringComparison.OrdinalIgnoreCase)) tep = "suite";
        else if (ten.Contains("family", StringComparison.OrdinalIgnoreCase)
                 || ten.Contains("gia đình", StringComparison.OrdinalIgnoreCase)) tep = "family";
        else if (ten.Contains("deluxe", StringComparison.OrdinalIgnoreCase)) tep = "deluxe";
        else if (ten.Contains("đôi", StringComparison.OrdinalIgnoreCase)
                 || ten.Contains("double", StringComparison.OrdinalIgnoreCase)
                 || ten.Contains("twin", StringComparison.OrdinalIgnoreCase)) tep = "doi";
        else if (ten.Contains("đơn", StringComparison.OrdinalIgnoreCase)
                 || ten.Contains("single", StringComparison.OrdinalIgnoreCase)) tep = "don";
        else
        {
            var n = AnhTheoThuTu.Length;
            tep = AnhTheoThuTu[(((maLoaiPhong - 1) % n) + n) % n];
        }
        return $"/images/phong/{tep}.svg";
    }

    public static string Url(IWebHostEnvironment env, Phong phong)
        => AnhRieng(env, phong.MaPhong) ?? AnhMacDinh(phong);

    public static void XoaAnh(IWebHostEnvironment env, int maPhong)
    {
        var thuMuc = ThuMucVatLy(env);
        foreach (var duoi in DuoiChoPhep)
        {
            var duongDan = Path.Combine(thuMuc, $"phong-{maPhong}{duoi}");
            if (System.IO.File.Exists(duongDan)) System.IO.File.Delete(duongDan);
        }
    }

    // Nhận dạng định dạng ảnh theo nội dung tệp (không tin tên tệp do người dùng gửi).
    public static string? NhanDangDuoi(Stream luong)
    {
        var dau = new byte[12];
        var doc = 0;
        while (doc < dau.Length)
        {
            var n = luong.Read(dau, doc, dau.Length - doc);
            if (n == 0) break;
            doc += n;
        }
        if (doc >= 3 && dau[0] == 0xFF && dau[1] == 0xD8 && dau[2] == 0xFF) return ".jpg";
        if (doc >= 8 && dau[0] == 0x89 && dau[1] == 0x50 && dau[2] == 0x4E && dau[3] == 0x47
            && dau[4] == 0x0D && dau[5] == 0x0A && dau[6] == 0x1A && dau[7] == 0x0A) return ".png";
        if (doc >= 12 && dau[0] == 0x52 && dau[1] == 0x49 && dau[2] == 0x46 && dau[3] == 0x46
            && dau[8] == 0x57 && dau[9] == 0x45 && dau[10] == 0x42 && dau[11] == 0x50) return ".webp";
        return null;
    }
}
