// Họ và tên: Nguyễn Đình Quang
// Mã sinh viên: 23103100259
// Nội dung thực hiện: Module 3 - Entity đặt phòng, liên kết hồ sơ khách hàng và phòng.
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
    public KhachHang KhachHang { get; set; } = null!;

    [Required]
    [Display(Name = "Phòng")]
    // Phòng chính cho luồng đặt/tiếp nhận; mọi phòng và tiền lưu trú được lưu ở ChiTietDatPhongs.
    public int MaPhong { get; set; }
    public Phong Phong { get; set; } = null!;

    [Display(Name = "Ngày đặt")]
    public DateTime NgayDat { get; set; } = DateTime.Now;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày nhận (dự kiến)")]
    public DateTime NgayNhan { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày trả (dự kiến)")]
    public DateTime NgayTraDuKien { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "Ngày nhận thực tế")]
    public DateTime? NgayNhanThucTe { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "Ngày trả thực tế")]
    public DateTime? NgayTraThucTe { get; set; }

    [StringLength(500)]
    [Display(Name = "Lý do hủy")]
    public string? LyDoHuy { get; set; }

    [Range(1, 20, ErrorMessage = "Số người phải từ 1 đến 20")]
    [Display(Name = "Số người")]
    public int SoNguoi { get; set; } = 1;

    [Range(typeof(decimal), "0", "9999999999999999.99", ErrorMessage = "Tiền cọc phải lớn hơn hoặc bằng 0")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Tiền cọc")]
    public decimal TienCoc { get; set; }

    [Required]
    [StringLength(30)]
    [Display(Name = "Trạng thái")]
    public string TrangThai { get; set; } = TrangThaiDatPhong.ChoDuyet;

    [StringLength(500)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    public ICollection<ChiTietDatPhong> ChiTietDatPhongs { get; set; } = new List<ChiTietDatPhong>();
}
