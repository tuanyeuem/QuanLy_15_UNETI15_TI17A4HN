// Họ và tên: Nguyễn Đình Tuấn
// Mã sinh viên: 23103100235
// Nội dung thực hiện: Entity giao dịch đặt phòng, người đặt, nhân viên tiếp nhận và trạng thái.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QuanLy_15_UNETI15_TI17A4HN.Models;
public static class TrangThaiDatPhong
{
    public const string ChoXuLy = "ChoXuLy";
    public const string DangXuLy = "DangXuLy";
    public const string HoanThanh = "HoanThanh";
    public const string Huy = "Huy";
    public const string TuChoi = "TuChoi";
}
public class DatPhong
{
    [Key] public int MaDatPhong { get; set; }
    [Required] public int MaKhachHang { get; set; }
    public KhachHang? KhachHang { get; set; }
    public int? MaNhanVienTiepNhan { get; set; }
    [ForeignKey(nameof(MaNhanVienTiepNhan))]
    public TaiKhoan? NhanVienTiepNhan { get; set; }
    [DataType(DataType.DateTime), Display(Name="Ngày đặt")]
    public DateTime NgayDat { get; set; } = DateTime.UtcNow;
    [Required, DataType(DataType.DateTime), Display(Name="Ngày nhận phòng")]
    public DateTime NgayNhan { get; set; }
    [Required, DataType(DataType.DateTime), Display(Name="Ngày trả dự kiến")]
    public DateTime NgayTraDuKien { get; set; }
    [Range(1,100), Display(Name="Số người")]
    public int SoNguoi { get; set; }
    [Range(typeof(decimal),"0","9999999999999999.99"), Column(TypeName="decimal(18,2)"), Display(Name="Tiền cọc")]
    public decimal TienCoc { get; set; }
    [Required, StringLength(20), RegularExpression("^(ChoXuLy|DangXuLy|HoanThanh|Huy|TuChoi)$")]
    public string TrangThai { get; set; } = TrangThaiDatPhong.ChoXuLy;
    [StringLength(1000)] public string? GhiChu { get; set; }
    public ICollection<ChiTietDatPhong> ChiTietDatPhongs { get; set; } = new List<ChiTietDatPhong>();
}
