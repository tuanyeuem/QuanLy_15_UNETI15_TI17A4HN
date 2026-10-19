<<<<<<< HEAD:QuanLy_15_UNETI15_TI17A4HN/Models/KhachHang.cs
// Họ và tên: Vũ Xuân Tuấn
// Mã sinh viên: 23103100035
// Nội dung thực hiện: Models KhachHang


=======
// Họ và tên: Lê Văn Thanh
// Mã sinh viên: 23103100200
// Nội dung thực hiện: Module 5 - Entity KhachHang (nền tảng) liên kết khóa ngoại, xem kết quả của khách.
// Entity nền tảng (khung tối thiểu theo đề) để Module 5 liên kết khóa ngoại.
// Module 3 (Quản lý khách hàng, hồ sơ cá nhân) sẽ hoàn thiện thêm CRUD/validation.
>>>>>>> origin/md5:Models/KhachHang.cs
using System.ComponentModel.DataAnnotations;

namespace QuanLy_15_UNETI15_TI17A4HN.Models;

public class KhachHang
{
    [Key]
    public int MaKhachHang { get; set; }

<<<<<<< HEAD:QuanLy_15_UNETI15_TI17A4HN/Models/KhachHang.cs
    public int? MaTaiKhoan { get; set; }
=======
    [Display(Name = "Tài khoản")]
    public int? MaTaiKhoan { get; set; }
    public TaiKhoan? TaiKhoan { get; set; }
>>>>>>> origin/md5:Models/KhachHang.cs

    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [StringLength(100)]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = string.Empty;

<<<<<<< HEAD:QuanLy_15_UNETI15_TI17A4HN/Models/KhachHang.cs
=======
    [DataType(DataType.Date)]
>>>>>>> origin/md5:Models/KhachHang.cs
    [Display(Name = "Ngày sinh")]
    public DateTime? NgaySinh { get; set; }

    [StringLength(10)]
    [Display(Name = "Giới tính")]
    public string? GioiTinh { get; set; }

<<<<<<< HEAD:QuanLy_15_UNETI15_TI17A4HN/Models/KhachHang.cs
    [StringLength(20)]
    [Display(Name = "CCCD")]
    public string? CCCD { get; set; }

    [Phone]
=======
    [Required(ErrorMessage = "CCCD là bắt buộc")]
    [StringLength(12, MinimumLength = 9, ErrorMessage = "CCCD gồm 9-12 ký tự")]
    public string CCCD { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
>>>>>>> origin/md5:Models/KhachHang.cs
    [StringLength(15)]
    [Display(Name = "Số điện thoại")]
    public string? SoDienThoai { get; set; }

<<<<<<< HEAD:QuanLy_15_UNETI15_TI17A4HN/Models/KhachHang.cs
    [EmailAddress]
    [StringLength(100)]
    [Display(Name = "Email")]
=======
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [StringLength(100)]
>>>>>>> origin/md5:Models/KhachHang.cs
    public string? Email { get; set; }

    [StringLength(200)]
    [Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }

    [StringLength(50)]
    [Display(Name = "Quốc tịch")]
<<<<<<< HEAD:QuanLy_15_UNETI15_TI17A4HN/Models/KhachHang.cs
    public string? QuocTich { get; set; } = "Việt Nam";
=======
    public string? QuocTich { get; set; }
>>>>>>> origin/md5:Models/KhachHang.cs

    [Display(Name = "Hoạt động")]
    public bool TrangThai { get; set; } = true;

    [StringLength(500)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

<<<<<<< HEAD:QuanLy_15_UNETI15_TI17A4HN/Models/KhachHang.cs
    // Navigation
    public TaiKhoan? TaiKhoan { get; set; }
    public ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();
}
=======
    public ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();
}
>>>>>>> origin/md5:Models/KhachHang.cs
