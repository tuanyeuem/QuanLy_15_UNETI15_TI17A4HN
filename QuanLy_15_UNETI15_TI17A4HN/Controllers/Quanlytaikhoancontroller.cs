// Họ và tên: Nguyễn Đình Tuấn
// Mã sinh viên: 23103100235
// Nội dung thực hiện: Admin quản lý tài khoản: danh sách, tìm kiếm, lọc, thêm, sửa, khóa/mở khóa, đặt lại mật khẩu.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLy_15_UNETI15_TI17A4HN.Data;
using QuanLy_15_UNETI15_TI17A4HN.Helpers;
using QuanLy_15_UNETI15_TI17A4HN.Models;
using QuanLy_15_UNETI15_TI17A4HN.ViewModels;

namespace QuanLy_15_UNETI15_TI17A4HN.Controllers;

[PhanQuyen(VaiTroTaiKhoan.Admin)]
public class QuanLyTaiKhoanController : Controller
{
    private static readonly string[] CacVaiTro =
    {
        VaiTroTaiKhoan.Admin, VaiTroTaiKhoan.NhanVien, VaiTroTaiKhoan.KhachHang
    };

    private readonly AppDbContext _context;

    public QuanLyTaiKhoanController(AppDbContext context) => _context = context;

    // Danh sách + tìm kiếm + lọc theo vai trò, trạng thái
    public async Task<IActionResult> Index(string? tuKhoa, string? vaiTro, string? trangThai)
    {
        var query = _context.TaiKhoans.AsQueryable();

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var k = tuKhoa.Trim();
            query = query.Where(t => t.TenDangNhap.Contains(k) || t.HoTen.Contains(k) || t.Email.Contains(k));
        }
        if (!string.IsNullOrEmpty(vaiTro) && CacVaiTro.Contains(vaiTro))
            query = query.Where(t => t.VaiTro == vaiTro);
        if (trangThai == "hoatdong") query = query.Where(t => t.TrangThai);
        if (trangThai == "khoa") query = query.Where(t => !t.TrangThai);

        ViewData["TuKhoa"] = tuKhoa;
        ViewData["VaiTro"] = vaiTro;
        ViewData["TrangThai"] = trangThai;
        ViewData["MaDangDangNhap"] = HttpContext.Session.GetInt32("MaTaiKhoan");

