// Họ và tên: Nguyễn Đình Tuấn
// Mã sinh viên: 23103100235
// Nội dung thực hiện: Attribute kiểm tra đăng nhập và vai trò tại Controller.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLy_15_UNETI15_TI17A4HN.Helpers;

// Dùng: [PhanQuyen] (chỉ cần đăng nhập) hoặc [PhanQuyen("Admin", "NhanVien")]
public class PhanQuyenAttribute : ActionFilterAttribute
{
    private readonly string[] _vaiTros;

    public PhanQuyenAttribute(params string[] vaiTros) => _vaiTros = vaiTros;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        var request = context.HttpContext.Request;

        if (session.GetInt32("MaTaiKhoan") == null)
        {
            context.Result = new RedirectToActionResult("DangNhap", "TaiKhoan",
                new { returnUrl = request.Path + request.QueryString });
            return;
        }

        var vaiTro = session.GetString("VaiTro");
        if (_vaiTros.Length > 0 && !_vaiTros.Contains(vaiTro))
        {
            context.Result = new RedirectToActionResult("KhongCoQuyen", "TaiKhoan", null);
        }
    }
}