// Họ và tên: Lê Văn Thanh
// Mã sinh viên: 23103100200
// Nội dung thực hiện: Module 5 - Entity ChiTietDatPhong (kết quả/chi tiết đặt phòng, tính tiền phòng).
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLy_15_UNETI15_TI17A4HN.Models;

// Trạng thái của từng phòng trong một lần đặt.
public static class TrangThaiChiTiet
{
    public const string ChuaNhan = "Chưa nhận";
    public const string DangO = "Đang ở";
    public const string DaTra = "Đã trả";
    public const string DaHuy = "Đã hủy";

    public static readonly string[] TatCa = { ChuaNhan, DangO, DaTra, DaHuy };
}

public class ChiTietDatPhong
{
    [Key]
    public int MaChiTiet { get; set; }

    [Display(Name = "Mã đặt phòng")]
    public int MaDatPhong { get; set; }
    public DatPhong DatPhong { get; set; } = null!;

    [Display(Name = "Phòng")]
    public int MaPhong { get; set; }
    public Phong Phong { get; set; } = null!;

    [DataType(DataType.Date)]
    [Display(Name = "Ngày nhận")]
    public DateTime NgayNhan { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày trả")]
    public DateTime NgayTra { get; set; }

    [Range(typeof(decimal), "0", "9999999999999999.99", ErrorMessage = "Đơn giá phải lớn hơn hoặc bằng 0")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Đơn giá")]
    public decimal DonGia { get; set; }

    [Range(0, 3650, ErrorMessage = "Số ngày không hợp lệ")]
    [Display(Name = "Số ngày")]
    public int SoNgay { get; set; }

    [Range(typeof(decimal), "0", "9999999999999999.99", ErrorMessage = "Thành tiền phải lớn hơn hoặc bằng 0")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Thành tiền")]
    public decimal ThanhTien { get; set; }

    [Required]
    [StringLength(20)]
    [Display(Name = "Trạng thái")]
    public string TrangThai { get; set; } = TrangThaiChiTiet.ChuaNhan;

    // Do hệ thống gán khi nhân viên chốt kết quả; người dùng không được tự sửa.
    [Display(Name = "Cập nhật lúc")]
    public DateTime? NgayCapNhat { get; set; }
}
