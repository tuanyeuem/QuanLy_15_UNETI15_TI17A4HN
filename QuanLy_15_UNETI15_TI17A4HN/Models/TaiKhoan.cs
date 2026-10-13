// Họ và tên: Nguyễn Đình Tuấn
// Mã sinh viên: 23103100235
// Nội dung thực hiện: Entity tài khoản và vai trò hệ thống.
using System.ComponentModel.DataAnnotations;
namespace QuanLy_15_UNETI15_TI17A4HN.Models;
public static class VaiTroTaiKhoan
{
    public const string Admin = "Admin";
    public const string NhanVien = "NhanVien";
    public const string KhachHang = "KhachHang";
}
public class TaiKhoan
{
    [Key] public int MaTaiKhoan { get; set; }
    [Required(ErrorMessage="Tên đăng nhập là bắt buộc"), StringLength(50), Display(Name="Tên đăng nhập")]
    public string TenDangNhap { get; set; } = string.Empty;
    [Required(ErrorMessage="Mật khẩu là bắt buộc"), StringLength(255), Display(Name="Mật khẩu")]
    public string MatKhau { get; set; } = string.Empty;
    [Required(ErrorMessage="Họ tên là bắt buộc"), StringLength(100), Display(Name="Họ tên")]
    public string HoTen { get; set; } = string.Empty;
    [Required(ErrorMessage="Email là bắt buộc"), EmailAddress(ErrorMessage="Email không đúng định dạng"), StringLength(100)]
    public string Email { get; set; } = string.Empty;
    [Required, StringLength(20), Display(Name="Vai trò")]
    public string VaiTro { get; set; } = VaiTroTaiKhoan.KhachHang;
    [Display(Name="Hoạt động")]
    public bool TrangThai { get; set; } = true;
    public KhachHang? KhachHang { get; set; }
}
