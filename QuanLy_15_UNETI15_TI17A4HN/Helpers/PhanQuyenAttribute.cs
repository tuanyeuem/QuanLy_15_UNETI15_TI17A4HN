// Họ và tên: Nguyễn Đình Quang
// Mã sinh viên: 23103100259
// Nội dung thực hiện: Hợp nhất bộ lọc phân quyền, xác nhận Session với tài khoản hoạt động trong DB.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using QuanLy_15_UNETI15_TI17A4HN.Data;

namespace QuanLy_15_UNETI15_TI17A4HN.Helpers;

public sealed class PhanQuyenAttribute : ActionFilterAttribute
{
    private readonly string[] _vaiTros;

    public PhanQuyenAttribute(params string[] vaiTros) => _vaiTros = vaiTros;

    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var http = context.HttpContext;
        var session = http.Session;
        var maTaiKhoan = session.GetInt32("MaTaiKhoan");

        if (!maTaiKhoan.HasValue)
        {
            context.Result = new RedirectToActionResult("DangNhap", "TaiKhoan", new
            {
                returnUrl = http.Request.Path + http.Request.QueryString
            });
            return;
        }

        var db = http.RequestServices.GetRequiredService<AppDbContext>();
        var taiKhoan = await db.TaiKhoans.AsNoTracking()
            .FirstOrDefaultAsync(t => t.MaTaiKhoan == maTaiKhoan.Value);

        if (taiKhoan == null || !taiKhoan.TrangThai)
        {
            session.Clear();
            context.Result = new RedirectToActionResult("DangNhap", "TaiKhoan", null);
            return;
        }

        session.SetString("HoTen", taiKhoan.HoTen);
        session.SetString("VaiTro", taiKhoan.VaiTro);
        if (_vaiTros.Length > 0 && !_vaiTros.Contains(taiKhoan.VaiTro, StringComparer.Ordinal))
        {
            context.Result = new RedirectToActionResult("KhongCoQuyen", "TaiKhoan", null);
            return;
        }

        await next();
    }
}
