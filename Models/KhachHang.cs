// Họ và tên: Lê Văn Thanh
// Mã sinh viên: 23103100200
// Nội dung thực hiện: Module 5 - Entity KhachHang (nền tảng) liên kết khóa ngoại, xem kết quả của khách.
// Entity nền tảng (khung tối thiểu theo đề) để Module 5 liên kết khóa ngoại.
// Module 3 (Quản lý khách hàng, hồ sơ cá nhân) sẽ hoàn thiện thêm CRUD/validation.
using System.ComponentModel.DataAnnotations;

namespace QuanLy_15_UNETI15_TI17A4HN.Models;

public class KhachHang
{
    [Key]
    public int MaKhachHang { get; set; }

    [Display(Name = "Tài khoản")]
    public int? MaTaiKhoan { get; set; }
    public TaiKhoan? TaiKhoan { get; set; }

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

    [Required(ErrorMessage = "CCCD là bắt buộc")]
    [StringLength(12, MinimumLength = 9, ErrorMessage = "CCCD gồm 9-12 ký tự")]
    public string CCCD { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [StringLength(15)]
    [Display(Name = "Số điện thoại")]
    public string? SoDienThoai { get; set; }

    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(200)]
    [Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }

    [StringLength(50)]
    [Display(Name = "Quốc tịch")]
    public string? QuocTich { get; set; }

    [Display(Name = "Hoạt động")]
    public bool TrangThai { get; set; } = true;

    [StringLength(500)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    public ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();
}
