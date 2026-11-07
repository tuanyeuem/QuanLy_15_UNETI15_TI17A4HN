// Họ và tên: Nguyễn Đình Tuấn
// Mã sinh viên: 23103100235
// Nội dung thực hiện: ViewModel tạo, sửa và đặt lại mật khẩu tài khoản.
using System.ComponentModel.DataAnnotations;
using QuanLy_15_UNETI15_TI17A4HN.Models;
namespace QuanLy_15_UNETI15_TI17A4HN.ViewModels;
public class TaiKhoanTaoViewModel {
 [Required(ErrorMessage="Tên đăng nhập là bắt buộc"), StringLength(50), Display(Name="Tên đăng nhập")] public string TenDangNhap {get;set;}=string.Empty;
 [Required(ErrorMessage="Mật khẩu là bắt buộc"), StringLength(100,MinimumLength=8,ErrorMessage="Mật khẩu phải từ 8 đến 100 ký tự"), DataType(DataType.Password), Display(Name="Mật khẩu")] public string MatKhau {get;set;}=string.Empty;
 [Required(ErrorMessage="Họ tên là bắt buộc"), StringLength(100), Display(Name="Họ tên")] public string HoTen {get;set;}=string.Empty;
 [Required(ErrorMessage="Email là bắt buộc"), EmailAddress(ErrorMessage="Email không đúng định dạng"), StringLength(100)] public string Email {get;set;}=string.Empty;
 [Required,RegularExpression("^(Admin|NhanVien|KhachHang)$",ErrorMessage="Vai trò không hợp lệ"),Display(Name="Vai trò")] public string VaiTro {get;set;}=VaiTroTaiKhoan.NhanVien;
 [Display(Name="Hoạt động")] public bool TrangThai {get;set;}=true;
}
public class TaiKhoanSuaViewModel {
 public int MaTaiKhoan {get;set;}
 [Display(Name="Tên đăng nhập")] public string TenDangNhap {get;set;}=string.Empty;
 [Required(ErrorMessage="Họ tên là bắt buộc"),StringLength(100),Display(Name="Họ tên")] public string HoTen {get;set;}=string.Empty;
 [Required(ErrorMessage="Email là bắt buộc"),EmailAddress(ErrorMessage="Email không đúng định dạng"),StringLength(100)] public string Email {get;set;}=string.Empty;
 [Required,RegularExpression("^(Admin|NhanVien|KhachHang)$",ErrorMessage="Vai trò không hợp lệ"),Display(Name="Vai trò")] public string VaiTro {get;set;}=VaiTroTaiKhoan.NhanVien;
 [Display(Name="Hoạt động")] public bool TrangThai {get;set;}=true;
}
public class DatLaiMatKhauViewModel {
 public int MaTaiKhoan {get;set;}
 [Display(Name="Tên đăng nhập")] public string TenDangNhap {get;set;}=string.Empty;
 [Required(ErrorMessage="Mật khẩu mới là bắt buộc"),StringLength(100,MinimumLength=8,ErrorMessage="Mật khẩu phải từ 8 đến 100 ký tự"),DataType(DataType.Password),Display(Name="Mật khẩu mới")] public string MatKhauMoi {get;set;}=string.Empty;
}