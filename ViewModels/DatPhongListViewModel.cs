// Họ và tên: Vũ Xuân Tuấn
// Mã sinh viên: 23103100035
// Nội dung thực hiện: ViewModel danh sách đặt phòng, tìm kiếm, lọc dữ liệu nền (Loại phòng), trạng thái, khoảng ngày, sắp xếp tăng/giảm, phân trang.
using QuanLy_15_UNETI15_TI17A4HN.Models;

namespace QuanLy_15_UNETI15_TI17A4HN.ViewModels;

public class DatPhongListViewModel
{
    public List<DatPhong> DanhSach { get; set; } = new();

    // Bộ lọc
    public string? TimKiem      { get; set; }
    public string? LocTrangThai { get; set; }
    public int? MaLoaiPhong     { get; set; }    // Lọc theo dữ liệu nền (Loại phòng)
    public DateTime? TuNgay     { get; set; }
    public DateTime? DenNgay    { get; set; }
    public string SapXep        { get; set; } = "ngaydat_moi";

    // Phân trang
    public int TrangHienTai   { get; set; } = 1;
    public int TongSoTrang    { get; set; }
    public int SoTrangHienThi { get; set; } = 10;

    // Dropdown dữ liệu nền (Loại phòng)
    public List<LoaiPhong> DanhSachLoaiPhong { get; set; } = new();

    // Dropdown trạng thái
    public List<string> DanhSachTrangThai { get; set; } = new()
    {
        TrangThaiDatPhong.ChoDuyet,
        TrangThaiDatPhong.DaDuyet,
        TrangThaiDatPhong.DaNhanPhong,
        TrangThaiDatPhong.DaTraPhong,
        TrangThaiDatPhong.Huy,
    };
}