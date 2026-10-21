using System.ComponentModel.DataAnnotations;

namespace QuanLy_15_UNETI15_TI17A4HN.Models;

public class KhachHang
{
    [Key]
    public int MaKhachHang { get; set; }

    [Display(Name = "Tài khoản")]
    public int? MaTaiKhoan { get; set; }
    public TaiKhoan? TaiKhoan { get; set; }

    [Required, StringLength(100), Display(Name = "Họ tên")]
    public string HoTen { get; set; } = string.Empty;

    [DataType(DataType.Date), Display(Name = "Ngày sinh")]
    public DateTime? NgaySinh { get; set; }

    [StringLength(10), Display(Name = "Giới tính")]
    public string? GioiTinh { get; set; }

    [Required, StringLength(12, MinimumLength = 9), Display(Name = "CCCD")]
    public string CCCD { get; set; } = string.Empty;

    [Phone, StringLength(15), Display(Name = "Số điện thoại")]
    public string? SoDienThoai { get; set; }

    [EmailAddress, StringLength(100)]
    public string? Email { get; set; }

    [StringLength(200), Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }

    [StringLength(50), Display(Name = "Quốc tịch")]
    public string? QuocTich { get; set; } = "Việt Nam";

    [Display(Name = "Hoạt động")]
    public bool TrangThai { get; set; } = true;

    [StringLength(500), Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    public ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();
}
