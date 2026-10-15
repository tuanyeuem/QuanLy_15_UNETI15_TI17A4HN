// Họ và tên: Vũ Xuân Tuấn
// Mã sinh viên: 23103100035
// Nội dung thực hiện: Models KhachHang


using System.ComponentModel.DataAnnotations;

namespace QuanLy_15_UNETI15_TI17A4HN.Models;

public class KhachHang
{
    [Key]
    public int MaKhachHang { get; set; }

    public int? MaTaiKhoan { get; set; }

    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [StringLength(100)]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = string.Empty;

    [Display(Name = "Ngày sinh")]
    public DateTime? NgaySinh { get; set; }

    [StringLength(10)]
    [Display(Name = "Giới tính")]
    public string? GioiTinh { get; set; }

    [StringLength(20)]
    [Display(Name = "CCCD")]
    public string? CCCD { get; set; }

    [Phone]
    [StringLength(15)]
    [Display(Name = "Số điện thoại")]
    public string? SoDienThoai { get; set; }

    [EmailAddress]
    [StringLength(100)]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [StringLength(200)]
    [Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }

    [StringLength(50)]
    [Display(Name = "Quốc tịch")]
    public string? QuocTich { get; set; } = "Việt Nam";

    [Display(Name = "Hoạt động")]
    public bool TrangThai { get; set; } = true;

    [StringLength(500)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    // Navigation
    public TaiKhoan? TaiKhoan { get; set; }
    public ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();
}