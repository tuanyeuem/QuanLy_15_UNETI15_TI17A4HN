using System.ComponentModel.DataAnnotations;

namespace QuanLy_15_UNETI15_TI17A4HN.Models;

public class KhachHang
{
    [Key]
    public int MaKhachHang { get; set; }

    [Display(Name = "T?i kho?n")]
    public int? MaTaiKhoan { get; set; }
    public TaiKhoan? TaiKhoan { get; set; }

    [Required, StringLength(100), Display(Name = "H? t?n")]
    public string HoTen { get; set; } = string.Empty;

    [DataType(DataType.Date), Display(Name = "Ng?y sinh")]
    public DateTime? NgaySinh { get; set; }

    [StringLength(10), Display(Name = "Gi?i t?nh")]
    public string? GioiTinh { get; set; }

    [Required, StringLength(12, MinimumLength = 9), Display(Name = "CCCD")]
    public string CCCD { get; set; } = string.Empty;

    [Phone, StringLength(15), Display(Name = "S? ?i?n tho?i")]
    public string? SoDienThoai { get; set; }

    [EmailAddress, StringLength(100)]
    public string? Email { get; set; }

    [StringLength(200), Display(Name = "??a ch?")]
    public string? DiaChi { get; set; }

    [StringLength(50), Display(Name = "Qu?c t?ch")]
    public string? QuocTich { get; set; } = "Vi?t Nam";

    [Display(Name = "Ho?t ??ng")]
    public bool TrangThai { get; set; } = true;

    [StringLength(500), Display(Name = "Ghi ch?")]
    public string? GhiChu { get; set; }

    public ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();
}
