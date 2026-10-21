// Họ và tên: Nguyễn Đình Quang
// Mã sinh viên: 23103100259
// Nội dung thực hiện: Hợp nhất entity chi tiết lưu trú, giá phòng và trạng thái sử dụng.
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


public static class TrangThaiChiTiet
{
    public const string ChuaNhan = TrangThaiChiTietDatPhong.DaDat;
    public const string DangO = TrangThaiChiTietDatPhong.DangSuDung;
    public const string DaTra = TrangThaiChiTietDatPhong.DaTraPhong;
    public const string DaHuy = TrangThaiChiTietDatPhong.Huy;
    public static readonly string[] TatCa = { ChuaNhan, DangO, DaTra, DaHuy };
}

public class ChiTietDatPhong
{
    [Key]
    public int MaChiTiet { get; set; }

    [Required]
    [Display(Name = "Mã đặt phòng")]
    public int MaDatPhong { get; set; }
    public DatPhong DatPhong { get; set; } = null!;

    [Required]
    [Display(Name = "Mã phòng")]
    public int MaPhong { get; set; }
    public Phong Phong { get; set; } = null!;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày nhận")]
    public DateTime NgayNhan { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày trả")]
    public DateTime NgayTra { get; set; }

    [Range(typeof(decimal), "0", "9999999999999999.99")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Đơn giá")]
    public decimal DonGia { get; set; }

    [Range(0, 3650)]
    [Display(Name = "Số ngày")]
    public int SoNgay { get; set; }

    [Range(typeof(decimal), "0", "9999999999999999.99")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Thành tiền")]
    public decimal ThanhTien { get; set; }

    [Required]
    [StringLength(20)]
    [Display(Name = "Trạng thái")]
    public string TrangThai { get; set; } = TrangThaiChiTietDatPhong.DaDat;

    [Display(Name = "Cập nhật lúc")]
    public DateTime? NgayCapNhat { get; set; }

    public ICollection<SuDungDichVu> SuDungDichVus { get; set; } = new List<SuDungDichVu>();
}
