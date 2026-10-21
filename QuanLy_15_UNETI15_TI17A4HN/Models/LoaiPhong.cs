using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLy_15_UNETI15_TI17A4HN.Models;

public class LoaiPhong
{
    [Key]
    public int MaLoaiPhong { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Tên loại phòng")]
    public string TenLoai { get; set; } = string.Empty;

    [Range(1, 20)]
    [Display(Name = "Số người tối đa")]
    public int SoNguoiToiDa { get; set; }

    [Range(0, double.MaxValue)]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Đơn giá/ngày")]
    public decimal DonGiaNgay { get; set; }

    [StringLength(500)]
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [Display(Name = "Hoạt động")]
    public bool TrangThai { get; set; } = true;
}
