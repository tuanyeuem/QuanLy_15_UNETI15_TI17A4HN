// Họ và tên: Lê Văn Thanh
// Mã sinh viên: 23103100200
// Nội dung thực hiện: Module 5 - ViewModel danh sách, form và chốt kết quả chi tiết đặt phòng.
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLy_15_UNETI15_TI17A4HN.Helpers;
using QuanLy_15_UNETI15_TI17A4HN.Models;

namespace QuanLy_15_UNETI15_TI17A4HN.ViewModels;

// Danh sách + tìm kiếm + lọc + sắp xếp + phân trang.
public class ChiTietDatPhongIndexViewModel
{
    public string? TuKhoa { get; set; }
    public string? TrangThai { get; set; }
    public int? MaLoaiPhong { get; set; }
    public DateTime? TuNgay { get; set; }
    public DateTime? DenNgay { get; set; }
    public string SapXep { get; set; } = "ngay_giam";

    public DanhSachPhanTrang<ChiTietDatPhong> KetQua { get; set; } = null!;
    public List<SelectListItem> LoaiPhongs { get; set; } = new();
}

// Form thêm/sửa chi tiết. Đơn giá, số ngày, thành tiền do hệ thống tính nên không có trong form.
public class ChiTietDatPhongFormViewModel : IValidatableObject
{
    public int MaChiTiet { get; set; }

    public int MaDatPhong { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn phòng")]
    [Display(Name = "Phòng")]
    public int? MaPhong { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn ngày nhận")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày nhận")]
    public DateTime? NgayNhan { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn ngày trả")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày trả")]
    public DateTime? NgayTra { get; set; }

    // Chỉ để hiển thị.
    public DatPhong? DatPhong { get; set; }
    public List<SelectListItem> Phongs { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (NgayNhan.HasValue && NgayTra.HasValue && NgayTra.Value.Date <= NgayNhan.Value.Date)
            yield return new ValidationResult("Ngày trả phải sau ngày nhận.", new[] { nameof(NgayTra) });
    }
}

public class DongTinhTien
{
    public ChiTietDatPhong ChiTiet { get; set; } = null!;
    public DateTime NgayTra { get; set; }
    public int SoNgay { get; set; }
    public decimal ThanhTien { get; set; }
}

// Màn hình tính tiền/chốt kết quả cho một lần đặt phòng.
public class ChotKetQuaViewModel
{
    public DatPhong DatPhong { get; set; } = null!;
    public List<DongTinhTien> Dong { get; set; } = new();

    [DataType(DataType.Date)]
    [Display(Name = "Ngày trả thực tế")]
    public DateTime NgayTra { get; set; }

    public decimal TongTien => Dong.Sum(d => d.ThanhTien);
    public decimal TienCoc => DatPhong.TienCoc;
    public decimal ConPhaiThu => TongTien - TienCoc;

    public bool DuDieuKien { get; set; }
    public string? LyDoKhongDuDieuKien { get; set; }
}

// Xem kết quả của một lần đặt phòng (cho khách và nhân viên).
public class KetQuaDatPhongViewModel
{
    public DatPhong DatPhong { get; set; } = null!;
    public List<ChiTietDatPhong> ChiTiets { get; set; } = new();

    public decimal TongTien => ChiTiets.Where(c => c.TrangThai != TrangThaiChiTiet.DaHuy).Sum(c => c.ThanhTien);
    public decimal ConPhaiThu => TongTien - DatPhong.TienCoc;
    public bool DaHoanTat => DatPhong.TrangThai == TrangThaiDatPhong.HoanThanh;
}

public class KetQuaCuaToiItem
{
    public DatPhong DatPhong { get; set; } = null!;
    public int SoPhong { get; set; }
    public decimal TongTien { get; set; }
}
