// Họ và tên: Lê Văn Thanh
// Mã sinh viên: 23103100200
// Nội dung thực hiện: Module 5 - ViewModel Dashboard, thống kê LINQ và báo cáo doanh thu.
using QuanLy_15_UNETI15_TI17A4HN.Models;

namespace QuanLy_15_UNETI15_TI17A4HN.ViewModels;

public class DashboardViewModel
{
    public int TongLoaiPhong { get; set; }
    public int TongPhong { get; set; }
    public int PhongKhaDung { get; set; }
    public int TongKhachHang { get; set; }
    public int TongGiaoDich { get; set; }
    public int ChoXuLy { get; set; }
    public int DangXuLy { get; set; }
    public int HoanThanh { get; set; }
    public int DaHuy { get; set; }
    public decimal TongDoanhThu { get; set; }
    public List<DatPhong> GanDay { get; set; } = new();
}

public class SoLuongTheoTen
{
    public string Ten { get; set; } = string.Empty;
    public int SoLuong { get; set; }
}

public class TyLeSuDungLoaiPhong
{
    public string TenLoai { get; set; } = string.Empty;
    public int TongPhong { get; set; }
    public int DangSuDung { get; set; }
    public double TyLe => TongPhong == 0 ? 0 : Math.Round(DangSuDung * 100.0 / TongPhong, 1);
}

public class GiaTriTheoThang
{
    public int Thang { get; set; }
    public int SoLuot { get; set; }
    public decimal DoanhThu { get; set; }
}

public class DoanhThuTheoLoai
{
    public string TenLoai { get; set; } = string.Empty;
    public int SoLuot { get; set; }
    public decimal DoanhThu { get; set; }
}

public class PhongDatNhieu
{
    public string SoPhong { get; set; } = string.Empty;
    public string TenLoai { get; set; } = string.Empty;
    public int SoLuot { get; set; }
    public decimal TongTien { get; set; }
}

public class ThongKeViewModel
{
    public int Nam { get; set; }
    public List<int> CacNam { get; set; } = new();

    public List<SoLuongTheoTen> SoPhongTheoLoai { get; set; } = new();
    public List<TyLeSuDungLoaiPhong> TyLeSuDung { get; set; } = new();
    public int PhongDangSuDung { get; set; }
    public int PhongKinhDoanh { get; set; }
    public double TyLeSuDungChung => PhongKinhDoanh == 0 ? 0 : Math.Round(PhongDangSuDung * 100.0 / PhongKinhDoanh, 1);

    public List<GiaTriTheoThang> TheoThang { get; set; } = new();
    public List<DoanhThuTheoLoai> DoanhThuTheoLoai { get; set; } = new();
    public List<PhongDatNhieu> PhongDatNhieuNhat { get; set; } = new();

    public bool CoDuLieu { get; set; }
    public int SoLuotHuyTrongNam { get; set; }
    public int TongLuotDatTrongNam { get; set; }
    public decimal TongDoanhThuTrongNam { get; set; }
}

public class BaoCaoViewModel
{
    public DateTime TuNgay { get; set; }
    public DateTime DenNgay { get; set; }
    public List<ChiTietDatPhong> Dong { get; set; } = new();
    public List<DoanhThuTheoLoai> TheoLoai { get; set; } = new();
    public decimal TongDoanhThu => Dong.Sum(d => d.ThanhTien);
    public int TongSoDem => Dong.Sum(d => d.SoNgay);
}
