using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLy_15_UNETI15_TI17A4HN.Models;

public static class TrangThaiPhong
{
    public const string SanSang = "S?n s?ng";
    public const string DangSuDung = "?ang s? d?ng";
    public const string BaoTri = "B?o tr?";
    public const string KhongKinhDoanh = "Kh?ng kinh doanh";
    public static readonly string[] TatCa = { SanSang, DangSuDung, BaoTri, KhongKinhDoanh };
}

public class Phong
{
    [Key]
    public int MaPhong { get; set; }

    [Required, StringLength(10)]
    [Display(Name = "S? ph?ng")]
    public string SoPhong { get; set; } = string.Empty;

    [Required, Display(Name = "Lo?i ph?ng")]
    public int MaLoaiPhong { get; set; }
    public LoaiPhong LoaiPhong { get; set; } = null!;

    [Range(1, 100), Display(Name = "T?ng")]
    public int Tang { get; set; }

    [StringLength(50), Display(Name = "H??ng ph?ng")]
    public string? HuongPhong { get; set; }

    [StringLength(500), Display(Name = "Ti?n nghi")]
    public string? TienNghi { get; set; }

    [Range(0, 1_000_000_000)]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "??n gi?/ng?y")]
    public decimal DonGia { get; set; }

    [Required, StringLength(30), Display(Name = "Tr?ng th?i")]
    public string TrangThai { get; set; } = TrangThaiPhong.SanSang;

    [StringLength(500), Display(Name = "Ghi ch?")]
    public string? GhiChu { get; set; }

    [DataType(DataType.Date), Display(Name = "Ng?y b?o tr?")]
    public DateTime? NgayBaoTri { get; set; }

    [StringLength(500), Display(Name = "Ghi ch? b?o tr?")]
    public string? GhiChuBaoTri { get; set; }

    public ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();
    public ICollection<ChiTietDatPhong> ChiTietDatPhongs { get; set; } = new List<ChiTietDatPhong>();
}
