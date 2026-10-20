// Họ và tên: Vũ Xuân Tuấn
// Mã sinh viên: 23103100035
// Nội dung thực hiện: Models DatPhong




using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLy_15_UNETI15_TI17A4HN.Models;

public class DatPhong
{
    [Key]
    public int MaDatPhong { get; set; }

    [Required]
    [Display(Name = "Khách hàng")]
    public int MaKhachHang { get; set; }

    [Required]
    [Display(Name = "Phòng")]
    public int MaPhong { get; set; }

    [Display(Name = "Ngày đặt")]
    public DateTime NgayDat { get; set; } = DateTime.Now;

    [Required]
    [Display(Name = "Ngày nhận (dự kiến)")]
    public DateTime NgayNhan { get; set; }

    [Required]
    [Display(Name = "Ngày trả (dự kiến)")]
    public DateTime NgayTraDuKien { get; set; }

    // ─── 3 trường Module 4 quản lý ───
    [Display(Name = "Ngày nhận thực tế")]
    public DateTime? NgayNhanThucTe { get; set; }

    [Display(Name = "Ngày trả thực tế")]
    public DateTime? NgayTraThucTe { get; set; }

    [StringLength(500)]
    [Display(Name = "Lý do hủy")]
    public string? LyDoHuy { get; set; }
    // ──────────────────────────────────

    [Range(1, 20)]
    [Display(Name = "Số người")]
    public int SoNguoi { get; set; } = 1;

    [Range(0, double.MaxValue)]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Tiền cọc")]
    public decimal TienCoc { get; set; }

    // TrangThai: "ChoDuyet", "DaDuyet", "DaNhanPhong", "DaTraPhong", "Huy"
    [Required]
    [StringLength(30)]
    [Display(Name = "Trạng thái")]
    public string TrangThai { get; set; } = "ChoDuyet";

    [StringLength(500)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    // Navigation
    public KhachHang KhachHang { get; set; } = null!;
    public Phong Phong { get; set; } = null!;
    public ICollection<ChiTietDatPhong> ChiTietDatPhongs { get; set; } = new List<ChiTietDatPhong>();
}