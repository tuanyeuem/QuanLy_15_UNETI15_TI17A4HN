// Họ và tên: Lê Văn Thanh
// Mã sinh viên: 23103100200
// Nội dung thực hiện: Module 5 - ViewModel cho form đăng nhập.
using System.ComponentModel.DataAnnotations;

namespace QuanLy_15_UNETI15_TI17A4HN.ViewModels;

public class DangNhapViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
    [Display(Name = "Tên đăng nhập")]
    public string TenDangNhap { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string MatKhau { get; set; } = string.Empty;
}