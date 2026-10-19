// Họ và tên: Vũ Xuân Tuấn | Mã sinh viên: 23103100035 | Nội dung: Entity Phong ban đầu.
// Họ và tên: Lê Văn Thanh | Mã sinh viên: 23103100200 | Nội dung: Liên kết nền với chi tiết lưu trú (Module 5).
// Họ và tên: Nguyễn Đình Tuấn | Mã sinh viên: 23103100235 | Nội dung: Entity phòng và trạng thái kinh doanh.
// Họ và tên: Nguyễn Đình Quang | Mã sinh viên: 23103100259 | Nội dung: Hợp nhất entity, trạng thái và navigation để các module dùng chung.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLy_15_UNETI15_TI17A4HN.Models;

public static class TrangThaiPhong
{
    public const string TrongRanh = "TrongRanh";
    public const string Trong = "Trong";
    public const string SanSang = "TrongRanh";
    public const string DaDat = "DaDat";
    public const string DangSuDung = "DangSuDung";
    public const string BaoTri = "BaoTri";
    public const string KhongKinhDoanh = "KhongKinhDoanh";
    public const string NgungKinhDoanh = "NgungKinhDoanh";
}

public class Phong
{
    [Key]
    public int MaPhong { get; set; }

    [Required(ErrorMessage = "Số phòng là bắt buộc")]
    [StringLength(10)]
    [Display(Name = "Số phòng")]
    public string SoPhong { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Loại phòng")]
    public int MaLoaiPhong { get; set; }
    public LoaiPhong LoaiPhong { get; set; } = null!;

    [Range(1, 100, ErrorMessage = "Tầng phải từ 1 đến 100")]
    [Display(Name = "Tầng")]
    public int Tang { get; set; }

    [StringLength(50)]
    [Display(Name = "Hướng phòng")]
    public string? HuongPhong { get; set; }

    [StringLength(500)]
    [Display(Name = "Tiện nghi")]
    public string? TienNghi { get; set; }

    [Range(typeof(decimal), "0", "9999999999999999.99")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Đơn giá/ngày")]
    public decimal DonGia { get; set; }

    [Required]
    [StringLength(30)]
    [Display(Name = "Trạng thái")]
    public string TrangThai { get; set; } = TrangThaiPhong.TrongRanh;

    [StringLength(500)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày bảo trì")]
    public DateTime? NgayBaoTri { get; set; }

    [StringLength(500)]
    [Display(Name = "Ghi chú bảo trì")]
    public string? GhiChuBaoTri { get; set; }

    public ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();
    public ICollection<ChiTietDatPhong> ChiTietDatPhongs { get; set; } = new List<ChiTietDatPhong>();
}
