// Họ và tên: Nguyễn Đình Tuấn | Mã sinh viên: 23103100235 | Nội dung: Entity dữ liệu nền dịch vụ.
// Họ và tên: Lê Văn Thanh | Mã sinh viên: 23103100200 | Nội dung: Entity nền phục vụ dữ liệu mẫu và thống kê.
// Họ và tên: Nguyễn Đình Quang | Mã sinh viên: 23103100259 | Nội dung: Hợp nhất entity và navigation sử dụng dịch vụ.
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

    [Required]
    [StringLength(30)]
    [Display(Name = "Đơn vị tính")]
    public string DonViTinh { get; set; } = "lần";

    [Range(typeof(decimal), "0", "9999999999999999.99")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Đơn giá")]
    public decimal DonGia { get; set; }

    [StringLength(500)]
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [Display(Name = "Hoạt động")]
    public bool TrangThai { get; set; } = true;

    public ICollection<SuDungDichVu> SuDungDichVus { get; set; } = new List<SuDungDichVu>();
}
