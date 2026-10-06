// Họ và tên: Lê Văn Thanh
// Mã sinh viên: 23103100200
// Nội dung thực hiện: Module 5 - Entity DatPhong (nền tảng) liên kết khóa ngoại với ChiTietDatPhong.
// Entity nền tảng (khung tối thiểu theo đề) để Module 5 liên kết khóa ngoại.
// Module 3 (tạo đặt phòng) và Module 4 (tiếp nhận, nhận/trả phòng, luồng trạng thái) sẽ hoàn thiện nghiệp vụ.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLy_15_UNETI15_TI17A4HN.Models;

// Luồng: Chờ xử lý -> Đang xử lý -> Hoàn thành; có thể Đã hủy khi còn Chờ xử lý.
public static class TrangThaiDatPhong
{
    public const string ChoXuLy = "Chờ xử lý";
    public const string DangXuLy = "Đang xử lý";
    public const string HoanThanh = "Hoàn thành";
    public const string DaHuy = "Đã hủy";

    public static readonly string[] TatCa = { ChoXuLy, DangXuLy, HoanThanh, DaHuy };
}

public class DatPhong
{
    [Key]
    public int MaDatPhong { get; set; }

    [Display(Name = "Khách hàng")]
    public int MaKhachHang { get; set; }
    public KhachHang KhachHang { get; set; } = null!;

    [Display(Name = "Ngày đặt")]
    public DateTime NgayDat { get; set; } = DateTime.Now;

    [DataType(DataType.Date)]
    [Display(Name = "Ngày nhận")]
    public DateTime NgayNhan { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày trả dự kiến")]
    public DateTime NgayTraDuKien { get; set; }

    [Range(1, 50, ErrorMessage = "Số người phải lớn hơn 0")]
    [Display(Name = "Số người")]
    public int SoNguoi { get; set; } = 1;

    [Range(0, 1_000_000_000, ErrorMessage = "Tiền cọc phải lớn hơn hoặc bằng 0")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Tiền cọc")]
    public decimal TienCoc { get; set; }

    [Required]
    [StringLength(30)]
    [Display(Name = "Trạng thái")]
    public string TrangThai { get; set; } = TrangThaiDatPhong.ChoXuLy;

    [StringLength(500)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    public ICollection<ChiTietDatPhong> ChiTietDatPhongs { get; set; } = new List<ChiTietDatPhong>();
}
