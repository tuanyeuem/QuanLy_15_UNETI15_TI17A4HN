
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLy_15_UNETI15_TI17A4HN.Models;

public class LoaiPhong
{
    [Key]
    public string MaLoaiPhong { get; set; }

    [Required(ErrorMessage = "Tên loại phòng là bắt buộc")]
    [StringLength(100)]
    [Display(Name = "Tên loại phòng")]
    public string TenLoai { get; set; } = string.Empty;

    [Range(1, 20, ErrorMessage = "Số người tối đa từ 1 đến 20")]
    [Display(Name = "Số người tối đa")]
    public int SoNguoiToiDa { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Đơn giá phải lớn hơn hoặc bằng 0")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Đơn giá/ngày")]
    public decimal DonGiaNgay { get; set; }

    [StringLength(500)]
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [Display(Name = "Hoạt động")]
    public bool TrangThai { get; set; } = true;
    public ICollection<Phong> phongs { get; set; }
}