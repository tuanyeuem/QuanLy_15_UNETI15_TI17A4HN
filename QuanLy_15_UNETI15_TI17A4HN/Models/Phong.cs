// Họ và tên: Vũ Xuân Tuấn
// Mã sinh viên: 23103100035
// Nội dung thực hiện: Models Phong



using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLy_15_UNETI15_TI17A4HN.Models;

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

    [Display(Name = "Tầng")]
    public int Tang { get; set; }

    [StringLength(50)]
    [Display(Name = "Hướng phòng")]
    public string? HuongPhong { get; set; }

    [StringLength(500)]
    [Display(Name = "Tiện nghi")]
    public string? TienNghi { get; set; }

    [Range(0, double.MaxValue)]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Đơn giá/ngày")]
    public decimal DonGia { get; set; }

    // TrangThai: "TrongRanh", "DangSuDung", "BaoTri", "KhongKinhDoanh"
    [Required]
    [StringLength(30)]
    [Display(Name = "Trạng thái")]
    public string TrangThai { get; set; } = "TrongRanh";

    [StringLength(500)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    [Display(Name = "Ngày bảo trì")]
    public DateTime? NgayBaoTri { get; set; }

    [StringLength(500)]
    [Display(Name = "Ghi chú bảo trì")]
    public string? GhiChuBaoTri { get; set; }

    // Navigation Property
    public LoaiPhong LoaiPhong { get; set; } = null!;
    public ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();
}