// Họ và tên: Nguyễn Đình Quang
// Mã sinh viên: 23103100259
// Nội dung thực hiện: Module 3 - Nhân viên/Admin quản lý hồ sơ khách hàng.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using QuanLy_15_UNETI15_TI17A4HN.Data;
using QuanLy_15_UNETI15_TI17A4HN.Helpers;
using QuanLy_15_UNETI15_TI17A4HN.Models;
using QuanLy_15_UNETI15_TI17A4HN.ViewModels;

namespace QuanLy_15_UNETI15_TI17A4HN.Controllers;

[PhanQuyen(VaiTroTaiKhoan.Admin, VaiTroTaiKhoan.NhanVien)]
public class QuanLyKhachHangController : Controller
{
    private const int SoDongMoiTrang = 10;
    private readonly AppDbContext _db;

    public QuanLyKhachHangController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Index(string? tuKhoa, int trang = 1)
    {
        // Chuẩn hóa cả dữ liệu lẫn từ khóa trong .NET để kết quả không phụ thuộc vào collation của SQL Server (đặc biệt với dấu tiếng Việt).
        // Bảng hồ sơ khách hàng nhỏ; lọc trước khi phân trang để số trang chính xác.
        var khachHangs = await _db.KhachHangs.AsNoTracking().ToListAsync();
        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var keyword = ChuanHoaTimKiem(tuKhoa);
            khachHangs = khachHangs.Where(k =>
                new[] { k.HoTen, k.CCCD, k.SoDienThoai, k.Email, k.DiaChi, k.QuocTich, k.GioiTinh, k.GhiChu }
                    .Any(giaTri => !string.IsNullOrEmpty(giaTri) && ChuanHoaTimKiem(giaTri).Contains(keyword)))
                .ToList();
        }

        var tongSoBanGhi = khachHangs.Count;
        var tongSoTrang = Math.Max(1, (int)Math.Ceiling(tongSoBanGhi / (double)SoDongMoiTrang));
        trang = Math.Clamp(trang, 1, tongSoTrang);
        ViewBag.TuKhoa = tuKhoa;
        ViewBag.Trang = trang;
        ViewBag.TongSoTrang = tongSoTrang;
        ViewBag.TongSoBanGhi = tongSoBanGhi;

        var danhSach = khachHangs.OrderBy(k => k.HoTen)
            .Skip((trang - 1) * SoDongMoiTrang).Take(SoDongMoiTrang).ToList();
        return View(danhSach);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var khachHang = await _db.KhachHangs.AsNoTracking()
            .Include(k => k.DatPhongs).ThenInclude(d => d.Phong)
            .FirstOrDefaultAsync(k => k.MaKhachHang == id);
        return khachHang == null ? NotFound() : View(khachHang);
    }

    [HttpGet]
    public IActionResult Create() => View(new KhachHangQuanLyViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(KhachHangQuanLyViewModel model)
    {
        await ValidateKhachHangAsync(model);
        if (!ModelState.IsValid) return View(model);

        var khachHang = new KhachHang { TrangThai = true };
        GhiDuLieu(khachHang, model);
        _db.KhachHangs.Add(khachHang);
        await _db.SaveChangesAsync();
        TempData["ThanhCong"] = "Đã thêm khách hàng.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var khachHang = await _db.KhachHangs.FindAsync(id);
        return khachHang == null ? NotFound() : View(MapQuanLy(khachHang));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, KhachHangQuanLyViewModel model)
    {
        if (id != model.MaKhachHang) return BadRequest();
        var khachHang = await _db.KhachHangs.FirstOrDefaultAsync(k => k.MaKhachHang == id);
        if (khachHang == null) return NotFound();

        await ValidateKhachHangAsync(model, id);
        if (!ModelState.IsValid) return View(model);

        GhiDuLieu(khachHang, model);
        await _db.SaveChangesAsync();
        TempData["ThanhCong"] = "Đã cập nhật khách hàng.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var khachHang = await _db.KhachHangs.AsNoTracking()
            .Include(k => k.DatPhongs)
            .FirstOrDefaultAsync(k => k.MaKhachHang == id);
        return khachHang == null ? NotFound() : View(khachHang);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var khachHang = await _db.KhachHangs
            .Include(k => k.DatPhongs)
            .FirstOrDefaultAsync(k => k.MaKhachHang == id);
        if (khachHang == null) return NotFound();
        if (khachHang.DatPhongs.Count > 0 || khachHang.MaTaiKhoan.HasValue)
        {
            TempData["Loi"] = "Không thể xóa hồ sơ đã liên kết tài khoản hoặc có lịch sử đặt phòng. Hãy chuyển trạng thái hồ sơ sang ngừng hoạt động.";
            return RedirectToAction(nameof(Details), new { id });
        }

        _db.KhachHangs.Remove(khachHang);
        await _db.SaveChangesAsync();
        TempData["ThanhCong"] = "Đã xóa khách hàng.";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateKhachHangAsync(KhachHangQuanLyViewModel model, int? id = null)
    {
        if (model.NgaySinh.HasValue && model.NgaySinh.Value.Date > DateTime.Today)
            ModelState.AddModelError(nameof(model.NgaySinh), "Ngày sinh không thể ở tương lai.");
        var cccd = Clean(model.CCCD);
        if (cccd != null && await _db.KhachHangs.AnyAsync(k =>
                k.CCCD == cccd && (!id.HasValue || k.MaKhachHang != id.Value)))
            ModelState.AddModelError(nameof(model.CCCD), "CCCD đã được sử dụng cho hồ sơ khác.");
    }

    private static void GhiDuLieu(KhachHang target, KhachHangQuanLyViewModel source)
    {
        target.HoTen = source.HoTen.Trim();
        target.NgaySinh = source.NgaySinh?.Date;
        target.GioiTinh = Clean(source.GioiTinh);
        target.CCCD = Clean(source.CCCD);
        target.SoDienThoai = Clean(source.SoDienThoai);
        target.Email = Clean(source.Email);
        target.DiaChi = Clean(source.DiaChi);
        target.QuocTich = Clean(source.QuocTich);
        target.TrangThai = source.TrangThai;
        target.GhiChu = Clean(source.GhiChu);
    }

    private static KhachHangQuanLyViewModel MapQuanLy(KhachHang source) => new()
    {
        MaKhachHang = source.MaKhachHang,
        HoTen = source.HoTen,
        NgaySinh = source.NgaySinh,
        GioiTinh = source.GioiTinh,
        CCCD = source.CCCD,
        SoDienThoai = source.SoDienThoai,
        Email = source.Email,
        DiaChi = source.DiaChi,
        QuocTich = source.QuocTich,
        TrangThai = source.TrangThai,
        GhiChu = source.GhiChu
    };

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string ChuanHoaTimKiem(string value)
    {
        var normalized = value.Trim().Normalize(NormalizationForm.FormD);
        var result = new StringBuilder(normalized.Length);
        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
                result.Append(character == 'đ' ? 'd' : character == 'Đ' ? 'D' : character);
        }

        return result.ToString().Normalize(NormalizationForm.FormC).ToUpperInvariant();
    }
}
