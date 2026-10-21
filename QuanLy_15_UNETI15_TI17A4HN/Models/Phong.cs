using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLy_15_UNETI15_TI17A4HN.Models;

public static class TrangThaiPhong
{
    public const string SanSang = "Sẵn sàng";
    public const string DangSuDung = "Đang sử dụng";
    public const string BaoTri = "Bảo trì";
    public const string KhongKinhDoanh = "Không kinh doanh";
    public static readonly string[] TatCa = { SanSang, DangSuDung, BaoTri, KhongKinhDoanh };
}

public class Phong
{
    [Key]
    public int MaPhong { get; set; }

    [Required, StringLength(10)]
    [Display(Name = "Số phòng")]
    public string SoPhong { get; set; } = string.Empty;

    [Required, Display(Name = "Loại phòng")]
    public int MaLoaiPhong { get; set; }
    public LoaiPhong LoaiPhong { get; set; } = null!;

    [Range(1, 100), Display(Name = "Tầng")]
    public int Tang { get; set; }

    [StringLength(50), Display(Name = "Hướng phòng")]
    public string? HuongPhong { get; set; }

    [StringLength(500), Display(Name = "Tiện nghi")]
    public string? TienNghi { get; set; }

    [Range(0, 1_000_000_000)]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Đơn giá/ngày")]
    public decimal DonGia { get; set; }

    [Required, StringLength(30), Display(Name = "Trạng thái")]
    public string TrangThai { get; set; } = TrangThaiPhong.SanSang;

    [StringLength(500), Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    [DataType(DataType.Date), Display(Name = "Ngày bảo trì")]
    public DateTime? NgayBaoTri { get; set; }

    [StringLength(500), Display(Name = "Ghi chú bảo trì")]
    public string? GhiChuBaoTri { get; set; }

    public ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();
    public ICollection<ChiTietDatPhong> ChiTietDatPhongs { get; set; } = new List<ChiTietDatPhong>();
}
