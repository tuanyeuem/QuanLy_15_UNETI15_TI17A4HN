// Họ và tên: Lê Văn Thanh
// Mã sinh viên: 23103100200
// Nội dung thực hiện: Module 5 - Entity Phong (nền tảng) liên kết khóa ngoại với ChiTietDatPhong.
// Entity nền tảng (khung tối thiểu theo đề) để Module 5 liên kết khóa ngoại.
// Module 2 (Quản lý phòng) sẽ hoàn thiện thêm CRUD/tìm kiếm/lọc/phân trang trên entity này.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLy_15_UNETI15_TI17A4HN.Models;

public static class TrangThaiPhong
{
    public const string SanSang = "Sẵn sàng";
    public const string DangSuDung = "Đang sử dụng";
    public const string BaoTri = "Bảo trì";

    public static readonly string[] TatCa = { SanSang, DangSuDung, BaoTri };
}

public class Phong
{
    [Key]
    public int MaPhong { get; set; }

    [Required(ErrorMessage = "Số phòng là bắt buộc")]
    [StringLength(10)]
    [Display(Name = "Số phòng")]
    public string SoPhong { get; set; } = string.Empty;

    [Display(Name = "Loại phòng")]
    public int MaLoaiPhong { get; set; }
    public LoaiPhong LoaiPhong { get; set; } = null!;

    [Range(0, 100, ErrorMessage = "Tầng không hợp lệ")]
    [Display(Name = "Tầng")]
    public int Tang { get; set; }

    [StringLength(50)]
    [Display(Name = "Hướng phòng")]
    public string? HuongPhong { get; set; }

    [StringLength(500)]
    [Display(Name = "Tiện nghi")]
    public string? TienNghi { get; set; }

    [Range(0, 1_000_000_000, ErrorMessage = "Đơn giá phải lớn hơn hoặc bằng 0")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Đơn giá")]
    public decimal DonGia { get; set; }

    [Required]
    [StringLength(30)]
    [Display(Name = "Trạng thái")]
    public string TrangThai { get; set; } = TrangThaiPhong.SanSang;

    [StringLength(500)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày bảo trì")]
    public DateTime? NgayBaoTri { get; set; }

    [StringLength(500)]
    [Display(Name = "Ghi chú bảo trì")]
    public string? GhiChuBaoTri { get; set; }

    public ICollection<ChiTietDatPhong> ChiTietDatPhongs { get; set; } = new List<ChiTietDatPhong>();
}
