// Họ và tên: Nguyễn Đình Tuấn
// Mã sinh viên: 23103100235
// Nội dung thực hiện: Entity hồ sơ khách hàng liên kết tài khoản đăng nhập.
using System.ComponentModel.DataAnnotations;
namespace QuanLy_15_UNETI15_TI17A4HN.Models;
public class KhachHang
{
    [Key] public int MaKhachHang { get; set; }
    [Display(Name="Tài khoản")]
    public int? MaTaiKhoan { get; set; }
    public TaiKhoan? TaiKhoan { get; set; }
    [Required, StringLength(100), Display(Name="Họ tên")]
    public string HoTen { get; set; } = string.Empty;
    [DataType(DataType.Date), Display(Name="Ngày sinh")]
    public DateTime? NgaySinh { get; set; }
    [StringLength(20), Display(Name="Giới tính")]
    public string? GioiTinh { get; set; }
    [StringLength(20), Display(Name="CCCD")]
    public string? CCCD { get; set; }
    [Phone, StringLength(20), Display(Name="Số điện thoại")]
    public string? SoDienThoai { get; set; }
    [EmailAddress, StringLength(100)]
    public string? Email { get; set; }
    [StringLength(200), Display(Name="Địa chỉ")]
    public string? DiaChi { get; set; }
    [StringLength(100), Display(Name="Quốc tịch")]
    public string? QuocTich { get; set; }
    [Display(Name="Hoạt động")]
    public bool TrangThai { get; set; } = true;
    [StringLength(500)] public string? GhiChu { get; set; }
    public ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();
}
