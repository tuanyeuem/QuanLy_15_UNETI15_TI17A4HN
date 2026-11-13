// Họ và tên: Nguyễn Đình Quang
// Mã sinh viên: 23103100259
// Nội dung thực hiện: Đồng bộ tên trạng thái đặt phòng giữa Module 3, 4 và 5.
namespace QuanLy_15_UNETI15_TI17A4HN.Models;

public static class TrangThaiDatPhong
{
    public const string ChoDuyet = "ChoDuyet";
    public const string DaDuyet = "DaDuyet";
    public const string DaNhanPhong = "DaNhanPhong";
    public const string DaTraPhong = "DaTraPhong";
    public const string Huy = "Huy";
    public const string TuChoi = "TuChoi";

    // Bí danh giữ cho các màn hình Module 5 tương thích với luồng Module 4.
    public const string ChoXuLy = ChoDuyet;
    public const string DangXuLy = DaDuyet;
    public const string HoanThanh = DaTraPhong;
    public const string DaHuy = Huy;

    // Tên hiển thị theo đề bài. Mã lưu trong CSDL không đổi nên không cần migration.
    public static string Ten(string? maTrangThai) => maTrangThai switch
    {
        ChoDuyet => "Chờ xử lý",
        DaDuyet => "Đang xử lý (đã duyệt)",
        DaNhanPhong => "Đang xử lý (đang lưu trú)",
        DaTraPhong => "Hoàn thành",
        Huy => "Đã hủy",
        TuChoi => "Từ chối",
        _ => maTrangThai ?? string.Empty
    };
}
