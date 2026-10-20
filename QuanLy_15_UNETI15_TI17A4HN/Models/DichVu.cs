// Họ và tên: Lê Văn Thanh
// Mã sinh viên: 23103100200
// Nội dung thực hiện: Module 5 - Entity DichVu (nền tảng) phục vụ dữ liệu mẫu và thống kê.
// Entity nền tảng (đề yêu cầu có Entity DichVu trong thiết kế CSDL).
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLy_15_UNETI15_TI17A4HN.Models;

public class DichVu
{
    [Key]
    public int MaDichVu { get; set; }

    [Required(ErrorMessage = "Tên dịch vụ là bắt buộc")]
    [StringLength(100)]
    [Display(Name = "Tên dịch vụ")]
    public string TenDichVu { get; set; } = string.Empty;

    [StringLength(30)]
    [Display(Name = "Đơn vị tính")]
    public string? DonViTinh { get; set; }

    [Range(0, 1_000_000_000, ErrorMessage = "Đơn giá phải lớn hơn hoặc bằng 0")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Đơn giá")]
    public decimal DonGia { get; set; }

    [Display(Name = "Hoạt động")]
    public bool TrangThai { get; set; } = true;
}