        return View(await query.OrderBy(t => t.VaiTro).ThenBy(t => t.TenDangNhap).ToListAsync());
    }

    [HttpGet]
    public IActionResult Create() => View(new TaiKhoanTaoViewModel { VaiTro = VaiTroTaiKhoan.NhanVien });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaiKhoanTaoViewModel model)
    {
        model.TenDangNhap = model.TenDangNhap?.Trim() ?? string.Empty;
        model.Email = model.Email?.Trim() ?? string.Empty;

        if (!CacVaiTro.Contains(model.VaiTro))
            ModelState.AddModelError(nameof(model.VaiTro), "Vai trò không hợp lệ.");

        if (await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == model.TenDangNhap))
            ModelState.AddModelError(nameof(model.TenDangNhap), "Tên đăng nhập đã tồn tại.");

        if (await _context.TaiKhoans.AnyAsync(t => t.Email == model.Email))
            ModelState.AddModelError(nameof(model.Email), "Email đã được sử dụng.");

        if (!ModelState.IsValid) return View(model);

        _context.TaiKhoans.Add(new TaiKhoan
        {
            TenDangNhap = model.TenDangNhap,
            MatKhau = MatKhauHelper.BamMatKhau(model.MatKhau),
            HoTen = model.HoTen.Trim(),
            Email = model.Email,
            VaiTro = model.VaiTro,
            TrangThai = model.TrangThai
        });
        await _context.SaveChangesAsync();

        TempData["ThongBao"] = $"Đã thêm tài khoản \"{model.TenDangNhap}\".";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var tk = await _context.TaiKhoans.FindAsync(id);
        if (tk == null) return NotFound();

        ViewData["LaChinhMinh"] = tk.MaTaiKhoan == HttpContext.Session.GetInt32("MaTaiKhoan");
        return View(new TaiKhoanSuaViewModel
        {
            MaTaiKhoan = tk.MaTaiKhoan,
            TenDangNhap = tk.TenDangNhap,
            HoTen = tk.HoTen,
            Email = tk.Email,
            VaiTro = tk.VaiTro,
            TrangThai = tk.TrangThai
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TaiKhoanSuaViewModel model)
    {
        if (id != model.MaTaiKhoan) return NotFound();
        var tk = await _context.TaiKhoans.FindAsync(id);
        if (tk == null) return NotFound();

        model.TenDangNhap = tk.TenDangNhap; // tên đăng nhập không cho đổi
        model.Email = model.Email?.Trim() ?? string.Empty;
        var laChinhMinh = tk.MaTaiKhoan == HttpContext.Session.GetInt32("MaTaiKhoan");
        ViewData["LaChinhMinh"] = laChinhMinh;

        // Admin không được tự đổi vai trò hoặc tự khóa chính mình
        if (laChinhMinh)
        {
            model.VaiTro = tk.VaiTro;
            model.TrangThai = tk.TrangThai;
        }

        if (!CacVaiTro.Contains(model.VaiTro))
            ModelState.AddModelError(nameof(model.VaiTro), "Vai trò không hợp lệ.");

        if (await _context.TaiKhoans.AnyAsync(t => t.Email == model.Email && t.MaTaiKhoan != id))
            ModelState.AddModelError(nameof(model.Email), "Email đã được sử dụng.");

        // Phải còn ít nhất 1 Admin đang hoạt động
        var matAdmin = tk.VaiTro == VaiTroTaiKhoan.Admin && tk.TrangThai
                        && (model.VaiTro != VaiTroTaiKhoan.Admin || !model.TrangThai);
        if (matAdmin && await LaAdminHoatDongCuoi(id))
            ModelState.AddModelError(string.Empty, "Không thể thay đổi: đây là Admin hoạt động cuối cùng của hệ thống.");

        if (!ModelState.IsValid) return View(model);

        tk.HoTen = model.HoTen.Trim();
        tk.Email = model.Email;
        tk.VaiTro = model.VaiTro;
        tk.TrangThai = model.TrangThai;
        await _context.SaveChangesAsync();

        TempData["ThongBao"] = $"Đã cập nhật tài khoản \"{tk.TenDangNhap}\".";
        return RedirectToAction(nameof(Index));
    }

    // Khóa / mở khóa nhanh
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoiTrangThai(int id)
    {
        var tk = await _context.TaiKhoans.FindAsync(id);
        if (tk == null) return NotFound();

        if (tk.MaTaiKhoan == HttpContext.Session.GetInt32("MaTaiKhoan"))
        {
            TempData["Loi"] = "Bạn không thể tự khóa tài khoản đang đăng nhập.";
            return RedirectToAction(nameof(Index));
        }

        if (tk.TrangThai && tk.VaiTro == VaiTroTaiKhoan.Admin && await LaAdminHoatDongCuoi(id))
        {
            TempData["Loi"] = "Không thể khóa Admin hoạt động cuối cùng của hệ thống.";
            return RedirectToAction(nameof(Index));
        }

        tk.TrangThai = !tk.TrangThai;
        await _context.SaveChangesAsync();

        TempData["ThongBao"] = tk.TrangThai
            ? $"Đã mở khóa tài khoản \"{tk.TenDangNhap}\"."
            : $"Đã khóa tài khoản \"{tk.TenDangNhap}\".";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> DatLaiMatKhau(int? id)
    {
        if (id == null) return NotFound();
        var tk = await _context.TaiKhoans.FindAsync(id);
        if (tk == null) return NotFound();

        return View(new DatLaiMatKhauViewModel { MaTaiKhoan = tk.MaTaiKhoan, TenDangNhap = tk.TenDangNhap });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DatLaiMatKhau(int id, DatLaiMatKhauViewModel model)
    {
        if (id != model.MaTaiKhoan) return NotFound();
        var tk = await _context.TaiKhoans.FindAsync(id);
        if (tk == null) return NotFound();

        model.TenDangNhap = tk.TenDangNhap;
        if (!ModelState.IsValid) return View(model);

        tk.MatKhau = MatKhauHelper.BamMatKhau(model.MatKhauMoi);
        await _context.SaveChangesAsync();

        TempData["ThongBao"] = $"Đã đặt lại mật khẩu cho \"{tk.TenDangNhap}\".";
        return RedirectToAction(nameof(Index));
    }

    // true nếu ngoài tài khoản này ra không còn Admin nào đang hoạt động
    private async Task<bool> LaAdminHoatDongCuoi(int maTaiKhoan) =>
        !await _context.TaiKhoans.AnyAsync(t =>
            t.MaTaiKhoan != maTaiKhoan && t.VaiTro == VaiTroTaiKhoan.Admin && t.TrangThai);
}