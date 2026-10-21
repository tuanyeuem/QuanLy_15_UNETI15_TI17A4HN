// Họ và tên: Nguyễn Đình Tuấn
// Mã sinh viên: 23103100235
// Nội dung thực hiện: Entity phát sinh dịch vụ theo chi tiết lưu trú và lưu giá tại thời điểm dùng.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QuanLy_15_UNETI15_TI17A4HN.Models;
public class SuDungDichVu
{
    [Key] public int MaSuDungDichVu { get; set; }
    [Required] public int MaChiTiet { get; set; }
    public ChiTietDatPhong? ChiTietDatPhong { get; set; }
    [Required] public int MaDichVu { get; set; }
    public DichVu? DichVu { get; set; }
    [Range(1,10000)] public int SoLuong { get; set; }
    [Range(typeof(decimal),"0","9999999999999999.99"), Column(TypeName="decimal(18,2)")]
    public decimal DonGia { get; set; }
    [Range(typeof(decimal),"0","9999999999999999.99"), Column(TypeName="decimal(18,2)")]
    public decimal ThanhTien { get; set; }
    public DateTime NgaySuDung { get; set; } = DateTime.UtcNow;
    [StringLength(500)] public string? GhiChu { get; set; }
}
