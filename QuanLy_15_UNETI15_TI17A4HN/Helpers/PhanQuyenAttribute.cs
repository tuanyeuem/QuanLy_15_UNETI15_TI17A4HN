// Họ và tên: Nguyễn Đình Tuấn
// Mã sinh viên: 23103100235
// Nội dung thực hiện: Kiểm tra đăng nhập, trạng thái và vai trò tại Controller.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using QuanLy_15_UNETI15_TI17A4HN.Data;
namespace QuanLy_15_UNETI15_TI17A4HN.Helpers;
public class PhanQuyenAttribute : ActionFilterAttribute {
 private readonly string[] _vaiTros;
 public PhanQuyenAttribute(params string[] vaiTros)=>_vaiTros=vaiTros;
 public override async Task OnActionExecutionAsync(ActionExecutingContext context,ActionExecutionDelegate next){
  var http=context.HttpContext;var session=http.Session;var id=session.GetInt32("MaTaiKhoan");
  if(id==null){context.Result=new RedirectToActionResult("DangNhap","TaiKhoan",new{returnUrl=http.Request.Path+http.Request.QueryString});return;}
  var db=http.RequestServices.GetRequiredService<AppDbContext>();
  var tk=await db.TaiKhoans.AsNoTracking().FirstOrDefaultAsync(x=>x.MaTaiKhoan==id.Value);
  if(tk==null||!tk.TrangThai){session.Clear();context.Result=new RedirectToActionResult("DangNhap","TaiKhoan",null);return;}
  session.SetString("HoTen",tk.HoTen);session.SetString("VaiTro",tk.VaiTro);
  if(_vaiTros.Length>0&&!_vaiTros.Contains(tk.VaiTro)){context.Result=new RedirectToActionResult("KhongCoQuyen","TaiKhoan",null);return;}
  await next();
 }
}