// Họ và tên: Nguyễn Đình Tuấn
// Mã sinh viên: 23103100235
// Nội dung thực hiện: Entity dữ liệu nền dịch vụ khách sạn.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QuanLy_15_UNETI15_TI17A4HN.Models;
public class DichVu
{
    [Key] public int MaDichVu { get; set; }
    [Required, StringLength(100), Display(Name="Tên dịch vụ")]
    public string TenDichVu { get; set; } = string.Empty;
    [Required, StringLength(30), Display(Name="Đơn vị tính")]
    public string DonViTinh { get; set; } = "lần";
    [Range(typeof(decimal),"0","9999999999999999.99"), Column(TypeName="decimal(18,2)"), Display(Name="Đơn giá")]
    public decimal DonGia { get; set; }
    [StringLength(500)] public string? MoTa { get; set; }
    public bool TrangThai { get; set; } = true;
    public ICollection<SuDungDichVu> SuDungDichVus { get; set; } = new List<SuDungDichVu>();
}
