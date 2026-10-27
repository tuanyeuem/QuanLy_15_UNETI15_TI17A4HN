// Họ và tên: Lê Văn Thanh
// Mã sinh viên: 23103100200
// Nội dung thực hiện: Module 5 - Chọn màu badge hiển thị theo trạng thái nghiệp vụ.
using QuanLy_15_UNETI15_TI17A4HN.Models;

namespace QuanLy_15_UNETI15_TI17A4HN.Helpers;

public static class TrangThaiUi
{
    public static string Badge(string? trangThai) => trangThai switch
    {
        TrangThaiDatPhong.ChoXuLy or TrangThaiChiTiet.ChuaNhan => "bg-warning text-dark",
        TrangThaiDatPhong.DangXuLy or TrangThaiDatPhong.DaNhanPhong or TrangThaiChiTiet.DangO => "bg-primary",
        TrangThaiDatPhong.HoanThanh or TrangThaiChiTiet.DaTra => "bg-success",
        TrangThaiDatPhong.DaHuy or TrangThaiDatPhong.TuChoi => "bg-secondary",
        _ => "bg-light text-dark"
    };

    public static string Tien(decimal soTien) => soTien.ToString("N0") + " đ";

    // Nhãn tiếng Việt cho trạng thái phòng (nhận cả giá trị mã và giá trị tiếng Việt cũ).
    public static string NhanPhong(string? trangThai) => (trangThai ?? "").Trim() switch
    {
        "TrongRanh" or "Trong" or "SanSang" or "Trống" or "Sẵn sàng" => "Còn trống",
        "DaDat" or "Đã đặt" => "Đã đặt",
        "DangSuDung" or "Đang sử dụng" => "Đang sử dụng",
        "BaoTri" or "Bảo trì" => "Bảo trì",
        "KhongKinhDoanh" or "NgungKinhDoanh" or "Không kinh doanh" => "Ngừng kinh doanh",
        "" => "Chưa rõ",
        var khac => khac
    };

    // Lớp CSS tương ứng (xem .tt-* trong wwwroot/css/site.css).
    public static string LopPhong(string? trangThai) => NhanPhong(trangThai) switch
    {
        "Còn trống" => "tt-ok",
        "Đã đặt" or "Đang sử dụng" => "tt-info",
        "Bảo trì" => "tt-warn",
        _ => "tt-off"
    };

    // Tách chuỗi tiện nghi (ngăn cách bởi dấu phẩy hoặc chấm phẩy) thành tối đa N mục.
    public static string[] TienNghi(string? tienNghi, int toiDa = 4)
    {
        if (string.IsNullOrWhiteSpace(tienNghi)) return Array.Empty<string>();
        return tienNghi
            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Take(toiDa)
            .ToArray();
    }
}
