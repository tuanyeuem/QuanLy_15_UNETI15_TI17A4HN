// Họ và tên: Nguyễn Đình Quang
// Mã sinh viên: 23103100259
// Nội dung thực hiện: Module 3 - ViewModel cập nhật hồ sơ và tạo đặt phòng.
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLy_15_UNETI15_TI17A4HN.ViewModels;

public class CapNhatHoSoViewModel
{
    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [StringLength(100)]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Ngày sinh")]
    public DateTime? NgaySinh { get; set; }

    [StringLength(10)]
    [Display(Name = "Giới tính")]
    public string? GioiTinh { get; set; }

    [StringLength(20)]
    [RegularExpression(@"^\d{12}$", ErrorMessage = "CCCD phải gồm đúng 12 chữ số")]
    [Display(Name = "CCCD")]
    public string? CCCD { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [StringLength(15)]
    [Display(Name = "Số điện thoại")]
    public string? SoDienThoai { get; set; }

    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [StringLength(100)]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [StringLength(200)]
    [Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }

    [StringLength(50)]
    [Display(Name = "Quốc tịch")]
    public string? QuocTich { get; set; }
}

public class TaoDatPhongViewModel
{
    [Required(ErrorMessage = "Vui lòng chọn phòng")]
    [Display(Name = "Phòng")]
    public int? MaPhong { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày nhận")]
    public DateTime NgayNhan { get; set; } = DateTime.Today.AddDays(1);

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày trả dự kiến")]
    public DateTime NgayTraDuKien { get; set; } = DateTime.Today.AddDays(2);

    [Range(1, 20, ErrorMessage = "Số người phải từ 1 đến 20")]
    [Display(Name = "Số người")]
    public int SoNguoi { get; set; } = 1;

    [Range(typeof(decimal), "0", "9999999999999999.99", ErrorMessage = "Tiền cọc phải lớn hơn hoặc bằng 0")]
    [Display(Name = "Tiền cọc")]
    public decimal TienCoc { get; set; }

    [StringLength(500)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    public List<SelectListItem> DanhSachPhong { get; set; } = new();
}

public class KhachHangQuanLyViewModel
{
    public int MaKhachHang { get; set; }

    [Required(ErrorMessage = "Họ tên là bắt buộc"), StringLength(100)]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = string.Empty;

    [DataType(DataType.Date), Display(Name = "Ngày sinh")]
    public DateTime? NgaySinh { get; set; }

    [StringLength(10), Display(Name = "Giới tính")]
    public string? GioiTinh { get; set; }

    [StringLength(20), Display(Name = "CCCD")]
    [RegularExpression(@"^\d{12}$", ErrorMessage = "CCCD phải gồm đúng 12 chữ số")]
    public string? CCCD { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ"), StringLength(15)]
    [Display(Name = "Số điện thoại")]
    public string? SoDienThoai { get; set; }

    [EmailAddress(ErrorMessage = "Email không đúng định dạng"), StringLength(100)]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [StringLength(200), Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }

    [StringLength(50), Display(Name = "Quốc tịch")]
    public string? QuocTich { get; set; }

    [Display(Name = "Hoạt động")]
    public bool TrangThai { get; set; } = true;

    [StringLength(500), Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }
}
