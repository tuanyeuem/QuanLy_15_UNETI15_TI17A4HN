// Họ và tên: Nguyễn Đình Tuấn
// Mã sinh viên: 23103100235
// Nội dung thực hiện: Entity chi tiết kỳ lưu trú, phòng, đơn giá và thành tiền.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QuanLy_15_UNETI15_TI17A4HN.Models;
public static class TrangThaiChiTietDatPhong
{
    public const string DaDat = "DaDat";
    public const string DangSuDung = "DangSuDung";
    public const string DaTraPhong = "DaTraPhong";
    public const string Huy = "Huy";
}
public class ChiTietDatPhong
{
    [Key] public int MaChiTiet { get; set; }
    [Required] public int MaDatPhong { get; set; }
    public DatPhong? DatPhong { get; set; }
    [Required] public int MaPhong { get; set; }
    public Phong? Phong { get; set; }
    [Required, DataType(DataType.DateTime)] public DateTime NgayNhan { get; set; }
    [Required, DataType(DataType.DateTime)] public DateTime NgayTra { get; set; }
    [Range(typeof(decimal),"0","9999999999999999.99"), Column(TypeName="decimal(18,2)")]
    public decimal DonGia { get; set; }
    [Range(1,3650)] public int SoNgay { get; set; }
    [Range(typeof(decimal),"0","9999999999999999.99"), Column(TypeName="decimal(18,2)")]
    public decimal ThanhTien { get; set; }
    [Required, StringLength(20), RegularExpression("^(DaDat|DangSuDung|DaTraPhong|Huy)$")]
    public string TrangThai { get; set; } = TrangThaiChiTietDatPhong.DaDat;
    public ICollection<SuDungDichVu> SuDungDichVus { get; set; } = new List<SuDungDichVu>();
}
