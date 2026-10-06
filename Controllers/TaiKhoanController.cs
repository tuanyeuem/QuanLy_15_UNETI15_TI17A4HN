// Họ và tên: Lê Văn Thanh
// Mã sinh viên: 23103100200
// Nội dung thực hiện: Module 5 - Đăng nhập, đăng xuất, lưu Session và trang không có quyền (phân quyền cho các chức năng Module 5).
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLy_15_UNETI15_TI17A4HN.Data;
using QuanLy_15_UNETI15_TI17A4HN.Helpers;
using QuanLy_15_UNETI15_TI17A4HN.ViewModels;

namespace QuanLy_15_UNETI15_TI17A4HN.Controllers;

public class TaiKhoanController : Controller
{
    private readonly AppDbContext _context;

    public TaiKhoanController(AppDbContext context) => _context = context;

    [HttpGet]
    public IActionResult DangNhap(string? returnUrl = null)
    {
        if (HttpContext.Session.GetInt32("MaTaiKhoan") != null)
            return RedirectToAction("Index", "Home");

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DangNhap(DangNhapViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(model);

        var tk = await _context.TaiKhoans
            .FirstOrDefaultAsync(t => t.TenDangNhap == model.TenDangNhap);

        if (tk == null || !MatKhauHelper.KiemTraMatKhau(model.MatKhau, tk.MatKhau))
        {
            ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
            return View(model);
        }

        if (!tk.TrangThai)
        {
            ModelState.AddModelError(string.Empty, "Tài khoản đã bị khóa, vui lòng liên hệ quản trị viên.");
            return View(model);
        }

        HttpContext.Session.SetInt32("MaTaiKhoan", tk.MaTaiKhoan);
        HttpContext.Session.SetString("HoTen", tk.HoTen);
        HttpContext.Session.SetString("VaiTro", tk.VaiTro);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DangXuat()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(DangNhap));
    }

    [HttpGet]
    public IActionResult KhongCoQuyen() => View();
}