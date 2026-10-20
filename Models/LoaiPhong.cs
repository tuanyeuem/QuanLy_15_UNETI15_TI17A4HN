using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLy_15_UNETI15_TI17A4HN.Models;

public class LoaiPhong
{
    [Key]
    public int MaLoaiPhong { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "T?n lo?i ph?ng")]
    public string TenLoai { get; set; } = string.Empty;

    [Range(1, 20)]
    [Display(Name = "S? ng??i t?i ?a")]
    public int SoNguoiToiDa { get; set; }

    [Range(0, double.MaxValue)]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "??n gi?/ng?y")]
    public decimal DonGiaNgay { get; set; }

    [StringLength(500)]
    [Display(Name = "M? t?")]
    public string? MoTa { get; set; }

    [Display(Name = "Ho?t ??ng")]
    public bool TrangThai { get; set; } = true;
}
