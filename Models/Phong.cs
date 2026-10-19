<<<<<<< HEAD:QuanLy_15_UNETI15_TI17A4HN/Models/Phong.cs
// Họ và tên: Vũ Xuân Tuấn
// Mã sinh viên: 23103100035
// Nội dung thực hiện: Models Phong



=======
// Họ và tên: Lê Văn Thanh
// Mã sinh viên: 23103100200
// Nội dung thực hiện: Module 5 - Entity Phong (nền tảng) liên kết khóa ngoại với ChiTietDatPhong.
// Entity nền tảng (khung tối thiểu theo đề) để Module 5 liên kết khóa ngoại.
// Module 2 (Quản lý phòng) sẽ hoàn thiện thêm CRUD/tìm kiếm/lọc/phân trang trên entity này.
>>>>>>> origin/md5:Models/Phong.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLy_15_UNETI15_TI17A4HN.Models;

<<<<<<< HEAD:QuanLy_15_UNETI15_TI17A4HN/Models/Phong.cs
=======
public static class TrangThaiPhong
{
    public const string SanSang = "Sẵn sàng";
    public const string DangSuDung = "Đang sử dụng";
    public const string BaoTri = "Bảo trì";

    public static readonly string[] TatCa = { SanSang, DangSuDung, BaoTri };
}

>>>>>>> origin/md5:Models/Phong.cs
public class Phong
{
    [Key]
    public int MaPhong { get; set; }

    [Required(ErrorMessage = "Số phòng là bắt buộc")]
    [StringLength(10)]
    [Display(Name = "Số phòng")]
    public string SoPhong { get; set; } = string.Empty;

<<<<<<< HEAD:QuanLy_15_UNETI15_TI17A4HN/Models/Phong.cs
    [Required]
    [Display(Name = "Loại phòng")]
    public int MaLoaiPhong { get; set; }

=======
    [Display(Name = "Loại phòng")]
    public int MaLoaiPhong { get; set; }
    public LoaiPhong LoaiPhong { get; set; } = null!;

    [Range(0, 100, ErrorMessage = "Tầng không hợp lệ")]
>>>>>>> origin/md5:Models/Phong.cs
    [Display(Name = "Tầng")]
    public int Tang { get; set; }

    [StringLength(50)]
    [Display(Name = "Hướng phòng")]
    public string? HuongPhong { get; set; }

    [StringLength(500)]
    [Display(Name = "Tiện nghi")]
    public string? TienNghi { get; set; }

<<<<<<< HEAD:QuanLy_15_UNETI15_TI17A4HN/Models/Phong.cs
    [Range(0, double.MaxValue)]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Đơn giá/ngày")]
    public decimal DonGia { get; set; }

    // TrangThai: "TrongRanh", "DangSuDung", "BaoTri", "KhongKinhDoanh"
    [Required]
    [StringLength(30)]
    [Display(Name = "Trạng thái")]
    public string TrangThai { get; set; } = "TrongRanh";
=======
    [Range(0, 1_000_000_000, ErrorMessage = "Đơn giá phải lớn hơn hoặc bằng 0")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Đơn giá")]
    public decimal DonGia { get; set; }

    [Required]
    [StringLength(30)]
    [Display(Name = "Trạng thái")]
    public string TrangThai { get; set; } = TrangThaiPhong.SanSang;
>>>>>>> origin/md5:Models/Phong.cs

    [StringLength(500)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

<<<<<<< HEAD:QuanLy_15_UNETI15_TI17A4HN/Models/Phong.cs
=======
    [DataType(DataType.Date)]
>>>>>>> origin/md5:Models/Phong.cs
    [Display(Name = "Ngày bảo trì")]
    public DateTime? NgayBaoTri { get; set; }

    [StringLength(500)]
    [Display(Name = "Ghi chú bảo trì")]
    public string? GhiChuBaoTri { get; set; }

<<<<<<< HEAD:QuanLy_15_UNETI15_TI17A4HN/Models/Phong.cs
    // Navigation Property
    public LoaiPhong LoaiPhong { get; set; } = null!;
    public ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();
}
=======
    public ICollection<ChiTietDatPhong> ChiTietDatPhongs { get; set; } = new List<ChiTietDatPhong>();
}
>>>>>>> origin/md5:Models/Phong.cs
