// Họ và tên: Nguyễn Đình Tuấn
// Mã sinh viên: 23103100235
// Nội dung thực hiện: Entity phòng khách sạn và trạng thái kinh doanh.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QuanLy_15_UNETI15_TI17A4HN.Models;
public static class TrangThaiPhong
{
    public const string Trong = "Trong";
    public const string DaDat = "DaDat";
    public const string DangSuDung = "DangSuDung";
    public const string BaoTri = "BaoTri";
    public const string NgungKinhDoanh = "NgungKinhDoanh";
}
public class Phong
{
    [Key] public int MaPhong { get; set; }
    [Required, StringLength(20), Display(Name="Số phòng")]
    public string SoPhong { get; set; } = string.Empty;
    [Required, Display(Name="Loại phòng")]
    public int MaLoaiPhong { get; set; }
    public LoaiPhong? LoaiPhong { get; set; }
    [Range(1,100)] public int Tang { get; set; }
    [StringLength(50), Display(Name="Hướng phòng")]
    public string? HuongPhong { get; set; }
    [StringLength(1000), Display(Name="Tiện nghi")]
    public string? TienNghi { get; set; }
    [Range(typeof(decimal),"0","9999999999999999.99"), Column(TypeName="decimal(18,2)"), Display(Name="Đơn giá")]
    public decimal DonGia { get; set; }
    [Required, StringLength(20), RegularExpression("^(Trong|DaDat|DangSuDung|BaoTri|NgungKinhDoanh)$")]
    public string TrangThai { get; set; } = TrangThaiPhong.Trong;
    [StringLength(500)] public string? GhiChu { get; set; }
    [DataType(DataType.Date), Display(Name="Ngày bảo trì")]
    public DateTime? NgayBaoTri { get; set; }
    [StringLength(500), Display(Name="Ghi chú bảo trì")]
    public string? GhiChuBaoTri { get; set; }
    public ICollection<ChiTietDatPhong> ChiTietDatPhongs { get; set; } = new List<ChiTietDatPhong>();
}
